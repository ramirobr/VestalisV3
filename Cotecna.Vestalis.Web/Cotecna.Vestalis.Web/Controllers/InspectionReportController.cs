using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Cotecna.Vestalis.Core;
using Cotecna.Vestalis.Web.Common;
using System.Web;

namespace Cotecna.Vestalis.Web.Controllers
{
    /// <summary>
    /// This class contains all action methods needed for managing all about inspection reports
    /// </summary>
    public class InspectionReportController : BaseController
    {
        #region fields
        static IList<DynamicCaptionGrid> _captions = null;
        static string _inspectionReportId = string.Empty;
        static FormCollection _collection = null;
        #endregion

        #region methods

        #region Index
        /// <summary>
        /// Search the definition of the report and show a search criteria and a grid the columns of the report
        /// </summary>
        /// <param name="serviceOrderReportId">Id of service order</param>
        /// <param name="inspectionReportName">Inspection report name to be displayed</param>
        /// <returns>ActionResult</returns>
        public ActionResult Index(Guid? serviceOrderReportId, string inspectionReportName)
        {
            bool isClient = User.IsInRole("Client");
            if (serviceOrderReportId.HasValue)
                Session.Add("serviceOrderReportId", serviceOrderReportId);
            else
            {
                if (Session["serviceOrderReportId"] != null)
                    serviceOrderReportId = new Guid(Session["serviceOrderReportId"].ToString());
            }
            Guid businessApplicationId = new Guid(Convert.ToString(Session["BusinessAplicationId"]));
            Form formDefinition = ServiceOrderBusiness.GetServiceOrderForm(businessApplicationId, serviceOrderReportId, isClient, true);
            DynamicDataGrid gridColumns = InspectionReportBusiness.GetInspectionReportGridDefinition(businessApplicationId, isClient, inspectionReportName);
            InspectionReportModel model = new InspectionReportModel();
            _captions = gridColumns.Captions;
            model.GridColumns = gridColumns;
            model.Links = InspectionReportBusiness.GetInspectionReportLinks(serviceOrderReportId, isClient);
            model.ServiceOrderHeader = formDefinition.ServiceOrderHeader;
            model.OrderIdentifier = formDefinition.OrderIdentifier;
            model.InspectionReportName = inspectionReportName;
            model.PrincipalFormName = Session["PrincipalFormName"].ToString();
            model.CaptionBreadcrumbsPrincipal = Session["CaptionBreadcrumbs"].ToString();
            model.CaptionTitlePrincipal = Session["CaptionTitle"].ToString();
            Session.Add("InspectionReportId", gridColumns.FormName);
            return View("Index", model);
        }
        #endregion

        #region ChangeReport
        /// <summary>
        /// Search the definition of the report and show a search criteria and a grid the columns of the report
        /// </summary>
        /// <param name="inspectionReportName">Inspection report name</param>
        /// <returns></returns>
        public ActionResult ChangeReport(string inspectionReportName)
        {
            if (string.IsNullOrEmpty(inspectionReportName))
            {
                inspectionReportName = Session["InspectionReportName"].ToString();
            }
            else
            {
                Session.Add("InspectionReportName", inspectionReportName);
            }
            bool isClient = User.IsInRole("Client");
            Guid businessApplicationId = new Guid(Convert.ToString(Session["BusinessAplicationId"]));
            Guid serviceOrderReportId = new Guid(Session["serviceOrderReportId"].ToString());

            Form formDefinition = ServiceOrderBusiness.GetServiceOrderForm(businessApplicationId, serviceOrderReportId, isClient, true);
            DynamicDataGrid gridColumns = InspectionReportBusiness.GetInspectionReportGridDefinition(businessApplicationId, isClient, inspectionReportName);
            InspectionReportModel model = new InspectionReportModel();
            _captions = gridColumns.Captions;
            model.GridColumns = gridColumns;
            model.Links = InspectionReportBusiness.GetInspectionReportLinks(serviceOrderReportId, isClient);
            model.ServiceOrderHeader = formDefinition.ServiceOrderHeader;
            model.OrderIdentifier = formDefinition.OrderIdentifier;
            model.InspectionReportName = inspectionReportName;
            model.PrincipalFormName = Session["PrincipalFormName"].ToString();
            model.CaptionBreadcrumbsPrincipal = Session["CaptionBreadcrumbs"].ToString();
            model.CaptionTitlePrincipal = Session["CaptionTitle"].ToString();
            return View("Index", model);
        }
        #endregion

