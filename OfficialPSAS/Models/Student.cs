//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OfficialPSAS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Student
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Student()
        {
            this.AppointmentRequests = new HashSet<AppointmentRequests>();
        }
    
        public string st_id { get; set; }
        public string semester { get; set; }
        public string section { get; set; }
        public Nullable<double> cgpa { get; set; }
        public string Grade { get; set; }
        public string image { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AppointmentRequests> AppointmentRequests { get; set; }
        public virtual GroupMember GroupMember { get; set; }
        public virtual users users { get; set; }
    }
}
