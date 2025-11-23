using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos {
    /// <summary>
    /// Represents information about the daily challenge assigned to the user.
    /// </summary>
    public record DailyChallengeDto {
        /// <summary>
        /// Identifier of the current daily challenge.
        /// </summary>
        public int ChallengeId { get; init; }
    };
}
