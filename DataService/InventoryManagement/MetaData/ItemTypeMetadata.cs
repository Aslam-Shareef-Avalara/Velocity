using System;
using System.ComponentModel.DataAnnotations;
using DataService.InventoryManagement;
namespace DataService.InventoryManagement
{
    [MetadataType(typeof(ItemTypeMetadata))]
    public partial class ItemType
    {
    }

    public partial class ItemTypeMetadata
    {
        [Required(ErrorMessage = "Please enter : Id")]
        [Display(Name = "Id")]
        public long Id { get; set; }

        [Required(ErrorMessage = "Please enter : Name")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter : OrgId")]
        [Display(Name = "OrgId")]
        public int OrgId { get; set; }
    }
}

