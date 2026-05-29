using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class GameEntry
{
    public string homeTeam;
    public string awayTeam;
    public int gameNumber;
    public bool isPlayed;
    public int homeScore;
    public int awayScore;

    public GameEntry(string home, string away, int number)
    {
        homeTeam   = home;
        awayTeam   = away;
        gameNumber = number;
        isPlayed   = false;
        homeScore  = 0;
        awayScore  = 0;
    }
}

[System.Serializable]
public class SeasonSchedule
{
    public List<GameEntry> allGames = new List<GameEntry>();
    public int currentGameIndex     = 0;
    public int totalGames           = 0;
}

public class SeasonScheduler : MonoBehaviour
{
    // Official geographic rivals using new abbreviations
    private Dictionary<string, string> rivals = new Dictionary<string, string>
    {
        { "NYA", "NYC" }, { "NYC", "NYA" },
        { "BST", "ATB" }, { "ATB", "BST" },
        { "TRN", "PHF" }, { "PHF", "TRN" },
        { "BLT", "WAS" }, { "WAS", "BLT" },
        { "TBS", "MMP" }, { "MMP", "TBS" },
        { "CHH", "CHW" }, { "CHW", "CHH" },
        { "CLN", "CNR" }, { "CNR", "CLN" },
        { "DTE", "PGI" }, { "PGI", "DTE" },
        { "KCP", "SLA" }, { "SLA", "KCP" },
        { "MNV", "MWB" }, { "MWB", "MNV" },
        { "HST", "COP" }, { "COP", "HST" },
        { "LAC", "LAB" }, { "LAB", "LAC" },
        { "OKP", "SFF" }, { "SFF", "OKP" },
        { "SET", "SDS" }, { "SDS", "SET" },
        { "TXL", "AZS" }, { "AZS", "TXL" },
    };

