
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CountryInfo.API.Entities
{
    using System.Collections.Generic;

    public class Country 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(5)]
        public string Abbreviation { get; set; }

        [MaxLength(10)]
        public string PostalCodeFormat { get; set; }

        [MaxLength(25)]
        public string Continent { get; set; }

        public ICollection<AreaPostalCode> PostalCodes { get; set; } = new HashSet<AreaPostalCode>();
    }
}
