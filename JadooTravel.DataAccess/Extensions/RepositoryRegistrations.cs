using JadooTravel.DataAccess.Abstract;
using JadooTravel.DataAccess.EntityFramework;
using JadooTravel.DataAccess.Repositories;
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
            services.AddScoped<IBookingDal, EfBookingDal>();

            services.AddScoped<ICategoryDal, EfCategoryDal>();
            services.AddScoped<IDestinationDal, EfDestinationDal>();
            services.AddScoped<IFeatureDal, EfFeatureDal>();
            services.AddScoped<ITestimonialDal, EfTestimonialDal>();
            services.AddScoped<ITripPlanDal, EfTripPlanDal>();
            services.AddScoped(typeof(IGenericDal<>), typeof(GenericRepository<>));
        }
    }
}