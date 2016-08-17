using System;
using System.ComponentModel.DataAnnotations;
using DataService.InventoryManagement;
namespace DataService.InventoryManagement
{
    [MetadataType(typeof(AssetIssueMetadata))]
    public partial class AssetIssue
    {
    }

    public partial class AssetIssueMetadata
    {
        [Required(ErrorMessage = "Please enter : Id")]
        [Display(Name = "Id")]
        public long Id { get; set; }

        [Display(Name = "Asset")]
        public long AssetId { get; set; }

        [Display(Name = "Date Issued")]
        public DateTime DateIssued { get; set; }

        [Display(Name = "Issued To")]
        public Guid IssuedTo { get; set; }

        [Display(Name = "Comment")]
        public string Comment { get; set; }

        [Display(Name = "Issued By")]
        public Guid IssuedBy { get; set; }

        [Display(Name = "Asset")]
        public Asset Asset { get; set; }

    }
}
