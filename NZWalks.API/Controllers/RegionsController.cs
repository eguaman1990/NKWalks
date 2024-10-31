using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route( "api/[controller]" )]
    [ApiController]
    [Authorize]
    public class RegionsController : ControllerBase
    {

        private readonly IRegionRepository regionRepository;
        private readonly IMapper mapper;
        public RegionsController(IRegionRepository regionRepository, IMapper mapper)
        {

            this.regionRepository = regionRepository;
            this.mapper = mapper;
        }

        // GET ALL REGIONS
        // GET: https://localhost:portnumber/api/regions
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {

            // Get Data from database - domain model
            var regionsDomain = await this.regionRepository.GetAllAsync( );

            //map domain model to DTO
            //var regionsDto = new List<RegionDto>();
            //foreach (var regionDomain in regionsDomain)
            //{
            //    regionsDto.Add(new RegionDto
            //    {
            //        Id = regionDomain.Id,
            //        Name = regionDomain.Name,
            //        Code = regionDomain.Code,
            //        RegionImageUrl = regionDomain.RegionImageUrl
            //    });
            //}

            //using automapper
            var regionsDto = this.mapper.Map<List<RegionDto>>( regionsDomain );

            // return DTOs
            return Ok( regionsDto );
        }

        // GET REGION BY ID
        // GET: https://localhost:portnumber/api/regions/{id}
        [HttpGet]
        [Route( "{id:Guid}" )]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var regionDomain = await this.regionRepository.GetByIdAsync( id );

            if(regionDomain == null)
            {
                return NotFound( );
            }

            //var regionDto = new RegionDto
            //{
            //    Id = region.Id,
            //    Name = region.Name,
            //    Code = region.Code,
            //    RegionImageUrl = region.RegionImageUrl
            //};

            //using automapper
            var regionDto = this.mapper.Map<RegionDto>( regionDomain );

            return Ok( regionDto );
        }

        //POST TO Create a new Region
        //POST: https://localhost:portnumber/api/regions
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto)
        {

            //var regionDomainModel = new Region
            //{
            //    Name = addRegionRequestDto.Name,
            //    Code = addRegionRequestDto.Code,
            //    RegionImageUrl = addRegionRequestDto.RegionImageUrl
            //};

            //using automapper
            var regionDomainModel = this.mapper.Map<Region>( addRegionRequestDto );

            //use domain model to create region
            regionDomainModel = await this.regionRepository.CreateAsync( regionDomainModel );

            //map domain model back to DTO
            //var regionDto = new RegionDto
            //{
            //    Id = regionDomainModel.Id,
            //    Name = regionDomainModel.Name,
            //    Code = regionDomainModel.Code,
            //    RegionImageUrl = regionDomainModel.RegionImageUrl
            //};

            //using automapper
            var regionDto = this.mapper.Map<RegionDto>( regionDomainModel );

            return CreatedAtAction( nameof( GetById ), new { id = regionDomainModel.Id }, regionDto );

        }

        //PUT TO Update a Region
        //PUT: https://localhost:portnumber/api/regions/{id}
        [HttpPut]
        [Route( "{id:Guid}" )]
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDTO updateRegionRequestDto)
        {
            // map dto to domain model
            //var regionDomainModel = new Region
            //{
            //    Id = id,
            //    Name = updateRegionRequestDto.Name,
            //    Code = updateRegionRequestDto.Code,
            //    RegionImageUrl = updateRegionRequestDto.RegionImageUrl
            //};

            //using automapper
            var regionDomainModel = this.mapper.Map<Region>( updateRegionRequestDto );


            //use domain model to update region
            regionDomainModel = await this.regionRepository.UpdateAsync( regionDomainModel );

            if(regionDomainModel == null)
            {
                return NotFound( );
            }

            // Convert domain model back to DTO
            //var regionDto = new RegionDto
            //{
            //    Id = regionDomainModel.Id,
            //    Name = regionDomainModel.Name,
            //    Code = regionDomainModel.Code,
            //    RegionImageUrl = regionDomainModel.RegionImageUrl
            //};

            //using automapper
            var regionDto = this.mapper.Map<RegionDto>( regionDomainModel );

            return Ok( regionDto );
        }

        //DELETE TO Delete a Region
        //DELETE: https://localhost:portnumber/api/regions/{id}
        [HttpDelete]
        [Route( "{id:Guid}" )]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var regionDomainModel = await this.regionRepository.DeleteAsync( id );

            if(regionDomainModel == null)
            {
                return NotFound( );
            }

            var regionDto = new RegionDto
            {
                Id = regionDomainModel.Id,
                Name = regionDomainModel.Name,
                Code = regionDomainModel.Code,
                RegionImageUrl = regionDomainModel.RegionImageUrl
            };

            return Ok( regionDto );
        }
    }
}
