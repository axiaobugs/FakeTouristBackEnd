using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using XiechengAPI.Dtos;
using XiechengAPI.Moldes;

namespace XiechengAPI.Profiles
{
    public class TouristRouteProfiles :Profile
    {
        public TouristRouteProfiles()
        {
            CreateMap<TouristRoute, TouristRouteDto>()
                .ForMember(
                    dest => dest.Price,
                    opt => opt.MapFrom(src => src.OriPrice * (decimal) (1+src.DiscountPresent ?? 1))
                    )
                .ForMember(
                    dest=>dest.TravelDays,
                    opt=>opt.MapFrom(src=>src.TravelDays.ToString()))
                .ForMember(
                            dest => dest.TravelType,
                            opt => opt.MapFrom(src => src.TravelType.ToString()))
                .ForMember(
                    dest=>dest.DepartureCity,
                    opt=>opt.MapFrom(src=>src.DepartureCity.ToString())
                    );
            CreateMap<TouristRouteCreationDto, TouristRoute>()
                .ForMember(
                   dest=>dest.Id,
                   opt=>opt.MapFrom(src=>Guid.NewGuid())
                    );
            CreateMap<TouristRouteUpdateDto, TouristRoute>();
           
            CreateMap<TouristRoute, TouristRouteUpdateDto>();
        }
    }
}
