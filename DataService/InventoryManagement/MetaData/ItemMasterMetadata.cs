using System;
using System.ComponentModel.DataAnnotations;
using DataService.InventoryManagement;
namespace DataService.InventoryManagement
{
    [MetadataType(typeof(ItemMasterMetadata))]
    public partial class ItemMaster
    {
    }

    public partial class ItemMasterMetadata
    {
        [Required(ErrorMessage = "Please enter : Id")]
        [Display(Name = "Id")]
        public long Id { get; set; }

        [Required(ErrorMessage = "Please enter : Name")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter : Item Type")]
        [Display(Name = "Item Type")]
        public long ItemTypeId { get; set; }

        [Required(ErrorMessage = "Please enter : Quantity")]
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }

        [Required(ErrorMessage = "Please enter : Perishable")]
        [Display(Name = "Perishable")]
        public bool Perishable { get; set; }

        [Required(ErrorMessage = "Please enter : OrgId")]
        [Display(Name = "OrgId")]
        public int OrgId { get; set; }
    }
}

