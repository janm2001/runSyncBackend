namespace runSyncBackend.Models
{
    public class TrainingResultCoachUpdate
    {
        public int? CoachGrade { get; set; } // 1-10
        public string? CoachNotes { get; set; }
        public string? ImprovementAreas { get; set; }
    }
}
