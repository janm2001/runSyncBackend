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
            // If an ID isn't provided, generate a new ObjectId string for it.
            if (string.IsNullOrEmpty(training.Id))
            {
                training.Id = ObjectId.GenerateNewId().ToString();
            }
            
            await _trainings.InsertOneAsync(training);

            return CreatedAtRoute("GetTraining", new { id = training.Id }, training);
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
