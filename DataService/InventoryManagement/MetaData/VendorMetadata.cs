using System;
using System.ComponentModel.DataAnnotations;
using DataService.InventoryManagement;

namespace DataService.InventoryManagement
{
    [MetadataType(typeof(VendorMetadata))]
    public partial class Vendor
    {
    }

    public partial class VendorMetadata
    {
        [Required(ErrorMessage = "Please enter : Id")]
        [Display(Name = "Id")]
        public long Id { get; set; }

        [Required(ErrorMessage = "Please enter : Name")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter : Legal Entity")]
        [Display(Name = "Legal Entity")]
        public long LegalEntityId { get; set; }

        [Required(ErrorMessage = "Please enter : Email")]
        [Display(Name = "Email")]
        public string email { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please enter : Date")]
        [Display(Name = "Date")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Please enter : OrgId")]
        [Display(Name = "OrgId")]
        public int OrgId { get; set; }

        [Display(Name = "LegalEntity")]
        public LegalEntity LegalEntity { get; set; }

        [Display(Name = "VendorAddresses")]
        public VendorAddress VendorAddresses { get; set; }

        [Display(Name = "VendorBankDetails")]
        public VendorBankDetail VendorBankDetails { get; set; }

        [Display(Name = "VendorContacts")]
        public VendorContact VendorContacts { get; set; }

        [Display(Name = "VendorTaxDetails")]
        public VendorTaxDetail VendorTaxDetails { get; set; }

    }
}
