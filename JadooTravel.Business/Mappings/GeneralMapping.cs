using AutoMapper;
using JadooTravel.Dto.Dtos.BookingDtos;
using JadooTravel.Dto.Dtos.CategoryDtos;
using JadooTravel.Dto.Dtos.ChatDtos;
using JadooTravel.Dto.Dtos.DestinationDtos;
using JadooTravel.Dto.Dtos.FeatureDtos;
using JadooTravel.Dto.Dtos.ReviewDtos;
using JadooTravel.Dto.Dtos.TestimonialDtos;
using JadooTravel.Dto.Dtos.TripPlanDtos;
using JadooTravel.Dto.Dtos.UserDtos;
using JadooTravel.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Business.Mappings
{
    public class GeneralMapping:Profile
    {
        public GeneralMapping()
        {
            CreateMap<ResultBookingDto,Booking>().ReverseMap();
            CreateMap<CreateBookingDto,Booking>().ReverseMap();
            CreateMap<UpdateBookingDto,Booking>().ReverseMap();
            CreateMap<Booking, UserBookingDto>();
            CreateMap<ResultCategoryDto, Category>().ReverseMap();
            CreateMap<CreateCategoryDto, Category>().ReverseMap();
            CreateMap<UpdateCategoryDto, Category>().ReverseMap();

            CreateMap<ResultDestinationDto, Destination>().ReverseMap();
            CreateMap<CreateDestinationDto, Destination>().ReverseMap();
            CreateMap<UpdateDestinationDto, Destination>().ReverseMap();

            CreateMap<ResultFeatureDto, Feature>().ReverseMap();
            CreateMap<CreateFeatureDto, Feature>().ReverseMap();
            CreateMap<UpdateFeatureDto, Feature>().ReverseMap();

            CreateMap<ResultTestimonialDto, Testimonial>().ReverseMap();
            CreateMap<CreateTestimonialDto, Testimonial>().ReverseMap();
            CreateMap<UpdateTestimonialDto, Testimonial>().ReverseMap();

            CreateMap<ResultTripPlanDto, TripPlan>().ReverseMap();
            CreateMap<CreateTripPlanDto, TripPlan>().ReverseMap();
            CreateMap<UpdateTripPlanDto, TripPlan>().ReverseMap();
            CreateMap<PageViewTrack, dynamic>().ReverseMap();
            CreateMap<ConversionTrack, dynamic>().ReverseMap();
            CreateMap<UserFavorite, UserFavoriteDto>().ReverseMap();

            CreateMap<DestinationReview, ResultReviewDto>().ReverseMap();
            CreateMap<DestinationReview, UserReviewDto>().ReverseMap();
            CreateMap<ChatMessage, ChatMessageResponseDto>().ReverseMap();
            CreateMap<ChatMessage, ChatHistoryDto>().ReverseMap();
            CreateMap<FAQ, FAQDto>().ReverseMap();
         
        }
    }
}
