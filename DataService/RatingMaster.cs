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
    
    public partial class RatingMaster
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Description { get; set; }
        public int OrgId { get; set; }
    
        public virtual Organization Organization { get; set; }
    }
}
