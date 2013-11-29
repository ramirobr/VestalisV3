using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Cotecna.Vestalis.Core;
using Cotecna.Vestalis.Entities;
using System.IO;

namespace Cotecna.Vestalis.Web.Controllers
{
    /// <summary>
    /// This class contains all action methods needed for managing all about document screen
    /// </summary>
    public class DocumentController : BaseController
    {
        #region methods

        /// <summary>
        /// Search the documents to displayed when the screen is loaded the first time
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(Guid? serviceOrderReportIdDoc)
        {
            if (serviceOrderReportIdDoc.HasValue)
                Session.Add("serviceOrderReportId", serviceOrderReportIdDoc);
            else
            {
                if (Session["serviceOrderReportId"] != null)
                    serviceOrderReportIdDoc = new Guid(Session["serviceOrderReportId"].ToString());
            }

            //Get the page size used for pagination
            int pageSize = Cotecna.Vestalis.Web.Properties.Settings.Default.PageSize;
            //Search the documents of the service order
            DocumentSearchModel model = PictureDocumentBusiness.SearchDocuments(serviceOrderReportIdDoc.Value, pageSize, 1,
                                                                              User.IsInRole("Client"));

            Guid businessApplicationId = new Guid(Convert.ToString(Session["BusinessAplicationId"]));

            //Get the service order header required in documents
            Form formDefinition = ServiceOrderBusiness.GetServiceOrderForm(businessApplicationId, serviceOrderReportIdDoc, User.IsInRole("Client"), true);

            model.ServiceOrderHeader = formDefinition.ServiceOrderHeader;
            model.OrderIdentifier = formDefinition.OrderIdentifier;
            model.PrincipalFormName = Session["PrincipalFormName"].ToString();
            model.CaptionBreadcrumbsPrincipal = Session["CaptionBreadcrumbs"].ToString();
            model.CaptionTitlePrincipal = Session["CaptionTitle"].ToString();
            return View("Index", model);
        }

        /// <summary>
        /// Call the initialization of ajax uploader control
        /// </summary>
        /// <param name="context"></param>
        private void SetupUploader(HttpContext context)
        {
            using (CuteWebUI.MvcUploader uploader = new CuteWebUI.MvcUploader(context))
            {
                uploader.UploadUrl = Response.ApplyAppPathModifier(VirtualPathUtility.ToAbsolute("~/UploadHandlerDocument.ashx"));
                //Set the max bytes allowed to be uploaded
                uploader.MaxSizeKB = Cotecna.Vestalis.Web.Properties.Settings.Default.MaxFileUploadSizeKB;
                //the data of the uploader will render as <input type='hidden' name='myuploader'> 
                uploader.Name = "myuploader";
                //File extensions allowed to execute the upload
                uploader.AllowedFileExtensions = Cotecna.Vestalis.Web.Properties.Settings.Default.ExtensionDocumentAllowed;
                //set the uploader do not automatically start upload after selecting files
                uploader.ManualStartUpload = true;
                //Flag to upload multiple files
                uploader.MultipleFilesUpload = true;
                uploader.InsertButtonID = "uploadbutton";

                //prepair html code for the view
                ViewData["uploaderhtml"] = uploader.Render();
            }
        }

        /// <summary>
        /// Display the screen to upload a new document
        /// </summary>
        /// <returns></returns>
        public ActionResult NewDocument()
        {
            Guid serviceOrderReportId = Guid.Empty;
            //Get the service order identifier
            if (Session["serviceOrderReportId"] != null)
            {
                serviceOrderReportId = new Guid(Session["serviceOrderReportId"].ToString());
            }
            //Get the model for the screen -> new
            DocumentModel documentModel = PictureDocumentBusiness.GetNewEditDocument(serviceOrderReportId, null, User.IsInRole("Client"));

            Guid businessApplicationId = new Guid(Convert.ToString(Session["BusinessAplicationId"]));

            //Get the service order header required in the new document
            Form formDefinition = ServiceOrderBusiness.GetServiceOrderForm(businessApplicationId, serviceOrderReportId, User.IsInRole("Client"), true);

            documentModel.ServiceOrderHeader = formDefinition.ServiceOrderHeader;
            documentModel.OrderIdentifier = formDefinition.OrderIdentifier;
            //Call the ajax uloader initialization
            SetupUploader(System.Web.HttpContext.Current);
            documentModel.ScreenOpenMode = ScreenOpenMode.Add;
            documentModel.PrincipalFormName = Session["PrincipalFormName"].ToString();
            documentModel.CaptionBreadcrumbsPrincipal = Session["CaptionBreadcrumbs"].ToString();
            documentModel.CaptionTitlePrincipal = Session["CaptionTitle"].ToString();
            return View("Document", documentModel);
        }

        /// <summary>
        /// Upload the picture in the database
        /// </summary>
        /// <param name="myuploader">Files uploaded separated with the format guid/guid/guid</param>
        /// <param name="description">Document description</param>
        /// <param name="documentId">Document identifier</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(string myuploader, string description, string documentId)
        {
            Guid serviceOrderReportId = Guid.Empty;
            Guid? documentIdReal = null;
            if (!String.IsNullOrEmpty(documentId))
                documentIdReal = new Guid(documentId);
            //Get the service order identifier
            if (Session["serviceOrderReportId"] != null)
            {
                serviceOrderReportId = new Guid(Session["serviceOrderReportId"].ToString());
            }
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
                            //Save the document in the database
                            PictureDocumentBusiness.SaveDocument(file, documentIdReal, description, UserName, serviceOrderReportId);
                            processedfiles.Add(file.FileName);
                            file.Delete();
                        }
                    }

