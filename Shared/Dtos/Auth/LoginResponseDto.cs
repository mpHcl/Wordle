using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.Auth {
    /// <summary>
    /// Response returned after a successful login.
    /// </summary
    public class LoginResponseDto {
        /// <summary>
        /// JWT token used for authorized API requests.
        /// </summary>
        public required string Token { get; set; }

        /// <summary>
        /// The user's saved settings.
        /// </summary>
        public required SettingsDto Settings { get; set; }
    }
}
