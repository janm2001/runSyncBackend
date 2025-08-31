using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using runSyncBackend.Models;


    
namespace runSyncBackend.Controllers;

   [ApiController]
    [Route("api/[controller]")]
    public class TrainingsController : ControllerBase
    {

        private readonly IMongoCollection<Training> _trainings;
    
    private static readonly Mutex _trainingMutex = new Mutex(false, "TrainingCreationMutex");
    
    private static readonly object _attendanceLock = new object();
    
    private static int _trainingCounter = 0;

        public TrainingsController(IMongoDatabase database)
        {
            _trainings = database.GetCollection<Training>("trainings");
        }

        [HttpGet]
        public async Task<List<Training>> Get() =>
            await _trainings.Find(training => true).ToListAsync();

        [HttpGet("{id}", Name = "GetTraining")]
        public async Task<ActionResult<Training>> Get(string id)
        {
            // The custom serializer handles the query correctly now.
            var training = await _trainings.Find<Training>(t => t.Id == id).FirstOrDefaultAsync();
            if (training == null)
            {
                return NotFound();
            }
            return training;
        }

        [HttpPost]
    public async Task<ActionResult<Training>> Create(Training training)
    {
        // MUTEX DEMONSTRATION: Ensure only one training is created at a time
        bool mutexAcquired = false;
        try
        {
            mutexAcquired = _trainingMutex.WaitOne(TimeSpan.FromSeconds(10));
            if (!mutexAcquired)
            {
                return StatusCode(408, "Training creation timeout - another process is creating training");
            }

            if (string.IsNullOrEmpty(training.Id))
            {
                training.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
            }

            // Simulate some processing time
            await Task.Delay(100);
            
            Interlocked.Increment(ref _trainingCounter);
            
            await _trainings.InsertOneAsync(training);
            
            return CreatedAtRoute("GetTraining", new { id = training.Id }, new 
            { 
                training = training,
                creationOrder = _trainingCounter,
                message = "Training created using MUTEX protection"
            });
        }
        finally
        {
            if (mutexAcquired)
            {
                _trainingMutex.ReleaseMutex();
            }
        }
    }

    [HttpPost("{id}/attendance")]
    public async Task<ActionResult> UpdateAttendance(string id, [FromBody] int athleteId)
    {
        return await Task.Run(() =>
        {
            lock (_attendanceLock)
            {
                var training = _trainings.Find(t => t.Id == id).FirstOrDefault();
                if (training == null)
                {
                    return NotFound();
                }

                Thread.Sleep(50);

                if (!training.Attendance.Contains(athleteId))
                {
                    training.Attendance.Add(athleteId);
                }

                var updateResult = _trainings.ReplaceOne(t => t.Id == id, training);

                return Ok(new
                {
                    message = "Attendance updated using CRITICAL SECTION protection",
                    athleteId = athleteId,
                    totalAttendees = training.Attendance.Count
                }) as ActionResult;
            }
        });
    }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Training trainingIn)
        {
            // Ensure the ID in the body matches the ID in the route
            trainingIn.Id = id;
            var result = await _trainings.ReplaceOneAsync(t => t.Id == id, trainingIn);

            if (result.MatchedCount == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            // The custom serializer ensures this query works for both "2" and a 24-char ID.
            var result = await _trainings.DeleteOneAsync(t => t.Id == id);

            if (result.DeletedCount == 0)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
