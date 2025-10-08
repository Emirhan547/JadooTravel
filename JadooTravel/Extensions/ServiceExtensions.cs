

using JadooTravel.Business.Abstract;
using JadooTravel.Business.Concrete;
using JadooTravel.DataAccess.Abstract;
using JadooTravel.DataAccess.EntityFramework;
using JadooTravel.DataAccess.Repositories;

namespace JadooTravel.UI.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddServiceExtensions(this IServiceCollection services)
        {
            services.AddScoped<IBookingDal, EfBookingDal>();
            services.AddScoped<IBookingService, BookingManager>();
            
            services.AddScoped<ICategoryDal, EfCategoryDal>();
            services.AddScoped<ICategoryService, CategoryManager>();

            services.AddScoped<IDestinationDal, EfDestinationDal>();
            services.AddScoped<IDestinationService, DestinationManager>();

            services.AddScoped<IFeatureDal, EfFeatureDal>();
            services.AddScoped<IFeatureService, FeatureManager>();

            services.AddScoped<ITestimonialDal, EfTestimonialDal>();
            services.AddScoped<ITestimonialService, TestimonialManager>();

            services.AddScoped<ITripPlanDal, EfTripPlanDal>();
            services.AddScoped<ITripPlanService, TripPlanManager>();

            services.AddScoped(typeof(IGenericDal<>), typeof(GenericRepository<>));
            services.AddScoped(typeof(IGenericService<>), typeof(GenericManager<>));


        }
    }
}
