using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using XiechengAPI.Dtos;

namespace XiechengAPI.ValidationAttributes
{
    public class TouristRouteTileMustBeDifferentFromDescriptionAttribute:ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var touristRouteDto = (TouristRouteCreationDto)validationContext.ObjectInstance;
            if (touristRouteDto.Title == touristRouteDto.Description)
            {
                return new ValidationResult(
                    "Title must be different with Description",
                    new[] { "TouristRouteCreationDto" });
            }
            return ValidationResult.Success;
        }
        
    }
}
