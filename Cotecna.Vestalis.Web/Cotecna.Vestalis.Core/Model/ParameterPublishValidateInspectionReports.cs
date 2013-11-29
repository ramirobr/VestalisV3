using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotecna.Vestalis.Core
{
    public class ParameterPublishValidateInspectionReports
    {
        public string InspectionReportName { get; set; }
        public Guid ServiceOrderId { get; set; }
        public List<string> RolesForUser { get; set; }
        public string UserName { get; set; }
        public bool IsPublish { get; set; }
        public List<Guid?> SelectedIds { get; set; }
    }
}
