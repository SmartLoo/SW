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
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Loo.API
{
    public class Sensors : Controller
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _db;
        IMongoCollection<Sensor> _ctx;
        IMongoCollection<SensorHistory> _history;
        private readonly UserManager<AuthenticatedUser> _userManager;

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
        [HttpGet("api/buildings")]
        public JsonResult GetBuildings()
        {
            var user = User.Identity;
            var buildings = _ctx.Distinct(x => x.BuildingName, "{\"ClientId\" : \"" + user.Name + "\"}").ToList();
            return new JsonResult(buildings);
        }

        /// <summary>
        /// Gets list of Loo connected restroom names in a given building.
        /// </summary>
        /// <returns>JSON array of restroom names for a given building.</returns>
        /// <param name="clientId">Client name.</param>
        /// <param name="buildingName">Building name.</param>
        [HttpGet("api/restrooms")]
        public JsonResult GetRestrooms(string buildingName)
        {
            var user = User.Identity;
            var restrooms = _ctx.Distinct(x => x.LocationName, "{\"ClientId\" : \"" + user.Name + "\", \"BuildingName\" : \"" + buildingName + "\"}").ToList();
            return new JsonResult(restrooms);
        }

        /// <summary>
        /// Gets all sensors for a given client and building.
        /// </summary>
        /// <returns>JSON array of sensors for a given building.</returns>
        /// <param name="clientId">Client identifier.</param>
        /// <param name="buildingName">Building name.</param>
        [HttpGet("api/sensors")]
        public JsonResult GetSensors(string buildingName)
        {
            var user = User.Identity;
            var restrooms = _ctx.Find("{\"ClientId\" : \"" + user.Name + "\", \"BuildingName\" : \"" + buildingName + "\"}").ToList();
            return new JsonResult(restrooms);
        }

        /// <summary>
        /// Returns first sensor matching bridge ID for location data when
        /// adding new accessory to Loo Cloud.
        /// </summary>
        /// <returns>The sensor by bridge.</returns>
        /// <param name="bridgeId">Bridge identifier.</param>
        [HttpGet("api/bridge")]
        public JsonResult GetSensorByBridge(string bridgeId)
        {
            var restroom = _ctx.Find("{\"BridgeId\" : \"" + bridgeId + "\"}")
                        .ToList()
                        .FirstOrDefault(x => x.LocationName != "");

            if (restroom != null)
            {
                return new JsonResult(restroom);
            }
            else
            {
                return new JsonResult("[]");
            }
        }

        /// <summary>
        /// Gets all sensors for a given client, building, and specified restroom.
        /// </summary>
        /// <returns>The sensors.</returns>
        /// <param name="clientId">Client identifier.</param>
        /// <param name="buildingName">Building name.</param>
        /// <param name="restroomName">Restroom name.</param>
        [HttpGet("api/restroom")]
        public JsonResult GetSensors(string buildingName, string restroomName)
        {
            var user = User.Identity;
            var restrooms = _ctx.Find("{\"ClientId\" : \"" + user.Name + "\", \"BuildingName\" : \"" + buildingName + "\", \"LocationName\" : \"" + restroomName + "\"}").ToList();
            return new JsonResult(restrooms);
        }

        /// <summary>
        /// Updates sensor value and timestamp.
        /// </summary>
        /// <returns>Updated sensor information.</returns>
        /// <param name="s">S.</param>
        [HttpPost("api/sensor")]
        public JsonResult UpdateSensor([FromBody] SensorUpdate s)
        {
            var sensor = _ctx.Find("{\"SensorId\" : \"" + s.SensorId + "\"}").FirstOrDefault();

            if (sensor == null)
            {
                sensor = new Sensor(s.SensorId);         
            }

            if (sensor.SensorId[0] == 'S')
            {
                sensor.SensorValue += 1;
            }
            else if (sensor.SensorId[0] == 'R')
            {
                sensor.SensorValue = (s.Value - 10) * (float)1.17;
            }
            else if (sensor.SensorId[0] == 'P')
            {
                sensor.SensorValue = s.Value - (float)2.54;
            }
            else
            {
                sensor.SensorValue = s.Value;
            }

            sensor.TimeStamp = DateTime.Now;

            var historyItem = new SensorHistory()
            {
                SensorId = sensor.SensorId,
                SensorValue = sensor.SensorValue,
                Timestamp = sensor.TimeStamp
            };

            _ctx.ReplaceOne("{\"SensorId\" : \"" + s.SensorId + "\"}", sensor);
            _history.InsertOne(historyItem);

            return new JsonResult(sensor);
        }

        [HttpPost("api/add_accessory")]
        public async Task<JsonResult> AddAccessoryAsync([FromBody] Sensor s)
        {
            var user = User.Identity;

            var sensor = _ctx.Find("{\"SensorId\" : \"" + s.SensorId + "\"}").FirstOrDefault();
            s.Id = sensor.Id;
            s.ClientName = user.Name;
            s.ClientId = user.Name;

            if (sensor != null)
            {
                _ctx.ReplaceOne("{\"SensorId\" : \"" + s.SensorId + "\"}", s);
                return new JsonResult("OK");
            }
            else
            {
                return new JsonResult("[]");
            }
        }

        [HttpGet("api/validate")]
        public JsonResult ValidateAccessory(string accessoryCode)
        {
            var sensor = _ctx.Find("{\"SensorId\" : \"" + accessoryCode + "\"}").FirstOrDefault();

            if (sensor != null && sensor.SensorName == "")
            {
                return new JsonResult(sensor);
            }
            else
            {
                return new JsonResult("Invalid");
            }
        }

    }

    public class SensorUpdate
    {
		[JsonProperty(PropertyName = "sensorId")]
        public string SensorId { get; set; }
		[JsonProperty(PropertyName = "sensorValue")]
		public float Value { get; set; }
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
