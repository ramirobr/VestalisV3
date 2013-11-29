
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Cotecna.Vestalis.Core.DynamicForm;
using Cotecna.Vestalis.Entities;
using Cotecna.Vestalis.Core.Resources;
namespace Cotecna.Vestalis.Core
{
    /// <summary>
    /// This class contains all methods needed to manage all information about the inspection reports
    /// </summary>
    public static class InspectionReportBusiness
    {

        #region GetInspectionReportByName
        /// <summary>
        /// Get an inspection report object by name
        /// </summary>
        /// <param name="inspectionReportName">Inspection report name</param>
        /// <param name="serviceOrderId">Id of service order</param>
        /// <returns>InspectionReport</returns>
        public static InspectionReport GetInspectionReportByName(string inspectionReportName, Guid serviceOrderId)
        {
            InspectionReport inspectionReport = new InspectionReport();
            using (VestalisEntities context = new VestalisEntities())
            {
                inspectionReport = context.InspectionReports
                    .Where(data => data.ServiceOrderId == serviceOrderId 
                        && data.FormName == inspectionReportName 
                        && data.IsDeleted == false)
                    .FirstOrDefault();
            }
            return inspectionReport;
        }
        #endregion

        #region GetInspectionReportDefinition
        /// <summary>
        /// Get the difinition of a inspection report
        /// </summary>
        /// <param name="businessApplicationId">Id of business application</param>
        /// <param name="serviceOrderId">Id of service order</param>
        /// <param name="inspectionReportName">Name of inspection report</param>
        /// <param name="isClient">Flag to know if the logged user is a client</param>
        /// <param name="inspectionReportItemId">Inspection report identifier</param>
        /// <returns>Form</returns>
        public static Form GetInspectionReportDefinition(Guid businessApplicationId, Guid? serviceOrderId, string inspectionReportName, bool isClient, Guid? inspectionReportItemId = null)
        {
            IList<FormValue> formValuesList = new List<FormValue>();
            FormValue formValue = null;
            Form inspectionReportForm = null;
            int pictureFields = 0;
            //Get the xml form definition of the inspection report (case new or edit). 
            if (inspectionReportItemId.HasValue)
            {
                inspectionReportForm = DynamicFormEngine.GetExistingInspectionReportForm(inspectionReportItemId.Value);
            }
            else
            {
                inspectionReportForm = CacheHandler.Get(String.Format("Form{0}{1}", inspectionReportName, businessApplicationId),
                                            () =>
                                            DynamicFormEngine.GetFormDefinition(businessApplicationId, FormType.InspectionReport, inspectionReportName, isClient));
            }

            //Get the fields definition for the business application
            Fields fieldBusinessApplication = CacheHandler.Get(String.Format("Field{0}", businessApplicationId),
                                            () => DynamicFormEngine.GetFields(businessApplicationId));

            //Get the service field values for editing
            if (inspectionReportItemId.HasValue)
            {
                using (VestalisEntities ctx = new VestalisEntities())
                {
                    formValuesList =
                        (from formValues in ctx.FormValues
                         where formValues.IsDeleted == false && formValues.ServiceOrderId == serviceOrderId.Value
                         && formValues.InspectionReportItemId == inspectionReportItemId
                         select formValues).ToList();
                }
            }

            foreach (var section in inspectionReportForm.Sections)
            {
                foreach (FormSectionElement element in section.FormElements)
                {
                    if ((element.IsVisibleClient && isClient) || !isClient)
                    {
                        element.Field = fieldBusinessApplication.Items.Single(x => x.FieldName == element.Identifier);
                        element.Field.InitFieldType(businessApplicationId);
                        element.Field.FillFormValidations();
                        if (element.Field.FieldType == FieldType.PictureField)
                            pictureFields++;
                        //Case edit
                        if (inspectionReportItemId.HasValue)
                        {
                            formValue = formValuesList.FirstOrDefault(item => item.FieldName == element.Identifier);
                            element.Field.FieldValue = formValue != null ? GetFormValue(formValue) : String.Empty;
                            if (element.Field.FieldType == FieldType.Catalogue &&
                                !String.IsNullOrEmpty(element.Field.FieldValue))
                            {
                                FieldsCatalogueField fieldCatalogue = element.Field as FieldsCatalogueField;
                                SelectListItem itemFoundinSelectList =
                                    fieldCatalogue.ItemsSource.FirstOrDefault(
                                        item => item.Value == element.Field.FieldValue);
                                if (itemFoundinSelectList != null)
                                {
                                    itemFoundinSelectList.Selected = true;
                                }
                            }
                        }
                        //Case new
                        else
                        {
                            element.Field.FieldValue = element.DefaultValue;
                        }
                    }
                    else
                    {
                        section.FormElements =
                        section.FormElements.Except(section.FormElements.Where(item => item.Identifier == element.Identifier)).ToArray();
                    }

                }
            }
            inspectionReportForm.HasPictures = pictureFields > 0;
            return inspectionReportForm;
        }

        #endregion

        #region GetFormValue
        /// <summary>
        /// Get the service form values
        /// </summary>
        /// <param name="formValue">Service order field</param>
        /// <returns>Field value</returns>
        private static string GetFormValue(FormValue formValue)
        {
            string valueToReturn = String.Empty;
            switch (formValue.FieldType)
            {
                case (int)FieldType.Boolean:
                    valueToReturn = formValue.CheckValue.HasValue ? formValue.CheckValue.GetValueOrDefault().ToString() : string.Empty;
                    break;
                case (int)FieldType.Catalogue:
                    valueToReturn = formValue.CatalogueValueId.HasValue ? formValue.CatalogueValueId.GetValueOrDefault().ToString() : string.Empty;
                    break;
                case (int)FieldType.Datepicker:
                    valueToReturn = formValue.DateValue.HasValue ? formValue.DateValue.GetValueOrDefault().ToString("dd/MM/yyyy") : string.Empty;
                    break;
                case (int)FieldType.Decimal:
                    valueToReturn = formValue.DecimalValue.HasValue ? formValue.DecimalValue.GetValueOrDefault().ToString() : string.Empty;
                    break;
                case (int)FieldType.Integer:
                    valueToReturn = formValue.IntegerValue.HasValue ? formValue.IntegerValue.GetValueOrDefault().ToString() : string.Empty;
                    break;
                case (int)FieldType.MultipleTextLine:
                case (int)FieldType.RegularExpressionText:
                case (int)FieldType.SingleTextLine:
                case (int)FieldType.Time:
                case (int)FieldType.AutoComplete:
                    valueToReturn = formValue.TextValue;
                    break;
                case (int)FieldType.User:
                    valueToReturn = formValue.UserName;
                    break;
                //TODO
                case (int)FieldType.None:
                    break;
            }
            return valueToReturn;
        }
        #endregion

        #region GetInspectionReportGridDefinition
        /// <summary>
        /// Get the definition of grid columns
        /// </summary>
        /// <param name="businessApplicationId">Id of business application</param>
        /// <param name="clientVisible">Is clint visible</param>
        /// <param name="inspectionReportName">Name of inspection report</param>
        /// <returns>DynamicDataGrid</returns>
        public static DynamicDataGrid GetInspectionReportGridDefinition(Guid businessApplicationId, bool clientVisible, string inspectionReportName = null)
        {
            DynamicDataGrid dynamicGrid = new DynamicDataGrid();
            Form serviceOrder = null;
            //Get the xml form definition of the service order given for the business application

            if (!string.IsNullOrEmpty(inspectionReportName))
            {
                serviceOrder = DynamicFormEngine.GetFormDefinition(businessApplicationId, FormType.InspectionReport, inspectionReportName, clientVisible);
            }
            else
            {
                serviceOrder = DynamicFormEngine.GetFormDefinition(businessApplicationId, FormType.InspectionReport, clientVisible);
            }

            //Get the fields definition for the business application
            Fields fieldBusinessApplication = CacheHandler.Get(String.Format("Field{0}", businessApplicationId),
                                                               () =>
                                                               DynamicFormEngine.GetFields(businessApplicationId));
            dynamicGrid.FormName = serviceOrder.Name;
            IList<Field> filters = new List<Field>();
            if (serviceOrder != null)
            {
                //For each section
                foreach (var section in serviceOrder.Sections)
                    //For each element of the section
                    foreach (var element in section.FormElements)
                    {
                        //Verify attributte "IsDataGridVisible" to set it as grid column
                        if (element.IsDataGridVisible && (!clientVisible || (clientVisible && element.IsVisibleClient)))
                        {
                            Field field = fieldBusinessApplication.Items.Single(x => x.FieldName == element.Identifier);
                            dynamicGrid.Captions.Add(new DynamicCaptionGrid
                            {
                                Caption = (string.IsNullOrEmpty(field.CaptionGrid) ? field.Caption : field.CaptionGrid),
                                FieldName = element.Identifier,
                                Width = string.IsNullOrEmpty(field.Width) ? 0 : int.Parse(field.Width),
                                ExcelColumnWidth = string.IsNullOrEmpty(field.ExcelColumnWidth) ? 20 : int.Parse(field.ExcelColumnWidth)
                            });
                        }
                        //Verify attributte "IsFilterVisible" to set it as search filter in the grid
                        if (element.IsFilterVisible && (!clientVisible || (clientVisible && element.IsVisibleClient)))
                        {
                            Field field = fieldBusinessApplication.Items.Single(x => x.FieldName == element.Identifier);
                            field.InitFieldType(businessApplicationId);
                            field.FieldValue = String.Empty;
                            filters.Add(field);
                        }
                    }
            }
            dynamicGrid.Filters = filters;
            return dynamicGrid;
        }
        #endregion

