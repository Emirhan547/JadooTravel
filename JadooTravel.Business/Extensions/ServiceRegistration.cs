using JadooTravel.Business.Abstract;
using JadooTravel.Business.Concrete;

using Microsoft.Extensions.DependencyInjection;


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
            services.AddScoped<IUserProfileService, UserProfileService>();
            services.AddScoped<ITripPlanService, TripPlanManager>();
            services.AddScoped<ICtaService, CtaManager>();
            services.AddScoped<IPartnerService, PartnerManager>();
           

        }
    }
}
