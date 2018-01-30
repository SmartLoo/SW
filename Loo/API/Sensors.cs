﻿using System;
using System.Threading.Tasks;
using Loo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.Client;
using Loo;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Loo.API
{
    public class Sensors : Controller
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _db;

        public Sensors()
        {
            _client = new MongoClient(Constants.MongoConnectionString);
            _db = _client.GetDatabase(Constants.MongoDatabase);
        }

        /// <summary>
        /// Retrieve all locations where sensors exist.
        /// </summary>
        /// <returns>The locations.</returns>
        [HttpGet("api/clients")]
        public JsonResult GetClients()
        {
            IMongoCollection<Client> clientCollection = _db.GetCollection<Client>("Clients");
            List<string> clients = clientCollection.Find("{}").Project<Client>("{Name: 1}").ToList().Select(x => x.Name).ToList();
            return new JsonResult(clients);
        }

        [HttpGet("api/sensor/{clientName}")]
        public JsonResult GetClient(string clientName)
        {
            IMongoCollection<Client> clientCollection = _db.GetCollection<Client>("Clients");
            Client client = clientCollection.Find(String.Format("{Name: {0}", clientName)).First();
            return new JsonResult(client);
        }

    }

    public class SensorReq
    {
		[JsonProperty(PropertyName = "sensorId")]
        public string SensorId { get; set; }
		[JsonProperty(PropertyName = "value")]
		public float Value { get; set; }
		[JsonProperty(PropertyName = "battery")]
		public float Battery { get; set; }
    }

    public class SensorAddReq : Sensor
    {
        [JsonProperty(PropertyName = "building")]
        public string Building { get; set; }
        [JsonProperty(PropertyName = "location")]
        public string LocationName { get; set; }
    }
}
