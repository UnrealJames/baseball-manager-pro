using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AwardsSystem : MonoBehaviour
{
    // -------------------------------------------------------
    // MAIN ENTRY POINT
    // Call this after season simulation is complete
    // -------------------------------------------------------
    public void ProcessEndOfSeason(List<Team> allTeams)
    {
        List<Player> allPlayers = GetAllPlayers(allTeams);
        List<Player> batters    = allPlayers
            .Where(p => p.position != "SP" && p.position != "RP")
            .Where(p => p.seasonAtBats >= 200) // Minimum ABs
            .ToList();
        List<Player> pitchers   = allPlayers
            .Where(p => p.position == "SP" || p.position == "RP")
            .Where(p => p.seasonInningsPitched >= 50) // Minimum IP
            .ToList();

        PrintBattingLeaders(batters);
        PrintPitchingLeaders(pitchers);
        PrintAwards(allTeams, batters, pitchers);
    }

    // -------------------------------------------------------
    // BATTING LEADERS
    // -------------------------------------------------------
    void PrintBattingLeaders(List<Player> batters)
    {
        Debug.Log("\n========== BATTING LEADERS ==========");

        // Batting Average
        Debug.Log("\n-- Batting Average --");
        var avgLeaders = batters
            .OrderByDescending(p => p.SeasonBattingAverage())
            .Take(10).ToList();
        for (int i = 0; i < avgLeaders.Count; i++)
        {
            Player p = avgLeaders[i];
            Debug.Log((i + 1) + ". " + p.FullName().PadRight(20) +
                      p.team.PadRight(5) +
                      p.SeasonBattingAverage().ToString("F3"));
        }

        // Home Runs
        Debug.Log("\n-- Home Runs --");
        var hrLeaders = batters
            .OrderByDescending(p => p.seasonHomeRuns)
            .Take(10).ToList();
        for (int i = 0; i < hrLeaders.Count; i++)
        {
            Player p = hrLeaders[i];
            Debug.Log((i + 1) + ". " + p.FullName().PadRight(20) +
                      p.team.PadRight(5) +
                      p.seasonHomeRuns);
        }

        // RBI
        Debug.Log("\n-- RBI --");
        var rbiLeaders = batters
            .OrderByDescending(p => p.seasonRbi)
            .Take(10).ToList();
        for (int i = 0; i < rbiLeaders.Count; i++)
        {
            Player p = rbiLeaders[i];
            Debug.Log((i + 1) + ". " + p.FullName().PadRight(20) +
                      p.team.PadRight(5) +
                      p.seasonRbi);
        }

        // OPS
        Debug.Log("\n-- OPS --");
        var opsLeaders = batters
            .OrderByDescending(p => p.SeasonOPS())
            .Take(10).ToList();
        for (int i = 0; i < opsLeaders.Count; i++)
        {
            Player p = opsLeaders[i];
            Debug.Log((i + 1) + ". " + p.FullName().PadRight(20) +
                      p.team.PadRight(5) +
                      p.SeasonOPS().ToString("F3"));
        }

        // wOBA
        Debug.Log("\n-- wOBA --");
        var wobaLeaders = batters
            .OrderByDescending(p => p.SeasonwOBA())
            .Take(10).ToList();
        for (int i = 0; i < wobaLeaders.Count; i++)
        {
            Player p = wobaLeaders[i];
            Debug.Log((i + 1) + ". " + p.FullName().PadRight(20) +
                      p.team.PadRight(5) +
                      p.SeasonwOBA().ToString("F3"));
        }

        // Stolen Bases — using speed as proxy for now
        Debug.Log("\n-- Runs Scored --");
        var runsLeaders = batters
            .OrderByDescending(p => p.seasonRuns)
            .Take(10).ToList();
        for (int i = 0; i < runsLeaders.Count; i++)
        {
            Player p = runsLeaders[i];
            Debug.Log((i + 1) + ". " + p.FullName().PadRight(20) +
                      p.team.PadRight(5) +
                      p.seasonRuns);
        }
    }

    // -------------------------------------------------------
    // PITCHING LEADERS
    // -------------------------------------------------------
    void PrintPitchingLeaders(List<Player> pitchers)
    {
        Debug.Log("\n========== PITCHING LEADERS ==========");

        // ERA
        Debug.Log("\n-- ERA (min 50 IP) --");
        var eraLeaders = pitchers
            .OrderBy(p => p.SeasonERA())
            .Take(10).ToList();
        for (int i = 0; i < eraLeaders.Count; i++)
        {
            Player p = eraLeaders[i];
            Debug.Log((i + 1) + ". " + p.FullName().PadRight(20) +
                      p.team.PadRight(5) +
                      p.SeasonERA().ToString("F2"));
        }

        // Strikeouts
        Debug.Log("\n-- Strikeouts --");
        var kLeaders = pitchers
            .OrderByDescending(p => p.seasonStrikeoutsThrown)
            .Take(10).ToList();
        for (int i = 0; i < kLeaders.Count; i++)
        {
            Player p = kLeaders[i];
            Debug.Log((i + 1) + ". " + p.FullName().PadRight(20) +
                      p.team.PadRight(5) +
                      p.seasonStrikeoutsThrown);
        }

        // Wins
        Debug.Log("\n-- Wins --");
        var winsLeaders = pitchers
            .OrderByDescending(p => p.seasonWins)
            .Take(10).ToList();
        for (int i = 0; i < winsLeaders.Count; i++)
        {
            Player p = winsLeaders[i];
            Debug.Log((i + 1) + ". " + p.FullName().PadRight(20) +
                      p.team.PadRight(5) +
                      p.seasonWins + "-" + p.seasonLosses);
        }

        // WHIP
        Debug.Log("\n-- WHIP --");
        var whipLeaders = pitchers
            .Where(p => p.seasonInningsPitched > 0)
            .OrderBy(p => SeasonWHIP(p))
            .Take(10).ToList();
        for (int i = 0; i < whipLeaders.Count; i++)
        {
            Player p = whipLeaders[i];
            Debug.Log((i + 1) + ". " + p.FullName().PadRight(20) +
                      p.team.PadRight(5) +
                      SeasonWHIP(p).ToString("F2"));
        }

        // Innings Pitched
        Debug.Log("\n-- Innings Pitched --");
        var ipLeaders = pitchers
            .OrderByDescending(p => p.seasonInningsPitched)
            .Take(10).ToList();
        for (int i = 0; i < ipLeaders.Count; i++)
        {
            Player p = ipLeaders[i];
            Debug.Log((i + 1) + ". " + p.FullName().PadRight(20) +
                      p.team.PadRight(5) +
                      p.seasonInningsPitched);
        }
    }

    // -------------------------------------------------------
    // SEASON AWARDS
    // -------------------------------------------------------
    void PrintAwards(List<Team> allTeams,
                     List<Player> batters, List<Player> pitchers)
    {
        Debug.Log("\n========== SEASON AWARDS ==========");

        foreach (string league in new[] { "AL", "NL" })
        {
            Debug.Log("\n--- " + league + " Awards ---");

            List<Team> leagueTeams = allTeams
                .Where(t => t.league == league).ToList();
            List<string> leagueAbbrs = leagueTeams
                .Select(t => t.abbreviation).ToList();

            List<Player> leagueBatters = batters
                .Where(p => leagueAbbrs.Contains(p.team)).ToList();
            List<Player> leaguePitchers = pitchers
                .Where(p => leagueAbbrs.Contains(p.team)).ToList();

            // MVP — best wOBA + overall rating
            Player mvp = leagueBatters
                .OrderByDescending(p => p.SeasonwOBA() * 0.6f +
                                        p.overall / 99f * 0.4f)
                .FirstOrDefault();
            if (mvp != null)
                Debug.Log("MVP: " + mvp.FullName() +
                          " (" + mvp.team + ")" +
                          " | AVG: " + mvp.SeasonBattingAverage().ToString("F3") +
                          " HR: "  + mvp.seasonHomeRuns +
                          " RBI: " + mvp.seasonRbi +
                          " OPS: " + mvp.SeasonOPS().ToString("F3"));

            // Cy Young — best ERA + strikeouts
            Player cyYoung = leaguePitchers
                .Where(p => p.seasonInningsPitched >= 100)
                .OrderBy(p => p.SeasonERA() * 0.5f -
                              p.seasonStrikeoutsThrown * 0.01f)
                .FirstOrDefault();
            if (cyYoung != null)
                Debug.Log("Cy Young: " + cyYoung.FullName() +
                          " (" + cyYoung.team + ")" +
                          " | ERA: " + cyYoung.SeasonERA().ToString("F2") +
                          " K: "     + cyYoung.seasonStrikeoutsThrown +
                          " W-L: "   + cyYoung.seasonWins +
                          "-"        + cyYoung.seasonLosses);

            // Rookie of the Year — best wOBA under age 25
            Player roy = leagueBatters
                .Where(p => p.age <= 25)
                .OrderByDescending(p => p.SeasonwOBA())
                .FirstOrDefault();
            if (roy != null)
                Debug.Log("Rookie of Year: " + roy.FullName() +
                          " (" + roy.team + ")" +
                          " | Age: " + roy.age +
                          " AVG: "   + roy.SeasonBattingAverage().ToString("F3") +
                          " HR: "    + roy.seasonHomeRuns +
                          " OPS: "   + roy.SeasonOPS().ToString("F3"));

            // Manager of the Year — team that most exceeded expectations
            // Based on runs scored vs runs allowed (pythag win%)
            Team moty = leagueTeams
                .OrderByDescending(t => PythagRecord(t) - t.WinPercentage())
                .FirstOrDefault();
            if (moty != null)
                Debug.Log("Manager of Year: " + moty.city + " " +
                          moty.nickname + " | Record: " + moty.Record());

            // Silver Slugger — best hitter at each position
            Debug.Log("\nSilver Sluggers:");
            string[] positions = new string[]
            { "C", "1B", "2B", "3B", "SS", "LF", "CF", "RF", "DH" };

            foreach (string pos in positions)
            {
                Player ss = leagueBatters
                    .Where(p => p.position == pos)
                    .OrderByDescending(p => p.SeasonOPS())
                    .FirstOrDefault();
                if (ss != null)
                    Debug.Log("  " + pos.PadRight(3) + ": " +
                              ss.FullName() + " (" + ss.team + ")" +
                              " OPS: " + ss.SeasonOPS().ToString("F3"));
            }
        }
    }

    // -------------------------------------------------------
    // HELPERS
    // -------------------------------------------------------
    float SeasonWHIP(Player p)
    {
        if (p.seasonInningsPitched == 0) return 99f;
        return (float)(p.seasonWalksAllowed + p.seasonHitsAllowed) /
               p.seasonInningsPitched;
    }

    float PythagRecord(Team t)
    {
        if (t.runsScored + t.runsAllowed == 0) return 0f;
        float rs2 = t.runsScored  * t.runsScored;
        float ra2 = t.runsAllowed * t.runsAllowed;
        return rs2 / (rs2 + ra2);
    }

    List<Player> GetAllPlayers(List<Team> allTeams)
    {
        List<Player> all = new List<Player>();
        foreach (Team t in allTeams)
            if (t.roster != null)
                all.AddRange(t.roster);
        return all;
    }
}
