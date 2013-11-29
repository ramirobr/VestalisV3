using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotecna.Vestalis.Core
{
    /// <summary>
    /// This is used to setup the search picture screen
    /// </summary>
    public class PictureSearchModel
    {
        /// <summary>
        /// get an instance of PictureSearchModel class
        /// </summary>
        public PictureSearchModel()
        {
            Links = new List<string>();
            PictureList = new List<RowPictureCollection>();
        }
        /// <summary>
        /// Get or set Links
        /// </summary>
        public IList<string> Links { get; set; }
        /// <summary>
        /// Get or set PictureList
        /// </summary>
        public List<RowPictureCollection> PictureList { get; set; }
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

    public class PictureSearchModelItem
    {
        public PictureSearchModelItem()
        {
            PictureId = Guid.Empty;
            SizeHeight = 0;
            SizeWidth = 0;
        }

        public Guid PictureId { get; set; }
        public int SizeWidth { get; set; }
        public int SizeHeight { get; set; }
    }

    public class RowPictureCollection
    {
        public List<PictureSearchModelItem> PictureCollection { get; set; }
        public int RowIdentifier { get; set; }

        public RowPictureCollection()
        {
            PictureCollection = new List<PictureSearchModelItem>();
        }
    }
}
