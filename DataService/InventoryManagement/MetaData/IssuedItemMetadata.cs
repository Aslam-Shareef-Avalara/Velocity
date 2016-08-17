using System;
using DataService.InventoryManagement;
using System.ComponentModel.DataAnnotations;

namespace DataService.InventoryManagement
{
    [MetadataType(typeof(IssuedItemMetadata))]
    public partial class IssuedItem
    {
    }

    public partial class IssuedItemMetadata
    {
        [Required(ErrorMessage = "Please enter : Id")]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Please enter : Item")]
        [Display(Name = "Item")]
        public long ItemMasterId { get; set; }

        [Required(ErrorMessage = "Please enter : Issued On")]
        [Display(Name = "Issued On")]
        public DateTime IssuedOn { get; set; }

        [Required(ErrorMessage = "Please enter : Quantity")]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Please enter : Issued By")]
        [Display(Name = "Issued By")]
        public Guid IssuedBy { get; set; }

        [Required(ErrorMessage = "Please enter : Issued To")]
        [Display(Name = "Issued To")]
        public Guid IssuedTo { get; set; }

        [Required(ErrorMessage = "Please enter : OrgId")]
        [Display(Name = "OrgId")]
        public int OrgId { get; set; }

        [Display(Name = "ItemMaster")]
        public ItemMaster ItemMaster { get; set; }

    }
}
