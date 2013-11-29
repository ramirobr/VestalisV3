using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Security;
using Cotecna.Vestalis.Core.DynamicForm;
using Cotecna.Vestalis.Entities;

namespace Cotecna.Vestalis.Core
{
    /// <summary>
    /// This class contanis the methods for manage all about service orders
    /// </summary>
    public static class ServiceOrderBusiness
    {
        #region methods

        #region GetServiceOrderGridDefinition

        /// <summary>
        /// Get the definition to create the search service order screen
        /// </summary>
        /// <param name="businessApplicationId">Business application identifier</param>
        /// <param name="clientVisible">Is visible or not for the client</param>
        /// <returns>Values to build dynamically the search service order screen</returns>
        public static DynamicDataGrid GetServiceOrderGridDefinition(Guid businessApplicationId, bool clientVisible)
        {
            DynamicDataGrid dynamicGrid = new DynamicDataGrid();
            //Get the xml form definition of the service order given for the business application
            Form serviceOrder = CacheHandler.Get(String.Format("Form{0}", businessApplicationId),
                                                 () =>
                                                 DynamicFormEngine.GetFormDefinition(businessApplicationId,
                                                                                       FormType.ServiceOrder, clientVisible));

            //Get the fields definition for the business application
            Fields fieldBusinessApplication = CacheHandler.Get(String.Format("Field{0}", businessApplicationId),
                                                               () =>
                                                               DynamicFormEngine.GetFields(businessApplicationId));

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
                                Width = string.IsNullOrEmpty(field.Width) ? 0: int.Parse(field.Width),
                                ExcelColumnWidth = string.IsNullOrEmpty(field.ExcelColumnWidth) ? 0: int.Parse(field.ExcelColumnWidth)
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
            dynamicGrid.BusinessApplicationName = AuthorizationBusiness.GetBusinessApplicationById(businessApplicationId).BusinessApplicationName;
            dynamicGrid.Filters = filters;
            dynamicGrid.FormName = serviceOrder.Name;
            dynamicGrid.CaptionBreadcrumbs = serviceOrder.CaptionBreadcrumbs;
            dynamicGrid.CaptionTitle = serviceOrder.CaptionTitle;
            return dynamicGrid;
        }
        #endregion

        #region GetServiceOrderForm
        /// <summary>
        /// Get the definition of a new service order for a specific business application
        /// </summary>
        /// <param name="businessApplicationId">Business application identifier</param>
        /// <param name="serviceOrderId">Service Order identifier</param>
        /// <param name="isClient">Optional Parameter used for filter the list of inspection reports</param>
        /// <param name="onlyHeader">Get only the service order header data required in inspection reports</param>
        /// <returns>Service order form data</returns>
        public static Form GetServiceOrderForm(Guid businessApplicationId, Guid? serviceOrderId, bool isClient = false, bool onlyHeader = false)
        {
            IList<FormValue> formValuesList = new List<FormValue>();
            FormValue formValue = null;
            IList<Field> _serviceOrderHeader = new List<Field>();
            Form serviceOrder = null;
            //Get the xml form definition of the service order given for the business application
            if (serviceOrderId.HasValue)
            {
                serviceOrder = DynamicFormEngine.GetExistingServiceOrderForm(serviceOrderId.Value);
            }
            else
            {
                serviceOrder = CacheHandler.Get(String.Format("Form{0}", businessApplicationId),
                                            () =>
                                            DynamicFormEngine.GetFormDefinition(businessApplicationId, FormType.ServiceOrder, isClient));
            }


            //Get the fields definition for the business application
            Fields fieldBusinessApplication = CacheHandler.Get(String.Format("Field{0}", businessApplicationId),
                                            () => DynamicFormEngine.GetFields(businessApplicationId));

            //Get the service field values for editing
            if (serviceOrderId.HasValue)
            {
                formValuesList = GetServiceOrderFormValues(serviceOrderId);
            }

            //serviceOrder.Rules.DefaultValueDependentOnCatalogue.TextField
            foreach (var section in serviceOrder.Sections)
            {
                foreach (FormSectionElement element in section.FormElements)
                {
                   
                    if ((onlyHeader && element.IsInspectionReportVisible) || !onlyHeader || element.IsOrderIdentifier)
                    {
                        if ((element.IsVisibleClient && isClient) || !isClient)
                        {
                            element.Field = fieldBusinessApplication.Items.Single(x => x.FieldName == element.Identifier);
                            element.Field.InitFieldType(businessApplicationId);
                            element.Field.FillFormValidations();

                            //Case edit
                            if (serviceOrderId.HasValue)
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

                            if (element.IsInspectionReportVisible)
                            {
                                _serviceOrderHeader.Add(element.Field);
                            }

                            //Add the order field identifier
                            if(element.IsOrderIdentifier)
                            {
                                serviceOrder.OrderIdentifier = element.Field;
                            }
                        }
                        else
                        {
                            section.FormElements =
                            section.FormElements.Except(section.FormElements.Where(item => item.Identifier == element.Identifier)).ToArray();
                        }
                    }
                }
            }
            serviceOrder.ServiceOrderHeader = _serviceOrderHeader;
            return serviceOrder;
        }

