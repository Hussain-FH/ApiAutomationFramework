
namespace ApiAutomationFramework.Models.Request.Users
{
    public class UsersUpdateRequest
    {
        public int userId { get; set; }
        public List<int> pclIdDelete { get; set; }
        public List<int> pclIdInsert { get; set; }
        public List<int> roleIdDelete { get; set; }
        public List<int> roleIdInsert { get; set; }
        public int statusCodeId { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
    }
}
