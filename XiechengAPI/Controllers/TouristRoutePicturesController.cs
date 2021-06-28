using System;
using System.Collections.Generic;
using System.Linq;
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
        public IActionResult GetPictureListForTouristRoute(Guid touristRouteId)
        {
            if (!_touristRepository.TouristRouteExists(touristRouteId))
            {
                return NotFound("not found this route in our database");
            }

            var pictureFromRepo = _touristRepository.GetPictureByTouristRouteId(touristRouteId);
            if (pictureFromRepo == null || pictureFromRepo.Count()<=0)
            {
                return NotFound("picture not exists");
            }

            return Ok(_mapper.Map<IEnumerable<TouristRoutePictureDto>>(pictureFromRepo));
        }

        [HttpGet("{pictureId}",Name = "GetPicture")]
        public IActionResult GetPicture(Guid touristRouteId,int pictureId)
        {
            if (!_touristRepository.TouristRouteExists(touristRouteId))
            {
                return NotFound("not found this route in our database");
            }
            var pictureFromRepo = _touristRepository.GetPicture(pictureId);

            if (pictureFromRepo == null)
            {
                return NotFound("picture not exists");
            }
            return Ok(_mapper.Map<TouristRoutePictureDto>(pictureFromRepo));
        }

        [HttpPost]
        public IActionResult CreateTouristRoutePicture(
            [FromRoute] Guid touristRouteId,
            [FromBody] TouristRoutePictureForCreationDto touristRoutePictureForCreationDto)
        {
            if (!_touristRepository.TouristRouteExists(touristRouteId))
            {
                return NotFound("not found this route in our database");
            }

            var pictureModel = _mapper.Map<TouristRoutePicture>(touristRoutePictureForCreationDto);
            _touristRepository.AddTouristRoutePicture(touristRouteId,pictureModel);
            _touristRepository.Save();
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


    }
}
