using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CountryInfo.API.Services;
using AutoMapper;
using System.Collections.Generic;
using CountryInfo.API.Models;
using System;
using CountryInfo.API.Helpers;
using CountryInfo.API.Entities;

namespace CountryInfo.API.Controllers
{
    [Route("api/Countries")]
    public class CountriesController : Controller
    {
        private readonly ILogger<CountriesController> _logger;
        private readonly ICountryInfoRepository _repository;
        private IUrlHelper _urlHelper;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;

        public CountriesController(ILogger<CountriesController> logger, 
            ICountryInfoRepository repository,
            IUrlHelper urlHelper,
            IPropertyMappingService propertyMappingService,
            ITypeHelperService typeHelperService)
        {
            _logger = logger;
            _repository = repository;
            _urlHelper = urlHelper;
            _propertyMappingService = propertyMappingService;
            _typeHelperService = typeHelperService;
        }

        // Filtering: api/countries?continent=asia
        // Searching: api/countries?searchquery=me
        // Combination: api/countries?continent=asia&searchquery=ja
        // Paging: api/countries?pagesize=5
        // Sorting: api/countries?orderBy=name desc
        // Sorting: api/countries?orderBy=name desc,continent
        // Shaping: api/countries?fieilds=name,continent
        // Shaping with paging: countries?pagesize=5&pagenumber=2&fields=name,continent
        /*
         * Not implemented yet:
         * Expand child resources: api/countries?expand=postalcodes
         * Expanded resources: api/countries?field=name,continent,postalcodes.city
         */
        [HttpGet(Name = "GetCountries")]
        public IActionResult GetCountries(CountriesResourceParameters countriesResourceParameters)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<CountryDto, Country>
                (countriesResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            if (!_typeHelperService.TypeHasProperties<CountryDto>
                  (countriesResourceParameters.Fields))
            {
                return BadRequest();
            }

            var countriesFromRepo = _repository.GetCountries(countriesResourceParameters);

            var previousPageLink = countriesFromRepo.HasPrevious ?
               CreateCountriesResourceUri(countriesResourceParameters,
               ResourceUriType.PreviousPage) : null;

            var nextPageLink = countriesFromRepo.HasNext ?
                CreateCountriesResourceUri(countriesResourceParameters,
                ResourceUriType.NextPage) : null;

            var paginationMetadata = new
            {
                totalCount = countriesFromRepo.TotalCount,
                pageSize = countriesFromRepo.PageSize,
                currentPage = countriesFromRepo.CurrentPage,
                totalPages = countriesFromRepo.TotalPages,
                previousPageLink = previousPageLink,
                nextPageLink = nextPageLink
            };

            Response.Headers.Add("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

            var countries = Mapper.Map<IEnumerable<Models.CountryDto>>(countriesFromRepo);
            return Ok(countries.ShapeData(countriesResourceParameters.Fields));
        }

        [HttpGet("{id}", Name = "GetCountry")]
        public IActionResult GetCountry(int id, bool includePostalcodes = false)
        {
            var country = _repository.GetCountry(id, includePostalcodes);

            if (country == null)
            {
                return NotFound();
            }

            if (includePostalcodes)
            {
                var countryWithPostalCodesResult = Mapper.Map<Models.CountryWithPostalCodesDto>(country);
                return Ok(countryWithPostalCodesResult);
            }

            var countryResult = Mapper.Map<Models.CountryDto>(country);
            return Ok(countryResult);
        }

        [HttpPost]
        public IActionResult CreateCountry([FromBody] CountryForCreation country)
        {
            if (country == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                // return 422
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var countryEntity = Mapper.Map<Entities.Country>(country);

            _repository.AddCountry(countryEntity);

            if (!_repository.Save())
            {
                throw new Exception("Creating a country failed on save.");
                // return StatusCode(500, "A problem happened with handling your request.");
            }

            var countryToReturn = Mapper.Map<Models.CountryWithPostalCodesDto>(countryEntity);

            return CreatedAtRoute("GetCountry",
                new { id = countryToReturn.Id },
                countryToReturn);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCountry(int id)
        {
            var countryFromRepo = _repository.GetCountry(id, false);
            if (countryFromRepo == null)
            {
                return NotFound();
            }

            _repository.DeleteCountry(countryFromRepo);

            if (!_repository.Save())
            {
                throw new Exception($"Deleting country {id} failed on save.");
            }

            return NoContent();
        }

        private string CreateCountriesResourceUri(
             CountriesResourceParameters countriesResourceParameters,
             ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _urlHelper.Link("GetCountries",
                      new
                      {
                          fields = countriesResourceParameters.Fields,
                          orderBy = countriesResourceParameters.OrderBy,
                          searchQuery = countriesResourceParameters.SearchQuery,
                          pageNumber = countriesResourceParameters.PageNumber - 1,
                          pageSize = countriesResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return _urlHelper.Link("GetCountries",
                      new
                      {
                          fields = countriesResourceParameters.Fields,
                          orderBy = countriesResourceParameters.OrderBy,
                          searchQuery = countriesResourceParameters.SearchQuery,
                          pageNumber = countriesResourceParameters.PageNumber + 1,
                          pageSize = countriesResourceParameters.PageSize
                      });

                default:
                    return _urlHelper.Link("GetCountries",
                    new
                    {
                        fields = countriesResourceParameters.Fields,
                        orderBy = countriesResourceParameters.OrderBy,
                        searchQuery = countriesResourceParameters.SearchQuery,
                        pageNumber = countriesResourceParameters.PageNumber,
                        pageSize = countriesResourceParameters.PageSize
                    });
            }
        }
    }
}