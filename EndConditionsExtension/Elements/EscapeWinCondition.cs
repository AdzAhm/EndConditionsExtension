using PlayerRoles.RoleAssign;
using System.ComponentModel;

namespace EndConditionsExtension.Elements
{
    public class EscapeWinCondition
    {
        [Description("The custom team identifier this applies to (e.g., 'SerpentHand')")]
        public string CustomTeamId { get; set; } = "SerpentHand";

        [Description("How many players of this custom team must escape to trigger the win condition")]
        public int RequiredEscapes { get; set; } = 1;

        [Description("Does at least one SCP need to be alive for the escape to count towards the win condition?")]
        public bool RequireScpAlive { get; set; } = true;

        [Description("How many SCPs must be alive when the required escapes is reached? (Requires RequireScpAlive = true)")]
        public int RequireScpEscortCount { get; set; } = 1;

        [Description("Which team wins the round when this condition is met?")]
        public LeadingTeam WinningTeam { get; set; } = LeadingTeam.ChaosInsurgency;

        [Description("How many points to award the winning team (if scoring is enabled)")]
        public int AwardPoints { get; set; } = 2;

        [Description("Optional broadcast to send to all players when this condition is met (Leave empty to disable)")]
        public string Broadcast { get; set; } = "<color=green>The Serpent's Hand has escaped with an SCP! They win!</color>";
    }
}
