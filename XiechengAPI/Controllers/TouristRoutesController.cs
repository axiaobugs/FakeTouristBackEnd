using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
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
        private readonly IUrlHelper _urlHelper;

        public TouristRoutesController(
            ITouristRepository touristRepository,
            IMapper mapper, IUrlHelperFactory urlHelperFactory,
            IActionContextAccessor actionContextAccessor)
        {
            _touristRepository = touristRepository;
            _mapper = mapper;
            _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
        }

        private string GenerateTouristRouteResourceURL(
            TouristRouteResourceParameters parameters,
            PaginationResourceParameters parameters2,
            ResourceUriType type)
        {
            return type switch
            {
                ResourceUriType.PreviousPage => _urlHelper.Link("GetTouristRoutesAsync",
                    new
                    {
                        keyword = parameters.Keyword,
                        rating = parameters.Rating,
                        PageNumber = parameters2.PageNumber - 1,
                        PageSize = parameters2.PageSize
                    }),
                ResourceUriType.NextPage => _urlHelper.Link("GetTouristRoutesAsync",
                    new
                    {
                        keyword = parameters.Keyword,
                        rating = parameters.Rating,
                        pageNumber = parameters2.PageNumber + 1,
                        pageSize = parameters2.PageSize
                    }),
                _ => _urlHelper.Link("GetTouristRoutesAsync",new
                {
                    keyword = parameters.Keyword,
                    rating = parameters.Rating,
                    pageNumber = parameters2.PageNumber,
                    pageSize = parameters2.PageSize
                })
            };
        }

        [HttpGet(Name = "GetTouristRoutesAsync")]
        [HttpHead]
        public async Task<IActionResult> GetTouristRoutesAsync(
            [FromQuery] TouristRouteResourceParameters parameters,
            [FromQuery] PaginationResourceParameters parameters2)
        {

            var touristRouteFromRepo =await _touristRepository
                .GetTouristRoutesAsync(
                    parameters.Keyword, 
                    parameters.RatingOperator, 
                    parameters.RatingValue,
                    parameters2.PageSize,
                    parameters2.PageNumber);
            if (touristRouteFromRepo == null || !touristRouteFromRepo.Any())
            {
                return NotFound("No tourist route found");
            }

            var touristRouteDto = _mapper.Map<IEnumerable<TouristRouteDto>>(touristRouteFromRepo);

            var previousPageLink = touristRouteFromRepo.HasPrevious
                ? GenerateTouristRouteResourceURL(parameters, parameters2, ResourceUriType.PreviousPage)
                : null;

            var nextPageLink = touristRouteFromRepo.HasNext
                ? GenerateTouristRouteResourceURL(parameters, parameters2, ResourceUriType.NextPage)
                : null;

            // x-pagination
            var paginationMetedata = new
            {
                previousPageLink,
                nextPageLink,
                totalCount = touristRouteFromRepo.TotalCount,
                pageSize = touristRouteFromRepo.PageSize,
                currentPage = touristRouteFromRepo.CurrentPage,
                totalPages = touristRouteFromRepo.TotalPages
            };
            // add to Header
            Response.Headers.Add("x-pagination",Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetedata));

            return Ok(touristRouteDto);
        }

        // api/touristRoutes/{touristRouteId}
        [HttpGet("{touristRouteId}",Name = "GetTouristRouteById")]
        public async Task<IActionResult> GetTouristRouteByIdAsync(Guid touristRouteId)
        {
            var touristRouteFromRepo =await _touristRepository.GetTouristRouteAsync(touristRouteId);
            
            if (touristRouteFromRepo == null)
            {
                return NotFound("No found tourist route by query id");
            }

            var touristRouteDto = _mapper.Map<TouristRouteDto>(touristRouteFromRepo);
            return Ok(touristRouteDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTouristRouteAsync([FromBody] TouristRouteCreationDto touristRouteCreationDto)
        {
            var touristRouteModel = _mapper.Map<TouristRoute>(touristRouteCreationDto);
            _touristRepository.AddTouristRoute(touristRouteModel);
            await _touristRepository.SaveAsync();
            var touristRouteToReturn = _mapper.Map<TouristRouteDto>(touristRouteModel);
            return CreatedAtRoute(
                "GetTouristRouteById",
                new {touristRouteId = touristRouteToReturn.Id},
                touristRouteToReturn);
        }

        [HttpPut("{touristRouteId}")]
        public async Task<IActionResult> UpdateTouristRouteAsync([FromRoute] Guid touristRouteId,
            [FromBody] TouristRouteUpdateDto touristRouteUpdateDto)
        {
            if (!(await _touristRepository.TouristRouteExistsAsync(touristRouteId)))
            {
                return NotFound("Can't found");
            }

            var touristRouteFromRepo =await _touristRepository.GetTouristRouteAsync(touristRouteId);
            // 1: map the touristRouteFromRepo to Dto
            // 2: update the dto
            // 3: map dto to touristRouteFromRepo
            _mapper.Map(touristRouteUpdateDto, touristRouteFromRepo);
            await _touristRepository.SaveAsync();
            return NoContent();
        }

        [HttpPatch("{touristRouteId}")]
        public async Task<IActionResult> PartiallyUpdateTouristRouteAsync([FromRoute] Guid touristRouteId,
            [FromBody] JsonPatchDocument<TouristRouteUpdateDto> patchDocument)
        {
            if (!(await _touristRepository.TouristRouteExistsAsync(touristRouteId)))
            {
                return NotFound("Can't found tourist route");
            }

            var touristRouteFromRepo =await _touristRepository.GetTouristRouteAsync(touristRouteId);
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
            await _touristRepository.SaveAsync();
            return NoContent();
        }

        [HttpDelete("{touristRouteId}")]
        public async Task<IActionResult> DeleteTouristRouteAsync([FromRoute] Guid touristRouteId)
        {
            if (!(await _touristRepository.TouristRouteExistsAsync(touristRouteId)))
            {
                return NotFound("Can't found tourist route");
            }

            var touristRouteFromRepo =await _touristRepository.GetTouristRouteAsync(touristRouteId);
            _touristRepository.DeleteTouristRoute(touristRouteFromRepo);
            await _touristRepository.SaveAsync();
            return NoContent();
        }

        [HttpDelete("({touristRouteIds})")]
        public async Task<IActionResult> DeleteByIdsAsync([ModelBinder(BinderType = typeof(ArrayModelBinder))] [FromRoute]
            IEnumerable<Guid> touristRouteIds)
        {
            
            if (touristRouteIds == null)
            {
                return BadRequest();
            }
            
            var touristRoutesFromRepo =await _touristRepository.GeTouristRouteByIdListAsync(touristRouteIds);
            _touristRepository.DeleteTouristRoutes(touristRoutesFromRepo);
            await _touristRepository.SaveAsync();
            return NoContent();

        }

    }
}
