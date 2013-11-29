using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Profile;
using System.Web.Security;
using System.Web.UI.WebControls;
using Cotecna.Vestalis.Entities;
using Cotecna.Vestalis.Core.Resources;

namespace Cotecna.Vestalis.Core
{
    /// <summary>
    /// This class is used for manage all about the users information and operations
    /// </summary>
    public class AuthorizationBusiness
    {
        #region fields
        private readonly MembershipProvider _provider;
        private static AuthorizationBusiness _instance;
        #endregion

        #region constructor
        public AuthorizationBusiness()
            : this(null)
        {

        }

        public AuthorizationBusiness(MembershipProvider provider)
        {
            _provider = provider ?? Membership.Provider;
        }
        #endregion

        #region properties
        /// <summary>
        /// Get an instance of the class AuthorizationBusiness
        /// </summary>
        public static AuthorizationBusiness Instance 
        {
            get 
            {
                return _instance == null ? _instance = new AuthorizationBusiness() : _instance;
            } 
        }

        /// <summary>
        /// Get the minimum password length
        /// </summary>
        public int MinPasswordLength 
        { 
            get 
            {
                return _provider.MinRequiredPasswordLength;
            } 
        }
        #endregion

        #region methods

        #region GetBusinessApplicationsByUser
        /// <summary>
        /// Get the list of business application by the user
        /// </summary>
        /// <param name="userName">Name of the user</param>
        /// <returns>List of BusinessApplicationByUser</returns>
        public static List<BusinessApplicationByUser> GetBusinessApplicationsByUser(string userName)
        {
            List<BusinessApplicationByUser> response = null;
            using (VestalisEntities context = new VestalisEntities())
            {
                //Get the list of business applications by user
                response = (from businessApplication in context.BusinessApplications
                            join vestalisUserApplication in context.VestalisUserApplications
                                on businessApplication.BusinessApplicationId equals
                                vestalisUserApplication.BusinessApplicationId
                            where vestalisUserApplication.UserName == userName &&
                                  vestalisUserApplication.IsDeleted == false &&
                                  businessApplication.IsDeleted == false
                            select new BusinessApplicationByUser
                                       {
                                           LanguageCode = businessApplication.Country.DefaultLanguage,
                                           Name = businessApplication.BusinessApplicationName,
                                           Id = businessApplication.BusinessApplicationId,
                                           Prefix = businessApplication.Prefix
                                       }).ToList();
            }
            return response;
        }
        #endregion

        #region SearchUsers
        /// <summary>
        /// Search users by a business application id and role name
        /// </summary>
        /// <param name="businessAplicationId">id of business application</param>
        /// <param name="roleName">The name of the role</param>
        /// <returns>List of UserInformation</returns>
        public List<UserInformation> SearchUsers(Guid businessAplicationId, string roleName)
        {

            List<UserInformation> result = new List<UserInformation>();
            List<string> usersByBusinessApplication = null;
            using (VestalisEntities context = new VestalisEntities())
            {
                usersByBusinessApplication = context.VestalisUserApplications
                    .Where(data => data.BusinessApplicationId == businessAplicationId)
                    .Select(data => data.UserName).Distinct().ToList();

                var membershipUsers = (from membershipUser in context.aspnet_Users
                                       select membershipUser).ToList();
                if (membershipUsers.Count == 0) throw new IndexOutOfRangeException(LanguageResource.UsersNotFound);

                var filteredUsers = membershipUsers.ToList().Where(data => usersByBusinessApplication.Contains(data.UserName)).ToList();


                usersByBusinessApplication.ForEach(user =>
                {
                    if (filteredUsers.Any(data => data.UserName == user))
                    {
                        if (Roles.IsUserInRole(user, roleName))
                        {
                            var fullname = AuthorizationBusiness.GetUserParameter(user, "UserFullName");
                            result.Add(new UserInformation { FullName = fullname, UserName = user });
                        }
                    }
                });
            }

            return result;
        }
        #endregion

        #region LogOn
        /// <summary>
        /// Perform the logOn into system
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="password">Password</param>
        /// <returns>bool</returns>
        public bool LogOn(string userName, string password)
        {
            bool response = false;

            if (String.IsNullOrEmpty(userName))
                throw new ArgumentException(LanguageResource.NullOrEmptyValue, "userName");
            if (String.IsNullOrEmpty(password))
                throw new ArgumentException(LanguageResource.NullOrEmptyValue, "password");

            response = _provider.ValidateUser(userName, password);
            return response;
        }
        #endregion

        #region ChangePassword
        /// <summary>
        /// Change the password of the current user
        /// </summary>
        /// <param name="userName">name of the user</param>
        /// <param name="oldPassword">old password</param>
        /// <param name="newPassword">new password</param>
        /// <returns>bool</returns>
        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            bool response = false;

            // The underlying ChangePassword() will throw an exception rather
            // than return false in certain failure scenarios.

            MembershipUser currentUser = _provider.GetUser(userName, true /* userIsOnline */);
            response = currentUser.ChangePassword(oldPassword, newPassword);
            return response;
        }
        #endregion

