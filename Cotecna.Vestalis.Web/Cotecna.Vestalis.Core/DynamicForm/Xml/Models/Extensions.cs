using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Cotecna.Vestalis.Entities;


namespace Cotecna.Vestalis.Core
{

    public enum FieldType : int
    {
        None = 0,
        Catalogue = 1,
        SingleTextLine = 2,
        MultipleTextLine = 3,
        Boolean = 4,
        Datepicker = 5,
        Time = 6,
        Integer = 7,
        Decimal = 8,
        RegularExpressionText = 9,
        User = 10,
        AutoComplete = 11,
        PictureField = 12,
        StatusField = 13
    }


    public partial class FormSectionElement
    {
        [System.Xml.Serialization.XmlIgnore]
        public Field Field { get; set; }
    }

    public partial class Field
    {
        [System.Xml.Serialization.XmlIgnore]
        public FieldType FieldType { get; set; }

        [System.Xml.Serialization.XmlIgnore]
        public string FieldValue { get; set; }

        [System.Xml.Serialization.XmlIgnore]
        public IList<RulesForm> RulesForms { get; set; }

        public void InitFieldType(Guid businessApplicationId)
        {
            FieldType ft = FieldType.None;

            if (this is FieldsSingleTextLineField)
                ft = FieldType.SingleTextLine;
            else if (this is FieldsBooleanField)
                ft = FieldType.Boolean;
            else if (this is FieldsCatalogueField)
            {
                ft = FieldType.Catalogue;
                InitCatalogueField(this as FieldsCatalogueField, businessApplicationId);
            }
            else if (this is FieldsDatepickerField)
                ft = FieldType.Datepicker;
            else if (this is FieldsDecimalField)
                ft = FieldType.Decimal;
            else if (this is FieldsIntegerField)
                ft = FieldType.Integer;
            else if (this is FieldsMultipleTextLineField)
                ft = FieldType.MultipleTextLine;
            else if (this is FieldsRegularExpressionTextField)
                ft = FieldType.RegularExpressionText;
            else if (this is FieldsTimeField)
                ft = FieldType.Time;
            else if (this is FieldsUserField)
            {
                ft = FieldType.User;
                InitUserField(this as FieldsUserField, businessApplicationId);
            }
            else if (this is FieldsAutoCompleteTextField)
                ft = FieldType.AutoComplete;
            else if (this is FieldsPictureField)
                ft = FieldType.PictureField;
            else if (this is FieldsStatusField)
                ft = FieldType.StatusField;
            this.FieldType = ft;
        }

        /// <summary>
        /// Initialize the value of the user fields like a catalogue fields
        /// </summary>
        /// <param name="fieldsUserField">User field</param>
        /// <param name="businessApplicationId">Id of Business application</param>
        public void InitUserField(FieldsUserField fieldsUserField, Guid businessApplicationId)
        {
            List<UserInformation> users = AuthorizationBusiness.Instance.SearchUsers(businessApplicationId, fieldsUserField.Role);

            var selectedList = new List<SelectListItem>();
            foreach (var item in users)
            {
                selectedList.Add(new SelectListItem { Text = item.FullName, Value = item.UserName, Selected = false });
            }
            fieldsUserField.ItemsSource = selectedList;
        }