        #region SearchInspectionReport
        /// <summary>
        /// Search a new inspection Report
        /// </summary>
        /// <param name="page">Page</param>
        /// <param name="inspectionReportId">Id of inspection report</param>
        /// <param name="collection">Form collection</param>
        /// <returns>PartialViewResult</returns>
        public PartialViewResult SearchInspectionReport(int? page, string inspectionReportId, FormCollection collection)
        {
            Guid businessApplicationId = new Guid(Convert.ToString(Session["BusinessAplicationId"]));
            Guid serviceOrderReportId = new Guid(Session["serviceOrderReportId"].ToString());

            collection.Remove("InspectionReportId");
            collection.Remove("selectedInspectionReports");
            _collection = collection;
            _inspectionReportId = inspectionReportId;

            List<string> fielsdWithLike = new List<string>();
            foreach (string name in collection.AllKeys.Where(name => name.Contains("IsLike")))
            {
                fielsdWithLike.Add(name.Replace("IsLike", ""));
                collection.Remove(name);
            }

            ParameterSearchInspectionReport parameters = new ParameterSearchInspectionReport
            {
                BusinessApplicationId = businessApplicationId,
                Collection = collection,
                InspectionReportName = inspectionReportId,
                ServiceOrderId = serviceOrderReportId,
                UserName = UserName,
                RolesForUser = Roles.GetRolesForUser(UserName).ToList(),
                PageSize = Cotecna.Vestalis.Web.Properties.Settings.Default.PageSize,
                SelectedPage = page == null ? 1 : page.Value,
                IsClient = User.IsInRole("Client"),
                FieldsWithLike = fielsdWithLike  
            };

            DynamicDataGrid model = InspectionReportBusiness.SearchInspectionReportList(parameters);
            model.Captions = _captions;
            model.UserLevel = InspectionReportBusiness.GetRoleLevel(Roles.GetRolesForUser(UserName), businessApplicationId);
            Session.Add("DataRowsInspectionReport", model.DataRows);

            return PartialView("_InspectionReportGrid", model);
        }
        #endregion

        #region SearchInspectionReportPaginated
        /// <summary>
        /// Search a new inspection Report
        /// </summary>
        /// <param name="page">Page</param>
        /// <returns>PartialViewResult</returns>
        public PartialViewResult SearchInspectionReportPaginated(int? page)
        {
            Guid businessApplicationId = new Guid(Convert.ToString(Session["BusinessAplicationId"]));
            Guid serviceOrderReportId = new Guid(Session["serviceOrderReportId"].ToString());

            if (_collection.ToFilledDictionary().Keys.Contains("InspectionReportId"))
                _collection.Remove("InspectionReportId");

            List<string> fielsdWithLike = new List<string>();
            foreach (string name in _collection.AllKeys.Where(name => name.Contains("IsLike")))
            {
                fielsdWithLike.Add(name.Replace("IsLike", ""));
            }

            ParameterSearchInspectionReport parameters = new ParameterSearchInspectionReport
            {
                BusinessApplicationId = businessApplicationId,
                Collection = _collection,
                InspectionReportName = _inspectionReportId,
                ServiceOrderId = serviceOrderReportId,
                UserName = UserName,
                RolesForUser = Roles.GetRolesForUser(UserName).ToList(),
                PageSize = Cotecna.Vestalis.Web.Properties.Settings.Default.PageSize,
                SelectedPage = page == null ? 1 : page.Value,
                IsClient = User.IsInRole("Client"),
                FieldsWithLike = fielsdWithLike
            };

            DynamicDataGrid model = InspectionReportBusiness.SearchInspectionReportList(parameters);
            model.Captions = _captions;
            model.UserLevel = InspectionReportBusiness.GetRoleLevel(Roles.GetRolesForUser(UserName), businessApplicationId);
            return PartialView("_InspectionReportGrid", model);
        }
        #endregion

