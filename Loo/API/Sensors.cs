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
        IMongoCollection<SensorHistory> _history;

        public Sensors()
        {
            _client = new MongoClient(Constants.MongoConnectionString);
            _db = _client.GetDatabase(Constants.MongoDatabase);
            _ctx = _db.GetCollection<Sensor>("Sensors");
            _history = _db.GetCollection<SensorHistory>("SensorHistory");
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
        /// <param name="clientId">Client ID.</param>
        [HttpGet("api/buildings")]
        public JsonResult GetBuildings(string clientId)
        {
            var buildings = _ctx.Distinct(x => x.BuildingName, "{\"ClientId\" : \"" + clientId + "\"}").ToList();
            return new JsonResult(buildings);
        }

        /// <summary>
        /// Gets list of Loo connected restroom names in a given building.
        /// </summary>
        /// <returns>JSON array of restroom names for a given building.</returns>
        /// <param name="clientId">Client name.</param>
        /// <param name="buildingName">Building name.</param>
        [HttpGet("api/restrooms")]
        public JsonResult GetRestrooms(string clientId, string buildingName)
        {
            var restrooms = _ctx.Distinct(x => x.LocationName, "{\"ClientId\" : \"" + clientId + "\", \"BuildingName\" : \"" + buildingName + "\"}").ToList();
            return new JsonResult(restrooms);
        }

        /// <summary>
        /// Gets all sensors for a given client and building.
        /// </summary>
        /// <returns>JSON array of sensors for a given building.</returns>
        /// <param name="clientId">Client identifier.</param>
        /// <param name="buildingName">Building name.</param>
        [HttpGet("api/sensors")]
        public JsonResult GetSensors(string clientId, string buildingName)
        {
            var restrooms = _ctx.Find("{\"ClientId\" : \"" + clientId + "\", \"BuildingName\" : \"" + buildingName + "\"}").ToList();
            return new JsonResult(restrooms);
        }

        /// <summary>
        /// Gets all sensors for a given client, building, and specified restroom.
        /// </summary>
        /// <returns>The sensors.</returns>
        /// <param name="clientId">Client identifier.</param>
        /// <param name="buildingName">Building name.</param>
        /// <param name="restroomName">Restroom name.</param>
        [HttpGet("api/restroom")]
        public JsonResult GetSensors(string clientId, string buildingName, string restroomName)
        {
            var restrooms = _ctx.Find("{\"ClientId\" : \"" + clientId + "\", \"BuildingName\" : \"" + buildingName + "\", \"LocationName\" : \"" + restroomName + "\"}").ToList();
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

            /*
            if (sensor.SensorId[0] == 'S')
            {
                sensor.SensorValue += 1;
            }
            else 
            {
                sensor.SensorValue = s.Value;
            }
            */

            sensor.SensorValue = s.Value;
            sensor.SensorBattery = s.Battery;

            var historyItem = new SensorHistory()
            {
                SensorId = sensor.SensorId,
                SensorValue = sensor.SensorValue,
                SensorBattery = sensor.SensorBattery,
                Timestamp = DateTime.Now
            };

            _ctx.ReplaceOne("{\"SensorId\" : \"" + s.SensorId + "\"}", sensor);
            _history.InsertOne(historyItem);

            return new JsonResult(sensor);
        }

        [HttpPost("api/add_sensor")]
        public JsonResult AddSensor([FromBody] SensorRegistration r)
        {
            return new JsonResult("GOOD");
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

    public class SensorRegistration
    {
        [JsonProperty(PropertyName = "sensorId")]
        public string SensorId { get; set; }
        [JsonProperty(PropertyName = "sensorName")]
        public string SensorName { get; set; }
        [JsonProperty(PropertyName = "calibrationValue")]
        public string CalibrationValue { get; set; }
        [JsonProperty(PropertyName = "sensorPlacement")]
        public string SensorPlacement { get; set; }
    }
}