        #region SearchInspectionReportList
        /// <summary>
        /// Get the list of inspection reports
        /// </summary>
        /// <param name="parameters">Filter data to execute the search</param>
        /// <returns>DynamicDataGrid</returns>
        public static DynamicDataGrid SearchInspectionReportList(ParameterSearchInspectionReport parameters)
        {
            DynamicDataGrid dynamicDataGrid = new DynamicDataGrid();
            DynamicDataRow dataRow = null;
            PaginatedList<FormValue> resultQuery = null;
            string fieldName = string.Empty;
            string fieldValue = string.Empty;
            int fieldType = 0;
            List<PropertyInfo> filterProperties = null;
            List<string> gridFields = new List<string>();
            Dictionary<string, DynamicDataRowValue> temRowValues = null;
            List<FormValue> inspectionReportItemQuery = null;
            List<ApprovalItem> approvalItems = null;
            List<Guid?> inspectionReportItemIds = null;
            //obtain the definition of the form
            Form inspectionReportForm = DynamicFormEngine.GetFormDefinition(parameters.BusinessApplicationId, FormType.InspectionReport, parameters.InspectionReportName, parameters.IsClient);
            //obtain the id of the selected inspection report
            Guid inspectionReportId = GetInspectionReportByName(parameters.InspectionReportName, parameters.ServiceOrderId).InspectionReportId;


            using (VestalisEntities context = new VestalisEntities())
            {
                //get the query 
                resultQuery = GetInspectionReportQuery(context, inspectionReportId, parameters, ref inspectionReportItemIds);
                //set data of pagination
                dynamicDataGrid.Page = parameters.SelectedPage;
                dynamicDataGrid.NumberOfPages = resultQuery.NumberOfPages;
                dynamicDataGrid.TotalNumberOfItemsWithoutPagination = resultQuery.TotalCount;
                dynamicDataGrid.PageSize = parameters.PageSize;

                //Get approval items for the inspection report and the roles of the logged user
                
                approvalItems = GetApprovalItemsGivenIds(context, inspectionReportItemIds, parameters.RolesForUser);
                //set flags for show buttons
                dynamicDataGrid.CanPublishAll = approvalItems.Any(data => data.CanPublish.GetValueOrDefault() && data.ApprovalStatus == (int)ApprovalStatus.Ready);
                dynamicDataGrid.CanValidateAll = approvalItems.Any(data => !data.CanPublish.GetValueOrDefault() && data.ApprovalStatus == (int)ApprovalStatus.Ready);
                
                inspectionReportItemQuery = resultQuery.Collection;


                var groupInspectionReports = (from formValue in inspectionReportItemQuery
                                              group formValue by new { formValue.InspectionReportItemId }
                                                  into rows
                                                  select new
                                                  {
                                                      serviceOrderId = rows.Key,
                                                      rows
                                                  });

                if (inspectionReportForm != null && (inspectionReportItemQuery != null && inspectionReportItemQuery.Count > 0))
                {
                    //get the fields that can be showed in the grid
                    foreach (var section in inspectionReportForm.Sections)
                    {
                        if (parameters.IsClient)
                        {
                            gridFields.AddRange(
                                section.FormElements.Where(element => element.IsDataGridVisible && element.IsVisibleClient).Select(
                                    element => element.Identifier));
                        }
                        else
                        {
                            gridFields.AddRange(
                                section.FormElements.Where(element => element.IsDataGridVisible).Select(
                                    element => element.Identifier));
                        }
                    }

                    var formValueProperties = new List<string>();
                    formValueProperties.Add("FieldType");
                    formValueProperties.Add("FieldName");


                    //Get the fields definition for the business application
                    Fields fieldBusinessApplication = CacheHandler.Get(String.Format("Field{0}", parameters.BusinessApplicationId),
                                                    () => DynamicFormEngine.GetFields(parameters.BusinessApplicationId));

                    foreach (var group in groupInspectionReports)
                    {
                        dataRow = new DynamicDataRow();
                        temRowValues = new Dictionary<string, DynamicDataRowValue>();
                        gridFields.ForEach(field =>
                        {
                            Field currentField = fieldBusinessApplication.Items.FirstOrDefault(x => x.FieldName == field);
                            if (currentField is FieldsPictureField)
                            {
                                if (!parameters.IsExport)
                                {
                                    temRowValues.Add(field, new DynamicDataRowValue() { FieldType = (int)FieldType.PictureField });
                                    temRowValues[field].Pictures = PictureDocumentBusiness.SearchPictureGridInspectionReport(parameters.ServiceOrderId, group.serviceOrderId.InspectionReportItemId.Value);
                                }
                                else
                                {
                                    var captionPic = parameters.Captions.FirstOrDefault(caption => caption.FieldName == currentField.FieldName);
                                    parameters.Captions.Remove(captionPic);
                                }
                            }
                            else if (currentField is FieldsStatusField)
                            {
                                temRowValues.Add(field, new DynamicDataRowValue() { FieldType = (int)FieldType.StatusField });
                            }
                            else
                            {
                                temRowValues.Add(field, new DynamicDataRowValue() { FieldType = 2 });
                            }
                               
                        });
                        foreach (var data in group.rows)
                        {
                            filterProperties = data.GetType().GetProperties().Where(property => formValueProperties.Contains(property.Name)).ToList();

                            //Get the field name
                            if (filterProperties.FirstOrDefault(item => item.Name == "FieldName") != null)
                            {
                                fieldName = data.GetPropertyValue<object>("FieldName").ToString();
                            }

                            //Get the field type and field value
                            if (filterProperties.FirstOrDefault(item => item.Name == "FieldType") != null)
                            {
                                string tempValue = data.GetPropertyValue<object>("FieldType").ToString();
                                if (!string.IsNullOrEmpty(tempValue))
                                {
                                    fieldType = int.Parse(tempValue);
                                    //Get the value as a string according the type
                                    fieldValue = GetFieldValue(data, fieldType, fieldValue);
                                }
                            }

                            //if all values are filled,the system continues for create the result
                            if ((!string.IsNullOrEmpty(fieldName) && gridFields.Any(field => field == fieldName))
                                && !string.IsNullOrEmpty(fieldValue) && fieldType > 0)
                            {
                                temRowValues[fieldName].FieldValue = fieldValue;
                                temRowValues[fieldName].FieldType = fieldType;
                            }

                            fieldName = string.Empty;
                            fieldValue = string.Empty;
                            fieldType = 0;
                        }

                        dataRow.FieldValues.Clear();
                        temRowValues.Values.ToList().ForEach(value =>
                        {
                            dataRow.FieldValues.Add(new DynamicDataRowValue { FieldType = value.FieldType, FieldValue = value.FieldValue, Pictures = value.Pictures });
                        });

                        //get the identifier of row
                        Guid inspectionReportItemId = group.serviceOrderId.InspectionReportItemId.Value;
                        dataRow.RowIdentifier = inspectionReportItemId;

                        SetApprovalItems(dataRow, approvalItems, inspectionReportItemId);

                        //add the row to the dinamic grid
                        dynamicDataGrid.DataRows.Add(dataRow);
                    }
                }
            }

            SetStatusInspectionReport(dynamicDataGrid);
            dynamicDataGrid.Captions = parameters.Captions;
            return dynamicDataGrid;
        }
        #endregion

        #region SetStatusInspectionReport
        /// <summary>
        /// Set the current status for each inspection report item
        /// </summary>
        /// <param name="dataGrid">DataGrid with the result of the search</param>
        private static void SetStatusInspectionReport(DynamicDataGrid dataGrid)
        {
            //check all rows in the result
            foreach (DynamicDataRow row in dataGrid.DataRows)
            {
                //take all fields with the type StatusField
                foreach (DynamicDataRowValue value in row.FieldValues.Where(field => field.FieldType ==(int)FieldType.StatusField))
                {

                    if (row.ApprovalStatus == (int)ApprovalStatus.Ready)
                    {
                        //set UnPublish or UnValidated status
                        if (row.CanPublish && !row.CanValidate)
                        {
                            value.FieldValue = LanguageResource.Unpublished;
                        }
                        else if (!row.CanPublish && row.CanValidate)
                        {
                            value.FieldValue = LanguageResource.Unvalidated;
                        }
                    }
                    else if (row.IsReadOnly && row.CanPublish)
                    {
                        //Set published status
                        value.FieldValue = LanguageResource.Published;
                    }
                    else if (row.ApprovalStatus == (int)ApprovalStatus.Completed && row.CurrentCompletedLevel >= 1 && !row.IsPublished)
                    {
                        //Set validated status
                        value.FieldValue = LanguageResource.Validated;
                    }
                    else if (row.ApprovalStatus == (int)ApprovalStatus.Completed && row.CurrentCompletedLevel > 1 && row.IsPublished)
                    {
                        //Set published status
                        value.FieldValue = LanguageResource.Published;
                    }

                }
            }
        }
        #endregion

        #region SetApprovalItems
        /// <summary>
        /// Set the approval status in the inspection report item
        /// </summary>
        /// <param name="dataRow">Current datarow</param>
        /// <param name="approvalItems">List of approval items</param>
        /// <param name="inspectionReportItemId">Id of inspection report</param>
        private static void SetApprovalItems(DynamicDataRow dataRow, List<ApprovalItem> approvalItems, Guid inspectionReportItemId)
        {
            if (approvalItems != null)
            {
                ApprovalItem approvalItem = null;
                //get the approval item
                if (approvalItems.Count > 1)
                {
                    approvalItem = approvalItems.OrderByDescending(data => data.ApprovalLevel).Where(data => data.ApprovalStatus == (int)ApprovalStatus.Ready && data.InspectionReportItemId == inspectionReportItemId).FirstOrDefault();
                    if (approvalItem == null)
                    {
                        approvalItem = approvalItems.OrderByDescending(data => data.ApprovalLevel).Where(data => data.ApprovalStatus == (int)ApprovalStatus.Completed && data.InspectionReportItemId == inspectionReportItemId).FirstOrDefault();
                    }
                }
                else
                {
                    approvalItem = approvalItems.FirstOrDefault(data => data.InspectionReportItemId == inspectionReportItemId);
                }

                //if exist the approval item, the system will continue with the process
                if (approvalItem != null)
                {
                    //set publish option
                    if (approvalItem.CanPublish.GetValueOrDefault())
                    {
                        dataRow.CanPublish = true;
                        dataRow.CanValidate = false;
                    }
                    else
                    {
                        //set validate option
                        dataRow.CanPublish = false;
                        dataRow.CanValidate = true;
                    }
                    //set read only
                    if (approvalItem.InspectionReportItem.PublicationDate != null)
                    {
                        dataRow.IsReadOnly = true;
                    }
                    else
                    {
                        dataRow.IsReadOnly = approvalItem.IsReadOnly.GetValueOrDefault();
                    }
                    dataRow.CurrentCompletedLevel = approvalItem.InspectionReportItem.CurrentCompletedLevel.GetValueOrDefault();
                    dataRow.ApprovalStatus = approvalItem.ApprovalStatus.GetValueOrDefault();
                    dataRow.IsPublished = approvalItem.InspectionReportItem.PublicationDate != null;
                }
            }
        }
        #endregion

        #region GetCurrentApprovalStatus
        /// <summary>
        /// Get current approval status
        /// </summary>
        /// <param name="userRoles">Roles of the user</param>
        /// <param name="inspectionReportItemId">Id of inspection report item</param>
        /// <returns>int</returns>
        public static int GetCurrentApprovalStatus(List<string> userRoles, Guid inspectionReportItemId)
        {
            int status = 0;
            using (VestalisEntities context = new VestalisEntities())
            {
                status = (from appItem in context.ApprovalItems
                          where appItem.IsDeleted == false && userRoles.Contains(appItem.RoleName)
                          && appItem.InspectionReportItemId == inspectionReportItemId
                          select appItem.ApprovalStatus).FirstOrDefault() ?? 0;
            }
            return status;
        }
        #endregion

