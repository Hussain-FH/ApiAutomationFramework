
using Microsoft.Extensions.Configuration;

namespace ApiAutomationFramework
{
    public static class ApiEndpoints
    {
        // Load appsettings.json once
        private static readonly IConfiguration _config =
            new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

        // Read values from appsettings.json
        private static readonly string BaseUrl = _config["BaseUrl_UAT"] ?? string.Empty;
        private static readonly string AuthUrl = _config["AuthUrl_UAT"] ?? string.Empty;

        // Helper method to build URLs
        private static string Url(string path) => $"{BaseUrl}/{path}";

        // ✅ API Endpoints


        //EMV CardProfiles
        public static string EMVCardProfiles_create => Url("cardprofiles");
        public static string EMVCardProfiles_update => Url("cardprofiles");
        public static string EMVCardProfiles_Get => Url("cardprofiles");

        //EMV Modules
        public static string EMVModules_create => Url("modules");
        public static string EMVModules_update => Url("modules");
        public static string EMVModules_GetAll => Url("modules");

        //users  

        public static string Users_createuser => Url("users");
        public static string Users_updateuser => Url("users/updateuser");
        public static string Users_GetUsers => Url("users?PclId=13&IsInternal=true");
        public static string UpdateUser(string id) => Url($"users/{id}");
        public static string DeleteUser(string id) => Url($"users/{id}");


        // ✅ Auth Endpoint
        public static string SignIn => AuthUrl;

        //SLA
        public static string SLA_createSLA => Url("slaconfigurationparameters");
        public static string SLA_updateSLA => Url("slaconfigurationparameters");
        public static string SLA_GetSLA => Url("slaconfigurationparameters?PclId=13");
        public static string SLA_DeleteSLA(int id) => Url($"slaconfigurationparameters?Id={id}");


        //Client Settings - Hot Stamps

        public static string CSHotStamps_create => Url("hotstamps");
        public static string CSHotStamps_update => Url("hotstamps");
        public static string CSHotStamps_Get => Url("hotstamps?PclId=13");


        //Client Settings - ClientProfile

        public static string CSClientProfile_create => Url("clientprofiles");
        public static string CSClientProfile_update => Url("clientprofiles");
        public static string CSClientProfile_Get => Url("clientprofiles/getmasterdata");
        public static string CSClientProfile_Delete => Url("clientprofiles");


        //HotStampDie DropDown
        public static string HotStampDieDrp_Get => Url("programs/hotstampdie?PclId=13");
        public static string TippingModulDrp_Get => Url("programs/tippingmodule");

        //categories
        public static string Categories_Create => Url("categories");
        public static string Categories_Rename => Url("categories/rename");
        public static string Categories_TurnOffON => Url("categories/turnonoff");
        public static string Categories_MoveupDown => Url("categories/moveupdown");
        public static string Categories_Makedefault => Url("categories/makedefault");
        public static string Categories_RemoveCat => Url("categories/remove?categoryId={0}");


        //CSPProgram

        public static string CSPProgramCardholderdrp_Get => Url("programs/dynamicinformationcarrier?clientprogramcodeid=1458");
        public static string CSPProgramDymInfo_Get => Url("programs/dynamicinformationcarrier?clientprogramcodeid=1459");
        public static string CSPProgramComponent_Get => Url("programs/cspprogramcomponents?CardProgramId=100");
    }
}
