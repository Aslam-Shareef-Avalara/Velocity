using System;
using System.ComponentModel.DataAnnotations;
using DataService.InventoryManagement;
namespace DataService.InventoryManagement
{
    [MetadataType(typeof(StateMetadata))]
    public partial class State
    {
    }

    public partial class StateMetadata
    {
        [Required(ErrorMessage = "Please enter : Id")]
        [Display(Name = "Id")]
        public long Id { get; set; }

        [Required(ErrorMessage = "Please enter : Name")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter : Country")]
        [Display(Name = "Country")]
        public long CountryId { get; set; }

        [Display(Name = "Country")]
        public Country Country { get; set; }

    }
}
