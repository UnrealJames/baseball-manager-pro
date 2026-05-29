using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class SeriesResult
{
    public Team winner;
    public Team loser;
    public int  winnerWins;
    public int  loserWins;
    public string seriesName;

    public SeriesResult(Team w, Team l, int ww, int lw, string name)
    {
        winner     = w;
        loser      = l;
        winnerWins = ww;
        loserWins  = lw;
        seriesName = name;
    }

    public string Summary()
    {
        return winner.city + " " + winner.nickname +
               " defeats " +
               loser.city + " " + loser.nickname +
               " (" + winnerWins + "-" + loserWins + ")";
    }
}

public class PostseasonSimulator : MonoBehaviour
{
    private Dictionary<int, Player> postseasonStats =
        new Dictionary<int, Player>();

    public Player worldSeriesMVP;
    public Player alLCSMVP;
    public Player nlLCSMVP;

    // -------------------------------------------------------
    // MAIN ENTRY POINT
    // -------------------------------------------------------
    public void SimulatePostseason(List<Team> allTeams)
    {
        Debug.Log("\n\n========== POSTSEASON BEGINS ==========");

        List<Team> alPlayoff = GetPlayoffTeams(
            allTeams.Where(t => t.league == "AL").ToList());
        List<Team> nlPlayoff = GetPlayoffTeams(
            allTeams.Where(t => t.league == "NL").ToList());

        PrintPlayoffField("AL", alPlayoff);
        PrintPlayoffField("NL", nlPlayoff);

        ResetPostseasonStats(alPlayoff);
        ResetPostseasonStats(nlPlayoff);

        // -------------------------------------------------------
        // WILD CARD SERIES — Best of 3
        // 3 seed hosts 6 seed
        // 4 seed hosts 5 seed
        // -------------------------------------------------------
        Debug.Log("\n===== WILD CARD SERIES =====");

        SeriesResult alWC1 = SimulateSeries(
            alPlayoff[2], alPlayoff[5], 3, "AL Wild Card (3 vs 6)");
        SeriesResult alWC2 = SimulateSeries(
            alPlayoff[3], alPlayoff[4], 3, "AL Wild Card (4 vs 5)");

        Debug.Log(alWC1.Summary());
        Debug.Log(alWC2.Summary());

        SeriesResult nlWC1 = SimulateSeries(
            nlPlayoff[2], nlPlayoff[5], 3, "NL Wild Card (3 vs 6)");
        SeriesResult nlWC2 = SimulateSeries(
            nlPlayoff[3], nlPlayoff[4], 3, "NL Wild Card (4 vs 5)");

        Debug.Log(nlWC1.Summary());
        Debug.Log(nlWC2.Summary());

        // -------------------------------------------------------
        // DIVISION SERIES — Best of 5
        // 1 seed vs lowest WC winner
        // 2 seed vs highest WC winner
        // -------------------------------------------------------
        Debug.Log("\n===== DIVISION SERIES =====");

        SeriesResult alDS1 = SimulateSeries(
            alPlayoff[0], alWC2.winner, 5, "ALDS 1");
        SeriesResult alDS2 = SimulateSeries(
            alPlayoff[1], alWC1.winner, 5, "ALDS 2");

        Debug.Log(alDS1.Summary());
        Debug.Log(alDS2.Summary());

        SeriesResult nlDS1 = SimulateSeries(
            nlPlayoff[0], nlWC2.winner, 5, "NLDS 1");
        SeriesResult nlDS2 = SimulateSeries(
            nlPlayoff[1], nlWC1.winner, 5, "NLDS 2");

        Debug.Log(nlDS1.Summary());
        Debug.Log(nlDS2.Summary());

        // -------------------------------------------------------
        // LEAGUE CHAMPIONSHIP SERIES — Best of 7
        // -------------------------------------------------------
        Debug.Log("\n===== LEAGUE CHAMPIONSHIP SERIES =====");

        SeriesResult alcs = SimulateSeries(
            alDS1.winner, alDS2.winner, 7, "ALCS");
        SeriesResult nlcs = SimulateSeries(
            nlDS1.winner, nlDS2.winner, 7, "NLCS");

        Debug.Log("ALCS: " + alcs.Summary());
        Debug.Log("NLCS: " + nlcs.Summary());

        // LCS MVPs
        alLCSMVP = GetSeriesMVP(alcs.winner);
        nlLCSMVP = GetSeriesMVP(nlcs.winner);

        if (alLCSMVP != null)
            Debug.Log("ALCS MVP: " + alLCSMVP.FullName() +
                      " (" + alLCSMVP.team + ")");
        if (nlLCSMVP != null)
            Debug.Log("NLCS MVP: " + nlLCSMVP.FullName() +
                      " (" + nlLCSMVP.team + ")");

        // -------------------------------------------------------
        // WORLD SERIES — Best of 7
        // -------------------------------------------------------
        Debug.Log("\n===== WORLD SERIES =====");
        Debug.Log(alcs.winner.city + " " + alcs.winner.nickname +
                  " (AL) vs " +
                  nlcs.winner.city + " " + nlcs.winner.nickname +
                  " (NL)");

        SeriesResult ws = SimulateSeries(
            alcs.winner, nlcs.winner, 7, "World Series");

        Debug.Log("\n🏆 WORLD SERIES CHAMPION 🏆");
        Debug.Log(ws.Summary());
        Debug.Log(ws.winner.city + " " + ws.winner.nickname +
                  " ARE WORLD SERIES CHAMPIONS!");

        worldSeriesMVP = GetSeriesMVP(ws.winner);
        if (worldSeriesMVP != null)
            Debug.Log("World Series MVP: " +
                      worldSeriesMVP.FullName() +
                      " (" + worldSeriesMVP.team + ")");

        // Print postseason stats
        PrintPostseasonStats();
    }

