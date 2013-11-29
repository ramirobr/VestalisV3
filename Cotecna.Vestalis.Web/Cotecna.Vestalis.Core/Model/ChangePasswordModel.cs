
namespace Cotecna.Vestalis.Core
{
    public class ChangePasswordModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly")]
        public string ReNewPassword { get; set; }
        public string UserName { get; set; }
        public int UserType { get; set; }
    }
}
