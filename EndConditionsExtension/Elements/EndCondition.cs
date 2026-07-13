using EndConditionsExtension.Structures;
using PlayerRoles;
using PlayerRoles.RoleAssign;
using System.Collections.Generic;
using System.ComponentModel;

namespace EndConditionsExtension.Elements
{
    public class EndCondition : IEndCondition
    {
        [Description("Decide if to end the round must remain only the CustomRole's team, no matter the number")]
        public bool MustRemainOnlyOneTeam { get; set; } = false;

        [Description("If the must_remain_only_one_team bool is false here you can decide which teams are needed to end the round and the maximum number of members that they can have. Leave it empty to allow all roles\n# You don't need to include here the role team")]
        public Dictionary<Team, int> RemainingTeams { get; set; } = new()
        {
            { Team.ClassD, 5 },
            { Team.Scientists, 1 }
        };

        [Description("Here you can decide how many people are needed to keep the round going (this will be effective if the first bool is false) and evaluates all of the roles in the remaining_teams dictionary")]
        public int MaxPlayersToEnd { get; set; }

        [Description("Set the team who will win if this condition will be true")]
        public LeadingTeam WinningTeam { get; set; } = LeadingTeam.Draw;
    }
}
