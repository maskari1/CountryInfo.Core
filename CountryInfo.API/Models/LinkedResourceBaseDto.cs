﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CountryInfo.API.Models
{
    public abstract class LinkedResourceBaseDto
    {
        public List<LinkDto> Links { get; set; }
        = new List<LinkDto>();
    }
}
