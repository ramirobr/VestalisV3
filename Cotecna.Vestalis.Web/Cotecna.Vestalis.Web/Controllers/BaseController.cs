using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Mvc;
using System.Web.Routing;
using Cotecna.Vestalis.Core;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.Unity;

namespace Cotecna.Vestalis.Web.Controllers
{
    /// <summary>
    /// This class is the base class for all controllers, and have the common methods used for manage the exceptions and switch the language
    /// </summary>
    public class BaseController : Controller
    {
        #region fields
        private ExceptionManager _exceptionManager;
        private LogWriter _logWriter;
        #endregion

        #region properties
        
        /// <summary>
        /// Get or set IsGlobalAdmin
        /// </summary>
        public bool IsGlobalAdmin { get; set; }
        
        /// <summary>
        /// Get or Set UserTypes
        /// </summary>
        public Dictionary<int, string> UserTypes { get; set; }

        /// <summary>
        /// The username requesting the action
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Used to handle exceptions and register them in the log file
        /// </summary>    
        public ExceptionManager ExceptionManager
        {
            get
            {
                IUnityContainer container = new UnityContainer();
                try
                {
                    if (_exceptionManager == null)
                    {

                        container.AddNewExtension<Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Unity.
                                    EnterpriseLibraryCoreExtension>();
                        return container.Resolve<ExceptionManager>();
                    }
                    else
                    {
                        return _exceptionManager;
                    }
                }
                finally
                {
                    ((IDisposable)container).Dispose();
                }

            }
            set
            {
                _exceptionManager = value;
            }
        }

        /// <summary>
        /// Used to log entries in the log file
        /// </summary>
        public LogWriter LogWriter
        {
            get
            {
                IUnityContainer container = new UnityContainer();
                try
                {
                    if (_logWriter == null)
                    {
                        container.AddNewExtension
                            <
                                Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Unity.
                                    EnterpriseLibraryCoreExtension>();
                        return container.Resolve<LogWriter>();
                    }
                    else
                        return _logWriter;
                }
                finally
                {
                    ((IDisposable)container).Dispose();
                }
            }
            set
            {
                _logWriter = value;
            }
        }

        #endregion

        #region methods

        #region InitialzeUserTypes
        /// <summary>
        ///  Initialize the list of user types
        /// </summary>
        public void InitialzeUserTypes()
        {
            UserTypes = new Dictionary<int, string>();
            IsGlobalAdmin = User.IsInRole("GlobalAdministrator");
            if (IsGlobalAdmin)
                UserTypes.Add(1, Resources.Administration.UserTypeGlobal);
            UserTypes.Add(3, Resources.Administration.UserTypeClient);
            UserTypes.Add(4, Resources.Administration.UserTypeCotecna);
        }
        #endregion

        #region OnException
        /// <summary>
        /// Management the exceptions
        /// </summary>
        /// <param name="filterContext">ExceptionContext</param>
        protected override void OnException(ExceptionContext filterContext)
        {
            //Handle the exception with Enterprise Library (configured in Web.Config)
            //Search for this in the Web.config file <exceptionPolicies> tag
            this.ExceptionManager.HandleException(filterContext.Exception, "AllExceptionsPolicy");

            bool callMonitoring = bool.Parse(ConfigurationManager.AppSettings["CallMonitoring"].ToString());

            if (callMonitoring)
            {
                //Save Exception information in monitoring service, and return the ticket number generated
                ExceptionBusiness.CatchExceptionInMonitoringService(filterContext.Exception);
            }
            //Show basic Error view
            filterContext.ExceptionHandled = true;

            //Clear any data in the model as it wont be needed
            ViewData.Model = null;

            //Show basic Error view
            View("Error").ExecuteResult(this.ControllerContext);
        }
        #endregion

        #region OnActionExecuting
        /// <summary>
        /// Verify if Session variables exist to redirect to the login screen
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            UserName = User.Identity.Name;

