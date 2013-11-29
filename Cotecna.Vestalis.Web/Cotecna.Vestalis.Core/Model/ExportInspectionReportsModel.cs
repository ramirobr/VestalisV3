using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotecna.Vestalis.Core
{
    public class ExportInspectionReportsModel
    {
        public Dictionary<string, DynamicDataGrid> InspectionReports { get; set; }
        public Form ServiceOrderData { get; set; }
        public bool IsSelectedServiceOrder { get; set; }
        public string ServiceOrderSheetName { get; set; }
        public string BusinessApplicationName { get; set; }

        public ExportInspectionReportsModel()
        {
            InspectionReports = new Dictionary<string, DynamicDataGrid>();
            ServiceOrderData = new Form();
        }
    }
}
