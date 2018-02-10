using CountryInfo.API.Entities;
using CountryInfo.API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CountryInfo.API.Services
{
    public interface ICountryInfoRepository
    {
        PagedList<Country> GetCountries(CountriesResourceParameters countriesResourceParameters);
        Country GetCountry(int countryId, bool includePostalCodes);
        void AddCountry(Country country);
        void DeleteCountry(Country country);
        void UpdateCountry(Country country);
        bool CountryExists(int countryId);
        IEnumerable<AreaPostalCode> GetPostalCodesForCountry(int countryId);
        AreaPostalCode GetPostalCodeForCountry(int countryId, int postalCodeId);
        void AddPostalCodeForCountry(int countryId, AreaPostalCode postalcode);
        void UpdatePostalCodeForCountry(AreaPostalCode postalCode);
        void DeletePostalCode(AreaPostalCode postalCode);
        bool Save();
    }
}