        #region GetRoleLevel
        /// <summary>
        /// Get the current role for the user
        /// </summary>
        /// <param name="userRoles">Listo of roles</param>
        /// <param name="businessApplicationId">Id of business application</param>
        /// <returns>Rol level</returns>
        public static int GetRoleLevel(string[] userRoles, Guid businessApplicationId)
        {
            int result = 0;
            using (VestalisEntities context = new VestalisEntities())
            {
                result = (from role in context.ValidationRoles
                          where role.IsDeleted == false && userRoles.Contains(role.RoleName)
                          && role.BusinessApplicationId == businessApplicationId
                          select role.RoleLevel).FirstOrDefault();
            }
            return result;
        }
        #endregion

        #region GetApprovalItemsGivenIds
        /// <summary>
        /// Get approval items for the inspection report and the roles of the logged user
        /// </summary>
        /// <param name="context">Data base context</param>
        /// <param name="inspectionReportItemIds">List of inspection report items ids</param>
        /// <param name="rolesForUser">List of roles for the user</param>
        /// <returns>List of approval items</returns>
        public static List<ApprovalItem> GetApprovalItemsGivenIds(VestalisEntities context, List<Guid?> inspectionReportItemIds, List<string> rolesForUser)
        {
            List<ApprovalItem> approvalItems = null;
            approvalItems = (from appItem in context.ApprovalItems
                             where inspectionReportItemIds.Contains(appItem.InspectionReportItemId)
                             && rolesForUser.Contains(appItem.RoleName) && appItem.IsDeleted == false
                             select appItem).ToList();
            return approvalItems;
        }
        #endregion

        #region GetApprovalItems
        /// <summary>
        /// Get approval items for the inspection report and the roles of the logged user
        /// </summary>
        /// <param name="context">Data base context</param>
        /// <param name="serviceOrderId">Id of service order</param>
        /// <param name="inspectionReportId">Id of inspection report</param>
        /// <param name="rolesForUser">List of roles for the user</param>
        /// <returns>List of approval items</returns>
        public static List<ApprovalItem> GetApprovalItems(VestalisEntities context, Guid serviceOrderId, Guid inspectionReportId, List<string> rolesForUser)
        {
            List<ApprovalItem> approvalItems = null;

            var inspectionReportItemIds = (from inspectionReportItem in context.InspectionReportItems
                                           join inspectionReport in context.InspectionReports on inspectionReportItem.InspectionReportId equals inspectionReport.InspectionReportId
                                           join appItem in context.ApprovalItems on inspectionReportItem.InspectionReportItemId equals appItem.InspectionReportItemId
                                           where inspectionReportItem.IsDeleted == false && inspectionReportItem.InspectionReportId == inspectionReportId
                                           && inspectionReport.ServiceOrderId == serviceOrderId
                                           && appItem.IsDeleted == false && rolesForUser.Contains(appItem.RoleName)
                                           && (appItem.ApprovalStatus == (int)ApprovalStatus.Completed || appItem.ApprovalStatus == (int)ApprovalStatus.Ready)
                                           orderby inspectionReportItem.CreationDate descending
                                           select inspectionReportItem.InspectionReportItemId).AsEnumerable().Cast<Guid?>().ToList();

            approvalItems = (from appItem in context.ApprovalItems
                             where inspectionReportItemIds.Contains(appItem.InspectionReportItemId)
                             && rolesForUser.Contains(appItem.RoleName) && appItem.IsDeleted == false
                             select appItem).ToList();


            return approvalItems;
        }

        /// <summary>
        /// Get approval items for the inspection report and the roles of the logged user
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="rolesForUser">List of roles for the user</param>
        /// <param name="selectedIds">Selected inspection report item ids</param>
        /// <returns>List of approval items</returns>
        public static List<ApprovalItem> GetApprovalItems(VestalisEntities context, List<string> rolesForUser,List<Guid?> selectedIds)
        {
            List<ApprovalItem> approvalItems = null;

            approvalItems = (from appItem in context.ApprovalItems
                             where selectedIds.Contains(appItem.InspectionReportItemId)
                             && rolesForUser.Contains(appItem.RoleName) && appItem.IsDeleted == false
                             select appItem).ToList();


            return approvalItems;
        }
        #endregion

        #region GetFieldValue
        /// <summary>
        /// Get the value of a field
        /// </summary>
        /// <param name="data">data</param>
        /// <param name="fieldType">field type</param>
        /// <param name="fieldValue">field value</param>
        /// <returns></returns>
        private static string GetFieldValue(FormValue data, int fieldType, string fieldValue)
        {
            switch (fieldType)
            {
                case (int)FieldType.Boolean:
                    fieldValue = data.CheckValue.HasValue ? data.CheckValue.GetValueOrDefault().ToString() : string.Empty;
                    break;
                case (int)FieldType.Catalogue:
                    if (data.CatalogueValueId.HasValue)
                    {
                        Guid catalogValueId = new Guid(data.CatalogueValueId.Value.ToString());
                        fieldValue = CatalogueBusiness.GetCatalogueValue(catalogValueId).CatalogueValueData;
                    }
                    else
                    {
                        fieldValue = string.Empty;
                    }
                    break;
                case (int)FieldType.Datepicker:
                    if (data.DateValue.HasValue)
                    {
                        DateTime temDateValue = DateTime.Parse(data.DateValue.Value.ToString());
                        fieldValue = string.Format("{0:dd/MM/yyyy}", temDateValue);
                    }
                    else
                    {
                        fieldValue = string.Empty;
                    }
                    break;
                case (int)FieldType.Decimal:
                    fieldValue = data.DecimalValue.HasValue ? data.DecimalValue.GetValueOrDefault().ToString() : string.Empty;
                    break;
                case (int)FieldType.Integer:
                    fieldValue = data.IntegerValue.HasValue ? data.IntegerValue.GetValueOrDefault().ToString() : string.Empty;
                    break;
                case (int)FieldType.MultipleTextLine:
                case (int)FieldType.RegularExpressionText:
                case (int)FieldType.SingleTextLine:
                case (int)FieldType.Time:
                case (int)FieldType.User:
                case (int)FieldType.AutoComplete:
                    fieldValue = data.TextValue;
                    break;
                case (int)FieldType.None:
                    break;
            }
            return fieldValue;
        }
        #endregion

        #region GetInspectionReportQuery

        /// <summary>
        /// Get the query of inspection reports
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="inspectionReportId">Id of inspectionReport</param>
        /// <param name="parameters">Parametes</param>
        /// <param name="inspectionReportItemIds">List of inspection report id</param>
        /// <returns>List of FormValue</returns>
        private static PaginatedList<FormValue> GetInspectionReportQuery(VestalisEntities context, Guid inspectionReportId, ParameterSearchInspectionReport parameters, ref List<Guid?> inspectionReportItemIds)
        {
            PaginatedList<FormValue> result = new PaginatedList<FormValue>();
            int currentIndex = (parameters.SelectedPage - 1) * parameters.PageSize;
            List<Guid?> inspectionReportItemIdsTemp = new List<Guid?>();

            if (parameters.Collection.ToFilledDictionary().Count > 0)
            {
                //get the data from database
                inspectionReportItemIdsTemp = GetInspectionReportsFiltered(context, inspectionReportId, parameters);

                inspectionReportItemIds = inspectionReportItemIdsTemp;

                result.TotalCount = inspectionReportItemIdsTemp.Count;
                if (!parameters.IsExport)
                    inspectionReportItemIdsTemp = inspectionReportItemIdsTemp.Skip(currentIndex).Take(parameters.PageSize).ToList();
                //get the data from database
                result.Collection = (from formValue in context.FormValues
                                     join inspectionReportItem in context.InspectionReportItems on formValue.InspectionReportItemId equals inspectionReportItem.InspectionReportItemId
                                     where inspectionReportItemIdsTemp.Contains(formValue.InspectionReportItemId) && formValue.IsDeleted == false
                                     orderby inspectionReportItem.CreationDate descending
                                     select formValue).ToList();
            }
            else
            {
                //get the data from database
                GetInspectionReportQueryParameters(parameters, context, inspectionReportId, result, currentIndex, ref inspectionReportItemIds);
            }


            result.NumberOfPages = (int)Math.Ceiling((double)result.TotalCount / (double)parameters.PageSize);
            return result;
        }
        #endregion

        #region GetInspectionReportQueryParameters

        /// <summary>
        /// Get the inspection report items result of the query
        /// </summary>
        /// <param name="parameters">Parameters</param>
        /// <param name="context">Database context</param>
        /// <param name="inspectionReportId">Id of inpection repot</param>
        /// <param name="result">Result</param>
        /// <param name="currentIndex">Current index</param>
        /// <param name="inspectionReportItemIds">List of inspection report item ids</param>
        private static void GetInspectionReportQueryParameters(ParameterSearchInspectionReport parameters, VestalisEntities context, Guid inspectionReportId, PaginatedList<FormValue> result, int currentIndex, ref List<Guid?> inspectionReportItemIds)
        {
            List<Guid?> inspectionReportItemIdsTemp = new List<Guid?>();
            if (parameters.RolesForUser.Contains("Client"))
            {
                inspectionReportItemIdsTemp = (from inspectionReportItem in context.InspectionReportItems
                                               join inspectionReport in context.InspectionReports on inspectionReportItem.InspectionReportId equals inspectionReport.InspectionReportId
                                               join appItem in context.ApprovalItems on inspectionReportItem.InspectionReportItemId equals appItem.InspectionReportItemId
                                               where inspectionReportItem.IsDeleted == false && inspectionReportItem.InspectionReportId == inspectionReportId
                                                     && inspectionReport.ServiceOrderId == parameters.ServiceOrderId && inspectionReport.IsDeleted == false
                                                     && appItem.IsDeleted == false
                                                     && appItem.ApprovalStatus == (int)ApprovalStatus.Completed && appItem.CanPublish == true
                                               orderby inspectionReportItem.CreationDate descending
                                               select inspectionReportItem.InspectionReportItemId).AsEnumerable().Distinct().Cast<Guid?>().ToList();
            }
            else
            {
                inspectionReportItemIdsTemp = (from inspectionReportItem in context.InspectionReportItems
                                               join inspectionReport in context.InspectionReports on inspectionReportItem.InspectionReportId equals inspectionReport.InspectionReportId
                                               join appItem in context.ApprovalItems on inspectionReportItem.InspectionReportItemId equals appItem.InspectionReportItemId
                                               where inspectionReportItem.IsDeleted == false && inspectionReportItem.InspectionReportId == inspectionReportId
                                                     && inspectionReport.ServiceOrderId == parameters.ServiceOrderId && inspectionReport.IsDeleted == false
                                                     && appItem.IsDeleted == false && parameters.RolesForUser.Contains(appItem.RoleName)
                                                     && (appItem.ApprovalStatus == (int)ApprovalStatus.Completed || appItem.ApprovalStatus == (int)ApprovalStatus.Ready)
                                               orderby inspectionReportItem.CreationDate descending
                                               select inspectionReportItem.InspectionReportItemId).AsEnumerable().Cast<Guid?>().ToList();
            }

            inspectionReportItemIds = inspectionReportItemIdsTemp;

            result.TotalCount = inspectionReportItemIdsTemp.Count;
            if (!parameters.IsExport)
                inspectionReportItemIdsTemp = inspectionReportItemIdsTemp.Skip(currentIndex).Take(parameters.PageSize).ToList();

            result.Collection = (from formValue in context.FormValues
                                 join inspectionReportItem in context.InspectionReportItems on formValue.InspectionReportItemId equals inspectionReportItem.InspectionReportItemId
                                 where inspectionReportItemIdsTemp.Contains(formValue.InspectionReportItemId) && formValue.IsDeleted == false
                                 orderby inspectionReportItem.CreationDate descending
                                 select formValue).ToList();
        }

