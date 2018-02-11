using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CountryInfo.API.Services;
using AutoMapper;
using System.Collections.Generic;
using CountryInfo.API.Models;
using System;
using System.Linq;
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
            var countries = Mapper.Map<IEnumerable<CountryDto>>(countriesFromRepo);

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
                previousPageLink,
                nextPageLink
            };

            Response.Headers.Add("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

            var links = CreateLinksForCountries(countriesResourceParameters,
                 countriesFromRepo.HasNext, countriesFromRepo.HasPrevious);

            var shapedCountries = countries.ShapeData(countriesResourceParameters.Fields);

            var shapedCountriesWithLinks = shapedCountries.Select(author =>
            {
                var countryAsDictionary = author as IDictionary<string, object>;
                var countryLinks = CreateLinksForCountry(
                    (int)countryAsDictionary["Id"], countriesResourceParameters.Fields);

                countryAsDictionary.Add("links", countryLinks);

                return countryAsDictionary;
            });

            var linkedCollectionResource = new
            {
                value = shapedCountriesWithLinks,
                links
            };

            return Ok(linkedCollectionResource);
        }

        [HttpGet("{id}", Name = "GetCountry")]
        public IActionResult GetCountry(int id, [FromQuery] string fields, bool includePostalCodes = false)
        {
            if (!_typeHelperService.TypeHasProperties<CountryDto>(fields))
            {
                return BadRequest();
            }

            var countryFromRepo = _repository.GetCountry(id, includePostalCodes);

            if (countryFromRepo == null)
            {
                return NotFound();
            }

            if (includePostalCodes)
            {
                var countryWithPostalCodes= Mapper.Map<Models.CountryWithPostalCodesDto>(countryFromRepo);
                return Ok(countryWithPostalCodes);
            }

            var country = Mapper.Map<Models.CountryDto>(countryFromRepo);

            var links = CreateLinksForCountry(id, fields);

            var linkedResourceToReturn = country.ShapeData(fields)
                as IDictionary<string, object>;

            linkedResourceToReturn.Add("links", links);

            return Ok(linkedResourceToReturn);
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

        [HttpDelete("{id}", Name = "DeleteCountry")]
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

        private IEnumerable<LinkDto> CreateLinksForCountry(int id, string fields)
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                  new LinkDto(_urlHelper.Link("GetCountry", new {  id }),
                  "self",
                  "GET"));
            }
            else
            {
                links.Add(
                  new LinkDto(_urlHelper.Link("GetCountry", new {  id,  fields }),
                  "self",
                  "GET"));
            }

            links.Add(
              new LinkDto(_urlHelper.Link("DeleteCountry", new {  id }),
              "delete_country",
              "DELETE"));

            links.Add(
              new LinkDto(_urlHelper.Link("CreatePostalCodeForCountry", new { countryId = id }),
              "create_postalcode_for_country",
              "POST"));

            links.Add(
               new LinkDto(_urlHelper.Link("GetPostalCodesForCountry", new { countryId = id }),
               "books",
               "GET"));

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForCountries(
            CountriesResourceParameters countriesResourceParameters,
            bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            // self 
            links.Add(
               new LinkDto(CreateCountriesResourceUri(countriesResourceParameters,
               ResourceUriType.Current)
               , "self", "GET"));

            if (hasNext)
            {
                links.Add(
                  new LinkDto(CreateCountriesResourceUri(countriesResourceParameters,
                  ResourceUriType.NextPage),
                  "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(
                    new LinkDto(CreateCountriesResourceUri(countriesResourceParameters,
                    ResourceUriType.PreviousPage),
                    "previousPage", "GET"));
            }

            return links;
        }
    }
}