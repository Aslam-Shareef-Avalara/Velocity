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
    
    public partial class Employee
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Employee()
        {
            this.EmployeeEducations = new HashSet<EmployeeEducation>();
            this.EmployeeSkills = new HashSet<EmployeeSkill>();
            this.EmploymentHistories = new HashSet<EmploymentHistory>();
        }
    
        public System.Guid gid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Department { get; set; }
        public Nullable<System.Guid> Manager { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int OrgId { get; set; }
        public bool Active { get; set; }
        public Nullable<System.Guid> UserId { get; set; }
        public string Designation { get; set; }
        public Nullable<System.DateTime> DoJ { get; set; }
        public byte[] ProfilePix { get; set; }
        public string OrgEmpId { get; set; }
        public Nullable<System.Guid> LocationId { get; set; }
        public string Mobile { get; set; }
        public Nullable<bool> FirstTime { get; set; }
        public Nullable<System.DateTime> DoB { get; set; }
        public string Gender { get; set; }
        public string EmpType { get; set; }
        public Nullable<System.DateTime> LastVisit { get; set; }
        public Nullable<System.Guid> Reviewer { get; set; }
        public Nullable<int> OrgLocationId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeEducation> EmployeeEducations { get; set; }
        public virtual Organization Organization { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeSkill> EmployeeSkills { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmploymentHistory> EmploymentHistories { get; set; }
    }
}
