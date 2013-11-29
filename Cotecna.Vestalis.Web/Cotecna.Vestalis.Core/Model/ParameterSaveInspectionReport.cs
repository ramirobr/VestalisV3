
using System;
using System.Collections.Generic;
using System.Web.Mvc;
namespace Cotecna.Vestalis.Core
{
    public class ParameterSaveInspectionReport
    {
        public FormCollection FormCollection { get; set; }
        public Guid BusinessApplicationId { get; set; }
        public string UserName { get; set; }
        public Guid ServiceOrderId { get; set; }
        public string InspectionReportName { get; set; }
        public Guid InspectionReportItemId { get; set; }
        public List<string> RolesForUser { get; set; }
        public bool IsClient { get; set; }
    }
}
