using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace Cotecna.Vestalis.Core
{
    public class ParameterSearchPermission
    {
        public int PageSize { get; set; }
        public int SelectedPage { get; set; }
        public string SortedColumn { get; set; }
        public SortDirection SortDirection { get; set; }
        public string LoginName { get; set; }
        public bool IsExport { get; set; }
        public string LoggedUserName { get; set; }
        public bool IsGlobalAdmin { get; set; }
    }
}
