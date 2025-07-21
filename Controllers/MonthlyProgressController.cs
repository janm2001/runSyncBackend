
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using runSyncBackend.Models;
namespace runSyncBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MonthlyProgressController : ControllerBase
    {
        private readonly IMongoCollection<MonthlyProgress> _monthlyProgress;

        public MonthlyProgressController(IMongoDatabase database)
        {
            _monthlyProgress = database.GetCollection<MonthlyProgress>("monthlyProgress");
        }

        [HttpGet]
        public async Task<List<MonthlyProgress>> Get() =>
            await _monthlyProgress.Find(progress => true).ToListAsync();
    }
}