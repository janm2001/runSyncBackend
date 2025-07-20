using Microsoft.AspNetCore.Mvc;
using runSyncBackend.Models;

namespace runSyncBackend.Controllers;


[ApiController]
[Route("[controller]")]
public class TrainingController : ControllerBase
{
    

    private readonly ILogger<TrainingController> _logger;

    public TrainingController(ILogger<TrainingController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetTraining")]
    public IEnumerable<Training> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new Training
        {
            Date = DateTime.Now.AddDays(index),
            Description = "Sample Training",
            Distance = "5km",
            Duration = "30 minutes",
            Group = "Beginner",
            Intervals = new Intervals
            {
                Id = index,
                distance = "400m",
                repetitions = 4,
                rest = "2 minutes",
                targetPace = "4:30/km"
            },
            MemberCount = 10,
            Title = "Training Session " + index,
            Type = "Interval Training",
            Id = index
            
            
        })
        .ToArray();
     }
   
}
