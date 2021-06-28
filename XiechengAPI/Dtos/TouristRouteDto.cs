﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XiechengAPI.Moldes;

namespace XiechengAPI.Dtos
{
    public class TouristRouteDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public DateTime? DepartureTime { get; set; }
        public string Features { get; set; }
        public string Fees { get; set; }
        public string Note { get; set; }

        public double? Rating { get; set; }
       
        public string TravelDays { get; set; }
        public string TravelType { get; set; }
        public string DepartureCity { get; set; }

        public IEnumerable<TouristRoutePictureDto> TouristRoutePictures { get; set; }
    }
}
