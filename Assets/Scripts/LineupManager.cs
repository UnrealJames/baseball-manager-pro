using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LineupManager : MonoBehaviour
{
    public List<Player> BuildOptimalLineup(Team team, Player opposingPitcher)
    {
        // Get all position players (no pitchers)
        List<Player> positionPlayers = team.roster
            .Where(p => p.position != "SP" && p.position != "RP")
            .ToList();

        if (positionPlayers.Count == 0)
        {
            Debug.LogError("No position players found for " + team.nickname);
            return new List<Player>();
        }

        // Separate by platoon advantage
        List<Player> advantageBatters    = new List<Player>();
        List<Player> disadvantageBatters = new List<Player>();

        foreach (Player p in positionPlayers)
        {
            if (HasPlatoonAdvantage(p, opposingPitcher))
                advantageBatters.Add(p);
            else
                disadvantageBatters.Add(p);
        }

        // Sort each group by overall rating
        advantageBatters    = advantageBatters
            .OrderByDescending(p => p.overall).ToList();
        disadvantageBatters = disadvantageBatters
            .OrderByDescending(p => p.overall).ToList();

        List<Player> lineup = new List<Player>();

        // Slot 1 — best leadoff (speed + contact)
        Player leadoff = advantageBatters.Count > 0
            ? advantageBatters.OrderByDescending(p => p.speed + p.contact).First()
            : disadvantageBatters.OrderByDescending(p => p.speed + p.contact).First();
        lineup.Add(leadoff);
        advantageBatters.Remove(leadoff);
        disadvantageBatters.Remove(leadoff);

        // Slot 2 — second best contact
        Player second = advantageBatters.Count > 0
            ? advantageBatters.OrderByDescending(p => p.contact).First()
            : disadvantageBatters.OrderByDescending(p => p.contact).First();
        lineup.Add(second);
        advantageBatters.Remove(second);
        disadvantageBatters.Remove(second);

        // Slot 3 — best overall hitter
        Player best = positionPlayers
            .Where(p => !lineup.Contains(p))
            .OrderByDescending(p => p.overall)
            .FirstOrDefault();
        if (best != null)
        {
            lineup.Add(best);
            advantageBatters.Remove(best);
            disadvantageBatters.Remove(best);
        }

        // Slot 4 — best power hitter (cleanup)
        Player cleanup = positionPlayers
            .Where(p => !lineup.Contains(p))
            .OrderByDescending(p => p.power)
            .FirstOrDefault();
        if (cleanup != null)
        {
            lineup.Add(cleanup);
            advantageBatters.Remove(cleanup);
            disadvantageBatters.Remove(cleanup);
        }

        // Slots 5-9 — fill remaining by overall
        List<Player> remaining = positionPlayers
            .Where(p => !lineup.Contains(p))
            .OrderByDescending(p => p.overall)
            .ToList();

        lineup.AddRange(remaining);

        // Log the lineup
        Debug.Log("\n=== " + team.city + " " + team.nickname +
                  " LINEUP vs " + opposingPitcher.throwingArm +
                  "HP " + opposingPitcher.FullName() + " ===");

        for (int i = 0; i < lineup.Count; i++)
        {
            Player p = lineup[i];
            Debug.Log((i + 1) + ". " + p.FullName().PadRight(18) +
                      " | " + p.battingHand + " | " + p.position);
        }

        return lineup;
    }

    bool HasPlatoonAdvantage(Player batter, Player pitcher)
    {
        if (batter.battingHand == "S") return true;
        if (batter.battingHand == "L" && pitcher.throwingArm == "R") return true;
        if (batter.battingHand == "R" && pitcher.throwingArm == "L") return true;
        return false;
    }
}
