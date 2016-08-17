using System;
using System.ComponentModel.DataAnnotations;
using DataService.InventoryManagement;
namespace DataService.InventoryManagement
{
    [MetadataType(typeof(AssetMetadata))]
    public partial class Asset
    {
    }

    public partial class AssetMetadata
    {
        [Required(ErrorMessage = "Please enter : Id")]
        [Display(Name = "Id")]
        public long Id { get; set; }

        [Display(Name = "Item")]
        public long ItemMasterId { get; set; }

        [Display(Name = "GRN Id")]
        public Guid GRNId { get; set; }

        [Display(Name = "Is Assetized")]
        public bool IsAssetized { get; set; }

        [Display(Name = "Asset Tag")]
        public string AssetTag { get; set; }

        [Required(ErrorMessage = "Please enter : Assetized By")]
        [Display(Name = "Assetized By")]
        public Guid AssetizedBy { get; set; }

        [Required(ErrorMessage = "Please enter : Assetized Date")]
        [Display(Name = "Assetized Date")]
        public DateTime AssetizedDate { get; set; }

        [Display(Name = "Comment")]
        public string Comment { get; set; }

        [Required(ErrorMessage = "Please enter : Is Issued")]
        [Display(Name = "Is Issued")]
        public bool IsIssued { get; set; }

        [Required(ErrorMessage = "Please enter : ItemMaster")]
        [Display(Name = "ItemMaster")]
        public ItemMaster ItemMaster { get; set; }

        [Display(Name = "AssetIssues")]
        public AssetIssue AssetIssues { get; set; }

    }
}
