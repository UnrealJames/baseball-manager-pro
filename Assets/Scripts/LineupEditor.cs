using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LineupEditor : MonoBehaviour
{
    // -------------------------------------------------------
    // SET BATTING ORDER
    // Takes a list of player IDs in batting order 1-9
    // -------------------------------------------------------
    public bool SetBattingOrder(Team team, List<int> playerIds)
    {
        if (playerIds.Count != 9)
        {
            Debug.LogError("Batting order must have exactly 9 players!");
            return false;
        }

        // Validate all players exist and are position players
        foreach (int id in playerIds)
        {
            Player p = GetPlayerById(team, id);
            if (p == null)
            {
                Debug.LogError("Player ID " + id + " not found!");
                return false;
            }
            if (p.position == "SP" || p.position == "RP")
            {
                Debug.LogError(p.FullName() + " is a pitcher — " +
                               "cannot be in batting order!");
                return false;
            }
            if (p.isInjured)
            {
                Debug.LogWarning(p.FullName() + " is injured — " +
                                 "consider replacing them!");
            }
        }

        team.lineup = playerIds;
        Debug.Log("Batting order set for " +
                  team.city + " " + team.nickname);
        return true;
    }

    // -------------------------------------------------------
    // SET PITCHING ROTATION
    // Takes list of SP player IDs in rotation order
    // -------------------------------------------------------
    public bool SetRotation(Team team, List<int> pitcherIds)
    {
        if (pitcherIds.Count < 1 || pitcherIds.Count > 6)
        {
            Debug.LogError("Rotation must have 1-6 starters!");
            return false;
        }

        foreach (int id in pitcherIds)
        {
            Player p = GetPlayerById(team, id);
            if (p == null)
            {
                Debug.LogError("Pitcher ID " + id + " not found!");
                return false;
            }
            if (p.position != "SP")
            {
                Debug.LogError(p.FullName() +
                               " is not a starter!");
                return false;
            }
            if (p.isInjured)
            {
                Debug.LogWarning(p.FullName() +
                                 " is injured!");
            }
        }

        team.rotation = pitcherIds;
        Debug.Log("Rotation set for " +
                  team.city + " " + team.nickname);
        return true;
    }

    // -------------------------------------------------------
    // AUTO BUILD OPTIMAL LINEUP
    // CPU suggests best lineup based on player ratings
    // -------------------------------------------------------
    public List<int> BuildOptimalLineup(Team team)
    {
        // Get healthy position players
        List<Player> available = team.roster
            .Where(p => p.position != "SP" &&
                        p.position != "RP" &&
                        !p.isInjured)
            .ToList();

        if (available.Count < 9)
        {
            Debug.LogWarning("Not enough healthy players! " +
                             "Using injured players as needed.");
            available = team.roster
                .Where(p => p.position != "SP" &&
                            p.position != "RP")
                .ToList();
        }

        List<Player> lineup   = new List<Player>();
        List<Player> used     = new List<Player>();

        // Slot 1 — Leadoff: best speed + contact
        Player leadoff = available
            .Where(p => !used.Contains(p))
            .OrderByDescending(p => p.speed + p.contact)
            .FirstOrDefault();
        if (leadoff != null) { lineup.Add(leadoff); used.Add(leadoff); }

        // Slot 2 — Contact hitter
        Player slot2 = available
            .Where(p => !used.Contains(p))
            .OrderByDescending(p => p.contact)
            .FirstOrDefault();
        if (slot2 != null) { lineup.Add(slot2); used.Add(slot2); }

        // Slot 3 — Best overall hitter
        Player slot3 = available
            .Where(p => !used.Contains(p))
            .OrderByDescending(p => p.overall)
            .FirstOrDefault();
        if (slot3 != null) { lineup.Add(slot3); used.Add(slot3); }

        // Slot 4 — Cleanup: best power
        Player cleanup = available
            .Where(p => !used.Contains(p))
            .OrderByDescending(p => p.power)
            .FirstOrDefault();
        if (cleanup != null) { lineup.Add(cleanup); used.Add(cleanup); }

        // Slot 5 — Second best power
        Player slot5 = available
            .Where(p => !used.Contains(p))
            .OrderByDescending(p => p.power)
            .FirstOrDefault();
        if (slot5 != null) { lineup.Add(slot5); used.Add(slot5); }

        // Slots 6-9 — Fill by overall rating
        List<Player> remaining = available
            .Where(p => !used.Contains(p))
            .OrderByDescending(p => p.overall)
            .Take(4)
            .ToList();
        lineup.AddRange(remaining);

        // Convert to ID list
        List<int> lineupIds = lineup.Select(p => p.id).ToList();

        // Pad to 9 if needed
        while (lineupIds.Count < 9)
            lineupIds.Add(lineup[0].id);

        return lineupIds;
    }

    // -------------------------------------------------------
    // AUTO BUILD OPTIMAL ROTATION
    // -------------------------------------------------------
    public List<int> BuildOptimalRotation(Team team)
    {
        List<Player> starters = team.roster
            .Where(p => p.position == "SP" && !p.isInjured)
            .OrderByDescending(p => p.pitching)
            .Take(5)
            .ToList();

        if (starters.Count == 0)
        {
            Debug.LogError("No healthy starters found!");
            return new List<int>();
        }

        return starters.Select(p => p.id).ToList();
    }

    // -------------------------------------------------------
    // PRINT LINEUP
    // -------------------------------------------------------
    public void PrintLineup(Team team)
    {
        Debug.Log("\n=== " + team.city + " " +
                  team.nickname + " LINEUP ===");

        if (team.lineup == null || team.lineup.Count == 0)
        {
            Debug.Log("No lineup set — using auto lineup");
            team.lineup = BuildOptimalLineup(team);
        }

        for (int i = 0; i < team.lineup.Count; i++)
        {
            Player p = GetPlayerById(team, team.lineup[i]);
            if (p == null) continue;

            string injuryFlag = p.isInjured ? " ⚠ INJURED" : "";
            Debug.Log((i + 1) + ". " +
                      p.FullName().PadRight(20) +
                      " | " + p.position.PadRight(3) +
                      " | " + p.battingHand + " | " +
                      "OVR: " + p.overall +
                      injuryFlag);
        }
    }

    // -------------------------------------------------------
    // PRINT ROTATION
    // -------------------------------------------------------
    public void PrintRotation(Team team)
    {
        Debug.Log("\n=== " + team.city + " " +
                  team.nickname + " ROTATION ===");

        if (team.rotation == null || team.rotation.Count == 0)
        {
            Debug.Log("No rotation set — using auto rotation");
            team.rotation = BuildOptimalRotation(team);
        }

        for (int i = 0; i < team.rotation.Count; i++)
        {
            Player p = GetPlayerById(team, team.rotation[i]);
            if (p == null) continue;

            string injuryFlag = p.isInjured ? " ⚠ INJURED" : "";
            Debug.Log("SP" + (i + 1) + ". " +
                      p.FullName().PadRight(20) +
                      " | " + p.throwingArm + "HP" +
                      " | Pitching: " + p.pitching +
                      " | Stamina: "  + p.stamina +
                      injuryFlag);
        }

        // Print bullpen
        Debug.Log("\n-- BULLPEN --");
        List<Player> bullpen = team.roster
            .Where(p => p.position == "RP")
            .OrderBy(p => p.bullpenRole)
            .ToList();

        foreach (Player p in bullpen)
        {
            string injuryFlag = p.isInjured ? " ⚠ INJURED" : "";
            Debug.Log(p.bullpenRole.PadRight(3) + " " +
                      p.FullName().PadRight(20) +
                      " | " + p.throwingArm + "HP" +
                      " | Pitching: " + p.pitching +
                      injuryFlag);
        }
    }

    // -------------------------------------------------------
    // SWAP PLAYERS IN LINEUP
    // -------------------------------------------------------
    public void SwapLineupSlots(Team team, int slot1, int slot2)
    {
        if (team.lineup == null || team.lineup.Count < 9)
        {
            Debug.LogError("No lineup set!");
            return;
        }

        // Convert from 1-based to 0-based
        int idx1 = slot1 - 1;
        int idx2 = slot2 - 1;

        if (idx1 < 0 || idx1 >= 9 || idx2 < 0 || idx2 >= 9)
        {
            Debug.LogError("Slot numbers must be 1-9!");
            return;
        }

        int temp = team.lineup[idx1];
        team.lineup[idx1] = team.lineup[idx2];
        team.lineup[idx2] = temp;

        Player p1 = GetPlayerById(team, team.lineup[idx1]);
        Player p2 = GetPlayerById(team, team.lineup[idx2]);

        Debug.Log("Swapped: " +
                  (p2 != null ? p2.FullName() : "?") +
                  " (slot " + slot1 + ") ↔ " +
                  (p1 != null ? p1.FullName() : "?") +
                  " (slot " + slot2 + ")");
    }

    // -------------------------------------------------------
    // REPLACE INJURED PLAYER IN LINEUP
    // -------------------------------------------------------
    public void ReplaceInjuredPlayer(Team team, int injuredPlayerId)
    {
        if (team.lineup == null) return;

        int slotIndex = team.lineup.IndexOf(injuredPlayerId);
        if (slotIndex == -1)
        {
            Debug.Log("Player not in lineup");
            return;
        }

        Player injured = GetPlayerById(team, injuredPlayerId);

        // Find best available replacement at same position
        Player replacement = team.roster
            .Where(p => p.position == injured.position &&
                        !p.isInjured &&
                        !team.lineup.Contains(p.id))
            .OrderByDescending(p => p.overall)
            .FirstOrDefault();

        // If no exact position match find any healthy player
        if (replacement == null)
        {
            replacement = team.roster
                .Where(p => p.position != "SP" &&
                            p.position != "RP" &&
                            !p.isInjured &&
                            !team.lineup.Contains(p.id))
                .OrderByDescending(p => p.overall)
                .FirstOrDefault();
        }

        if (replacement == null)
        {
            Debug.LogWarning("No replacement found for " +
                             injured.FullName());
            return;
        }

        team.lineup[slotIndex] = replacement.id;

        Debug.Log("LINEUP CHANGE: " +
                  replacement.FullName() +
                  " replaces " + injured.FullName() +
                  " at slot " + (slotIndex + 1));
    }

    // -------------------------------------------------------
    // VALIDATE LINEUP
    // Checks for injuries and missing positions
    // -------------------------------------------------------
    public void ValidateAndFixLineup(Team team)
    {
        if (team.lineup == null || team.lineup.Count == 0)
        {
            team.lineup = BuildOptimalLineup(team);
            return;
        }

        // Check each slot for injuries
        for (int i = 0; i < team.lineup.Count; i++)
        {
            Player p = GetPlayerById(team, team.lineup[i]);
            if (p != null && p.isInjured)
            {
                Debug.Log(p.FullName() +
                          " is injured — finding replacement...");
                ReplaceInjuredPlayer(team, p.id);
            }
        }
    }

    // -------------------------------------------------------
    // HELPER
    // -------------------------------------------------------
    Player GetPlayerById(Team team, int id)
    {
        if (team.roster == null) return null;
        return team.roster.FirstOrDefault(p => p.id == id);
    }
}
