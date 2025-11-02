namespace Server.Models
{
    public class WordleSettings
    {
        public int Id { get;set; }
        public bool DarkMode { get; set; } = false;
        public bool HardMode { get; set; } = false;
        public bool HighContrastMode { get; set; } = false;
        public bool ShowHints { get; set; } = true;
    }
}
