﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class OfficialSASEntities41 : DbContext
    {
        public OfficialSASEntities41()
            : base("name=OfficialSASEntities41")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AppointmentRequests> AppointmentRequests { get; set; }
        public virtual DbSet<group> group { get; set; }
        public virtual DbSet<GroupMember> GroupMember { get; set; }
        public virtual DbSet<groupRequests> groupRequests { get; set; }
        public virtual DbSet<Meeting> Meeting { get; set; }
        public virtual DbSet<Project> Project { get; set; }
        public virtual DbSet<projectDomain> projectDomain { get; set; }
        public virtual DbSet<projectRequests> projectRequests { get; set; }
        public virtual DbSet<Schedule> Schedule { get; set; }
        public virtual DbSet<Student> Student { get; set; }
        public virtual DbSet<SupervisorGroupConnection> SupervisorGroupConnection { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<Task> Task { get; set; }
        public virtual DbSet<TaskProgress> TaskProgress { get; set; }
        public virtual DbSet<teacher> teacher { get; set; }
        public virtual DbSet<TechnicalExpertTechnology> TechnicalExpertTechnology { get; set; }
        public virtual DbSet<Technology> Technology { get; set; }
        public virtual DbSet<TechnologyExpert> TechnologyExpert { get; set; }
        public virtual DbSet<TimeSlots> TimeSlots { get; set; }
        public virtual DbSet<users> users { get; set; }
    }
}
