using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.JsonPatch;
using XiechengAPI.Dtos;
using XiechengAPI.Helper;
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

        [HttpPut("{touristRouteId}")]
        public IActionResult UpdateTouristRoute([FromRoute] Guid touristRouteId,
            [FromBody] TouristRouteUpdateDto touristRouteUpdateDto)
        {
            if (!_touristRepository.TouristRouteExists(touristRouteId))
            {
                return NotFound("Can't found");
            }

            var touristRouteFromRepo = _touristRepository.GetTouristRoute(touristRouteId);
            // 1: map the touristRouteFromRepo to Dto
            // 2: update the dto
            // 3: map dto to touristRouteFromRepo
            _mapper.Map(touristRouteUpdateDto, touristRouteFromRepo);
            _touristRepository.Save();
            return NoContent();
        }

        [HttpPatch("{touristRouteId}")]
        public IActionResult PartiallyUpdateTouristRoute([FromRoute] Guid touristRouteId,
            [FromBody] JsonPatchDocument<TouristRouteUpdateDto> patchDocument)
        {
            if (!_touristRepository.TouristRouteExists(touristRouteId))
            {
                return NotFound("Can't found tourist route");
            }

            var touristRouteFromRepo = _touristRepository.GetTouristRoute(touristRouteId);
            var touristRouteUpdateToPatch = _mapper.Map<TouristRouteUpdateDto>(touristRouteFromRepo);
            // patch to the data from repo
            patchDocument.ApplyTo(touristRouteUpdateToPatch,ModelState);
            // validation the dto which had patched
            if (!TryValidateModel(touristRouteUpdateToPatch))
            {
                return ValidationProblem(ModelState);
            }
            // mapper patch to dto
            _mapper.Map(touristRouteUpdateToPatch, touristRouteFromRepo);
            // save data to database
            _touristRepository.Save();
            return NoContent();
        }

        [HttpDelete("{touristRouteId}")]
        public IActionResult DeleteTouristRoute([FromRoute] Guid touristRouteId)
        {
            if (!_touristRepository.TouristRouteExists(touristRouteId))
            {
                return NotFound("Can't found tourist route");
            }

            var touristRouteFromRepo = _touristRepository.GetTouristRoute(touristRouteId);
            _touristRepository.DeleteTouristRoute(touristRouteFromRepo);
            _touristRepository.Save();
            return NoContent();
        }

        [HttpDelete("({touristRouteIds})")]
        public IActionResult DeleteByIds([ModelBinder(BinderType = typeof(ArrayModelBinder))] [FromRoute]
            IEnumerable<Guid> touristRouteIds)
        {
            if (touristRouteIds == null)
            {
                return BadRequest();
            }

            var touristRoutesFromRepo = _touristRepository.GeTouristRouteByIdList(touristRouteIds);
            _touristRepository.DeleteTouristRoutes(touristRoutesFromRepo);
            _touristRepository.Save();
            return NoContent();
        }

    }
}
