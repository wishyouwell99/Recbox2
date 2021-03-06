﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Recbox2.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class RedboxEntities : DbContext
    {
        public RedboxEntities()
            : base("name=RedboxEntities")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<KioskMovy> KioskMovies { get; set; }
        public virtual DbSet<Kiosk> Kiosks { get; set; }
        public virtual DbSet<Movy> Movies { get; set; }
        public virtual DbSet<Genre> Genres { get; set; }
        public virtual DbSet<Rating> Ratings { get; set; }
        public virtual DbSet<Rental> Rentals { get; set; }
    
        public virtual ObjectResult<spKioskMoviesForRentGet_Result> spKioskMoviesForRentGet(Nullable<int> kioskId)
        {
            var kioskIdParameter = kioskId.HasValue ?
                new ObjectParameter("kioskId", kioskId) :
                new ObjectParameter("kioskId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spKioskMoviesForRentGet_Result>("spKioskMoviesForRentGet", kioskIdParameter);
        }
    
        public virtual ObjectResult<spCustomerRentals_Result> spCustomerRentals(Nullable<int> customerId)
        {
            var customerIdParameter = customerId.HasValue ?
                new ObjectParameter("customerId", customerId) :
                new ObjectParameter("customerId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spCustomerRentals_Result>("spCustomerRentals", customerIdParameter);
        }
    }
}
