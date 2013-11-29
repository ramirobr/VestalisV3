using System;
using System.Linq;
using Cotecna.Vestalis.Entities;

namespace Cotecna.Vestalis.Core.DynamicForm
{
    /// <summary>
    /// Execute queries and operation in XML files
    /// </summary>
    public static class DynamicFormEngine
    {
        /// <summary>
        /// Get the xml that was used when the service order was created
        /// </summary>
        /// <param name="serviceOrderId">Service order identifier</param>
        /// <returns>Form definition of the existing service order</returns>
        public static Form GetExistingServiceOrderForm(Guid serviceOrderId)
        {
            Form formReturn = null;

            using (VestalisEntities ctx = new VestalisEntities())
            {
                string xmlFormDefinitionInstance =
                    (from serviceOrder in ctx.ServiceOrders
                     where serviceOrder.ServiceOrderId == serviceOrderId
                     select serviceOrder.XmlFormDefinitionInstance).FirstOrDefault();
                if (xmlFormDefinitionInstance != null)
                {
                    //Convert to a Form object
                    formReturn = XmlHelper.ReadFormFromXml<Form>(xmlFormDefinitionInstance);
                }
            }

            return formReturn;
        }

        /// <summary>
        /// Get the xml file from the database of a specific type and load into a Form object
        /// </summary>
        /// <param name="businessApplicationId">Busines application id</param>
        /// <param name="formType">Form type</param>
        /// <param name="isClient">Flag to filter the client's reports</param>
        /// <returns>The Form definition for a specific type</returns>
        public static Form GetFormDefinition(Guid businessApplicationId, FormType formType, bool isClient)
        {
            Form formReturn = null;
            FormDefinition formDefinitionProcess = null;
            using (VestalisEntities ctx = new VestalisEntities())
            {
                //Form type code
                string formTypeText = formType.ToString();
                if (isClient)
                {
                    //Execute search in the database to get the form definition according a type
                    formDefinitionProcess = (from formDefinition in ctx.FormDefinitions
                                             where formDefinition.BusinessApplicationId == businessApplicationId
                                                   && formDefinition.CatalogueValue.CatalogueValueData == formTypeText
                                                   && formDefinition.IsClientVisible == isClient
                                             select formDefinition).FirstOrDefault();
                }
                else
                {
                    //Execute search in the database to get the form definition according a type
                    formDefinitionProcess = (from formDefinition in ctx.FormDefinitions
                                             where formDefinition.BusinessApplicationId == businessApplicationId
                                                   && formDefinition.CatalogueValue.CatalogueValueData == formTypeText
                                             select formDefinition).FirstOrDefault();
                }
                if (formDefinitionProcess != null)
                {
                    //Convert to a Form object
                    formReturn = XmlHelper.ReadFormFromXml<Form>(formDefinitionProcess.XmlFormDefinition);
                }
                return formReturn;
            }
        }


        public static Form GetFormDefinition(Guid businessApplicationId, FormType formType, string formName, bool isClient)
        {
            Form formReturn = null;
            FormDefinition formDefinitionProcess = null;
            using (VestalisEntities ctx = new VestalisEntities())
            {
                //Form type code
                string formTypeText = formType.ToString();

                if (isClient)
                {
                    //Execute search in the database to get the form definition according a type
                    formDefinitionProcess = (from formDefinition in ctx.FormDefinitions
                                             where formDefinition.BusinessApplicationId == businessApplicationId
                                                   && formDefinition.CatalogueValue.CatalogueValueData == formTypeText
                                                   && formDefinition.FormName == formName
                                                   && formDefinition.IsClientVisible == isClient
                                             orderby formDefinition.FormOrder
                                             select formDefinition).FirstOrDefault();
                }
                else
                {
                    //Execute search in the database to get the form definition according a type
                    formDefinitionProcess = (from formDefinition in ctx.FormDefinitions
                                             where formDefinition.BusinessApplicationId == businessApplicationId
                                                   && formDefinition.CatalogueValue.CatalogueValueData == formTypeText
                                                   && formDefinition.FormName == formName
                                             orderby formDefinition.FormOrder
                                             select formDefinition).FirstOrDefault();
                }

                if (formDefinitionProcess != null)
                {
                    //Convert to a Form object
                    formReturn = XmlHelper.ReadFormFromXml<Form>(formDefinitionProcess.XmlFormDefinition);
                    //formReturn.FormName = formDefinitionProcess.FormName;
                }
                return formReturn;
            }
        }


        /// <summary>
        /// Get the fields of the business application
        /// </summary>
        /// <param name="businessApplicationId">Business application identifier</param>
        /// <returns>Fields of a business application</returns>
        public static Fields GetFields(Guid businessApplicationId)
        {
            Fields fieldReturn = null;
            using (VestalisEntities ctx = new VestalisEntities())
            {
                //Execute search in the database the fields used in a business application
                BusinessApplication businessApplicationProcess = (from businessApplication in ctx.BusinessApplications
                                                                  where businessApplication.BusinessApplicationId == businessApplicationId
                                                                  select businessApplication).FirstOrDefault();
                if (businessApplicationProcess != null)
                {

                    //Convert to a Field object
                    fieldReturn = XmlHelper.ReadFormFromXml<Fields>(businessApplicationProcess.XmlFieldDefinition);
                }
                return fieldReturn;
            }

        }

        /// <summary>
        /// Get the xml that was used when the service order and inspection reports were created
        /// </summary>
        /// <param name="inspectionReportItemId">Inspection report item identifier</param>
        /// <returns>Form definition of the existing service order</returns>
        public static Form GetExistingInspectionReportForm(Guid inspectionReportItemId)
        {
            Form formReturn = null;

            using (VestalisEntities ctx = new VestalisEntities())
            {
                string xmlFormDefinitionInstance =
                    (from inspectionReportItem in ctx.InspectionReportItems
                     where inspectionReportItem.InspectionReportItemId == inspectionReportItemId
                     select inspectionReportItem.InspectionReport.XmlFormDefinitionInstance).FirstOrDefault();
                if (xmlFormDefinitionInstance != null)
                {
                    //Convert to a Form object
                    formReturn = XmlHelper.ReadFormFromXml<Form>(xmlFormDefinitionInstance);
                }
            }

            return formReturn;
        }

        /// <summary>
        /// Initialize the auto complete field
        /// </summary>
        /// <param name="autoCompleteField">Field</param>
        /// <param name="serviceOrderId">Id of service order</param>
        public static void InitAutoCompleteInspectionReportField(FieldsAutoCompleteTextField autoCompleteField, Guid? serviceOrderId = null)
        {
            //Get the sql statement for filling the itemsource
            string command = autoCompleteField.ItemSourceCondition.Replace("SERVICEORDERID", serviceOrderId.GetValueOrDefault().ToString());
            //Execute the sql statement and fill the itemsource
            autoCompleteField.ItemSource = InspectionReportBusiness.GetAutoCompleteItemSource(command);
        }

    }
}
