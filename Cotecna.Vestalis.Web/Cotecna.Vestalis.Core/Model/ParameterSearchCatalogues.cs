using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace Cotecna.Vestalis.Core
{
    public class ParameterSearchCatalogues
    {
        public Guid BusinessApplicationId { get; set; }
        public int PageSize { get; set; }
        public SortDirection SortDirection { get; set; }
        public string SortedColumn { get; set; }
        public int SelectedPage { get; set; }
        public bool IsExport { get; set; }
        public string UserName { get; set; }
    }
}