    // -------------------------------------------------------
    // GET PLAYOFF TEAMS
    // Seeds 1-3: division winners (best record first)
    // Seeds 4-6: wild cards (best record first)
    // -------------------------------------------------------
    List<Team> GetPlayoffTeams(List<Team> leagueTeams)
    {
        var divisions     = leagueTeams.GroupBy(t => t.division);
        List<Team> divWinners = new List<Team>();

        foreach (var div in divisions)
        {
            Team winner = div.OrderByDescending(t => t.wins).First();
            divWinners.Add(winner);
        }

        divWinners = divWinners
            .OrderByDescending(t => t.wins).ToList();

        List<Team> wildCards = leagueTeams
            .Where(t => !divWinners.Contains(t))
            .OrderByDescending(t => t.wins)
            .Take(3).ToList();

        List<Team> playoff = new List<Team>();
        playoff.AddRange(divWinners);
        playoff.AddRange(wildCards);

        return playoff;
    }

    void PrintPlayoffField(string league, List<Team> teams)
    {
        Debug.Log("\n--- " + league + " Playoff Field ---");
        string[] labels = { "1", "2", "3", "WC1", "WC2", "WC3" };
        for (int i = 0; i < teams.Count; i++)
            Debug.Log("(" + labels[i] + ") " +
                      teams[i].city + " " + teams[i].nickname +
                      " " + teams[i].Record());
    }

