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
    
    public partial class FeedbackAnswer
    {
        public System.Guid AnswerId { get; set; }
        public long QuestionId { get; set; }
        public string Answer { get; set; }
        public System.Guid EmployeeId { get; set; }
        public long EvalCycleId { get; set; }
    }
}
