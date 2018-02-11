namespace CountryInfo.API.Models
{
    public class AreaPostalCodeDto : LinkedResourceBaseDto
    {
        public int Id { get; set; }

        public string PostalCode { get; set; }
        public string City { get; set; }
        public string StateAbbrev { get; set; }
        public string County { get; set; }
        public string StateCode { get; set; }
        public int CountryId { get; set; }
    }
}
