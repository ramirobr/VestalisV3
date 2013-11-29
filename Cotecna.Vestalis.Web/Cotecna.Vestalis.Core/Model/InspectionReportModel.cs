
using System;
using System.Collections.Generic;
namespace Cotecna.Vestalis.Core
{
    /// <summary>
    /// This class is used to configure the inspection report screen.
    /// </summary>
    public class InspectionReportModel
    {
        /// <summary>
        /// Get or set Links
        /// </summary>
        public IList<string> Links { get; set; }
        /// <summary>
        /// Get or set GridColumns
        /// </summary>
        public DynamicDataGrid GridColumns { get; set; }
        /// <summary>
        /// Get or set ServiceOrderHeader
        /// </summary>
        public IList<Field> ServiceOrderHeader { get; set; }
        /// <summary>
        /// Get or set FormDefinition
        /// </summary>
        public Form FormDefinition { get; set; }
        /// <summary>
        /// Get or set ScreenOpenMode
        /// </summary>
        public ScreenOpenMode ScreenOpenMode { get; set; }
        /// <summary>
        /// Get or set CanPublish
        /// </summary>
        public bool CanPublish { get; set; }
        /// <summary>
        /// Get or set CanValidate
        /// </summary>
        public bool CanValidate { get; set; }
        /// <summary>
        /// Get or set InspectionReportItemId
        /// </summary>
        public Guid InspectionReportItemId { get; set; }
        /// <summary>
        /// Get or set IsPublished
        /// </summary>
        public bool IsPublished { get; set; }
        /// <summary>
        /// Get or set ApprovalStatus
        /// </summary>
        public int ApprovalStatus { get; set; }
        /// <summary>
        /// Get or set OrderIdentifier
        /// </summary>
        public Field OrderIdentifier { get; set; }
        /// <summary>
        /// Get or set ServiceOrderId
        /// </summary>
        public Guid ServiceOrderId { get; set; }
        /// <summary>
        /// Get or set PictureModel
        /// </summary>
        public PictureReportModel PictureModel { get; set; }
        /// <summary>
        /// Get or set InspectionReportName
        /// </summary>
        public string InspectionReportName { get; set; }
        /// <summary>
        /// Get or set PrincipalFormName
        /// </summary>
        public string PrincipalFormName { get; set; }
        /// <summary>
        /// Get or set CaptionBreadcrumbsPrincipal
        /// </summary>
        public string CaptionBreadcrumbsPrincipal { get; set; }
        /// <summary>
        /// Get or set CaptionTitlePrincipal
        /// </summary>
        public string CaptionTitlePrincipal { get; set; }
        
        /// <summary>
        /// Get an instance of InspectionReportModel class
        /// </summary>
        public InspectionReportModel()
        {
            Links = new List<string>();
            GridColumns = new DynamicDataGrid();
            ServiceOrderHeader = new List<Field>();
            FormDefinition = new Form();
            OrderIdentifier = new Field();
            PictureModel = new PictureReportModel();
        }
    }

    /// <summary>
    /// This class is used to configure the report when contains images
    /// </summary>
    public class PictureReportModel
    {
        /// <summary>
        /// Get or set PictureList
        /// </summary>
        public List<RowPictureCollection> PictureList { get; set; }
        /// <summary>
        /// Get or set ScreenOpenMode
        /// </summary>
        public ScreenOpenMode ScreenOpenMode { get; set; }
        
        /// <summary>
        /// Get an instance of PictureReportModel class
        /// </summary>
        public PictureReportModel()
        {
            PictureList = new List<RowPictureCollection>();
        }
    }

}
