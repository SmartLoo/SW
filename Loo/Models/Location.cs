using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Loo.Models
{
    public class Location
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "building")]
        public string Building { get; set; }

        [JsonProperty(PropertyName = "location")]
        public string LocationName { get; set; }

        [JsonProperty(PropertyName = "locationId")]
        public string LocationId { get; set; }

        [JsonProperty(PropertyName = "sensors")]
        public List<Sensor> Sensors { get; set; }
    }

    public class Sensor
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        [JsonProperty(PropertyName = "status")]
        public Status Status { get; set; }
        [JsonProperty(PropertyName = "sensorHistory")]
        public List<Status> SensorHistory { get; set; }
    }

    public class Status
    {
        [JsonProperty(PropertyName = "value")]
        public float Value { get; set; }
        [JsonProperty(PropertyName = "timestamp")]
        public DateTime? TimeStamp { get; set; }
        [JsonProperty(PropertyName = "battery")]
        public float Battery { get; set; }
    }
}
