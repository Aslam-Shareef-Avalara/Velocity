using System;
using System.ComponentModel.DataAnnotations;
using DataService.InventoryManagement;
namespace DataService.InventoryManagement
{
    [MetadataType(typeof(LegalEntityMetadata))]
    public partial class LegalEntity
    {
    }

    public partial class LegalEntityMetadata
    {
        [Required(ErrorMessage = "Please enter : Id")]
        [Display(Name = "Id")]
        public long Id { get; set; }

        [Required(ErrorMessage = "Please enter : Name")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Vendors")]
        public Vendor Vendors { get; set; }

    }
}
