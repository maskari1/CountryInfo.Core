using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CountryInfo.API.Entities
{
    public class AreaPostalCode 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(10)]
        public string PostalCode { get; set; }

        [Required]
        [MaxLength(50)]
        public string City { get; set; }

        [MaxLength(50)]
        public string StateAbbrev { get; set; }

        [MaxLength(50)]
        public string County { get; set; }

        public int CountryId { get; set; }

        [MaxLength(5)]
        public string StateCode { get; set; }

        [ForeignKey("CountryId")]
        public Country Country { get; set; }
    }
}
