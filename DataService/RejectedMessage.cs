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
    
    public partial class RejectedMessage
    {
        public System.Guid Id { get; set; }
        public long EvalCycleId { get; set; }
        public Nullable<System.Guid> ManagerID { get; set; }
        public Nullable<System.Guid> EmployeeId { get; set; }
        public string RejectionReason { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string EvaluationPhase { get; set; }
        public Nullable<System.Guid> GoalId { get; set; }
    }
}
