using System;
using System.Collections.Generic;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace Loo.Models
{
    public class Client
    {
        [JsonProperty(PropertyName = "_id")]
        public ObjectId Id { get; set; }

        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "Phone")]
        public string Phone { get; set; }

        [JsonProperty(PropertyName = "Email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "Buildings")]
        public List<Building> Buildings { get; set; }
    }

    public class Building
    {
        [JsonProperty(PropertyName = "BuildingName")]
        public string BuildingName { get; set; }

        [JsonProperty(PropertyName = "BuildingCode")]
        public string BuildingCode { get; set; }

        [JsonProperty(PropertyName = "StreetAddress")]
        public string StreetAddress { get; set; }

        [JsonProperty(PropertyName = "City")]
        public string City { get; set; }

        [JsonProperty(PropertyName = "State")]
        public string State { get; set; }

        [JsonProperty(PropertyName = "Zip")]
        public string Zip { get; set; }

        [JsonProperty(PropertyName = "Restrooms")]
        public List<Restroom> Restrooms { get; set; }
    }

    public class Restroom
    {
        [JsonProperty(PropertyName = "RestroomName")]
        public string RestroomName { get; set; }

        [JsonProperty(PropertyName = "RestroomCode")]
        public string RestroomCode { get; set; }

        [JsonProperty(PropertyName = "Sensors")]
        public List<Sensor> Sensors { get; set; }
    }

    public class Sensor
    {
        [JsonProperty(PropertyName = "SensorName")]
        public string SensorName { get; set; }

        [JsonProperty(PropertyName = "SensorId")]
        public string SensorId { get; set; }

        [JsonProperty(PropertyName = "SensorValue")]
        public float SensorValue { get; set; }

        [JsonProperty(PropertyName = "SensorBattery")]
        public float SensorBattery { get; set; }
    }
}