        #region NewInspectionReport
        /// <summary>
        /// Call to InspectionReport for create a new inspection report
        /// </summary>
        /// <returns>ActionResult</returns>
        public ActionResult NewInspectionReport()
        {
            InspectionReportModel model = new InspectionReportModel();
            Guid businessApplicationId = new Guid(Convert.ToString(Session["BusinessAplicationId"]));
            Guid serviceOrderReportId = new Guid(Session["serviceOrderReportId"].ToString());
            string inspectionReportName = Session["InspectionReportName"].ToString();
            Form formDefinition = ServiceOrderBusiness.GetServiceOrderForm(businessApplicationId, serviceOrderReportId, User.IsInRole("Client"), true);
            model.Links = InspectionReportBusiness.GetInspectionReportLinks(serviceOrderReportId, User.IsInRole("Client"));
            model.ServiceOrderHeader = formDefinition.ServiceOrderHeader;
            model.OrderIdentifier = formDefinition.OrderIdentifier;
            formDefinition = InspectionReportBusiness.GetInspectionReportDefinition(businessApplicationId, serviceOrderReportId, inspectionReportName, User.IsInRole("Client"));

            DynamicDataGrid gridColumns = InspectionReportBusiness.GetInspectionReportGridDefinition(businessApplicationId, false, inspectionReportName);
            _captions = gridColumns.Captions;
            model.GridColumns = gridColumns;
            model.FormDefinition = formDefinition;
            model.ServiceOrderId = serviceOrderReportId;
            model.ScreenOpenMode = ScreenOpenMode.Add;
            model.PrincipalFormName = Session["PrincipalFormName"].ToString();
            model.CaptionBreadcrumbsPrincipal = Session["CaptionBreadcrumbs"].ToString();
            model.CaptionTitlePrincipal = Session["CaptionTitle"].ToString();

            if(formDefinition.HasPictures)
                SetupUploader(System.Web.HttpContext.Current,false);

            Session.Add("frmNewInspection", model);
            return View("InspectionReport", model);
        }
        #endregion

        #region SetupUploader

        /// <summary>
        /// Call the initialization of ajax uploader control
        /// </summary>
        /// <param name="context"></param>
        /// <param name="isEdit">Is or not edition mode</param>
        private void SetupUploader(HttpContext context, bool isEdit)
        {
            using (CuteWebUI.MvcUploader uploader = new CuteWebUI.MvcUploader(context))
            {
                uploader.UploadUrl = Response.ApplyAppPathModifier(VirtualPathUtility.ToAbsolute("~/UploadHandler.ashx"));
                //Set the max bytes allowed to be uploaded
                uploader.MaxSizeKB = Cotecna.Vestalis.Web.Properties.Settings.Default.MaxFileUploadSizeKB;
                //the data of the uploader will render as <input type='hidden' name='myuploader'> 
                uploader.Name = "myuploader";
                //File extensions allowed to execute the upload
                uploader.AllowedFileExtensions = Cotecna.Vestalis.Web.Properties.Settings.Default.ExtensionPictureAllowed;
                //Flag to upload multiple files
                uploader.MultipleFilesUpload = true;
                uploader.InsertButtonID = "uploadbutton";
                uploader.ManualStartUpload = !isEdit;
                //prepair html code for the view
                ViewData["uploaderhtml"] = uploader.Render();
            }
        }
        #endregion