        #region GetUserParameter
        /// <summary>
        /// Get a profile of the user
        /// </summary>
        /// <param name="userName">The name of the user</param>
        /// <param name="parameterName">Profile name</param>
        /// <returns>Profile value</returns>
        public static string GetUserParameter(string userName, string parameterName)
        {
            //Get the complete profile of the user
            UserProfile profileBase = UserProfile.GetUserProfile(userName);
            if (profileBase != null)
                //retrieve the specific value of a property
                return profileBase.GetPropertyValue(parameterName).ToString();
            else
                return string.Empty;
        }
        #endregion

        #region GetUserList
        /// <summary>
        /// Get the list of user and by business applications
        /// </summary>
        /// <param name="parameters">The list of parameters</param>
        /// <returns></returns>
        public PaginatedList<UserGridModel> GetUserList(ParameterSearchUser parameters)
        {
            PaginatedList<UserGridModel> finalResult = new PaginatedList<UserGridModel>();
            List<UserGridModel> tempResult = new List<UserGridModel>();
            UserGridModel model = null;
            int currentIndex = (parameters.SelectedPage - 1) * parameters.PageSize;
            List<string> queryUsers = null;
            using (VestalisEntities context = new VestalisEntities())
            {
                queryUsers = GetUserQuery(parameters.BusinessApplicationId, context);

                if (queryUsers != null)
                {
                    tempResult = GetResultUserQuery(tempResult, model, queryUsers,parameters, context);
                }
                if (tempResult.Count > 0)
                {
                    finalResult.SortDirection = parameters.SortDirection;
                    finalResult.SortedColumn = parameters.SortedColumn;

                    //order the result
                    tempResult = (parameters.SortDirection == SortDirection.Ascending
                                           ? tempResult.OrderBy(ExtensionMethods.GetField<UserGridModel>(parameters.SortedColumn))
                                           : tempResult.OrderByDescending(ExtensionMethods.GetField<UserGridModel>(parameters.SortedColumn))).ToList();

                    //set the paginated colletion
                    if (!parameters.IsExport)
                        finalResult.Collection = tempResult.Skip(currentIndex).Take(parameters.PageSize).ToList();
                    else
                        finalResult.Collection = tempResult;

                    //set the quantity of elements without pagination
                    finalResult.TotalCount = tempResult.Count;
                    //set the number of pages
                    finalResult.NumberOfPages = (int)Math.Ceiling((double)finalResult.TotalCount / (double)parameters.PageSize);
                    //set the current page
                    finalResult.Page = parameters.SelectedPage;
                    //set the page size
                    finalResult.PageSize = parameters.PageSize;
                }

            }
            return finalResult;
        }

        /// <summary>
        /// Get the result of the query
        /// </summary>
        /// <param name="tempResult">Temporal result</param>
        /// <param name="model">Model</param>
        /// <param name="queryUsers">User list</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="context">Database context</param>
        /// <returns></returns>
        private static List<UserGridModel> GetResultUserQuery(List<UserGridModel> tempResult, UserGridModel model, List<string> queryUsers, ParameterSearchUser parameters, VestalisEntities context)
        {
            string businessApps = string.Empty;
            List<string> businessApplicationsLocalAdmin = new List<string>();
            List<string> queryBusinessAppByUser = new List<string>();
            List<BusinessApplication> businessAppUserLocal = null;

            if (!parameters.IsGlobalAdmin)
            {
                businessAppUserLocal = GetBusinessAppLocalAdminLogged(parameters.LoggedUserName, context);
                string[] rolesforUser = Roles.GetRolesForUser(parameters.LoggedUserName);
                foreach (var item in businessAppUserLocal)
                {
                    string localAdminRoles = string.Format("ApplicationAdministrator_{0}", item.Prefix);
                    if (rolesforUser.Contains(localAdminRoles))
                        businessApplicationsLocalAdmin.Add(item.BusinessApplicationName);
                }
            }

            foreach (string user in queryUsers)
            {
                if (!Roles.IsUserInRole(user, "GlobalAdministrator"))
                {
                    if (!parameters.IsGlobalAdmin)
                    {
                        queryBusinessAppByUser = (from vestalisBusinessApp in context.VestalisUserApplications
                                                  join businessApplication in context.BusinessApplications on vestalisBusinessApp.BusinessApplicationId equals businessApplication.BusinessApplicationId
                                                  where vestalisBusinessApp.IsDeleted == false && businessApplication.IsDeleted == false
                                                  && vestalisBusinessApp.UserName == user 
                                                  && businessApplicationsLocalAdmin.Contains(businessApplication.BusinessApplicationName)
                                                  orderby businessApplication.BusinessApplicationName
                                                  select businessApplication.BusinessApplicationName).ToList();
                    }
                    else
                    {
                        queryBusinessAppByUser = (from vestalisBusinessApp in context.VestalisUserApplications
                                                  join businessApplication in context.BusinessApplications on vestalisBusinessApp.BusinessApplicationId equals businessApplication.BusinessApplicationId
                                                  where vestalisBusinessApp.IsDeleted == false && businessApplication.IsDeleted == false
                                                  && vestalisBusinessApp.UserName == user
                                                  orderby businessApplication.BusinessApplicationName
                                                  select businessApplication.BusinessApplicationName).ToList();
                    }

                    string roles = string.Empty;
                    foreach (var item in queryBusinessAppByUser)
                    {
                        roles = GetRolesOfBusinessApplicationByUser(context, item, user);
                        if (item != queryBusinessAppByUser.Last())
                            businessApps += string.Format("{0} ({1})",item,roles) + " | ";
                        else
                            businessApps += string.Format("{0} ({1})", item, roles);
                    }
                }
                else
                {
                    businessApps = LanguageResource.All;
                }
                string obtainedUserType = GetUserParameter(user, "UserType");
                int userType = string.IsNullOrEmpty(obtainedUserType) ? 0 : int.Parse(obtainedUserType);

                if (!parameters.IsGlobalAdmin)
                {
                    if (userType != 1 && queryBusinessAppByUser.Count > 0)
                    {
                        model = new UserGridModel
                        {
                            Email = user,
                            UserType = parameters.UserTypes.FirstOrDefault(data => data.Key == userType).Value,
                            BusinessApplications = businessApps
                        };
                        tempResult.Add(model);
                    }
                }
                else
                {
                    model = new UserGridModel
                    {
                        Email = user,
                        UserType = parameters.UserTypes.FirstOrDefault(data => data.Key == userType).Value,
                        BusinessApplications = businessApps
                    };
                    tempResult.Add(model);
                }
                
                
                businessApps = string.Empty;
            }

            return tempResult;
        }

