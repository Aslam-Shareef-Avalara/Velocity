using System;
using System.ComponentModel.DataAnnotations;
using DataService.InventoryManagement;
namespace DataService.InventoryManagement
{
    [MetadataType(typeof(DepartmentMetadata))]
    public partial class Department
    {
    }

    public partial class DepartmentMetadata
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

