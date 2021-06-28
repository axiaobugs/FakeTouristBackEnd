using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using XiechengAPI.Dtos;
using XiechengAPI.Moldes;

namespace XiechengAPI.Profiles
{
    public class TouristRoutePictureProfiles:Profile
    {

        public TouristRoutePictureProfiles()
        {
            CreateMap<TouristRoutePicture, TouristRoutePictureDto>();
            CreateMap<TouristRoutePictureForCreationDto, TouristRoutePicture>();
        }

    }
}
