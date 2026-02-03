using ApiAutomationFramework.Models.Request;
using ApiAutomationFramework.Models.Request.SLA;
using ApiAutomationFramework.Models.Request.Users;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace APITestSolution.DataProviders
{
    public static class SLADataProvider
    {

            private const string DefaultEmailDomain = "zensar.com";

            // =========================
            // Random helpers
            // =========================
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

                public static string AutoTag(int letters = 5) => "Auto_" + LettersOnly(letters);
                public static string Email(string domain) => $"{AutoTag(5)}@{domain}";
                public static string FutureDate(int minDays = 7, int maxDays = 365)
                {
                    if (minDays < 0) minDays = 0;
                    if (maxDays < minDays) maxDays = minDays;
                    var days = RandomNumberGenerator.GetInt32(minDays, maxDays + 1);
                    return DateTime.UtcNow.AddDays(days).ToString("yyyy-MM-dd");
                }
                public static string PastDate(int minDaysAgo = 1, int maxDaysAgo = 30)
                {
                    if (minDaysAgo < 0) minDaysAgo = 0;
                    if (maxDaysAgo < minDaysAgo) maxDaysAgo = minDaysAgo;
                    var days = RandomNumberGenerator.GetInt32(minDaysAgo, maxDaysAgo + 1);
                    return DateTime.UtcNow.AddDays(-days).ToString("yyyy-MM-dd");
                }
                public static string Digits(int length = 10)
                {
                    if (length <= 0) return string.Empty;
                    var sb = new StringBuilder(length);
                    for (int i = 0; i < length; i++)
                        sb.Append(RandomNumberGenerator.GetInt32(0, 10));
                    return sb.ToString();
                }
            }

            // =========================
            // Generic auto-fill
            // =========================
            public sealed class FillOptions
            {
                public string EmailDomain { get; init; } = DefaultEmailDomain;
                public bool OnlyFillWhenEmpty { get; init; } = true;
                public int AutoLetters { get; init; } = 5;
                public HashSet<string> ExcludeProperties { get; } = new(StringComparer.OrdinalIgnoreCase);
            }

            public static T Prepare<T>(Action<T>? seed = null, FillOptions? options = null)
                where T : new()
            {
                options ??= new FillOptions();
                var dto = new T();
                seed?.Invoke(dto);
                FillStringsRecursively(dto!, options);
                return dto!;
            }

            private static void FillStringsRecursively(object obj, FillOptions options)
            {
                if (obj == null) return;

                if (obj is IList list)
                {
                    foreach (var item in list)
                        FillStringsRecursively(item, options);
                    return;
                }

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

                    if (options.ExcludeProperties.Contains(name))
                        continue;

                    // Strings
                    if (propType == typeof(string))
                    {
                        var current = p.GetValue(obj) as string;
                        var isEmpty = string.IsNullOrWhiteSpace(current);
                        if (!isEmpty && options.OnlyFillWhenEmpty)
                            continue;

                        string value =
                            lower.Contains("email") ? RandomData.Email(options.EmailDomain) :
                            lower.Contains("mobile") || lower.Contains("telephone") || lower.Contains("phone") ? RandomData.Digits(10) :
                            lower.Contains("expir") && lower.Contains("date") ? RandomData.FutureDate(15, 365) :
                            lower.Contains("date") ? RandomData.PastDate(1, 30) :
                            "Auto_" + RandomData.LettersOnly(options.AutoLetters);

                        p.SetValue(obj, value);
                        continue;
                    }

                    // DateTime
                    if (propType == typeof(DateTime) || propType == typeof(DateTime?))
                    {
                        var isNullable = propType == typeof(DateTime?);
                        var current = p.GetValue(obj);

                        var shouldFill = !options.OnlyFillWhenEmpty
                                         || current == null
                                         || (current is DateTime existing && existing == default);

                        if (shouldFill)
                        {
                            var fillDate = lower.Contains("expir")
                                ? DateTime.UtcNow.AddDays(30)
                                : DateTime.UtcNow.AddDays(-7);

                            p.SetValue(obj, isNullable ? (object?)fillDate : fillDate);
                        }
                        continue;
                    }

                    // Nested complex
                    if (!IsSimple(propType))
                    {
                        var child = p.GetValue(obj);
                        if (child == null &&
                            propType.IsClass && !propType.IsAbstract &&
                            propType.GetConstructor(Type.EmptyTypes) != null)
                        {
                            child = Activator.CreateInstance(propType);
                            p.SetValue(obj, child);
                        }
                        if (child != null)
                            FillStringsRecursively(child, options);
                    }
                }
            }

            private static bool IsSimple(Type t) =>
                t.IsPrimitive || t.IsEnum || t == typeof(string) || t == typeof(decimal) || t == typeof(DateTime) || t == typeof(Guid);

           

            //// =====================================================
            //// SLA – CREATE (POST) — split providers
            //// =====================================================
            //public static IEnumerable<TestCaseData> SLA_Create_Positive_TestData()
            //{
            //    yield return new TestCaseData(
            //        Prepare(seed: (SLACreateRequest req) =>
            //        {
            //            req.id = 0;
            //            req.cardProgramId = 1111;
            //            req.isSpecialProject = true;
            //            req.isRepeatedEveryYear = true;
            //            req.sladay = 0;
            //            req.specialProjectMinimumShipmentCardCount = 1;
            //            req.specialProjectSladay = 33;
            //        })
            //    ).SetName("Positive_Data_Usercreation");
            //}

            //public static IEnumerable<TestCaseData> SLA_Create_Negative_TestData()
            //{
            //    yield return new TestCaseData(
            //        Prepare(
            //            seed: (SLACreateRequest req) =>
            //            {
            //                req.id = 0;
            //                req.cardProgramId = 1298;
            //                req.isSpecialProject = true;
            //                req.isRepeatedEveryYear = true;
            //                req.sladay = 0;
            //                req.specialProjectMinimumShipmentCardCount = 1;
            //                req.specialProjectSladay = 00;

            //            },
            //            options: new FillOptions { ExcludeProperties = { "specialProjectSladay" } })
            //    ).SetName("Negative_Testdata_FirstnameEmpty");
            //}


    }
}



