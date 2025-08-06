
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
            // Read JSON file once to avoid multiple file I/O operations
            var jsonData = File.ReadAllText("data.json");
            var root = JsonConvert.DeserializeObject<Root>(jsonData);
            if (root == null)
            {
                return;
            }

            var tasks = new[]
            {
                SeedCollection<User>("users", root),
                SeedCollection<Athlete>("athletes", root),
                SeedCollection<Group>("groups", root),
                SeedCollection<Training>("trainings", root),
                SeedCollection<MonthlyProgress>("monthlyProgress", root),
                SeedCollection<AttendanceData>("attendanceData", root)
            };

            await Task.WhenAll(tasks);
        }

        private async Task SeedCollection<T>(string collectionName, Root root)
        {
            var collection = _database.GetCollection<T>(collectionName);
            if (await collection.CountDocumentsAsync(FilterDefinition<T>.Empty) == 0)
            {
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