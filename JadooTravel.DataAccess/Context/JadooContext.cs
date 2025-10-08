using JadooTravel.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.DataAccess.Context
{
    public class JadooContext : DbContext
    {
        public JadooContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Category> Categorys { get; set; }
        public DbSet<Destination> Destinations { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<Testimonial> Testimonials { get; set; }
        public DbSet<TripPlan> TripPlans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<Booking>().ToCollection("Bookings");
            //modelBuilder.Entity<Category>().ToCollection("Categorys");
            //modelBuilder.Entity<Destination>().ToCollection("Destinations");
            //modelBuilder.Entity<Feature>().ToCollection("Features");
            //modelBuilder.Entity<Product>().ToCollection("Products");
            //modelBuilder.Entity<Testimonial>().ToCollection("Testimonials");
            //modelBuilder.Entity<TripPlan>().ToCollection("TripPlans");


        }
    }

    }
    

