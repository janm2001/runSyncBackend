namespace runSyncBackend.Models
{
    public class Athlete
{
    public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;
        public DateTime JoinDate { get; set; }
        public int Performance { get; set; }
        public string Improvement { get; set; } = string.Empty;
        public string LastRun { get; set; } = string.Empty;
        public PersonalBests PersonalBests { get; set; } = new();
        public int Attendance { get; set; }
}
 }

