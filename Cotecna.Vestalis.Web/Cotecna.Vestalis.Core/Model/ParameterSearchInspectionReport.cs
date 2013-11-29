using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Cotecna.Vestalis.Core
{
    /// <summary>
    /// This class is used to send the parameters for searching all report items of a inspection report
    /// </summary>
    public class ParameterSearchInspectionReport
    {
        /// <summary>
        /// Get or set Collection
        /// </summary>
        public FormCollection Collection { get; set; }
        /// <summary>
        /// Get or set BusinessApplicationId
        /// </summary>
        public Guid BusinessApplicationId { get; set; }
        /// <summary>
        /// Get or set ServiceOrderId
        /// </summary>
        public Guid ServiceOrderId { get; set; }
        /// <summary>
        /// Get or set InspectionReportName
        /// </summary>
        public string InspectionReportName { get; set; }
        /// <summary>
        /// Get or set RolesForUser
        /// </summary>
        public List<string> RolesForUser { get; set; }
        /// <summary>
        /// Get or set UserName
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Get or set PageSize
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// Get or set SelectedPage
        /// </summary>
        public int SelectedPage { get; set; }
        /// <summary>
        /// Get or set IsClient
        /// </summary>
        public bool IsClient { get; set; }
        /// <summary>
        /// Get or set IsExport
        /// </summary>
        public bool IsExport { get; set; }
        /// <summary>
        /// Get or set FieldsWithLike
        /// </summary>
        public List<string> FieldsWithLike { get; set; }
        /// <summary>
        /// Get or set Captions
        /// </summary>
        public IList<DynamicCaptionGrid> Captions { get; set; }

        /// <summary>
        /// Get an instance of ParameterSearchInspectionReport class
        /// </summary>
        public ParameterSearchInspectionReport()
        {
            Collection = new FormCollection();
            RolesForUser = new List<string>();
            FieldsWithLike = new List<string>();
            Captions = new List<DynamicCaptionGrid>();
        }
    }
}
