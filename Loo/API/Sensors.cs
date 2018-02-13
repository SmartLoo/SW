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
        IMongoCollection<Sensor> _ctx;

        public Sensors()
        {
            _client = new MongoClient(Constants.MongoConnectionString);
            _db = _client.GetDatabase(Constants.MongoDatabase);
            _ctx = _db.GetCollection<Sensor>("Sensors");
        }

        /// <summary>
        /// Get client list.
        /// </summary>
        /// <returns>JSON array of clients.</returns>
        [HttpGet("api/clients")]
        public JsonResult GetClients()
        {
            var clients = _ctx.Distinct(x => x.ClientName, "{ }").ToList();
            
            return new JsonResult(clients);
        }

        /// <summary>
        /// Get sensor specified by GUID.
        /// </summary>
        /// <returns>Sensor data</returns>
        /// <param name="sensorId">Sensor GUID.</param>
        [HttpGet("api/sensor")]
        public JsonResult GetSensor(string sensorId)
        {
            var sensor = _ctx.Find("{\"SensorId\" : \"" + sensorId + "\"}").ToList();
            return new JsonResult(sensor);
        }

        /// <summary>
        /// Gets buildings associated with client.
        /// </summary>
        /// <returns>JSON array of buildings.</returns>
        /// <param name="clientName">Client name.</param>
        [HttpGet("api/buildings")]
        public JsonResult GetBuildings(string clientName)
        {
            var buildings = _ctx.Distinct(x => x.BuildingName, "{\"ClientName\" : \"" + clientName + "\"}").ToList();
            return new JsonResult(buildings);
        }

        /// <summary>
        /// Gets Loo connected restrooms in a given building.
        /// </summary>
        /// <returns>JSON array of restroom names for a given building.</returns>
        /// <param name="clientName">Client name.</param>
        /// <param name="buildingName">Building name.</param>
        [HttpGet("api/restrooms")]
        public JsonResult GetRestrooms(string clientName, string buildingName)
        {
            var restrooms = _ctx.Distinct(x => x.LocationName, "{\"ClientName\" : \"" + clientName + "\", \"BuildingName\" : \"" + buildingName + "\"}").ToList();
            return new JsonResult(restrooms);
        }

        /// <summary>
        /// Updates sensor value and battery level.
        /// </summary>
        /// <returns>Updated sensor information.</returns>
        /// <param name="s">S.</param>
        [HttpPost("api/sensor")]
        public JsonResult UpdateSensor([FromBody] SensorUpdate s)
        {
            var sensor = _ctx.Find("{\"SensorId\" : \"" + s.SensorId + "\"}").FirstOrDefault();
            sensor.SensorValue = s.Value;
            sensor.SensorBattery = s.Battery;

            _ctx.ReplaceOne("{\"SensorId\" : \"" + s.SensorId + "\"}", sensor);

            return new JsonResult(sensor);
        }

    }

    public class SensorUpdate
    {
		[JsonProperty(PropertyName = "sensorId")]
        public string SensorId { get; set; }
		[JsonProperty(PropertyName = "sensorValue")]
		public float Value { get; set; }
		[JsonProperty(PropertyName = "batteryLevel")]
		public float Battery { get; set; }
    }
}