        #region SaveInspectionReport
        /// <summary>
        /// Perform the save operation of inspection report operation
        /// </summary>
        /// <param name="collection">Form collection</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult SaveInspectionReport(FormCollection collection)
        {
            if (collection.AllKeys.Contains("serviceOrderId")) collection.Remove("serviceOrderId");
            InspectionReportModel model = Session["frmNewInspection"] as InspectionReportModel;
            Guid serviceOrderReportId = new Guid(Session["serviceOrderReportId"].ToString());
            Guid businessApplicationId = new Guid(Convert.ToString(Session["BusinessAplicationId"]));
            Guid inspectionReportItemId = Guid.Empty;
            string publishValidate = string.Empty;
            string myuploader = string.Empty;
            //validate form and business rules
            if (model.ScreenOpenMode != ScreenOpenMode.View)
            {
                ValidateForm(model.FormDefinition, collection);
                ValidateBusinessRules(model.FormDefinition, collection);
            }
            if (ModelState.IsValid)
            {
                //if publish or validate button is clicked, take the value and remove the key from the collection
                if (collection.AllKeys.Contains("PublishValidateOption") && !string.IsNullOrEmpty(collection["PublishValidateOption"]))
                    publishValidate = collection["PublishValidateOption"];
                collection.Remove("PublishValidateOption");

                if(collection.AllKeys.Contains("UploadLib_Uploader_js"))
                    collection.Remove("UploadLib_Uploader_js");

                if (collection.AllKeys.Contains("__RequestVerificationToken"))
                    collection.Remove("__RequestVerificationToken");

                if (collection.AllKeys.Contains("myuploader"))
                {
                    myuploader = collection["myuploader"].ToString();
                    collection.Remove("myuploader");
                }


                //Set parameters for the method
                ParameterSaveInspectionReport parameters = new ParameterSaveInspectionReport
                {
                    BusinessApplicationId = businessApplicationId,
                    FormCollection = collection,
                    InspectionReportName = model.GridColumns.FormName,
                    ServiceOrderId = serviceOrderReportId,
                    UserName = UserName,
                    RolesForUser = Roles.GetRolesForUser(UserName).ToList(),
                    IsClient = User.IsInRole("Client")
                };
                if (model.ScreenOpenMode == ScreenOpenMode.Add)
                   inspectionReportItemId =  InspectionReportBusiness.AddInspectionReport(parameters);
                else if (model.ScreenOpenMode == ScreenOpenMode.Edit)
                {
                    inspectionReportItemId = new Guid(Session["inspectionReportItemId"].ToString());
                    parameters.InspectionReportItemId = inspectionReportItemId;

                    //perform edit operation
                    InspectionReportBusiness.EditInspectionReport(parameters);
                    //publish or validate inspection report item
                    if (!string.IsNullOrEmpty(publishValidate))
                        InspectionReportBusiness.PublishValidateInspectionReport(inspectionReportItemId, UserName);

                }
                else if (model.ScreenOpenMode == ScreenOpenMode.View && !string.IsNullOrEmpty(publishValidate) && publishValidate == "UnPublish")
                {
                    //Get the id of inspection report item
                    inspectionReportItemId = new Guid(Session["inspectionReportItemId"].ToString());
                    //Perform unpublish operation
                    InspectionReportBusiness.UnPublishInspectionReport(inspectionReportItemId, UserName, Roles.GetRolesForUser(UserName).ToList());
                }
                else if (model.ScreenOpenMode == ScreenOpenMode.View && !string.IsNullOrEmpty(publishValidate) && (publishValidate == "Publish" || publishValidate == "Validate"))
                {
                    //Get the id of inspection report item
                    inspectionReportItemId = new Guid(Session["inspectionReportItemId"].ToString());
                    //Perform unpublish operation
                    InspectionReportBusiness.PublishValidateInspectionReport(inspectionReportItemId, UserName);
                }


                if (!string.IsNullOrEmpty(myuploader) && model.ScreenOpenMode == ScreenOpenMode.Add)
                {
                    using (CuteWebUI.MvcUploader uploader = new CuteWebUI.MvcUploader(System.Web.HttpContext.Current))
                    {
                        if (!string.IsNullOrEmpty(myuploader))
                        {
                            List<string> processedfiles = new List<string>();
                            //for multiple files , the value is string : guid/guid/guid 
                            foreach (string strguid in myuploader.Split('/'))
                            {
                                //for single file , the value is guid string
                                Guid fileguid = new Guid(strguid);
                                CuteWebUI.MvcUploadFile file = uploader.GetUploadedFile(fileguid);
                                if (file != null)
                                {
                                    //Save the picture in the database
                                    PictureDocumentBusiness.UploadPicture(file, serviceOrderReportId, UserName, inspectionReportItemId);
                                    processedfiles.Add(file.FileName);
                                    file.Delete();
                                }
                            }
                            if (processedfiles.Count > 0)
                                ViewData["UploadedMessage"] = string.Join(",", processedfiles.ToArray()) + " have been processed.";
                        }
                    }
                }
                
                return RedirectToAction("ChangeReport", "InspectionReport");
            }
            else
            {
                return View("InspectionReport", model);
            }
        }
        #endregion

