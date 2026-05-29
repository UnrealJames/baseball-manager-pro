using UnityEngine;
using System.Collections.Generic;

public class GameSimulator : MonoBehaviour
{
    private AtBatCalculator atBatCalculator;
    private InningSimulator inningSimulator;
    private LineupManager   lineupManager;
    private BenchManager    benchManager;

    void Start()
    {
        atBatCalculator = gameObject.AddComponent<AtBatCalculator>();
        inningSimulator = gameObject.AddComponent<InningSimulator>();
        lineupManager   = gameObject.AddComponent<LineupManager>();
        benchManager    = gameObject.AddComponent<BenchManager>();
    }

    public void SimulateGame(Team homeTeam, Team awayTeam)
    {
        Debug.Log("=== GAME START ===");
        Debug.Log(awayTeam.city + " " + awayTeam.nickname +
                  " vs " + homeTeam.city + " " + homeTeam.nickname);

        // Reset stats for this game
        foreach (Player p in homeTeam.roster)
            ResetGameStats(p);
        foreach (Player p in awayTeam.roster)
            ResetGameStats(p);

        // Reset bench manager
        benchManager.ResetForNewGame();

        // Get starting pitchers
        Player homePitcher = homeTeam.GetStartingPitcher();
        Player awayPitcher = awayTeam.GetStartingPitcher();

        if (homePitcher == null || awayPitcher == null)
        {
            Debug.LogError("Could not find starting pitchers!");
            return;
        }

        // Initialize pitchers
        homePitcher.InitializePitcher();
        awayPitcher.InitializePitcher();

        Debug.Log("Home pitcher: " + homePitcher.FullName() +
                  " | Confidence: " + homePitcher.confidence.ToString("F0") +
                  " | Stamina: "    + homePitcher.stamina);
        Debug.Log("Away pitcher: " + awayPitcher.FullName() +
                  " | Confidence: " + awayPitcher.confidence.ToString("F0") +
                  " | Stamina: "    + awayPitcher.stamina);

        // Track current pitchers (can change during game)
        Player currentHomePitcher = homePitcher;
        Player currentAwayPitcher = awayPitcher;

        // Build optimal lineups
        List<Player> homeLineup = lineupManager.BuildOptimalLineup(homeTeam, currentAwayPitcher);
        List<Player> awayLineup = lineupManager.BuildOptimalLineup(awayTeam, currentHomePitcher);

        int homeRuns = 0;
        int awayRuns = 0;

        int homeBatterIndex = 0;
        int awayBatterIndex = 0;

        int inning    = 1;
        int maxInnings = 18;

        while (inning <= maxInnings)
        {
            // --- TOP OF INNING (away bats) ---
            Debug.Log("\n== Inning " + inning + " - Top ==");

            // Check if we should pull home pitcher
            int awayRunDiff = awayRuns - homeRuns;
            if (benchManager.ShouldPullPitcher(currentHomePitcher, inning,
                currentHomePitcher.earnedRuns, awayRunDiff))
            {
                Player reliever = GetReliever(homeTeam, currentHomePitcher, inning, homeRuns - awayRuns);
                if (reliever != null)
                {
                    Debug.Log("PITCHING CHANGE: " + reliever.FullName() +
                              " comes in for " + homeTeam.nickname +
                              " (" + reliever.bullpenRole + ")");
                    reliever.InitializePitcher();
                    currentHomePitcher = reliever;
                }
            }

            bool isExtra = inning > 9;
            awayRuns += inningSimulator.SimulateInning(
                awayLineup, currentHomePitcher, inning,
                ref awayBatterIndex, isExtra, false);

            // --- BOTTOM OF INNING (home bats) ---
            // Skip if home leads after 9th+
            if (inning >= 9 && homeRuns > awayRuns)
            {
                Debug.Log("\n== Inning " + inning + " - Bottom ==");
                Debug.Log("Home team leads — game over!");
                break;
            }

            Debug.Log("\n== Inning " + inning + " - Bottom ==");

            // Check if we should pull away pitcher
            int homeRunDiff = homeRuns - awayRuns;
            if (benchManager.ShouldPullPitcher(currentAwayPitcher, inning,
                currentAwayPitcher.earnedRuns, homeRunDiff))
            {
                Player reliever = GetReliever(awayTeam, currentAwayPitcher, inning, awayRuns - homeRuns);
                if (reliever != null)
                {
                    Debug.Log("PITCHING CHANGE: " + reliever.FullName() +
                              " comes in for " + awayTeam.nickname +
                              " (" + reliever.bullpenRole + ")");
                    reliever.InitializePitcher();
                    currentAwayPitcher = reliever;
                }
            }

            homeRuns += inningSimulator.SimulateInning(
                homeLineup, currentAwayPitcher, inning,
                ref homeBatterIndex, isExtra, false);

            // Check game over after 9+
            if (inning >= 9 && homeRuns != awayRuns)
                break;

            // Extra innings announcement
            if (inning == 9 && homeRuns == awayRuns)
                Debug.Log("\n=== TIE GAME — EXTRA INNINGS! ===");

            inning++;
        }

        // Safety tie after 18
        if (homeRuns == awayRuns)
            Debug.Log("Game called after 18 innings — TIE!");

                // Push game stats into season stats immediately
        foreach (Player p in homeTeam.roster)
            AccumulateSeasonStats(p);
        foreach (Player p in awayTeam.roster)
            AccumulateSeasonStats(p);


        // Update pitcher confidence after game
        homePitcher.UpdateConfidenceAfterGame(
            homePitcher.earnedRuns, homePitcher.inningsPitched);
        awayPitcher.UpdateConfidenceAfterGame(
            awayPitcher.earnedRuns, awayPitcher.inningsPitched);

        // Final score
        Debug.Log("\n=== FINAL SCORE ===");
        Debug.Log(awayTeam.city  + " " + awayTeam.nickname  + ": " + awayRuns);
        Debug.Log(homeTeam.city  + " " + homeTeam.nickname  + ": " + homeRuns);

        if (homeRuns > awayRuns)
            Debug.Log(homeTeam.city + " " + homeTeam.nickname + " WIN!");
        else if (awayRuns > homeRuns)
            Debug.Log(awayTeam.city + " " + awayTeam.nickname + " WIN!");
        else
            Debug.Log("TIE GAME!");

        Debug.Log("Home pitcher confidence after game: " +
                  homePitcher.confidence.ToString("F0"));
        Debug.Log("Away pitcher confidence after game: " +
                  awayPitcher.confidence.ToString("F0"));

        // Update team records
        if (homeRuns > awayRuns)      { homeTeam.wins++;  awayTeam.losses++; }
        else if (awayRuns > homeRuns) { awayTeam.wins++;  homeTeam.losses++; }

        homeTeam.runsScored  += homeRuns;
        homeTeam.runsAllowed += awayRuns;
        awayTeam.runsScored  += awayRuns;
        awayTeam.runsAllowed += homeRuns;
    }

