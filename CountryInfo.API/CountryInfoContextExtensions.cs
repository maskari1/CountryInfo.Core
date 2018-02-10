using CountryInfo.API.Entities;
using System.Collections.Generic;
using System.Linq;

namespace CountryInfo.API
{
    public static class CountryInfoContextExtensions
    {
        public static void EnsureSeedDataForContext(this CountryInfoContext context, bool recreate = false)
        {
            if (recreate)
            {
                context.Countries.RemoveRange(context.Countries);
                context.SaveChanges();
            }

            if (context.Countries.Any())
            {
                return;
            }

            // init seed data
            var countries = new List<Country>
            {
                new Country
                {
                    Name = "United States",
                    Abbreviation = "USA",
                    PostalCodeFormat = "#####-####",
                    Continent = "North America",
                    PostalCodes = new List<AreaPostalCode>
                    {
                        new AreaPostalCode
                        {
                            City = "Burlington",
                            PostalCode = "01803-3931",
                            StateAbbrev = "MA",
                            County = "Middlesex County",
                            StateCode = "MA"
                        },
                        new AreaPostalCode
                        {
                            City = "Boston",
                            PostalCode = "01803-3931",
                            StateAbbrev = "MA",
                            County = "Suffolk County",
                            StateCode = "MA"
                        },
                        new AreaPostalCode
                        {
                            City = "Worcester",
                            PostalCode = "01803-3931",
                            StateAbbrev = "MA",
                            County = "Worcester County",
                            StateCode = "MA"
                        },
                        new AreaPostalCode
                        {
                            City = "Springfield",
                            PostalCode = "01803-3931",
                            StateAbbrev = "MA",
                            County = "Hampden County",
                            StateCode = "MA"
                        },

                        new AreaPostalCode
                        {
                            City = "Woburn",
                            PostalCode = "01801-3931",
                            StateAbbrev = "MA",
                            County = "Middlesex County",
                            StateCode = "MA"
                        },
                        new AreaPostalCode
                        {
                            City = "Manhattan",
                            PostalCode = "10001-3931",
                            StateAbbrev = "NY",
                            County = "New York",
                            StateCode = "NY"
                        },
                        new AreaPostalCode
                        {
                            City = "Los Angeles",
                            PostalCode = "10001-3931",
                            StateAbbrev = "CA",
                            County = "San Bernardino",
                            StateCode = "CA"
                        },
                        new AreaPostalCode
                        {
                            City = "San Fransisco",
                            PostalCode = "10001-3931",
                            StateAbbrev = "CA",
                            County = "San Mateo",
                            StateCode = "CA"
                        },
                        new AreaPostalCode
                        {
                            City = "Santa Barbara",
                            PostalCode = "10001-3931",
                            StateAbbrev = "CA",
                            County = "Ventura",
                            StateCode = "CA"
                        },
                        new AreaPostalCode
                        {
                            City = "Sacramento",
                            PostalCode = "10001-3931",
                            StateAbbrev = "CA",
                            County = "Placer",
                            StateCode = "CA"
                        },
                        new AreaPostalCode
                        {
                            City = "San Diego",
                            PostalCode = "10001-3931",
                            StateAbbrev = "CA",
                            County = "Imperial",
                            StateCode = "CA"
                        }
                    }
                },
                new Country
                {
                    Name = "Canada",
                    Abbreviation = "CA",
                    PostalCodeFormat = "#####-####",
                    Continent = "North America",
                    PostalCodes = new List<AreaPostalCode>
                    {
                        new AreaPostalCode
                        {
                            City = "Montreal",
                            PostalCode = "01803-3931",
                            StateAbbrev = "BC",
                            County = "British Columbia"
                        }
                    }
                },
                new Country
                {
                    Name = "Colombia",
                    Abbreviation = "CO",
                    PostalCodeFormat = "######",
                    Continent = "South America",
                    PostalCodes = new List<AreaPostalCode>
                    {
                       new AreaPostalCode
                       {
                            City = "Medlilne",
                            PostalCode = "05002",
                            StateAbbrev = "",
                            County = "Antioquia"
                        }
                    }
                },
                new Country
                {
                    Name = "China",
                    Abbreviation = "CN",
                    PostalCodeFormat = "#####",
                    Continent = "Asia",
                    PostalCodes = new List<AreaPostalCode>()
                },
                new Country
                {
                    Name = "Japan",
                    Abbreviation = "JPN",
                    PostalCodeFormat = "###-####",
                    Continent = "Asia",
                    PostalCodes = new List<AreaPostalCode>
                    {
                       new AreaPostalCode
                       {
                            City = "Tokyo",
                            PostalCode = "111-0032",
                            StateAbbrev = "",
                            County = "Kanto"
                        }
                    }
                },
                new Country
                {
                    Name = "Chile",
                    Abbreviation = "CL",
                    PostalCodeFormat = "#####",
                    Continent = "South America",
                    PostalCodes = new List<AreaPostalCode>()
                },
                new Country
                {
                    Name = "Mexico",
                    Abbreviation = "MEX",
                    PostalCodeFormat = "######",
                    Continent = "North America",
                    PostalCodes = new List<AreaPostalCode>
                    {
                       new AreaPostalCode
                       {
                            City = "Roma",
                            PostalCode = "31050",
                            StateAbbrev = "Col",
                            County = "Chihauahua"
                        }
                    }
                },
                new Country
                {
                    Name = "Morocco",
                    Abbreviation = "MA",
                    PostalCodeFormat = "######",
                    Continent = "Africa",
                    PostalCodes = new List<AreaPostalCode>
                    {
                       new AreaPostalCode
                       {
                            City = "Agadir",
                            PostalCode = "80000",
                            StateAbbrev = "AG",
                            County = "Agadir"
                        }
                    }
                },
                new Country
                {
                    Name = "France",
                    Abbreviation = "FR",
                    PostalCodeFormat = "#####",
                    Continent = "Europe",
                    PostalCodes = new List<AreaPostalCode>
                    {
                       new AreaPostalCode
                       {
                            City = "Paris",
                            PostalCode = "75001",
                            StateAbbrev = "LD",
                            County = "Lie-de"
                        }
                    }
                },
                new Country
                {
                    Name = "Italy",
                    Abbreviation = "ITL",
                    PostalCodeFormat = "#####",
                    Continent = "Europe",
                    PostalCodes = new List<AreaPostalCode>
                    {
                       new AreaPostalCode
                       {
                            City = "Rome",
                            PostalCode = "00199",
                            StateAbbrev = "LZ",
                            County = "Lazio"
                        }
                    }
                },
                new Country
                {
                    Name = "Turkey",
                    Abbreviation = "TK",
                    PostalCodeFormat = "#####",
                    Continent = "Asia",
                    PostalCodes = new List<AreaPostalCode>
                    {
                        new AreaPostalCode
                        {
                            City = "Ankara",
                            PostalCode = "06670",
                            StateAbbrev = "AK",
                            County = "Ankara"
                        }
                    }
                }
            };

            context.Countries.AddRange(countries);
            context.SaveChanges();
        }
    }
}
