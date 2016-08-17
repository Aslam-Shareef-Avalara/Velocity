using System;
using System.ComponentModel.DataAnnotations;
using DataService.InventoryManagement;

namespace DataService.InventoryManagement
{
    [MetadataType(typeof(VendorContactMetadata))]
    public partial class VendorContact
    {
    }

    public partial class VendorContactMetadata
    {
        [Required(ErrorMessage = "Please enter : Id")]
        [Display(Name = "Id")]
        public long Id { get; set; }

        [Required(ErrorMessage = "Please enter : Vendor")]
        [Display(Name = "Vendor")]
        public long VendorId { get; set; }

        [Required(ErrorMessage = "Please enter : First Name")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter : Last Name")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter : Phone")]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Please enter : Mobile")]
        [Display(Name = "Mobile")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "Please enter : Email")]
        [Display(Name = "Email")]
        public string email { get; set; }

        [Display(Name = "Vendor")]
        public Vendor Vendor { get; set; }

    }
}
