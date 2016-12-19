//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataService
{
    using System;
    using System.Collections.Generic;
    
    public partial class Department
    {
        public Department()
        {
            this.Employees = new HashSet<Employee>();
            this.GRNs = new HashSet<GRN>();
            this.InventoryClassDepartmentMaps = new HashSet<InventoryClassDepartmentMap>();
            this.InvPurchaseApprovers = new HashSet<InvPurchaseApprover>();
            this.ItemMasters = new HashSet<ItemMaster>();
            this.PurchaseRequestHeaders = new HashSet<PurchaseRequestHeader>();
        }
    
        public long Id { get; set; }
        public string Name { get; set; }
        public int OrgId { get; set; }
        public Nullable<System.Guid> DepartmentHead { get; set; }
    
        public virtual Employee Employee { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
        public virtual ICollection<GRN> GRNs { get; set; }
        public virtual ICollection<InventoryClassDepartmentMap> InventoryClassDepartmentMaps { get; set; }
        public virtual ICollection<InvPurchaseApprover> InvPurchaseApprovers { get; set; }
        public virtual ICollection<ItemMaster> ItemMasters { get; set; }
        public virtual ICollection<PurchaseRequestHeader> PurchaseRequestHeaders { get; set; }
    }
}
