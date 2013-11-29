using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using Cotecna.Vestalis.Core;
using Cotecna.Vestalis.Entities;
using Cotecna.Vestalis.Web.Common;
using System.Text.RegularExpressions;
using Resources;

namespace Cotecna.Vestalis.Web.Controllers
{
    /// <summary>
    /// This class contains all action and validation methods for managing all about user section
    /// </summary>
    public class AccountController : BaseController
    {

        #region methods

        #region Index
        /// <summary>
        /// Show the first page of user management
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            UserManagementModel model = new UserManagementModel();
            model.BusinessApplications = RetrieveBusinessApplications();
            return View(model);
        }
        #endregion

        #region SearchUsers
        /// <summary>
        /// Perform the search operation of the users
        /// </summary>
        /// <param name="page">Current page</param>
        /// <param name="businessApplicationId">Id of business application</param>
        /// <returns></returns>
        public PartialViewResult SearchUsers(int? page, Guid? businessApplicationId)
        {
            PaginatedList<UserGridModel> model = new PaginatedList<UserGridModel>();
            page = page == null ? 1 : page;
            int pageSize = Cotecna.Vestalis.Web.Properties.Settings.Default.PageSize;

            ParameterSearchUser parameters = new ParameterSearchUser
            {
                BusinessApplicationId = businessApplicationId.GetValueOrDefault(),
                PageSize = pageSize,
                SelectedPage = page.GetValueOrDefault(),
                SortDirection = SortDirection.Ascending,
                SortedColumn = "UserType",
                UserTypes = UserTypes,
                IsGlobalAdmin = IsGlobalAdmin,
                LoggedUserName = UserName
            };

            model = AuthorizationBusiness.Instance.GetUserList(parameters);
            Session.Add("selectedAppId", businessApplicationId);
            Session.Add("UserSearch", model);
            return PartialView("_UsersGrid", model);
        }
        #endregion

        #region SearchUsersPaginated
        /// <summary>
        /// Perform the search operation of the users
        /// </summary>
        /// <param name="sortDirection">Sort direction</param>
        /// <param name="sortedColumn">Sorted column</param>
        /// <param name="page">Current page</param>
        /// <returns></returns>
        public PartialViewResult SearchUsersPaginated(SortDirection? sortDirection, string sortedColumn, int? page)
        {
            PaginatedList<UserGridModel> model = new PaginatedList<UserGridModel>();
            Guid? businessApplicationId = Session["selectedAppId"] as Guid?;
            page = page == null ? 1 : page;
            int pageSize = Cotecna.Vestalis.Web.Properties.Settings.Default.PageSize;

            ParameterSearchUser parameters = new ParameterSearchUser
            {
                BusinessApplicationId = businessApplicationId.GetValueOrDefault(),
                PageSize = pageSize,
                SelectedPage = page.GetValueOrDefault(),
                SortDirection = sortDirection.GetValueOrDefault(),
                SortedColumn = sortedColumn,
                UserTypes = UserTypes,
                IsGlobalAdmin = IsGlobalAdmin,
                LoggedUserName = UserName
            };

            model = AuthorizationBusiness.Instance.GetUserList(parameters);
            Session.Add("selectedAppId", businessApplicationId);
            Session.Add("UserSearch", model);
            return PartialView("_UsersGrid", model);
        }
        #endregion

        #region NewUser
        /// <summary>
        /// Show the screen User in mode Add
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult NewUser()
        {
            UserModel model = new UserModel();
            model.OpenMode = (int)ScreenOpenMode.Add;
            model.UserTypes = UserTypes;

            model.BusinessApplications = RetrieveBusinessApplications();
            
            Session.Add("UserModel", model);
            return View("User", model);
        }
        #endregion

