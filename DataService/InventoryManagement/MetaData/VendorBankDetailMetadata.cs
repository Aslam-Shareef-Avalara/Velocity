using System;
using System.ComponentModel.DataAnnotations;
using DataService.InventoryManagement;
namespace DataService.InventoryManagement
{
    [MetadataType(typeof(VendorBankDetailMetadata))]
    public partial class VendorBankDetail
    {
    }

    public partial class VendorBankDetailMetadata
    {
        [Required(ErrorMessage = "Please enter : Id")]
        [Display(Name = "Id")]
        public long Id { get; set; }

        [Required(ErrorMessage = "Please enter : Vendor")]
        [Display(Name = "Vendor")]
        public long VendorId { get; set; }

        [Required(ErrorMessage = "Please enter : Beneficiary Name")]
        [Display(Name = "Beneficiary Name")]
        public string BeneficiaryName { get; set; }

        [Required(ErrorMessage = "Please enter : Account Number")]
        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }

        [Required(ErrorMessage = "Please enter : Bank Name")]
        [Display(Name = "Bank Name")]
        public string BankName { get; set; }

        [Required(ErrorMessage = "Please enter : Branch")]
        [Display(Name = "Branch")]
        public byte[] Branch { get; set; }

        [Display(Name = "MICR")]
        public byte[] MICR { get; set; }

        [Display(Name = "IFSC")]
        public string IFSC { get; set; }

        [Display(Name = "Vendor")]
        public Vendor Vendor { get; set; }

    }
}
