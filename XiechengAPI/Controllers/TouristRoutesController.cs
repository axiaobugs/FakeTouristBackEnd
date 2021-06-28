using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using XiechengAPI.Dtos;
using XiechengAPI.Moldes;
using XiechengAPI.ResourceParameters;
using XiechengAPI.Services;

namespace XiechengAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TouristRoutesController : ControllerBase
    {
        private readonly ITouristRepository _touristRepository;
        private readonly IMapper _mapper;

        public TouristRoutesController(
            ITouristRepository touristRepository,
            IMapper mapper)
        {
            _touristRepository = touristRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [HttpHead]
        public IActionResult GetTouristRoutes(
            [FromQuery] TouristRouteResourceParameters parameters)
        {

            var touristRouteFromRepo = _touristRepository.GetTouristRoutes(parameters.Keyword, parameters.RatingOperator, parameters.RatingValue);
            if (touristRouteFromRepo == null || !touristRouteFromRepo.Any())
            {
                return NotFound("No tourist route found");
            }

            var touristRouteDto = _mapper.Map<IEnumerable<TouristRouteDto>>(touristRouteFromRepo);
            return Ok(touristRouteDto);
        }

        // api/touristRoutes/{touristRouteId}
        [HttpGet("{touristRouteId}",Name = "GetTouristRouteById")]
        public IActionResult GetTouristRouteById(Guid touristRouteId)
        {
            var touristRouteFromRepo = _touristRepository.GetTouristRoute(touristRouteId);
            
            if (touristRouteFromRepo == null)
            {
                return NotFound("No found tourist route by query id");
            }

            var touristRouteDto = _mapper.Map<TouristRouteDto>(touristRouteFromRepo);
            return Ok(touristRouteDto);
        }

        [HttpPost]
        public IActionResult CreateTouristRoute([FromBody] TouristRouteCreationDto touristRouteCreationDto)
        {
            var touristRouteModel = _mapper.Map<TouristRoute>(touristRouteCreationDto);
            _touristRepository.AddTouristRoute(touristRouteModel);
            _touristRepository.Save();
            var touristRouteToReturn = _mapper.Map<TouristRouteDto>(touristRouteModel);
            return CreatedAtRoute(
                "GetTouristRouteById",
                new {touristRouteId = touristRouteToReturn.Id},
                touristRouteToReturn);
        }
    }
}
