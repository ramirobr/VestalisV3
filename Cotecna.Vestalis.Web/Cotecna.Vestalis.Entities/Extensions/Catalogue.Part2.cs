
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Cotecna.Vestalis.Entities
{
    [MetadataType(typeof(CatalogueMetadata))]
    public partial class Catalogue
    {
        public bool CatalogueExist { get; set; }
    }

    public sealed class CatalogueMetadata : IValidatableObject
    {
        [Required(ErrorMessage = "Please select a business application")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
        public Guid? BusinessApplicationId;
        [Required(ErrorMessage = "Please write a valid category name")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
        public String CatalogueCategoryName;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
        public bool CatalogueExist;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CatalogueExist)
            {
                yield return new ValidationResult("The entered category alredy exists, please verify your information");
            }
        }
    }
}
