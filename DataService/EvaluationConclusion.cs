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
    
    public partial class EvaluationConclusion
    {
        public System.Guid gid { get; set; }
        public System.Guid employeeid { get; set; }
        public long evalcycleid { get; set; }
        public string training { get; set; }
        public string meetingsummary { get; set; }
        public Nullable<System.DateTime> modifiedon { get; set; }
    }
}
