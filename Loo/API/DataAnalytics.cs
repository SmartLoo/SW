using System;
using System.Collections.Generic;
using Loo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using RestSharp;

namespace Loo.API
{
    public class DataAnalytics : Controller
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _db;
        IMongoCollection<Sensor> _ctx;
        IMongoCollection<SensorHistory> _history;

        public DataAnalytics()
        {
            _client = new MongoClient(Constants.MongoConnectionString);
            _db = _client.GetDatabase(Constants.MongoDatabase);
            _ctx = _db.GetCollection<Sensor>("Sensors");
            _history = _db.GetCollection<SensorHistory>("SensorHistory");
        }

        [HttpGet("api/analyze")]
        public JsonResult AnalyzeRestroom(string bridgeId)
        {
            var sensorHistory = _history.Find(x => x.BridgeId == bridgeId).ToList();

            CRSensorData sensorData = new CRSensorData();
            sensorData.Values = new List<List<string>>();
            sensorData.ColumnNames = new List<string>() { "SensorValue", "Hour" };

            foreach (SensorHistory sh in sensorHistory)
            {
                sensorData.Values.Add(new List<string>(){sh.SensorValue.ToString(), sh.Hour.ToString()});
            }

            CRBody body = new CRBody()
            {
                SensorData = sensorData
            };

            ClusterRequest clusterRequest = new ClusterRequest()
            {
                Inputs = body
            };

            var client = new RestClient("https://ussouthcentral.services.azureml.net/workspaces/ac4377bbf823443da6029323fa81c238/services/f7c0fdcf17b448d881ff7bdcf2c37a3c/execute?api-version=2.0&details=true");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("authorization", "Bearer 5FEo9jrAUwr0XSb1H1YYVKGN+1W8bm7uBlrONEu3qgcNMjgaPVVWkanWkgFlFFYKZk3MWYcIHgEi9ICUNc4TbQ==");
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", clusterRequest, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);


            return new JsonResult(clusterRequest);
        }

        [HttpGet("api/convert")]
        public JsonResult ConvertValues()
        {
            var sensorHistory = _history.Find("{}").ToList();

            foreach (SensorHistory h in sensorHistory)
            {
                var updateDef = Builders<SensorHistory>.Update.Set(o => o.Hour, h.Timestamp.Hour);
                _history.UpdateOne(o => o.Id == h.Id, updateDef);
            }


            return new JsonResult("OK");
        }
    }

}

public class ClusterRequest
{
    public CRBody Inputs;
}

public class CRBody
{
    public CRSensorData SensorData;
}

public class CRSensorData
{
    public List<string> ColumnNames;
    public List<List<string>> Values;
}

