using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using XiechengAPI.Dtos;
using XiechengAPI.Migrations;
using XiechengAPI.Moldes;
using XiechengAPI.Services;

namespace XiechengAPI.Controllers
{
    [Route("api/touristRoutes/{touristRouteId}/pictures")]
    [ApiController]
    public class TouristRoutePicturesController:ControllerBase
    {

        private readonly ITouristRepository _touristRepository;
        private readonly IMapper _mapper;

        public TouristRoutePicturesController(
            ITouristRepository touristRepository,
            IMapper mapper)
        {
            _touristRepository = touristRepository ??
                throw new ArgumentNullException(nameof(TouristRouteRepository));
            _mapper = mapper ?? 
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<IActionResult> GetPictureListForTouristRouteAsync(Guid touristRouteId)
        {
        
            if (!(await _touristRepository.TouristRouteExistsAsync(touristRouteId)))
            {
                return NotFound("not found this route in our database");
            }

            var pictureFromRepo =await _touristRepository.GetPictureByTouristRouteIdAsync(touristRouteId);
            if (pictureFromRepo == null || pictureFromRepo.Count()<=0)
            {
                return NotFound("picture not exists");
            }

            return Ok(_mapper.Map<IEnumerable<TouristRoutePictureDto>>(pictureFromRepo));
        }

        [HttpGet("{pictureId}",Name = "GetPicture")]
        public async Task<IActionResult> GetPictureAsync(Guid touristRouteId,int pictureId)
        {
            if (!(await _touristRepository.TouristRouteExistsAsync(touristRouteId)))
            {
                return NotFound("not found this route in our database");
            }
            var pictureFromRepo =await _touristRepository.GetPictureAsync(pictureId);

            if (pictureFromRepo == null)
            {
                return NotFound("picture not exists");
            }
            return Ok(_mapper.Map<TouristRoutePictureDto>(pictureFromRepo));
        }

        [HttpPost]
        public async Task<IActionResult> CreateTouristRoutePicture(
            [FromRoute] Guid touristRouteId,
            [FromBody] TouristRoutePictureForCreationDto touristRoutePictureForCreationDto)
        {
            if (!(await _touristRepository.TouristRouteExistsAsync(touristRouteId)))
            {
                return NotFound("not found this route in our database");
            }

            var pictureModel = _mapper.Map<TouristRoutePicture>(touristRoutePictureForCreationDto);
            _touristRepository.AddTouristRoutePicture(touristRouteId,pictureModel);
            await _touristRepository.SaveAsync();
            var pictureToReturn = _mapper.Map<TouristRoutePictureDto>(pictureModel);
            return CreatedAtRoute(
                "GetPicture",
                new
                {
                    touristRouteId = pictureModel.TouristRouteId,
                    pictureId = pictureModel.Id
                },
                pictureToReturn);
        }

        [HttpDelete("{pictureId}")]
        public async Task<IActionResult> DeleteTouristRoutePictureAsync([FromRoute] Guid touristRouteId,
            [FromRoute] int pictureId)
        {
            if (!(await _touristRepository.TouristRouteExistsAsync(touristRouteId)))
            {
                return NotFound("not found this route in our database");
            }

            var touristRoutePictureFromRepo =await _touristRepository.GetPictureAsync(pictureId);
            _touristRepository.DeleteTouristRoutePicture(touristRoutePictureFromRepo);
            await _touristRepository.SaveAsync();
            return NoContent();
        }


    }
}
