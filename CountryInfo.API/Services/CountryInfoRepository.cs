using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CountryInfo.API.Entities;
using Microsoft.EntityFrameworkCore;
using CountryInfo.API.Helpers;
using CountryInfo.API.Models;

namespace CountryInfo.API.Services
{
    public class CountryInfoRepository : ICountryInfoRepository
    {
        private readonly CountryInfoContext _context;
        private IPropertyMappingService _propertyMappingService;

        public CountryInfoRepository(CountryInfoContext context, IPropertyMappingService propertyMappingService)
        {
            _context = context;
            _propertyMappingService = propertyMappingService;
        }

        public PagedList<Country> GetCountries(CountriesResourceParameters countriesResourceParameters)
        {
            //var collectionBeforePaging =
            //    _context.Countries
            //    .OrderBy(c => c.Name)
            //    .AsQueryable();

            var collectionBeforePaging =
                 _context.Countries
                 .ApplySort(countriesResourceParameters.OrderBy,
                 _propertyMappingService.GetPropertyMapping<CountryDto, Country>());


            if (!string.IsNullOrEmpty(countriesResourceParameters.Continent))
            {
                // trim & ignore casing
                var continentForWhereClause = countriesResourceParameters.Continent
                    .Trim().ToLowerInvariant();
                collectionBeforePaging = collectionBeforePaging
                    .Where(c => c.Continent.ToLowerInvariant() == continentForWhereClause);
            }

            if (!string.IsNullOrEmpty(countriesResourceParameters.SearchQuery))
            {
                // trim & ignore casing
                var searchQueryForWhereClause = countriesResourceParameters.SearchQuery
                    .Trim().ToLowerInvariant();

                collectionBeforePaging = collectionBeforePaging
                    .Where(c => c.Name.ToLowerInvariant().Contains(searchQueryForWhereClause));
            }

            return PagedList<Country>.Create(collectionBeforePaging,
                countriesResourceParameters.PageNumber,
                countriesResourceParameters.PageSize);
        }

        public void AddCountry(Country country)
        {
            _context.Countries.Add(country);

            if (country.PostalCodes.Any())
            {
                foreach (var pc in country.PostalCodes)
                {
                    pc.CountryId = country.Id;
                }
            }
        }

        public void AddPostalCodeForCountry(int countryId, AreaPostalCode postalCode)
        {
            var country = _context.Countries.Where(c => c.Id == countryId).FirstOrDefault();

            if (country != null)
            {
                country.PostalCodes.Add(postalCode);
            }
        }

        public void UpdatePostalCodeForCountry(AreaPostalCode postalCode)
        {
            // no code in this implementation
        }

        public bool CountryExists(int countryId)
        {
            return _context.Countries.Any(c => c.Id == countryId);
        }

        public void DeleteCountry(Country country)
        {
            _context.Countries.Remove(country);
        }

        public void DeletePostalCode(AreaPostalCode postalCode)
        {
            _context.AreaPostalCodes.Remove(postalCode);
        }

        public Country GetCountry(int countryId, bool includePostalCodes)
        {
            if (includePostalCodes)
            {
                return _context.Countries.Include(c => c.PostalCodes)
                        .Where(c => c.Id == countryId).FirstOrDefault();
            }

            return _context.Countries.Where(c => c.Id == countryId).FirstOrDefault();
        }

        public AreaPostalCode GetPostalCodeForCountry(int countryId, int postalCodeId)
        {
            return _context.AreaPostalCodes.Where(p => p.CountryId == countryId && p.Id == postalCodeId).FirstOrDefault();
        }

        public IEnumerable<AreaPostalCode> GetPostalCodes(int countryId)
        {
            return _context.AreaPostalCodes.Where(p => p.CountryId == countryId).ToList();
        }

        public IEnumerable<AreaPostalCode> GetPostalCodesForCountry(int countryId)
        {
            return _context.AreaPostalCodes.Where(pc => pc.CountryId == countryId).ToList();
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void UpdateCountry(Country country)
        {
            throw new NotImplementedException();
        }
    }
}
