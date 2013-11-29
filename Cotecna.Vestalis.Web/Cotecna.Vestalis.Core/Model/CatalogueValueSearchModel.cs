using System;
using Cotecna.Vestalis.Entities;

namespace Cotecna.Vestalis.Core
{
    public class CatalogueValueSearchModel
    {
        public string CatalogueSelectedName { get; set; }
        public string BusinessApplicatioName { get; set; }
        public Guid CatalogueSelectedId { get; set; }
        public PaginatedList<CatalogueValue> SearchResult { get; set; }

        public CatalogueValueSearchModel()
        {
            CatalogueSelectedName = string.Empty;
            BusinessApplicatioName = string.Empty;
            CatalogueSelectedId = Guid.Empty;
            SearchResult = new PaginatedList<CatalogueValue>();
        }
    }
}
