using EndConditionsExtension.Elements;
using System.Collections.Generic;
using System.ComponentModel;

namespace EndConditionsExtension
{
    public class Config
    {
        [Description("Is the plugin enabled?")]
        public bool IsEnabled { get; set; } = true;

        [Description("Enable debug (developer) mode?")]
        public bool Debug { get; set; } = false;

        [Description("Traditional per-role-ID end conditions (Key = Role ID)")]
        public Dictionary<int, EndCondition> EndConditions { get; set; } = new()
        {
            { 1, new EndCondition() }
        };

        [Description("Custom-team based end conditions (Key = CustomTeamId name)")]
        public Dictionary<string, CustomTeamEndCondition> CustomTeamEndConditions { get; set; } = new()
        {
            { "SerpentHand", new CustomTeamEndCondition() }
        };

        [Description("Escape-based win conditions (Key = Identifier)")]
        public Dictionary<string, EscapeWinCondition> EscapeWinConditions { get; set; } = new()
        {
            { "SerpentHandEscape", new EscapeWinCondition() }
        };
    }
}
