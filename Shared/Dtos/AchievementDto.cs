using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos {
    /// <summary>
    /// Represents a single achievement available in the game.
    /// </summary>
    public class AchievementDto {
        /// <summary>
        /// Unique identifier of the achievement.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Display name of the achievement.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Description explaining how the achievement is earned.
        /// </summary>
        public required string Description { get; set; }

        /// <summary>
        /// Indicates whether the current user has completed the achievement.
        /// </summary>
        public bool? Achieved { get; set; }

        /// <summary>
        /// Percentage of users who have earned this achievement.
        /// </summary>
        public double? PercentOfUsers { get; set; }
    }
}
