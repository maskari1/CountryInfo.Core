using System.Collections.Generic;

namespace CountryInfo.API.Models
{
    public class CountryDto
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string PostalCodeFormat { get; set; }
        public string Continent{ get; set; }
    }
}
