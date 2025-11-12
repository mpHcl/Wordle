using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.Auth {
    public class LoginResponseDto {
        public required string Token { get; set; }
        public required SettingsDto Settings { get; set; }
    }
}
