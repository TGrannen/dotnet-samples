using AutoBogus;
using HttpREPL.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace HttpREPL.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<Person> Get()
        {
            var rng = new Random();
            return new AutoFaker<Person>()
                .RuleFor(fake => fake.Id, fake => fake.Random.Int(1, 1000))
                .RuleFor(fake => fake.FirstName, f => f.Person.FirstName)
                .RuleFor(fake => fake.LastName, f => f.Person.LastName)
                .Generate(rng.Next(0, 5));
        }
    }
}