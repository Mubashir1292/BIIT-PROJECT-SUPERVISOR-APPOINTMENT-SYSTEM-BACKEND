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
    
    public partial class GroupMember
    {
        public string st_id { get; set; }
    
        public virtual group group { get; set; }
        public virtual Student Student { get; set; }
        public virtual Technology Technology { get; set; }
    }
}