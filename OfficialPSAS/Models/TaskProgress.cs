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
    
    public partial class TaskProgress
    {
        public int progress_id { get; set; }
        public Nullable<int> status { get; set; }
        public string Comments { get; set; }
    
        public virtual GroupMember GroupMember { get; set; }
        public virtual Task Task { get; set; }
    }
}
