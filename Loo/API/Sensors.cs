using System;
using System.Threading.Tasks;
using Loo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.Client;
using Loo;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Loo.API
{
    public class Sensors : Controller
    {
        private DocumentClient _client;
        private Uri _locationCollectionUri;

        public Sensors()
        {
            _client = new DocumentClient(new Uri(Constants.CosmosEndpoint), Constants.CosmosKey);
            _locationCollectionUri = UriFactory.CreateDocumentCollectionUri(Constants.LooDb, Constants.LocationCollection);
        }

        /// <summary>
        /// Retrieve all locations where sensors exist.
        /// </summary>
        /// <returns>The locations.</returns>
        [HttpGet("api/locations")]
        public JsonResult GetLocations()
        {
            var locations = _client.CreateDocumentQuery<Location>(
                _locationCollectionUri).ToList();
            return new JsonResult(locations);
        }

        /// <summary>
        /// Retrieve sensor by ID and location.
        /// </summary>
        /// <returns>The sensor.</returns>
        [HttpGet("api/sensor/{building}/{location}/{id}")]
        public JsonResult GetSensor(string location, string id, string building)
        {
			var loc = _client.CreateDocumentQuery<Location>(
				_locationCollectionUri)
                             .Where(x => x.LocationName == location && x.Building == building)
						 .ToList()
						 .FirstOrDefault();

			if (loc == null)
				return new JsonResult(null);

			var sen = loc.Sensors
                         .Where(x => x.Id == id)
					 .ToList()
					 .FirstOrDefault();

			if (sen == null)
				return new JsonResult(null);
            
            return new JsonResult(sen);
        }

        [HttpPost("/api/sensor")]
        public JsonResult AddSensor([FromBody] SensorReq sensor)
        {
            return new JsonResult("Sensor added.");
        }

        /// <summary>
        /// Update current sensor value and battery life.
        /// </summary>
        /// <returns>Update document.</returns>
        /// <param name="sensor">Request containing the sensor id, property name, location name, value, and battery.</param>
        [HttpPost("/api/sensor_data/update")]
        public async Task<JsonResult> UpdateSensorDataAsync([FromBody] SensorReq sensor)
        {
            var loc = _client.CreateDocumentQuery<Location>(
                _locationCollectionUri)
                             .Where(x => x.LocationName == sensor.LocationName && x.Building == sensor.Building)
                         .ToList()
                         .FirstOrDefault();

            if (loc == null)
                return new JsonResult(null);

            var sen = loc.Sensors
                     .Where(x => x.Id == sensor.SensorId)
                     .ToList()
                     .FirstOrDefault();

            if (sen == null)
                return new JsonResult(null);

            if (sen.SensorHistory == null)
            {
                sen.SensorHistory = new List<Status>();
            }

            sen.Status.Value = sensor.Value;
            sen.Status.TimeStamp = DateTime.Now;
            sen.Status.Battery = sensor.Battery;
            sen.SensorHistory.Add(sen.Status);

            var response = await _client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(Constants.LooDb, Constants.LocationCollection, loc.Id), loc);

            return new JsonResult("OK");
        }
    }

    public class SensorReq
    {
		[JsonProperty(PropertyName = "sensorId")]
		public string SensorId { get; set; }
		[JsonProperty(PropertyName = "building")]
		public string Building { get; set; }
		[JsonProperty(PropertyName = "location")]
		public string LocationName { get; set; }
		[JsonProperty(PropertyName = "value")]
		public float Value { get; set; }
		[JsonProperty(PropertyName = "battery")]
		public float Battery { get; set; }
    }
}
