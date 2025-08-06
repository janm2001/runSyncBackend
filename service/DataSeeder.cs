
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
        private readonly object _lockObject = new object();
        private int _completedTasks = 0;

        // Event for thread-safe progress reporting (safe for UI updates)
        public event Action<string, int, int>? ProgressUpdated;

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

            _completedTasks = 0;
            var totalTasks = 6;

            // Create tasks for parallel execution - demonstrates safe UI updates from multiple threads
            var tasks = new[]
            {
                SeedCollectionWithProgress<User>("users", root, totalTasks),
                SeedCollectionWithProgress<Athlete>("athletes", root, totalTasks),
                SeedCollectionWithProgress<Group>("groups", root, totalTasks),
                SeedCollectionWithProgress<Training>("trainings", root, totalTasks),
                SeedCollectionWithProgress<MonthlyProgress>("monthlyProgress", root, totalTasks),
                SeedCollectionWithProgress<AttendanceData>("attendanceData", root, totalTasks)
            };

            // Wait for all tasks to complete concurrently
            await Task.WhenAll(tasks);
        }

        private async Task SeedCollectionWithProgress<T>(string collectionName, Root root, int totalTasks)
        {
            var collection = _database.GetCollection<T>(collectionName);
            if (await collection.CountDocumentsAsync(FilterDefinition<T>.Empty) == 0)
            {
                var propertyName = collectionName.Substring(0, 1).ToUpper() + collectionName.Substring(1);
                var propertyInfo = root.GetType().GetProperty(propertyName);
                if (propertyInfo == null)
                {
                    ReportProgress(collectionName, totalTasks);
                    return;
                }
                var value = propertyInfo.GetValue(root);
                var data = value as List<T>;
                if (data != null && data.Count > 0)
                {
                    await collection.InsertManyAsync(data);
                }
            }
            
            // Thread-safe progress reporting - safe for UI updates
            ReportProgress(collectionName, totalTasks);
        }

        // Thread-safe method for reporting progress (Synchronize method equivalent)
        private void ReportProgress(string collectionName, int totalTasks)
        {
            lock (_lockObject) // Ensures thread-safe access
            {
                _completedTasks++;
                // This event can be safely consumed by UI thread using Invoke/BeginInvoke
                ProgressUpdated?.Invoke($"Completed seeding {collectionName}", _completedTasks, totalTasks);
            }
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