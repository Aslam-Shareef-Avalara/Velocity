using System;
using System.ComponentModel.DataAnnotations;
using DataService.InventoryManagement;
namespace DataService.InventoryManagement
{
    [MetadataType(typeof(VendorAddressMetadata))]
    public partial class VendorAddress
    {
    }

    public partial class VendorAddressMetadata
    {
        [Required(ErrorMessage = "Please enter : Id")]
        [Display(Name = "Id")]
        public long Id { get; set; }

        [Required(ErrorMessage = "Please enter : Vendor")]
        [Display(Name = "Vendor")]
        public long VendorId { get; set; }

        [Required(ErrorMessage = "Please enter : Line1")]
        [Display(Name = "Line1")]
        public string Line1 { get; set; }

        [Display(Name = "Line2")]
        public string Line2 { get; set; }

        [Required(ErrorMessage = "Please enter : City")]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required(ErrorMessage = "Please enter : State")]
        [Display(Name = "State")]
        public long StateId { get; set; }

        [Display(Name = "County")]
        public string County { get; set; }

        [Required(ErrorMessage = "Please enter : Country")]
        [Display(Name = "Country")]
        public long CountryId { get; set; }

        [Required(ErrorMessage = "Please enter : Zip")]
        [Display(Name = "Zip")]
        public string Zip { get; set; }

        [Required(ErrorMessage = "Please enter : Phone1")]
        [Display(Name = "Phone1")]
        public string Phone1 { get; set; }

        [Display(Name = "Phone2")]
        public string Phone2 { get; set; }

        [Display(Name = "Landmark")]
        public string Landmark { get; set; }

        [Display(Name = "Country")]
        public Country Country { get; set; }

        [Display(Name = "State")]
        public State State { get; set; }

        [Display(Name = "Vendor")]
        public Vendor Vendor { get; set; }

    }
}