        /// <summary>
        /// Set the links of the service order
        /// </summary>
        /// <param name="serviceOrderId">Service Order identifier</param>
        /// <returns></returns>
        private static IList<FormValue> GetServiceOrderFormValues(Guid? serviceOrderId)
        {
            IList<FormValue> formValuesList;
            using (VestalisEntities ctx = new VestalisEntities())
            {
                formValuesList =
                    (from formValues in ctx.FormValues
                     where formValues.IsDeleted == false && formValues.ServiceOrderId == serviceOrderId.Value
                     select formValues).ToList();
            }
            return formValuesList;
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

        #region SearchOrderList
        /// <summary>
        /// Get the list of service orders
        /// </summary>
        /// <param name="parameters">Parameters</param>
        /// <returns>DynamicDataGrid</returns>
        public static DynamicDataGrid SearchOrderList(ParameterSearchServicerOrder parameters)
        {
            DynamicDataGrid dynamicDataGrid = new DynamicDataGrid();
            DynamicDataRow dataRow = null;
            string fieldName = string.Empty;
            string fieldValue = string.Empty;
            int fieldType = 0;
            List<PropertyInfo> filterProperties = null;
            List<string> gridFields = new List<string>();
            bool isOk = false;
            Dictionary<string, DynamicDataRowValue> temRowValues = null;
            List<FormValue> serviceOrderQuery = null;
            PaginatedList<FormValue> resultQuery = null;
            List<ApprovalItem> approvalItems = null;
            //obtain the definition of the form
            Form serviceOrder = CacheHandler.Get(String.Format("Form{0}", parameters.BusinessApplicationId),
                                                 () =>
                                                 DynamicFormEngine.GetFormDefinition(parameters.BusinessApplicationId,
                                                                                       FormType.ServiceOrder, false));
            using (VestalisEntities context = new VestalisEntities())
            {

                //Get the values from data base
                resultQuery = GetServiceOrderQuery(context, parameters);

                serviceOrderQuery = resultQuery.Collection;

                dynamicDataGrid.Page = parameters.Page;
                dynamicDataGrid.NumberOfPages = resultQuery.NumberOfPages;
                dynamicDataGrid.TotalNumberOfItemsWithoutPagination = resultQuery.TotalCount;
                dynamicDataGrid.PageSize = parameters.PageSize;

                //agroup the result by service order
                var groupServices = (from item in serviceOrderQuery
                                     group item by new { item.ServiceOrderId }
                                         into rows
                                         select new
                                         {
                                             serviceOrderId = rows.Key,
                                             rows
                                         });

                //verify if the result have data in the query
                if (serviceOrder != null && (serviceOrderQuery != null && serviceOrderQuery.Count > 0))
                {
                    //get the fields that can be showed in the grid
                    foreach (var section in serviceOrder.Sections)
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

                    //verify the result of the query
                    foreach (var group in groupServices)
                    {
                        dataRow = new DynamicDataRow();
                        temRowValues = new Dictionary<string, DynamicDataRowValue>();
                        gridFields.ForEach(field =>
                        {
                            temRowValues.Add(field, new DynamicDataRowValue() { FieldType = 2});
                        });
                        foreach (var data in group.rows)
                        {
                            filterProperties =
                                data.GetType().GetProperties().Where(property => formValueProperties.Contains(property.Name)).ToList();
                            foreach (var property in filterProperties)
                            {
                                //obtain the values of the necesary properties for create the result
                                if (property.Name == "FieldName")
                                {
                                    fieldName = data.GetPropertyValue<object>(property.Name).ToString();
                                }
                                else if (property.Name == "FieldType")
                                {
                                    string tempValue = data.GetPropertyValue<object>(property.Name).ToString();
                                    if (!string.IsNullOrEmpty(tempValue))
                                    {
                                        fieldType = int.Parse(tempValue);
                                        fieldValue = GetFieldValue(data, fieldType, fieldValue);
                                    }
                                }

                                //if all values are filled,the system continues for create the result
                                if ((!string.IsNullOrEmpty(fieldName) && gridFields.Any(field => field == fieldName))
                                    && !string.IsNullOrEmpty(fieldValue) && fieldType > 0)
                                {
                                    //if in the result the system founds a catalogue id,the system retrieves the value of catalogue
                                    fieldValue = GetFinalFieldValue(temRowValues, fieldName, fieldType, fieldValue);
                                    isOk = true;
                                }
                                if (isOk)
                                {
                                    isOk = false;
                                    dataRow.FieldValues.Clear();
                                    temRowValues.Values.ToList().ForEach(value =>
                                    {
                                        dataRow.FieldValues.Add(new DynamicDataRowValue { FieldType = value.FieldType, FieldValue = value.FieldValue });
                                    });
                                    break;
                                }
                            }
                            fieldName = string.Empty;
                            fieldValue = string.Empty;
                            fieldType = 0;
                        }
                        //get the identifier of row
                        dataRow.RowIdentifier = group.serviceOrderId.ServiceOrderId;
                        approvalItems = GetApprovalItemsOfServiceOrder(dataRow.RowIdentifier.Value, parameters.RolesForUser, context);

                        dataRow.CanPublish = approvalItems.Any(data => data.CanPublish.GetValueOrDefault());
                        dataRow.CanValidate = approvalItems.Any(data => !data.CanPublish.GetValueOrDefault());

                        if (parameters.IsClient)
                        {
                            //Get the first report that has data published to the client
                            dataRow.FirstInspectionReportClient = (
                                                                      from inspectionReport in context.InspectionReports
                                                                      where
                                                                          inspectionReport.ServiceOrderId ==
                                                                          dataRow.RowIdentifier.Value
                                                                          && inspectionReport.IsDeleted == false &&
                                                                          inspectionReport.IsClientVisible == true &&
                                                                          inspectionReport.InspectionReportItems.Count(
                                                                              item =>
                                                                              item.IsDeleted == false &&
                                                                              item.PublicationDate.HasValue) > 0
                                                                      orderby inspectionReport.FormOrder
                                                                      select
                                                                          inspectionReport.FormName).FirstOrDefault();
                            //Get the number of pictures related to the service order
                            int countPictures =
                                context.Pictures.Count(
                                    item =>
                                    item.IsDeleted == false && item.ServiceOrderId == dataRow.RowIdentifier.Value);

                            //Get the number of documents related to the service order
                            int countDocuments =
                                context.Documents.Count(
                                    item =>
                                    item.IsDeleted == false && item.ServiceOrderId == dataRow.RowIdentifier.Value);

                            dataRow.HasPicturesClient = countPictures > 0
                                                            ? true
                                                            : false;

                            dataRow.HasDocumentsClient = countDocuments > 0
                                                             ? true
                                                             : false;
                        }

                        //add the row to the dinamic grid
                        dynamicDataGrid.DataRows.Add(dataRow);
                    }
                }
            }
            return dynamicDataGrid;
        }
        #endregion

        #region GetFinalFieldValue
        /// <summary>
        /// Get the final value of the field
        /// </summary>
        /// <param name="temRowValues">Temporal row values</param>
        /// <param name="fieldName">field name</param>
        /// <param name="fieldType">Field type</param>
        /// <param name="fieldValue">Field valie</param>
        /// <returns>string</returns>
        private static string GetFinalFieldValue(Dictionary<string, DynamicDataRowValue> temRowValues, string fieldName, int fieldType, string fieldValue)
        {
            if (fieldType == (int)FieldType.Catalogue)
            {
                Guid catalogValueId = new Guid(fieldValue);
                fieldValue =
                    CatalogueBusiness.GetCatalogueValue(catalogValueId).CatalogueValueData;
            }
            else if (fieldType == (int)FieldType.Datepicker)
            {
                DateTime temDateValue = DateTime.Parse(fieldValue);
                fieldValue = string.Format("{0:dd/MM/yyyy}", temDateValue);
            }
            else if (fieldType == (int)FieldType.Boolean)
            {
                fieldValue = fieldValue.Equals("True") ? "Yes" : "No";
            }
            //finally, the system creates the result
            temRowValues[fieldName].FieldValue = fieldValue;
            temRowValues[fieldName].FieldType = fieldType;
            return fieldValue;
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
                    fieldValue = data.CatalogueValueId.HasValue ? data.CatalogueValueId.GetValueOrDefault().ToString() : string.Empty;
                    break;
                case (int)FieldType.Datepicker:
                    fieldValue = data.DateValue.HasValue ? data.DateValue.GetValueOrDefault().ToString() : string.Empty;
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
                    fieldValue = data.TextValue;
                    break;
                case (int)FieldType.None:
                    break;
            }
            return fieldValue;
        }
        #endregion

