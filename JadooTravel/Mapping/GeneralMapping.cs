using AutoMapper;
using JadooTravel.Dto.Dtos.BookingDtos;
using JadooTravel.Dto.Dtos.CategoryDtos;
using JadooTravel.Dto.Dtos.DestinationDtos;
using JadooTravel.Dto.Dtos.FeatureDtos;
using JadooTravel.Dto.Dtos.TestimonialDtos;
using JadooTravel.Dto.Dtos.TripPlanDtos;

using JadooTravel.Entity.Entities;

namespace JadooTravel.Mapping
{
    public class GeneralMapping : Profile
    {
        public GeneralMapping()
        {
            CreateMap<Category, ResultCategoryDto>().ReverseMap();
            CreateMap<Category, CreateCategoryDto>().ReverseMap();
            CreateMap<Category, UpdateCategoryDto>().ReverseMap();
           

            CreateMap<Destination, ResultDestinationDto>().ReverseMap();
            CreateMap<Destination, CreateDestinationDto>().ReverseMap();
            CreateMap<Destination, UpdateDestinationDto>().ReverseMap();

            CreateMap<Booking, ResultBookingDto>().ReverseMap();
            CreateMap<Booking, CreateBookingDto>().ReverseMap();
            CreateMap<Booking, UpdateBookingDto>().ReverseMap();

            CreateMap<Feature, ResultFeatureDto>().ReverseMap();
            CreateMap<Feature, CreateFeatureDto>().ReverseMap();
            CreateMap<Feature, UpdateFeatureDto>().ReverseMap();

            CreateMap<Testimonial, ResultTestimonialDto>().ReverseMap();
            CreateMap<Testimonial, CreateTestimonialDto>().ReverseMap();
            CreateMap<Testimonial, UpdateTestimonialDto>().ReverseMap();

            CreateMap<TripPlan, ResultTripPlanDto>().ReverseMap();
            CreateMap<TripPlan, CreateTripPlanDto>().ReverseMap();
            CreateMap<TripPlan, UpdateTripPlanDto>().ReverseMap();


        }
    }
}
