using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Caching.Distributed;

namespace KeyPublicWebApi.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
	private readonly IDistributedCache _distributedCache;
	public ValuesController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        
        [HttpGet]
	public IEnumerable<string> Get()
	{
	var cachedMessage = _distributedCache.GetString("i0");
	return new List<string> { cachedMessage };
	}

	// GET api/values/5
	[HttpGet("{id}")]
	public string Get(string id)
	{
	var cachedMessage = _distributedCache.GetString(id);
	return cachedMessage;
	}

	}
}
