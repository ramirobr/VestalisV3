using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using Cotecna.Vestalis.Entities;

namespace Cotecna.Vestalis.Core
{
    /// <summary>
    /// This class contains all methods needed to manage all information about pictures
    /// </summary>
    public static class PictureDocumentBusiness
    {
        /// <summary>
        /// Get the Picture according the identifier
        /// </summary>
        /// <param name="pictureId">Picture identifier</param>
        /// <returns>Picture data</returns>
        public static Picture GetDetailPicture(Guid pictureId)
        {
            using (VestalisEntities ctx = new VestalisEntities())
            {
                return (from picture in ctx.Pictures
                        where picture.PictureId == pictureId
                        select picture).FirstOrDefault();
            }
        }

        /// <summary>
        /// Search the pictures
        /// </summary>
        /// <param name="serviceOrderId">Service order identifier</param>
        /// <param name="rowSize">Size of the row</param>
        /// <param name="isClient">Flag to know if the user is the client</param>
        /// <returns>Picture results</returns>
        public static PictureSearchModel SearchPictures(Guid serviceOrderId,int rowSize, bool isClient = false)
        {
            PictureSearchModel pictureSearchModel = new PictureSearchModel();

            using (VestalisEntities ctx = new VestalisEntities())
            {
                 
                //Get the pictures in a specific page
                var tempResult = (from picture in ctx.Pictures
                                  where picture.ServiceOrderId == serviceOrderId && picture.IsDeleted == false
                                  && picture.InspectionReportItemId == null
                                  orderby picture.CreationDate
                                  select new { picture.PictureId, picture.PictureFile }).ToList();

                //if exists data, the system makes the pagination
                if (tempResult != null)
                {
                    int rowsCount = (int)Math.Ceiling((double)tempResult.Count / (double)rowSize);

                    for (int i = 0; i < rowsCount; i++)
                    {
                        int currentIndex = i * rowSize;
                        RowPictureCollection currentRow = new RowPictureCollection();
                        currentRow.RowIdentifier = i + 1;
                        //set the paginated colletion
                        foreach (var tempPicture in tempResult.Skip(currentIndex).Take(rowSize))
                        {
                            PictureSearchModelItem pictureSearchModelItem = new PictureSearchModelItem();
                            pictureSearchModelItem.PictureId = tempPicture.PictureId;
                            Image pictureImage = ByteArrayToImage(tempPicture.PictureFile);
                            pictureSearchModelItem.SizeHeight = pictureImage.Height;
                            pictureSearchModelItem.SizeWidth = pictureImage.Width;
                            currentRow.PictureCollection.Add(pictureSearchModelItem);
                        }
                        pictureSearchModel.PictureList.Add(currentRow);
                    }
                    
                }
            }

            //Search the links to the inspection reports
            pictureSearchModel.Links = InspectionReportBusiness.GetInspectionReportLinks(serviceOrderId, isClient);
            return pictureSearchModel;
        }

