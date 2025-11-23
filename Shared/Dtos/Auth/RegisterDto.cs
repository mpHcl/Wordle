using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.Auth
{
    /// <summary>
    /// Represents the data required to register a new user.
    /// </summary>
    public record RegisterDto {
        /// <summary>
        /// Email address used for account creation.
        /// </summary>
        [Required, EmailAddress]
        public string Email { get; init; }

        /// <summary>
        /// Desired username for the new account.
        /// </summary>
        [Required]
        public string UserName { get; init; }

        /// <summary>
        /// Password for the account.
        /// </summary>
        [Required]
        public string Password { get; init; }
    }

}
