using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace Cotecna.Vestalis.Core
{
    public class PaginatedList<T>
    {
        /// <summary>
        /// The collection that have been filtered
        /// </summary>
        public List<T> Collection { get; set; }
        /// <summary>
        /// Total numbers of elements
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// The number of pages
        /// </summary>
        public int NumberOfPages { get; set; }
        /// <summary>
        /// Get or Set Page
        /// </summary>
        public int Page { get; set; }
        /// <summary>
        /// Get or Set PageSize
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// The list will be ordered by the value of this Property 
        /// </summary>
        public string SortedColumn { get; set; }

        /// <summary>
        /// Ascending or Descending enum
        /// </summary>
        public SortDirection SortDirection { get; set; }


        /// <summary>
        /// Get a new instance of the PaginatedList
        /// </summary>
        public PaginatedList()
        {
            TotalCount = 0;
            NumberOfPages = 0;
            Page = 0;
            PageSize = 0;
            Collection = new List<T>();
        }
    }
}
