using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using runSyncBackend.Models;

namespace RunSync.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AthletesController : ControllerBase
    {
        private readonly IMongoCollection<Athlete> _athletes;

        public AthletesController(IMongoDatabase database)
        {
            _athletes = database.GetCollection<Athlete>("athletes");
        }

        [HttpGet]
        public async Task<List<Athlete>> Get() =>
            await _athletes.Find(athlete => true).ToListAsync();

        [HttpGet("{id:length(24)}", Name = "GetAthlete")]
        public async Task<ActionResult<Athlete>> Get(string id)
        {
            var athlete = await _athletes.Find<Athlete>(a => a.Id == id).FirstOrDefaultAsync();
            if (athlete == null)
            {
                return NotFound();
            }
            return athlete;
        }

        [HttpPost]
        public async Task<ActionResult<Athlete>> Create(Athlete athlete)
        {
            await _athletes.InsertOneAsync(athlete);
            return CreatedAtRoute("GetAthlete", new { id = athlete.Id.ToString() }, athlete);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Athlete athleteIn)
        {
            var athlete = await _athletes.Find<Athlete>(a => a.Id == id).FirstOrDefaultAsync();
            if (athlete == null)
            {
                return NotFound();
            }
            await _athletes.ReplaceOneAsync(a => a.Id == id, athleteIn);
            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var athlete = await _athletes.Find<Athlete>(a => a.Id == id).FirstOrDefaultAsync();
            if (athlete == null)
            {
                return NotFound();
            }
            await _athletes.DeleteOneAsync(a => a.Id == id);
            return NoContent();
        }
    }
}