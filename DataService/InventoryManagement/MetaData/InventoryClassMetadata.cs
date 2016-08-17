using DataService.InventoryManagement;
using System;
using System.ComponentModel.DataAnnotations;


namespace DataService.InventoryManagement
{
    [MetadataType(typeof(InventoryClassMetadata))]
    public partial class InventoryClass
    {
    }

    public partial class InventoryClassMetadata
    {
        [Required(ErrorMessage = "Please enter : Id")]
        [Display(Name = "Id")]
        public long Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter : OrgId")]
        [Display(Name = "OrgId")]
        public int OrgId { get; set; }

        [Required(ErrorMessage = "Please enter : Inventory Class Department Maps")]
        [Display(Name = "Inventory Class Department Maps")]
        public InventoryClassDepartmentMap InventoryClassDepartmentMaps { get; set; }

        [Required(ErrorMessage = "Please enter : ItemTypes")]
        [Display(Name = "ItemTypes")]
        public ItemType ItemTypes { get; set; }

    }
}
