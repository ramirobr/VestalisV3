using System;

namespace Cotecna.Vestalis.Initalization
{
    class Program
    {
        /// <summary>
        /// Entry point
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                HelperInitialization helperInitialization = new HelperInitialization();
                HelperInitialization.CreateRoles();
                helperInitialization.CreateUsers();

                UserProfile profile = UserProfile.GetUserProfile("adminGlobal");

                profile.UserFullName = "Admin Global";
                profile.UserType = "1";
                profile.Save();
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("Exception ocurred. Detail: {0}", ex.Message));
            }

            Console.ReadLine();
        }
    }
}
