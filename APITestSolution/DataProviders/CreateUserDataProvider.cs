
using ApiAutomationFramework.Models.Request;
using ApiAutomationFramework.Models.Request.Users;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace APITestSolution.DataProviders
{
    public static class UserDataProvider
    {
        // Fixed email domain for all randomized emails
        private const string DefaultEmailDomain = "zensar.com";

        // =====================================================
        // 🔧 RANDOM HELPERS (safe for parallel test runs)
        // =====================================================
        private static class RandomData
        {
            private const string Letters = "abcdefghijklmnopqrstuvwxyz";

            public static string LettersOnly(int length = 5)
            {
                if (length <= 0) return string.Empty;
                var bytes = new byte[length];
                RandomNumberGenerator.Fill(bytes);
                var sb = new StringBuilder(length);
                foreach (var b in bytes)
                    sb.Append(Letters[b % Letters.Length]);
                return sb.ToString();
            }

            // "Auto_" + 5 letters
            public static string AutoTag(int letters = 5) => "Auto_" + LettersOnly(letters);

            // Email with fixed domain and random local-part "Auto_xxxxx"
            public static string Email(string domain) => $"{AutoTag(5)}@{domain}";

            // Random future date yyyy-MM-dd (min..max days from now)
            public static string FutureDate(int minDays = 7, int maxDays = 365)
            {
                if (minDays < 0) minDays = 0;
                if (maxDays < minDays) maxDays = minDays;
                var days = RandomNumberGenerator.GetInt32(minDays, maxDays + 1);
                return DateTime.UtcNow.AddDays(days).ToString("yyyy-MM-dd");
            }

            // Random recent past date yyyy-MM-dd
            public static string PastDate(int minDaysAgo = 1, int maxDaysAgo = 30)
            {
                if (minDaysAgo < 0) minDaysAgo = 0;
                if (maxDaysAgo < minDaysAgo) maxDaysAgo = minDaysAgo;
                var days = RandomNumberGenerator.GetInt32(minDaysAgo, maxDaysAgo + 1);
                return DateTime.UtcNow.AddDays(-days).ToString("yyyy-MM-dd");
            }

            // Digits-only string (e.g., mobiles as strings)
            public static string Digits(int length = 10)
            {
                if (length <= 0) return string.Empty;
                var sb = new StringBuilder(length);
                for (int i = 0; i < length; i++)
                    sb.Append(RandomNumberGenerator.GetInt32(0, 10));
                return sb.ToString();
            }
        }

        // =====================================================
        // 🧠 GENERIC AUTO-FILL (works for ANY module DTO)
        // =====================================================
        /// <summary>
        /// Options for auto-filling the DTO.
        /// </summary>
        public sealed class FillOptions
        {
            public string EmailDomain { get; init; } = DefaultEmailDomain;

            /// <summary>
            /// If true, only fills when current string is null/empty/whitespace.
            /// Non-empty strings you set are preserved. (Empty strings are considered "empty".)
            /// </summary>
            public bool OnlyFillWhenEmpty { get; init; } = true;

            /// <summary>Letters count for Auto_ prefix.</summary>
            public int AutoLetters { get; init; } = 5;

            /// <summary>
            /// Property names to skip from auto-fill (case-insensitive).
            /// Use this to keep intentionally invalid values like empty strings.
            /// </summary>
            public HashSet<string> ExcludeProperties { get; } = new(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Creates and auto-fills a DTO of type T:
        /// - Seed numerics/fixed values inside <paramref name="seed"/>.
        /// - All string properties randomized per rules (no override of non-empty strings).
        /// - Email strings honor <paramref name="options.EmailDomain"/>.
        /// - Date strings auto-generated based on property name.
        /// - Nested objects / lists are filled recursively.
        /// </summary>
        public static T Prepare<T>(Action<T>? seed = null, FillOptions? options = null)
            where T : new()
        {
            options ??= new FillOptions();
            var dto = new T();

            // 1) Caller seeds known ints, lists, or specific string overrides.
            seed?.Invoke(dto);

            // 2) Auto-fill strings recursively (does not touch numerics).
            FillStringsRecursively(dto!, options);

            return dto!;
        }

        private static void FillStringsRecursively(object obj, FillOptions options)
        {
            if (obj == null) return;

            // Lists/arrays
            if (obj is IList list)
            {
                foreach (var item in list)
                    FillStringsRecursively(item, options);
                return;
            }

            // Dictionaries (rare in DTOs)
            if (obj is IDictionary dict)
            {
                foreach (DictionaryEntry entry in dict)
                    FillStringsRecursively(entry.Value, options);
                return;
            }

            var type = obj.GetType();
            foreach (var p in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!p.CanWrite) continue;

                var propType = p.PropertyType;
                var name = p.Name;
                var lower = name.ToLowerInvariant();

                // ---- Skip excluded properties
                if (options.ExcludeProperties.Contains(name))
                    continue;

                // ---- Strings
                if (propType == typeof(string))
                {
                    var current = p.GetValue(obj) as string;
                    var isEmpty = string.IsNullOrWhiteSpace(current);

                    if (!isEmpty && options.OnlyFillWhenEmpty)
                        continue;

                    string value;
                    if (lower.Contains("email"))
                        value = RandomData.Email(options.EmailDomain);
                    else if (lower.Contains("mobile") || lower.Contains("telephone") || lower.Contains("phone"))
                        value = RandomData.Digits(10);
                    else if (lower.Contains("expir") && lower.Contains("date"))
                        value = RandomData.FutureDate(15, 365);
                    else if (lower.Contains("date"))
                        value = RandomData.PastDate(1, 30);
                    else
                        value = "Auto_" + RandomData.LettersOnly(options.AutoLetters);

                    p.SetValue(obj, value);
                    continue;
                }

                // ---- DateTime properties (if DTO uses DateTime)
                if (propType == typeof(DateTime) || propType == typeof(DateTime?))
                {
                    var isNullable = propType == typeof(DateTime?);
                    var current = p.GetValue(obj);

                    var shouldFill = true;
                    if (options.OnlyFillWhenEmpty)
                    {
                        shouldFill = current == null || (current is DateTime dt && dt == default);
                    }

                    if (shouldFill)
                    {
                        var dt = lower.Contains("expir") ? DateTime.UtcNow.AddDays(30) : DateTime.UtcNow.AddDays(-7);
                        p.SetValue(obj, isNullable ? (object?)dt : dt);
                    }
                    continue;
                }

                // ---- Nested complex types
                if (!IsSimple(propType))
                {
                    var child = p.GetValue(obj);
                    if (child == null)
                    {
                        if (propType.IsClass && !propType.IsAbstract && propType.GetConstructor(Type.EmptyTypes) != null)
                        {
                            child = Activator.CreateInstance(propType);
                            p.SetValue(obj, child);
                        }
                    }
                    if (child != null)
                        FillStringsRecursively(child, options);
                }
            }
        }

        private static bool IsSimple(Type t)
        {
            return t.IsPrimitive
                   || t.IsEnum
                   || t == typeof(string)
                   || t == typeof(decimal)
                   || t == typeof(DateTime)
                   || t == typeof(Guid);
        }

        // =====================================================
        // ✅ YOUR EXISTING PROVIDERS (ready to use)
        // =====================================================

        // EMV Card Profile - Create (POST)
        public static IEnumerable<TestCaseData> EMVCardProfile_Create_TestData()
        {
            // Positive: issuerId kept, expiration future, other strings randomized
            yield return new TestCaseData(
                Prepare(seed: (EMVCardProfilesCreateRequest req) =>
                {
                    req.issuerId = 8431;                               // keep int
                    req.expirationDate = RandomData.FutureDate(30, 365); // explicit future
                })
            ).SetName("Positive_Data_EMVCardProfilesCreate");

            // Negative: invalid issuerId=0, expiration in past
            yield return new TestCaseData(
                Prepare(seed: (EMVCardProfilesCreateRequest req) =>
                {
                    req.issuerId = 0;                                  // invalid
                    req.expirationDate = RandomData.PastDate(1, 90);   // past date
                })
            ).SetName("Negative_EMVCardProfilesCreate_InvalidissuerId");
        }

        // EMV Card Profile - Update (PUT)
        public static IEnumerable<TestCaseData> GetUpdateUserTestData()
        {
            // Positive update for id=1
            yield return new TestCaseData(
                1,
                Prepare(seed: (EMVCardProfilesCreateRequest req) =>
                {
                    req.issuerId = 8431;
                    req.expirationDate = RandomData.FutureDate(15, 365);
                })
            ).SetName("UpdateUser_Positive");

            // Negative: invalid id=9999 with bad issuerId/past expiration
            yield return new TestCaseData(
                9999,
                Prepare(seed: (EMVCardProfilesCreateRequest req) =>
                {
                    req.issuerId = 0;
                    req.expirationDate = RandomData.PastDate(1, 60);
                })
            ).SetName("UpdateUser_Negative_InvalidId");
        }

        // Users - Get by ID (GET)
        public static IEnumerable<TestCaseData> GetGetUserTestData()
        {
            yield return new TestCaseData(1).SetName("GetUser_Positive");
            yield return new TestCaseData(9999).SetName("GetUser_Negative_InvalidId");
        }

        // Users - Get All (GET)
        public static IEnumerable<TestCaseData> GetGetAllUsersTestData()
        {
            yield return new TestCaseData().SetName("GetAllUsers_Positive");
        }

        // Users - Delete (DELETE)
        public static IEnumerable<TestCaseData> GetDeleteUserTestData()
        {
            yield return new TestCaseData(1).SetName("DeleteUser_Positive");
            yield return new TestCaseData(9999).SetName("DeleteUser_Negative_InvalidId");
        }

        // Users - Create (POST)
        public static IEnumerable<TestCaseData> Users_Create_TestData()
        {
            // Positive: strings randomized, ints kept, email fixed domain
            yield return new TestCaseData(
                Prepare(seed: (UserscreateRequest req) =>
                {
                    req.pclIds = new List<int> { 13 };
                    req.roleIds = new List<int> { 5 };
                    req.StatusId = 1216;
                })
            ).SetName("Positive_Data_Usercreation");

            // Negative: empty firstName (invalid) — exclude from auto-fill so it stays empty
            yield return new TestCaseData(
                Prepare(
                    seed: (UserscreateRequest req) =>
                    {
                        req.pclIds = new List<int> { 13 };
                        req.roleIds = new List<int> { 5 };
                        req.StatusId = 1216;
                        req.firstName = ""; // intended invalid
                    },
                    options: new FillOptions
                    {
                        // Keep all defaults, but exclude 'firstName' from auto-fill
                        ExcludeProperties = { "firstName" }
                    })
            ).SetName("Negative_Testdata_Firstname");
        }
    }
}
