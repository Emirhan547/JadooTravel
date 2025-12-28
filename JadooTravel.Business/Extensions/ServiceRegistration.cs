using JadooTravel.Business.Abstract;
using JadooTravel.Business.Concrete;
using JadooTravel.DataAccess.Abstract;
using JadooTravel.DataAccess.EntityFramework;
using JadooTravel.DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Business.Extensions
{
    public static class ServiceRegistration
    {
        public static void AddServiceExtensions(this IServiceCollection services)
        {
            services.AddScoped<IBookingService, BookingManager>();

            services.AddScoped<ICategoryService, CategoryManager>();

            services.AddScoped<IDestinationService, DestinationManager>();

            services.AddScoped<IFeatureService, FeatureManager>();

            services.AddScoped<ITestimonialService, TestimonialManager>();

            services.AddScoped<ITripPlanService, TripPlanManager>();


        }
    }
}
