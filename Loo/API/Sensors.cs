using System;
using System.Threading.Tasks;
using Loo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.Client;
using Loo;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using MongoDB.Driver;
using MongoDB.Bson;

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
            List<KeyValuePair<string, string>> clients = clientCollection
                .Find("{}")
                .Project<Client>("{Name: 1}")
                .ToEnumerable()
                .Select(o => new KeyValuePair<string, string>(o.Name, o.Id.ToString()))
                .ToList();
            
            return new JsonResult(clients);
        }

        [HttpGet("api/client")]
        public JsonResult GetClient(string clientId)
        {
            IMongoCollection<Client> clientCollection = _db.GetCollection<Client>("Clients");
            Client client = clientCollection.Find("{ _id : ObjectId(\"" + clientId + "\") }").Single();
            return new JsonResult(client);
        }

        [HttpGet("api/sensors")]
        public JsonResult GetSensor(string clientId, string buildingId, string restroomId, string sensorId)
        {
            IMongoCollection<Client> clientCollection = _db.GetCollection<Client>("Clients");
            Client client = clientCollection
                .Find("{ _id : ObjectId(\"" + clientId + "\") }")
                .Single();

            var sensor = client.Buildings
                 .FirstOrDefault(b => b.BuildingCode == buildingId)
                 .Restrooms
                 .FirstOrDefault(r => r.RestroomCode == restroomId)
                 .Sensors
                 .FirstOrDefault(s => s.SensorId == sensorId);
            
            return new JsonResult(sensor);
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
