﻿using System;
using System.Threading.Tasks;
using Loo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using MongoDB.Driver;
using Microsoft.AspNetCore.Identity;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Loo.API
{
    public class Sensors : Controller
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _db;
        IMongoCollection<Sensor> _ctx;
        IMongoCollection<SensorHistory> _history;
        private readonly UserManager<AuthenticatedUser> _userManager;

        public Sensors(UserManager<AuthenticatedUser> userManager)
        {
            _client = new MongoClient(Constants.MongoConnectionString);
            _db = _client.GetDatabase(Constants.MongoDatabase);
            _ctx = _db.GetCollection<Sensor>("Sensors");
            _history = _db.GetCollection<SensorHistory>("SensorHistory");
            _userManager = userManager;
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
        public JsonResult GetSensorByBridge(string bridgeId, string sensorId)
        {
            var restroom = _ctx.Find(x => x.BridgeId == bridgeId && x.SensorId != sensorId && x.BuildingName != null)
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
        public async Task<JsonResult> UpdateSensorAsync([FromBody] SensorUpdate s)
        {
            var user = User.Identity;

            AuthenticatedUser acc = new AuthenticatedUser();

            if (user.Name != null)
            {
                acc = await _userManager.FindByEmailAsync(user.Name);
            }
            else {
                acc = await _userManager.FindByEmailAsync("bcorn@bu.edu");
            }
             
            var sensor = _ctx.Find("{\"SensorId\" : \"" + s.SensorId + "\"}").FirstOrDefault();

            if (sensor == null)
            {
                sensor = new Sensor(s.SensorId, s.BridgeId);         
            }

            if (sensor.SensorName != null)
            {
                switch (sensor.SensorId[0])
                {
                    case 'S':
                        sensor.SensorValue = sensor.SensorValue + s.Value;
                        break;
                    case 'T':
                        sensor.SensorValue = (1 - (((sensor.CInitialDist - sensor.CMinDist) - s.Value) / (sensor.CInitialDist - sensor.CMinDist))) * 100;
                        break;
                    case 'P':
                        sensor.SensorValue = (1 - (((sensor.CDiameter - sensor.CMinDist) - (s.Value - sensor.CInitialDist)) / (sensor.CDiameter - sensor.CMinDist))) * 100;
                        break;
                }
            }
            else
            {
                sensor.SensorValue = 0;
            }

            //BRANDON EDIT TEST
            if (sensor.SensorId[0] == 'L' && acc != null)
            {
                const string accountSid = "AC1d76e73d266055af1367304012973fa3";
                const string authToken = "6a50629c95d7c85089b209b120df0bb3";
                TwilioClient.Init(accountSid, authToken);

                var to = new PhoneNumber("+1" + acc.Phone);
                var message = MessageResource.Create(
                    to,
                    from: new PhoneNumber("+16507795970"),
                    body: "A spill has been detected in " + sensor.BuildingName + " in the " + sensor.LocationName + " at " + sensor.SensorName);

                Console.WriteLine(message.Sid);
            }

            sensor.TimeStamp = DateTime.Now;

            var historyItem = new SensorHistory()
            {
                SensorId = sensor.SensorId,
                BridgeId = sensor.BridgeId,
                SensorValue = sensor.SensorValue,
                Timestamp = sensor.TimeStamp,
                Hour = sensor.TimeStamp.Hour
            };

            if (sensor.Id.Pid == 0)
                _ctx.InsertOne(sensor);
            else
                _ctx.ReplaceOne("{\"SensorId\" : \"" + s.SensorId + "\"}", sensor);

            if (sensor.SensorName != null)
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

        [HttpGet("api/sensor_data")]
        public JsonResult GetSensorData(string bridgeId, string sensorId)
        {
            var sensorData = _history.Find(x => x.BridgeId == bridgeId && x.SensorId == sensorId && x.Timestamp >= DateTime.Now.AddDays(-7)).ToList();
            return new JsonResult(sensorData);
        }

        [HttpGet("api/validate")]
        public JsonResult ValidateAccessory(string accessoryCode)
        {
            var sensor = _ctx.Find("{\"SensorId\" : \"" + accessoryCode + "\"}").FirstOrDefault();

            if (sensor != null && sensor.SensorName == null)
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
        [JsonProperty(PropertyName = "bridgeId")]
        public string BridgeId { get; set; }
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
