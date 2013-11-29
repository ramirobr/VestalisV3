
namespace Cotecna.Vestalis.Core
{
    public class UserGridModel
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string BusinessApplications { get; set; }
        public string UserType { get; set; }

        public UserGridModel()
        {
            Email = string.Empty;
            FullName = string.Empty;
            BusinessApplications = string.Empty;
        }
    }
}
