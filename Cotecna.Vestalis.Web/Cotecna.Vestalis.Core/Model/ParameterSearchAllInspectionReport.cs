using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotecna.Vestalis.Core
{
    public class ParameterSearchAllInspectionReport
    {
        public Guid ServiceOrderId { get; set; }
        public Guid BusinessApplicationId { get; set; }
        public string UserName { get; set; }
        public List<string> SelectedReports { get; set; }
        public bool IsSelectedServiceOrder { get; set; }
        public string ServiceOrderReportName { get; set; }
    }
}
