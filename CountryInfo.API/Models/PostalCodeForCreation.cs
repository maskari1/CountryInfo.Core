using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CountryInfo.API.Models
{
    public class PostalCodeForCreation
    {
        [Required(ErrorMessage = "You should fill out a postalCode.")]
        [MaxLength(10, ErrorMessage = "The postaCcode shouldn't have more than 10 characters.")]

        public string PostalCode { get; set; }

        [Required(ErrorMessage = "You should fill out a city.")]
        [MaxLength(50, ErrorMessage = "The city shouldn't have more than 50 characters.")]
        public string City { get; set; }

        [MaxLength(50, ErrorMessage = "The stateAbbrev shouldn't have more than 50 characters.")]
        public string StateAbbrev { get; set; }

        [MaxLength(50, ErrorMessage = "The county shouldn't have more than 50 characters.")]
        public string County { get; set; }

        [MaxLength(5, ErrorMessage = "The stateCode shouldn't have more than 5 characters.")]
        public string StateCode { get; set; }
    }
}