        public void FillFormValidations()
        {
            RulesForms = new List<RulesForm>();

            if (this.HasProperty("Mandatory") && this.GetPropertyValue<bool>("Mandatory"))
            {
                RulesForms.Add(RulesForm.RuleMandatory);
            }

            if (this.HasProperty("MinLength") && !String.IsNullOrEmpty(this.GetPropertyValue<string>("MinLength")))
            {
                RulesForms.Add(RulesForm.RuleMinLength);
            }

            if (this.HasProperty("MaxLength") && !String.IsNullOrEmpty(this.GetPropertyValue<string>("MaxLength")))
            {
                RulesForms.Add(RulesForm.RuleMaxLength);
            }

            if (this.HasProperty("StartDate") && !String.IsNullOrEmpty(this.GetPropertyValue<string>("StartDate")))
            {
                RulesForms.Add(RulesForm.RuleStartDate);
            }

            if (this.HasProperty("EndDate") && !String.IsNullOrEmpty(this.GetPropertyValue<string>("EndDate")))
            {
                RulesForms.Add(RulesForm.RuleEndDate);
            }

            if (this.HasProperty("NumDigit") && !String.IsNullOrEmpty(this.GetPropertyValue<string>("NumDigit")))
            {
                RulesForms.Add(RulesForm.RuleNumDigit);
            }

            if (this.HasProperty("MaxValue") && !String.IsNullOrEmpty(this.GetPropertyValue<object>("MaxValue")!= null ? this.GetPropertyValue<object>("MaxValue").ToString() : ""))
            {
                RulesForms.Add(RulesForm.RuleMaxValue);
            }

            if (this.HasProperty("MinValue") && !String.IsNullOrEmpty(this.GetPropertyValue<object>("MinValue")!=null ? this.GetPropertyValue<object>("MinValue").ToString() : ""))
            {
                RulesForms.Add(RulesForm.RuleMinValue);
            }

            if (this.HasProperty("Expression") && !String.IsNullOrEmpty(this.GetPropertyValue<string>("Expression")))
            {
                RulesForms.Add(RulesForm.RuleExpression);
            }

            if (this.HasProperty("Time"))
            {
                RulesForms.Add(RulesForm.RuleTime);
            }

        }

        public bool HasMethod(string methodName)
        {
            var type = this.GetType();
            return type.GetMethod(methodName) != null;
        }

        public bool HasProperty(string methodName)
        {
            var type = this.GetType();
            return type.GetProperty(methodName) != null;
        }

        public void InitCatalogueField(FieldsCatalogueField fieldsCatalogueField, Guid businessApplicationId)
        {
            IList<CatalogueValue> list = CacheHandler.Get(String.Format("{0}{1}", fieldsCatalogueField.CatalogueName, businessApplicationId),
                                            () =>
                                            CatalogueBusiness.GetCatalogueList(fieldsCatalogueField.CatalogueName, businessApplicationId));

            var selectedList = new List<SelectListItem>();
            foreach (var item in list)
            {
                selectedList.Add(new SelectListItem { Text = item.CatalogueValueData, Value = item.CatalogueValueId.ToString(), Selected = false });
            }

            fieldsCatalogueField.ItemsSource = selectedList;
        }
    }

    public partial class FieldsCatalogueField
    {
        [System.Xml.Serialization.XmlIgnore]
        public IList<System.Web.Mvc.SelectListItem> ItemsSource { get; set; }
    }

    public partial class FieldsUserField
    {
        [System.Xml.Serialization.XmlIgnore]
        public IList<System.Web.Mvc.SelectListItem> ItemsSource { get; set; }
    }

    public partial class FieldsAutoCompleteTextField
    {
        [System.Xml.Serialization.XmlIgnore]
        public IList<string> ItemSource { get; set; }
        public FieldsAutoCompleteTextField()
        {
            ItemSource = new List<string>();
        }
    }

    public class DynamicDataRow
    {
        public DynamicDataRow()
        {
            FieldValues = new List<DynamicDataRowValue>();
        }
        /// <summary>
        /// Based on this field the dynamic form will open later.
        /// It could be the inspection report identifier or service order identifier.
        /// </summary>
        public Guid? RowIdentifier { get; set; }
        /// <summary>
        /// List of fields values
        /// </summary>
        public IList<DynamicDataRowValue> FieldValues { get; set; }
        /// <summary>
        /// Show or hide publish button
        /// </summary>
        public bool CanPublish { get; set; }
        /// <summary>
        /// Show or hide validate button
        /// </summary>
        public bool CanValidate { get; set; }
        /// <summary>
        /// Get or Set IsReadOnly
        /// </summary>
        public bool IsReadOnly { get; set; }
        /// <summary>
        /// Get or set ApprovalLevel
        /// </summary>
        public int ApprovalStatus { get; set; }
        /// <summary>
        /// Get or set CurrentCompletedLevel
        /// </summary>
        public int CurrentCompletedLevel { get; set; }
        /// <summary>
        /// Get or set IsPublished
        /// </summary>
        public bool IsPublished { get; set; }
        /// <summary>
        /// Flag to know if the inspection report has data to be displayed to the client. 
        /// </summary>
        public string FirstInspectionReportClient { get; set; }
        /// <summary>
        /// Flag to know if the inspection report has pictures to be displayed to the client. 
        /// </summary>
        public bool HasPicturesClient { get; set; }
        /// <summary>
        /// Flag to know if the inspection report has documents to be displayed to the client. 
        /// </summary>
        public bool HasDocumentsClient { get; set; }
    }

