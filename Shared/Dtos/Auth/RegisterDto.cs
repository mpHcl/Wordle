using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.Auth
{
    public record RegisterDto(string UserName, string Email, string Password);
}
