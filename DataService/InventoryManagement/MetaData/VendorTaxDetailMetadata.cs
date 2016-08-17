using System;
using System.ComponentModel.DataAnnotations;
using DataService.InventoryManagement;

namespace DataService.InventoryManagement
{
    [MetadataType(typeof(VendorTaxDetailMetadata))]
    public partial class VendorTaxDetail
    {
    }

    public partial class VendorTaxDetailMetadata
    {
        [Required(ErrorMessage = "Please enter : Id")]
        [Display(Name = "Id")]
        public long Id { get; set; }

        [Required(ErrorMessage = "Please enter : Vendor")]
        [Display(Name = "Vendor")]
        public long VendorId { get; set; }

        [Display(Name = "VAT Number")]
        public string VATNumber { get; set; }

        [Display(Name = "TIN Number")]
        public string TINNumber { get; set; }

        [Display(Name = "Service Tax Number")]
        public string ServiceTaxNumber { get; set; }

        [Display(Name = "Pan Number")]
        public string PanNumber { get; set; }

        [Display(Name = "Vendor")]
        public Vendor Vendor { get; set; }

    }
}
