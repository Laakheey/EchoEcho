﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia_Common.UserView
{
    public class Country
    {
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
    }

    public class Countries
    {
        public Name Name { get; set; }
        public string Cca2 { get; set; }
    }

    public class Name
    {
        public string Common { get; set; }
        public string Official { get; set; }
    }

    public class CitiesResponse
    {
        public List<string> Data { get; set; }
    }
}
