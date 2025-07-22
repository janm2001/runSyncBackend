using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("{id:length(24)}", Name = "GetTraining")]
        public async Task<ActionResult<Training>> Get(string id)
        {
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
            await _trainings.InsertOneAsync(training);
            return CreatedAtRoute("GetTraining", new { id = training.Id.ToString() }, training);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Training trainingIn)
        {
            var training = await _trainings.Find<Training>(t => t.Id == id).FirstOrDefaultAsync();
            if (training == null)
            {
                return NotFound();
            }
            await _trainings.ReplaceOneAsync(t => t.Id == id, trainingIn);
            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var training = await _trainings.Find<Training>(t => t.Id == id).FirstOrDefaultAsync();
            if (training == null)
            {
                return NotFound();
            }
            await _trainings.DeleteOneAsync(t => t.Id == id);
            return NoContent();
        }
    }
