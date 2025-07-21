
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using runSyncBackend.Models;

namespace runSyncBackend.Controllers
{
   [ApiController]
   [Route("api/[controller]")]
    public class AttendanceDataController : ControllerBase
    {
        private readonly IMongoCollection<AttendanceData> _attendanceData;

        public AttendanceDataController(IMongoDatabase database)
        {
            _attendanceData = database.GetCollection<AttendanceData>("attendanceData");
        }

        [HttpGet]
        public async Task<List<AttendanceData>> Get() =>
            await _attendanceData.Find(data => true).ToListAsync();
    }
}