        /// <summary>
        /// Retrieve the list of business application for a local administrator
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="context">Database context</param>
        /// <returns>List of business application</returns>
        private static List<BusinessApplication> GetBusinessAppLocalAdminLogged(string userName, VestalisEntities context)
        {
            List<BusinessApplication> result = new List<BusinessApplication>();

            result = (from businessApp in context.VestalisUserApplications
                      where businessApp.IsDeleted == false
                      && businessApp.UserName == userName
                      orderby businessApp.BusinessApplication.BusinessApplicationName
                      select businessApp.BusinessApplication).Distinct().ToList();

            return result;
        }


        /// <summary>
        /// Get the assigned roles in a business application
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="businessApplicationName">Business application name</param>
        /// <param name="userName">User name</param>
        /// <returns>string</returns>
        private static string GetRolesOfBusinessApplicationByUser(VestalisEntities context,string businessApplicationName, string userName)
        {
            string selectedRoles = string.Empty;
            string prefix = "_";

            prefix += context.BusinessApplications.FirstOrDefault(data => data.BusinessApplicationName == businessApplicationName).Prefix;

            var rolesByBusinessApp = (from role in context.aspnet_Roles
                                      where role.RoleName.Contains(prefix)
                                      select role.RoleName).ToList();

            string[] rolesForUser = Roles.GetRolesForUser(userName);

            if (rolesByBusinessApp != null)
            {

                var finalRoles = rolesByBusinessApp.Where(data => rolesForUser.Contains(data)).ToList();
                if (finalRoles.Count > 0)
                {
                    if (rolesForUser.Contains("ApplicationAdministrator" + prefix))
                    {
                        selectedRoles += "Application administrator";
                        finalRoles.Remove("ApplicationAdministrator" + prefix);
                    }
                    
                    foreach (var role in finalRoles)
                    {
                        selectedRoles += ", " + role.Replace(prefix, "");
                    }
                    if (selectedRoles.First() == ',')
                    {
                        selectedRoles = selectedRoles.Remove(0, 2);
                    }
                }
                else if (finalRoles.Count == 0 && rolesForUser.Count() > 0)
                {
                    foreach (var role in rolesForUser)
                    {
                        selectedRoles += role;
                    }
                }

            }

            return selectedRoles;
        }

        /// <summary>
        /// Get the list of users
        /// </summary>
        /// <param name="businessApplicationId">Id of business application</param>
        /// <param name="context">Database context</param>
        /// <returns></returns>
        private List<string> GetUserQuery(Guid businessApplicationId,  VestalisEntities context)
        {
            List<string> queryUsers = null;
            queryUsers = businessApplicationId == Guid.Empty ? GetAllQueryUsers(context) : GetFilteredQueryUsers(businessApplicationId, context);
            return queryUsers;
        }

        /// <summary>
        /// Get the complete list of users by business application
        /// </summary>
        /// <param name="businessApplicationId">Business application id</param>
        /// <param name="context">DataBase context</param>
        /// <returns></returns>
        private List<string> GetFilteredQueryUsers(Guid businessApplicationId, VestalisEntities context)
        {
            List<string> queryUsers;
            //get the list of users by business application
            queryUsers = (from user in context.VestalisUserApplications
                          where user.IsDeleted == false && user.BusinessApplicationId == businessApplicationId
                          orderby user.UserName
                          select user.UserName).ToList();


            int totalRecords;
            //retrieve the list of membership users
            List<MembershipUser> allUsers = _provider.GetAllUsers(0, int.MaxValue, out totalRecords).ToList();

            foreach (MembershipUser user in allUsers)
            {
                if (!queryUsers.Contains(user.UserName))
                {
                    //add global administrator users
                    bool isGlobaladmin = Roles.IsUserInRole(user.UserName, "GlobalAdministrator");
                    if (isGlobaladmin)
                    {
                        queryUsers.Add(user.UserName);
                    }
                }
            }
            return queryUsers;
        }

