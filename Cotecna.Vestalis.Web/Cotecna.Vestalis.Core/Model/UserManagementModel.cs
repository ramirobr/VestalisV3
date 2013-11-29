using System.Collections.Generic;
using Cotecna.Vestalis.Entities;
using System.Web.Mvc;

namespace Cotecna.Vestalis.Core
{
    public class UserManagementModel
    {
        public PaginatedList<UserGridModel> SearchResult { get; set; }
        public List<SelectListItem> BusinessApplications { get; set; }

        public UserManagementModel()
        {
            SearchResult = new PaginatedList<UserGridModel>();
            BusinessApplications = new List<SelectListItem>();
        }
    }
}
