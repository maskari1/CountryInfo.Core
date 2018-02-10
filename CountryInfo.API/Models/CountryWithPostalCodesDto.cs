using System.Collections.Generic;

namespace CountryInfo.API.Models
{
    public class CountryWithPostalCodesDto
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string PostalCodeFormat { get; set; }
        public string Continent { get; set; }

        public ICollection<AreaPostalCodeDto> PostalCodes { get; set; } = new List<AreaPostalCodeDto>();
    }
}