    public SeasonSchedule GenerateSchedule(List<Team> allTeams)
    {
        SeasonSchedule schedule = new SeasonSchedule();
        int gameNumber = 1;

        List<Team> alTeams = allTeams.Where(t => t.league == "AL").ToList();
        List<Team> nlTeams = allTeams.Where(t => t.league == "NL").ToList();

        // -------------------------------------------------------
        // PHASE 1: DIVISION GAMES — 52 per team
        // 4 rivals x 13 games
        // i < j so each pair scheduled once
        // -------------------------------------------------------
        foreach (var grp in allTeams.GroupBy(t => t.division))
        {
            var d = grp.ToList();
            for (int i = 0; i < d.Count; i++)
            for (int j = i + 1; j < d.Count; j++)
            {
                gameNumber = Add(schedule, d[i].abbreviation,
                                 d[j].abbreviation, 7, gameNumber);
                gameNumber = Add(schedule, d[j].abbreviation,
                                 d[i].abbreviation, 6, gameNumber);
            }
        }

        Verify("Phase 1 - Division", schedule, allTeams, 52);

        // -------------------------------------------------------
        // PHASE 2: INTRALEAGUE NON-DIVISION — 62 per team
        // Step 2a: base 6 games vs every non-div opponent
        // Step 2b: 1 extra game to bring total to 62
        // -------------------------------------------------------

        // Step 2a: base 6 games
        foreach (var league in new[] { alTeams, nlTeams })
        {
            for (int i = 0; i < league.Count; i++)
            for (int j = i + 1; j < league.Count; j++)
            {
                Team a = league[i];
                Team b = league[j];
                if (a.division == b.division) continue;
                gameNumber = Add(schedule, a.abbreviation,
                                 b.abbreviation, 3, gameNumber);
                gameNumber = Add(schedule, b.abbreviation,
                                 a.abbreviation, 3, gameNumber);
            }
        }

        Verify("Phase 2a - Intraleague base", schedule, allTeams, 112);

        // Step 2b: extra games to reach 62
        // AL extras
        List<string[]> alExtras = new List<string[]>
        {
            new string[] { "NYA", "CHH" },
            new string[] { "BST", "CLN" },
            new string[] { "TRN", "DTE" },
            new string[] { "BLT", "KCP" },
            new string[] { "TBS", "MNV" },
            new string[] { "CHH", "HST" },
            new string[] { "CLN", "LAC" },
            new string[] { "DTE", "OKP" },
            new string[] { "KCP", "SET" },
            new string[] { "MNV", "TXL" },
            new string[] { "HST", "NYA" },
            new string[] { "LAC", "BST" },
            new string[] { "OKP", "TRN" },
            new string[] { "SET", "BLT" },
            new string[] { "TXL", "TBS" },
        };

        // NL extras
        List<string[]> nlExtras = new List<string[]>
        {
            new string[] { "ATB", "CHW" },
            new string[] { "MMP", "CNR" },
            new string[] { "NYC", "MWB" },
            new string[] { "PHF", "PGI" },
            new string[] { "WAS", "SLA" },
            new string[] { "CHW", "LAB" },
            new string[] { "CNR", "AZS" },
            new string[] { "MWB", "SFF" },
            new string[] { "PGI", "SDS" },
            new string[] { "SLA", "COP" },
            new string[] { "LAB", "ATB" },
            new string[] { "AZS", "MMP" },
            new string[] { "SFF", "NYC" },
            new string[] { "SDS", "PHF" },
            new string[] { "COP", "WAS" },
        };

        foreach (string[] pair in alExtras)
            gameNumber = Add(schedule, pair[0], pair[1], 1, gameNumber);
        foreach (string[] pair in nlExtras)
            gameNumber = Add(schedule, pair[0], pair[1], 1, gameNumber);

        Verify("Phase 2b - Intraleague extra", schedule, allTeams, 114);

        // -------------------------------------------------------
        // PHASE 3: INTERLEAGUE — 48 per team
        // Step 3a: base 3 games vs every interleague opponent
        // Step 3b: 3 bonus games vs geographic rival
        // -------------------------------------------------------

        // Step 3a: base 3 games
        for (int i = 0; i < alTeams.Count; i++)
        for (int j = 0; j < nlTeams.Count; j++)
        {
            Team al = alTeams[i];
            Team nl = nlTeams[j];

            if ((i + j) % 2 == 0)
                gameNumber = Add(schedule, al.abbreviation,
                                 nl.abbreviation, 3, gameNumber);
            else
                gameNumber = Add(schedule, nl.abbreviation,
                                 al.abbreviation, 3, gameNumber);
        }

        Verify("Phase 3a - Interleague base", schedule, allTeams, 159);

        // Step 3b: 3 bonus games vs rival (AL hosts)
        foreach (var pair in rivals)
        {
            // Only process AL teams to avoid double scheduling
            Team alTeam = alTeams.FirstOrDefault(
                t => t.abbreviation == pair.Key);
            if (alTeam == null) continue;

            gameNumber = Add(schedule, pair.Key, pair.Value, 3, gameNumber);
        }

        Verify("Phase 3b - Rival bonus", schedule, allTeams, 162);

        schedule.totalGames = schedule.allGames.Count;

        Debug.Log("\n=== SCHEDULE COMPLETE ===");
        Debug.Log("Total: " + schedule.totalGames + " (target 2430)");

        int min = 999, max = 0;
        foreach (Team t in allTeams)
        {
            int tg = schedule.allGames
                .Count(g => g.homeTeam == t.abbreviation ||
                            g.awayTeam == t.abbreviation);
            min = Mathf.Min(min, tg);
            max = Mathf.Max(max, tg);
        }
        Debug.Log("Min: " + min + " | Max: " + max);

        if (min == 162 && max == 162)
            Debug.Log("PERFECT — all 30 teams have exactly 162 games!");

        return schedule;
    }

    void Verify(string phase, SeasonSchedule schedule,
                List<Team> allTeams, int expected)
    {
        bool allCorrect = true;
        foreach (Team t in allTeams)
        {
            int count = schedule.allGames
                .Count(g => g.homeTeam == t.abbreviation ||
                            g.awayTeam == t.abbreviation);
            if (count != expected)
            {
                Debug.LogWarning(phase + " — " + t.abbreviation +
                                 ": " + count +
                                 " (expected " + expected + ")");
                allCorrect = false;
            }
        }
        if (allCorrect)
            Debug.Log(phase + " — all correct at " + expected + " ✓");
    }

    int Add(SeasonSchedule schedule, string home,
            string away, int count, int num)
    {
        for (int i = 0; i < count; i++)
            schedule.allGames.Add(new GameEntry(home, away, num++));
        return num;
    }
}