        /// <summary>
        /// Search the pictures for showing in the inspection report grid
        /// </summary>
        /// <param name="serviceOrderId">Service order id</param>
        /// <param name="inspectionReportItemId">Inspection report id</param>
        /// <returns>PictureGridModel</returns>
        public static PictureGridModel SearchPictureGridInspectionReport(Guid serviceOrderId, Guid inspectionReportItemId)
        {
            PictureGridModel result = new PictureGridModel();
            
            using (VestalisEntities context = new VestalisEntities())
            {
                //Retrieve the total number of pictures
                result.PictureCount = context.Pictures.Where(data => data.ServiceOrderId == serviceOrderId
                    && data.InspectionReportItemId == inspectionReportItemId && data.IsDeleted == false).Count();

                //Get the pictures in a specific page
                var tempResult = (from picture in context.Pictures
                                  where picture.ServiceOrderId == serviceOrderId && picture.IsDeleted == false
                                  && picture.InspectionReportItemId == inspectionReportItemId
                                  orderby picture.CreationDate
                                  select new { picture.PictureId, picture.PictureFile }).Take(3).ToList();

                //if exists data, the system makes the pagination
                if (tempResult != null)
                {   
                    //set the paginated colletion
                    foreach (var tempPicture in tempResult)
                    {
                        PictureSearchModelItem pictureSearchModelItem = new PictureSearchModelItem();
                        pictureSearchModelItem.PictureId = tempPicture.PictureId;
                        Image pictureImage = ByteArrayToImage(tempPicture.PictureFile);
                        pictureSearchModelItem.SizeHeight = pictureImage.Height;
                        pictureSearchModelItem.SizeWidth = pictureImage.Width;
                        result.PictureList.Add(pictureSearchModelItem);
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// Search the pictures
        /// </summary>
        /// <param name="serviceOrderId">Service order identifier</param>
        /// <param name="inspectionReportItemId">Id of inspection report</param>
        /// <param name="rowSize">Row size</param>
        /// <returns>Picture results</returns>
        public static List<RowPictureCollection> SearchPicturesInspectionReport(Guid serviceOrderId, int rowSize, Guid inspectionReportItemId)
        {
            List<RowPictureCollection> result = new List<RowPictureCollection>();

            using (VestalisEntities ctx = new VestalisEntities())
            {

                //Get the pictures in a specific page
                var tempResult = (from picture in ctx.Pictures
                                  where picture.ServiceOrderId == serviceOrderId && picture.IsDeleted == false
                                  && picture.InspectionReportItemId == inspectionReportItemId
                                  orderby picture.CreationDate
                                  select new { picture.PictureId, picture.PictureFile }).ToList();

                //if exists data, the system makes the pagination
                if (tempResult != null)
                {
                    int rowsCount = (int)Math.Ceiling((double)tempResult.Count / (double)rowSize);

                    for (int i = 0; i < rowsCount; i++)
                    {
                        int currentIndex = i * rowSize;
                        RowPictureCollection currentRow = new RowPictureCollection();
                        currentRow.RowIdentifier = i + 1;
                        //set the paginated colletion
                        foreach (var tempPicture in tempResult.Skip(currentIndex).Take(rowSize))
                        {
                            PictureSearchModelItem pictureSearchModelItem = new PictureSearchModelItem();
                            pictureSearchModelItem.PictureId = tempPicture.PictureId;
                            Image pictureImage = ByteArrayToImage(tempPicture.PictureFile);
                            pictureSearchModelItem.SizeHeight = pictureImage.Height;
                            pictureSearchModelItem.SizeWidth = pictureImage.Width;
                            currentRow.PictureCollection.Add(pictureSearchModelItem);
                        }
                        result.Add(currentRow);
                    }


                }
            }

            return result;
        }

        /// <summary>
        /// Convert byte array to an image
        /// </summary>
        /// <param name="byteArrayIn">Bytes of the image</param>
        /// <returns>Image converted</returns>
        public static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            Image returnImage = null;
            MemoryStream ms = null;
            try
            {
                ms = new MemoryStream(byteArrayIn);
                returnImage = Image.FromStream(ms);
            }
            finally
            {
                if (ms != null) ms.Dispose();
            }
            return returnImage;
        }

        /// <summary>
        /// Save the picture in the database
        /// </summary>
        /// <param name="file">File uploaded by the user</param>
        /// <param name="userName">User name logged in the application</param>
        /// <param name="serviceOrderReportId">Service order identifier related</param>
        /// <param name="inspectionReportItemId">Id of inspections report item</param>
        public static void UploadPicture(CuteWebUI.MvcUploadFile file, Guid serviceOrderReportId, string userName, Guid? inspectionReportItemId = null)
        {
            //Add the file to DB along with metadata
            using (VestalisEntities context = new VestalisEntities())
            {
                //Create a new picture
                Picture picture = new Picture();
                picture.PictureId = Guid.NewGuid();
                picture.ServiceOrderId = serviceOrderReportId;
                if (inspectionReportItemId.HasValue)
                    picture.InspectionReportItemId = inspectionReportItemId;
                //Copy document to entity
                using (Stream fileStream = file.OpenStream())
                {
                    byte[] buffer = new byte[fileStream.Length];
                    fileStream.Position = 0;
                    fileStream.Read(buffer, 0, (int)fileStream.Length);
                    picture.PictureFile = buffer;
                    //Resize the thumbnail picture 
                    Image imageInput = Image.FromStream(fileStream);
                    Image thumbnailImage = ResizeImage(imageInput, new Size(200, 200));
                    ImageConverter converter = new ImageConverter();
                    byte[] byteArrayThumb = (byte[])converter.ConvertTo(thumbnailImage, typeof(byte[]));
                    //Set the thumbnail image to save in the database
                    picture.PictureFileThumbnail = byteArrayThumb;
                }
                picture.CreationBy = userName;
                picture.CreationDate = DateTime.UtcNow;
                context.Pictures.AddObject(picture);
                //Save the new picture
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Resize function that keep the height and width proportional and highest quality results.
        /// </summary>
        /// <param name="imgToResize"></param>
        /// <param name="size"></param>
        /// <returns>Image resized with highest quality</returns>
        public static Image ResizeImage(Image imgToResize, Size size)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)size.Width / (float)sourceWidth);
            nPercentH = ((float)size.Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();

            return (Image)b;
        }

        /// <summary>
        /// Delete pictures
        /// </summary>
        /// <param name="pictureIds">Pictures identifiers</param>
        /// <param name="userName">User name logged in the application</param>
        public static void DeletePictures(string pictureIds, string userName)
        {
            //Split the pictures ids according the separator "&&&"
            string[] values = pictureIds.Split(new string[] { "&&&" }, StringSplitOptions.RemoveEmptyEntries);
            Guid[] valuesGuid = values.Select(x => new Guid(x)).ToArray();
            Picture pictureResult = null;
            using (VestalisEntities context = new VestalisEntities())
            {
                foreach (Guid pictureId in valuesGuid)
                {
                    //Get the picture according the picture identifier
                    pictureResult =
                    (from picture in context.Pictures where picture.PictureId == pictureId select picture).
                        FirstOrDefault();
                    if (pictureResult != null)
                    {
                        //Set the picture as deleted
                        pictureResult.IsDeleted = true;
                        pictureResult.ModificationDate = DateTime.UtcNow;
                        pictureResult.ModificationBy = userName;
                    }
                }
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Search the documents related to one service order
        /// </summary>
        /// <param name="serviceOrderId">Service order identifier</param>
        /// <param name="pageSize">Number of documents to retrieve</param>
        /// <param name="selectedPage">Page requested by the user</param>
        /// <param name="isClient">Flag to know if the user is the client</param>
        /// <returns>List of the documents</returns>
        public static DocumentSearchModel SearchDocuments(Guid serviceOrderId, int pageSize, int selectedPage, bool isClient = false)
        {
            DocumentSearchModel documentSearchModel = new DocumentSearchModel();
            List<Document> tempResult = null;
            int currentIndex = (selectedPage - 1) * pageSize;
            int countElements = 0;

            using (VestalisEntities ctx = new VestalisEntities())
            {
                //Retrieve the total number of pictures
                countElements = ctx.Documents.Where(data => data.ServiceOrderId == serviceOrderId && data.IsDeleted == false).Count();

                if (countElements > 0)
                {
                    //Get the pictures in a specific page
                    tempResult = (from document in ctx.Documents
                                  where document.ServiceOrderId == serviceOrderId && document.IsDeleted == false
                                  orderby document.CreationDate
                                  select document).Skip(currentIndex).Take(pageSize).ToList();
                }
                else
                {
                    //Get the pictures in a specific page
                    tempResult = (from document in ctx.Documents
                                  where document.ServiceOrderId == serviceOrderId && document.IsDeleted == false
                                  orderby document.CreationDate
                                  select document).ToList();
                }

                //if exists data, the system makes the pagination
                if (tempResult != null)
                {
                    //set the paginated colletion
                    documentSearchModel.DocumentList.Collection = tempResult;
                    //set the quantity of elements without pagination
                    documentSearchModel.DocumentList.TotalCount = countElements;
                    //set the number of pages
                    documentSearchModel.DocumentList.NumberOfPages = (int)Math.Ceiling((double)documentSearchModel.DocumentList.TotalCount / (double)pageSize);
                    //set the current page
                    documentSearchModel.DocumentList.Page = selectedPage;
                    //set the page size
                    documentSearchModel.DocumentList.PageSize = pageSize;
                }
            }

            documentSearchModel.Links = InspectionReportBusiness.GetInspectionReportLinks(serviceOrderId, isClient);
            return documentSearchModel;
        }

        /// <summary>
        /// Get the document information to add or edit a document
        /// </summary>
        /// <param name="serviceOrderId">Service order identifier</param>
        /// <param name="documentId">Document identifier. It has value in edit mode; otherwise, it is null</param>
        /// <param name="isClient"></param>
        /// <returns></returns>
        public static DocumentModel GetNewEditDocument(Guid serviceOrderId, Guid? documentId, bool isClient = false)
        {
            DocumentModel documentModel = new DocumentModel();
            using (VestalisEntities ctx = new VestalisEntities())
            {
                //Get the document information when it is required for editing
                if (documentId.HasValue)
                {
                    documentModel.Document =
                    (from document in ctx.Documents
                     where document.DocumentId == documentId
                     select document).FirstOrDefault();
                }
            }
            documentModel.Links = InspectionReportBusiness.GetInspectionReportLinks(serviceOrderId, isClient);
            return documentModel;
        }

        /// <summary>
        /// Save the picture in the database
        /// </summary>
        /// <param name="file">File uploaded by the user</param>
        /// <param name="userName">User name logged in the application</param>
        /// <param name="serviceOrderReportId">Service order identifier related</param>
        public static void UploadDocument(CuteWebUI.MvcUploadFile file, Guid serviceOrderReportId, string userName)
        {
            //Add the file to DB along with metadata
            using (VestalisEntities context = new VestalisEntities())
            {
                //Create a new picture
                Picture picture = new Picture();
                picture.PictureId = Guid.NewGuid();
                picture.ServiceOrderId = serviceOrderReportId;
                //Copy the document to the entity
                using (Stream fileStream = file.OpenStream())
                {
                    byte[] buffer = new byte[fileStream.Length];
                    fileStream.Position = 0;
                    fileStream.Read(buffer, 0, (int)fileStream.Length);
                    picture.PictureFile = buffer;

                }
                picture.CreationBy = userName;
                picture.CreationDate = DateTime.UtcNow;
                context.Pictures.AddObject(picture);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Save a new or existing catalogue in the database
        /// </summary>
        /// <param name="file">File to be saved in the document</param>
        /// <param name="documentId">Document identifier</param>
        /// <param name="description">Document description</param>
        /// <param name="userName">Name of logged user</param>
        /// <param name="serviceOrderId">Service order idenfier</param>
        public static void SaveDocument(CuteWebUI.MvcUploadFile file, Guid? documentId, string description, string userName, Guid serviceOrderId)
        {
            Document documentNewEdit = null;
            using (VestalisEntities context = new VestalisEntities())
            {
                //if the entity don't have a catalogue id, the system will perform insert operation
                //otherwise, the system will perform edit operation
                if (documentId == Guid.Empty)
                {
                    documentNewEdit = new Document();
                    documentNewEdit.DocumentId = Guid.NewGuid();
                    documentNewEdit.ServiceOrderId = serviceOrderId;
                    //set auditory fields
                    documentNewEdit.CreationBy = userName;
                    documentNewEdit.CreationDate = DateTime.UtcNow;
                    documentNewEdit.DocumentDescription = description;
                    documentNewEdit.DocumentName = file.FileName;
                    //Copy the document to the entity
                    using (Stream fileStream = file.OpenStream())
                    {
                        byte[] buffer = new byte[fileStream.Length];
                        fileStream.Position = 0;
                        fileStream.Read(buffer, 0, (int)fileStream.Length);
                        documentNewEdit.DocumentFile = buffer;

                    }
                    context.Documents.AddObject(documentNewEdit);
                }
                else
                {
                    //get the current catalogue to edit
                    documentNewEdit = context.Documents
                    .FirstOrDefault(data => data.IsDeleted == false && data.DocumentId == documentId);

                    //set the description entered by the user
                    documentNewEdit.DocumentDescription = description;
                    //set auditory fields
                    documentNewEdit.ModificationBy = userName;
                    documentNewEdit.ModificationDate = DateTime.UtcNow;
                }

                //save changes
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Get the document information
        /// </summary>
        /// <param name="documentId">Document identifier</param>
        /// <returns>Document data</returns>
        public static Document GetDocumentDetail(Guid documentId)
        {
            using (VestalisEntities ctx = new VestalisEntities())
            {
                return (from document in ctx.Documents
                        where document.DocumentId == documentId
                        select document).FirstOrDefault();
            }
        }

        /// <summary>
        /// Delete a document
        /// </summary>
        /// <param name="documentId">Document identifier</param>
        /// <param name="userName">User name logged in the application</param>
        public static void DeleteDocument(Guid documentId, string userName)
        {
            Document documentToDelete = null;
            using (VestalisEntities context = new VestalisEntities())
            {
                //Get the document from the database 
                documentToDelete = context.Documents.FirstOrDefault(data => data.IsDeleted == false && data.DocumentId == documentId);
                if (documentToDelete != null)
                {
                    //Set the is_deleted flag as true
                    documentToDelete.IsDeleted = true;
                    //Set the audit fields
                    documentToDelete.ModificationBy = userName;
                    documentToDelete.ModificationDate = DateTime.UtcNow;
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Delete all selected documents
        /// </summary>
        /// <param name="selectedIds">Ids of selected documents</param>
        /// <param name="userName">Name of the current user</param>
        public static void DeleteSelectedDocuments(List<Guid> selectedIds, string userName)
        {
            using (VestalisEntities context = new VestalisEntities())
            {
                var documentList = (from document in context.Documents
                                    where document.IsDeleted == false &
                                    selectedIds.Contains(document.DocumentId)
                                    select document).ToList();
                
                if (documentList != null && documentList.Count > 0)
                {
                    documentList.ForEach(documentToDelete =>
                    {
                        //Set the is_deleted flag as true
                        documentToDelete.IsDeleted = true;
                        //Set the audit fields
                        documentToDelete.ModificationBy = userName;
                        documentToDelete.ModificationDate = DateTime.UtcNow;
                        context.SaveChanges();
                    });
                }
            }
        }
    }
}
