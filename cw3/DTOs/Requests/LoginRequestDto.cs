﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationSampleWebApp.DTOs
{
    public class LoginRequestDto
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
