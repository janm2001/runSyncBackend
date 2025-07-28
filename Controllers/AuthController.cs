using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using runSyncBackend.Models;


    
namespace runSyncBackend.Controllers;
   [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMongoCollection<User> _users;

        public AuthController(IMongoDatabase database)
        {
            _users = database.GetCollection<User>("users");
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(User user)
        {
            if (string.IsNullOrEmpty(user.Id))
            {
                user.Id = ObjectId.GenerateNewId().ToString();
            }

            await _users.InsertOneAsync(user);
            return CreatedAtAction(nameof(Register), new { id = user.Id }, user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<User>> Login(string email, string password)
        {
            var user = await _users.Find(u => u.Email == email && u.PasswordHash == password).FirstOrDefaultAsync();
            if (user == null)
            {
                return Unauthorized();
            }
            return user;
        }
    }