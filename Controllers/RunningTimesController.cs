using Microsoft.AspNetCore.Mvc;
using runSyncBackend.Models;
using System.Text.Json;

namespace runSyncBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RunningTimesController : ControllerBase
{
    [HttpGet("data")]
    public IActionResult GetRunningTimesData()
    {
        var baseData = new List<RunningTime>
        {
            new RunningTime("5K (3.1 miles)", "Beginner", "00:35:00"),
            new RunningTime("5K (3.1 miles)", "Intermediate", "00:25:00"),
            new RunningTime("5K (3.1 miles)", "Advanced", "00:20:00"),
            new RunningTime("5K (3.1 miles)", "Elite", "00:15:00"),
            new RunningTime("10K (6.2 miles)", "Beginner", "01:15:00"),
            new RunningTime("10K (6.2 miles)", "Intermediate", "00:55:00"),
            new RunningTime("10K (6.2 miles)", "Advanced", "00:43:00"),
            new RunningTime("10K (6.2 miles)", "Elite", "00:32:00"),
            new RunningTime("Half Marathon", "Beginner", "02:45:00"),
            new RunningTime("Half Marathon", "Intermediate", "02:10:00"),
            new RunningTime("Half Marathon", "Advanced", "01:45:00"),
            new RunningTime("Half Marathon", "Elite", "01:15:00"),
            new RunningTime("Marathon", "Beginner", "05:30:00"),
            new RunningTime("Marathon", "Intermediate", "04:30:00"),
            new RunningTime("Marathon", "Advanced", "03:30:00"),
            new RunningTime("Marathon", "Elite", "02:45:00")
        };

       
        const int repeatCount = 50000;
        var largeData = Enumerable.Repeat(baseData, repeatCount).SelectMany(list => list).ToList();

        // Serialize to a byte array to correctly set Content-Length
        var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(largeData);

        // Enable CORS for the React app (see Program.cs configuration)
        Response.Headers.Append("Access-Control-Allow-Origin", "*");

        return File(jsonBytes, "application/json", "running_times.json");
    }
}