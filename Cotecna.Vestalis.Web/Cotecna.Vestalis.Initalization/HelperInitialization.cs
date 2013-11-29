using System;
using System.Linq;
using System.Web.Security;

namespace Cotecna.Vestalis.Initalization
{
    public class HelperInitialization
    {
        private readonly MembershipProvider _provider;

        public HelperInitialization()
            : this(null)
        {
        }

        public HelperInitialization(MembershipProvider provider)
        {
            _provider = provider ?? Membership.Provider;
        }


        public bool CreateUser(string userName, string password)
        {
            MembershipCreateStatus status;
            _provider.CreateUser(userName, password, null, null, null, true, null, out status);
            if (status == MembershipCreateStatus.Success)
                Roles.AddUserToRole(userName, "Client");
            return status == MembershipCreateStatus.Success;
        }

        public bool DeleteUser(string userName, bool deleteAll)
        {
            return _provider.DeleteUser(userName, deleteAll);
        }

        /// <summary>
        /// Create initial roles of Brazil container business application.
        /// </summary>
        public static void CreateRoles()
        {
            string rolesConfig = System.Configuration.ConfigurationManager.AppSettings["roles"];
            string[] roles = rolesConfig.Split(';');
            foreach (string roleToAdd in roles)
            {
                if (!Roles.RoleExists(roleToAdd))
                {
                    Roles.CreateRole(roleToAdd);
                    Console.WriteLine(String.Format("Role {0} created successfully", roleToAdd));
                }
                else
                {
                    Console.WriteLine(String.Format("Role {0} already exists", roleToAdd));
                }
            }
        }

        /// <summary>
        /// Create admin user for Brazil container business application
        /// </summary>
        public void CreateUsers()
        {
            string usersConfig = System.Configuration.ConfigurationManager.AppSettings["users"];
            string userName;
            string userRole;
            string userPassword;
            MembershipUser muser = null;
            MembershipCreateStatus status;
            string[] usersWithRoles = usersConfig.Split(';');
            foreach (string userWithRole in usersWithRoles)
            {
                string[] userData = userWithRole.Split('|');
                if (userData.ToList().Count == 3)
                {
                    userName = userData[0];
                    userPassword = userData[1];
                    userRole = userData[2];
                    muser = _provider.GetUser(userName, false);
                    if (muser == null)
                    {
                        muser = _provider.CreateUser(userName, userPassword, null, null, null, true, null,
                                                     out status);
                        Console.WriteLine(status == MembershipCreateStatus.Success
                                              ? String.Format("User {0} created sucessfully", userName)
                                              : String.Format("User wasn't created. Detail: {0}", status));
                    }

                    if (Roles.FindUsersInRole(userRole, userName).ToList().Count == 0)
                    {
                        Roles.AddUserToRole(userName, userRole);
                        Console.WriteLine(String.Format("User {0} added to the role {1} created sucessfully", userName, userRole));
                    }
                    else
                    {
                        Console.WriteLine(String.Format("User {0} already has the role {1}", userName, userRole));
                    }
                }
                else
                {
                    Console.WriteLine("Bad configuration");
                }

            }

        }
    }
}
