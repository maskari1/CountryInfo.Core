using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CountryInfo.API.Models
{
    public class LinkedCollectionResourceWrapperDto<T> : LinkedResourceBaseDto
         where T : LinkedResourceBaseDto
    {
        public IEnumerable<T> Value { get; set; }

        public LinkedCollectionResourceWrapperDto(IEnumerable<T> value)
        {
            Value = value;
        }
    }
}
