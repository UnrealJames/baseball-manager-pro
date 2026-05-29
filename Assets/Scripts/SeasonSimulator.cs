using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SeasonSimulator : MonoBehaviour
{
    private GameSimulator       gameSimulator;
    private AwardsSystem        awardsSystem;
    private PostseasonSimulator postseasonSimulator;
    private InjurySystem        injurySystem;

    void Start()
    {
        gameSimulator       = GetComponent<GameSimulator>();
        awardsSystem        = GetComponent<AwardsSystem>();
        postseasonSimulator = GetComponent<PostseasonSimulator>();
        injurySystem        = GetComponent<InjurySystem>();
    }

    public void SimulateSeason(SeasonSchedule schedule, List<Team> allTeams)
    {
        Debug.Log("\n=== SEASON SIMULATION STARTING ===");
        Debug.Log("Simulating " + schedule.totalGames + " games...");

        // Reset all team records
        foreach (Team t in allTeams)
        {
            t.wins        = 0;
            t.losses      = 0;
            t.runsScored  = 0;
            t.runsAllowed = 0;
        }

        // Reset all player season stats
        foreach (Team t in allTeams)
            if (t.roster != null)
                foreach (Player p in t.roster)
                    ResetSeasonStats(p);

        // Build team lookup
        Dictionary<string, Team> teamLookup = new Dictionary<string, Team>();
        foreach (Team t in allTeams)
            teamLookup[t.abbreviation] = t;

        int gamesPlayed = 0;
        int totalGames  = schedule.allGames.Count;
        int logEvery    = 100;

        // Simulate every game
        foreach (GameEntry game in schedule.allGames)
        {
            if (!teamLookup.ContainsKey(game.homeTeam)) continue;
            if (!teamLookup.ContainsKey(game.awayTeam)) continue;

            Team homeTeam = teamLookup[game.homeTeam];
            Team awayTeam = teamLookup[game.awayTeam];

        if (homeTeam.roster == null || homeTeam.roster.Count < 5) continue;
        if (awayTeam.roster == null || awayTeam.roster.Count < 5) continue;

            SimulateQuickGame(homeTeam, awayTeam);

            // Process injuries every game
            injurySystem.ProcessGameDay(allTeams, gamesPlayed);

            game.isPlayed = true;
            gamesPlayed++;

            if (gamesPlayed % logEvery == 0)
            {
                float pct = (float)gamesPlayed / totalGames * 100f;
                Debug.Log("Progress: " + gamesPlayed + "/" +
                          totalGames + " (" + pct.ToString("F0") + "%)");
            }
        }

        Debug.Log("\n=== SEASON COMPLETE ===");
        Debug.Log("Games simulated: " + gamesPlayed);

        // Print standings
        PrintStandings(allTeams);

        // Print injury report
        injurySystem.PrintInjuryReport(allTeams);
        injurySystem.PrintSeasonInjurySummary(allTeams);

        // Process awards
        awardsSystem.ProcessEndOfSeason(allTeams);

        // Simulate postseason
        postseasonSimulator.SimulatePostseason(allTeams);
    }

    // -------------------------------------------------------
    // QUICK GAME SIMULATION
    // -------------------------------------------------------
    void SimulateQuickGame(Team homeTeam, Team awayTeam)
    {
        int homeRuns = 0;
        int awayRuns = 0;

        Player homePitcher = homeTeam.GetStartingPitcher();
        Player awayPitcher = awayTeam.GetStartingPitcher();

        if (homePitcher == null || awayPitcher == null) return;

        homePitcher.inningsPitched = 0;
        homePitcher.earnedRuns     = 0;
        awayPitcher.inningsPitched = 0;
        awayPitcher.earnedRuns     = 0;

        homePitcher.InitializePitcher();
        awayPitcher.InitializePitcher();

        // Only use healthy players
        List<Player> homeLineup = homeTeam.roster
            .Where(p => p.position != "SP" &&
                        p.position != "RP" &&
                        !p.isInjured)
            .ToList();
        List<Player> awayLineup = awayTeam.roster
            .Where(p => p.position != "SP" &&
                        p.position != "RP" &&
                        !p.isInjured)
            .ToList();

        // Fallback if too many injuries
        if (homeLineup.Count < 5)
            homeLineup = homeTeam.roster
                .Where(p => p.position != "SP" && p.position != "RP")
                .ToList();
        if (awayLineup.Count < 5)
            awayLineup = awayTeam.roster
                .Where(p => p.position != "SP" && p.position != "RP")
                .ToList();

        if (homeLineup.Count == 0 || awayLineup.Count == 0) return;

        int homeBatterIndex = 0;
        int awayBatterIndex = 0;
        int inning          = 1;
        int maxInnings      = 18;

        while (inning <= maxInnings)
        {
            awayRuns += SimulateQuickInning(
                awayLineup, homePitcher, ref awayBatterIndex);

            if (inning >= 9 && homeRuns > awayRuns) break;

            homeRuns += SimulateQuickInning(
                homeLineup, awayPitcher, ref homeBatterIndex);

            if (inning >= 9 && homeRuns != awayRuns) break;

            inning++;
        }

        // Accumulate pitcher stats
        AccumulatePitcherStats(homePitcher, homeRuns > awayRuns);
        AccumulatePitcherStats(awayPitcher, awayRuns > homeRuns);

        // Accumulate batter stats
        foreach (Player p in homeLineup) AccumulateBatterStats(p);
        foreach (Player p in awayLineup) AccumulateBatterStats(p);

        // Update team records
        if (homeRuns > awayRuns)
        {
            homeTeam.wins++;
            awayTeam.losses++;
        }
        else if (awayRuns > homeRuns)
        {
            awayTeam.wins++;
            homeTeam.losses++;
        }
        else
        {
            homeTeam.wins++;
            awayTeam.wins++;
        }

        homeTeam.runsScored  += homeRuns;
        homeTeam.runsAllowed += awayRuns;
        awayTeam.runsScored  += awayRuns;
        awayTeam.runsAllowed += homeRuns;

        // Update confidence
        homePitcher.UpdateConfidenceAfterGame(
            homePitcher.earnedRuns, homePitcher.inningsPitched);
        awayPitcher.UpdateConfidenceAfterGame(
            awayPitcher.earnedRuns, awayPitcher.inningsPitched);

        // Reset game stats
        foreach (Player p in homeTeam.roster) ResetGameStats(p);
        foreach (Player p in awayTeam.roster) ResetGameStats(p);
    }

    int SimulateQuickInning(List<Player> lineup, Player pitcher,
                             ref int batterIndex)
    {
        int outs  = 0;
        int runs  = 0;
        bool first  = false;
        bool second = false;
        bool third  = false;

        while (outs < 3)
        {
            Player batter = lineup[batterIndex % lineup.Count];
            batterIndex++;

            string result = QuickAtBat(batter, pitcher);

            if (result == "HOME RUN")
            {
                if (third)  runs++;
                if (second) runs++;
                if (first)  runs++;
                runs++;
                first = second = third = false;
            }
            else if (result == "TRIPLE")
            {
                if (third)  runs++;
                if (second) runs++;
                if (first)  runs++;
                first = second = false;
                third = true;
            }
            else if (result == "DOUBLE")
            {
                if (third)  runs++;
                if (second) runs++;
                third  = first;
                second = true;
                first  = false;
            }
            else if (result == "SINGLE" || result == "WALK")
            {
                if (third) runs++;
                third  = second;
                second = first;
                first  = true;
            }
            else
            {
                outs++;
            }

            if (result != "WALK") batter.atBats++;
            if (result == "HOME RUN")    { batter.hits++; batter.homeRuns++; }
            else if (result == "TRIPLE") { batter.hits++; batter.triples++;  }
            else if (result == "DOUBLE") { batter.hits++; batter.doubles++;  }
            else if (result == "SINGLE") { batter.hits++; batter.singles++;  }
            else if (result == "WALK")   { batter.walks++;                   }
            else if (result == "STRIKEOUT") { batter.strikeouts++;           }

            if (batterIndex > 200) break;
        }

        pitcher.inningsPitched++;
        return runs;
    }

    string QuickAtBat(Player batter, Player pitcher)
    {
        float hr  = 3.3f  + (batter.power   - 50) / 200f * 2f;
        float tri = 0.6f;
        float dbl = 5.3f  + (batter.power   - 50) / 200f * 1f;
        float sng = 15.0f + (batter.contact - 50) / 200f * 3f;
        float bb  = 8.5f;
        float so  = 22.5f - (batter.contact - 50) / 200f * 3f;

        float pitchMod   = (pitcher.pitching - 50) / 200f;
        so  += pitchMod * 3f;
        sng -= pitchMod * 2f;
        hr  -= pitchMod * 1f;

        float fatigue    = pitcher.GetFatigueMultiplier(pitcher.inningsPitched);
        float fatiguePen = 1f - fatigue;
        hr  += fatiguePen * 6f;
        sng += fatiguePen * 5f;
        so  -= fatiguePen * 8f;

        hr  = Mathf.Max(0.5f, hr);
        tri = Mathf.Max(0.1f, tri);
        dbl = Mathf.Max(1.0f, dbl);
        sng = Mathf.Max(5.0f, sng);
        bb  = Mathf.Max(2.0f, bb);
        so  = Mathf.Max(5.0f, so);

        float roll = Random.Range(0f, 100f);
        float c    = 0f;

        c += hr;  if (roll < c) return "HOME RUN";
        c += tri; if (roll < c) return "TRIPLE";
        c += dbl; if (roll < c) return "DOUBLE";
        c += sng; if (roll < c) return "SINGLE";
        c += bb;  if (roll < c) return "WALK";
        c += so;  if (roll < c) return "STRIKEOUT";
        return "OUT";
    }

    // -------------------------------------------------------
    // STAT ACCUMULATION
    // -------------------------------------------------------
    void AccumulateBatterStats(Player p)
    {
        p.seasonGamesPlayed++;
        p.seasonAtBats     += p.atBats;
        p.seasonHits       += p.hits;
        p.seasonSingles    += p.singles;
        p.seasonDoubles    += p.doubles;
        p.seasonTriples    += p.triples;
        p.seasonHomeRuns   += p.homeRuns;
        p.seasonRbi        += p.rbi;
        p.seasonRuns       += p.runs;
        p.seasonWalks      += p.walks;
        p.seasonStrikeouts += p.strikeouts;
    }

    void AccumulatePitcherStats(Player p, bool won)
    {
        p.seasonInningsPitched   += p.inningsPitched;
        p.seasonEarnedRuns       += p.earnedRuns;
        p.seasonHitsAllowed      += p.hitsAllowed;
        p.seasonWalksAllowed     += p.walksAllowed;
        p.seasonStrikeoutsThrown += p.strikeoutsThrown;
        if (won) p.seasonWins++;
        else     p.seasonLosses++;
    }

    // -------------------------------------------------------
    // STAT RESETS
    // -------------------------------------------------------
    void ResetGameStats(Player p)
    {
        p.gamesPlayed      = 0;
        p.atBats           = 0;
        p.hits             = 0;
        p.singles          = 0;
        p.doubles          = 0;
        p.triples          = 0;
        p.homeRuns         = 0;
        p.rbi              = 0;
        p.runs             = 0;
        p.walks            = 0;
        p.strikeouts       = 0;
        p.inningsPitched   = 0;
        p.earnedRuns       = 0;
        p.hitsAllowed      = 0;
        p.walksAllowed     = 0;
        p.strikeoutsThrown = 0;
    }

    void ResetSeasonStats(Player p)
    {
        p.seasonGamesPlayed      = 0;
        p.seasonAtBats           = 0;
        p.seasonHits             = 0;
        p.seasonSingles          = 0;
        p.seasonDoubles          = 0;
        p.seasonTriples          = 0;
        p.seasonHomeRuns         = 0;
        p.seasonRbi              = 0;
        p.seasonRuns             = 0;
        p.seasonWalks            = 0;
        p.seasonStrikeouts       = 0;
        p.seasonInningsPitched   = 0;
        p.seasonEarnedRuns       = 0;
        p.seasonHitsAllowed      = 0;
        p.seasonWalksAllowed     = 0;
        p.seasonStrikeoutsThrown = 0;
        p.seasonWins             = 0;
        p.seasonLosses           = 0;
        ResetGameStats(p);
    }

    // -------------------------------------------------------
    // STANDINGS
    // -------------------------------------------------------
    void PrintStandings(List<Team> allTeams)
    {
        string[] divisionOrder = new string[]
        {
            "AL East", "AL Central", "AL West",
            "NL East", "NL Central", "NL West"
        };

        Debug.Log("\n========== FINAL STANDINGS ==========");

        foreach (string div in divisionOrder)
        {
            List<Team> divTeams = allTeams
                .Where(t => t.division == div)
                .OrderByDescending(t => t.wins)
                .ThenBy(t => t.losses)
                .ToList();

            Debug.Log("\n--- " + div + " ---");
            Debug.Log("TEAM                 W    L    PCT   GB   RS   RA");
            Debug.Log("--------------------------------------------------");

            float leaderWins   = divTeams[0].wins;
            float leaderLosses = divTeams[0].losses;

            foreach (Team t in divTeams)
            {
                float pct = t.wins + t.losses > 0
                    ? (float)t.wins / (t.wins + t.losses) : 0f;

                float gb = ((leaderWins - t.wins) +
                            (t.losses - leaderLosses)) / 2f;

                string gbStr = gb == 0 ? " —  " : gb.ToString("F1");

                Debug.Log(
                    (t.city + " " + t.nickname).PadRight(20) + " " +
                    t.wins.ToString().PadRight(4) +
                    t.losses.ToString().PadRight(5) +
                    pct.ToString("F3") + " " +
                    gbStr.PadRight(6) +
                    t.runsScored.ToString().PadRight(5) +
                    t.runsAllowed.ToString()
                );
            }
        }

        PrintLeagueLeaders(allTeams);
    }

    void PrintLeagueLeaders(List<Team> allTeams)
    {
        Debug.Log("\n========== LEAGUE LEADERS ==========");

        Team alBest = allTeams
            .Where(t => t.league == "AL")
            .OrderByDescending(t => t.wins)
            .First();
        Team nlBest = allTeams
            .Where(t => t.league == "NL")
            .OrderByDescending(t => t.wins)
            .First();

        Debug.Log("AL Best: " + alBest.city + " " +
                  alBest.nickname + " " + alBest.Record());
        Debug.Log("NL Best: " + nlBest.city + " " +
                  nlBest.nickname + " " + nlBest.Record());

        Debug.Log("\n--- AL PLAYOFF PICTURE ---");
        PrintPlayoffPicture(allTeams
            .Where(t => t.league == "AL").ToList());

        Debug.Log("\n--- NL PLAYOFF PICTURE ---");
        PrintPlayoffPicture(allTeams
            .Where(t => t.league == "NL").ToList());
    }

    void PrintPlayoffPicture(List<Team> leagueTeams)
    {
        var divisions     = leagueTeams.GroupBy(t => t.division);
        List<Team> divWinners = new List<Team>();

        foreach (var div in divisions)
        {
            Team winner = div.OrderByDescending(t => t.wins).First();
            divWinners.Add(winner);
            Debug.Log("DIV: " + winner.city + " " +
                      winner.nickname + " " + winner.Record());
        }

        List<Team> wildCards = leagueTeams
            .Where(t => !divWinners.Contains(t))
            .OrderByDescending(t => t.wins)
            .Take(3).ToList();

        foreach (Team wc in wildCards)
            Debug.Log("WC:  " + wc.city + " " +
                      wc.nickname + " " + wc.Record());
    }
}
