using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CuteWebUI;

namespace Cotecna.Vestalis.Web
{
    /// <summary>
    /// Summary description for UploadHandlerDocument
    /// </summary>
    public class UploadHandlerDocument : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            using (CuteWebUI.MvcUploader uploader = new CuteWebUI.MvcUploader(context))
            {
                uploader.UploadUrl = context.Response.ApplyAppPathModifier(VirtualPathUtility.ToAbsolute("~/UploadHandlerDocument.ashx"));
                //the data of the uploader will render as <input type='hidden' name='myuploader'> 
                uploader.Name = "myuploader";
                uploader.AllowedFileExtensions =
                    Cotecna.Vestalis.Web.Properties.Settings.Default.ExtensionDocumentAllowed;
                //let uploader process the common task and return common result
                uploader.PreProcessRequest();

                //if need validate the file : (after the PreProcessRequest have validated the size/extensions)
                if (uploader.IsValidationRequest)
                {
                    //get the file need be validated:
                    uploader.WriteValidationOK();
                    return;
                }
                else
                {
                    uploader.WriteValidationError(Resources.Common.FileTypeIsNotAuthorized);
                }
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}