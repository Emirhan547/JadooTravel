using JadooTravel.DataAccess.Abstract;
using JadooTravel.DataAccess.Concrete;
using JadooTravel.DataAccess.Context;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.DataAccess.Extensions
{
    public static class RepositoryRegistrations
    {
        public static void AddRepositoryExtensions(this IServiceCollection services)
        {
            services.AddScoped<IBookingDal, MongoBookingDal>();

            services.AddScoped<ICategoryDal, MongoCategoryDal>();
            services.AddScoped<IDestinationDal, MongoDestinationDal>();
            services.AddScoped<IFeatureDal, MongoFeatureDal>();
            services.AddScoped<ITestimonialDal, MongoTestimonialDal>();
            services.AddScoped<ITripPlanDal, MongoTripPlanDal>();
            services.AddSingleton<AppDbContext>();
            services.AddScoped(typeof(IGenericDal<>), typeof(MongoGenericDal<>));
        }
    }
}