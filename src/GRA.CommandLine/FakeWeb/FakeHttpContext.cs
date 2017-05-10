﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using GRA.Controllers;
using Microsoft.AspNetCore.Http;

namespace GRA.CommandLine.FakeWeb
{
    class FakeHttpContext : IHttpContextAccessor
    {
        private HttpContext _httpContext;
        public FakeHttpContext()
        {
            _httpContext = new DefaultHttpContext()
            {
                Session = new FakeSession()
            };
        }

        public HttpContext HttpContext
        {
            get
            {
                return _httpContext;
            }
            set
            {
                _httpContext = value;
            }
        }
    }
}
