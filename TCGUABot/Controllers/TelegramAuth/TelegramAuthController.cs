using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TCGUABot.Controllers.TelegramAuth
{
    [Route("api/[controller]")]
    [ApiController]
    public class TelegramAuthController : ControllerBase
    {
        // GET: api/TelegramAuth
        [HttpGet("/login-telegram", Name = "TelegramAuthGet")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("/login-github", Name = "GitHubAuthGet")]
        public IEnumerable<string> GetGit()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/TelegramAuth/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/TelegramAuth
        [HttpPost("/login-telegram", Name ="TelegramAuthPost")]
        public string Post([FromBody] string value)
        {
            return Convert.ToString(value);
        }

        // PUT: api/TelegramAuth/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
