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
    
    public partial class BulkNotificationLog
    {
        public long Id { get; set; }
        public string ReminderType { get; set; }
        public System.DateTime SentOn { get; set; }
        public long NumberOfNotifications { get; set; }
    }
}