        #endregion

        #region GetServiceOrdersFiltered

        /// <summary>
        /// This method get dinamically the result according of the filters
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="inspectionReportId">if of inspection report item</param>
        /// <param name="parameters">Parameters</param>
        /// <returns></returns>
        private static List<Guid?> GetInspectionReportsFiltered(VestalisEntities context, Guid inspectionReportId, ParameterSearchInspectionReport parameters)
        {
            List<Guid?> result = new List<Guid?>();
            string query1 = string.Empty;
            //filter FormCollection object to get a dictionary only with the key that have a value
            Dictionary<string, string> formCollectionFiltered = parameters.Collection.ToFilledDictionary();

            //get the types of the filters
            var queryServiceOrder = (from formValue in context.FormValues
                                     join inspectionReportItem in context.InspectionReportItems on formValue.InspectionReportItemId equals inspectionReportItem.InspectionReportItemId
                                     where formCollectionFiltered.Keys.Contains(formValue.FieldName) && inspectionReportItem.InspectionReportId == inspectionReportId
                                     select new { FieldName = formValue.FieldName, TypeField = formValue.FieldType }).Distinct();

            //this query filter the result by inspection report
            if (parameters.RolesForUser.Contains("Client"))
            {
                query1 = "select VALUE DISTINCT inspectionReportItem.InspectionReportItemId from VestalisEntities.InspectionReportItems as inspectionReportItem" +
            " inner join  VestalisEntities.ApprovalItems as approvalItem on inspectionReportItem.InspectionReportItemId = approvalItem.InspectionReportItemId" +
            " inner join VestalisEntities.InspectionReports as inspectionReport on inspectionReportItem.InspectionReportId = inspectionReport.InspectionReportId" +
            " inner join VestalisEntities.ServiceOrders as serviceOrder on inspectionReport.ServiceOrderId = serviceOrder.ServiceOrderId" +
            " where inspectionReportItem.IsDeleted = false AND inspectionReportItem.InspectionReportId = GUID '" + inspectionReportId.ToString() + "'" +
            " and ApprovalItem.IsDeleted = false and ApprovalItem.ApprovalStatus = 3 and ApprovalItem.CanPublish = true" +
            " and serviceOrder.BusinessApplicationId = GUID '" + parameters.BusinessApplicationId.ToString() + "'";
            }
            else
            {
                string roles = string.Empty;
                foreach (string item in parameters.RolesForUser)
                {
                    if (item == parameters.RolesForUser.Last())
                        roles += "ApprovalItem.RoleName = '" + item + "'";
                    else
                        roles += "ApprovalItem.RoleName = '" + item + "' or ";
                }
                query1 = "select VALUE inspectionReportItem.InspectionReportItemId from VestalisEntities.InspectionReportItems as inspectionReportItem" +
            " inner join  VestalisEntities.ApprovalItems as approvalItem on inspectionReportItem.InspectionReportItemId = approvalItem.InspectionReportItemId" +
            " inner join VestalisEntities.InspectionReports as inspectionReport on inspectionReportItem.InspectionReportId = inspectionReport.InspectionReportId" +
            " inner join VestalisEntities.ServiceOrders as serviceOrder on inspectionReport.ServiceOrderId = serviceOrder.ServiceOrderId" +
            " where inspectionReportItem.IsDeleted = false AND inspectionReportItem.InspectionReportId = GUID '" + inspectionReportId.ToString() + "'" +
            " and ApprovalItem.IsDeleted = false and (" + roles + ") and (ApprovalItem.ApprovalStatus = 3 or ApprovalItem.ApprovalStatus = 2)" +
            " and serviceOrder.BusinessApplicationId = GUID '" + parameters.BusinessApplicationId.ToString() + "'";
            }


            ObjectQuery<Guid> query1Result = new ObjectQuery<Guid>(query1, context);

            ObjectQuery<Guid> tempQuery = null;

            //for each result of filters, the systems perform a query to filter the corresponding value, but this iteration is not valid when in the filters exist a date range
            foreach (var item in queryServiceOrder)
            {
                string fieldName = item.FieldName;
                switch (item.TypeField)
                {
                    case (int)FieldType.Boolean:
                        tempQuery = GetBooleanQuery(context, formCollectionFiltered, fieldName, tempQuery, query1Result);
                        break;
                    case (int)FieldType.Catalogue:
                        tempQuery = GetCatalogueQuery(context, formCollectionFiltered, fieldName, tempQuery, query1Result, parameters.FieldsWithLike.Contains(fieldName));
                        break;
                    case (int)FieldType.Decimal:
                        tempQuery = GetDecimalQuery(context, formCollectionFiltered, fieldName, tempQuery, query1Result);
                        break;
                    case (int)FieldType.Integer:
                        tempQuery = GetIntegerQuery(context, formCollectionFiltered, fieldName, tempQuery, query1Result);
                        break;
                    case (int)FieldType.MultipleTextLine:
                        tempQuery = GetMultiLineQuery(context, formCollectionFiltered, fieldName, tempQuery, query1Result, parameters.FieldsWithLike.Contains(fieldName));
                        break;
                    case (int)FieldType.RegularExpressionText:
                        tempQuery = GetRegularExpressionQuery(context, formCollectionFiltered, fieldName, tempQuery, query1Result, parameters.FieldsWithLike.Contains(fieldName));
                        break;
                    case (int)FieldType.SingleTextLine:
                        tempQuery = GetSingleTextQuery(context, formCollectionFiltered, fieldName, tempQuery, query1Result, parameters.FieldsWithLike.Contains(fieldName));
                        break;
                    case (int)FieldType.Time:
                        tempQuery = GetTimeQuery(context, formCollectionFiltered, fieldName, tempQuery, query1Result);
                        break;
                    default:
                        break;
                }
            }

            //if in the filter exist a date range, the system will perform the query for filter the results
            if (formCollectionFiltered.Keys.Any(key => key.EndsWith("to") || key.EndsWith("from")))
            {
                tempQuery = GetDateRangeQuery(context, formCollectionFiltered, tempQuery, query1Result);
            }

            //retreive the results of the dynamic query
            if (tempQuery != null)
            {
                var tempResult = tempQuery.ToList();

                if (tempResult.Count > 0)
                    result = tempResult.Cast<Guid?>().ToList();
            }


            return result;
        }
        #endregion

        #region GetTimeQuery
        /// <summary>
        /// Get time query
        /// </summary>
        /// <param name="context">Vestalis context</param>
        /// <param name="formCollectionFiltered">Form collection</param>
        /// <param name="fieldName">Name of the field</param>
        /// <param name="tempQuery">Temporal query</param>
        /// <param name="query1Result">First query</param>
        /// <returns>Object query of Guid</returns>
        private static ObjectQuery<Guid> GetTimeQuery(VestalisEntities context, Dictionary<string, string> formCollectionFiltered, string fieldName, ObjectQuery<Guid> tempQuery, ObjectQuery<Guid> query1Result)
        {
            string timeValue = formCollectionFiltered[fieldName];

            string queryTime = "select VALUE FormValue.InspectionReportItemId from VestalisEntities.FormValues as FormValue where FormValue.IsDeleted = false AND FormValue.TextValue= '"
                               + timeValue + "'  and FormValue.FieldName='" + fieldName + "' AND FormValue.InspectionReportItemId IS NOT NULL";
            ObjectQuery<Guid> objQueryTime = new ObjectQuery<Guid>(queryTime, context);

            if (tempQuery == null)
                tempQuery = query1Result.Intersect(objQueryTime);
            else
                tempQuery = tempQuery.Intersect(objQueryTime);
            return tempQuery;
        }
        #endregion

        #region GetSingleTextQuery

        /// <summary>
        /// Get single text query
        /// </summary>
        /// <param name="context">Vestalis context</param>
        /// <param name="formCollectionFiltered">Form collection</param>
        /// <param name="fieldName">Name of the field</param>
        /// <param name="tempQuery">Temporal query</param>
        /// <param name="query1Result">First query</param>
        /// <param name="isLikeSearch">Is or not a search with like statemet</param>
        /// <returns>Object query of Guid</returns>
        private static ObjectQuery<Guid> GetSingleTextQuery(VestalisEntities context, Dictionary<string, string> formCollectionFiltered, string fieldName, ObjectQuery<Guid> tempQuery, ObjectQuery<Guid> query1Result, bool isLikeSearch = false)
        {
            string singleTextValue = formCollectionFiltered[fieldName];
            string querysingleText = string.Empty;

            if (isLikeSearch)
            {
                querysingleText = "select VALUE FormValue.InspectionReportItemId from VestalisEntities.FormValues as FormValue where FormValue.IsDeleted = false AND FormValue.TextValue like '%"
                     + singleTextValue + "%'  and FormValue.FieldName='" + fieldName + "' AND FormValue.InspectionReportItemId IS NOT NULL";

            }
            else
            {
                querysingleText = "select VALUE FormValue.InspectionReportItemId from VestalisEntities.FormValues as FormValue where FormValue.IsDeleted = false AND FormValue.TextValue= '"
                                     + singleTextValue + "'  and FormValue.FieldName='" + fieldName + "' AND FormValue.InspectionReportItemId IS NOT NULL";
            }

            ObjectQuery<Guid> objQuerysingleText = new ObjectQuery<Guid>(querysingleText, context);

            if (tempQuery == null)
                tempQuery = query1Result.Intersect(objQuerysingleText);
            else
                tempQuery = tempQuery.Intersect(objQuerysingleText);
            return tempQuery;
        }
        #endregion

        #region GetRegularExpressionQuery

