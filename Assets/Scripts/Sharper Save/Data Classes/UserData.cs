using MongoDB.Bson;
using Realms;
using System;

namespace SharperSave.DataClasses
{
    [Serializable]
    public class UserData : RealmObject
    {
        [MapTo("_id")]
        [PrimaryKey]
        public ObjectId Id { get; set; }

        [MapTo("owner_id")]
        public string OwnerId { get; set; }

        [MapTo("saveData")]
        public string saveData { get; set; }
    }
}
