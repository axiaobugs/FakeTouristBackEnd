using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using XiechengAPI.Dtos;
using XiechengAPI.Moldes;

namespace XiechengAPI.Profiles
{
    public class ShoppingCartProfiles : Profile
    {

        public ShoppingCartProfiles()
        {
            CreateMap<ShoppingCart, ShoppingCartDto>();
            CreateMap<LineItem, LineItemDto>();
        }



    }
}