        #region SaveUser
        /// <summary>
        /// Save the user
        /// </summary>
        /// <param name="model">User Model</param>
        /// <returns>Json result</returns>
        [HttpPost]
        public ActionResult SaveUser(UserModel model)
        {
            ValidateUserSave(model);
            if (!model.HasErrors)
            {
                if (model.OpenMode == (int)ScreenOpenMode.Add)
                {
                    MembershipCreateStatus status = AuthorizationBusiness.Instance.SaveUser(model);
                    if (status != MembershipCreateStatus.Success)
                        model.ErrorList.Add(Resources.Administration.UserCreationError);
                    if (model.SelectedUserType != 1)
                        AuthorizationBusiness.VerifySaveVestalisUserApplications(model, UserName);
                    if (model.SelectedUserType == 4)
                        AuthorizationBusiness.SaveRolesForCotecnaUser(model);
                }
                else if (model.OpenMode == (int)ScreenOpenMode.Edit)
                {
                    AuthorizationBusiness.Instance.EditUser(model);
                }
            }
            Session.Add("UserModel", model);
            return Json(model);
        }
        #endregion

        #region ValidateUserSave
        /// <summary>
        /// Perform a validation of the user form before save
        /// </summary>
        /// <param name="model">User model</param>
        private void ValidateUserSave(UserModel model)
        {
            if (model.OpenMode == (int)ScreenOpenMode.Add)
            {
                //regular expression for validate e-mails
                Regex regEx = new Regex("^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$");

                //validations for the field e/mail
                if (string.IsNullOrEmpty(model.Email))//validate mandatory
                    model.ErrorList.Add(Resources.Administration.EmailMandatory);
                if (!string.IsNullOrEmpty(model.Email) && !regEx.IsMatch(model.Email))//validate format
                    model.ErrorList.Add(Resources.Administration.EmailFormatNotValid);
                //validations for the field password
                if (string.IsNullOrEmpty(model.Password))//validate mandatory
                    model.ErrorList.Add(Resources.Administration.PasswordNotFilled);
                if (!string.IsNullOrEmpty(model.Password) && model.Password.Length < 6)//validate lenght
                    model.ErrorList.Add(Resources.Administration.MinLegthPassword);

                //validation for mandatory business application
                if (model.BusinessApplicationId == Guid.Empty && model.SelectedUserType != 1)
                    model.ErrorList.Add(Resources.Administration.BusinessApplicationNotSelected);
                //validation for client field
                if (model.SelectedUserType == 3 && model.ClientId == null)
                    model.ErrorList.Add(Resources.Administration.ClientNotSelected);
                //validation for selected roles
                else if (model.SelectedUserType == 4 && string.IsNullOrEmpty(model.CheckedRoles))
                    model.ErrorList.Add(Resources.Administration.RolesNotSelected);
            }
            else if (model.OpenMode == (int)ScreenOpenMode.Edit)
            {
                //validations for the field password
                if (string.IsNullOrEmpty(model.Password))//validate mandatory
                    model.ErrorList.Add(Resources.Administration.PasswordNotFilled);
                if (!string.IsNullOrEmpty(model.Password) && model.Password.Length < 6)//validate lenght
                    model.ErrorList.Add(Resources.Administration.MinLegthPassword);
            }
            
        }
        #endregion

        #region GetBusinessApplication
        /// <summary>
        /// Get the list of business application according with the user role
        /// </summary>
        /// <returns>PartialViewResult</returns>
        public PartialViewResult GetBusinessApplication(bool isPermission,bool isEdition,Guid? businessAppSelected)
        {
            UserModel model = Session["UserModel"] as UserModel;
            model.BusinessApplications = RetrieveBusinessApplications();

            if (isEdition)
            {
                model.BusinessApplicationIdPer = businessAppSelected.GetValueOrDefault();
                model.OpenModePer = (int)ScreenOpenMode.Edit;                
            }
            else
            {
                model.BusinessApplicationIdPer = Guid.Empty;
                model.OpenModePer = (int)ScreenOpenMode.Add;
            }
            ViewBag.IspermissionBusinessApp = isPermission;
            return PartialView("_BusinessApplicationsUser", model);
        }
        #endregion

