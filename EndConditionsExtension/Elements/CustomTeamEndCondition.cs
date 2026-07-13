using PlayerRoles;
using PlayerRoles.RoleAssign;
using System.Collections.Generic;
using System.ComponentModel;

namespace EndConditionsExtension.Elements
{
    public class CustomTeamEndCondition
    {
        [Description("The custom team identifier this applies to (e.g., 'SerpentHand')")]
        public string CustomTeamId { get; set; } = "SerpentHand";

        [Description("List of base teams that are friendly to this custom team")]
        public List<Team> FriendlyTeams { get; set; } = new()
        {
            Team.ChaosInsurgency,
            Team.SCPs
        };

        [Description("Does this team need to be the only team left alive (along with friendlies)?")]
        public bool MustBeLastStanding { get; set; } = true;

        [Description("Maximum number of non-friendly players allowed alive for the round to end (Requires MustBeLastStanding = false)")]
        public int MaxEnemyPlayers { get; set; } = 0;

        [Description("Which team wins the round when this condition is met?")]
        public LeadingTeam WinningTeam { get; set; } = LeadingTeam.ChaosInsurgency;
    }
}
