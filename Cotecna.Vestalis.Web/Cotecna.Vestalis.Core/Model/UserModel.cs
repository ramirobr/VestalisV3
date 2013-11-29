
using System;
using System.Collections.Generic;
using Cotecna.Vestalis.Entities;
using System.Web.Mvc;
namespace Cotecna.Vestalis.Core
{
    public class UserModel
    {
        public string Password { get; set; }
        public string Email { get; set; }

        public List<SelectListItem> BusinessApplications { get; set; }
        public Guid BusinessApplicationId { get; set; }
        public Guid BusinessApplicationIdPer { get; set; }

        public Guid? ClientId { get; set; }
        public Guid? ClientIdPer { get; set; }
        public List<CatalogueValue> ClientList { get; set; }

        public Dictionary<string, string> Roles { get; set; }
        public Dictionary<string, string> SelectedRoles { get; set; }
        public string CheckedRoles { get; set; }

        public Dictionary<int, string> UserTypes { get; set; }
        public int SelectedUserType { get; set; }

        public int OpenMode { get; set; }
        public int OpenModePer { get; set; }
       
        public PaginatedList<PermissionGridModel> PermissionList { get; set; }

        public List<string> ErrorList { get; set; }
        
        public bool HasErrors 
        {
            get
            {
                return ErrorList.Count > 0;
            }
        }
        public bool IsUserFound { get; set; }
       
        public UserModel()
        {
            Password = string.Empty;
            Email = string.Empty;

            BusinessApplicationId = Guid.Empty;

            Roles = new Dictionary<string, string>();
            SelectedRoles = new Dictionary<string, string>();

            ClientList = new List<CatalogueValue>();
            ClientId = Guid.Empty;

            UserTypes = new Dictionary<int, string>();
            SelectedUserType = 0;
            PermissionList = new PaginatedList<PermissionGridModel>();

            BusinessApplications = new List<SelectListItem>();

            ErrorList = new List<string>();

        }
    }
}
