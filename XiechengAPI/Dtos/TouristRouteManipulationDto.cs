using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using XiechengAPI.ValidationAttributes;

namespace XiechengAPI.Dtos
{
    [TouristRouteTileMustBeDifferentFromDescription]
    public abstract class TouristRouteManipulationDto
    {
        [Required]
        [MaxLength]
        public string Title { get; set; }
        [Required]
        [MaxLength]
        public virtual string Description { get; set; }
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
        public ICollection<TouristRoutePictureForCreationDto> TouristRoutePictures { get; set; } = new List<TouristRoutePictureForCreationDto>();
    }
}
