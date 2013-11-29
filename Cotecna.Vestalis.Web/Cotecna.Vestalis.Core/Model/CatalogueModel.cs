using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cotecna.Vestalis.Core
{
    public class CatalogueModel
    {
        [Required(ErrorMessage = "Please select a business application")]
        public Guid? BusinessApplicationId { get; set; }
        public Guid CatalogueId { get; set; }
        public string BusinessApplicationName { get; set; }
        [Required(ErrorMessage = "Please write a valid category name")]
        public string CatalogueCategoryName { get; set; }
        public bool HasErrors { get { return Errors.Count > 0; } }
        public List<string> Errors { get; set; }
        public ScreenOpenMode ScreenOpenMode { get; set; }

        public CatalogueModel()
        {
            Errors = new List<string>();
        }
    }
}
