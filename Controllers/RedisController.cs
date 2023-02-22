using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WebApi.Dtos;
using Microsoft.AspNetCore.Http;

namespace WebApi.Controllers
{
    [Route("api")]
    [ApiController]
    public class RedisController : ControllerBase
    {
        private readonly IConnectionMultiplexer _redis;

        public RedisController(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }


        [HttpGet("bancoredis")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Transacao), 200)]
        [ProducesResponseType(typeof(Transacao), 400)]
        [ProducesResponseType(typeof(Transacao), 500)]
        public async Task<IActionResult> Get(string key)
        {
            try
            {
                var db = _redis.GetDatabase();
                var msg = await db.StringGetAsync(key);

                if (msg.HasValue)
                {
                    return Ok(JsonConvert.DeserializeObject<Transacao>(msg));
                }
                else
                {
                    return BadRequest(new Transacao());
                }
            }
            catch
            {
                return StatusCode(500, new Transacao());
            }
        }

        [HttpPost]
        [Route("bancoredis")]
        [AllowAnonymous]
        public async Task Set( [FromForm] string key, string value)
        {
            var db = _redis.GetDatabase();
            await db.StringSetAsync(key, value, TimeSpan.FromSeconds(60));
        }
    }
}
