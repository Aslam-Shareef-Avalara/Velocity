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
    
    public partial class IssuedItem
    {
        public System.Guid Id { get; set; }
        public long ItemMasterId { get; set; }
        public System.DateTime IssuedOn { get; set; }
        public int Quantity { get; set; }
        public Nullable<System.Guid> IssuedBy { get; set; }
        public Nullable<System.Guid> IssuedTo { get; set; }
        public Nullable<int> OrgId { get; set; }
    
        public virtual ItemMaster ItemMaster { get; set; }
        public virtual Organization Organization { get; set; }
    }
}
