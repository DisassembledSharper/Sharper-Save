using MongoDB.Bson;
using MongoDB.Driver;
using System;
using UnityEngine;

namespace SharperSave
{
    public static class MongoDBManager
    {
        private static readonly string _mongoDBURI = "";
        private static ObjectId _objectId = ObjectId.GenerateNewId();

        public static void LoadDB(string jsonData)
        {
            Debug.Log(_objectId.ToString());
            MongoClient mongoClient = new(_mongoDBURI);
            var db = mongoClient.GetDatabase("UsersDB");
            Debug.Log("Getted db");
            var collection = db.GetCollection<BsonDocument>("UsersData");
            Debug.Log("Getted collection");
            var saveDataBson = BsonDocument.Parse(jsonData);
            Debug.Log("Parsed json");
            saveDataBson.SetElement(0, new BsonElement("_id", _objectId));
            var filter = Builders<BsonDocument>.Filter.Eq("_id", _objectId);
            var oldSave = collection.Find(filter).FirstOrDefault();

            if (oldSave != null)
            {
                Debug.Log("Updating");
                collection.ReplaceOne(filter, saveDataBson);
                Debug.Log("Updated");
            }
            else
            {
                Debug.Log("Inserting");
                collection.InsertOne(saveDataBson);
                Debug.Log("Inserted");
            }
        }
    }
}