
using ApiAutomationFramework.Models.Request;
using ApiAutomationFramework.Models.Request.ClientSettiClientprofiles;
using ApiAutomationFramework.Models.Request.ClientSettingsHotStamps;
using ApiAutomationFramework.Models.Request.EMV;
using ApiAutomationFramework.Models.Request.SLA;
using ApiAutomationFramework.Models.Request.Users;
using ApiAutomationFramework.Models.Request.Categories;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
    public static class UserDataProvider
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

        // =====================================================
        // USERS – CREATE (POST) — split providers
        // =====================================================
        public static IEnumerable<TestCaseData> Users_Create_Positive_TestData()
        {
            yield return new TestCaseData(
                Prepare(seed: (UserscreateRequest req) =>
                {
                    req.pclIds = new List<int> { 13 };
                    req.roleIds = new List<int> { 5 };
                    req.StatusId = 1216;
                })
            ).SetName("Users_Create_Positive_Data");
        }

        public static IEnumerable<TestCaseData> Users_Create_Negative_TestData()
        {
            yield return new TestCaseData(
                Prepare(
                    seed: (UserscreateRequest req) =>
                    {
                        req.pclIds = new List<int> { 13 };
                        req.roleIds = new List<int> { 5 };
                        req.StatusId = 1216;
                        req.email = "abcd@zensar.com"; // invalid
                    },
                    options: new FillOptions { ExcludeProperties = { "Invalid Email" } })
            ).SetName("Users_Create_Negative_Data");
        }

        // =====================================================
        // USERS – UPDATE (PUT) — split providers
        // Payload fields:
        // userId, pclIdDelete[], pclIdInsert[], roleIdDelete[], roleIdInsert[], statusCodeId, firstName, middleName, lastName
        // DTO expected: UsersUpdateRequest
        // =====================================================
        public static IEnumerable<TestCaseData> Users_Update_Positive_TestData()
        {
            yield return new TestCaseData(
                Prepare(seed: (UsersUpdateRequest req) =>
                {

                    req.statusCodeId = 1216;
                    // positive inserts to match your environment
                    // req.pclIdInsert = new List<int> { 13 };
                    // req.roleIdInsert = new List<int> { 5 };
                    req.middleName = "AutoUpdates";


                })
            ).SetName("Users_Update_Positive_Data");
        }

        public static IEnumerable<TestCaseData> Users_Update_Negative_TestData()
        {
            yield return new TestCaseData(
                Prepare(
                    seed: (UsersUpdateRequest req) =>
                    {

                        req.statusCodeId = 1216;
                        req.pclIdInsert = new List<int> { 13 };
                        req.roleIdInsert = new List<int> { 5 };

                    })

            ).SetName("Users_Update_Negative_Data");
        }

        // =====================================================
        // USERS – GET by PCL — split providers (pclId, isInternal)
        // =====================================================
        public static IEnumerable<TestCaseData> User_GetBy_ValidPcl_TestData()
        {
            yield return new TestCaseData(13, true)
                .SetName("Users_GetBy_ValidPclId");
        }

        public static IEnumerable<TestCaseData> User_GetBy_InValidPcl_TestData()
        {
            yield return new TestCaseData(99999, true)
                .SetName("Users_GetBy_InValidPclId");
        }



        //EMV Card Profile - POST, GET, PUT, Delete
        public static IEnumerable<TestCaseData> EMV_CardProfile_Create_Positive_TestData()
        {
            yield return new TestCaseData(
                Prepare(seed: (EMVCardProfilesCreateRequests req) =>
                {

                    req.issuerId = 8440;


                })
            ).SetName("EMV_CardProfiles_Create_Positive_Data");
        }

        public static IEnumerable<TestCaseData> EMV_CardProfile_Create_Negative_TestData()
        {
            yield return new TestCaseData(
                Prepare(seed: (EMVCardProfilesCreateRequests req) =>
                {
                    req.name = "sbfjhwbejhfbwejhfgwjehfghegfjwegvfhgvfhgwevfhgwevfhkgwevfhgwevfhgw";
                    req.issuerId = 8440;


                })
            ).SetName("EMV_CardProfiles_Create_Negative_Data");
        }

        public static IEnumerable<TestCaseData> EMV_CardProfile_Update_Positive_TestData()
        {
            yield return new TestCaseData(
                Prepare(seed: (EMVCardProfilePutRequest req) =>
                {

                    req.issuerId = 8440;


                })
            ).SetName("EMV_CardProfiles_Update_Positive_Data");
        }

        public static IEnumerable<TestCaseData> EMV_CardProfile_Update_Negative_TestData()
        {
            yield return new TestCaseData(
                Prepare(seed: (EMVCardProfilePutRequest req) =>
                {
                    req.name = "sbfjhwbejhfbwejhfgwjehfghegfjwegvfhgvfhgwevfhgwevfhkgwevfhgwevfhgw";
                    req.issuerId = 8440;


                })
            ).SetName("EMV_CardProfiles_Update_Negative_Data");
        }


        // =====================================================
        // SLA – CREATE (POST) — split providers
        // =====================================================
        public static IEnumerable<TestCaseData> SLA_Create_Positive_TestData()
        {
            yield return new TestCaseData(
                Prepare(seed: (SLACreateRequest req) =>
                {
                    req.id = 0;
                    req.cardProgramId = 1111;
                    req.isSpecialProject = true;
                    req.isRepeatedEveryYear = true;
                    req.sladay = 0;
                    req.specialProjectMinimumShipmentCardCount = 1;
                    req.specialProjectSladay = 33;
                })
            ).SetName("Positive_Data_SLAcreation");
        }

        public static IEnumerable<TestCaseData> SLA_Create_Negative_TestData()
        {
            yield return new TestCaseData(
                Prepare(
                    seed: (SLACreateRequest req) =>
                    {
                        req.id = 0;
                        req.cardProgramId = 1298;
                        req.isSpecialProject = true;
                        req.isRepeatedEveryYear = true;
                        req.sladay = 0;
                        req.specialProjectMinimumShipmentCardCount = 1;
                        req.specialProjectSladay = 00;

                    },
                    options: new FillOptions { ExcludeProperties = { "specialProjectSladay" } })
            ).SetName("Negative_Testdata_SLACreation");
        }


        // =====================================================
        // SLA – UPDATE – POSITIVE
        // =====================================================
        public static IEnumerable<TestCaseData> SLA_Update_Positive_TestData()
        {
            yield return new TestCaseData(
                Prepare(seed: (SLAUpdateRequest req) =>
                {
                    // ✅ Use a VALID SLA id from your DB here
                    //req.id = 1001;

                    // Valid data for update
                    req.cardProgramId = 1111;              // any valid int (no card profile logic in tests)
                    req.isSpecialProject = true;
                    req.isRepeatedEveryYear = false;

                    req.sladay = 10;
                    req.specialProjectMinimumShipmentCardCount = 500;
                    req.specialProjectSladay = 15;

                    //req.effectiveDate = new DateTime(2025, 01, 01);
                    //req.endDate = new DateTime(2025, 12, 31);
                })
            ).SetName("Positive_Data_SLA_Update");
        }

        // =====================================================
        // SLA – UPDATE – NEGATIVE
        // =====================================================
        public static IEnumerable<TestCaseData> SLA_Update_Negative_TestData()
        {
            yield return new TestCaseData(
                Prepare(
                    seed: (SLAUpdateRequest req) =>
                    {
                        // Assume this id exists, but we'll make the payload invalid
                        req.id = 1001;

                        req.cardProgramId = 01;
                        req.isSpecialProject = true;
                        req.isRepeatedEveryYear = false;

                        // ❌ INVALID: negative sladay and endDate before effectiveDate
                        req.sladay = 0;
                        req.specialProjectMinimumShipmentCardCount = 500;
                        req.specialProjectSladay = 15;

                        req.effectiveDate = new DateTime(2025, 12, 31);
                        req.endDate = new DateTime(2025, 01, 01);
                    },
                    options: new FillOptions
                    {
                        // So auto-fill doesn't fix our invalid values
                        ExcludeProperties =
                        {
                        "sladay",
                        "effectiveDate",
                        "endDate"
                        }
                    })
            ).SetName("Negative_Data_SLA_Update_InvalidSladayAndDates");
        }

        // =====================================================
        // SLA – GET by Id – POSITIVE
        // =====================================================
        public static IEnumerable<TestCaseData> SLA_Get_Positive_TestData()
        {
            // ✅ Use a VALID SLA id that is present in your environment
            yield return new TestCaseData(101)
                .SetName("Positive_Data_SLA_Get_ById_101");
        }

        // =====================================================
        // SLA – GET by Id – NEGATIVE
        // =====================================================
        public static IEnumerable<TestCaseData> SLA_Get_Negative_TestData()
        {
            // ❌ Use an ID that does NOT exist
            yield return new TestCaseData(010101010)
                .SetName("Negative_Data_SLA_Get_ById_Invalid");
        }

        // =====================================================
        // SLA – DELETE – POSITIVE
        // =====================================================
        public static IEnumerable<TestCaseData> SLA_Delete_Positive_TestData()
        {
            // ✅ Use an SLA id that CAN be deleted in your test env
            yield return new TestCaseData(2002)
                .SetName("Positive_Data_SLA_Delete_ById_2002");
        }

        // =====================================================
        // SLA – DELETE – NEGATIVE
        // =====================================================
        public static IEnumerable<TestCaseData> SLA_Delete_Negative_TestData()
        {
            // ❌ ID that does not exist or cannot be deleted
            yield return new TestCaseData(999999)
                .SetName("Negative_Data_SLA_Delete_ById_Invalid");
        }

        // NOTE:
        // - Prepare<T> and FillOptions are assumed to be the same helpers
        //   you already use in UserDataProvider.


        // =====================================================
        // Client Settings - Hot Stamps – CREATE – POSITIVE
        // =====================================================

        public static IEnumerable<TestCaseData> CSHotStamp_Create_Positive_TestData()
        {
            yield return new TestCaseData(
                Prepare(seed: (CSHotStampsCreateR req) =>
                {
                    req.pclId = 13;
                    req.name = "Gold Stamp";
                    req.description = "High quality";
                    req.userName = "Samantha";
                })
            ).SetName("Positive_Data_CSHotStamp_Create");
        }

        // =====================================================
        // Client Settings - Hot Stamps – CREATE – NEGATIVE
        // =====================================================
        public static IEnumerable<TestCaseData> CSHotStamp_Create_Negative_TestData()
        {
            yield return new TestCaseData(
                Prepare(
                    seed: (CSHotStampsCreateR req) =>
                    {
                        req.pclId = 13;
                        req.name = null;  // ❌ invalid
                        req.description = "Invalid test case";
                        req.userName = "UN";
                    },
                    options: new FillOptions
                    {
                        ExcludeProperties = { "name" } // ensure null stays null
                    })
            ).SetName("Negative_Data_CSHotStamp_Create_NameMissing");
        }

        // =====================================================
        // Client Settings - Hot Stamps – UPDATE – POSITIVE
        // =====================================================
        public static IEnumerable<TestCaseData> CSHotStamp_Update_Positive_TestData()
        {
            yield return new TestCaseData(
                Prepare(seed: (CSHotStampsUpdateR req) =>
                {
                    req.pclId = "13";
                    req.id = 101;
                    req.name = "Silver Stamp";
                    req.description = "Updated hot stamp description";
                    req.userName = "USNAme";
                })
            ).SetName("Positive_Data_CSHotStamp_Update");
        }


        // =====================================================
        // Client Settings - Hot Stamps – UPDATE – NEGATIVE
        // =====================================================

        public static IEnumerable<TestCaseData> CSHotStamp_Update_Negative_TestData()
        {
            yield return new TestCaseData(
                Prepare(
                    seed: (CSHotStampsUpdateR req) =>
                    {
                        req.id = 999999;  // assume exists or ignore
                        req.id = 13;
                        req.name = "";    // ❌ invalid empty name
                        req.description = "Invalid update attempt";
                        req.userName = "kishor";
                    },
                    options: new FillOptions
                    {
                        ExcludeProperties = { "name" }
                    })
            ).SetName("Negative_Data_CSHotStamp_Update_InvalidName");
        }

        // =====================================================
        // Client Settings - Hot Stamps – GET – POSITIVE
        // =====================================================
        public static IEnumerable<TestCaseData> CSHotStamp_Get_Positive_TestData()
        {
            yield return new TestCaseData(101) // provide valid id from your env
                .SetName("Positive_Data_CSHotStamp_Get_ById_101");
        }

        // =====================================================
        // Client Settings - Hot Stamps – GET – NEGATIVE
        // =====================================================

        public static IEnumerable<TestCaseData> CSHotStamp_Get_Negative_TestData()
        {
            yield return new TestCaseData(9999999)
                .SetName("Negative_Data_CSHotStamp_Get_ById_Invalid");
        }


        // =====================================================
        // CLIENT SETTINGS – CLIENT PROFILE – CREATE (POST)
        // DTO: ClientprofilesCreateRequest
        // =====================================================
        public static IEnumerable<TestCaseData> CSClientProfile_Create_Positive_TestData()
        {
            yield return new TestCaseData(
                Prepare(seed: (ClientprofilesCreateRequest req) =>
                {
                    req.pclId = "13";
                    req.keyId = 28; // will be overwritten by pre-req helper in test if needed
                    req.userName = "AutoUser_CP";
                    req.value = "true";
                    req.id = 0;
                })
            ).SetName("CSClientProfile_Create_Positive_Data");
        }

        public static IEnumerable<TestCaseData> CSClientProfile_Create_Negative_TestData()
        {
            // NEGATIVE CASE: invalid keyId and empty value
            yield return new TestCaseData(
                Prepare(
                    seed: (ClientprofilesCreateRequest req) =>
                    {
                        req.pclId = "13";
                        req.keyId = 99990000;              // invalid
                        req.userName = "";          // invalid
                        req.value = "true";             // invalid
                    },
                    options: new FillOptions { ExcludeProperties = { "", "" } }
                )
            ).SetName("CSClientProfile_Create_Negative_Data");
        }

        // =====================================================
        // CLIENT SETTINGS – CLIENT PROFILE – UPDATE (PUT)
        // DTO: ClientprofilesUpdateRequest
        // =====================================================
        public static IEnumerable<TestCaseData> CSClientProfile_Update_Positive_TestData()
        {
            yield return new TestCaseData(
                Prepare(seed: (ClientprofilesUpdateRequest req) =>
                {
                    req.pclId = 13;
                    req.keyId = 10; // will be overwritten by pre-req helper in test
                    req.userName = "AutCP";
                    req.value = "true";
                })
            ).SetName("CSClientProfile_Update_Positive_Data");
        }


        // =====================================================
        // CLIENT SETTINGS – CLIENT PROFILE – GET MASTERDATA (GET)
        // =====================================================
        public static IEnumerable<TestCaseData> CSClientProfile_Get_Positive_TestData()
        {
            yield return new TestCaseData()
                .SetName("CSClientProfile_GetMasterData_Positive");
        }

        public static IEnumerable<TestCaseData> CSClientProfile_Get_Negative_TestData()
        {
            // Negative by calling wrong route suffix (forces 404)
            yield return new TestCaseData("clientprofiles/getmasterdata_invalid")
                .SetName("CSClientProfile_GetMasterData_Negative_InvalidEndpoint");
        }

        // =====================================================
        // CLIENT SETTINGS – CLIENT PROFILE – DELETE (DELETE)
        // DTO: ClientprofilesDeleteRequest
        // =====================================================
        public static IEnumerable<TestCaseData> CSClientProfile_Delete_Positive_TestData()
        {
            yield return new TestCaseData(
                Prepare(seed: (ClientprofilesDeleteRequest req) =>
                {
                    req.pclId = "13";
                    req.keyId = 28; // will be overwritten by pre-req helper in test
                    req.id = 1401772;
                })
            ).SetName("CSClientProfile_Delete_Positive_Data");
        }

        public static IEnumerable<TestCaseData> CSClientProfile_Delete_Negative_TestData()
        {
            // NEGATIVE: invalid keyId
            yield return new TestCaseData(
                Prepare(seed: (ClientprofilesDeleteRequest req) =>
                {
                    req.pclId = "13";
                    req.keyId = 90000;  // invalid
                    req.id = 1;
                })
            ).SetName("CSClientProfile_Delete_Negative_Data");
        }


        // =====================================================
        // HotStampDropdown – GET – POSITIVE
        // =====================================================
        public static IEnumerable<TestCaseData> HotStampdrp_Get_Positive_TestData()
        {
            yield return new TestCaseData() // provide valid id from your env
                .SetName("Positive_Data_HotStampdrp_Get_ById_13");
        }

        // =====================================================
        // Tipping Module DropDown – GET – POSITIVE
        // =====================================================
        public static IEnumerable<TestCaseData> TippingModuleDrp_Get_Positive_TestData()
        {
            yield return new TestCaseData() // provide valid id from your env
                .SetName("Positive_Data_TippingModuleDrp_Get_ById");
        }

        
        // =====================================================
        // Categories_Create_Positive_TestData
        // =====================================================
        public static IEnumerable Categories_Create_Positive_TestData()
        {
            yield return new TestCaseData(
                new CategoriesCreateRequest
                {
                    Name = "Automated_Category",
                    ParentCategoryId = 266,
                    PclId = 118
                }
            ).SetName("CreateCategory_Valid_Parent1");
        }

        // =====================================================
        // Categories_Create_Negative_TestData
        // =====================================================

        public static IEnumerable Categories_Create_Negative_TestData()
        {
            yield return new TestCaseData(
                new CategoriesCreateRequest
                {
                    Name = "",          // invalid
                    ParentCategoryId = 1,
                    PclId = 100
                }
            ).SetName("CreateCategory_Invalid_EmptyName");
        }


        // =====================================================
        // Categories_PUT_Rename_TestData
        // =====================================================
        public static IEnumerable Categories_PUT_Rename_TestData()
        {
            yield return new TestCaseData(
                new CategoriesRenameRequest
                {
                    name = "Automated_Category",
                    categoryId = 447,
                    pclId = 127
                }
            ).SetName("PutCategory_Rename_Valid");
        }

        // =====================================================
        // Categories_PUT_TurnOffON_TestData
        // =====================================================
        public static IEnumerable Categories_PUT_TurnOffON_TestData()
        {
            yield return new TestCaseData(
                new CategoriesturnonoffRequest
                {
                   pclid= 13,
                   categoryId= 447,
                   active= true 
                }
            ).SetName("PutCategory_TurnOffON_Valid");
        }

        // =====================================================
        // Categories_PUT_MoveupDown_TestData
        // =====================================================
        public static IEnumerable Categories_PUT_MoveupDown_TestData()
        {
            yield return new TestCaseData(
                new CategoriesmoveupdownRequest
                {
                    pclid = 127,  
                    isUp= true,  
                    categoryId= 446, 
                    parentCategoryId= 442 
                }
            ).SetName("PutCategory_MoveupDown_Valid");
        }

        // =====================================================
        // Categories_PUT_MoveupDown_TestData
        // =====================================================
        public static IEnumerable Categories_PUT_Makedefault_TestData()
        {
            yield return new TestCaseData(
                new CategoriesMakedefaultRequest
                {
                    PclId= 118 ,
                    CategoryId=475,
                    CategoryTypeId= 266
                }
            ).SetName("PutCategory_Makedefault_Valid");
        }

        //// =====================================================
        // Categories_Delete_Positivr_TestData
        // =====================================================
        public static IEnumerable Categories_Delete_Positive_TestData()
        {
            yield return new TestCaseData(1).SetName("DeleteCategory_Positive");
        }


        // =====================================================
        // CSP_Program_Positive_TestData
        // =====================================================

        // ✅ POSITIVE TEST DATA
        public static IEnumerable CSPProgramCardholderdrp_Get_Positive_TestData()
            {
                yield return new TestCaseData(1).SetName("CSPProgramCardholderdrp_Get_Positive");
            }

            public static IEnumerable CSPProgramDymInfo_Get_Positive_TestData()
            {
                yield return new TestCaseData(1).SetName("CSPProgramDymInfo_Get_Positive");
            }

        public static IEnumerable CSPProgramComponent_Get_Positive_TestData()
        {
            yield return new TestCaseData(1).SetName("CSPProgramComponent_Get_Positive");
        }

    }
}