        /// <summary>
        /// Get regular expression query
        /// </summary>
        /// <param name="context">Vestalis context</param>
        /// <param name="formCollectionFiltered">Form collection</param>
        /// <param name="fieldName">Name of the field</param>
        /// <param name="tempQuery">Temporal query</param>
        /// <param name="query1Result">First query</param>
        /// <param name="isLikeSearch">Is or not a search with like statemet</param>
        /// <returns>Object query of Guid</returns>
        private static ObjectQuery<Guid> GetRegularExpressionQuery(VestalisEntities context, Dictionary<string, string> formCollectionFiltered, string fieldName, ObjectQuery<Guid> tempQuery, ObjectQuery<Guid> query1Result, bool isLikeSearch)
        {
            string regularExpressionValue = formCollectionFiltered[fieldName];
            string queryRegularExpression = string.Empty;

            if (isLikeSearch)
            {
                queryRegularExpression = "select VALUE FormValue.InspectionReportItemId from VestalisEntities.FormValues as FormValue where FormValue.IsDeleted = false AND FormValue.TextValue like '%"
                                                + regularExpressionValue + "%'  and FormValue.FieldName='" + fieldName + "' AND FormValue.InspectionReportItemId IS NOT NULL";
            }
            else
            {
                queryRegularExpression = "select VALUE FormValue.InspectionReportItemId from VestalisEntities.FormValues as FormValue where FormValue.IsDeleted = false AND FormValue.TextValue= '"
                                                + regularExpressionValue + "'  and FormValue.FieldName='" + fieldName + "' AND FormValue.InspectionReportItemId IS NOT NULL";
            }
            ObjectQuery<Guid> objQueryRegularExpression = new ObjectQuery<Guid>(queryRegularExpression, context);

            if (tempQuery == null)
                tempQuery = query1Result.Intersect(objQueryRegularExpression);
            else
                tempQuery = tempQuery.Intersect(objQueryRegularExpression);
            return tempQuery;
        }
        #endregion

        #region GetMultiLineQuery

        /// <summary>
        /// Get multi line query
        /// </summary>
        /// <param name="context">Vestalis context</param>
        /// <param name="formCollectionFiltered">Form collection</param>
        /// <param name="fieldName">Name of the field</param>
        /// <param name="tempQuery">Temporal query</param>
        /// <param name="query1Result">First query</param>
        /// <param name="isLikeSearch">Is or not a search with like statemet</param>
        /// <returns>Object query of Guid</returns>
        private static ObjectQuery<Guid> GetMultiLineQuery(VestalisEntities context, Dictionary<string, string> formCollectionFiltered, string fieldName, ObjectQuery<Guid> tempQuery, ObjectQuery<Guid> query1Result,bool isLikeSearch)
        {
            string multilineValue = formCollectionFiltered[fieldName];
            string queryMultiText = string.Empty;

            if (isLikeSearch)
            {
                queryMultiText = "select VALUE FormValue.InspectionReportItemId from VestalisEntities.FormValues as FormValue where  FormValue.IsDeleted = false AND FormValue.TextValue like '%"
                                    + multilineValue + "%'  and FormValue.FieldName='" + fieldName + "' AND FormValue.InspectionReportItemId IS NOT NULL";
            }
            else
            {
                queryMultiText = "select VALUE FormValue.InspectionReportItemId from VestalisEntities.FormValues as FormValue where  FormValue.IsDeleted = false AND FormValue.TextValue= '"
                                    + multilineValue + "'  and FormValue.FieldName='" + fieldName + "' AND FormValue.InspectionReportItemId IS NOT NULL";
            }
            
            ObjectQuery<Guid> objQueryMultiText = new ObjectQuery<Guid>(queryMultiText, context);

            if (tempQuery == null)
                tempQuery = query1Result.Intersect(objQueryMultiText);
            else
                tempQuery = tempQuery.Intersect(objQueryMultiText);
            return tempQuery;
        }
        #endregion

        #region GetIntegerQuery
        /// <summary>
        /// Get integer query
        /// </summary>
        /// <param name="context">Vestalis context</param>
        /// <param name="formCollectionFiltered">Form collection</param>
        /// <param name="fieldName">Name of the field</param>
        /// <param name="tempQuery">Temporal query</param>
        /// <param name="query1Result">First query</param>
        /// <returns>Object query of Guid</returns>s
        private static ObjectQuery<Guid> GetIntegerQuery(VestalisEntities context, Dictionary<string, string> formCollectionFiltered, string fieldName, ObjectQuery<Guid> tempQuery, ObjectQuery<Guid> query1Result)
        {
            int intValue = int.Parse(formCollectionFiltered[fieldName]);

            string queryInteger = "select VALUE FormValue.InspectionReportItemId from VestalisEntities.FormValues as FormValue where FormValue.IsDeleted = false AND FormValue.IntegerValue= "
                                  + intValue.ToString() + "  and FormValue.FieldName='" + fieldName + "' AND FormValue.InspectionReportItemId IS NOT NULL";
            ObjectQuery<Guid> objQueryInteger = new ObjectQuery<Guid>(queryInteger, context);


            if (tempQuery == null)
                tempQuery = query1Result.Intersect(objQueryInteger);
            else
                tempQuery = tempQuery.Intersect(objQueryInteger);
            return tempQuery;
        }
        #endregion

        #region GetDecimalQuery
        /// <summary>
        /// Get decimal query
        /// </summary>
        /// <param name="context">Vestalis context</param>
        /// <param name="formCollectionFiltered">Form collection</param>
        /// <param name="fieldName">Name of the field</param>
        /// <param name="tempQuery">Temporal query</param>
        /// <param name="query1Result">First query</param>
        /// <returns>Object query of Guid</returns>
        private static ObjectQuery<Guid> GetDecimalQuery(VestalisEntities context, Dictionary<string, string> formCollectionFiltered, string fieldName, ObjectQuery<Guid> tempQuery, ObjectQuery<Guid> query1Result)
        {
            decimal decimalValue = decimal.Parse(formCollectionFiltered[fieldName]);

            string queryDecimal = "select VALUE FormValue.InspectionReportItemId from VestalisEntities.FormValues as FormValue where FormValue.IsDeleted = false AND FormValue.DecimalValue= "
                                  + decimalValue.ToString() + "  and FormValue.FieldName='" + fieldName + "' AND FormValue.InspectionReportItemId IS NOT NULL";
            ObjectQuery<Guid> objQueryDecimal = new ObjectQuery<Guid>(queryDecimal, context);

            if (tempQuery == null)
                tempQuery = query1Result.Intersect(objQueryDecimal);
            else
                tempQuery = tempQuery.Intersect(objQueryDecimal);
            return tempQuery;
        }
        #endregion

        #region GetCatalogueQuery

        /// <summary>
        /// Get catalogue query
        /// </summary>
        /// <param name="context">Vestalis context</param>
        /// <param name="formCollectionFiltered">Form collection</param>
        /// <param name="fieldName">Name of the field</param>
        /// <param name="tempQuery">Temporal query</param>
        /// <param name="query1Result">First query</param>
        /// <param name="isLikeSearch">Is or not a search with like statemet</param>
        /// <returns>Object query of Guid</returns>
        private static ObjectQuery<Guid> GetCatalogueQuery(VestalisEntities context, Dictionary<string, string> formCollectionFiltered, string fieldName, ObjectQuery<Guid> tempQuery, ObjectQuery<Guid> query1Result, bool isLikeSearch)
        {
            Guid guidValue = Guid.Empty;
            string likeValue = string.Empty;
            string queryCatalogue = string.Empty;
            if (isLikeSearch)
            {
                likeValue = formCollectionFiltered[fieldName];
                queryCatalogue = "select VALUE FormValue.ServiceOrderId from VestalisEntities.FormValues as FormValue "
                                 + " inner join VestalisEntities.CatalogueValues as CatalogueValue on FormValue.CatalogueValueId = CatalogueValue.CatalogueValueId"
                                 + " where FormValue.IsDeleted = false"
                                 + " and FormValue.FieldName='" + fieldName + "' AND FormValue.InspectionReportItemId IS NOT NULL"
                                 + " and CatalogueValue.CatalogueValueData like '" + likeValue + "%' and CatalogueValue.IsDeleted = false";
            }
            else
            {
                guidValue = new Guid(formCollectionFiltered[fieldName]);
                queryCatalogue = "select VALUE FormValue.InspectionReportItemId from VestalisEntities.FormValues as FormValue where FormValue.IsDeleted = false AND FormValue.CatalogueValueId =Guid '"
                               + guidValue.ToString() + "'  and FormValue.FieldName='" + fieldName + "' AND FormValue.InspectionReportItemId IS NOT NULL";
            }


            ObjectQuery<Guid> objQueryGuid = new ObjectQuery<Guid>(queryCatalogue, context);

            if (tempQuery == null)
                tempQuery = query1Result.Intersect(objQueryGuid);
            else
                tempQuery = tempQuery.Intersect(objQueryGuid);
            return tempQuery;
        }
        #endregion

        #region GetBooleanQuery
        /// <summary>
        /// Get bool query
        /// </summary>
        /// <param name="context">Vestalis context</param>
        /// <param name="formCollectionFiltered">Form collection</param>
        /// <param name="fieldName">Name of the field</param>
        /// <param name="tempQuery">Temporal query</param>
        /// <param name="query1Result">First query</param>
        /// <returns>Object query of Guid</returns>
        private static ObjectQuery<Guid> GetBooleanQuery(VestalisEntities context, Dictionary<string, string> formCollectionFiltered, string fieldName, ObjectQuery<Guid> tempQuery, ObjectQuery<Guid> query1Result)
        {
            bool checkValue = bool.Parse(formCollectionFiltered[fieldName]);

            string queryBool = "select VALUE FormValue.InspectionReportItemId from VestalisEntities.FormValues as FormValue where FormValue.IsDeleted = false AND FormValue.CheckValue="
                               + checkValue.ToString() + "  and FormValue.FieldName='" + fieldName + "' AND FormValue.InspectionReportItemId IS NOT NULL";
            ObjectQuery<Guid> objQueryBool = new ObjectQuery<Guid>(queryBool, context);
            if (tempQuery == null)
                tempQuery = query1Result.Intersect(objQueryBool);
            else
                tempQuery = tempQuery.Intersect(objQueryBool);
            return tempQuery;
        }
        #endregion

