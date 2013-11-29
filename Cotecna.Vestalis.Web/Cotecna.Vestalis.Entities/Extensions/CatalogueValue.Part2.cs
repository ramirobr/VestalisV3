
using System.ComponentModel.DataAnnotations;
namespace Cotecna.Vestalis.Entities
{
    [MetadataType(typeof(CatalogueValueMetaData))]
    public partial class CatalogueValue
    {

    }

    public sealed class CatalogueValueMetaData
    {
        [Required(ErrorMessage = "Please write a valid value")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
        public string CatalogueValueData;
    }
}
