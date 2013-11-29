
namespace Cotecna.Vestalis.Core
{
    public class PermissionModel
    {
        public string LoginName { get; set; }
        public PaginatedList<PermissionGridModel> PermissionList { get; set; }
        public int UserType { get; set; }

        public PermissionModel()
        {
            LoginName = string.Empty;
            PermissionList = new PaginatedList<PermissionGridModel>();
            UserType = 0;
        }
    }
}