        #region GetServiceOrderQuery

        /// <summary>
        /// Get the list of service orders
        /// </summary>
        /// <param name="context">Vestalis context</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>List of FormValue</returns>
        private static PaginatedList<FormValue> GetServiceOrderQuery(VestalisEntities context, ParameterSearchServicerOrder parameters)
        {
            PaginatedList<FormValue> result = new PaginatedList<FormValue>();
            int currentIndex = (parameters.Page - 1) * parameters.PageSize;
            List<Guid?> serviceOrderIds = null;
            if (parameters.FormCollection.ToFilledDictionary().Count > 0)
            {
                //get the data from database
                serviceOrderIds = GetServiceOrdersFiltered(context, parameters);
                result.TotalCount = serviceOrderIds.Count;

                if (!parameters.IsExport)
                    serviceOrderIds = serviceOrderIds.Skip(currentIndex).Take(parameters.PageSize).ToList();

                //get the data from database
                result.Collection = (from formValue in context.FormValues
                                     join serviceOrders in context.ServiceOrders on formValue.ServiceOrderId equals serviceOrders.ServiceOrderId
                                     where serviceOrderIds.Contains(formValue.ServiceOrderId) && formValue.IsDeleted == false
                                     orderby serviceOrders.CreationDate descending
                                     select formValue).ToList();
            }
            else
            {
                //get the data from database

                serviceOrderIds = (from serviceOrder in context.ServiceOrders
                                   where serviceOrder.BusinessApplicationId == parameters.BusinessApplicationId
                                   && serviceOrder.IsDeleted == false
                                   orderby serviceOrder.CreationDate descending
                                   select serviceOrder.ServiceOrderId).AsEnumerable().Cast<Guid?>().ToList();

                result.TotalCount = serviceOrderIds.Count;

                if (!parameters.IsExport)
                    serviceOrderIds = serviceOrderIds.Skip(currentIndex).Take(parameters.PageSize).ToList();

                //get the data from database
                result.Collection = (from formValue in context.FormValues
                                     join serviceOrders in context.ServiceOrders on formValue.ServiceOrderId equals serviceOrders.ServiceOrderId
                                     where serviceOrderIds.Contains(formValue.ServiceOrderId) && formValue.IsDeleted == false
                                     orderby serviceOrders.CreationDate descending
                                     select formValue).ToList();
            }

            result.NumberOfPages = (int)Math.Ceiling((double)result.TotalCount / (double)parameters.PageSize);

            return result;
        }
        #endregion

