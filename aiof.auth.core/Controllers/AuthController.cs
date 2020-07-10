using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace aiof.auth.core.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        public AuthController()
        {
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("");
        }
    }
}
