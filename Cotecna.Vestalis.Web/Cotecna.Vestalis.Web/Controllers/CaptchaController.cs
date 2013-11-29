using System;
using System.Configuration;
using System.Web.Mvc;
using Cotecna.Vestalis.Core;
using Cotecna.Vestalis.Web.Common;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.Unity;
using XCaptcha;

namespace Cotecna.Vestalis.Web.Controllers
{
    /// <summary>
    /// This class contains all action methods needed for showing a captcha image 
    /// </summary>
    public class CaptchaController : Controller
    {
        #region constants
        internal const string FORGOTPASSWORD_CAPTCHAKEY = "ForgotPasswordCaptchaSessionKey";
        internal const string CREATEUSER_CAPTCHAKEY = "CreateUserCaptchaSessionKey";
        #endregion

        #region fields
        private ExceptionManager _exceptionManager;
        #endregion

        #region properties
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

        #region overriden methods
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

        #region public methods
        /// <summary>
        /// Provides a captcha gif image for the forgot password page. 
        /// The user need to provide the 6 letter human test
        /// </summary>
        /// <returns>A gif distorted image</returns>
        [OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
        public ActionResult ForgotPasswordCaptcha()
        {
            CaptchaResult result = CreateCaptcha();

            Session.Add(FORGOTPASSWORD_CAPTCHAKEY, result.Solution);

            return new FileContentResult(result.Image, result.ContentType);

        }

        /// <summary>
        /// Create the captcha
        /// </summary>
        /// <returns></returns>
        private CaptchaResult CreateCaptcha()
        {
            CotecnaCanvas ourCanvas = new CotecnaCanvas();
            CotecnaTextStyle ourTextStyle = new CotecnaTextStyle();
            CotecnaProvidedNoise ourNoise = new CotecnaProvidedNoise();
            CotecnaDistort ourDistort = new CotecnaDistort();

            XCaptcha.ImageBuilder builder = new XCaptcha.ImageBuilder(6, ourCanvas, ourTextStyle, ourDistort, ourNoise);

            //Create a 6 letter captcha
            return builder.Create();
        }

        /// <summary>
        /// Provides a captcha gif image for creating a new user
        /// The user need to provide the 6 letter human test
        /// </summary>
        /// <returns>A gif distorted image</returns>
        [OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
        public ActionResult CreateUserCaptcha()
        {
            //Create a 6 letter captcha
            XCaptcha.CaptchaResult result = CreateCaptcha();

            Session.Add(CREATEUSER_CAPTCHAKEY, result.Solution);

            return new FileContentResult(result.Image, result.ContentType);
        }
        #endregion
    }
}
