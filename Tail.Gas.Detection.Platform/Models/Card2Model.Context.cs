﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Tail.Gas.Detection.Platform.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class cardb2Entities1 : DbContext
    {
        public cardb2Entities1()
            : base("name=cardb2Entities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<CarInfo> CarInfo { get; set; }
        public DbSet<CarNoBlong> CarNoBlong { get; set; }
        public DbSet<CarNoProvince> CarNoProvince { get; set; }
        public DbSet<CarStatusInfo> CarStatusInfo { get; set; }
        public DbSet<CarType> CarType { get; set; }
        public DbSet<InputCarStatusInfo> InputCarStatusInfo { get; set; }
        public DbSet<PageInfo> PageInfo { get; set; }
        public DbSet<RoleInfo> RoleInfo { get; set; }
        public DbSet<UserInfo> UserInfo { get; set; }
        public DbSet<UserRoleInfo> UserRoleInfo { get; set; }
    }
}
