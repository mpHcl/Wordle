using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    public class WordleUser : IdentityUser
    {
        public int SettingsId { get; set; }
        public WordleSettings Settings { get; set; } = new WordleSettings();

        public ICollection<UserAchievements> UserAchievements { get; set; } = new List<UserAchievements>();

        public ICollection<Game> Games { get; set; } = new List<Game>();

        [NotMapped]
        public IEnumerable<Achievements> Achievements => UserAchievements?.Select(ua => ua.Achievement);
    }
}
