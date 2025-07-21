
namespace runSyncBackend.service {
using MongoDB.Driver;
using Newtonsoft.Json;
    using runSyncBackend.Models;
    using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RunSync.Services
{
    public class DataSeeder
    {
        private readonly IMongoDatabase _database;

        public DataSeeder(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task SeedData()
        {
            await SeedCollection<Athlete>("athletes");
            await SeedCollection<Group>("groups");
            await SeedCollection<Training>("trainings");
            await SeedCollection<MonthlyProgress>("monthlyProgress");
            await SeedCollection<AttendanceData>("attendanceData");
        }

        private async Task SeedCollection<T>(string collectionName)
        {
            var collection = _database.GetCollection<T>(collectionName);
            if (await collection.CountDocumentsAsync(FilterDefinition<T>.Empty) == 0)
            {
                var jsonData = File.ReadAllText("data.json");
                var root = JsonConvert.DeserializeObject<Root>(jsonData);
                if (root == null)
                {
                    return;
                }
                var propertyName = collectionName.Substring(0, 1).ToUpper() + collectionName.Substring(1);
                var propertyInfo = root.GetType().GetProperty(propertyName);
                if (propertyInfo == null)
                {
                    return;
                }
                var value = propertyInfo.GetValue(root);
                var data = value as List<T>;
                if (data != null && data.Count > 0)
                {
                    await collection.InsertManyAsync(data);
                }
            }
        }
    }
}

}