        /// <summary>
        /// Gte the list of users without filters
        /// </summary>
        /// <param name="context">Database context</param>
        /// <returns></returns>
        private List<string> GetAllQueryUsers(VestalisEntities context)
        {
            List<string> queryUsers = null;
            //get the complete list of users
            queryUsers = (from user in context.VestalisUserApplications
                          where user.IsDeleted == false
                          orderby user.UserName
                          select user.UserName).Distinct().ToList();

            int totalRecords = 0;
            //retrieves the user in the system.
            List<MembershipUser> allUsers = _provider.GetAllUsers(0, int.MaxValue, out totalRecords).ToList();

            foreach (MembershipUser user in allUsers)
            {
                //verify and add the users that are not exists in the last array
                if (!queryUsers.Contains(user.UserName))
                {
                    queryUsers.Add(user.UserName);
                }
            }
            return queryUsers;
        }

        #endregion

        #region GetAllBusinessAplications
        /// <summary>
        /// Get the list of all business applications
        /// </summary>
        /// <returns></returns>
        public static List<BusinessApplication> GetAllBusinessAplications()
        {
            List<BusinessApplication> result = null;
            using (VestalisEntities context = new VestalisEntities())
            {
                result = context.BusinessApplications.Where(data => data.IsDeleted == false).ToList();
            }
            return result;
        }
        #endregion

        #region GetBusinessApplicationById
        /// <summary>
        /// Get a business application by id
        /// </summary>
        /// <param name="businessApplicationId">Id of business application</param>
        /// <returns>BusinessApplication</returns>
        public static BusinessApplication GetBusinessApplicationById(Guid businessApplicationId)
        {
            using (VestalisEntities context=new VestalisEntities())
            {
                return (context.BusinessApplications.FirstOrDefault(data => data.BusinessApplicationId == businessApplicationId));
            }
        }
        #endregion

        #region GetClientIdByBusinnessApplication
        /// <summary>
        /// Get the id of the client in the current business application
        /// </summary>
        /// <param name="businessApplicationId">Id of the business application</param>
        /// <param name="userName">Name or email of the user</param>
        /// <returns>Guid nullable</returns>
        public static Guid? GetClientIdByBusinnessApplication(Guid businessApplicationId,string userName)
        {
            Guid? clientId = null;
            using (VestalisEntities context=new VestalisEntities())
            {
                clientId = context.VestalisUserApplications.Where(data => data.BusinessApplicationId == businessApplicationId
                    && data.UserName == userName
                    && data.IsDeleted == false).FirstOrDefault().ClientId;
            }
            return clientId;
        }
        #endregion

        #region SaveUser
        /// <summary>
        /// Create a new user for the system
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns>MembershipCreateStatus</returns>
        public MembershipCreateStatus SaveUser(UserModel model)
        {
            MembershipCreateStatus status;

            _provider.CreateUser(model.Email, model.Password, model.Email, null, null, true, null, out status);
            if (status == MembershipCreateStatus.Success)
            {
                UserProfile profile = UserProfile.GetUserProfile(model.Email);
                profile.UserType = model.SelectedUserType.ToString();
                profile.Save();
                if (model.SelectedUserType > 0 && model.SelectedUserType != 4)
                {
                    switch (model.SelectedUserType)
                    {
                        case 1:
                            Roles.AddUserToRole(model.Email, "GlobalAdministrator");
                            break;
                        case 3:
                            Roles.AddUserToRole(model.Email, "Client");
                            break;
                    }
                }
            }
            return status;
        }
        #endregion

        #region VerifySaveVestalisUserApplications
        /// <summary>
        /// Verify and save VestalisUserApplications
        /// </summary>
        /// <param name="model">Model</param>
        /// <param name="loggedUser">Logged user</param>
        public static void VerifySaveVestalisUserApplications(UserModel model, string loggedUser)
        {
            using (VestalisEntities context = new VestalisEntities())
            {
                //if the selected business application is already added to the user, the system will show an error message
                //otherwise the system continues with the process
                bool exists = (from userApp in context.VestalisUserApplications
                               where userApp.IsDeleted == false && userApp.UserName == model.Email
                               && userApp.BusinessApplicationId == model.BusinessApplicationId
                               select userApp).Any();

                if (model.OpenMode == (int)ScreenOpenMode.Add)
                {
                    if (!exists)
                    {
                        VestalisUserApplication userApplication = new VestalisUserApplication
                        {
                            BusinessApplicationId = model.BusinessApplicationId,
                            UserName = model.Email,
                            ClientId = model.ClientId,
                            CreationBy = loggedUser,
                            CreationDate = DateTime.UtcNow
                        };

                        context.VestalisUserApplications.AddObject(userApplication);
                        context.SaveChanges();
                    }
                    else
                    {
                        model.ErrorList.Add(LanguageResource.UserInBusinessApplication);
                    }
                }
                else if (model.OpenMode == (int)ScreenOpenMode.Edit)
                {
                    UpdateBusinessApplicationUser(model, loggedUser, context, exists);

                }

            }
        }

