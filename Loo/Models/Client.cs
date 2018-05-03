using System;
using System.Collections.Generic;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace Loo.Models
{
    public class BuildingAddress
    {
        [JsonProperty(PropertyName = "BuildingStreet")]
        public string BuildingStreet { get; set; }

        [JsonProperty(PropertyName = "BuildingCity")]
        public string BuildingCity { get; set; }

        [JsonProperty(PropertyName = "BuildingState")]
        public string BuildingState { get; set; }

        [JsonProperty(PropertyName = "BuildingZip")]
        public string BuildingZip { get; set; }
    }

    public class Sensor
    {
        public Sensor(string sensorId, string bridgeId)
        {
            SensorId = sensorId;
            BridgeId = bridgeId;
        }

        [JsonProperty(PropertyName = "_id")]
        public ObjectId Id { get; set; }

        [JsonProperty(PropertyName = "ClientName")]
        public string ClientName { get; set; }

        [JsonProperty(PropertyName = "ClientId")]
        public string ClientId { get; set; }

        [JsonProperty(PropertyName = "BuildingName")]
        public string BuildingName { get; set; }

        [JsonProperty(PropertyName = "BuildingCode")]
        public string BuildingCode { get; set; }

        [JsonProperty(PropertyName = "BuildingAddress")]
        public BuildingAddress BuildingAddress { get; set; }

        [JsonProperty(PropertyName = "SensorName")]
        public string SensorName { get; set; }

        [JsonProperty(PropertyName = "SensorId")]
        public string SensorId { get; set; }

        [JsonProperty(PropertyName = "BridgeId")]
        public string BridgeId { get; set; }

        [JsonProperty(PropertyName = "LocationName")]
        public string LocationName { get; set; }

        [JsonProperty(PropertyName = "LocationCode")]
        public string LocationCode { get; set; }

        [JsonProperty(PropertyName = "SensorValue")]
        public float SensorValue { get; set; }

        [JsonProperty(PropertyName = "CMinDist")]
        public float CMinDist { get; set; }

        [JsonProperty(PropertyName = "CInitialDist")]
        public float CInitialDist { get; set; }

        [JsonProperty(PropertyName = "CDiameter")]
        public float CDiameter { get; set; }

        [JsonProperty(PropertyName = "TimeStamp")]
        public DateTime TimeStamp { get; set; }
    }

    public class SensorHistory
    {
        [JsonProperty(PropertyName = "_id")]
        public ObjectId Id { get; set; }

        [JsonProperty(PropertyName = "SensorId")]
        public string SensorId { get; set; }

        [JsonProperty(PropertyName = "BridgeId")]
        public string BridgeId { get; set; }

        [JsonProperty(PropertyName = "SensorValue")]
        public float SensorValue { get; set; }

        [JsonProperty(PropertyName = "Timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty(PropertyName = "Hour")]
        public double Hour { get; set; }
    }
}