    // -------------------------------------------------------
    // SERIES SIMULATOR
    // -------------------------------------------------------
    SeriesResult SimulateSeries(Team homeTeam, Team awayTeam,
                                 int bestOf, string seriesName)
    {
        int winsNeeded = (bestOf / 2) + 1;
        int homeWins   = 0;
        int awayWins   = 0;
        int gameNum    = 1;

        Debug.Log("\n" + seriesName + ": " +
                  homeTeam.city + " " + homeTeam.nickname +
                  " vs " +
                  awayTeam.city + " " + awayTeam.nickname);

        while (homeWins < winsNeeded && awayWins < winsNeeded)
        {
            bool homeField = IsHomeGame(gameNum, bestOf);
            Team actualHome = homeField ? homeTeam : awayTeam;
            Team actualAway = homeField ? awayTeam : homeTeam;

            int homeRuns = 0;
            int awayRuns = 0;
            SimulatePostseasonGame(actualHome, actualAway,
                                   out homeRuns, out awayRuns);

            bool homeTeamWon = homeField ?
                homeRuns > awayRuns :
                awayRuns > homeRuns;

            if (homeTeamWon) homeWins++;
            else             awayWins++;

            Debug.Log("  Game " + gameNum + ": " +
                      actualHome.abbreviation + " " + homeRuns +
                      " - " + awayRuns + " " +
                      actualAway.abbreviation +
                      " (" + homeTeam.abbreviation + " leads " +
                      homeWins + "-" + awayWins + ")");

            gameNum++;
            if (gameNum > bestOf + 2) break;
        }

        Team winner = homeWins >= winsNeeded ? homeTeam : awayTeam;
        Team loser  = homeWins >= winsNeeded ? awayTeam : homeTeam;
        int  ww     = homeWins >= winsNeeded ? homeWins : awayWins;
        int  lw     = homeWins >= winsNeeded ? awayWins : homeWins;

        return new SeriesResult(winner, loser, ww, lw, seriesName);
    }

    bool IsHomeGame(int gameNum, int bestOf)
    {
        if (bestOf == 3)
            return gameNum <= 2;
        else if (bestOf == 5)
            return gameNum == 1 || gameNum == 2 || gameNum == 5;
        else
            return gameNum == 1 || gameNum == 2 ||
                   gameNum == 6 || gameNum == 7;
    }

    void SimulatePostseasonGame(Team homeTeam, Team awayTeam,
                                 out int homeRuns, out int awayRuns)
    {
        homeRuns = 0;
        awayRuns = 0;

        Player homePitcher = homeTeam.GetStartingPitcher();
        Player awayPitcher = awayTeam.GetStartingPitcher();

        if (homePitcher == null || awayPitcher == null) return;

        homePitcher.inningsPitched = 0;
        homePitcher.earnedRuns     = 0;
        awayPitcher.inningsPitched = 0;
        awayPitcher.earnedRuns     = 0;

        homePitcher.InitializePitcher();
        awayPitcher.InitializePitcher();

        List<Player> homeLineup = homeTeam.roster
            .Where(p => p.position != "SP" && p.position != "RP")
            .ToList();
        List<Player> awayLineup = awayTeam.roster
            .Where(p => p.position != "SP" && p.position != "RP")
            .ToList();

        if (homeLineup.Count == 0 || awayLineup.Count == 0) return;

        int homeBatterIndex = 0;
        int awayBatterIndex = 0;
        int inning          = 1;
        int maxInnings      = 18;

        while (inning <= maxInnings)
        {
            awayRuns += SimulatePostseasonInning(
                awayLineup, homePitcher, ref awayBatterIndex);

            if (inning >= 9 && homeRuns > awayRuns) break;

            homeRuns += SimulatePostseasonInning(
                homeLineup, awayPitcher, ref homeBatterIndex);

            if (inning >= 9 && homeRuns != awayRuns) break;

            inning++;
        }

        foreach (Player p in homeLineup) AccumulatePostseasonStats(p);
        foreach (Player p in awayLineup) AccumulatePostseasonStats(p);
    }