        #region AddServiceOrder
        /// <summary>
        /// Save the service order in the database
        /// </summary>
        /// <param name="formCollection">Values entered in the form</param>
        /// <param name="businessApplicationId">Business application identifier</param>
        /// <param name="userName">User name for auditing</param>
        public static void AddServiceOrder(FormCollection formCollection, Guid businessApplicationId, string userName)
        {
            //Get the fields definition for the business application
            Fields fieldBusinessApplication = CacheHandler.Get(String.Format("Field{0}", businessApplicationId),
                                            () => DynamicFormEngine.GetFields(businessApplicationId));

            Guid serviceOrderId = Guid.NewGuid();
            using (VestalisEntities ctx = new VestalisEntities())
            {
                IList<FormDefinition> businessFormDefinition =
                    (from formDefinition in ctx.FormDefinitions
                     where
                         formDefinition.BusinessApplicationId == businessApplicationId &&
                         formDefinition.IsDeleted == false
                     orderby formDefinition.FormOrder
                     select formDefinition).ToList();

                //Set the service Order information
                ServiceOrder serviceOrder = new ServiceOrder();
                SetServiceOrderToSave(businessApplicationId, serviceOrder, serviceOrderId, userName, businessFormDefinition);

                //Add the service order to the context
                ctx.ServiceOrders.AddObject(serviceOrder);

                foreach (var formDefinition in businessFormDefinition.Where(item => item.CatalogueValue.CatalogueValueData != FormType.ServiceOrder.ToString()).OrderBy(data => data.FormOrder))
                {
                    //Set the inspection report information
                    InspectionReport inspectionReport = new InspectionReport();
                    SetInspectionReportToSaveOrder(serviceOrderId, userName, inspectionReport, formDefinition);
                    //Add the inspection report to the context
                    ctx.InspectionReports.AddObject(inspectionReport);
                }

                //For each field form
                foreach (var formCollectionValue in formCollection.Keys)
                {
                    FillFormValue(ctx, serviceOrderId, userName, formCollection, formCollectionValue, fieldBusinessApplication);
                }
                ctx.SaveChanges();
            }
        }

        /// <summary>
        /// Save the values entered in the service order
        /// </summary>
        /// <param name="ctx">EF context</param>
        /// <param name="serviceOrderId">Service order identifier</param>
        /// <param name="userName">User creator</param>
        /// <param name="formCollection"></param>
        /// <param name="formCollectionValue"></param>
        /// <param name="fieldBusinessApplication"></param>
        private static void FillFormValue(VestalisEntities ctx, Guid serviceOrderId, string userName, FormCollection formCollection, object formCollectionValue, Fields fieldBusinessApplication)
        {
            string fieldName;
            string valData;
            fieldName = formCollectionValue.ToString();
            ValueProviderResult val = formCollection.GetValue(fieldName);
            if (val != null)
            {
                //Get the form value for the specific field
                valData = val.AttemptedValue.ToString();
                if (!String.IsNullOrEmpty(valData))
                {
                    //Set the form value
                    FormValue formValue = SetFormValueToSaveOrder(serviceOrderId, userName, fieldName, valData,
                                                                  fieldBusinessApplication);
                    //Add the form value to the context
                    ctx.FormValues.AddObject(formValue);
                }
            }
        }

        #endregion

        #region SetServiceOrderToSave
        /// <summary>
        /// Set the service order data
        /// </summary>
        /// <param name="businessApplicationId">Business application identifier</param>
        /// <param name="serviceOrder">Service order to be saved in the database</param>
        /// <param name="serviceOrderId">Service order identifier</param>
        /// <param name="userName">User name for auditing</param>
        /// <param name="businessFormDefinition">Object that contains the xml form definition to create the service order dynamic form</param>
        private static void SetServiceOrderToSave(Guid businessApplicationId, ServiceOrder serviceOrder, Guid serviceOrderId, string userName, IList<FormDefinition> businessFormDefinition)
        {
            FormDefinition serviceOrderFormDefinition =
                businessFormDefinition.FirstOrDefault(
                    item => item.CatalogueValue.CatalogueValueData == FormType.ServiceOrder.ToString());

            serviceOrder.ServiceOrderId = serviceOrderId;
            serviceOrder.BusinessApplicationId = businessApplicationId;
            serviceOrder.StatusCode = ConstantApplication.ServicePendingPublish;
            serviceOrder.XmlFormDefinitionInstance = serviceOrderFormDefinition.XmlFormDefinition;
            serviceOrder.CreationBy = userName;
            serviceOrder.CreationDate = DateTime.UtcNow;
            serviceOrder.ModificationDate = DateTime.UtcNow;
            serviceOrder.ModificationBy = userName;
        }
        #endregion

        #region SetInspectionReportToSaveOrder
        /// <summary>
        /// Set the new Inspection report to be saved
        /// </summary>
        /// <param name="serviceOrderId">Service order identifier</param>
        /// <param name="userName">User name for auditing</param>
        /// <param name="inspectionReport">Inspection report to be inserted in the database</param>
        /// <param name="formDefinition">Form definition data to be inserted in the Inspection report</param>
        private static void SetInspectionReportToSaveOrder(Guid serviceOrderId, string userName, InspectionReport inspectionReport, FormDefinition formDefinition)
        {
            inspectionReport.InspectionReportId = Guid.NewGuid();
            inspectionReport.ServiceOrderId = serviceOrderId;
            inspectionReport.StatusCode = ConstantApplication.InspectionReportPendingPublish;
            inspectionReport.XmlFormDefinitionInstance = formDefinition.XmlFormDefinition;
            inspectionReport.IsClientVisible = formDefinition.IsClientVisible;
            inspectionReport.FormName = formDefinition.FormName;
            inspectionReport.CreationBy = userName;
            inspectionReport.CreationDate = DateTime.UtcNow;
            inspectionReport.ModificationBy = userName;
            inspectionReport.ModificationDate = DateTime.UtcNow;
            inspectionReport.FormOrder = formDefinition.FormOrder;
        }
        #endregion