        #region EditViewInspectionReport
        /// <summary>
        /// Goes to view or edit screen
        /// </summary>
        /// <param name="inspectionReportItemIdEv">Inspection report item id</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult EditViewInspectionReport(Guid? inspectionReportItemIdEv)
        {
            List<DynamicDataRow> rows = Session["DataRowsInspectionReport"] as List<DynamicDataRow>;
            DynamicDataRow actualDataRow = rows.FirstOrDefault(data => data.RowIdentifier == inspectionReportItemIdEv);

            //if the user is a client, the system will go to the view option.
            if (User.IsInRole("Client"))
            {
                return ViewInspectionReport(inspectionReportItemIdEv, actualDataRow.IsPublished);
            }
            else
            {
                //if the item is in read only mode and is completed, the system go to view option.
                if (actualDataRow.IsReadOnly && actualDataRow.ApprovalStatus == (int)ApprovalStatus.Completed)
                {
                    return ViewInspectionReport(inspectionReportItemIdEv, actualDataRow.IsPublished);
                }
                //if the item is in read only mode and is not completed, the system go to view option.
                else if (actualDataRow.IsReadOnly && actualDataRow.ApprovalStatus != (int)ApprovalStatus.Completed)
                {
                    return ViewInspectionReport(inspectionReportItemIdEv, actualDataRow.IsPublished);
                }
                else
                {
                    //if the last conditions are not true, the system will go to the edit option.
                    return EditInspectionReport(inspectionReportItemIdEv);
                }
            }
        }
        #endregion

        #region EditInspectionReport
        /// <summary>
        /// Call the inspection report for editing
        /// </summary>
        /// <param name="inspectionReportItemId"></param>
        /// <returns>View with values of the service order to edit</returns>
        [HttpPost]
        public ActionResult EditInspectionReport(Guid? inspectionReportItemId)
        {
            InspectionReportModel model = new InspectionReportModel();
            Guid businessApplicationId = new Guid(Convert.ToString(Session["BusinessAplicationId"]));
            Guid serviceOrderReportId = new Guid(Session["serviceOrderReportId"].ToString());
            string inspectionReportName = Session["InspectionReportName"].ToString();
            Form formDefinition = ServiceOrderBusiness.GetServiceOrderForm(businessApplicationId, serviceOrderReportId, User.IsInRole("Client"), true);
            model.Links = InspectionReportBusiness.GetInspectionReportLinks(serviceOrderReportId, User.IsInRole("Client"));
            model.ServiceOrderHeader = formDefinition.ServiceOrderHeader;
            model.OrderIdentifier = formDefinition.OrderIdentifier;
            DynamicDataGrid gridColumns = InspectionReportBusiness.GetInspectionReportGridDefinition(businessApplicationId, false, inspectionReportName);
            _captions = gridColumns.Captions;
            model.GridColumns = gridColumns;
            gridColumns.DataRows = Session["DataRowsInspectionReport"] as IList<DynamicDataRow>;
            DynamicDataRow actualDataRow = gridColumns.DataRows.FirstOrDefault(datarow => datarow.RowIdentifier == inspectionReportItemId);
            model.CanPublish = actualDataRow.CanPublish;
            model.CanValidate = actualDataRow.CanValidate;
            model.InspectionReportItemId = inspectionReportItemId.GetValueOrDefault();
            model.ServiceOrderId = serviceOrderReportId;
            formDefinition = InspectionReportBusiness.GetInspectionReportDefinition(businessApplicationId, serviceOrderReportId, inspectionReportName, User.IsInRole("Client"), inspectionReportItemId);
            model.FormDefinition = formDefinition;

            model.ScreenOpenMode = ScreenOpenMode.Edit;
            model.ApprovalStatus = InspectionReportBusiness.GetCurrentApprovalStatus(Roles.GetRolesForUser(UserName).ToList(), inspectionReportItemId.Value);
            int rowSize = Properties.Settings.Default.PageSizePicture;
            ViewBag.PageSizePicture = rowSize;
            model.PictureModel.PictureList = PictureDocumentBusiness.SearchPicturesInspectionReport(serviceOrderReportId, rowSize, inspectionReportItemId.Value);
            model.PictureModel.ScreenOpenMode = ScreenOpenMode.Edit;
            model.PrincipalFormName = Session["PrincipalFormName"].ToString();
            model.CaptionBreadcrumbsPrincipal = Session["CaptionBreadcrumbs"].ToString();
            model.CaptionTitlePrincipal = Session["CaptionTitle"].ToString();
            Session.Add("frmNewInspection", model);
            Session.Add("inspectionReportItemId", inspectionReportItemId.Value);

            if (formDefinition.HasPictures)
                SetupUploader(System.Web.HttpContext.Current,true);

            return View("InspectionReport", model);
        }
        #endregion

        #region ViewInspectionReport

