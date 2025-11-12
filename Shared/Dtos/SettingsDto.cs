using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos {
    public class SettingsDto {
        public bool DarkMode { get; set; }
        public bool HardMode { get; set; }
        public bool HighContrastMode { get; set; }
        public bool ShowHints { get; set; }
}
}
