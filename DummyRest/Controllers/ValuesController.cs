﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DummyRest.Schema;
using Microsoft.AspNetCore.Mvc;

namespace DummyRest.Controllers
{
    
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        [Route("api/baseline")]
        public IActionResult Baseline()
        {
            return Ok();
        }

        // GET api/values/5
        [Route("api/test")]
        [HttpPost]
        public TestRestPoco Get([FromBody]TestRestPoco request)
        {
            return new TestRestPoco()
            {
                Prop1 = Guid.NewGuid().ToString(),
                Prop2 = Guid.NewGuid().ToString(),
                Prop3 = Guid.NewGuid().ToString(),
                Prop4 = Int32.MinValue,
                Prop5 = Int32.MaxValue,
                Prop6 = Int32.MaxValue / 7,
                Prop7 = Double.MaxValue,
                Prop8 = double.MinValue,
                Prop9 = double.MaxValue / 7,
                Prop10 = true,
                Prop11 = false,
                Prop12 = true
            };
        }

    }
}
