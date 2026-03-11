using MongoDB.Bson;
using MongoDB.Driver;
using runSyncBackend.Models;

namespace runSyncBackend.Services
{
    public record ServiceResult<T>(bool Success, T? Data, string? Error, int StatusCode)
    {
        public static ServiceResult<T> Ok(T data) => new(true, data, null, 200);
        public static ServiceResult<T> Created(T data) => new(true, data, null, 201);
        public static ServiceResult<T> NotFound(string error) => new(false, default, error, 404);
        public static ServiceResult<T> BadRequest(string error) => new(false, default, error, 400);
    }

    public interface ITrainingResultsService
    {
        Task<ServiceResult<List<TrainingResult>>> GetForTraining(string trainingId);
        Task<ServiceResult<TrainingResult>> CreateForTraining(string trainingId, TrainingResult result);
        Task<ServiceResult<List<TrainingResult>>> GetForAthlete(string athleteId);
        Task<ServiceResult<bool>> UpdateCoachFeedback(string trainingId, string resultId, TrainingResultCoachUpdate update);
    }

    public class TrainingResultsService : ITrainingResultsService
    {
        private readonly IMongoCollection<TrainingResult> _results;
        private readonly IMongoCollection<Training> _trainings;
        private readonly IMongoCollection<Athlete> _athletes;

        public TrainingResultsService(IMongoDatabase database)
        {
            _results = database.GetCollection<TrainingResult>("trainingResults");
            _trainings = database.GetCollection<Training>("trainings");
            _athletes = database.GetCollection<Athlete>("athletes");
        }

        public async Task<ServiceResult<List<TrainingResult>>> GetForTraining(string trainingId)
        {
            var training = await _trainings.Find(t => t.Id == trainingId).FirstOrDefaultAsync();
            if (training == null)
            {
                return ServiceResult<List<TrainingResult>>.NotFound("Training not found.");
            }

            var results = await _results.Find(r => r.TrainingId == trainingId).ToListAsync();
            return ServiceResult<List<TrainingResult>>.Ok(results);
        }

        public async Task<ServiceResult<TrainingResult>> CreateForTraining(string trainingId, TrainingResult result)
        {
            var training = await _trainings.Find(t => t.Id == trainingId).FirstOrDefaultAsync();
            if (training == null)
            {
                return ServiceResult<TrainingResult>.NotFound("Training not found.");
            }

            var athlete = await _athletes.Find(a => a.Id == result.AthleteId).FirstOrDefaultAsync();
            if (athlete == null)
            {
                return ServiceResult<TrainingResult>.BadRequest("Athlete not found.");
            }

            result.Id = string.IsNullOrWhiteSpace(result.Id)
                ? ObjectId.GenerateNewId().ToString()
                : result.Id;
            result.TrainingId = trainingId;
            result.CompletedAt ??= DateTime.UtcNow;

            await _results.InsertOneAsync(result);
            return ServiceResult<TrainingResult>.Created(result);
        }

        public async Task<ServiceResult<List<TrainingResult>>> GetForAthlete(string athleteId)
        {
            var athlete = await _athletes.Find(a => a.Id == athleteId).FirstOrDefaultAsync();
            if (athlete == null)
            {
                return ServiceResult<List<TrainingResult>>.NotFound("Athlete not found.");
            }

            var results = await _results.Find(r => r.AthleteId == athleteId).ToListAsync();
            return ServiceResult<List<TrainingResult>>.Ok(results);
        }

        public async Task<ServiceResult<bool>> UpdateCoachFeedback(string trainingId, string resultId, TrainingResultCoachUpdate update)
        {
            if (update.CoachGrade.HasValue && (update.CoachGrade < 1 || update.CoachGrade > 10))
            {
                return ServiceResult<bool>.BadRequest("CoachGrade must be between 1 and 10.");
            }

            var filter = Builders<TrainingResult>.Filter.Eq(r => r.Id, resultId) &
                         Builders<TrainingResult>.Filter.Eq(r => r.TrainingId, trainingId);

            var updateDef = Builders<TrainingResult>.Update
                .Set(r => r.CoachGrade, update.CoachGrade)
                .Set(r => r.CoachNotes, update.CoachNotes)
                .Set(r => r.ImprovementAreas, update.ImprovementAreas);

            var result = await _results.UpdateOneAsync(filter, updateDef);
            if (result.MatchedCount == 0)
            {
                return ServiceResult<bool>.NotFound("Result not found.");
            }

            return ServiceResult<bool>.Ok(true);
        }
    }
}
