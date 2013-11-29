using System;
using System.Collections.Generic;

namespace Cotecna.Vestalis.Core
{
    public class CatalogueValueModel
    {
        public Guid? CatalogueId { get; set; }
        public string CatalogValData { get; set; }
        public string CatalogDesc { get; set; }
        public Guid? CatalogValId { get; set; }

        public List<string> Errors { get; set; }
        public bool HasErrors { get { return Errors.Count > 0; } }

        public CatalogueValueModel()
        {
            Errors = new List<string>();
        }
    }
}
