using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace KeyPublicWebApi.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
	private readonly IDistributedCache _distributedCache;
	private readonly IRedisConnectionFactory _redisFac;
	public ValuesController(IDistributedCache distributedCache, IRedisConnectionFactory connFac)
        {
            _distributedCache = distributedCache;
	    _redisFac = connFac;
        }
        
        [HttpGet]
	public IEnumerable<string> Get()
	{
	    ConnectionMultiplexer m = _redisFac.Connection();
            List<string> keys = m.GetServer("redis:6379").Keys().Select(key => (string)key).ToList();
	    List<string> values = new List<string>();
	    foreach(string key in keys)
	    {
	    	values.Add(Encoding.UTF8.GetString(_distributedCache.Get(key)));
	    }
	     return values;
	}

	// GET api/values/5
	[HttpGet("{id}")]
	public string Get(string id)
	{
	var cachedMessage = _distributedCache.GetString(id);
	return cachedMessage;
	}

    }
    
    public interface IRedisConnectionFactory
    {
        ConnectionMultiplexer Connection();
    }
	
    public class RedisConnectionFactory : IRedisConnectionFactory
    {
        /// <summary>
        ///     The _connection.
        /// </summary>
        private readonly Lazy<ConnectionMultiplexer> _connection;

        public RedisConnectionFactory()
        {
            this._connection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect("redis:6379"));
        }

        public ConnectionMultiplexer Connection()
        {
            return this._connection.Value;
        }
    }
}