        #region GetDateRangeQuery
        /// <summary>
        /// Get date range query
        /// </summary>
        /// <param name="context">Vestalis context</param>
        /// <param name="formCollectionFiltered">Form collection</param>
        /// <param name="tempQuery">Temporal query</param>
        /// <param name="query1Result">First query</param>
        /// <returns>Object query of Guid</returns>
        private static ObjectQuery<Guid> GetDateRangeQuery(VestalisEntities context, Dictionary<string, string> formCollectionFiltered, ObjectQuery<Guid> tempQuery, ObjectQuery<Guid> query1Result)
        {
            string[] OrderDatefromValue = formCollectionFiltered[formCollectionFiltered.Keys.First(key => key.EndsWith("from"))].Split(new char[] { '/', '-' }, StringSplitOptions.RemoveEmptyEntries);
            string[] OrderDateToValue = formCollectionFiltered[formCollectionFiltered.Keys.First(key => key.EndsWith("to"))].Split(new char[] { '/', '-' }, StringSplitOptions.RemoveEmptyEntries);
            string fieldName = formCollectionFiltered.Keys.First(key => key.EndsWith("to"));
            fieldName = fieldName.Remove(fieldName.Length - 2, 2);

            DateTime OrderDatefrom = new DateTime(int.Parse(OrderDatefromValue[2]), int.Parse(OrderDatefromValue[1]), int.Parse(OrderDatefromValue[0]));
            DateTime OrderDateTo = new DateTime(int.Parse(OrderDateToValue[2]), int.Parse(OrderDateToValue[1]), int.Parse(OrderDateToValue[0]));

            OrderDatefrom = OrderDatefrom.Date;
            OrderDateTo = OrderDateTo.Date.AddDays(1).AddSeconds(-1);

            string queryDate = "select VALUE FormValue.InspectionReportItemId from VestalisEntities.FormValues as FormValue where FormValue.IsDeleted = false and FormValue.DateValue >= DATETIME '"
                               + OrderDatefrom.ToString("yyyy-MM-dd HH:mm:ss.fffffff") + "' and FormValue.DateValue <= DATETIME '" + OrderDateTo.ToString("yyyy-MM-dd HH:mm:ss.fffffff")
                               + "' and FormValue.FieldName='" + fieldName + "' AND FormValue.InspectionReportItemId IS NOT NULL";
            ObjectQuery<Guid> objQueryDate = new ObjectQuery<Guid>(queryDate, context);

            if (tempQuery == null)
                tempQuery = query1Result.Intersect(objQueryDate);
            else
                tempQuery = tempQuery.Intersect(objQueryDate);
            return tempQuery;
        }
        #endregion

        #region AddInspectionReport
        /// <summary>
        /// Create a new Inspection report item
        /// </summary>
        /// <param name="parameters">Parameters to save the information</param>
        public static Guid AddInspectionReport(ParameterSaveInspectionReport parameters)
        {
            Guid inspectionReportId = GetInspectionReportByName(parameters.InspectionReportName, parameters.ServiceOrderId).InspectionReportId;
            Fields fieldBusinessApplication = CacheHandler.Get(String.Format("Field{0}", parameters.BusinessApplicationId),
                                            () => DynamicFormEngine.GetFields(parameters.BusinessApplicationId));

            string fieldName = string.Empty;
            string valData = string.Empty;
            Guid inspectionReportItemId = Guid.NewGuid();
            InspectionReportItem inspectionReportItem = null;
            using (VestalisEntities context = new VestalisEntities())
            {
                inspectionReportItem = new InspectionReportItem
                {
                    InspectionReportId = inspectionReportId,
                    StatusCode = ConstantApplication.InspectionReportPendingPublish,
                    CreationBy = parameters.UserName,
                    CreationDate = DateTime.UtcNow,
                    InspectionReportItemId = inspectionReportItemId
                };
                context.InspectionReportItems.AddObject(inspectionReportItem);

                AddApprovalItem(parameters, inspectionReportItemId, context, inspectionReportItem);

                foreach (var formCollectionValue in parameters.FormCollection.Keys)
                {
                    fieldName = formCollectionValue.ToString();
                    ValueProviderResult val = parameters.FormCollection.GetValue(fieldName);
                    if (val != null)
                    {
                        //Get the form value for the specific field
                        valData = val.AttemptedValue.ToString();
                        if (!String.IsNullOrEmpty(valData))
                        {
                            //Set the form value
                            FormValue formValue = SetFormValueToInspectionReport(parameters, inspectionReportItemId, fieldName, valData,
                                                                          fieldBusinessApplication);
                            //Add the form value to the context
                            context.FormValues.AddObject(formValue);
                        }
                    }
                }
                context.SaveChanges();
            }
            return inspectionReportItem.InspectionReportItemId;
        }
        #endregion

        #region AddApprovalItem
        /// <summary>
        /// Add approval items
        /// </summary>
        /// <param name="parameterSaveInspectionReport">parameterSaveInspectionReport</param>
        /// <param name="inspectionReportItemId">inspectionReportItemId</param>
        /// <param name="context">Data base context</param>
        /// <param name="inspectionReportItem">inspectionReportItem</param>
        private static void AddApprovalItem(ParameterSaveInspectionReport parameterSaveInspectionReport, Guid inspectionReportItemId, VestalisEntities context, InspectionReportItem inspectionReportItem)
        {
            ApprovalItem approvalItem = null;

            List<ValidationRole> roles = (from role in context.ValidationRoles
                                          where role.BusinessApplicationId == parameterSaveInspectionReport.BusinessApplicationId
                                          && role.IsDeleted == false
                                          select role).ToList();

            foreach (ValidationRole role in roles)
            {
                approvalItem = new ApprovalItem
                {
                    ApprovalLevel = role.RoleLevel,
                    CanPublish = role.CanPublish,
                    CreationBy = parameterSaveInspectionReport.UserName,
                    CreationDate = DateTime.UtcNow,
                    InspectionReportItemId = inspectionReportItemId,
                    IsReadOnly = role.IsReadOnly,
                    RoleName = role.RoleName,
                    ApprovalStatus = (int)ApprovalStatus.Waiting
                };
                if (role.RoleLevel == 1)
                {
                    if (!role.CanPublish.GetValueOrDefault())
                    {
                        approvalItem.ApprovalStatus = (int)ApprovalStatus.Completed;
                        inspectionReportItem.CurrentCompletedLevel = 1;
                    }
                    else
                    {
                        approvalItem.ApprovalStatus = (int)ApprovalStatus.Ready;
                    }
                }
                if (role.RoleLevel == 2)
                {
                    approvalItem.ApprovalStatus = (int)ApprovalStatus.Ready;
                }
                context.ApprovalItems.AddObject(approvalItem);
            }
        }
        #endregion

        #region EditInspectionReport
        /// <summary>
        /// Save the service order in the database
        /// </summary>
        /// <param name="parameters">Inspection report parameters</param>
        public static void EditInspectionReport(ParameterSaveInspectionReport parameters)
        {
            IList<string> keyNames = new List<string>();
            //Get the fields definition for the business application
            Fields fieldBusinessApplication = CacheHandler.Get(String.Format("Field{0}", parameters.BusinessApplicationId),
                                            () => DynamicFormEngine.GetFields(parameters.BusinessApplicationId));

            using (VestalisEntities context = new VestalisEntities())
            {
                //Get the form values inserted in the database but they aren't in formCollection.Keys
                foreach (var formCollectionValue in parameters.FormCollection.Keys)
                {
                    keyNames.Add(formCollectionValue.ToString());
                }

                var formsNotSent = (from formValues in context.FormValues
                                    where
                                        formValues.ServiceOrderId == parameters.ServiceOrderId &&
                                        formValues.InspectionReportItemId == parameters.InspectionReportItemId &&
                                        formValues.IsDeleted == false &&
                                        !keyNames.Contains(formValues.FieldName)
                                    select formValues);

                foreach (var formNotSent in formsNotSent)
                {
                    formNotSent.IsDeleted = true;
                    formNotSent.ModificationBy = parameters.UserName;
                    formNotSent.ModificationDate = DateTime.UtcNow;
                }

                //For each field form
                foreach (var formCollectionValue in parameters.FormCollection.Keys)
                {
                    SetFieldValuesEditInspection(parameters, context, formCollectionValue, fieldBusinessApplication);
                }
                context.SaveChanges();
            }

            EditApprovalItem(parameters);
        }

        /// <summary>
        /// Insert the edited values of the inspection report
        /// </summary>
        /// <param name="parameters">Inspection report parameters</param>
        /// <param name="context">EF Context</param>
        /// <param name="formCollectionValue"></param>
        /// <param name="fieldBusinessApplication"></param>
        private static void SetFieldValuesEditInspection(ParameterSaveInspectionReport parameters, VestalisEntities context, object formCollectionValue, Fields fieldBusinessApplication)
        {
            string fieldName;
            string valData;
            fieldName = formCollectionValue.ToString();
            ValueProviderResult val = parameters.FormCollection.GetValue(fieldName);
            //Get the form value of the service order according the field name
            FormValue formValueToEdit = (from formValues in context.FormValues
                                         where
                                             formValues.ServiceOrderId == parameters.ServiceOrderId &&
                                             formValues.InspectionReportItemId == parameters.InspectionReportItemId &&
                                             formValues.IsDeleted == false &&
                                             formValues.FieldName == fieldName
                                         select formValues).FirstOrDefault();

            if (val != null)
            {
                //Get the form value for the specific field
                valData = val.AttemptedValue.ToString();

                if (!String.IsNullOrEmpty(valData))
                {
                    if (formValueToEdit != null)
                    {
                        SetFormValueToEditInspectionReport(formValueToEdit, valData, parameters.UserName);
                    }
                    else
                    {
                        //Set the form value
                        FormValue formValue = SetFormValueToInspectionReport(parameters, parameters.InspectionReportItemId, fieldName,
                                                                             valData,
                                                                             fieldBusinessApplication);
                        //Add the form value to the context
                        context.FormValues.AddObject(formValue);
                    }
                }
                //Case this field had a value in the past, but the user has updated now without value
                else if (formValueToEdit != null)
                {
                    formValueToEdit.IsDeleted = true;
                    formValueToEdit.ModificationBy = parameters.UserName;
                    formValueToEdit.ModificationDate = DateTime.UtcNow;
                }
            }
        }

        #endregion

