using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using XiechengAPI.ValidationAttributes;

namespace XiechengAPI.Dtos
{
    
    public class TouristRouteUpdateDto : TouristRouteManipulationDto
    {

        [Required(ErrorMessage = "Update must have this")]
        [MaxLength]
        public override string Description { get; set; }

    }
}
