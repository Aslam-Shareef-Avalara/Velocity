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
    
    public partial class DefinitionIdentityTable
    {
        public long SurrogateIdentityId { get; set; }
        public System.Guid DefinitionIdentityHash { get; set; }
        public System.Guid DefinitionIdentityAnyRevisionHash { get; set; }
        public string Name { get; set; }
        public string Package { get; set; }
        public Nullable<long> Build { get; set; }
        public Nullable<long> Major { get; set; }
        public Nullable<long> Minor { get; set; }
        public Nullable<long> Revision { get; set; }
    }
}
