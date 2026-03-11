using Microsoft.AspNetCore.Mvc;
using runSyncBackend.Models;
using runSyncBackend.Services;

namespace runSyncBackend.Controllers;

[ApiController]
public class TrainingResultsController : ControllerBase
{
    private readonly ITrainingResultsService _trainingResultsService;

    public TrainingResultsController(ITrainingResultsService trainingResultsService)
    {
        _trainingResultsService = trainingResultsService;
    }

    [HttpGet("api/trainings/{trainingId}/results")]
    public async Task<ActionResult<List<TrainingResult>>> GetForTraining(string trainingId)
    {
        var result = await _trainingResultsService.GetForTraining(trainingId);
        if (!result.Success)
        {
            return StatusCode(result.StatusCode, result.Error);
        }
        return Ok(result.Data);
    }

    [HttpPost("api/trainings/{trainingId}/results")]
    public async Task<ActionResult<TrainingResult>> CreateForTraining(string trainingId, TrainingResult result)
    {
        var serviceResult = await _trainingResultsService.CreateForTraining(trainingId, result);
        if (!serviceResult.Success)
        {
            return StatusCode(serviceResult.StatusCode, serviceResult.Error);
        }

        return CreatedAtAction(nameof(GetForTraining), new { trainingId }, serviceResult.Data);
    }

    [HttpGet("api/athletes/{athleteId}/results")]
    public async Task<ActionResult<List<TrainingResult>>> GetForAthlete(string athleteId)
    {
        var result = await _trainingResultsService.GetForAthlete(athleteId);
        if (!result.Success)
        {
            return StatusCode(result.StatusCode, result.Error);
        }
        return Ok(result.Data);
    }

    [HttpPut("api/trainings/{trainingId}/results/{resultId}")]
    public async Task<IActionResult> UpdateCoachFeedback(
        string trainingId,
        string resultId,
        [FromBody] TrainingResultCoachUpdate update)
    {
        var result = await _trainingResultsService.UpdateCoachFeedback(trainingId, resultId, update);
        if (!result.Success)
        {
            return StatusCode(result.StatusCode, result.Error);
        }
        return NoContent();
    }
}