        #region RetrieveBusinessApplications
        /// <summary>
        /// Retrieve the list of business applications depending of the user type
        /// </summary>
        /// <returns>List of SelectListItem</returns>
        private List<SelectListItem> RetrieveBusinessApplications()
        {
            List<SelectListItem> businessApp = new List<SelectListItem>();
            if (User.IsInRole("GlobalAdministrator"))
            {
                businessApp = AuthorizationBusiness.GetAllBusinessAplications().Select(data => new SelectListItem
                {
                    Text = data.BusinessApplicationName,
                    Value = data.BusinessApplicationId.ToString()
                }).ToList();
            }
            else
            {
                List<BusinessApplicationByUser> businessAplicationsByUser = AuthorizationBusiness.GetBusinessApplicationsByUser(UserName);
                string[] roles = Roles.GetRolesForUser(User.Identity.Name);
                foreach (var item in businessAplicationsByUser)
                {
                    businessApp.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString()});
                }
            }
            return businessApp;
        }
        #endregion

        #region GetRolesByBusinessApplication
        /// <summary>
        /// Get the list of roles and clients
        /// </summary>
        /// <param name="businessApplicationId">Id of business applications</param>
        /// <param name="selectedUserType">User type</param>
        /// <param name="divRolesId">Id div roles</param>
        /// <param name="isPermission">Is permission</param>
        /// <param name="isEditPermission">Is permission edit</param>
        /// <param name="userName">User name</param>
        /// <returns>PartialViewResult</returns>
        public PartialViewResult GetRolesByBusinessApplication(Guid? businessApplicationId, int selectedUserType, string divRolesId, bool isPermission, bool? isEditPermission, string userName)
        {
            UserModel model = Session["UserModel"] as UserModel;
            if (businessApplicationId != null)
            {
                string prefixBusinessApp = AuthorizationBusiness.GetBusinessApplicationById(businessApplicationId.GetValueOrDefault()).Prefix;
                model.Roles = AuthorizationBusiness.GetRolesForBusinessApplication(businessApplicationId.GetValueOrDefault(), prefixBusinessApp);
                model.ClientList = AuthorizationBusiness.GetClientsByBusinessApp(businessApplicationId.GetValueOrDefault());
                model.SelectedUserType = selectedUserType;
                model.BusinessApplicationId = businessApplicationId.GetValueOrDefault();
                model.BusinessApplications = RetrieveBusinessApplications();
                if (isEditPermission.GetValueOrDefault())
                {
                    if (selectedUserType == 3)
                    {
                        Guid? clientId = AuthorizationBusiness.GetClientIdByBusinnessApplication(businessApplicationId.GetValueOrDefault(), userName);
                        model.ClientIdPer = clientId;
                    }
                    else if (selectedUserType == 4)
                    {
                        string prefix = "_";
                        prefix += AuthorizationBusiness.GetAllBusinessAplications().FirstOrDefault(data => data.BusinessApplicationId == businessApplicationId.Value).Prefix;
                        string[] rolesForUser = Roles.GetRolesForUser(userName).Where(role => role.Contains(prefix) || role == "ApplicationAdministrator").ToArray();
                        model.SelectedRoles.Clear();
                        foreach (string role in rolesForUser)
                        {
                            model.SelectedRoles.Add(role, role.Replace(prefix, ""));
                        }
                    }
                }
                else
                {
                    if (selectedUserType == 3)
                        model.ClientIdPer = null;
                    else if(selectedUserType == 4)
                        model.SelectedRoles.Clear();
                }
            }
            ViewBag.IdDivRoles = divRolesId;
            ViewBag.IsPermission = isPermission;
            return PartialView("_RoleClient", model);
        }
        #endregion

        #region EditUser
        /// <summary>
        /// Open the screen User in Edit modes
        /// </summary>
        /// <param name="userNameEdit">User for editing</param>
        /// <returns></returns>
        public ActionResult EditUser(string userNameEdit)
        {
            int userType = int.Parse(AuthorizationBusiness.GetUserParameter(userNameEdit, "UserType"));
            UserModel model = new UserModel();
            model.UserTypes = UserTypes;
            model.OpenMode = (int)ScreenOpenMode.Edit;
            model.SelectedUserType = userType;
            MembershipUser user = Membership.GetUser(userNameEdit);
            model.Email = user.UserName;
            model.BusinessApplications = RetrieveBusinessApplications();
            int pageSize = Cotecna.Vestalis.Web.Properties.Settings.Default.PageSize;

            ParameterSearchPermission parameters = new ParameterSearchPermission()
            {
                PageSize = pageSize,
                SelectedPage = 1,
                SortedColumn = "BusinessApplication",
                SortDirection = SortDirection.Ascending,
                LoginName = userNameEdit,
                IsGlobalAdmin = IsGlobalAdmin,
                LoggedUserName = UserName
            };


            model.PermissionList = AuthorizationBusiness.GetPermissionListByUser(parameters);
            Session.Add("UserModel", model);
            return View("User", model);
        }
        #endregion

        #region SearchPermisssions
        /// <summary>
        /// Perform the search with pagination
        /// </summary>
        /// <param name="sortedColumn">Sorted column</param>
        /// <param name="page">Page</param>
        /// <param name="sortDirection">Sort direction</param>
        /// <returns>PartialViewResult</returns>
        public PartialViewResult SearchPermisssions(SortDirection? sortDirection, string sortedColumn, int? page)
        {
            UserModel userModel = Session["UserModel"] as UserModel;
            int pageSize = Cotecna.Vestalis.Web.Properties.Settings.Default.PageSize;
            if (string.IsNullOrEmpty(sortedColumn))
                sortedColumn = "BusinessApplication";
            if (page == null) page = 1;
            if (sortDirection == null) sortDirection = SortDirection.Ascending;
            
            ParameterSearchPermission parameters = new ParameterSearchPermission()
            {
                PageSize = pageSize,
                SelectedPage = page.GetValueOrDefault(),
                SortedColumn = sortedColumn,
                SortDirection = sortDirection.GetValueOrDefault(),
                LoginName = userModel.Email,
                IsGlobalAdmin = IsGlobalAdmin,
                LoggedUserName = UserName
            };

            PaginatedList<PermissionGridModel> model = AuthorizationBusiness.GetPermissionListByUser(parameters);
            return PartialView("_PermissionGrid", model);
        }
        #endregion

        #region SavePermission
        /// <summary>
        /// Save the permission in the system
        /// </summary>
        /// <param name="selectedRoles">List of chosen roles</param>
        /// <param name="clientId">Id of selected client</param>
        /// <param name="businessAppId">If of selected business application</param>
        /// <param name="businessAppEdit">if of business application to edit</param>
        /// <param name="userType">User type</param>
        /// <param name="userName">E-mail of the user</param>
        /// <param name="openMode">Open mode</param>
        /// <returns>Json result</returns>
        public ActionResult SavePermission(string selectedRoles, Guid? clientId, Guid? businessAppId, Guid? businessAppEdit, int? userType, string userName, int? openMode)
        {
            UserModel model = new UserModel()
            {
                BusinessApplicationId = businessAppId.GetValueOrDefault(),
                BusinessApplicationIdPer = businessAppEdit.GetValueOrDefault(),
                CheckedRoles = selectedRoles,
                ClientId = clientId,
                SelectedUserType = userType.GetValueOrDefault(),
                Email = userName,
                OpenMode = openMode.GetValueOrDefault()
            };
            ValidateSavePermission(model);
            if (!model.HasErrors)
            {
                if (userType.GetValueOrDefault() == 3)
                {
                    AuthorizationBusiness.VerifySaveVestalisUserApplications(model, UserName);
                }
                else if (userType.GetValueOrDefault() == 4)
                {
                    AuthorizationBusiness.VerifySaveVestalisUserApplications(model, UserName);
                    if (!model.HasErrors)
                        AuthorizationBusiness.SaveRolesForCotecnaUser(model);
                }
            }
            return Json(model);
        }
        #endregion

        #region ValidateSavePermission
        /// <summary>
        /// Perform a validation when a permission is saved
        /// </summary>
        /// <param name="model"></param>
        private void ValidateSavePermission(UserModel model)
        {
            if (model.BusinessApplicationId == Guid.Empty)
                model.ErrorList.Add(Resources.Administration.BusinessApplicationNotSelected);

            if (model.SelectedUserType == 3 && model.ClientId == null)
                model.ErrorList.Add(Resources.Administration.ClientNotSelected);
            //validation for selected roles
            else if (model.SelectedUserType == 4 && string.IsNullOrEmpty(model.CheckedRoles))
                model.ErrorList.Add(Resources.Administration.RolesNotSelected);
        }
        #endregion

        #region DeleteUser
        /// <summary>
        /// Perform the delete operation for the user Grid
        /// </summary>
        /// <param name="selectedUserName">Selected user name</param>
        /// <param name="userType">User type</param>
        public ActionResult DeleteUser(string selectedUserName,int userType)
        {
            string result = string.Empty;
            if (selectedUserName != UserName)
            {
                if (IsGlobalAdmin)
                    AuthorizationBusiness.Instance.DeleteUser(selectedUserName, UserName);
                else
                    AuthorizationBusiness.DeletePermissionsLocalAdmin(UserName, selectedUserName, userType);
            }
            else
            {
                result = Resources.Administration.NonDeleteUserMessage;
            }
            return Json(result);
        }
        #endregion

        #region DeleteSelectedUsers
        /// <summary>
        /// Delete all selected users
        /// </summary>
        /// <param name="selectedUsers">Ids of selected users</param>
        /// <returns>ActionResult</returns>
        public ActionResult DeleteSelectedUsers(string selectedUsers)
        {
            string result = string.Empty;
            string[] userNames = selectedUsers.Split(new string[] { "&&&" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string user in userNames)
            {
                int userType = int.Parse(UserProfile.GetUserProfile(user).UserType);
                if (user != UserName)
                {
                    if (IsGlobalAdmin)
                        AuthorizationBusiness.Instance.DeleteUser(user, UserName);
                    else
                        AuthorizationBusiness.DeletePermissionsLocalAdmin(UserName, user, userType);
                }
                else
                {
                    result = string.Format(Resources.Administration.NonDeleteMultipleUseMessage, user);
                }
            }
            return Json(result);
        }
        #endregion

        #region DeletePermission
        /// <summary>
        /// Perform the delete operation in the permission screen
        /// </summary>
        /// <param name="loginName">User name</param>
        /// <param name="businessAppId">Id of business application</param>
        /// <param name="userType">User type</param>
        /// <returns></returns>
        public ActionResult DeletePermission(string loginName, Guid? businessAppId, int? userType)
        {
            string result = string.Empty;
            if(loginName != UserName)
                AuthorizationBusiness.DeletePermission(loginName, businessAppId.GetValueOrDefault(), UserName, userType.GetValueOrDefault());
            else
                result = Resources.Administration.NonDeletePermissionMessage;

            return Json(result);

        }
        #endregion

        #region DeleteSelectedPermissions
        /// <summary>
        /// Perform the delete operation in order to delete all selected items
        /// </summary>
        /// <param name="loginName">The name of the selected user</param>
        /// <param name="selectedPermissions">List of selected permissions</param>
        /// <param name="userType">Id of user type</param>
        /// <returns>ActionResult</returns>
        public ActionResult DeleteSelectedPermissions(string loginName, string selectedPermissions,int? userType)
        {
            List<string> result = new List<string>();
            List<Guid> businessAppIds = new List<Guid>();
            string[] ids = selectedPermissions.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            ids.ToList().ForEach(id => businessAppIds.Add(new Guid(id)));
            
            businessAppIds.ForEach(businessAppId => 
            {
                if (loginName != UserName)
                    AuthorizationBusiness.DeletePermission(loginName, businessAppId, UserName, userType.GetValueOrDefault());
                else
                {
                    string businessAppName = AuthorizationBusiness.GetBusinessApplicationById(businessAppId).BusinessApplicationName;
                    result.Add(string.Format(Administration.NonDeleteMultiplePermissionMessage,businessAppName));
                };
            });
            return Json(result);
        }
        #endregion

        #region ChangePassword
        /// <summary>
        /// Show ChangePassword screen
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangePassword()
        {
            ChangePasswordModel model = new ChangePasswordModel();
            int userType = int.Parse(AuthorizationBusiness.GetUserParameter(UserName, "UserType"));
            model.UserName = UserName;
            model.UserType = userType;
            return View(model);
        }
        #endregion

        #region SaveChangePassword
        /// <summary>
        /// Perform change password operation
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveChangePassword(ChangePasswordModel model)
        {
            if (string.IsNullOrEmpty(model.OldPassword))
                ModelState.AddModelError("OldPassRequired", Resources.Administration.OldPassRequired);
            if (model.OldPassword.Length < AuthorizationBusiness.Instance.MinPasswordLength)
                ModelState.AddModelError("", Resources.Administration.MinLegthPassword);

            if (string.IsNullOrEmpty(model.NewPassword))
                ModelState.AddModelError("NewPassRequired", Resources.Administration.NewPassRequired);
            if (model.NewPassword.Length < AuthorizationBusiness.Instance.MinPasswordLength)
                ModelState.AddModelError("", Resources.Administration.MinLegthPassword);

            if (string.IsNullOrEmpty(model.ReNewPassword))
                ModelState.AddModelError("ReNewPassWordRequired", Resources.Administration.ReNewPassWordRequired);
            if (model.ReNewPassword.Length < AuthorizationBusiness.Instance.MinPasswordLength)
                ModelState.AddModelError("", Resources.Administration.MinLegthPassword);

            if (model.NewPassword != model.ReNewPassword)
                ModelState.AddModelError("", Resources.Administration.NotEqualPassword);

            if (ModelState.IsValid)
            {
                bool result = AuthorizationBusiness.Instance.ChangePassword(model.UserName, model.OldPassword, model.ReNewPassword);

                if (result)
                {
                    List<BusinessApplicationByUser> businessAplicationsByUser = Session["BusinessAplicationsByUser"] as List<BusinessApplicationByUser>;
                    BusinessApplicationByUser applicationByUser = businessAplicationsByUser.FirstOrDefault();
                    if (applicationByUser != null)
                    {
                        int numRoles = Roles.GetRolesForUser(User.Identity.Name).Where(rol => rol.Contains("_" + applicationByUser.Prefix)).ToList().Count;
                        if (numRoles == 1 && (User.IsInRole("GlobalAdministrator") || (User.IsInRole("ApplicationAdministrator_" + applicationByUser.Prefix))))
                        {
                            return RedirectToAction("Index", "Catalogue");
                        }
                        else
                        {
                            return RedirectToAction("Index", "ServiceOrder");
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Catalogue");
                    }
                }
                else
                {
                    ModelState.AddModelError("", Resources.Administration.ChangePasswordError);
                    return View("ChangePassword", model);
                }

            }
            else
            {
                return View("ChangePassword", model);
            }
        }
        #endregion

        #region ExportExcelUsers
        /// <summary>
        /// Perform the search operation for export the result to an excel file
        /// </summary>
        /// <param name="businessAppId">Id of business application</param>
        public void SeachUsersExport(Guid? businessAppId)
        {
            PaginatedList<UserGridModel> model = new PaginatedList<UserGridModel>();
            int pageSize = Cotecna.Vestalis.Web.Properties.Settings.Default.PageSize;

            ParameterSearchUser parameters = new ParameterSearchUser
            {
                BusinessApplicationId = businessAppId.GetValueOrDefault(),
                PageSize = pageSize,
                SortDirection = SortDirection.Ascending,
                SortedColumn = "UserType",
                UserTypes = UserTypes,
                IsExport = true,
                IsGlobalAdmin = IsGlobalAdmin,
                LoggedUserName = UserName
            };
            model = AuthorizationBusiness.Instance.GetUserList(parameters);
            Session.Add("ResultSearchUsers", model);
        }


        /// <summary>
        /// Generate user list report
        /// </summary>
        /// <returns>FileStreamResult</returns>
        [HttpPost]
        public FileStreamResult ExportExcelUsers()
        {
            PaginatedList<UserGridModel> model = Session["ResultSearchUsers"] as PaginatedList<UserGridModel>;
            Session.Remove("ResultSearchUsers");

            string logoPath = Server.MapPath("~/Images/logo.png");

            MemoryStream report = ExcelBusiness.GenerateUserReport(model.Collection, logoPath);
            report.Position = 0;
            string currentDateTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            //download the report
            MicrosoftExcelStreamResult result = new MicrosoftExcelStreamResult(report, "UserReport" + currentDateTime + ".xlsx");

            return result;
        }
        #endregion

        #region ExportExcelPermissions
        /// <summary>
        /// Search all permissions for the selected user
        /// </summary>
        public void SearchPermissionExcel(string userName, int userType)
        {
            Session.Add("userNameAccess", userName);
            Session.Add("userTypeAccess", userType);

            ParameterSearchPermission parameters = new ParameterSearchPermission()
            {
                PageSize = 0,
                SelectedPage = 0,
                SortedColumn = "BusinessApplication",
                SortDirection = SortDirection.Ascending,
                LoginName = userName,
                IsExport = true,
                IsGlobalAdmin = IsGlobalAdmin,
                LoggedUserName = UserName
            };

            PaginatedList<PermissionGridModel> model = AuthorizationBusiness.GetPermissionListByUser(parameters);
            Session.Add("SearchPermissionExcel", model);
        }

        /// <summary>
        /// Generate permission report
        /// </summary>
        /// <returns></returns>
        public FileStreamResult ExportExcelPermissions()
        {
            string userNameAccess = Session["userNameAccess"].ToString();
            int userType = int.Parse(Session["userTypeAccess"].ToString());
            string userTypeName = UserTypes.First(data => data.Key == userType).Value;
            PaginatedList<PermissionGridModel> model = Session["SearchPermissionExcel"] as PaginatedList<PermissionGridModel>;
            Session.Remove("SearchPermissionExcel");
            string logoPath = Server.MapPath("~/Images/logo.png");
            MemoryStream report = ExcelBusiness.GeneratePermissionReport(model.Collection, userNameAccess, userTypeName, logoPath);
            report.Position = 0;
            string currentDateTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            //download the report
            MicrosoftExcelStreamResult result = new MicrosoftExcelStreamResult(report, "PermissionReport" + currentDateTime + ".xlsx");

            return result;
        }
        #endregion

        #region VerifyUser
        /// <summary>
        /// Verify if a user exists
        /// </summary>
        /// <param name="userName">User name</param>
        /// <returns>JSON</returns>
        public ActionResult VerifyUser(string userName)
        {
            bool result = AuthorizationBusiness.Instance.VerifyUser(userName);

            return Json(result);
        }
        #endregion

        #endregion
    }
}
