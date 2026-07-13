using PlayerRoles;
using PlayerRoles.RoleAssign;
using System.Collections.Generic;

namespace EndConditionsExtension.Structures
{
    public interface IEndCondition
    {
        bool MustRemainOnlyOneTeam { get; set; }
        Dictionary<Team, int> RemainingTeams { get; set; }
        int MaxPlayersToEnd { get; set; }
        LeadingTeam WinningTeam { get; set; }
    }
}
