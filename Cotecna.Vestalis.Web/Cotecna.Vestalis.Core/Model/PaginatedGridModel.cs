
using System.Web.UI.WebControls;
namespace Cotecna.Vestalis.Core
{
    public class PaginatedGridModel
    {
        /// <summary>
        /// The list will be ordered by the value of this Property 
        /// </summary>
        public string SortedColumn { get; set; }

        /// <summary>
        /// Ascending or Descending enum
        /// </summary>
        public SortDirection SortDirection { get; set; }

        /// <summary>
        /// The number of the page. If not declared the first is taken
        /// </summary>
        public int? Page { get; set; }

        /// <summary>
        /// When one of the methods is used, it returns the number of items in the list
        /// </summary>
        public int? TotalCount { get; set; }

        /// <summary>
        /// When a paging method is used, it returns the number of pages in the list
        /// </summary>
        public int? NumberOfPages { get; set; }

        /// <summary>
        /// If the paginated list has more register to show
        /// </summary>
        /// <returns></returns>
        public bool HasNext()
        {
            if (NumberOfPages > Page && NumberOfPages > 1)
                return true;
            return false;
        }

        public int TotalNumberOfItemsWithoutPagination { get; set; }

        public int PageSize { get; set; }
    }
}
