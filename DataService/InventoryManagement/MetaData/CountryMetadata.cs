using System;
using System.ComponentModel.DataAnnotations;
using DataService.InventoryManagement;

namespace DataService.InventoryManagement
{
    [MetadataType(typeof(CountryMetadata))]
    public partial class Country
    {
    }

    public partial class CountryMetadata
    {
        [Required(ErrorMessage = "Please enter : Id")]
        [Display(Name = "Id")]
        public long Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "States")]
        public State States { get; set; }

    }
}