        /// <summary>
        /// Call the service order for view
        /// </summary>
        /// <param name="inspectionReportItemIdView">Id of inspection report item</param>
        /// <param name="isPublished">Is or not published</param>
        /// <returns>View with values of the service order to edit</returns>
        public ViewResult ViewInspectionReport(Guid? inspectionReportItemIdView, bool? isPublished)
        {
            InspectionReportModel model = new InspectionReportModel();
            Guid businessApplicationId = new Guid(Convert.ToString(Session["BusinessAplicationId"]));
            Guid serviceOrderReportId = new Guid(Session["serviceOrderReportId"].ToString());
            string inspectionReportName = Session["InspectionReportName"].ToString();
            Form formDefinition = ServiceOrderBusiness.GetServiceOrderForm(businessApplicationId, serviceOrderReportId, User.IsInRole("Client"), true);
            model.Links = InspectionReportBusiness.GetInspectionReportLinks(serviceOrderReportId, User.IsInRole("Client"));
            model.ServiceOrderHeader = formDefinition.ServiceOrderHeader;
            model.OrderIdentifier = formDefinition.OrderIdentifier;
            DynamicDataGrid gridColumns = InspectionReportBusiness.GetInspectionReportGridDefinition(businessApplicationId, false, inspectionReportName);
            _captions = gridColumns.Captions;
            model.GridColumns = gridColumns;

            formDefinition = InspectionReportBusiness.GetInspectionReportDefinition(businessApplicationId, serviceOrderReportId, inspectionReportName, User.IsInRole("Client"), inspectionReportItemIdView);
            model.FormDefinition = formDefinition;

            gridColumns.DataRows = Session["DataRowsInspectionReport"] as IList<DynamicDataRow>;
            DynamicDataRow actualDataRow = gridColumns.DataRows.FirstOrDefault(datarow => datarow.RowIdentifier == inspectionReportItemIdView.GetValueOrDefault());
            model.CanPublish = actualDataRow.CanPublish;
            model.CanValidate = actualDataRow.CanValidate;
            model.ServiceOrderId = serviceOrderReportId;
            model.ScreenOpenMode = ScreenOpenMode.View;
            model.InspectionReportItemId = inspectionReportItemIdView.GetValueOrDefault();
            model.IsPublished = isPublished.GetValueOrDefault();
            model.ApprovalStatus = InspectionReportBusiness.GetCurrentApprovalStatus(Roles.GetRolesForUser(UserName).ToList(), inspectionReportItemIdView.GetValueOrDefault());

            int rowSize = Properties.Settings.Default.PageSizePicture;
            ViewBag.PageSizePicture = rowSize;
            model.PictureModel.PictureList = PictureDocumentBusiness.SearchPicturesInspectionReport(serviceOrderReportId, rowSize, inspectionReportItemIdView.GetValueOrDefault());
            model.PictureModel.ScreenOpenMode = ScreenOpenMode.View;
            model.PrincipalFormName = Session["PrincipalFormName"].ToString();
            model.CaptionBreadcrumbsPrincipal = Session["CaptionBreadcrumbs"].ToString();
            model.CaptionTitlePrincipal = Session["CaptionTitle"].ToString();
            Session.Add("frmNewInspection", model);
            Session.Add("inspectionReportItemId", inspectionReportItemIdView.Value);

            return View("InspectionReport", model);
        }
        #endregion

        #region DeleteInspectionReport
        /// <summary>
        /// Delete the inspection report
        /// </summary>
        /// <param name="inspectionReportItemId"></param>
        /// <returns></returns>
        public ActionResult DeleteInspectionReport(Guid? inspectionReportItemId)
        {
            InspectionReportBusiness.DeleteInspectionReport(inspectionReportItemId.GetValueOrDefault(), UserName);
            return Json(true);
        }
        #endregion

        #region DeleteSelectedInspectionReport
        /// <summary>
        /// Delete selected inspection reports
        /// </summary>
        /// <param name="selectedIds">Ids of selected report items</param>
        /// <returns>ActionResult</returns>
        public ActionResult DeleteSelectedInspectionReport(string selectedIds)
        {
            string[] ids = selectedIds.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            List<Guid> reportIds = new List<Guid>();
            ids.ToList().ForEach(id => {
                reportIds.Add(new Guid(id));
            });
            InspectionReportBusiness.DeleteSelectedInspectionReports(reportIds, UserName);
            return Json(true);
        }
        #endregion