    int SimulatePostseasonInning(List<Player> lineup, Player pitcher,
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
        float hr  = 3.0f  + (batter.power   - 50) / 200f * 2f;
        float tri = 0.5f;
        float dbl = 5.0f  + (batter.power   - 50) / 200f * 1f;
        float sng = 14.0f + (batter.contact - 50) / 200f * 3f;
        float bb  = 8.0f;
        float so  = 23.5f - (batter.contact - 50) / 200f * 3f;

        float pitchMod   = (pitcher.pitching - 50) / 200f;
        so  += pitchMod * 4f;
        sng -= pitchMod * 2f;
        hr  -= pitchMod * 1f;

        float fatigue    = pitcher.GetFatigueMultiplier(pitcher.inningsPitched);
        float fatiguePen = 1f - fatigue;
        hr  += fatiguePen * 5f;
        sng += fatiguePen * 4f;
        so  -= fatiguePen * 7f;

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
    // POSTSEASON STATS
    // -------------------------------------------------------
    void ResetPostseasonStats(List<Team> teams)
    {
        foreach (Team t in teams)
            if (t.roster != null)
                foreach (Player p in t.roster)
                {
                    p.atBats     = 0;
                    p.hits       = 0;
                    p.homeRuns   = 0;
                    p.rbi        = 0;
                    p.walks      = 0;
                    p.strikeouts = 0;
                    p.singles    = 0;
                    p.doubles    = 0;
                    p.triples    = 0;
                }
    }

    void AccumulatePostseasonStats(Player p)
    {
        if (!postseasonStats.ContainsKey(p.id))
        {
            Player ps    = new Player();
            ps.id        = p.id;
            ps.firstName = p.firstName;
            ps.lastName  = p.lastName;
            ps.position  = p.position;
            ps.team      = p.team;
            postseasonStats[p.id] = ps;
        }

        Player stats      = postseasonStats[p.id];
        stats.atBats     += p.atBats;
        stats.hits       += p.hits;
        stats.homeRuns   += p.homeRuns;
        stats.rbi        += p.rbi;
        stats.walks      += p.walks;
        stats.strikeouts += p.strikeouts;
        stats.singles    += p.singles;
        stats.doubles    += p.doubles;
        stats.triples    += p.triples;

        p.atBats = p.hits = p.homeRuns = p.rbi = 0;
        p.walks  = p.strikeouts = p.singles    = 0;
        p.doubles = p.triples   = 0;
    }

    Player GetSeriesMVP(Team winner)
    {
        Player mvp    = null;
        float bestOPS = 0f;

        if (winner.roster == null) return null;

        foreach (Player p in winner.roster)
        {
            if (p.position == "SP" || p.position == "RP") continue;
            if (!postseasonStats.ContainsKey(p.id)) continue;

            Player ps  = postseasonStats[p.id];
            float obp  = (ps.atBats + ps.walks) > 0 ?
                         (float)(ps.hits + ps.walks) /
                         (ps.atBats + ps.walks) : 0f;
            float slg  = ps.atBats > 0 ?
                         (float)(ps.singles + (ps.doubles * 2) +
                         (ps.triples * 3) + (ps.homeRuns * 4)) /
                         ps.atBats : 0f;
            float ops  = obp + slg;

            if (ops > bestOPS)
            {
                bestOPS = ops;
                mvp     = p;
            }
        }

        return mvp;
    }

    void PrintPostseasonStats()
    {
        Debug.Log("\n========== POSTSEASON BATTING LEADERS ==========");

        List<Player> leaders = postseasonStats.Values
            .Where(p => p.atBats >= 10)
            .OrderByDescending(p => p.BattingAverage())
            .ToList();

        Debug.Log("\n-- Postseason Batting Average --");
        int rank = 1;
        foreach (Player p in leaders.Take(10))
        {
            Debug.Log(rank + ". " + p.FullName().PadRight(20) +
                      p.team.PadRight(6) +
                      p.BattingAverage().ToString("F3") +
                      " (" + p.hits + "/" + p.atBats + ")" +
                      " HR: "  + p.homeRuns +
                      " RBI: " + p.rbi);
            rank++;
        }

        Debug.Log("\n-- Postseason Home Runs --");
        rank = 1;
        foreach (Player p in postseasonStats.Values
            .OrderByDescending(p => p.homeRuns).Take(10))
        {
            Debug.Log(rank + ". " + p.FullName().PadRight(20) +
                      p.team.PadRight(6) +
                      p.homeRuns + " HR");
            rank++;
        }
    }
}
