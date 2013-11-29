using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cotecna.Vestalis.Entities;

namespace Cotecna.Vestalis.Core
{
    /// <summary>
    /// This class is used to setup the document search screen
    /// </summary>
    public class DocumentSearchModel
    {
        /// <summary>
        /// Get an instance of DocumentSearchModel class
        /// </summary>
        public DocumentSearchModel()
        {
            Links = new List<string>();
            DocumentList = new PaginatedList<Document>();
        }

        /// <summary>
        /// Get or set Links
        /// </summary>
        public IList<string> Links { get; set; }
        /// <summary>
        /// Get or set DocumentList
        /// </summary>
        public PaginatedList<Document> DocumentList { get; set; }
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
