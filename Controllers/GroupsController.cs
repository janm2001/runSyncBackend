using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using runSyncBackend.Models;
namespace runSyncBackend.Controllers;

    [ApiController]
    [Route("api/[controller]")]
    public class GroupsController : ControllerBase
    {
        private readonly IMongoCollection<Group> _groups;

        public GroupsController(IMongoDatabase database)
        {
            _groups = database.GetCollection<Group>("groups");
        }

        [HttpGet]
        public async Task<List<Group>> Get() =>
            await _groups.Find(group => true).ToListAsync();

        [HttpGet("{id:length(24)}", Name = "GetGroup")]
        public async Task<ActionResult<Group>> Get(string id)
        {
            var group = await _groups.Find<Group>(g => g.Id == id).FirstOrDefaultAsync();
            if (group == null)
            {
                return NotFound();
            }
            return group;
        }

        [HttpPost]
        public async Task<ActionResult<Group>> Create(Group group)
        {
            await _groups.InsertOneAsync(group);
            return CreatedAtRoute("GetGroup", new { id = group.Id.ToString() }, group);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Group groupIn)
        {
            var group = await _groups.Find<Group>(g => g.Id == id).FirstOrDefaultAsync();
            if (group == null)
            {
                return NotFound();
            }
            await _groups.ReplaceOneAsync(g => g.Id == id, groupIn);
            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var group = await _groups.Find<Group>(g => g.Id == id).FirstOrDefaultAsync();
            if (group == null)
            {
                return NotFound();
            }
            await _groups.DeleteOneAsync(g => g.Id == id);
            return NoContent();
        }
    }