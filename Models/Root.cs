
using runSyncBackend.Models;

public class Root
    {
        public List<User> Users { get; set; } = new List<User>();
        public List<Athlete> Athletes { get; set; } = new List<Athlete>();
        public List<Group> Groups { get; set; } = new List<Group>();
        public List<Training> Trainings { get; set; } = new List<Training>();
        public List<MonthlyProgress> MonthlyProgress { get; set; } = new List<MonthlyProgress>();
        public List<AttendanceData> AttendanceData { get; set; } = new List<AttendanceData>();
    }