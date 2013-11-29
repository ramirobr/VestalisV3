using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Cotecna.Vestalis.Core;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.Unity;
using System.Text.RegularExpressions;
namespace Cotecna.Vestalis.Web.Controllers
{
    /// <summary>
    /// This class contains all action methods needed for performing a login in the system
    /// </summary>
    public class HomeController : Controller
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

        #region action methods

        #region Index
        /// <summary>
        /// Initialize the screen the first time
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region Index
        /// <summary>
        /// Perform the login operation into system
        /// </summary>
        /// <param name="model">LoginModel</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult Index(LoginModel model)
        {
            
            if (string.IsNullOrEmpty(model.UserName))
                ModelState.AddModelError("UserName not filled", Resources.Common.UserNameNotFilled);
            else
            {
                Regex regEx = new Regex("^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$");
                if (!regEx.IsMatch(model.UserName))
                    ModelState.AddModelError("", Resources.Administration.EmailFormatNotValid);
            }
            
            if (string.IsNullOrEmpty(model.Password))
                ModelState.AddModelError("Password not filled", Resources.Common.PasswordNotFilled);
            
            //verify if all information is completed
            if (ModelState.IsValid)
            {
                //validate the user
                if (AuthorizationBusiness.Instance.LogOn(model.UserName, model.Password))
                {
                    string[] rolesForUser = Roles.GetRolesForUser(model.UserName);
                    //if its ok, set the login
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    //get the list of business applications by of the user
                    List<BusinessApplicationByUser> businessAplicationsByUser = AuthorizationBusiness.GetBusinessApplicationsByUser(model.UserName);

                    var businessAplicationsByUserTemp = new List<BusinessApplicationByUser>(businessAplicationsByUser.AsEnumerable());

                    foreach (var businessApp in businessAplicationsByUser)
                    {
                        BusinessApplicationByUser app = businessApp;
                        var rolesByApp = rolesForUser.Where(role => role.Contains("_" + app.Prefix)).ToList();
                        if (rolesByApp.Any(role => role.Contains("ApplicationAdministrator")) && rolesByApp.Count == 1)
                        {
                            businessAplicationsByUserTemp.Remove(businessApp);
                        }
                    }
                    businessAplicationsByUser = businessAplicationsByUserTemp;

                    SetBusinessApplication(model, businessAplicationsByUser);

                    //add in a session this list
                    Session.Add("BusinessAplicationsByUser", businessAplicationsByUser);

                    //if the user is GlobalAdministrator or ApplicationAdministrator, the system will display catalogue administation screen by default.
                    //otherwise the system will display service order screen.
                    if (Roles.IsUserInRole(model.UserName, "GlobalAdministrator") || rolesForUser.Any(role => role.Contains("ApplicationAdministrator")))
                    {
                        //redirect to business screen

                        if (Roles.IsUserInRole(model.UserName, "GlobalAdministrator") || 
                            (rolesForUser.Any(role => role.Contains("ApplicationAdministrator")) && (rolesForUser.Count(role => role.Contains("ApplicationAdministrator")) == rolesForUser.Count())))
                            return RedirectToAction("Index", "Catalogue");
                        else
                            return RedirectToAction("Index", "ServiceOrder");
                    }
                    else
                    {   
                        //redirect to service order screen
                        return RedirectToAction("Index", "ServiceOrder");
                    }
                }
                else
                {
                    //if the user is not valid add an error message
                    ModelState.AddModelError("Login Error", Resources.Common.WrongLogin);
                }
            }
            return View(model);
        }

        /// <summary>
        /// Set business applications
        /// </summary>
        /// <param name="model">Login model</param>
        /// <param name="businessAplicationsByUser">List of business applications</param>
        /// <returns></returns>
        private BusinessApplicationByUser SetBusinessApplication(LoginModel model, List<BusinessApplicationByUser> businessAplicationsByUser)
        {
            //Set the default application
            BusinessApplicationByUser applicationByUser = new BusinessApplicationByUser();
            UserProfile profile = UserProfile.GetUserProfile(model.UserName);
            if (string.IsNullOrEmpty(profile.ApplicationDefault))
            {
                applicationByUser = businessAplicationsByUser.FirstOrDefault();
            }
            else
            {
                Guid businessAppId = new Guid(profile.ApplicationDefault);
                if (businessAplicationsByUser.Select(data => data.Id).Contains(businessAppId))
                    applicationByUser = businessAplicationsByUser.FirstOrDefault(data => data.Id == businessAppId);
                else
                    applicationByUser = businessAplicationsByUser.FirstOrDefault();
            }
            if (applicationByUser != null)
            {
                Session.Add("BusinessAplicationId", applicationByUser.Id);
                Session.Add("LanguageAplication", applicationByUser.LanguageCode);
                Session.Add("objDefaultBusinessApp", applicationByUser);
            }
            return applicationByUser;
        }
        #endregion

        #region LogOut
        /// <summary>
        /// Perform the logout operation
        /// </summary>
        /// <returns>ActionResult</returns>
        public ActionResult LogOut()
        {
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }
        #endregion

        #region Error
        public ActionResult Error()
        {
            return View("Error");
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

    }
}
