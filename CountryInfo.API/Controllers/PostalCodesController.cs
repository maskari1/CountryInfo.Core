using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CountryInfo.API.Services;
using Microsoft.Extensions.Logging;
using AutoMapper;
using CountryInfo.API.Models;
using CountryInfo.API.Helpers;
using Microsoft.AspNetCore.JsonPatch;

namespace CountryInfo.API.Controllers
{
    [Route("api/countries/{countryId}/postalcodes")]
    public class PostalCodesController : Controller
    {
        private readonly ILogger<PostalCodesController> _logger;
        private readonly ICountryInfoRepository _repository;
        private IUrlHelper _urlHelper;

        public PostalCodesController(ILogger<PostalCodesController> logger, 
            ICountryInfoRepository repository,
            IUrlHelper urlHelper)
        {
            _logger = logger;
            _repository = repository;
            _urlHelper = urlHelper;
        }

        [HttpGet(Name = "GetPostalCodesForCountry")]
        public IActionResult GetPostalCodesForCountry(int countryId)
        {
            if (!_repository.CountryExists(countryId))
            {
                return NotFound();
            }

            var postalCodesForCountryFromRepo = _repository.GetPostalCodesForCountry(countryId);

            var postalCodesForCountry = Mapper.Map<IEnumerable<Models.AreaPostalCodeDto>>(postalCodesForCountryFromRepo);

            postalCodesForCountry = postalCodesForCountry.Select(postalCode =>
            {
                postalCode = CreateLinksForPostalCode(postalCode);
                return postalCode;
            });

            var wrapper = new LinkedCollectionResourceWrapperDto<AreaPostalCodeDto>(postalCodesForCountry);

            return Ok(CreateLinksForPostalCodes(wrapper));
        }

        [HttpGet("{id}", Name = "GetPostalCodeForCountry")]
        public IActionResult GetPostalCodeForCountry(int countryId, int id)
        {
            if (!_repository.CountryExists(countryId))
            {
                return NotFound();
            }

            var postalCodeForCountryFromRepo = _repository.GetPostalCodeForCountry(countryId, id);
            if (postalCodeForCountryFromRepo == null)
            {
                return NotFound();
            }

            var postalcodeForCountry = Mapper.Map<Models.AreaPostalCodeDto>(postalCodeForCountryFromRepo);
            return Ok(CreateLinksForPostalCode(postalcodeForCountry));
        }

        [HttpPost(Name = "CreatePostalCodeForCountry")]
        public IActionResult CreatePostalCodeForCountry(int countryId,
            [FromBody] PostalCodeForCreation postalCode)
        {
            if (postalCode == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            if (!_repository.CountryExists(countryId))
            {
                return NotFound();
            }

            var postalEntity = Mapper.Map<Entities.AreaPostalCode>(postalCode);

            _repository.AddPostalCodeForCountry(countryId, postalEntity);

            if (!_repository.Save())
            {
                throw new Exception($"Creating a postal code for country {countryId} failed on save.");
            }

            var postalToReturn = Mapper.Map<Models.AreaPostalCodeDto>(postalEntity);

            return CreatedAtRoute("GetPostalCodeForCountry",
                new {  countryId, id = postalToReturn.Id },
                CreateLinksForPostalCode(postalToReturn));
        }

        [HttpPatch("{id}", Name = "PartiallyUpdatePostalCodeForCountry")]
        public IActionResult PartiallyUpdatePostalCodeForCountry(int countryId, int id,
            [FromBody]JsonPatchDocument<PostalCodeForCreation> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            if (!_repository.CountryExists(countryId))
            {
                return BadRequest();
            }

            var postalCodeEntity = _repository.GetPostalCodeForCountry(countryId, id);
            if (postalCodeEntity == null)
            {
                return NotFound();
            }

            var postalCodeToPatch = Mapper.Map<Models.PostalCodeForCreation>(postalCodeEntity);
            patchDoc.ApplyTo(postalCodeToPatch, ModelState);

            TryValidateModel(postalCodeToPatch);

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            Mapper.Map(postalCodeToPatch, postalCodeEntity);

            _repository.UpdatePostalCodeForCountry(postalCodeEntity);

            if (!_repository.Save())
            {
                throw new Exception($"Patching postal code {id} for country {countryId} failed on save.");
            }

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeletePostalCodeForCountry")]
        public IActionResult DeletePostalCodeForCountry(int countryId, int id)
        {
            if (!_repository.CountryExists(countryId))
            {
                return NotFound();
            }

            var postalCodeEntity = _repository.GetPostalCodeForCountry(countryId, id);
            if (postalCodeEntity == null)
            {
                return NotFound();
            }

            _repository.DeletePostalCode(postalCodeEntity);

            if (!_repository.Save())
            {
                throw new Exception($"Deleting postal code {id} for country {countryId} failed on save.");
            }

            _logger.LogInformation(100, $"Postal code {id} for country {countryId} was deleted.");

            return NoContent();
        }

        private AreaPostalCodeDto CreateLinksForPostalCode(AreaPostalCodeDto postalCode)
        {
            postalCode.Links.Add(new LinkDto(_urlHelper.Link("GetPostalCodeForCountry",
                new { id = postalCode.Id }),
                "self",
                "GET"));

            postalCode.Links.Add(
                new LinkDto(_urlHelper.Link("DeletePostalCodeForCountry",
                new { id = postalCode.Id }),
                "delete_postalcode",
                "DELETE"));

            postalCode.Links.Add(
                new LinkDto(_urlHelper.Link("UpdatePostalCodeForCountry",
                new { id = postalCode.Id }),
                "update_postalcode",
                "PUT"));

            postalCode.Links.Add(
                new LinkDto(_urlHelper.Link("PartiallyUpdatePostalCodeForCountry",
                new { id = postalCode.Id }),
                "partially_update_postalcode",
                "PATCH"));

            return postalCode;
        }

        private LinkedCollectionResourceWrapperDto<AreaPostalCodeDto> CreateLinksForPostalCodes(
            LinkedCollectionResourceWrapperDto<AreaPostalCodeDto> postalCodesWrapper)
        {
            // link to self
            postalCodesWrapper.Links.Add(
                new LinkDto(_urlHelper.Link("GetPostalCodesForCountry", new { }),
                "self",
                "GET"));

            return postalCodesWrapper;
        }
    }
}