        /// <summary>
        /// Perform the update operaton when a permission is updated
        /// </summary>
        /// <param name="model">User model</param>
        /// <param name="loggedUser">Logged user</param>
        /// <param name="context">Database context</param>
        /// <param name="exists">Exist or not exist the user</param>
        private static void UpdateBusinessApplicationUser(UserModel model, string loggedUser, VestalisEntities context, bool exists)
        {
            VestalisUserApplication userApplication = (from userApp in context.VestalisUserApplications
                                                       where userApp.IsDeleted == false && userApp.UserName == model.Email
                                                       && userApp.BusinessApplicationId == model.BusinessApplicationId
                                                       select userApp).FirstOrDefault();

            if (userApplication != null && userApplication.BusinessApplicationId == model.BusinessApplicationIdPer)
            {
                userApplication.BusinessApplicationId = model.BusinessApplicationIdPer;
                userApplication.ClientId = model.ClientId;
                userApplication.ModificationBy = loggedUser;
                userApplication.CreationDate = DateTime.UtcNow;

                context.SaveChanges();
            }
            else if (userApplication != null && userApplication.BusinessApplicationId != model.BusinessApplicationIdPer)
            {
                if (!exists)
                {
                    userApplication.BusinessApplicationId = model.BusinessApplicationIdPer;
                    userApplication.ClientId = model.ClientId;
                    userApplication.ModificationBy = loggedUser;
                    userApplication.CreationDate = DateTime.UtcNow;

                    context.SaveChanges();
                }
                else
                {
                    model.ErrorList.Add(LanguageResource.UserInBusinessApplication);
                }
            }
        }
        #endregion

        #region SaveRolesForCotecnaUser
        /// <summary>
        /// Add the selected roles for the user
        /// </summary>
        /// <param name="model">UserModel</param>
        public static void SaveRolesForCotecnaUser(UserModel model)
        {
            string[] selectedRoles = model.CheckedRoles.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            if (model.OpenMode == (int)ScreenOpenMode.Add)
            {
                foreach (string role in selectedRoles)
                {
                    try
                    {
                        Roles.AddUserToRole(model.Email, role);
                    }
                    catch (Exception)
                    {
                        model.ErrorList.Add(string.Format(LanguageResource.UserExistInRole, role));
                    }
                    if (model.HasErrors)
                    {
                        Roles.RemoveUserFromRoles(model.Email, selectedRoles);
                    }
                }
            }
            else if (model.OpenMode == (int)ScreenOpenMode.Edit)
            {
                string[] currentBusinessAppRoles = Roles.GetRolesForUser(model.Email);
                string prefix = "_";
                prefix += GetAllBusinessAplications().FirstOrDefault(data => data.BusinessApplicationId == model.BusinessApplicationId).Prefix;
                currentBusinessAppRoles = currentBusinessAppRoles.Where(data => data.Contains(prefix)).ToArray();
                Roles.RemoveUserFromRoles(model.Email, currentBusinessAppRoles);
                foreach (string role in selectedRoles)
                {
                    Roles.AddUserToRole(model.Email, role);
                }
            }

        }
        #endregion

        #region EditUser
        /// <summary>
        /// Edit the information and password of a user
        /// </summary>
        /// <param name="model">Model</param>
        public void EditUser(UserModel model)
        {
            MembershipUser currentUser = _provider.GetUser(model.Email, true);
            currentUser.Email = model.Email;
            _provider.UpdateUser(currentUser);

            if (!string.IsNullOrEmpty(model.Password))
            {
                string tempPassword = currentUser.ResetPassword();

                _provider.ChangePassword(model.Email, tempPassword, model.Password);
            }
        }
        #endregion

        #region GetRolesForBusinessApplication
        /// <summary>
        /// Get the list of roles by business application id
        /// </summary>
        /// <param name="businessApplicationId">Id of business application</param>
        /// <param name="prefix">Prefix of business application</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetRolesForBusinessApplication(Guid businessApplicationId, string prefix)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            List<string> tempresult = new List<string>();
            prefix = "_" + prefix;
            //get the roles from data base
            using (VestalisEntities context = new VestalisEntities())
            {
                tempresult = (from role in context.aspnet_Roles
                              where role.RoleName.Contains(prefix)
                              select role.RoleName).ToList();
            }