    Player GetReliever(Team team, Player currentPitcher, int inning, int runDiff)
    {
        // Closer in 9th with lead of 1-3 runs
        if (inning >= 9 && runDiff > 0 && runDiff <= 3)
        {
            Player closer = team.GetCloser();
            if (closer != null && closer.inningsPitched == 0)
                return closer;
        }

        // Setup man in 7th or 8th
        if (inning >= 7 && inning <= 8)
        {
            Player setup = team.GetSetupMan();
            if (setup != null && setup.inningsPitched == 0)
                return setup;
        }

        // Middle relievers otherwise
        List<Player> middleRelievers = team.GetMiddleRelievers();
        foreach (Player mr in middleRelievers)
        {
            if (mr.inningsPitched == 0)
                return mr;
        }

        // Fall back to anyone available
        Player setup2 = team.GetSetupMan();
        if (setup2 != null && setup2.inningsPitched == 0)
            return setup2;

        Player closer2 = team.GetCloser();
        if (closer2 != null && closer2.inningsPitched == 0)
            return closer2;

        return null;
    }

    void ResetGameStats(Player p)
    {
        // Accumulate into season stats first
        p.seasonGamesPlayed      += p.gamesPlayed;
        p.seasonAtBats           += p.atBats;
        p.seasonHits             += p.hits;
        p.seasonSingles          += p.singles;
        p.seasonDoubles          += p.doubles;
        p.seasonTriples          += p.triples;
        p.seasonHomeRuns         += p.homeRuns;
        p.seasonRbi              += p.rbi;
        p.seasonRuns             += p.runs;
        p.seasonWalks            += p.walks;
        p.seasonStrikeouts       += p.strikeouts;
        p.seasonInningsPitched   += p.inningsPitched;
        p.seasonEarnedRuns       += p.earnedRuns;
        p.seasonHitsAllowed      += p.hitsAllowed;
        p.seasonWalksAllowed     += p.walksAllowed;
        p.seasonStrikeoutsThrown += p.strikeoutsThrown;

        // Reset game stats
        p.gamesPlayed       = 0;
        p.atBats            = 0;
        p.hits              = 0;
        p.singles           = 0;
        p.doubles           = 0;
        p.triples           = 0;
        p.homeRuns          = 0;
        p.rbi               = 0;
        p.runs              = 0;
        p.walks             = 0;
        p.strikeouts        = 0;
        p.inningsPitched    = 0;
        p.earnedRuns        = 0;
        p.hitsAllowed       = 0;
        p.walksAllowed      = 0;
        p.strikeoutsThrown  = 0;
    }
        void AccumulateSeasonStats(Player p)
    {
        p.seasonGamesPlayed      += p.gamesPlayed;
        p.seasonAtBats           += p.atBats;
        p.seasonHits             += p.hits;
        p.seasonSingles          += p.singles;
        p.seasonDoubles          += p.doubles;
        p.seasonTriples          += p.triples;
        p.seasonHomeRuns         += p.homeRuns;
        p.seasonRbi              += p.rbi;
        p.seasonRuns             += p.runs;
        p.seasonWalks            += p.walks;
        p.seasonStrikeouts       += p.strikeouts;
        p.seasonInningsPitched   += p.inningsPitched;
        p.seasonEarnedRuns       += p.earnedRuns;
        p.seasonHitsAllowed      += p.hitsAllowed;
        p.seasonWalksAllowed     += p.walksAllowed;
        p.seasonStrikeoutsThrown += p.strikeoutsThrown;
    }


}
