using System.ComponentModel.DataAnnotations;

namespace CountryInfo.API.Models
{
    public class AreaPostalCodeForCreation
    {
        [Required(ErrorMessage = "You should fill out a postal code.")]
        [MaxLength(10, ErrorMessage = "The postal code shouldn't have more than 10 characters.")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "You should fill out a city.")]
        [MaxLength(50, ErrorMessage = "The city shouldn't have more than 50 characters.")]
        public string City { get; set; }

        [MaxLength(50, ErrorMessage = "The stateAbbrev shouldn't have more than 50 characters.")]
        public string StateAbbrev { get; set; }

        [MaxLength(50, ErrorMessage = "The county shouldn't have more than 50 characters.")]
        public string County { get; set; }
        public string StateCode { get; set; }
    }
}
