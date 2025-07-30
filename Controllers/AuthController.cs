using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using runSyncBackend.Models;
using Microsoft.AspNetCore.Authorization;



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
    [AllowAnonymous] 
        public async Task<ActionResult<User>> Login([FromBody] LoginRequest request)
    {

        var user = await _users.Find(u => u.Email == request.Email).FirstOrDefaultAsync();
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
            return Unauthorized("Invalid email or password.");
        }
        return Ok(user);
    }
    }