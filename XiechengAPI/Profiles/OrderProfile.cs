using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using XiechengAPI.Dtos;
using XiechengAPI.Moldes;

namespace XiechengAPI.Profiles
{
    public class OrderProfile:Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderDto>().ForMember(
                dest=>dest.State,
                opt =>
                {
                    opt.MapFrom(src=>src.State.ToString());
                }
            );
        }

    }
}