            //set the prefix to the roles
            tempresult.ForEach(data =>
            {
                if (data.Contains("ApplicationAdministrator"))
                    result.Add(data, LanguageResource.ApplicationAdministrator);
                else
                    result.Add(data, data.Replace(prefix, ""));
                
            });
            return result;
        }
        #endregion

        #region GetClientsByBusinessApp
        /// <summary>
        /// Get the list of clients by business application id
        /// </summary>
        /// <param name="businessApplicationId">Id of busines applications</param>
        /// <returns></returns>
        public static List<CatalogueValue> GetClientsByBusinessApp(Guid businessApplicationId)
        {
            List<CatalogueValue> clients = new List<CatalogueValue>();
            using (VestalisEntities context = new VestalisEntities())
            {
                //Get the the list of catalogues from database
                clients = (from catalogue in context.Catalogues
                           join catalogueValue in context.CatalogueValues on catalogue.CatalogueId equals catalogueValue.CatalogueId
                           where catalogue.IsDeleted == false && catalogueValue.IsDeleted == false
                           && catalogue.BusinessApplicationId == businessApplicationId && catalogue.CatalogueCategoryName == "Client"
                           select catalogueValue).ToList();
            }
            return clients;
        }
        #endregion

        #region GetPermissionListByUser
        /// <summary>
        /// Get the list of business applications and roles by each business applications
        /// </summary>
        /// <returns></returns>
        public static PaginatedList<PermissionGridModel> GetPermissionListByUser(ParameterSearchPermission parameters)
        {
            PaginatedList<PermissionGridModel> result = new PaginatedList<PermissionGridModel>();
            List<PermissionGridModel> tempResult = new List<PermissionGridModel>();
            PermissionGridModel itemGrid = null;
            string rolesObtaines = string.Empty;
            string prefix = "_";
            int currentIndex = (parameters.SelectedPage - 1) * parameters.PageSize;
            List<Guid?> businessAppAdminLocal = null;
            using (VestalisEntities context = new VestalisEntities())
            {
                string[] rolesForUser = Roles.GetRolesForUser(parameters.LoginName);
                if (!parameters.IsGlobalAdmin)
                    businessAppAdminLocal = GetBusinessAppLocalAdminLogged(parameters.LoggedUserName,context).Select(data => data.BusinessApplicationId).Cast<Guid?>().ToList();
                List<VestalisUserApplication> businessAppListByUser = null;

                if (parameters.IsGlobalAdmin)
                {
                    businessAppListByUser = (from userApp in context.VestalisUserApplications.Include("BusinessApplication")
                                             where userApp.IsDeleted == false && userApp.UserName == parameters.LoginName
                                             select userApp).Distinct().ToList();
                }
                else
                {
                    businessAppListByUser = (from userApp in context.VestalisUserApplications.Include("BusinessApplication")
                                             where userApp.IsDeleted == false && userApp.UserName == parameters.LoginName
                                             && businessAppAdminLocal.Contains(userApp.BusinessApplicationId)
                                             select userApp).Distinct().ToList();
                }

                if (businessAppListByUser != null)
                {
                    foreach (var item in businessAppListByUser)
                    {

                        prefix += context.BusinessApplications.FirstOrDefault(data => data.BusinessApplicationId == item.BusinessApplicationId.Value).Prefix;

                        var rolesByBusinessApp = (from role in context.aspnet_Roles
                                                  where role.RoleName.Contains(prefix)
                                                  select role.RoleName).ToList();

                        if (rolesByBusinessApp != null)
                        {

                            var finalRoles = rolesByBusinessApp.Where(data => rolesForUser.Contains(data)).ToList();
                            if (finalRoles.Count > 0)
                            {
                                if (rolesForUser.Contains("ApplicationAdministrator" + prefix))
                                {
                                    rolesObtaines += LanguageResource.ApplicationAdministrator;
                                    finalRoles.Remove("ApplicationAdministrator" + prefix);
                                }
                                foreach (var role in finalRoles)
                                {
                                    rolesObtaines += ", " + role.Replace(prefix, "");
                                }
                                if (rolesObtaines.First() == ',')
                                {
                                    rolesObtaines = rolesObtaines.Remove(0, 2);
                                }
                            }
                            else if (finalRoles.Count == 0 && rolesForUser.Count() > 0)
                            {
                                foreach (var role in rolesForUser)
                                {
                                    if (role == "Client")
                                    {
                                        Guid? businessAppId = GetClientIdByBusinnessApplication(item.BusinessApplication.BusinessApplicationId, parameters.LoginName);
                                        string clientName = CatalogueBusiness.GetCatalogueValue(businessAppId.GetValueOrDefault()).CatalogueValueData;
                                        rolesObtaines += string.Format("{0} ({1})", role, clientName);
                                    }
                                    else
                                        rolesObtaines += role;
                                }
                            }
                            itemGrid = new PermissionGridModel
                            {
                                BusinessApplication = item.BusinessApplication.BusinessApplicationName,
                                BusinessApplictionId = item.BusinessApplication.BusinessApplicationId,
                                Roles = rolesObtaines,
                            };
                            tempResult.Add(itemGrid);
                            rolesObtaines = string.Empty;
                            prefix = "_";
                        }
                    }
                }


                if (tempResult.Count > 0)
                {

                    result.SortDirection = parameters.SortDirection;
                    result.SortedColumn = parameters.SortedColumn;

                    //order the result
                    tempResult = (parameters.SortDirection == SortDirection.Ascending
                                           ? tempResult.OrderBy(ExtensionMethods.GetField<PermissionGridModel>(parameters.SortedColumn))
                                           : tempResult.OrderByDescending(ExtensionMethods.GetField<PermissionGridModel>(parameters.SortedColumn))).ToList();

                    //set the paginated colletion
                    if (!parameters.IsExport)
                        result.Collection = tempResult.Skip(currentIndex).Take(parameters.PageSize).ToList();
                    else
                        result.Collection = tempResult;

                    //set the quantity of elements without pagination
                    result.TotalCount = tempResult.Count;
                    //set the number of pages
                    result.NumberOfPages = (int)Math.Ceiling((double)result.TotalCount / (double)parameters.PageSize);
                    //set the current page
                    result.Page = parameters.SelectedPage;
                    //set the page size
                    result.PageSize = parameters.PageSize;
                }


            }
            return result;
        }
        #endregion

        #region DeleteUser
        /// <summary>
        /// Delete a user from the system
        /// </summary>
        /// <param name="userName">Selected user name for deleting</param>
        /// <param name="loggedUserName">Logged user name</param>
        public void DeleteUser(string userName, string loggedUserName)
        {
            using (VestalisEntities context = new VestalisEntities())
            {
                //Get the list of business application attached to the user
                var businessAppList = (from businessApp in context.VestalisUserApplications
                                       where businessApp.IsDeleted == false && businessApp.UserName == userName
                                       select businessApp).ToList();

                foreach (var item in businessAppList)
                {
                    //delete all business applications
                    item.IsDeleted = true;
                    item.ModificationBy = loggedUserName;
                    item.ModificationDate = DateTime.UtcNow;
                }

                //get all roles of the user
                var rolesFromUser = Roles.GetRolesForUser(userName);

                //remove the roles
                if (rolesFromUser.Length > 0)
                    Roles.RemoveUserFromRoles(userName, rolesFromUser);

                //delete user from membership table
                var memberships = (from user in context.aspnet_Users
                                   join membership in context.aspnet_Membership on user.UserId equals membership.UserId
                                   where user.UserName == userName
                                   select membership).ToList();
                foreach (var item in memberships)
                {
                    context.aspnet_Membership.DeleteObject(item);
                }

                //delete profiles of the user
                var profiles = (from user in context.aspnet_Users
                                join profile in context.aspnet_Profile on user.UserId equals profile.UserId
                                where user.UserName == userName
                                select profile).ToList();
                foreach (var item in profiles)
                {
                    context.aspnet_Profile.DeleteObject(item);
                }

                //delete the user
                var userToDelete = (from user in context.aspnet_Users
                                    where user.UserName == userName
                                    select user).ToList();
                foreach (var item in userToDelete)
                {
                    context.aspnet_Users.DeleteObject(item);
                }

                context.SaveChanges();
            }
        }
        #endregion

        #region DeletePermissionsLocalAdmin
        /// <summary>
        /// Delete a user when is a local admin
        /// </summary>
        /// <param name="localAdminName">Local admin name</param>
        /// <param name="userName">Selected user to delete</param>
        /// <param name="userType">User type</param>
        public static void DeletePermissionsLocalAdmin(string localAdminName, string userName,int userType)
        {
            using (VestalisEntities context=new VestalisEntities())
            {
                List<Guid> businessAppLocalAdmin = GetBusinessAppLocalAdminLogged(localAdminName, context).Select(data => data.BusinessApplicationId).ToList();
                foreach (Guid item in businessAppLocalAdmin)
                {
                    DeletePermission(userName, item, localAdminName, userType);
                }
            }
        }
        #endregion

        #region DeletePermission

        /// <summary>
        /// Delete the permission for a business application
        /// </summary>
        /// <param name="userName">Selected user name</param>
        /// <param name="businessApplicationId">Id of business application</param>
        /// <param name="loggedUserName">Logged user</param>
        /// <param name="userType">Type of user</param>
        public static void DeletePermission(string userName, Guid businessApplicationId, string loggedUserName, int userType)
        {
            using (VestalisEntities context = new VestalisEntities())
            {
                //get the current business application to delete
                var currentBusinessApp = (from businessApp in context.VestalisUserApplications
                                          where businessApp.IsDeleted == false && businessApp.UserName == userName
                                          && businessApp.BusinessApplicationId == businessApplicationId
                                          select businessApp).FirstOrDefault();

                if (currentBusinessApp != null)
                {
                    //delete the business application
                    currentBusinessApp.IsDeleted = true;
                    currentBusinessApp.ModificationBy = loggedUserName;
                    currentBusinessApp.ModificationDate = DateTime.UtcNow;

                    context.SaveChanges();

                    //delete the permissions for the business applications
                    if (userType > 3)
                    {
                        string prefix = "_";
                        prefix += GetAllBusinessAplications().FirstOrDefault(data => data.BusinessApplicationId == businessApplicationId).Prefix;
                        string[] currentRoles = Roles.GetRolesForUser(userName).Where(role => role.Contains(prefix)).ToArray();
                        Roles.RemoveUserFromRoles(userName, currentRoles);
                    }
                }
            }
        }
        #endregion

        #region VerifyResetPassword
        /// <summary>
        /// Verify if is valid the current url
        /// </summary>
        /// <param name="sentTime">Change password date</param>
        /// <param name="userName">User name</param>
        /// <returns>bool</returns>
        public static bool VerifyResetPassword(DateTime sentTime, string userName)
        {
            bool result = false;
            using (VestalisEntities context = new VestalisEntities())
            {
                var query = (from membership in context.aspnet_Membership
                             join user in context.aspnet_Users on membership.UserId equals user.UserId
                             where user.UserName == userName
                             select membership).FirstOrDefault();

                TimeSpan wasChangedAlready = sentTime.ToUniversalTime() - query.LastPasswordChangedDate;
                result = wasChangedAlready.TotalDays < 0;
            }
            return result;
        }
        #endregion

        #region GetSaveTemporalPassword
        /// <summary>
        /// Get a temporal password that is required in the option "Reset password"
        /// </summary>
        /// <param name="currentUser">Curren tUser</param>
        /// <returns>Temporal password to be include in the mail</returns>
        public string GetSaveTemporalPassword(MembershipUser currentUser)
        {
            //Generate a temporal password
            string tempPassword = Membership.GeneratePassword(12, 1);
            //Encrypt the temporal password
            currentUser.Comment = EncryptionHelper.EncryptAes(tempPassword);
            _provider.UpdateUser(currentUser);
            return tempPassword;
        }
        #endregion

        #region CompareTemporalPassword
        /// <summary>
        /// Compare the temporal password entered by the user with the one saved in the database
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="temporalPassword">Temporal password entered by the user</param>
        /// <returns>True when it is the same than the one of the database; otherwise false</returns>
        public bool CompareTemporalPassword(string userName, string temporalPassword)
        {
            MembershipUser currentUser = _provider.GetUser(userName, true /* userIsOnline */);
            string tempPasswordSaved = EncryptionHelper.DecryptAes(currentUser.Comment);
            if (String.Equals(tempPasswordSaved, temporalPassword, StringComparison.Ordinal))
                return true;
            else
                return false;
        }
        #endregion

        #region ResetPassword
        /// <summary>
        /// Resets the user's password using the secret answer.
        /// </summary>
        /// <param name="userName">The user's clientEmail.</param>
        /// <param name="newPassword">The new password defined by the user.</param>
        /// <returns>True is the password has been successfully reset.</returns>
        public bool ResetPassword(string userName, string newPassword)
        {
            MembershipUser currentUser = _provider.GetUser(userName, true /* userIsOnline */);
            string tempPassword = currentUser.ResetPassword();
            return currentUser.ChangePassword(tempPassword, newPassword);
        }
        #endregion

        #region VerifyModel
        /// <summary>
        /// Verify if a user exists
        /// </summary>
        /// <param name="userName">User name</param>
        /// <returns>bool</returns>
        public bool VerifyUser(string userName)
        {
            
            MembershipUser currentUser = _provider.GetUser(userName, true);

            return currentUser != null;
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// This class is used for manage the user profiles
    /// </summary>
    public class UserProfile : ProfileBase
    {

        #region methods

        #region GetUserProfile
        /// <summary>
        /// This method is used for retreive the profile of a user
        /// </summary>
        /// <param name="username">Name of the user</param>
        /// <returns>UserProfile</returns>
        public static UserProfile GetUserProfile(string username)
        {
            return Create(username) as UserProfile;
        }
        #endregion

        #region GetUserProfile
        /// <summary>
        /// This method is used for retreive the profile of a user
        /// </summary>
        /// <returns>UserProfile</returns>
        public static UserProfile GetUserProfile()
        {
            return Create(Membership.GetUser().UserName) as UserProfile;
        }
        #endregion

        #endregion

        #region properties
        /// <summary>
        /// Get or Set the value of ApplicationDefault
        /// </summary>
        [SettingsAllowAnonymous(false)]
        [ProfileProvider("UserInfoProvider")]
        public string ApplicationDefault
        {
            get { return base["ApplicationDefault"] as string; }
            set { base["ApplicationDefault"] = value; }
        }

        [SettingsAllowAnonymous(false)]
        [ProfileProvider("UserInfoProvider")]
        public string ClientId
        {
            get { return base["ClientId"] as string; }
            set { base["ClientId"] = value; }
        }

        [SettingsAllowAnonymous(false)]
        [ProfileProvider("UserInfoProvider")]
        public string UserFullName
        {
            get { return base["UserFullName"] as string; }
            set { base["UserFullName"] = value; }
        }

        [SettingsAllowAnonymous(false)]
        [ProfileProvider("UserInfoProvider")]
        public string UserType
        {
            get { return base["UserType"] as string; }
            set { base["UserType"] = value; }
        }
        #endregion
    }
}
