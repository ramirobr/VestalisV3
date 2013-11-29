
namespace Cotecna.Vestalis.Core
{
    public class ResetPasswordModel
    {
        public string UserName { get; set; }
        public string CaptchaValue { get; set; }

        public ResetPasswordModel()
        {
            UserName = string.Empty;
            CaptchaValue = string.Empty;
        }
    }
}