        #region EditApprovalItem
        /// <summary>
        /// Edit approval items
        /// </summary>
        /// <param name="parameters">Parameters</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private static void EditApprovalItem(ParameterSaveInspectionReport parameters)
        {
            using (VestalisEntities context = new VestalisEntities())
            {
                InspectionReportItem inspectionReportItem = context.InspectionReportItems.FirstOrDefault(data => data.InspectionReportItemId == parameters.InspectionReportItemId);
                List<ValidationRole> roles = null;
                ValidationRole userRole = null;
                List<ApprovalItem> approvalItems = null;
                ApprovalItem approvalItem = null;
                bool isRole1Completed = false;

                if (inspectionReportItem != null)
                {
                    int currentApprovalLevel = inspectionReportItem.CurrentCompletedLevel.GetValueOrDefault();

                    userRole = (from role in context.ValidationRoles
                                where role.BusinessApplicationId == parameters.BusinessApplicationId
                                && role.IsDeleted == false && parameters.RolesForUser.Contains(role.RoleName)
                                select role).FirstOrDefault();

                    if (userRole.RoleLevel == 1)
                    {

                        roles = (from role in context.ValidationRoles
                                 where role.BusinessApplicationId == parameters.BusinessApplicationId
                                 && role.IsDeleted == false
                                 orderby role.RoleLevel
                                 select role).ToList();


                        approvalItems = (from appItems in context.ApprovalItems
                                         where appItems.InspectionReportItemId == parameters.InspectionReportItemId
                                         && appItems.IsDeleted == false
                                         select appItems).ToList();

                    }
                    else
                    {
                        roles = (from role in context.ValidationRoles
                                 where role.BusinessApplicationId == parameters.BusinessApplicationId
                                 && role.IsDeleted == false && role.RoleLevel >= currentApprovalLevel
                                 orderby role.RoleLevel
                                 select role).ToList();

                        approvalItems = (from appItems in context.ApprovalItems
                                         where appItems.InspectionReportItemId == parameters.InspectionReportItemId
                                         && appItems.IsDeleted == false && appItems.ApprovalLevel >= currentApprovalLevel
                                         select appItems).ToList();

                    }


                    foreach (ApprovalItem appItem in approvalItems)
                    {
                        appItem.IsDeleted = true;
                        appItem.ModificationBy = parameters.UserName;
                        appItem.ModificationDate = DateTime.UtcNow;
                    }

                    foreach (ValidationRole role in roles)
                    {
                        approvalItem = new ApprovalItem
                        {
                            ApprovalLevel = role.RoleLevel,
                            CanPublish = role.CanPublish,
                            CreationBy = parameters.UserName,
                            CreationDate = DateTime.UtcNow,
                            InspectionReportItemId = parameters.InspectionReportItemId,
                            IsReadOnly = role.IsReadOnly,
                            RoleName = role.RoleName,
                            ApprovalStatus = (int)ApprovalStatus.Waiting
                        };
                        if (role.RoleLevel == 1)
                        {
                            if (!role.CanPublish.GetValueOrDefault())
                            {
                                approvalItem.ApprovalStatus = (int)ApprovalStatus.Completed;
                                inspectionReportItem.CurrentCompletedLevel = 1;
                                inspectionReportItem.ModificationBy = parameters.UserName;
                                inspectionReportItem.ModificationDate = DateTime.UtcNow;
                                isRole1Completed = true;
                            }
                            else
                            {
                                approvalItem.ApprovalStatus = (int)ApprovalStatus.Ready;
                                isRole1Completed = false;
                            }


                        }
                        else if (isRole1Completed && role.RoleLevel == 2)
                        {
                            approvalItem.ApprovalStatus = (int)ApprovalStatus.Ready;
                        }
                        else if (role.RoleLevel == currentApprovalLevel)
                        {
                            approvalItem.ApprovalStatus = (int)ApprovalStatus.Ready;
                            inspectionReportItem.CurrentCompletedLevel = (currentApprovalLevel - 1);
                            inspectionReportItem.ModificationBy = parameters.UserName;
                            inspectionReportItem.ModificationDate = DateTime.UtcNow;
                        }
                        context.ApprovalItems.AddObject(approvalItem);
                    }
                }
                context.SaveChanges();
            }
        }
        #endregion

        #region DeleteInspectionReport
        /// <summary>
        /// Delete service order
        /// </summary>
        /// <param name="inspectionReporItemtId">Inspection report item id</param>
        /// <param name="userName">User name</param>
        public static void DeleteInspectionReport(Guid inspectionReporItemtId, string userName)
        {
            using (VestalisEntities ctx = new VestalisEntities())
            {
                //Get the service order
                InspectionReportItem inspectionReportItem =
                    (from inspectionReportItemQuery in ctx.InspectionReportItems
                     where inspectionReportItemQuery.InspectionReportItemId == inspectionReporItemtId
                     select inspectionReportItemQuery).FirstOrDefault();
                if (inspectionReportItem != null)
                {
                    //Delete the record
                    inspectionReportItem.IsDeleted = true;
                    inspectionReportItem.ModificationBy = userName;
                    inspectionReportItem.ModificationDate = DateTime.UtcNow;
                    ctx.SaveChanges();
                }
            }
        }
        #endregion

        #region DeleteSelectedInspectionReports
        /// <summary>
        /// Delete all selected report items 
        /// </summary>
        /// <param name="selectedIds">Ids of selected report items</param>
        /// <param name="userName">The name of the current user</param>
        public static void DeleteSelectedInspectionReports(List<Guid> selectedIds,string userName)
        {
            using (VestalisEntities context = new VestalisEntities())
            {
                var selectedReportItems = (from inspectionReportItem in context.InspectionReportItems
                                           where selectedIds.Contains(inspectionReportItem.InspectionReportItemId)
                                           select inspectionReportItem).ToList();

                if (selectedReportItems != null && selectedReportItems.Count > 0)
                {
                    selectedReportItems.ForEach(inspectionReportItem =>
                    {
                        //Delete the record
                        inspectionReportItem.IsDeleted = true;
                        inspectionReportItem.ModificationBy = userName;
                        inspectionReportItem.ModificationDate = DateTime.UtcNow;
                        context.SaveChanges();
                    });
                }
            }
        }
        #endregion

        #region SetFormValueToEditInspectionReport
        /// <summary>
        /// Set form values when a service order is editing
        /// </summary>
        /// <param name="formValue">FormValue to be saved in the database</param>
        /// <param name="valData">Form value</param>
        /// <param name="userName">User name for auditing</param>
        private static void SetFormValueToEditInspectionReport(FormValue formValue, string valData, string userName)
        {
            switch (formValue.FieldType)
            {
                case (int)FieldType.SingleTextLine:
                case (int)FieldType.MultipleTextLine:
                case (int)FieldType.RegularExpressionText:
                case (int)FieldType.Time:
                case (int)FieldType.AutoComplete:
                    formValue.TextValue = valData;
                    break;
                case (int)FieldType.Datepicker:
                    formValue.DateValue = Convert.ToDateTime(valData, new System.Globalization.CultureInfo("fr-FR"));
                    break;
                case (int)FieldType.Decimal:
                    formValue.DecimalValue = Convert.ToDecimal(valData);
                    break;
                case (int)FieldType.Integer:
                    formValue.IntegerValue = Convert.ToInt32(valData);
                    break;
                case (int)FieldType.Boolean:
                    if (valData == "on")
                    {
                        formValue.CheckValue = true;
                    }
                    break;
                case (int)FieldType.Catalogue:
                    formValue.CatalogueValueId = new Guid(valData);
                    break;
                case (int)FieldType.User:
                    formValue.UserName = valData;
                    break;
            }

            formValue.ModificationBy = userName;
            formValue.ModificationDate = DateTime.UtcNow;
        }
        #endregion

        #region SetFormValueToSaveOrder
        /// <summary>
        /// Create a FormValue according each value entered in the form by the user. It is convert to the appropiated data type according the field type
        /// </summary>
        /// <param name="parameterSaveInspectionReport">Class parameter</param>
        /// <param name="inspectionReportItemId">Id of inspection report item</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="valData">Val data</param>
        /// <param name="fieldBusinessApplication">Fields obtained from the XML </param>
        /// <returns>Form value to be inserted in the database</returns>
        private static FormValue SetFormValueToInspectionReport(ParameterSaveInspectionReport parameterSaveInspectionReport, Guid inspectionReportItemId,
            string fieldName, string valData, Fields fieldBusinessApplication)
        {
            FormValue formValue = new FormValue();
            formValue.ServiceOrderId = parameterSaveInspectionReport.ServiceOrderId;
            formValue.InspectionReportItemId = inspectionReportItemId;
            formValue.FieldName = fieldName;

            Field field = fieldBusinessApplication.Items.Single(x => x.FieldName == fieldName);

            if (field is FieldsSingleTextLineField)
            {
                formValue.FieldType = (int)FieldType.SingleTextLine;
                formValue.TextValue = valData;
            }
            else if (field is FieldsDatepickerField)
            {
                formValue.FieldType = (int)FieldType.Datepicker;
                string[] dateValue = valData.Split(new[] { '-', '/' });
                if (dateValue.Length > 1)
                    formValue.DateValue = new DateTime(int.Parse(dateValue[2]), int.Parse(dateValue[1]), int.Parse(dateValue[0]));
            }
            else if (field is FieldsDecimalField)
            {
                formValue.FieldType = (int)FieldType.Decimal;
                formValue.DecimalValue = Convert.ToDecimal(valData);
            }
            else if (field is FieldsIntegerField)
            {
                formValue.FieldType = (int)FieldType.Integer;
                formValue.IntegerValue = Convert.ToInt32(valData);
            }
            else if (field is FieldsMultipleTextLineField)
            {
                formValue.FieldType = (int)FieldType.MultipleTextLine;
                formValue.TextValue = valData;
            }
            else if (field is FieldsRegularExpressionTextField)
            {
                formValue.FieldType = (int)FieldType.RegularExpressionText;
                formValue.TextValue = valData;
            }
            else if (field is FieldsTimeField)
            {
                formValue.FieldType = (int)FieldType.Time;
                formValue.TextValue = valData;
            }
            else if (field is FieldsUserField)
            {
                formValue.FieldType = (int)FieldType.User;
                formValue.UserName = valData;
            }
            else if (field is FieldsBooleanField)
            {
                formValue.FieldType = (int)FieldType.Boolean;
                if (valData == "on")
                {
                    formValue.CheckValue = true;
                }
            }
            else if (field is FieldsCatalogueField)
            {
                formValue.FieldType = (int)FieldType.Catalogue;
                formValue.CatalogueValueId = new Guid(valData);
            }
            else if (field is FieldsAutoCompleteTextField)
            {
                formValue.FieldType = (int)FieldType.AutoComplete;
                formValue.TextValue = valData;
            }
            formValue.CreationBy = parameterSaveInspectionReport.UserName;
            formValue.CreationDate = DateTime.UtcNow;
            formValue.ModificationBy = parameterSaveInspectionReport.UserName;
            formValue.ModificationDate = DateTime.UtcNow;
            return formValue;
        }
        #endregion

        #region PublishValidateInspectionReport