        #region PublishValidateInspectionReport
        /// <summary>
        /// Publish or Validate an inspection report
        /// </summary>
        /// <param name="collection">Form collection</param>
        /// <returns>PartialViewResult</returns>
        public PartialViewResult PublishValidateInspectionReport(FormCollection collection)
        {
            ValueProviderResult val = collection.GetValue("inspectionReportItemId");
            Guid inspectionReportItemId = new Guid(val.AttemptedValue);
            collection.Remove("inspectionReportItemId");
            string inspectionReportName = Session["InspectionReportName"].ToString();

            InspectionReportBusiness.PublishValidateInspectionReport(inspectionReportItemId, UserName);

            return SearchInspectionReport(null, inspectionReportName, collection);
        }

        #endregion

        #region PublishValidateSelectedInspectionReports
        /// <summary>
        /// Validate or publish all inspection reports
        /// </summary>
        /// <param name="collection">Form collection</param>
        /// <returns>PartialViewResult</returns>
        public PartialViewResult PublishValidateSelectedInspectionReports(string selectedInspectionReports,FormCollection collection)
        {
            string inspectionReportName = Session["InspectionReportName"].ToString();
            string selectedOption = string.Empty;
            Guid serviceOrderReportId = new Guid(Session["serviceOrderReportId"].ToString());
            List<Guid?> selectedInspectionReportIds = new List<Guid?>();
            string[] selectedIds = selectedInspectionReports.Split(new string[] { "&&&" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in selectedIds)
            {
                selectedInspectionReportIds.Add(new Guid(item));
            }

            if (collection.AllKeys.Contains("inspectionReportItemId"))
                collection.Remove("inspectionReportItemId");

            if (collection.AllKeys.Contains("selectedOption"))
            {
                selectedOption = collection["selectedOption"].ToString();
                collection.Remove("selectedOption");
            }

            bool isPublish = selectedOption == "publish";

            ParameterPublishValidateInspectionReports parameters = new ParameterPublishValidateInspectionReports
            {
                InspectionReportName = inspectionReportName,
                ServiceOrderId = serviceOrderReportId,
                RolesForUser = Roles.GetRolesForUser(UserName).ToList(),
                UserName = UserName,
                IsPublish = isPublish,
                SelectedIds = selectedInspectionReportIds
            };

            InspectionReportBusiness.PublishValidateSelected(parameters);

            return SearchInspectionReport(null, inspectionReportName, collection);
        }

        #endregion

        #region UnPublishInspectionReport
        /// <summary>
        /// UnPublish an inspection report
        /// </summary>
        /// <param name="collection">Form Collection</param>
        /// <returns>PartialViewResult</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly")]
        public PartialViewResult UnPublishInspectionReport(FormCollection collection)
        {
            ValueProviderResult val = collection.GetValue("inspectionReportItemId");
            Guid inspectionReportItemId = new Guid(val.AttemptedValue);
            collection.Remove("inspectionReportItemId");
            string inspectionReportName = Session["InspectionReportName"].ToString();

            InspectionReportBusiness.UnPublishInspectionReport(inspectionReportItemId, UserName, Roles.GetRolesForUser(UserName).ToList());

            return SearchInspectionReport(null, inspectionReportName, collection);
        }
        #endregion

        #region ExportExcel

        /// <summary>
        /// Search the information for generate an excel report
        /// </summary>
        /// <param name="collection">The parameters that are sent by the view</param>
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")] 
        public ActionResult SearchInfoInspectionReport(FormCollection collection)
        {
            bool resultMethod;
            try
            {
                Guid businessApplicationId = new Guid(Convert.ToString(Session["BusinessAplicationId"]));
                Guid serviceOrderReportId = new Guid(Session["serviceOrderReportId"].ToString());

                //get the data

                DynamicDataGrid model = GetModelExportExcel(collection, businessApplicationId, serviceOrderReportId);
                string businessApplicationName = Session["businessApplicationName"].ToString();

                
                model.FormName = _inspectionReportId;
                model.BusinessApplicationName = businessApplicationName;

                string reportName = _inspectionReportId.Replace(" ", "");
                string logoPath = Server.MapPath("~/Images/logo.png");
                //generate the report
                MemoryStream report = ExcelBusiness.GenerateReportDinamically(model,logoPath);
                report.Position = 0;
                string currentDateTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                MicrosoftExcelStreamResult result = new MicrosoftExcelStreamResult(report, reportName + "Report" + currentDateTime + ".xlsx");
                resultMethod = true;
                Session.Add("ResultInspectionReportSearch", result);
            
            }
            catch (Exception ex)
            {
                resultMethod = false;
                ExceptionManager.HandleException(ex, "AllExceptionsPolicy");
            }

            return Json(resultMethod);
        }

        /// <summary>
        /// Get the needed data for writing the excel file
        /// </summary>
        /// <param name="collection">The collection sent by the form</param>
        /// <param name="businessApplicationId">Id of business application</param>
        /// <param name="serviceOrderReportId">Id of service order</param>
        /// <returns>DynamicDataGrid</returns>
        private DynamicDataGrid GetModelExportExcel(FormCollection collection, Guid businessApplicationId, Guid serviceOrderReportId)
        {
            if (collection.ToFilledDictionary().Keys.Contains("InspectionReportId"))
                collection.Remove("InspectionReportId");
            if (collection.ToFilledDictionary().Keys.Contains("selectedInspectionReports"))
                collection.Remove("selectedInspectionReports");

            List<string> fielsdWithLike = new List<string>();
            foreach (string name in collection.AllKeys.Where(name => name.Contains("IsLike")))
            {
                fielsdWithLike.Add(name.Replace("IsLike", ""));
                collection.Remove(name);
            }

            //set the parameters for search

            ParameterSearchInspectionReport parameters = new ParameterSearchInspectionReport
                                                             {
                                                                 BusinessApplicationId = businessApplicationId,
                                                                 Collection = collection,
                                                                 InspectionReportName = _inspectionReportId,
                                                                 ServiceOrderId = serviceOrderReportId,
                                                                 UserName = UserName,
                                                                 RolesForUser = Roles.GetRolesForUser(UserName).ToList(),
                                                                 PageSize = 0,
                                                                 SelectedPage = 0,
                                                                 IsClient = User.IsInRole("Client"),
                                                                 IsExport = true,
                                                                 Captions = _captions,
                                                                 FieldsWithLike = fielsdWithLike
                                                             };

            //get the data
            return InspectionReportBusiness.SearchInspectionReportList(parameters);
        }


        /// <summary>
        /// Export the information of current inspection report to excel
        /// </summary>
        /// <returns>FileStreamResult</returns>
        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")] 
        public FileStreamResult ExportExcel()
        {
            MicrosoftExcelStreamResult result = Session["ResultInspectionReportSearch"] as MicrosoftExcelStreamResult;
            Session.Remove("ResultInspectionReportSearch");
            //download the report
            return result;
        }
        #endregion

        #region DeletePictures
        /// <summary>
        /// Delete all selected pictures in the inspections
        /// </summary>
        /// <param name="pictureIds">selected pictures</param>
        /// <returns>PartialViewResult</returns>
        public void DeletePictures(string pictureIds)
        {
            PictureDocumentBusiness.DeletePictures(pictureIds, UserName);
        }
        #endregion

        #region UploadNewPictures
        /// <summary>
        /// add new pictures to an inspection report
        /// </summary>
        /// <param name="myuploader">pictures</param>
        /// <param name="serviceOrderId">Id of service order</param>
        /// <param name="inspectionReportItemId">Id of inspection report</param>
        public void UploadNewPictures(string myuploader, Guid? serviceOrderId, Guid? inspectionReportItemId)
        {
            using (CuteWebUI.MvcUploader uploader = new CuteWebUI.MvcUploader(System.Web.HttpContext.Current))
            {
                if (!string.IsNullOrEmpty(myuploader))
                {
                    List<string> processedfiles = new List<string>();
                    //for multiple files , the value is string : guid/guid/guid 
                    foreach (string strguid in myuploader.Split('/'))
                    {
                        //for single file , the value is guid string
                        Guid fileguid = new Guid(strguid);
                        CuteWebUI.MvcUploadFile file = uploader.GetUploadedFile(fileguid);
                        if (file != null)
                        {
                            //Save the picture in the database
                            PictureDocumentBusiness.UploadPicture(file, serviceOrderId.GetValueOrDefault(), UserName, inspectionReportItemId);
                            processedfiles.Add(file.FileName);
                            file.Delete();
                        }
                    }
                    if (processedfiles.Count > 0)
                        ViewData["UploadedMessage"] = string.Join(",", processedfiles.ToArray()) + " have been processed.";
                }
            }
        }
        #endregion

        #endregion
    }
}

