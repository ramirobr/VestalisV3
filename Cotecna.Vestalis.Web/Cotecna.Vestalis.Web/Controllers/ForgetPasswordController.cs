using System;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using Cotecna.Vestalis.Core;
using Cotecna.Vestalis.Web.Properties;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.Unity;

namespace Cotecna.Vestalis.Web.Controllers
{
    /// <summary>
    /// This class contains the action and validation methods needed for managing all about of forgot password functionality
    /// </summary>
    public class ForgetPasswordController : Controller
    {
        #region fields
        private ExceptionManager _exceptionManager;
        #endregion

        #region properties

        #region ExceptionManager
        /// <summary>
        /// Used to handle exceptions and register them in the log file
        /// </summary>    
        public ExceptionManager ExceptionManager
        {
            get
            {
                IUnityContainer container = new UnityContainer();
                try
                {
                    if (_exceptionManager == null)
                    {

                        container.AddNewExtension<Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Unity.
                                    EnterpriseLibraryCoreExtension>();
                        return container.Resolve<ExceptionManager>();
                    }
                    else
                    {
                        return _exceptionManager;
                    }
                }
                finally
                {
                    ((IDisposable)container).Dispose();
                }

            }
            set
            {
                _exceptionManager = value;
            }
        }
        #endregion

        #endregion

        #region overriden methods

        #region OnException
        /// <summary>
        /// Management the exceptions
        /// </summary>
        /// <param name="filterContext">ExceptionContext</param>
        protected override void OnException(ExceptionContext filterContext)
        {
            //Handle the exception with Enterprise Library (configured in Web.Config)
            //Search for this in the Web.config file <exceptionPolicies> tag
            this.ExceptionManager.HandleException(filterContext.Exception, "AllExceptionsPolicy");

            bool callMonitoring = bool.Parse(ConfigurationManager.AppSettings["CallMonitoring"].ToString());

            if (callMonitoring)
            {
                //Save Exception information in monitoring service, and return the ticket number generated
                ExceptionBusiness.CatchExceptionInMonitoringService(filterContext.Exception);
            }
            //Show basic Error view
            filterContext.ExceptionHandled = true;

            //Clear any data in the model as it wont be needed
            ViewData.Model = null;

            //Show basic Error view
            View("Error").ExecuteResult(this.ControllerContext);
        }
        #endregion

        #endregion

        #region public methods

        #region ForgetPasswordGetEmail
        /// <summary>
        /// Show the view for resetting the password
        /// </summary>
        /// <returns></returns>
        [OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
        public ActionResult ForgetPasswordGetEmail()
        {
            return View("ResetPassword", new ResetPasswordModel());
        }
        #endregion

        #region ResetPassword
        /// <summary>
        /// Begin the process for resetting password
        /// </summary>
        /// <param name="model">ResetPasswordModel</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            //the system validate if the user name have been written
            if (string.IsNullOrEmpty(model.UserName))
                ModelState.AddModelError("UserName", Resources.Administration.ForgetPasswordEmailNotValid);

            //the system get the solution of the captcha
            var solution = Session[CaptchaController.FORGOTPASSWORD_CAPTCHAKEY];
            Session.Remove(CaptchaController.FORGOTPASSWORD_CAPTCHAKEY);//only valid once, remove inmediately

            //the system validate if the captcha code have been written correctly
            if (string.Compare(solution.ToString(), model.CaptchaValue, true) != 0 || solution == null)
                ModelState.AddModelError("isHuman", Resources.Administration.ForgetPasswordCaptchaNotValid);

            //if not exist errors, the system continues with the process
            if (ModelState.IsValid)
            {
                //the system gets the user information
                MembershipUser user = Membership.GetUser(model.UserName);
                //if the user exist, the system continues with the process
                if (user != null)
                {
                    //the system reset the password of the user, and get a temporal password
                    string tempPassword = AuthorizationBusiness.Instance.GetSaveTemporalPassword(user);

                    //the system reads the template
                    string messageBody = System.IO.File.ReadAllText(Server.MapPath("~/Templates/PasswordResetMessageRequest.htm"));

                    //the system sets the encryted url
                    DateTime now = DateTime.Now;
                    string enc = "zi=" + System.Web.HttpUtility.UrlEncode(EncryptionHelper.EncryptAes(model.UserName + "&&" + now.Year + "&&" + now.Month + "&&" + now.Day + "&&" + now.Hour + "&&" + now.Minute));

                    //the system fills the template
                    messageBody = messageBody.Replace("{TEMPORAL_PASSWORD}", tempPassword);
                    messageBody = messageBody.Replace("{INSERT ENCRYPTED}", enc);

                    //the system send an email to the user, with the necessary information for reset the password
                    EmailBusiness.SendEmail(model.UserName, messageBody, Resources.Administration.ForgetPasswordSubjectEmail, Settings.Default.EmailSupport, Settings.Default.NameEmailSupport);
                    //the system shows a confirmation message
                    return View("ResetPasswordConfirmation");
                }
                else
                {
                    //if the user not exist, the system will show an error message
                    ModelState.AddModelError("userNotExist", Resources.Administration.ForgetPasswordUserNotFound);
                    ViewBag.HasErrors = true;
                    return View("ResetPassword", model);
                }
            }
            else
            {
                //if the validation process is not completed, the system will show all errors.
                ViewBag.HasErrors = true;
                return View("ResetPassword", model);
            }
        }
        #endregion

