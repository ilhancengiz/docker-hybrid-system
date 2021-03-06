﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
	private readonly IRedisConnectionFactory _redisFac;
	public ValuesController(IRedisConnectionFactory connFac)
        {
	    _redisFac = connFac;
        }
        
        [HttpGet]
	public IEnumerable<string> Get()
	{
	    ConnectionMultiplexer m = _redisFac.Connection();
            return m.GetServer("redis:6379").Keys().Select(key => (string)key).ToList();
	}

	// GET api/values/5
	[HttpGet("{id}")]
	public string Get(string id)
	{
	    ConnectionMultiplexer m = _redisFac.Connection();
	    var db = m.GetDatabase();
	    return db.StringGet(id);
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
