using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Cotecna.Vestalis.Core;
using Cotecna.Vestalis.Entities;

namespace Cotecna.Vestalis.Web.Controllers
{
    /// <summary>
    /// This class contains all action methods needed for managing all about pictures
    /// </summary>
    public class PictureController : BaseController
    {
       
        #region public methods

        #region Index
        /// <summary>
        /// Display the initial screen
        /// </summary>
        /// <returns>Render the Picture View</returns>
        public ActionResult Index(Guid? serviceOrderReportIdPicture)
        {
            if (serviceOrderReportIdPicture.HasValue)
                Session.Add("serviceOrderReportId", serviceOrderReportIdPicture);
            else
            {
                if (Session["serviceOrderReportId"] != null)
                    serviceOrderReportIdPicture = new Guid(Session["serviceOrderReportId"].ToString());
            }

            int rowSize = Properties.Settings.Default.PageSizePicture;
            //Search the pictures of the service order
            PictureSearchModel model = PictureDocumentBusiness.SearchPictures(serviceOrderReportIdPicture.GetValueOrDefault(), rowSize, User.IsInRole("Client"));
            Guid businessApplicationId = new Guid(Convert.ToString(Session["BusinessAplicationId"]));

            //Get the service order header required in pictures
            Form formDefinition = ServiceOrderBusiness.GetServiceOrderForm(businessApplicationId, serviceOrderReportIdPicture, User.IsInRole("Client"), true);

            model.ServiceOrderHeader = formDefinition.ServiceOrderHeader;
            model.OrderIdentifier = formDefinition.OrderIdentifier;
            model.PrincipalFormName = Session["PrincipalFormName"].ToString();
            model.CaptionBreadcrumbsPrincipal = Session["CaptionBreadcrumbs"].ToString();
            model.CaptionTitlePrincipal = Session["CaptionTitle"].ToString();
            //Call the initialization of ajax uploader control
            SetupUploader(System.Web.HttpContext.Current);
            //Number of pictures to be displayed in the grid
            ViewBag.PageSizePicture = Cotecna.Vestalis.Web.Properties.Settings.Default.PageSizePicture;
            return View("Index", model);
        }
        #endregion

        #region GetImage
        /// <summary>
        /// Get the imagen to be displayed as tumbnails according the picture identifier
        /// </summary>
        /// <param name="pictureId">Picture identifier</param>
        /// <returns></returns>
        public FileContentResult GetImage(Guid pictureId)
        {
            Picture pic = PictureDocumentBusiness.GetDetailPicture(pictureId);
            //Get the picture to be displayed in the screen. It is the thumbnail or the original picture.
            byte[] pictureThumb = pic.PictureFileThumbnail != null ? pic.PictureFileThumbnail : pic.PictureFile;
            if (pic != null)
            {
                return File(pictureThumb, "image/png");
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region ViewImage
        /// <summary>
        /// View the picture in a greater size
        /// </summary>
        /// <param name="pictureId">Picture size</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public ActionResult ViewImage(Guid pictureId)
        {
            byte[] bytes = GetImageBytes(pictureId);
            return new FileContentResult(bytes, "image/jpeg");
        }
        #endregion

        #region GetImageBytes
        /// <summary>
        /// Get the bytes of the image
        /// </summary>
        /// <param name="pictureId">Picture identifier</param>
        /// <returns>Picture bytes</returns>
        public static byte[] GetImageBytes(Guid pictureId)
        {
            byte[] bytes = null;
            Picture pic = PictureDocumentBusiness.GetDetailPicture(pictureId);
            if (pic != null)
            {
                bytes = pic.PictureFile;
            }
            return bytes;
        }
        #endregion

        #region Upload
        /// <summary>
        /// Upload the picture in the database
        /// </summary>
        /// <param name="myuploader"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(string myuploader)
        {
            Guid serviceOrderReportId = Guid.Empty;
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
                            //Save the picture in the database
                            PictureDocumentBusiness.UploadPicture(file, serviceOrderReportId, this.UserName);
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
        #endregion

        #region DeletePicture
        /// <summary>
        /// Delete some pictures selected by the user from the database
        /// </summary>
        /// <param name="pictureIds">Picture ids separated by &&&</param>
        /// <returns>Picture list refresed</returns>
        public PartialViewResult DeletePicture(string pictureIds)
        {
            Guid serviceOrderReportId = Guid.Empty;
            //Get the service order identifier
            if (Session["serviceOrderReportId"] != null)
            {
                serviceOrderReportId = new Guid(Session["serviceOrderReportId"].ToString());
            }

            //Delete the pictures from the database
            PictureDocumentBusiness.DeletePictures(pictureIds, this.UserName);
            //Search the pictures
            int rowSize = Properties.Settings.Default.PageSizePicture;
            PictureSearchModel model = PictureDocumentBusiness.SearchPictures(serviceOrderReportId, rowSize, User.IsInRole("Client"));
            
            Guid businessApplicationId = new Guid(Convert.ToString(Session["BusinessAplicationId"]));

            //Get the service order header required in pictures
            Form formDefinition = ServiceOrderBusiness.GetServiceOrderForm(businessApplicationId, serviceOrderReportId, User.IsInRole("Client"), true);
            model.ServiceOrderHeader = formDefinition.ServiceOrderHeader;
            model.OrderIdentifier = formDefinition.OrderIdentifier;
            ViewBag.PageSizePicture = Cotecna.Vestalis.Web.Properties.Settings.Default.PageSizePicture;
            return PartialView("_PictureGrid", model);
        }
        #endregion

        #endregion

        #region private methods

        #region SetupUploader
        /// <summary>
        /// Call the initialization of ajax uploader control
        /// </summary>
        /// <param name="context"></param>
        private void SetupUploader(HttpContext context)
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

                //prepair html code for the view
                ViewData["uploaderhtml"] = uploader.Render();
            }
        }
        #endregion

        #endregion

    }
}
