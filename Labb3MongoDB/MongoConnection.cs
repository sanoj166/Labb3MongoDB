using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Labb3MongoDB
{
    public static class MongoConnection
    {
        private static IMongoDatabase _database;

        public static IMongoDatabase Database
        {
            get
            {
                if (_database == null)
                {
                    MongoClientSettings settings = MongoClientSettings.FromUrl(
                        new MongoUrl("mongodb+srv://jhmongodb:jonas123@cluster0.ogwu5xe.mongodb.net/")
                    );

                    MongoClient client = new MongoClient(settings);
                    _database = client.GetDatabase("StoreDatabase");
                }
                return _database;
            }
        }
    }
}