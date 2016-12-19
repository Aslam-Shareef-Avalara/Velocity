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
    
    public partial class GRN
    {
        public GRN()
        {
            this.GRNLines = new HashSet<GRNLine>();
            this.SignOffCertificates = new HashSet<SignOffCertificate>();
        }
    
        public System.Guid Id { get; set; }
        public Nullable<System.Guid> InvoiceHeaderId { get; set; }
        public long DepartmentId { get; set; }
        public bool Approved { get; set; }
        public Nullable<System.Guid> ApprovedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string Notes { get; set; }
        public int OrgId { get; set; }
        public Nullable<System.Guid> QuoteId { get; set; }
        public Nullable<System.Guid> POId { get; set; }
        public Nullable<System.Guid> PRId { get; set; }
        public Nullable<bool> Published { get; set; }
        public Nullable<System.Guid> CreatedBy { get; set; }
        public string GRNNumber { get; set; }
    
        public virtual Department Department { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Employee Employee1 { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual ICollection<GRNLine> GRNLines { get; set; }
        public virtual ICollection<SignOffCertificate> SignOffCertificates { get; set; }
    }
}