            InitialzeUserTypes();

            if (Session.Count == 0)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Home" }, { "action", "Index" } });
                if (filterContext.ActionDescriptor.ActionName == "SearchServiceOrderApplication")
                    filterContext.Result = null;
                
            }

            if (Session["LanguageAplication"] != null)
            {
                SwitchLanguage(Session["LanguageAplication"].ToString());
            }
        }
        #endregion

        #region SwitchLanguage
        /// <summary>
        /// Switcht the languangue of the application
        /// </summary>
        /// <param name="language">Language id</param>
        private static void SwitchLanguage(string language)
        {
            switch (language)
            {
                case "es":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("es");
                    break;
                case "en":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
                    break;
                case "pt":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("pt");
                    break;
                default:
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
                    break;
            }
        }
        #endregion

        #region ValidateBusinessRules
        /// <summary>
        /// Validate the rules specified in the xml difinitions
        /// </summary>
        /// <param name="formDefinition">The definition of the form</param>
        /// <param name="formCollection">The collection of values obtained from the form</param>
        protected void ValidateBusinessRules(Form formDefinition, FormCollection formCollection)
        {
            FormRulesDateIntervalRule dateRule = null;
            FormRulesSummatoryRule summatoryRule = null;

            if (formDefinition.Rules == null) return;

            dateRule = formDefinition.Rules.DateIntervalRule;
            summatoryRule = formDefinition.Rules.SummatoryRule;

            //validate date range rule
            if (dateRule != null)
            {

                string[] beginDateValue = formCollection[dateRule.StartField].ToString().Split(new[] { '-', '/' });
                string[] endDateValue = formCollection[dateRule.EndField].ToString().Split(new[] { '-', '/' });
                DateTime beginDate = new DateTime();
                DateTime endDate = new DateTime();

                //if the begin date isn't filled, the system will add an error message
                if (beginDateValue.Length > 1)
                {
                    beginDate = new DateTime(int.Parse(beginDateValue[2]), int.Parse(beginDateValue[1]), int.Parse(beginDateValue[0]));
                }

                //if the end date isn't filled, the system will add an error messaage
                if (endDateValue.Length > 1)
                {
                    endDate = new DateTime(int.Parse(endDateValue[2]), int.Parse(endDateValue[1]), int.Parse(endDateValue[0]));
                }

                if (beginDateValue.Length > 1 && endDateValue.Length == 1)
                {
                    ModelState.AddModelError("EndDateValue", Resources.ServiceOrder.EndDateValue);
                }
                else if (beginDateValue.Length == 1 && endDateValue.Length > 1)
                {
                    ModelState.AddModelError("BeginDateValue", Resources.ServiceOrder.BeginDateValue);
                }

                //if the begin date is bigger than end date, the system will add an error message
                if ((beginDate > endDate) && (beginDateValue.Length > 1 && endDateValue.Length > 1))
                {
                    ModelState.AddModelError("DateRule", Resources.ServiceOrder.DateRule);
                }
            }
            //validate summatory rule
            if (summatoryRule != null)
            {
                decimal sumatoryValue = 0;
                decimal totalValue = summatoryRule.Value;
                //retreive the elements to get the sum value
                List<FormRulesSummatoryRuleElement> summatoryElements = summatoryRule.FormElements.ToList();

                summatoryElements.ForEach(element =>
                {
                    string tempSumValue = formCollection[element.Identifier].ToString();
                    //if any element hasn't value, the system won't sum the values.
                    if (!string.IsNullOrEmpty(tempSumValue))
                        sumatoryValue += decimal.Parse(tempSumValue);
                });

                //if the summatory value is different than expected value, the system will add an error message
                if (sumatoryValue > 0 && sumatoryValue != totalValue)
                {
                    ModelState.AddModelError("SummatoryRule", Resources.ServiceOrder.SummatoryRule);
                }

            }
        }
        #endregion

        #region ValidateServiceOrder
        /// <summary>
        /// Validate at server side the form values entered by the user. These values are validated comparing with the attibutes defined in the XML associated to the form
        /// </summary>
        /// <param name="formDefinition">XML file converted to an object</param>
        /// <param name="formCollection">Field and value entered by the user in the form</param>
        protected void ValidateForm(Form formDefinition, FormCollection formCollection)
        {
            //Field name
            string fieldName;
            //Form field value
            string valData;
            //Lenght allowed defined as a rule
            int length;
            //String value set in a xml attribute
            string stringXml;

            foreach (var section in formDefinition.Sections)
            {
                foreach (var element in section.FormElements)
                {
                    fieldName = element.Field.FieldName;
                    ValueProviderResult val = formCollection.GetValue(fieldName);
                    //Control check box input that is not sent when it isn't checked
                    valData = val != null ? val.AttemptedValue.ToString() : String.Empty;

                    element.Field.FieldValue = valData;

                    if (String.IsNullOrEmpty(valData))
                    {
                        //Validate the field as mandatory
                        ValidateMandatoryField(element, fieldName);
                    }
                    else
                    {
                        foreach (var rulesForm in element.Field.RulesForms)
                        {
                            switch (rulesForm)
                            {
                                //Rule Atributte -> EndDate
                                case RulesForm.RuleEndDate:
                                    {
                                        ValidateRuleEndDate(element, fieldName, valData);
                                        break;
                                    }
                                //Rule Atributte -> Expression
                                case RulesForm.RuleExpression:
                                    {
                                        stringXml = element.Field.GetPropertyValue<string>("Expression");
                                        Match match = Regex.Match(valData, stringXml,
                                                                  RegexOptions.IgnoreCase);
                                        //If the value doesn't match witht he regular expression then an error
                                        if (!match.Success)
                                        {
                                            ModelState.AddModelError("",
                                                                     String.Format(
                                                                         Resources.ServiceOrder.
                                                                             ErrorFieldRegularExpression,
                                                                         fieldName, stringXml));
                                        }
                                        break;
                                    }
                                //Rule Atributte -> MaxLenght
                                case RulesForm.RuleMaxLength:
                                    {
                                        length = Convert.ToInt32(element.Field.GetPropertyValue<string>("MaxLength"));

                                        //If the lenght of value is greater than the allowed one  then an error
                                        if (valData.Length > length)
                                        {
                                            ModelState.AddModelError("",
                                                                     String.Format(
                                                                         Resources.ServiceOrder.ErrorFieldTextMaxLength,
                                                                         fieldName));
                                        }
                                        break;
                                    }
                                //Rule Atributte -> MaxValue
                                case RulesForm.RuleMaxValue:
                                    {
                                        ValidateRuleMaxValue(element, fieldName, valData);
                                        break;
                                    }
                                //Rule Atributte -> MinLenght
                                case RulesForm.RuleMinLength:
                                    {
                                        length = Convert.ToInt32(element.Field.GetPropertyValue<string>("MinLength"));

                                        //If the lenght of the value is less than a one  then an error
                                        if (valData.Length < length)
                                        {
                                            ModelState.AddModelError("",
                                                                     String.Format(
                                                                         Resources.ServiceOrder.ErrorFieldTextMinLength,
                                                                         fieldName));
                                        }
                                        break;
                                    }
                                //Rule Atributte -> MinValue
                                case RulesForm.RuleMinValue:
                                    {
                                        ValidateRuleMinValue(element, fieldName, valData);
                                        break;
                                    }
                                //Rule Atributte -> NumDigit
                                case RulesForm.RuleNumDigit:
                                    {
                                        //Validate the number of the digits of the value entered by the user
                                        ValidateNumDigit(element, fieldName, valData);
                                        break;
                                    }
                                //Rule Atributte -> StartDate
                                case RulesForm.RuleStartDate:
                                    {
                                        ValidateRuleStartDate(element, fieldName, valData);
                                        break;
                                    }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region ValidateRuleMinValue
        /// <summary>
        /// Validate data comparing with a minimun value defined in the xml 
        /// </summary>
        /// <param name="element">Form element with the rule</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="valData">Form value entered by the user</param>
        protected void ValidateRuleMinValue(FormSectionElement element, string fieldName, string valData)
        {
            decimal maxMinValue;
            try
            {
                maxMinValue =
                    Convert.ToDecimal(element.Field.GetPropertyValue<object>("MinValue").ToString());
                //If the value is less than a min allowed then an error
                if (Convert.ToDecimal(valData) < maxMinValue)
                {
                    ModelState.AddModelError("",
                                             String.Format(
                                                 Resources.ServiceOrder.
                                                     ErrorFieldTextMinValue,
                                                 fieldName));
                }
            }
            catch (FormatException)
            {
                if (ModelState.Keys.FirstOrDefault(item => item == "ErrorNumeric") == null)
                    ModelState.AddModelError("ErrorNumeric",
                                             String.Format(
                                                 Resources.ServiceOrder.
                                                     ErrorFieldNumericIncorrect,
                                                 fieldName));
            }
        }
        #endregion

        #region ValidateRuleMaxValue
        /// <summary>
        /// Validate data comparing with a maximum value
        /// </summary>
        /// <param name="element">Form element</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="valData">Form value entered by the user</param>
        protected void ValidateRuleMaxValue(FormSectionElement element, string fieldName, string valData)
        {
            decimal maxMinValue;
            try
            {
                maxMinValue =
                    Convert.ToDecimal(element.Field.GetPropertyValue<object>("MaxValue").ToString());

                //If the value is greater than a max allowed then an error
                if (Convert.ToDecimal(valData) > maxMinValue)
                {
                    ModelState.AddModelError("",
                                             String.Format(
                                                 Resources.ServiceOrder.
                                                     ErrorFieldTextMaxValue,
                                                 fieldName));
                }
            }
            catch (FormatException)
            {
                if (ModelState.Keys.FirstOrDefault(item => item == "ErrorNumeric") == null)
                    ModelState.AddModelError("ErrorNumeric",
                                             String.Format(
                                                 Resources.ServiceOrder.
                                                     ErrorFieldNumericIncorrect,
                                                 fieldName));
            }
        }
        #endregion

        #region ValidateRuleStartDate
        /// <summary>
        /// Validate comparing with a minimum date
        /// </summary>
        /// <param name="element">Xml form element</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="valData">Form value entered by the user</param>
        protected void ValidateRuleStartDate(FormSectionElement element, string fieldName, string valData)
        {
            string stringXml;
            DateTime dateValue;
            DateTime startDate;
            stringXml = element.Field.GetPropertyValue<string>("StartDate");
            startDate = stringXml.ToUpper().Equals("TODAY") ? DateTime.Today : Convert.ToDateTime(stringXml);

            try
            {
                dateValue = Convert.ToDateTime(valData,
                                               new System.Globalization.CultureInfo("fr-FR"));
                //If the value is less than a date allowed then an error
                if (dateValue < startDate)
                {
                    ModelState.AddModelError("",
                                             String.Format(
                                                 Resources.ServiceOrder.
                                                     ErrorFieldDateRuleStartDate,
                                                 fieldName, startDate.ToShortDateString()));
                }
            }
            catch (FormatException)
            {
                if (ModelState.Keys.FirstOrDefault(item => item == "ErrorDate") == null)
                    ModelState.AddModelError("ErrorDate",
                                             String.Format(
                                                 Resources.ServiceOrder.
                                                     ErrorFieldDateIncorrect,
                                                 fieldName));
            }
        }
        #endregion

        #region ValidateRuleEndDate
        /// <summary>
        /// Validate a date comparing with a maximum date
        /// </summary>
        /// <param name="element">Form element</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="valData">Form value entered by the user</param>
        protected void ValidateRuleEndDate(FormSectionElement element, string fieldName, string valData)
        {
            string stringXml;
            DateTime dateValue;
            DateTime endDate;
            stringXml = element.Field.GetPropertyValue<string>("EndDate");
            endDate = stringXml.ToUpper().Equals("TODAY") ? DateTime.Today : Convert.ToDateTime(stringXml); //TODO CHECK IF IT IS NECCESARY TO CHANGE TO UTC
            try
            {
                dateValue = Convert.ToDateTime(valData, new System.Globalization.CultureInfo("fr-FR"));
                //If the value is greater than the end data then an error
                if (dateValue > endDate)
                {
                    ModelState.AddModelError("",
                                             String.Format(
                                                 Resources.ServiceOrder.
                                                     ErrorFieldDateRuleEndDate,
                                                 fieldName, endDate.ToShortDateString()));
                }
            }
            catch (FormatException)
            {
                if (ModelState.Keys.FirstOrDefault(item => item == "ErrorDate") == null)
                    ModelState.AddModelError("ErrorDate",
                                             String.Format(
                                                 Resources.ServiceOrder.
                                                     ErrorFieldDateIncorrect,
                                                 fieldName));
            }
        }
        #endregion

        #region ValidateNumDigit
        /// <summary>
        /// Validate the number of the digits in a value entered by the user
        /// </summary>
        /// <param name="element">Element of the XML file</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="valData">Field value entered by the user</param>
        protected void ValidateNumDigit(FormSectionElement element, string fieldName, string valData)
        {
            int numDigitsEnter;
            int numDigitsAllow;
            numDigitsAllow =
                Convert.ToInt32(element.Field.GetPropertyValue<string>("NumDigit"));

            //Get the number of decimal digits considering the period as separator
            if (valData.Contains("."))
            {
                int s = (valData.IndexOf(".") + 1); // the first numbers plus decimal point
                numDigitsEnter = ((valData.Length) - s);
            }
            //Get the number of decimal digits considering the comma as separator
            else if (valData.Contains(","))
            {
                int s = (valData.IndexOf(".") + 1); // the first numbers plus decimal point
                numDigitsEnter = ((valData.Length) - s);
            }
            else
            {
                numDigitsEnter = 0;
            }

            //In case the digits entered comparing with the allowed ones then an error
            if (numDigitsEnter != numDigitsAllow)
            {
                ModelState.AddModelError("",
                                         String.Format(
                                             Resources.ServiceOrder.ErrorFieldTextNumDigit,
                                             fieldName));
            }
        }
        #endregion

        #region ValidateMandatoryField
        /// <summary>
        /// Verify if the field name defined in the xml is as mandatory. In this case add an error to the model because this field doesn't have value.
        /// </summary>
        /// <param name="element">Xml form element</param>
        /// <param name="fieldName">Field name</param>
        protected void ValidateMandatoryField(FormSectionElement element, string fieldName)
        {
            if ((element.Field.RulesForms.Where(item => item == RulesForm.RuleMandatory)).ToList().Count > 0)
            {
                ModelState.AddModelError("", String.Format(Resources.ServiceOrder.ErrorFieldRequired, fieldName));
            }
        }
        #endregion

        #region GetDescriptionCatalogueOnDependent
        /// <summary>
        /// Get the description of a specific catalogue
        /// </summary>
        /// <param name="id">Catalogue Value Id</param>
        /// <returns>Description</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public string GetDescriptionCatalogueOnDependent(Guid? id)
        {
            string desc = String.Empty;
            if (id.HasValue)
            {
                //Get the description
                desc = CatalogueBusiness.GetCatalogueDescription(id.Value);
            }
            return desc;
        }
        #endregion

        #endregion

    }
}