        /// <summary>
        /// Advance one step in the validation process
        /// </summary>
        /// <param name="inspectionReportItemId">Id of inspection report</param>
        /// <param name="userName">User name</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static void PublishValidateInspectionReport(Guid inspectionReportItemId, string userName)
        {
            int approvalLevel = 0;
            using (VestalisEntities context = new VestalisEntities())
            {
                //get the current approval item
                ApprovalItem approvalItem = (from appItem in context.ApprovalItems
                                             where appItem.InspectionReportItemId == inspectionReportItemId
                                                    && appItem.IsDeleted == false
                                                    && appItem.ApprovalStatus == (int)ApprovalStatus.Ready
                                             select appItem).FirstOrDefault();

                //if exist, update the status
                if (approvalItem != null)
                {
                    approvalLevel = approvalItem.ApprovalLevel;
                    approvalItem.ApprovalStatus = (int)ApprovalStatus.Completed;
                    approvalItem.ModificationBy = userName;
                    approvalItem.ModificationDate = DateTime.UtcNow;

                    //Get the next approval item
                    ApprovalItem approvalItem2 = (from appItem in context.ApprovalItems
                                                  where appItem.InspectionReportItemId == inspectionReportItemId
                                                         && appItem.IsDeleted == false
                                                         && appItem.ApprovalStatus == (int)ApprovalStatus.Waiting
                                                         && appItem.ApprovalLevel == (approvalLevel + 1)
                                                  select appItem).FirstOrDefault();

                    //if exist, update the status
                    if (approvalItem2 != null)
                    {
                        approvalItem2.ApprovalStatus = (int)ApprovalStatus.Ready;
                        approvalItem2.ModificationBy = userName;
                        approvalItem2.ModificationDate = DateTime.UtcNow;
                    }

                    //update the inspection report item with the current completed level
                    InspectionReportItem currentInspectionReportItem = (from inspectionReportItem in context.InspectionReportItems
                                                                        where inspectionReportItem.InspectionReportItemId == inspectionReportItemId
                                                                        select inspectionReportItem).FirstOrDefault();

                    currentInspectionReportItem.CurrentCompletedLevel = approvalLevel;
                    currentInspectionReportItem.ModificationBy = userName;
                    currentInspectionReportItem.ModificationDate = DateTime.UtcNow;
                    if (approvalItem.CanPublish.GetValueOrDefault())
                    {
                        currentInspectionReportItem.PublicationDate = DateTime.UtcNow;
                    }



                    //save all changes
                    context.SaveChanges();
                }

            }
        }
        #endregion

        #region PublishValidateAll

        /// <summary>
        /// Validate or publish all inspection report items
        /// </summary>
        /// <param name="parameters">Parameters of PublishValidateSelected method</param>
        public static void PublishValidateSelected(ParameterPublishValidateInspectionReports parameters)
        {
            Guid inspectionReportId = GetInspectionReportByName(parameters.InspectionReportName, parameters.ServiceOrderId).InspectionReportId;
            List<ApprovalItem> approvalItems = null;
            int approvalLevel = 0;
            Guid inspectionReportItemId = Guid.Empty;
            using (VestalisEntities context = new VestalisEntities())
            {
                approvalItems = GetApprovalItems(context, parameters.RolesForUser,parameters.SelectedIds).Where(data => data.ApprovalStatus == (int)ApprovalStatus.Ready
                    && data.CanPublish == parameters.IsPublish).ToList();
                foreach (ApprovalItem approvalItem in approvalItems)
                {
                    approvalLevel = 0;
                    approvalLevel = approvalItem.ApprovalLevel;
                    approvalItem.ApprovalStatus = (int)ApprovalStatus.Completed;
                    approvalItem.ModificationBy = parameters.UserName;
                    approvalItem.ModificationDate = DateTime.UtcNow;

                    inspectionReportItemId = approvalItem.InspectionReportItemId.GetValueOrDefault();

                    //Get the next approval item
                    ApprovalItem approvalItem2 = (from appItem in context.ApprovalItems
                                                  where appItem.InspectionReportItemId == inspectionReportItemId
                                                         && appItem.IsDeleted == false
                                                         && appItem.ApprovalStatus == (int)ApprovalStatus.Waiting
                                                         && appItem.ApprovalLevel == (approvalLevel + 1)
                                                  select appItem).FirstOrDefault();

                    //if exist, update the status
                    if (approvalItem2 != null)
                    {
                        approvalItem2.ApprovalStatus = (int)ApprovalStatus.Ready;
                        approvalItem2.ModificationBy = parameters.UserName;
                        approvalItem2.ModificationDate = DateTime.UtcNow;
                    }

                    //update the inspection report item with the current completed level
                    InspectionReportItem currentInspectionReportItem = (from inspectionReportItem in context.InspectionReportItems
                                                                        where inspectionReportItem.InspectionReportItemId == inspectionReportItemId
                                                                        select inspectionReportItem).FirstOrDefault();

                    currentInspectionReportItem.CurrentCompletedLevel = approvalLevel;
                    currentInspectionReportItem.ModificationBy = parameters.UserName;
                    currentInspectionReportItem.ModificationDate = DateTime.UtcNow;
                    if (approvalItem.CanPublish.GetValueOrDefault())
                    {
                        currentInspectionReportItem.PublicationDate = DateTime.UtcNow;
                    }

                }
                //save all changes
                context.SaveChanges();
            }
        }
        #endregion

        #region UnPublishInspectionReport
        /// <summary>
        /// Un publish an inspection report
        /// </summary>
        /// <param name="inspectionReportItemId">Inspection report id</param>
        /// <param name="userName">User name</param>
        /// <param name="userRoles">Roles of the user</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static void UnPublishInspectionReport(Guid inspectionReportItemId, string userName, List<string> userRoles)
        {
            using (VestalisEntities context = new VestalisEntities())
            {

                //Get approval item that have can publish equals true and approval status completed for the roles of the logged user
                ApprovalItem currentAppItem = (from appItem in context.ApprovalItems
                                               where
                                                   appItem.IsDeleted == false &&
                                                   userRoles.Contains(appItem.RoleName)
                                                   && appItem.InspectionReportItemId == inspectionReportItemId
                                                   && appItem.CanPublish == true &&
                                                   appItem.ApprovalStatus == (int)ApprovalStatus.Completed
                                               select appItem).FirstOrDefault();

                if (currentAppItem != null)
                {
                    //if the current inspection report item was published && and get the approval status completed, the system continue with the process
                    if (currentAppItem.InspectionReportItem.PublicationDate != null &&
                        currentAppItem.ApprovalStatus == (int)ApprovalStatus.Completed)
                    {
                        currentAppItem.IsDeleted = true;
                        currentAppItem.ModificationBy = userName;
                        currentAppItem.ModificationDate = DateTime.UtcNow;

                        ApprovalItem newAppItem = new ApprovalItem
                                                      {
                                                          InspectionReportItemId = inspectionReportItemId,
                                                          RoleName = currentAppItem.RoleName,
                                                          ApprovalLevel = currentAppItem.ApprovalLevel,
                                                          CanPublish = currentAppItem.CanPublish,
                                                          IsReadOnly = currentAppItem.IsReadOnly,
                                                          ApprovalStatus = (int)ApprovalStatus.Ready,
                                                          CreationBy = userName,
                                                          CreationDate = DateTime.UtcNow
                                                      };
                        context.ApprovalItems.AddObject(newAppItem);

                        InspectionReportItem currentInspectionReportItem =
                            (from inspectionReportItem in context.InspectionReportItems
                             where inspectionReportItem.InspectionReportItemId == inspectionReportItemId
                             select inspectionReportItem).FirstOrDefault();

                        if (currentInspectionReportItem != null)
                        {
                            currentInspectionReportItem.ModificationBy = userName;
                            currentInspectionReportItem.ModificationDate = DateTime.UtcNow;
                            currentInspectionReportItem.PublicationDate = null;
                            currentInspectionReportItem.CurrentCompletedLevel -= 1;
                        }

                    }
                }
                context.SaveChanges();
            }
        }
        #endregion

        #region GetInspectionReportLinks

        /// <summary>
        /// Get the list of the inspection reports related to the service order
        /// </summary>
        /// <param name="serviceOrderId">Service order</param>
        /// <param name="isClient">Flag to know if the user is a client</param>
        /// <returns>List of inspection reports</returns>
        public static IList<string> GetInspectionReportLinks(Guid? serviceOrderId, bool isClient)
        {
            IList<InspectionReport> inspectionReportList;
            IList<string> linkList = new List<string>();
            using (VestalisEntities ctx = new VestalisEntities())
            {
                if (isClient)
                {
                    //Get the inspection reports related to the service order, and it is counted their items published
                    var inspectionReportCountItems =
                        (from inspectionReport in ctx.InspectionReports
                         where
                             inspectionReport.ServiceOrderId == serviceOrderId &&
                             inspectionReport.IsDeleted == false
                             && inspectionReport.IsClientVisible == isClient
                         orderby inspectionReport.FormOrder
                         select new { inspectionReport.InspectionReportId, inspectionReport.FormName, c = inspectionReport.InspectionReportItems.Count(i => i.IsDeleted == false && i.PublicationDate.HasValue) }).ToList();

                    foreach (var inspectionReportItem in inspectionReportCountItems)
                    {
                        if (inspectionReportItem.c > 0)
                            linkList.Add(inspectionReportItem.FormName);
                    }

                    //Verify how many pictures corresponding to the service order
                    int countPictures =
                            ctx.Pictures.Count(item => item.IsDeleted == false && 
                                item.ServiceOrderId == serviceOrderId && 
                                item.InspectionReportItemId == null);

                    //Verify how many documents corresponding to the service order
                    int countDocuments = ctx.Documents.Count(item => item.IsDeleted == false && item.ServiceOrderId == serviceOrderId);

                    //Add the link when there are pictures
                    if (countPictures > 0)
                    {
                        linkList.Add("Picture");
                    }

                    //Add the link when there are documents
                    if (countDocuments > 0)
                    {
                        linkList.Add("Document");
                    }
                }
                else
                {
                    inspectionReportList = (from inspectionReport in ctx.InspectionReports
                                            where
                                                inspectionReport.ServiceOrderId == serviceOrderId &&
                                                inspectionReport.IsDeleted == false
                                            orderby inspectionReport.FormOrder
                                            select inspectionReport).ToList();
                    foreach (var inspectionReport in inspectionReportList)
                    {
                        linkList.Add(inspectionReport.FormName);
                    }

                    linkList.Add("Picture" );
                    linkList.Add("Document");
                }
            }
            return linkList;
        }
        #endregion

        #region GetAutoCompleteItemSource
        /// <summary>
        /// Execute the sql statement that comes from autocomplete field
        /// </summary>
        /// <param name="command">Command to execute</param>
        /// <returns>List of strings</returns>
        public static List<string> GetAutoCompleteItemSource(string command)
        {
            using (VestalisEntities context = new VestalisEntities())
            {
                //The system creates the object that will be executed for getting the result.
                ObjectQuery<string> result = new ObjectQuery<string>(command, context);
                //The system performs the execution of the query.
                return result.ToList();
            }
        }
        #endregion
    }
}
