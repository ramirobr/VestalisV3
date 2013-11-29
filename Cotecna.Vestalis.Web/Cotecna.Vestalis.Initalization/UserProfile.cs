using System.Web.Profile;
using System.Web.Security;

namespace Cotecna.Vestalis.Initalization
{
    public class UserProfile : ProfileBase
    {
        public static UserProfile GetUserProfile(string username)
        {
            return Create(username) as UserProfile;


        }
        public static UserProfile GetUserProfile()
        {
            return Create(Membership.GetUser().UserName) as UserProfile;
        }

        [SettingsAllowAnonymous(false)]
        [ProfileProvider("UserInfoProvider")]
        public string ApplicationDefault
        {
            get { return base["ApplicationDefault"] as string; }
            set { base["ApplicationDefault"] = value; }
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
        public string ClientId
        {
            get { return base["ClientId"] as string; }
            set { base["ClientId"] = value; }
        }

        [SettingsAllowAnonymous(false)]
        [ProfileProvider("UserInfoProvider")]
        public string UserType
        {
            get { return base["UserType"] as string; }
            set { base["UserType"] = value; }
        }

    }

}
