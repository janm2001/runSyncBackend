namespace runSyncBackend.Models
{
    public class Training
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public string Group { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public string Duration { get; set; } = string.Empty;

        public string Distance { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public Intervals Intervals { get; set; } = new();


        public int MemberCount { get; set; }
        
        
        
}
 }