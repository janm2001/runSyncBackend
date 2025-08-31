using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using runSyncBackend.Models;

namespace runSyncBackend.Controllers;
    [ApiController]
    [Route("api/[controller]")]
    public class AthletesController : ControllerBase
    {
        private readonly IMongoCollection<Athlete> _athletes;
        private readonly IMongoCollection<Training> _trainings;
    public AthletesController(IMongoDatabase database)
    {
        _athletes = database.GetCollection<Athlete>("athletes");
        _trainings = database.GetCollection<Training>("trainings");
    }

    [HttpGet]
    public async Task<ActionResult<object>> Get(
        string? sortBy = "name", 
        string? sortOrder = "asc",
        string? filterGroup = null,
        int page = 1,
        int pageSize = 10)
    {
        var filterDefinition = Builders<Athlete>.Filter.Empty;
        
        // FILTERING: Filter by group if provided
        if (!string.IsNullOrEmpty(filterGroup))
        {
            filterDefinition = Builders<Athlete>.Filter.Eq(a => a.Group, filterGroup);
        }

        // SORTING: Build sort definition
        var sortDefinition = sortOrder?.ToLower() == "desc" 
            ? Builders<Athlete>.Sort.Descending(sortBy ?? "name")
            : Builders<Athlete>.Sort.Ascending(sortBy ?? "name");

        // Get athletes with filtering and sorting
        var athletes = await _athletes
            .Find(filterDefinition)
            .Sort(sortDefinition)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();

        // CALCULATED FIELD: Calculate age from join date
        // LOOKUP FIELD: Get training count for each athlete
        var enrichedAthletes = new List<object>();

        foreach (var athlete in athletes)
        {
            // CALCULATED: Days since joining
            var daysSinceJoining = (DateTime.Now - DateTime.Parse(athlete.JoinDate)).Days;
            
            // LOOKUP: Count trainings for this athlete
            var trainingCount = await _trainings.CountDocumentsAsync(
                Builders<Training>.Filter.AnyEq("attendance", int.Parse(athlete.Id))
            );

            enrichedAthletes.Add(new
            {
                athlete.Id,
                athlete.Name,
                athlete.Email,
                athlete.Group,
                athlete.JoinDate,
                // CALCULATED FIELDS
                DaysSinceJoining = daysSinceJoining,
                MembershipWeeks = daysSinceJoining / 7,
                // LOOKUP FIELDS
                TrainingCount = trainingCount,
                IsActiveAthlete = trainingCount > 0
            });
        }

        var totalCount = await _athletes.CountDocumentsAsync(filterDefinition);

        return Ok(new
        {
            athletes = enrichedAthletes,
            pagination = new
            {
                currentPage = page,
                pageSize = pageSize,
                totalCount = totalCount,
                totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            },
            sorting = new { sortBy, sortOrder },
            filtering = new { filterGroup }
        });
    }

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