        #region ChangePassword
        /// <summary>
        /// Show the view ChangePassword
        /// </summary>
        /// <param name="zi">Url parameter</param>
        /// <returns>ActionResult</returns>
        [HttpGet]
        public ActionResult ChangePassword(string zi)
        {
            DateTime now = DateTime.Now;
            DateTime sentTime;
            string userName = "";
            try
            {
                //read the parameters from url
                string dec = EncryptionHelper.DecryptAes(zi);
                string[] separator = { "&&" };
                string[] data = dec.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                //get the information
                userName = data[0];
                sentTime = new DateTime(Convert.ToInt32(data[1]), Convert.ToInt32(data[2]), Convert.ToInt32(data[3]), Convert.ToInt32(data[4]), Convert.ToInt32(data[5]), 0);

                //validate if is valid
                TimeSpan t = now - sentTime;
                if (t.TotalDays > 3)
                    return View("Expired");

                if (AuthorizationBusiness.VerifyResetPassword(sentTime, userName))
                    return View("Expired");

                //if all is ok, the system shows change password view
                ChangePasswordModel model = new ChangePasswordModel();
                model.UserName = userName;

                return View("ChangePassword", model);

            }
            catch (Exception)
            {
                return View("Unavailable");
            }
        }
        #endregion

        #region SaveChangePassword
        /// <summary>
        /// Save the new password
        /// </summary>
        /// <param name="model">ChangePasswordModel</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult SaveChangePassword(ChangePasswordModel model)
        {
            //the system validates old password
            if (string.IsNullOrEmpty(model.OldPassword))
                ModelState.AddModelError("OldPassRequired", Resources.Administration.OldPassRequired);
            if (model.OldPassword.Length < AuthorizationBusiness.Instance.MinPasswordLength)
                ModelState.AddModelError("", Resources.Administration.MinLegthPassword);
            //The system validates new password
            if (string.IsNullOrEmpty(model.NewPassword))
                ModelState.AddModelError("NewPassRequired", Resources.Administration.NewPassRequired);
            if (model.NewPassword.Length < AuthorizationBusiness.Instance.MinPasswordLength)
                ModelState.AddModelError("", Resources.Administration.MinLegthPassword);
            //the system validates new password
            if (string.IsNullOrEmpty(model.ReNewPassword))
                ModelState.AddModelError("ReNewPassWordRequired", Resources.Administration.ReNewPassWordRequired);
            if (model.ReNewPassword.Length < AuthorizationBusiness.Instance.MinPasswordLength)
                ModelState.AddModelError("", Resources.Administration.MinLegthPassword);
            //the system validates if the new password and the ReNewPassword are equals
            if (model.NewPassword != model.ReNewPassword)
                ModelState.AddModelError("", Resources.Administration.NotEqualPassword);
            //validate the temporary password
            if (!AuthorizationBusiness.Instance.CompareTemporalPassword(model.UserName, model.OldPassword))
                ModelState.AddModelError("", "The temporary password is not correct");

            //if there are no errors, the system will continue with the process
            if (ModelState.IsValid)
            {
                //change the password.
                if (AuthorizationBusiness.Instance.ResetPassword(model.UserName, model.ReNewPassword))
                {
                    string userEmail = model.UserName;
                    
                    //the system reads the template
                    string messageBody = System.IO.File.ReadAllText(Server.MapPath("~/Templates/PasswordChangedConfirmationTemplate.htm"));
                    
                    //the system send an email to the user, with the necessary information for reset the password
                    EmailBusiness.SendEmail(userEmail, messageBody, Resources.Administration.ForgetPasswordSubjectConfirEmail, Settings.Default.EmailSupport, Settings.Default.NameEmailSupport);

                    return View("ChangePasswordConfirmation");
                }
                else
                {
                    ModelState.AddModelError("", Resources.Administration.ForgetPasswordGeneralError);
                    return View("ChangePassword", model);
                }
            }
            else
            {
                //if exist errors, the system will display the errors.
                return View("ChangePassword", model);
            }

        }
        #endregion

        #endregion

    }
}
