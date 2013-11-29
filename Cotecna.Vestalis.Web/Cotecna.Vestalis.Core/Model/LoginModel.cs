using System.ComponentModel.DataAnnotations;
namespace Cotecna.Vestalis.Core
{
    public class LoginModel
    {
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
