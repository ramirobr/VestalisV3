using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cotecna.Vestalis.Entities;

namespace Cotecna.Vestalis.Core
{
    /// <summary>
    /// This class is used to setup add and edit document screens
    /// </summary>
    public class DocumentModel
    {
        /// <summary>
        /// Get an instance of DocumentModel
        /// </summary>
        public DocumentModel()
        {
            Links = new List<string>();
            Document = new Document();
        }
        /// <summary>
        /// Get or set ScreenOpenMode
        /// </summary>
        public ScreenOpenMode ScreenOpenMode { get; set; }
        /// <summary>
        /// Get o set Links
        /// </summary>
        public IList<string> Links { get; set; }
        /// <summary>
        /// Get or set Document
        /// </summary>
        public Document Document { get; set; }
        /// <summary>
        /// Get or set ServiceOrderHeader
        /// </summary>
        public IList<Field> ServiceOrderHeader { get; set; }
        /// <summary>
        /// Get or set OrderIdentifier
        /// </summary>
        public Field OrderIdentifier { get; set; }
        /// <summary>
        /// Get or set PrincipalFormName
        /// </summary>
        public string PrincipalFormName { get; set; }
        /// <summary>
        /// Get or set CaptionBreadcrumbsPrincipal
        /// </summary>
        public string CaptionBreadcrumbsPrincipal { get; set; }
        /// <summary>
        /// Get or set CaptionTitlePrincipal
        /// </summary>
        public string CaptionTitlePrincipal { get; set; }
    }
}
