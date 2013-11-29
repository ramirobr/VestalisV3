using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace Cotecna.Vestalis.Core
{
    /// <summary>
    /// This class is used for sending the parameters for searchs user method
    /// </summary>
    public class ParameterSearchUser
    {
        public int PageSize { get; set; }
        public int SelectedPage { get; set; }
        public string SortedColumn { get; set; }
        public SortDirection SortDirection { get; set; }
        public Guid BusinessApplicationId { get; set; }
        public bool IsExport { get; set; }
        public Dictionary<int, string> UserTypes { get; set; }
        public bool IsGlobalAdmin { get; set; }
        public string LoggedUserName { get; set; }
    }
}