        #region SetFormValueToSaveOrder
        /// <summary>
        /// Create a FormValue according each value entered in the form by the user. It is convert to the appropiated data type according the field type
        /// </summary>
        /// <param name="serviceOrderId">Service Order identifier</param>
        /// <param name="userName">User name for auditing</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="valData">Form value</param>
        /// <param name="fieldBusinessApplication">Fields obtained from the XML </param>
        /// <returns>Form value to be inserted in the database</returns>
        private static FormValue SetFormValueToSaveOrder(Guid serviceOrderId, string userName, string fieldName, string valData, Fields fieldBusinessApplication)
        {
            FormValue formValue = new FormValue();
            formValue.ServiceOrderId = serviceOrderId;
            formValue.FieldName = fieldName;
            formValue.FormValueId = Guid.NewGuid();

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
            formValue.CreationBy = userName;
            formValue.CreationDate = DateTime.UtcNow;
            formValue.ModificationBy = userName;
            formValue.ModificationDate = DateTime.UtcNow;
            return formValue;
        }
        #endregion

        #region SetFormValueToEditOrder
        /// <summary>
        /// Set form values when a service order is editing
        /// </summary>
        /// <param name="formValue">FormValue to be saved in the database</param>
        /// <param name="valData">Form value</param>
        /// <param name="userName">User name for auditing</param>
        private static void SetFormValueToEditOrder(FormValue formValue, string valData, string userName)
        {
            switch (formValue.FieldType)
            {
                case (int)FieldType.SingleTextLine:
                case (int)FieldType.MultipleTextLine:
                case (int)FieldType.RegularExpressionText:
                case (int)FieldType.Time:
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

        #region GetServiceOrdersFiltered

        /// <summary>
        /// This method get dinamically the result according of the filters
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="parameters">Parameters</param>
        /// <returns></returns>
        private static List<Guid?> GetServiceOrdersFiltered(VestalisEntities context, ParameterSearchServicerOrder parameters)
        {
            List<Guid?> result = new List<Guid?>();

            //filter FormCollection object to get a dictionary only with the key that have a value
            Dictionary<string, string> formCollectionFiltered = parameters.FormCollection.ToFilledDictionary();

            //get the types of the filters
            var queryServiceOrder = context.FormValues.Where(data => formCollectionFiltered.Keys.Contains(data.FieldName))
                .Select(data => new { FieldName = data.FieldName, TypeField = data.FieldType }).Distinct();

            //this query filter the result by business application
            string query1 = "select VALUE ServiceOrder.ServiceOrderId from VestalisEntities.ServiceOrders as ServiceOrder where ServiceOrder.IsDeleted = false AND ServiceOrder.BusinessApplicationId = GUID '" + parameters.BusinessApplicationId.ToString() + "'";
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
                        tempQuery = GetCatalogueQuery(context, formCollectionFiltered, fieldName, tempQuery, query1Result, parameters.FielsdWithLike.Contains(fieldName));
                        break;
                    case (int)FieldType.Decimal:
                        tempQuery = GetDecimalQuery(context, formCollectionFiltered, fieldName, tempQuery, query1Result);
                        break;
                    case (int)FieldType.Integer:
                        tempQuery = GetIntegerQuery(context, formCollectionFiltered, fieldName, tempQuery, query1Result);
                        break;
                    case (int)FieldType.MultipleTextLine:
                        tempQuery = GetMultiLineQuery(context, formCollectionFiltered, fieldName, tempQuery, query1Result, parameters.FielsdWithLike.Contains(fieldName));
                        break;
                    case (int)FieldType.RegularExpressionText:
                        tempQuery = GetRegularExpressionQuery(context, formCollectionFiltered, fieldName, tempQuery, query1Result, parameters.FielsdWithLike.Contains(fieldName));
                        break;
                    case (int)FieldType.SingleTextLine:
                        tempQuery = GetSingleTextQuery(context, formCollectionFiltered, fieldName, tempQuery, query1Result, parameters.FielsdWithLike.Contains(fieldName));
                        break;
                    case (int)FieldType.Time:
                        tempQuery = GetTimeQuery(context, formCollectionFiltered, fieldName, tempQuery, query1Result);
                        break;
                    default:
                        break;
                }
            }

            //if in the filter exist a date range, the system will perform the query for filter the results
            if (formCollectionFiltered.Any(keyPair => keyPair.Key.EndsWith("from")) && formCollectionFiltered.Any(keyPair => keyPair.Key.EndsWith("to")))
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

            string queryTime = "select VALUE FormValue.ServiceOrderId from VestalisEntities.FormValues as FormValue where FormValue.IsDeleted = false AND FormValue.TextValue= '"
                               + timeValue + "'  and FormValue.FieldName='" + fieldName + "' AND FormValue.InspectionReportItemId IS NULL";
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
        /// <param name="isLikeSearch">Is or not a search with like statement</param>
        /// <returns>Object query of Guid</returns>
        private static ObjectQuery<Guid> GetSingleTextQuery(VestalisEntities context, Dictionary<string, string> formCollectionFiltered, string fieldName, ObjectQuery<Guid> tempQuery, ObjectQuery<Guid> query1Result, bool isLikeSearch)
        {
            string singleTextValue = formCollectionFiltered[fieldName];
            string querysingleText = string.Empty;

            if (isLikeSearch)
            {
                querysingleText = "select VALUE FormValue.ServiceOrderId from VestalisEntities.FormValues as FormValue where FormValue.IsDeleted = false AND FormValue.TextValue like '%"
                                         + singleTextValue + "%'  and FormValue.FieldName='" + fieldName + "' AND FormValue.InspectionReportItemId IS NULL";

            }
            else
            {
                querysingleText = "select VALUE FormValue.ServiceOrderId from VestalisEntities.FormValues as FormValue where FormValue.IsDeleted = false AND FormValue.TextValue= '"
                                         + singleTextValue + "'  and FormValue.FieldName='" + fieldName + "' AND FormValue.InspectionReportItemId IS NULL";
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
        /// <param name="isLikeSearch">Is or not a search with like statement</param>
        /// <returns>Object query of Guid</returns>
        private static ObjectQuery<Guid> GetRegularExpressionQuery(VestalisEntities context, Dictionary<string, string> formCollectionFiltered, string fieldName, ObjectQuery<Guid> tempQuery, ObjectQuery<Guid> query1Result,bool isLikeSearch)
        {
            string regularExpressionValue = formCollectionFiltered[fieldName];
            string queryRegularExpression = string.Empty;

            if (isLikeSearch)
            {
                queryRegularExpression = "select VALUE FormValue.ServiceOrderId from VestalisEntities.FormValues as FormValue where FormValue.IsDeleted = false AND FormValue.TextValue like '%"
                                + regularExpressionValue + "%'  and FormValue.FieldName='" + fieldName + "' AND FormValue.InspectionReportItemId IS NULL";

            }
            else
            {
                queryRegularExpression = "select VALUE FormValue.ServiceOrderId from VestalisEntities.FormValues as FormValue where FormValue.IsDeleted = false AND FormValue.TextValue= '"
                                + regularExpressionValue + "'  and FormValue.FieldName='" + fieldName + "' AND FormValue.InspectionReportItemId IS NULL";

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
        /// <param name="isLikeSearch">Is or not a search with like statement</param>
        /// <returns>Object query of Guid</returns>
        private static ObjectQuery<Guid> GetMultiLineQuery(VestalisEntities context, Dictionary<string, string> formCollectionFiltered, string fieldName, ObjectQuery<Guid> tempQuery, ObjectQuery<Guid> query1Result, bool isLikeSearch)
        {
            string multilineValue = formCollectionFiltered[fieldName];
            string queryMultiText = string.Empty;

            if (isLikeSearch)
            {
                queryMultiText = "select VALUE FormValue.ServiceOrderId from VestalisEntities.FormValues as FormValue where  FormValue.IsDeleted = false AND FormValue.TextValue like '%"
                                        + multilineValue + "%'  and FormValue.FieldName='" + fieldName + "' AND FormValue.InspectionReportItemId IS NULL";

            }
            else
            {
                queryMultiText = "select VALUE FormValue.ServiceOrderId from VestalisEntities.FormValues as FormValue where  FormValue.IsDeleted = false AND FormValue.TextValue= '"
                                        + multilineValue + "'  and FormValue.FieldName='" + fieldName + "' AND FormValue.InspectionReportItemId IS NULL";
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

            string queryInteger = "select VALUE FormValue.ServiceOrderId from VestalisEntities.FormValues as FormValue where FormValue.IsDeleted = false AND FormValue.IntegerValue= "
                                  + intValue.ToString() + "  and FormValue.FieldName='" + fieldName + "' AND FormValue.InspectionReportItemId IS NULL";
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

            string queryDecimal = "select VALUE FormValue.ServiceOrderId from VestalisEntities.FormValues as FormValue where FormValue.IsDeleted = false AND FormValue.DecimalValue= "
                                  + decimalValue.ToString() + "  and FormValue.FieldName='" + fieldName + "' AND FormValue.InspectionReportItemId IS NULL";
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
        /// <param name="isLikeSearch">Is or not a search with like statement</param>
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
                                 + " and FormValue.FieldName='" + fieldName + "' AND FormValue.InspectionReportItemId IS NULL"
                                 + " and CatalogueValue.CatalogueValueData like '%" + likeValue + "%' and CatalogueValue.IsDeleted = false";
            }
            else
            {
                guidValue = new Guid(formCollectionFiltered[fieldName]);
                queryCatalogue = "select VALUE FormValue.ServiceOrderId from VestalisEntities.FormValues as FormValue where FormValue.IsDeleted = false AND FormValue.CatalogueValueId =Guid '"
                               + guidValue.ToString() + "'  and FormValue.FieldName='" + fieldName + "' AND FormValue.InspectionReportItemId IS NULL";
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

            string queryBool = "select VALUE FormValue.ServiceOrderId from VestalisEntities.FormValues as FormValue where FormValue.IsDeleted = false AND FormValue.CheckValue="
                               + checkValue.ToString() + "  and FormValue.FieldName='" + fieldName + "' AND FormValue.InspectionReportItemId IS NULL";
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
            string[] OrderDatefromValue = formCollectionFiltered.First(keyPair => keyPair.Key.EndsWith("from")).Value.Split(new char[] { '/', '-' }, StringSplitOptions.RemoveEmptyEntries);
            string[] OrderDateToValue = formCollectionFiltered.First(keyPair => keyPair.Key.EndsWith("to")).Value.Split(new char[] { '/', '-' }, StringSplitOptions.RemoveEmptyEntries);
            string fieldName = formCollectionFiltered.First(keyPair => keyPair.Key.EndsWith("to")).Key;
            fieldName = fieldName.Remove(fieldName.Length - 2, 2);

            DateTime OrderDatefrom = new DateTime(int.Parse(OrderDatefromValue[2]), int.Parse(OrderDatefromValue[1]), int.Parse(OrderDatefromValue[0]));
            DateTime OrderDateTo = new DateTime(int.Parse(OrderDateToValue[2]), int.Parse(OrderDateToValue[1]), int.Parse(OrderDateToValue[0]));

            OrderDatefrom = OrderDatefrom.Date;
            OrderDateTo = OrderDateTo.Date.AddDays(1).AddSeconds(-1);

            string queryDate = "select VALUE FormValue.ServiceOrderId from VestalisEntities.FormValues as FormValue where FormValue.IsDeleted = false and FormValue.DateValue >= DATETIME '"
                               + OrderDatefrom.ToString("yyyy-MM-dd HH:mm:ss.fffffff") + "' and FormValue.DateValue <= DATETIME '" + OrderDateTo.ToString("yyyy-MM-dd HH:mm:ss.fffffff")
                               + "' and FormValue.FieldName='" + fieldName + "' AND FormValue.InspectionReportItemId IS NULL";
            ObjectQuery<Guid> objQueryDate = new ObjectQuery<Guid>(queryDate, context);

            if (tempQuery == null)
                tempQuery = query1Result.Intersect(objQueryDate);
            else
                tempQuery = tempQuery.Intersect(objQueryDate);
            return tempQuery;
        }
        #endregion

        #region EditServiceOrder
        /// <summary>
        /// Save the service order in the database
        /// </summary>
        /// <param name="formCollection">Values entered in the form</param>
        /// <param name="businessApplicationId">Business application identifier</param>
        /// <param name="userName">User name for auditing</param>
        /// <param name="serviceOrderId">Service order identifier</param>
        public static void EditServiceOrder(FormCollection formCollection, Guid businessApplicationId, string userName, Guid serviceOrderId)
        {
            IList<string> keyNames = new List<string>();
            //Get the fields definition for the business application
            Fields fieldBusinessApplication = CacheHandler.Get(String.Format("Field{0}", businessApplicationId),
                                            () => DynamicFormEngine.GetFields(businessApplicationId));

            using (VestalisEntities ctx = new VestalisEntities())
            {
                //Get the form values inserted in the database but they aren't in formCollection.Keys
                foreach (var formCollectionValue in formCollection.Keys)
                {
                    keyNames.Add(formCollectionValue.ToString());
                }

                var formsNotSent = (from formValues in ctx.FormValues
                                    where
                                        formValues.ServiceOrderId == serviceOrderId &&
                                        formValues.IsDeleted == false && 
                                        formValues.InspectionReportItemId == null &&
                                        !keyNames.Contains(formValues.FieldName)
                                    select formValues);

                foreach (var formNotSent in formsNotSent)
                {
                    formNotSent.IsDeleted = true;
                    formNotSent.ModificationBy = userName;
                    formNotSent.ModificationDate = DateTime.UtcNow;
                }

                //For each field form
                foreach (var formCollectionValue in formCollection.Keys)
                {
                    FillFormValuesEdit(formCollection, ctx, serviceOrderId, userName, formCollectionValue, fieldBusinessApplication);
                }
                ctx.SaveChanges();
            }
        }

        /// <summary>
        /// Update service order values
        /// </summary>
        /// <param name="formCollection">Service order values</param>
        /// <param name="ctx">Context</param>
        /// <param name="serviceOrderId">Service order identifier</param>
        /// <param name="userName">User that modifies the information</param>
        /// <param name="formCollectionValue"></param>
        /// <param name="fieldBusinessApplication"></param>
        private static void FillFormValuesEdit(FormCollection formCollection, VestalisEntities ctx, Guid serviceOrderId, string userName, object formCollectionValue, Fields fieldBusinessApplication)
        {
            string fieldName;
            string valData;
            fieldName = formCollectionValue.ToString();
            ValueProviderResult val = formCollection.GetValue(fieldName);
            //Get the form value of the service order according the field name
            FormValue formValueToEdit = (from formValues in ctx.FormValues
                                         where
                                             formValues.ServiceOrderId == serviceOrderId &&
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
                        SetFormValueToEditOrder(formValueToEdit, valData, userName);
                    }
                    else
                    {
                        //Set the form value
                        FormValue formValue = SetFormValueToSaveOrder(serviceOrderId, userName, fieldName,
                                                                      valData,
                                                                      fieldBusinessApplication);
                        //Add the form value to the context
                        ctx.FormValues.AddObject(formValue);
                    }
                }
                //Case this field had a value in the past, but the user has updated now without value
                else if (formValueToEdit != null)
                {
                    formValueToEdit.IsDeleted = true;
                    formValueToEdit.ModificationBy = userName;
                    formValueToEdit.ModificationDate = DateTime.UtcNow;
                }
            }
        }

        #endregion

        #region DeleteServiceOrder
        /// <summary>
        /// Delete service order
        /// </summary>
        /// <param name="serviceOrderId">Service Order id</param>
        /// <param name="userName">User name</param>
        public static void DeleteServiceOrder(Guid serviceOrderId, string userName)
        {
            using (VestalisEntities ctx = new VestalisEntities())
            {
                //Get the service order
                ServiceOrder serviceOrder =
                    (from serviceOrderQuery in ctx.ServiceOrders
                     where serviceOrderQuery.ServiceOrderId == serviceOrderId
                     select serviceOrderQuery).FirstOrDefault();
                if (serviceOrder != null)
                {
                    //Delete the record
                    serviceOrder.IsDeleted = true;
                    serviceOrder.ModificationBy = userName;
                    serviceOrder.ModificationDate = DateTime.UtcNow;
                    ctx.SaveChanges();
                }
            }
        }
        #endregion

        #region DeleteSelectedOrders
        /// <summary>
        /// Delete selected orders
        /// </summary>
        /// <param name="selectedOrderIds">Ids of selected orders</param>
        /// <param name="userName">The name of the current user</param>
        public static void DeleteSelectedOrders(List<Guid> selectedOrderIds, string userName)
        {
            using (VestalisEntities context = new VestalisEntities())
            {
                var selectedOrders = (from serviceOrder in context.ServiceOrders
                                      where selectedOrderIds.Contains(serviceOrder.ServiceOrderId)
                                      select serviceOrder).ToList();

                if (selectedOrders != null && selectedOrders.Count > 0)
                {
                    selectedOrders.ForEach(serviceOrder =>
                    {
                        //Delete the record
                        serviceOrder.IsDeleted = true;
                        serviceOrder.ModificationBy = userName;
                        serviceOrder.ModificationDate = DateTime.UtcNow;
                        context.SaveChanges();
                    });
                }
            }
        }
        #endregion

        #region PublishValidateAllInspectionReports
        /// <summary>
        /// Publish or validate all inspection reports for one service order
        /// </summary>
        /// <param name="serviceOrderId">Id of service order</param>
        /// <param name="rolesForUser">Roles assigned for the logged user</param>
        /// <param name="userName">The name of logged user</param>
        public static void PublishValidateAllInspectionReports(Guid serviceOrderId, List<string> rolesForUser, string userName)
        {
            List<ApprovalItem> approvalItems = null;
            int approvalLevel = 0;
            Guid inspectionReportItemId = Guid.Empty;

            using (VestalisEntities context = new VestalisEntities())
            {

                approvalItems = GetApprovalItemsOfServiceOrder(serviceOrderId, rolesForUser, context);

                foreach (ApprovalItem approvalItem in approvalItems)
                {
                    approvalLevel = 0;
                    approvalLevel = approvalItem.ApprovalLevel;
                    approvalItem.ApprovalStatus = (int)ApprovalStatus.Completed;
                    approvalItem.ModificationBy = userName;
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
                }

                //save all changes
                context.SaveChanges();
            }
        }


        #endregion

        #region GetApprovalItemsOfServiceOrder

        /// <summary>
        /// Get the approbal items of the service order
        /// </summary>
        /// <param name="serviceOrderId">Id of service order</param>
        /// <param name="rolesForUser">List of roles</param>
        /// <param name="context">Data base context</param>
        /// <returns></returns>
        private static List<ApprovalItem> GetApprovalItemsOfServiceOrder(Guid serviceOrderId, List<string> rolesForUser, VestalisEntities context)
        {
            List<ApprovalItem> approvalItems = null;

            approvalItems = (from serviceOrder in context.ServiceOrders
                             join inspectionReport in context.InspectionReports on serviceOrder.ServiceOrderId equals inspectionReport.ServiceOrderId
                             join inspectionReportItem in context.InspectionReportItems on inspectionReport.InspectionReportId equals inspectionReportItem.InspectionReportId
                             join appItem in context.ApprovalItems on inspectionReportItem.InspectionReportItemId equals appItem.InspectionReportItemId
                             where serviceOrder.IsDeleted == false && inspectionReport.IsDeleted == false && inspectionReportItem.IsDeleted == false
                             && appItem.IsDeleted == false && serviceOrder.ServiceOrderId == serviceOrderId
                              && appItem.ApprovalStatus == (int)ApprovalStatus.Ready
                             && rolesForUser.Contains(appItem.RoleName)
                             select appItem).ToList();
            return approvalItems;
        }
        #endregion

        #region SearchInspectionReportsByServiceOrder
        /// <summary>
        /// Get the information of all inspection reports in the service order
        /// </summary>
        /// <returns>ExportInspectionReportsModel</returns>
        public static ExportInspectionReportsModel SearchInspectionReportsByServiceOrder(ParameterSearchAllInspectionReport parameters)
        {
            ExportInspectionReportsModel result = new ExportInspectionReportsModel();

            using (VestalisEntities context = new VestalisEntities())
            {
                //Get the list of inspection reports by service order
                var inspectionReports = (from inspectionRep in context.InspectionReports
                                         join serviceOrd in context.ServiceOrders on inspectionRep.ServiceOrderId equals serviceOrd.ServiceOrderId
                                         where inspectionRep.IsDeleted == false && serviceOrd.IsDeleted == false &&
                                         serviceOrd.ServiceOrderId == parameters.ServiceOrderId && parameters.SelectedReports.Contains(inspectionRep.FormName)
                                         orderby inspectionRep.FormOrder
                                         select new
                                         {
                                             inspectionRep.FormName,
                                             inspectionRep.FormOrder
                                         }).ToList();

                bool isClient = Roles.IsUserInRole(parameters.UserName, "Client");
                List<string> rolesForUser = Roles.GetRolesForUser(parameters.UserName).ToList();

                foreach (var item in inspectionReports)
                {
                    DynamicDataGrid gridColumns = InspectionReportBusiness.GetInspectionReportGridDefinition(parameters.BusinessApplicationId, isClient, item.FormName);
                    //set the parameters
                    ParameterSearchInspectionReport parameterSearchInspectionReport = new ParameterSearchInspectionReport
                    {
                        BusinessApplicationId = parameters.BusinessApplicationId,
                        Collection = new FormCollection(),
                        InspectionReportName = item.FormName,
                        ServiceOrderId = parameters.ServiceOrderId,
                        UserName = parameters.UserName,
                        RolesForUser = rolesForUser,
                        PageSize = 0,
                        SelectedPage = 0,
                        IsClient = isClient,
                        IsExport = true,
                        Captions = gridColumns.Captions
                    };
                    //get the information of the current inspection report
                    DynamicDataGrid model = InspectionReportBusiness.SearchInspectionReportList(parameterSearchInspectionReport);
                    
                    result.InspectionReports.Add(item.FormName, model);
                }

                result.ServiceOrderData = GetServiceOrderForm(parameters.BusinessApplicationId, parameters.ServiceOrderId);
                if (parameters.IsSelectedServiceOrder)
                {
                    result.IsSelectedServiceOrder = true;
                    result.ServiceOrderSheetName = parameters.ServiceOrderReportName;
                }
            }
            return result;
        }
        #endregion

        #endregion
    }

}
