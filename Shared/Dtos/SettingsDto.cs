using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos {
    /// <summary>
    /// Represents user interface and gameplay preference settings.
    /// </summary>
    public class SettingsDto {
        /// <summary>Enables or disables dark theme.</summary>
        public bool DarkMode { get; set; }

        /// <summary>Enables hard mode gameplay rules.</summary>
        public bool HardMode { get; set; }

        /// <summary>Enables high-contrast mode for accessibility.</summary>
        public bool HighContrastMode { get; set; }

        /// <summary>
        /// Determines whether hints should be visible during gameplay.
        /// </summary>
        public bool ShowHints { get; set; }
    }
}