    public class DynamicDataRowValue
    {
        public int FieldType { get; set; }
        public string FieldValue { get; set; }
        public PictureGridModel Pictures { get; set; }
        public DynamicDataRowValue()
        {
            Pictures = new PictureGridModel();
        }
    }

    public class PictureGridModel
    {
        public List<PictureSearchModelItem> PictureList { get; set; }
        public int PictureCount { get; set; }
        public PictureGridModel()
        {
            PictureList = new List<PictureSearchModelItem>();
        }
    }

    /// <summary>
    /// This class is used for settting the captions in the dynamic grid
    /// </summary>
    public class DynamicCaptionGrid
    {
        /// <summary>
        /// Get or set FieldName
        /// </summary>
        public string FieldName { get; set; }
        /// <summary>
        /// Get or set Caption
        /// </summary>
        public string Caption { get; set; }
        /// <summary>
        /// Get or set the Width
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// Get or set ExcelColumnWidth
        /// </summary> 
        public int ExcelColumnWidth { get; set; }
    }

    public class DynamicDataGrid : PaginatedGridModel
    {
        public DynamicDataGrid()
        {
            Captions = new List<DynamicCaptionGrid>();
            Filters = new List<Field>();
            DataRows = new List<DynamicDataRow>();
        }
        /// <summary>
        /// The list of rows to the grid
        /// </summary>
        public IList<DynamicDataRow> DataRows { get; set; }
        /// <summary>
        /// They must be the same number as the number of fields in the row
        /// </summary>
        public IList<DynamicCaptionGrid> Captions { get; set; }
        /// <summary>
        /// The list of filters for search
        /// </summary>
        public IList<Field> Filters { get; set; }
        /// <summary>
        /// The name of the form
        /// </summary>
        public string FormName { get; set; }
        /// <summary>
        /// Show or hide the button validate all
        /// </summary>
        public bool CanValidateAll { get; set; }
        /// <summary>
        /// Show or hide the button publish all
        /// </summary>
        public bool CanPublishAll { get; set; }
        /// <summary>
        /// Get IsReadOnly
        /// </summary>
        public bool IsReadOnly { get { return DataRows != null && DataRows.Any(dataRow => dataRow.IsReadOnly); } }
        /// <summary>
        /// Get or set UserLevel
        /// </summary>
        public int UserLevel { get; set; }
        /// <summary>
        /// Get or Set BusinessApplicationName
        /// </summary>
        public string BusinessApplicationName { get; set; }
        /// <summary>
        /// Get or set CaptionTitle
        /// </summary>
        public string CaptionTitle { get; set; }
        /// <summary>
        /// Get or set CaptionBreadcrumbs
        /// </summary>
        public string CaptionBreadcrumbs { get; set; }
    }

    public partial class Form
    {
        public bool IsReadOnly { get; set; }

        [System.Xml.Serialization.XmlIgnore]
        public Guid? FormIdentifier { get; set; }

        [System.Xml.Serialization.XmlIgnore]
        public IList<Field> ServiceOrderHeader { get; set; }

        [System.Xml.Serialization.XmlIgnore]
        public ScreenOpenMode ScreenOpenMode { get; set; }

        [System.Xml.Serialization.XmlIgnore]
        public Field OrderIdentifier { get; set; }

        [System.Xml.Serialization.XmlIgnore]
        public IList<string> Links { get; set; }

        [System.Xml.Serialization.XmlIgnore]
        public bool HasPictures { get; set; }

        public Form()
        {
            ServiceOrderHeader = new List<Field>();
            OrderIdentifier = new Field();
            Links = new List<string>();
        }

    }
}
