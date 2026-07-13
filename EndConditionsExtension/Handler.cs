using EndConditionsExtension.Elements;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Features.Wrappers;
using PlayerRoles;
using PlayerRoles.RoleAssign;
using System.Collections.Generic;
using System.Linq;
using UncomplicatedCustomRoles.API.Features;

namespace EndConditionsExtension
{
    internal class Handler
    {
        // Tracks how many escapes have occurred per CustomTeamId in the current round
        private Dictionary<string, int> _escapeCounts = new();

        public void OnWaitingForPlayers()
        {
            // Reset state for the new round
            _escapeCounts.Clear();
        }

        public void OnCustomRoleEscaped(object sender, UcrEvents.CustomRoleEscapeEventData ev)
        {
            if (string.IsNullOrEmpty(ev.CustomTeamId))
                return;

            if (!_escapeCounts.ContainsKey(ev.CustomTeamId))
                _escapeCounts[ev.CustomTeamId] = 0;

            _escapeCounts[ev.CustomTeamId]++;

            // Check if this escape triggers an escape-based win condition
            EvaluateEscapeWinConditions();
        }

        private void EvaluateEscapeWinConditions()
        {
            if (Plugin.Instance.Config.EscapeWinConditions == null)
                return;

            foreach (var kvp in Plugin.Instance.Config.EscapeWinConditions)
            {
                var condition = kvp.Value;
                if (!_escapeCounts.TryGetValue(condition.CustomTeamId, out int escapes) || escapes < condition.RequiredEscapes)
                    continue; // Not enough escapes

                if (condition.RequireScpAlive)
                {
                    int scpCount = Player.GetPlayers().Count(p => p.Role.Team == Team.SCPs);
                    if (scpCount < condition.RequireScpEscortCount)
                        continue; // Not enough SCPs alive
                }

                // Condition met! End the round.
                LabApi.Features.Server.Logger.Info($"Escape win condition '{kvp.Key}' met for team '{condition.CustomTeamId}'. Ending round.");

                if (!string.IsNullOrEmpty(condition.Broadcast))
                {
                    foreach (var player in Player.GetPlayers())
                        player.SendBroadcast(condition.Broadcast, 10);
                }

                // Note: LabAPI doesn't have a direct "Force End Round with specific winner" method in this API version
                // without reflection or using the RoundSummary object directly.
                // The easiest way to force a round end is to use RoundSummary.singleton.ForceEnd.
                // For now, we will trigger the end via RoundSummary.
                RoundSummary.singleton.ForceEnd = new RoundSummary.ForceEndArgs(condition.WinningTeam, true);
                return;
            }
        }

        public void OnRoundEnding(RoundEndingEventArgs ev)
        {
            if (EvaluatePerRoleEndConditions(ev) || EvaluateCustomTeamEndConditions(ev))
            {
                // We modified the round ending args or handled it
                return;
            }
        }

        private bool EvaluatePerRoleEndConditions(RoundEndingEventArgs ev)
        {
            if (Plugin.Instance.Config.EndConditions == null)
                return false;

            var allPlayers = Player.GetPlayers();

            foreach (var kvp in Plugin.Instance.Config.EndConditions)
            {
                int roleId = kvp.Key;
                EndCondition condition = kvp.Value;

                // Check if any player has this custom role
                var customPlayers = SummonedCustomRole.List.Values.Where(r => r.Role.Id == roleId).ToList();
                if (customPlayers.Count == 0)
                    continue; // No players with this role, skip

                var aliveTeams = GetAliveTeams(allPlayers);

                if (condition.MustRemainOnlyOneTeam)
                {
                    // For "Only One Team", we check if the ONLY teams alive are the custom role's base team
                    // Note: In original code, it just ended the round if true. We'll refine it:
                    if (aliveTeams.Count == 1) // Only one team left
                    {
                        ev.IsAllowed = true;
                        ev.LeadingTeam = condition.WinningTeam;
                        return true;
                    }
                }
                else
                {
                    // Check if the current alive teams match the condition's allowed teams
                    if (aliveTeams.All(t => condition.RemainingTeams.ContainsKey(t)))
                    {
                        int maxAllowed = condition.MaxPlayersToEnd;
                        int currentCount = allPlayers.Count(p => p.IsAlive && condition.RemainingTeams.ContainsKey(p.Role.Team));

                        if (currentCount <= maxAllowed)
                        {
                            ev.IsAllowed = true;
                            ev.LeadingTeam = condition.WinningTeam;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool EvaluateCustomTeamEndConditions(RoundEndingEventArgs ev)
        {
            if (Plugin.Instance.Config.CustomTeamEndConditions == null)
                return false;

            var allPlayers = Player.GetPlayers();

            foreach (var kvp in Plugin.Instance.Config.CustomTeamEndConditions)
            {
                string customTeamId = kvp.Key;
                CustomTeamEndCondition condition = kvp.Value;

                // Find players in this custom team
                var teamPlayers = SummonedCustomRole.List.Values
                    .Where(r => r.Role.CustomTeamId == customTeamId)
                    .Select(r => r.Player)
                    .ToList();

                if (teamPlayers.Count == 0)
                    continue; // No players in this custom team

                // Count enemy players (alive players NOT in this custom team AND NOT in friendly teams)
                int enemyCount = allPlayers.Count(p => 
                    p.IsAlive && 
                    !teamPlayers.Contains(p) && 
                    !condition.FriendlyTeams.Contains(p.Role.Team)
                );

                if (condition.MustBeLastStanding && enemyCount == 0)
                {
                    ev.IsAllowed = true;
                    ev.LeadingTeam = condition.WinningTeam;
                    return true;
                }
                else if (!condition.MustBeLastStanding && enemyCount <= condition.MaxEnemyPlayers)
                {
                    ev.IsAllowed = true;
                    ev.LeadingTeam = condition.WinningTeam;
                    return true;
                }
            }

            return false;
        }

        private List<Team> GetAliveTeams(List<Player> players)
        {
            return players
                .Where(p => p.IsAlive)
                .Select(p => p.Role.Team)
                .Distinct()
                .ToList();
        }
    }
}
