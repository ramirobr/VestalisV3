using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Cotecna.Vestalis.Core;
using Cotecna.Vestalis.Web.Common;

namespace Cotecna.Vestalis.Web.Controllers
{
    /// <summary>
    /// This class contains all action methods needed for managing all about service orders
    /// </summary>
    public class ServiceOrderController : BaseController
    {
        #region fields
        static IList<DynamicCaptionGrid> _captions = null;
        static FormCollection _formCollection = null;
        static string _formName = string.Empty;
        static string _businessApplicationName = string.Empty;
        #endregion

        #region methods

        #region Index

        /// <summary>
        /// Load the service order search
        /// </summary>
        /// <param name="page">Search query page</param>
        /// <param name="businessApplicationId">Id of business application</param>
        /// <returns></returns>
        [OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
        public ActionResult Index(int? page, Guid? businessApplicationId)
        {
            if ((businessApplicationId == Guid.Empty || !businessApplicationId.HasValue) && Session["BusinessAplicationId"] != null)
            {
                businessApplicationId = new Guid(Convert.ToString(Session["BusinessAplicationId"]));
            }
            if (businessApplicationId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            int totalNumberOfItemsWithoutPagination = 0; //query from database, plus the number of pages
            int pageSize = Cotecna.Vestalis.Web.Properties.Settings.Default.PageSize;
            int numberOfPages = (int)Math.Ceiling((double)totalNumberOfItemsWithoutPagination / (double)pageSize);
            int currentPage = page == null ? 1 : page.Value;

            DynamicDataGrid model = ServiceOrderBusiness.GetServiceOrderGridDefinition(businessApplicationId.Value, User.IsInRole("Client"));
            
            _formName = model.FormName;
            _businessApplicationName = model.BusinessApplicationName;

            Session.Add("businessApplicationName", _businessApplicationName);
            Session.Add("PrincipalFormName", _formName);
            Session.Add("CaptionBreadcrumbs", model.CaptionBreadcrumbs);
            Session.Add("CaptionTitle", model.CaptionTitle);

            model.PageSize = pageSize;
            model.TotalNumberOfItemsWithoutPagination = totalNumberOfItemsWithoutPagination;
            model.NumberOfPages = numberOfPages;
            model.Page = currentPage;

            _captions = model.Captions;

            return View(model);
        }
        #endregion

        #region SearchOrder
        /// <summary>
        /// Retrieve the list of service orders
        /// </summary>
        /// <param name="page">Current page</param>
        /// <param name="collection">Form collection</param>
        /// <returns>PartialViewResult</returns>
        [HttpPost]
        public PartialViewResult SearchOrder(int? page, FormCollection collection)
        {
            bool isClient = User.IsInRole("Client");
            Guid businessApplicationId = new Guid(Convert.ToString(Session["BusinessAplicationId"]));
            if (isClient)
            {
                if (collection.AllKeys.Contains("Client")) collection.Remove("Client");
                Guid? parameter = AuthorizationBusiness.GetClientIdByBusinnessApplication(businessApplicationId, UserName);
                collection.Add("Client", parameter.GetValueOrDefault().ToString());
            }
            int currentPage = page == null ? 1 : page.Value;
            int pageSize = Cotecna.Vestalis.Web.Properties.Settings.Default.PageSize;

            List<string> fielsdWithLike = new List<string>();
            foreach (string name in collection.AllKeys.Where(name => name.Contains("IsLike")))
            {
                fielsdWithLike.Add(name.Replace("IsLike", ""));
            }
            
            ParameterSearchServicerOrder parameters = new ParameterSearchServicerOrder
            {
                FormCollection = collection,
                BusinessApplicationId = businessApplicationId,
                RolesForUser = Roles.GetRolesForUser(UserName).ToList(),
                Page = currentPage,
                PageSize = pageSize,
                IsExport = false,
                IsClient = isClient,
                FielsdWithLike = fielsdWithLike
            };
            DynamicDataGrid model = ServiceOrderBusiness.SearchOrderList(parameters);

            _formCollection = collection;

            model.Captions = _captions;

            return PartialView("_ServiceOrderGrid", model);
        }
        #endregion

        #region SearchOrderPaginated
        /// <summary>
        /// Retrive the list of service orders with pagination
        /// </summary>
        /// <param name="page">Page number</param>
        /// <returns>PartialViewResult</returns>
        [HttpPost]
        public PartialViewResult SearchOrderPaginated(int? page)
        {
            Guid businessApplicationId = new Guid(Convert.ToString(Session["BusinessAplicationId"]));
            int pageSize = Cotecna.Vestalis.Web.Properties.Settings.Default.PageSize;
            int currentPage = page == null ? 1 : page.Value;

            List<string> fielsdWithLike = new List<string>();
            foreach (string name in _formCollection.AllKeys.Where(name => name.Contains("IsLike")))
            {
                fielsdWithLike.Add(name.Replace("IsLike", ""));
            }

            ParameterSearchServicerOrder parameters = new ParameterSearchServicerOrder
            {
                FormCollection = _formCollection,
                BusinessApplicationId = businessApplicationId,
                RolesForUser = Roles.GetRolesForUser(UserName).ToList(),
                Page = currentPage,
                PageSize = pageSize,
                IsExport = false,
                IsClient = User.IsInRole("Client"),
                FielsdWithLike = fielsdWithLike
            };

            DynamicDataGrid model = ServiceOrderBusiness.SearchOrderList(parameters);

            model.Captions = _captions;


            return PartialView("_ServiceOrderGrid", model);
        }
        #endregion

        #region SearchServiceOrderApplication
        /// <summary>
        /// Change the business application and refresh the search service order screen
        /// </summary>
        /// <param name="businessApplicationId"></param>
        /// <returns></returns>
        public string SearchServiceOrderApplication(Guid? businessApplicationId)
        {
            int numRoles = 0;

            if (Session.Count == 0)
            {
                return Url.Content("~/");
            }

            if (businessApplicationId.HasValue)
            {
                Session["BusinessAplicationId"] = businessApplicationId;
                UserProfile profile = UserProfile.GetUserProfile(UserName);
                profile.ApplicationDefault = businessApplicationId.GetValueOrDefault().ToString();
                profile.Save();
            }
            List<BusinessApplicationByUser> businessAplicationsByUser = Session["BusinessAplicationsByUser"] as List<BusinessApplicationByUser>;
            BusinessApplicationByUser applicationByUser = businessAplicationsByUser.FirstOrDefault(item => item.Id == businessApplicationId);
            if (applicationByUser != null)
            {
                Session.Add("LanguageAplication", applicationByUser.LanguageCode);
                Session.Add("objDefaultBusinessApp", applicationByUser);
                numRoles = Roles.GetRolesForUser(User.Identity.Name).Where(rol => rol.Contains("_" + applicationByUser.Prefix)).ToList().Count;
                if (numRoles == 1 && (User.IsInRole("GlobalAdministrator") || (User.IsInRole("ApplicationAdministrator_" + applicationByUser.Prefix))))
                {
                    return Url.Content("~/Catalogue");
                }
                else 
                {
                    return Url.Content("~/ServiceOrder");
                }
            }
            else
            {
                return Url.Content("~/");
            }
        }
        #endregion

        #region NewServiceOrder
        public ViewResult NewServiceOrder()
        {
            Guid businessApplicationId = new Guid(Convert.ToString(Session["BusinessAplicationId"]));
            Form model = ServiceOrderBusiness.GetServiceOrderForm(businessApplicationId, null);
            model.IsReadOnly = false;
            model.FormIdentifier = null;
            model.ScreenOpenMode = ScreenOpenMode.Add;
            ViewBag.ScreenModelOption = Resources.Common.New + " " + model.CaptionTitle;
            Session["FormNew"] = model;
            return View("ServiceOrder", model);
        }
        #endregion

        #region SaveServiceOrder
        [HttpPost]
        public ActionResult SaveServiceOrder(FormCollection collection)
        {
            ValueProviderResult val = collection.GetValue("serviceOrderId");
            string serviceOrderId = val.AttemptedValue;
            collection.Remove("serviceOrderId");

            Form formRetrieved = (Form)Session["FormNew"];
            ValidateForm(formRetrieved, collection);
            ValidateBusinessRules(formRetrieved, collection);

            if (ModelState.IsValid)
            {
                Guid businessApplicationId = new Guid(Convert.ToString(Session["BusinessAplicationId"]));
                if (!string.IsNullOrEmpty(serviceOrderId))
                {
                    ServiceOrderBusiness.EditServiceOrder(collection, businessApplicationId, UserName, new Guid(serviceOrderId));
                }
                else
                {
                    ServiceOrderBusiness.AddServiceOrder(collection, businessApplicationId, UserName);
                }
                return RedirectToAction("Index");
            }
            else
            {
                return View("ServiceOrder", formRetrieved);
            }

        }
        #endregion

        #region EditServiceOrder
        /// <summary>
        /// Call the service order for editing
        /// </summary>
        /// <param name="serviceOrderId">Service order to edit</param>
        /// <returns>View with values of the service order to edit</returns>
        public ViewResult EditServiceOrder(Guid? serviceOrderId)
        {
            if (serviceOrderId.HasValue)
                Session.Add("serviceOrderReportId", serviceOrderId);
            else
            {
                if (Session["serviceOrderReportId"] != null)
                    serviceOrderId = new Guid(Session["serviceOrderReportId"].ToString());
            }

            Guid businessApplicationId = new Guid(Convert.ToString(Session["BusinessAplicationId"]));
            Form model = ServiceOrderBusiness.GetServiceOrderForm(businessApplicationId, serviceOrderId);
            model.IsReadOnly = false;
            model.FormIdentifier = serviceOrderId;
            model.ScreenOpenMode = ScreenOpenMode.Edit;
            //Get the order number to be set in the title
            string orderNumber = model.OrderIdentifier==null || String.IsNullOrEmpty(model.OrderIdentifier.FieldValue)
                                     ? Resources.Common.ServiceOrderUndefined
                                     : model.OrderIdentifier.FieldValue;
            ViewBag.ScreenModelOption = String.Format(model.CaptionBreadcrumbs + " {0}",orderNumber);
            model.Links = InspectionReportBusiness.GetInspectionReportLinks(serviceOrderId, User.IsInRole("Client"));
            Session["FormNew"] = model;
            return View("ServiceOrder", model);
        }
        #endregion

        #region ViewServiceOrder
        /// <summary>
        /// Return the service order in read only mode
        /// </summary>
        /// <param name="serviceOrderId">Service order identifier</param>
        /// <returns>Service order data</returns>
        public ViewResult ViewServiceOrder(Guid? serviceOrderId)
        {
            if (serviceOrderId.HasValue)
                Session.Add("serviceOrderReportId", serviceOrderId);
            else
            {
                if (Session["serviceOrderReportId"] != null)
                    serviceOrderId = new Guid(Session["serviceOrderReportId"].ToString());
            }

            Guid businessApplicationId = new Guid(Convert.ToString(Session["BusinessAplicationId"]));
            Form model = ServiceOrderBusiness.GetServiceOrderForm(businessApplicationId, serviceOrderId, User.IsInRole("Client"));
            model.IsReadOnly = true;
            model.FormIdentifier = serviceOrderId;
            //Get the order number to be set in the title
            string orderNumber = model.OrderIdentifier == null || String.IsNullOrEmpty(model.OrderIdentifier.FieldValue)
                                     ? Resources.Common.ServiceOrderUndefined
                                     : model.OrderIdentifier.FieldValue;
            ViewBag.ScreenModelOption = String.Format(Resources.ServiceOrder.ServiceOrderNumber,
                                                      orderNumber);
            model.Links = InspectionReportBusiness.GetInspectionReportLinks(serviceOrderId, User.IsInRole("Client"));
            Session["FormNew"] = model;
            return View("ServiceOrder", model);
        }
        #endregion

        #region DeleteServiceOrder
        /// <summary>
        /// Delete the service Order
        /// </summary>
        /// <param name="serviceOrderId">Id of service Order</param>
        /// <returns></returns>
        public ActionResult DeleteServiceOrder(Guid? serviceOrderId)
        {
            ServiceOrderBusiness.DeleteServiceOrder(serviceOrderId.GetValueOrDefault(), UserName);

            return Json(true);
        }
        #endregion

        #region DeleteSelectedServiceOrders
        /// <summary>
        /// Delete selected service orders
        /// </summary>
        /// <param name="selectedIds">Selected service orders</param>
        /// <returns>ActionResult</returns>
        public ActionResult DeleteSelectedServiceOrders(string selectedIds)
        {
            string[] ids = selectedIds.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            List<Guid> selectedOrders = new List<Guid>();
            ids.ToList().ForEach(id => {
                selectedOrders.Add(new Guid(id));
            });
            ServiceOrderBusiness.DeleteSelectedOrders(selectedOrders, UserName);
            return Json(true);
        }
        #endregion

        #region ValidatePublishInspectionReports
        /// <summary>
        /// Delete the service Order
        /// </summary>
        /// <param name="collection">Service order identifier</param>
        /// <returns></returns>
        public PartialViewResult ValidatePublishInspectionReports(FormCollection collection)
        {
            ValueProviderResult val = collection.GetValue("serviceOrderId");
            Guid serviceOrderId = new Guid(val.AttemptedValue);
            collection.Remove("serviceOrderId");

            ServiceOrderBusiness.PublishValidateAllInspectionReports(serviceOrderId, Roles.GetRolesForUser(UserName).ToList(), UserName);

            return SearchOrder(null, collection);
        }
        #endregion

        #region ExportExcel

        /// <summary>
        /// Perform the creation of the excel file with the information of the service orders
        /// </summary>
        /// <param name="collection">Form collection</param>
        /// <returns>Json result</returns>
        public ActionResult ExecuteSeachServiceOrders(FormCollection collection)
        {
            bool resultMethod;
            try
            {
                bool isClient = User.IsInRole("Client");
                Guid businessApplicationId = new Guid(Convert.ToString(Session["BusinessAplicationId"]));
                //if the logged user is client, the system will add a new key in the collection with the id of the client
                DynamicDataGrid model = GetModelExcelExport(isClient, collection, businessApplicationId);

                model.Captions = _captions;
                model.BusinessApplicationName = _businessApplicationName;
                model.FormName = _formName;

                string logoPath = Server.MapPath("~/Images/logo.png");
                //generate the report
                MemoryStream report = ExcelBusiness.GenerateReportDinamically(model, logoPath);
                report.Position = 0;
                string currentDateTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                //download the report
                MicrosoftExcelStreamResult result = new MicrosoftExcelStreamResult(report, "ServiceOrderReport" + currentDateTime + ".xlsx");
                resultMethod = true;
                Session.Add("ResultSearchServiceOrders", result);
            }
            catch (Exception ex)
            {
                resultMethod = false;
                ExceptionManager.HandleException(ex, "AllExceptionsPolicy");
            }
            return Json(resultMethod);
        }

        /// <summary>
        /// Get the data needed for export to excel
        /// </summary>
        /// <param name="isClient">Is client</param>
        /// <param name="collection">Form collection</param>
        /// <param name="businessApplicationId">Id of business application</param>
        /// <returns>DynamicDataGrid</returns>
        private DynamicDataGrid GetModelExcelExport(bool isClient, FormCollection collection, Guid businessApplicationId)
        {
            
            if (isClient)
            {
                Guid? parameter = AuthorizationBusiness.GetClientIdByBusinnessApplication(businessApplicationId, UserName);
                collection.Add("Client", parameter.GetValueOrDefault().ToString());
            }
            
            //get the data

            ParameterSearchServicerOrder parameters = new ParameterSearchServicerOrder
                                                          {
                                                              FormCollection = collection,
                                                              BusinessApplicationId = businessApplicationId,
                                                              RolesForUser = Roles.GetRolesForUser(UserName).ToList(),
                                                              Page = 0,
                                                              PageSize = 0,
                                                              IsExport = true,
                                                              IsClient = isClient
                                                          };

            return ServiceOrderBusiness.SearchOrderList(parameters);
        }


        /// <summary>
        /// Export the information of all service order to excel
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public FileStreamResult ExportExcel()
        {
            //download the report
            MicrosoftExcelStreamResult result = Session["ResultSearchServiceOrders"] as MicrosoftExcelStreamResult;
            Session.Remove("ResultSearchServiceOrders");
            return result;
        }
        #endregion

        #region ExportAllInspectionReportToExcel

        /// <summary>
        /// Search all service orders for exporting to excel
        /// </summary>
        /// <param name="rowId">Id of service order</param>
        /// <param name="selectedReports">Report names selected for exporting to excel</param>
        public ActionResult ExecuteSearchAllInspectionReportsExport(Guid rowId, string selectedReports)
        {
            List<string> inspectionReports = selectedReports.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (inspectionReports.Count == 0)
            {
                return Json(Resources.ServiceOrder.GenerateReportMessage);
            }
            else
            {
                Guid businessApplicationId = new Guid(Convert.ToString(Session["BusinessAplicationId"]));

                ParameterSearchAllInspectionReport parameters = new ParameterSearchAllInspectionReport()
                {
                    BusinessApplicationId = businessApplicationId,
                    ServiceOrderId = rowId,
                    UserName = UserName,
                    SelectedReports = inspectionReports,
                    IsSelectedServiceOrder = inspectionReports.Contains(Resources.ServiceOrder.ServiceOrderMenu),
                    ServiceOrderReportName = Resources.ServiceOrder.ServiceOrderMenu
                };

                ExportInspectionReportsModel result = ServiceOrderBusiness.SearchInspectionReportsByServiceOrder(parameters);
                result.BusinessApplicationName = _businessApplicationName;
                Session.Add("ResultSearchAllInspectionReports", result);
                return Json("");
            }
        }

        /// <summary>
        /// Build the excel file
        /// </summary>
        /// <returns>JSON</returns>
        public ActionResult GenerateExcelAllInspectionReports()
        {
            bool resultMethod;
            try
            {
                ExportInspectionReportsModel result = Session["ResultSearchAllInspectionReports"] as ExportInspectionReportsModel;
                Session.Remove("ResultSearchAllInspectionReports");

                Guid businessApplicationId = new Guid(Convert.ToString(Session["BusinessAplicationId"]));

                result.InspectionReports.Values.ToList().ForEach(value => {
                    value.BusinessApplicationName = _businessApplicationName;
                });

                string currentDateTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                string orderNumber = result.ServiceOrderData.OrderIdentifier == null || String.IsNullOrEmpty(result.ServiceOrderData.OrderIdentifier.FieldValue)
                                         ? Resources.Common.ServiceOrderUndefined
                                         : result.ServiceOrderData.OrderIdentifier.FieldValue;

                string logoPath = Server.MapPath("~/Images/logo.png");

                MemoryStream ms = ExcelBusiness.GenerateAllInspectionReports(result, logoPath);
                ms.Position = 0;
                MicrosoftExcelStreamResult report = new MicrosoftExcelStreamResult(ms, "InspectionReports_SO_" + orderNumber + "_" + currentDateTime + ".xlsx");
                resultMethod = true;
                Session.Add("AllInspectionReports", report);
            }
            catch (Exception ex)
            {
                resultMethod = false;
                ExceptionManager.HandleException(ex, "AllExceptionsPolicy");
            }
            return Json(resultMethod);
        }

        /// <summary>
        /// Export all inspection reports to excel
        /// </summary>
        /// <returns>FileStreamResult</returns>
        [HttpPost]
        public FileStreamResult ExportAllInspectionReportToExcel()
        {
            MicrosoftExcelStreamResult report = Session["AllInspectionReports"] as MicrosoftExcelStreamResult;
            Session.Remove("AllInspectionReports"); 
            return report;
        }
        #endregion

        #region SearchReports
        /// <summary>
        /// Show the list of reports linked to the selected service order
        /// </summary>
        /// <param name="rowId">Service order Id</param>
        /// <returns>PartialViewResult</returns>
        public PartialViewResult SearchReports(Guid rowId)
        {
            IList<string> reportNames = new List<string>();
            reportNames = InspectionReportBusiness.GetInspectionReportLinks(rowId, User.IsInRole("Client"));
            reportNames.Remove("Picture");
            reportNames.Remove("Document");
            reportNames.Insert(0, Resources.ServiceOrder.ServiceOrderMenu);
            return PartialView("_ReportList", reportNames);
        }
        #endregion

        #endregion
    }
}