                    if (processedfiles.Count > 0)
                    {
                        ViewData["UploadedMessage"] = string.Join(",", processedfiles.ToArray()) + " have been processed.";
                    }
                }
            }

            return RedirectToAction("Index");

        }

        /// <summary>
        /// Perform the save operation when the document is edited
        /// </summary>
        /// <param name="documentIdEdit">Document identifier to edit</param>
        /// <param name="documentIdDescription">Document description to edit</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult SaveDocument(string documentIdEdit, string documentIdDescription)
        {
            Guid serviceOrderReportId = Guid.Empty;
            if (Session["serviceOrderReportId"] != null)
            {
                serviceOrderReportId = new Guid(Session["serviceOrderReportId"].ToString());
            }
            //Save the editing of the document
            PictureDocumentBusiness.SaveDocument(null, new Guid(documentIdEdit), documentIdDescription, UserName, serviceOrderReportId);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Perform the document download
        /// </summary>
        /// <param name="documentId">Document identifier</param>
        public void Preview(Guid documentId)
        {
            Document document =
            PictureDocumentBusiness.GetDocumentDetail(documentId);

            //Show the document for download
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("cache-control", "must-revalidate");
            Response.AddHeader("Content-Disposition", String.Format("attachment;filename={0}", document.DocumentName));
            Response.AddHeader("Content-Length", document.DocumentFile.Length.ToString());
            string extension = Path.GetExtension(document.DocumentName);
            if(extension.ToLower() == ".pdf")
                Response.ContentType = "application/pdf";
            else
                Response.ContentType = "application/otc-stream";
            Response.BinaryWrite(document.DocumentFile);
            Response.End();

            //No view here!
        }

        /// <summary>
        /// Display the screen in editing mode
        /// </summary>
        /// <param name="documentIdEdit">Document identifier that is going to be edited</param>
        /// <returns></returns>
        public ActionResult EditDocument(Guid documentIdEdit)
        {
            Guid serviceOrderReportId = Guid.Empty;
            //Get the service order identifier
            if (Session["serviceOrderReportId"] != null)
            {
                serviceOrderReportId = new Guid(Session["serviceOrderReportId"].ToString());
            }
            //Get the document data for editing
            DocumentModel documentModel = PictureDocumentBusiness.GetNewEditDocument(serviceOrderReportId, documentIdEdit, User.IsInRole("Client"));

            Guid businessApplicationId = new Guid(Convert.ToString(Session["BusinessAplicationId"]));

            //Get the service order header required in the document
            Form formDefinition = ServiceOrderBusiness.GetServiceOrderForm(businessApplicationId, serviceOrderReportId, User.IsInRole("Client"), true);

            documentModel.ServiceOrderHeader = formDefinition.ServiceOrderHeader;
            documentModel.OrderIdentifier = formDefinition.OrderIdentifier;
            documentModel.ScreenOpenMode = ScreenOpenMode.Edit;
            documentModel.PrincipalFormName = Session["PrincipalFormName"].ToString();
            documentModel.CaptionBreadcrumbsPrincipal = Session["CaptionBreadcrumbs"].ToString();
            documentModel.CaptionTitlePrincipal = Session["CaptionTitle"].ToString();
            return View("Document", documentModel);
        }

        /// <summary>
        /// Delete a document
        /// </summary>
        /// <param name="documentId">Document identifier to be deleted</param>
        public void DeleteDocument(Guid? documentId)
        {
            PictureDocumentBusiness.DeleteDocument(documentId.GetValueOrDefault(), UserName);
        }

        /// <summary>
        /// Delete selected documents
        /// </summary>
        /// <param name="selectedIds">Ids of selected documents</param>
        public void DeleteSelectedDocuments(string selectedIds)
        {
            string[] ids = selectedIds.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            List<Guid> documentIds = new List<Guid>();
            foreach (string id in ids)
            {
                documentIds.Add(new Guid(id));
            }
            PictureDocumentBusiness.DeleteSelectedDocuments(documentIds, UserName);
        }

        /// <summary>
        /// Find the document in a specific page selected by the user
        /// </summary>
        /// <param name="page">Page number</param>
        /// <returns>Document list</returns>
        public PartialViewResult SearchDocumentListPaginated(int? page)
        {
            Guid serviceOrderReportId = Guid.Empty;
            //Get the service order identifier
            if (Session["serviceOrderReportId"] != null)
            {
                serviceOrderReportId = new Guid(Session["serviceOrderReportId"].ToString());
            }
            int pageSize = Cotecna.Vestalis.Web.Properties.Settings.Default.PageSize;
            //Search the pictures according a specific page
            DocumentSearchModel model = PictureDocumentBusiness.SearchDocuments(serviceOrderReportId, pageSize, page.GetValueOrDefault(),
                                                                              User.IsInRole("Client"));

            //If there are not documents in the requested page then search in the previous page
            if (model.DocumentList.Collection.Count == 0)
            {
                model = PictureDocumentBusiness.SearchDocuments(serviceOrderReportId, pageSize, page.GetValueOrDefault() - 1,
                                                                              User.IsInRole("Client"));
            }

            Guid businessApplicationId = new Guid(Convert.ToString(Session["BusinessAplicationId"]));

            //Get the service order header required in documents
            Form formDefinition = ServiceOrderBusiness.GetServiceOrderForm(businessApplicationId, serviceOrderReportId, User.IsInRole("Client"), true);

            model.ServiceOrderHeader = formDefinition.ServiceOrderHeader;
            model.OrderIdentifier = formDefinition.OrderIdentifier;
            return PartialView("_DocumentGrid", model);
        }
        #endregion
    }
}
