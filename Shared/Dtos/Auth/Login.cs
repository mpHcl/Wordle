using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.Auth
{
    /// <summary>
    /// Contains credentials required to authenticate a user.
    /// </summary>
    /// <param name="Email">User's email address.</param>
    /// <param name="Password">User's password.</param>
    public record LoginDto(string Email, string Password);
}
