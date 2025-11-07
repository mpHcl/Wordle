namespace Server.Models {
    public class UserAchievements {
        public required string UserId { get; set; }
        public WordleUser? User { get; set; }
        public int AchievementId { get; set; }
        public Achievements? Achievement { get; set; }

        public DateTime DateAchieved { get; set; } = DateTime.UtcNow;
    }
}
