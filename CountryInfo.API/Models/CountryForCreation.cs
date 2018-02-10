using System.ComponentModel.DataAnnotations;

namespace CountryInfo.API.Models
{
    public class CountryForCreation
    {
        [Required(ErrorMessage = "You should fill out a name.")]
        [MaxLength(50, ErrorMessage = "The country shouldn't have more than 100 characters.")]
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string PostalCodeFormat { get; set; }
    }
}
