using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    // /api/walks
    [Route( "api/[controller]" )]
    [ApiController]
    public class WalksController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IWalkRepository walkRepository;

        public WalksController(IMapper mapper, IWalkRepository walkRepository)
        {
            this.mapper = mapper;
            this.walkRepository = walkRepository;
        }



        //Create Walk 
        //POST: /api/walks
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddWalkRequestDto addWalkRequestDto)
        {


            //map DTO to Domain model
            var walkDomain = mapper.Map<Walk>( addWalkRequestDto );

            // create walk
            var createdWalk = await walkRepository.CreateAsync( walkDomain );

            //map domain model to DTO
            var walkDto = mapper.Map<WalkDto>( createdWalk );

            return Ok( walkDto );
        }

        // get all walks
        // GET: /api/walks?filterOn=Name&filterQuery=Track&sortBy=Name&isAscending=true&pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? filterOn,
            [FromQuery] string? filterQuery,
            [FromQuery] string? sortBy,
            [FromQuery] bool? isAscending,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10
            )
        {
            var walksDomainModel = await walkRepository.GetAllAsync( filterOn, filterQuery, sortBy, isAscending ?? true, pageNumber, pageSize );

            var walkDtos = mapper.Map<List<WalkDto>>( walksDomainModel );

            return Ok( walkDtos );
        }

        // get walk by id
        // GET: /api/walks/{id}
        [HttpGet( "{id}" )]
        public async Task<IActionResult> GetById(Guid id)
        {
            var walkDomainModel = await walkRepository.GetByIdAsync( id );

            if(walkDomainModel == null)
            {
                return NotFound( );
            }

            var walkDto = mapper.Map<WalkDto>( walkDomainModel );

            return Ok( walkDto );
        }

        // update walk
        // PUT: /api/walks/{id}
        [HttpPut( "{id}" )]
        [ValidateModel]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWalkRequestDto updateWalkRequestDto)
        {

            // map the updated walk to the domain model
            var walkDomainModel = mapper.Map<Walk>( updateWalkRequestDto );

            walkDomainModel = await walkRepository.UpdateAsync( id, walkDomainModel );

            if(walkDomainModel == null)
            {
                return NotFound( );
            }


            return Ok( mapper.Map<WalkDto>( walkDomainModel ) );
        }

        // delete walk
        // DELETE: /api/walks/{id}
        [HttpDelete( "{id}" )]
        public async Task<IActionResult> Delete(Guid id)
        {

            var deleteWalkDomainModel = await walkRepository.DeleteAsync( id );
            if(deleteWalkDomainModel == null)
            {
                return NotFound( );
            }

            return Ok( mapper.Map<WalkDto>( deleteWalkDomainModel ) );
        }

    }
}
