using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class FranchiseData
{
    public string playerTeamAbbreviation;
    public string gmName;
    public int    difficulty;        // 0=Easy, 1=Normal, 2=Hard
    public int    currentSeason;     // 2026, 2027, 2028...
    public int    totalSeasons;      // how many seasons played
    public bool   franchiseStarted;

    // Season history
    public List<SeasonRecord> seasonHistory;

    public FranchiseData()
    {
        currentSeason    = 2026;
        totalSeasons     = 0;
        franchiseStarted = false;
        seasonHistory    = new List<SeasonRecord>();
    }
}

[System.Serializable]
public class SeasonRecord
{
    public int    season;
    public string teamAbbreviation;
    public int    wins;
    public int    losses;
    public string finishPosition; // "World Series Champions", "ALCS", etc
    public float  teamBudget;
}

public class FranchiseManager : MonoBehaviour
{
    public FranchiseData franchise;

    void Awake()
    {
        franchise = new FranchiseData();
    }

    // -------------------------------------------------------
    // START NEW FRANCHISE
    // -------------------------------------------------------
    public void StartNewFranchise(string teamAbbreviation,
                                   string gmName,
                                   int difficulty)
    {
        franchise.playerTeamAbbreviation = teamAbbreviation;
        franchise.gmName                 = gmName;
        franchise.difficulty             = difficulty;
        franchise.currentSeason          = 2026;
        franchise.totalSeasons           = 0;
        franchise.franchiseStarted       = true;
        franchise.seasonHistory          = new List<SeasonRecord>();

        Debug.Log("=== FRANCHISE STARTED ===");
        Debug.Log("GM: "         + gmName);
        Debug.Log("Team: "       + teamAbbreviation);
        Debug.Log("Difficulty: " + GetDifficultyName(difficulty));
        Debug.Log("Season: "     + franchise.currentSeason);
    }

    // -------------------------------------------------------
    // ADVANCE TO NEXT SEASON
    // -------------------------------------------------------
    public void AdvanceToNextSeason(List<Team> allTeams,
                                     string worldSeriesWinner,
                                     string playerTeamFinish)
    {
        // Record this season
        Team playerTeam = allTeams.FirstOrDefault(
            t => t.abbreviation == franchise.playerTeamAbbreviation);

        if (playerTeam != null)
        {
            SeasonRecord record = new SeasonRecord();
            record.season            = franchise.currentSeason;
            record.teamAbbreviation  = franchise.playerTeamAbbreviation;
            record.wins              = playerTeam.wins;
            record.losses            = playerTeam.losses;
            record.finishPosition    = playerTeamFinish;
            record.teamBudget        = playerTeam.budget;
            franchise.seasonHistory.Add(record);
        }

        // Advance season
        franchise.currentSeason++;
        franchise.totalSeasons++;

        Debug.Log("\n=== SEASON " + (franchise.currentSeason - 1) +
                  " COMPLETE ===");
        Debug.Log("World Series Champion: " + worldSeriesWinner);
        Debug.Log("Advancing to " + franchise.currentSeason + " season...");

        // Age all players
        AgeAllPlayers(allTeams);

        // Develop young players
        DevelopPlayers(allTeams);

        // Reset team records
        ResetTeamRecords(allTeams);

        Debug.Log("=== " + franchise.currentSeason +
                  " SEASON READY ===");
    }

    // -------------------------------------------------------
    // PLAYER AGING
    // Everyone ages 1 year between seasons
    // -------------------------------------------------------
    void AgeAllPlayers(List<Team> allTeams)
    {
        int aged = 0;
        foreach (Team t in allTeams)
        {
            if (t.roster != null)
                foreach (Player p in t.roster)
                {
                    p.age++;
                    aged++;
                }

            if (t.aaaRoster != null)
                foreach (Player p in t.aaaRoster)
                    p.age++;

            if (t.aaRoster != null)
                foreach (Player p in t.aaRoster)
                    p.age++;

            if (t.aRoster != null)
                foreach (Player p in t.aRoster)
                    p.age++;
        }

        Debug.Log("Aged " + aged + " MLB players");
    }

    // -------------------------------------------------------
    // PLAYER DEVELOPMENT
    // Young players improve, old players decline
    // -------------------------------------------------------
    void DevelopPlayers(List<Team> allTeams)
    {
        int improved = 0;
        int declined = 0;

        foreach (Team t in allTeams)
        {
            if (t.roster == null) continue;

            foreach (Player p in t.roster)
            {
                int change = CalculateDevelopment(p);

                if (change > 0)
                {
                    ApplyDevelopment(p, change);
                    improved++;
                }
                else if (change < 0)
                {
                    ApplyDevelopment(p, change);
                    declined++;
                }
            }
        }

        Debug.Log("Development: " + improved +
                  " improved, " + declined + " declined");
    }

    int CalculateDevelopment(Player p)
    {
        // Peak age is 27-29
        // Players improve 18-26, plateau 27-29, decline 30+

        if (p.age <= 22)
        {
            // Young prospects — big improvement chance
            float roll = Random.value;
            if (roll < 0.7f) return Random.Range(2, 6);
            if (roll < 0.9f) return Random.Range(0, 2);
            return 0;
        }
        else if (p.age <= 26)
        {
            // Still developing
            float roll = Random.value;
            if (roll < 0.5f) return Random.Range(1, 4);
            if (roll < 0.8f) return 0;
            return -Random.Range(1, 2);
        }
        else if (p.age <= 29)
        {
            // Peak — slight chance of improvement or decline
            float roll = Random.value;
            if (roll < 0.2f) return Random.Range(1, 3);
            if (roll < 0.7f) return 0;
            return -Random.Range(1, 2);
        }
        else if (p.age <= 33)
        {
            // Early decline
            float roll = Random.value;
            if (roll < 0.1f) return Random.Range(1, 2);
            if (roll < 0.4f) return 0;
            return -Random.Range(1, 3);
        }
        else if (p.age <= 36)
        {
            // Steep decline
            float roll = Random.value;
            if (roll < 0.2f) return 0;
            return -Random.Range(2, 5);
        }
        else
        {
            // Veteran decline
            return -Random.Range(3, 7);
        }
    }

    void ApplyDevelopment(Player p, int change)
    {
        bool isPitcher = p.position == "SP" || p.position == "RP";

        if (isPitcher)
        {
            p.pitching = Mathf.Clamp(p.pitching + change, 40, 99);
            p.stamina  = Mathf.Clamp(p.stamina  + change, 40, 99);
            p.overall  = p.pitching;
        }
        else
        {
            p.contact  = Mathf.Clamp(p.contact  + change, 40, 99);
            p.power    = Mathf.Clamp(p.power    + change, 40, 99);
            p.fielding = Mathf.Clamp(p.fielding + change, 40, 99);

            // Speed declines faster with age
            if (change < 0)
                p.speed = Mathf.Clamp(p.speed + change * 2, 30, 99);

            p.overall = (p.contact + p.power + p.speed +
                         p.arm + p.fielding) / 5;
        }

        // Cap overall
        p.overall = Mathf.Clamp(p.overall, 40, 99);
    }

    // -------------------------------------------------------
    // RESET TEAM RECORDS FOR NEW SEASON
    // -------------------------------------------------------
    void ResetTeamRecords(List<Team> allTeams)
    {
        foreach (Team t in allTeams)
        {
            t.wins        = 0;
            t.losses      = 0;
            t.runsScored  = 0;
            t.runsAllowed = 0;
        }
    }

    // -------------------------------------------------------
    // FRANCHISE HISTORY
    // -------------------------------------------------------
    public void PrintFranchiseHistory()
    {
        Debug.Log("\n========== FRANCHISE HISTORY ==========");
        Debug.Log("GM: " + franchise.gmName);
        Debug.Log("Team: " + franchise.playerTeamAbbreviation);
        Debug.Log("Seasons: " + franchise.totalSeasons);

        foreach (SeasonRecord r in franchise.seasonHistory)
        {
            Debug.Log(r.season + " — " +
                      r.wins + "-" + r.losses +
                      " | " + r.finishPosition);
        }
    }

    // -------------------------------------------------------
    // TEAM SELECTION — Show all 30 teams
    // -------------------------------------------------------
    public void PrintTeamSelectionMenu(List<Team> allTeams)
    {
        Debug.Log("\n========== SELECT YOUR TEAM ==========");

        string[] divisions = new string[]
        {
            "AL East", "AL Central", "AL West",
            "NL East", "NL Central", "NL West"
        };

        foreach (string div in divisions)
        {
            Debug.Log("\n--- " + div + " ---");
            List<Team> divTeams = allTeams
                .Where(t => t.division == div)
                .ToList();

            foreach (Team t in divTeams)
            {
                Debug.Log(t.abbreviation.PadRight(5) +
                          " | " + (t.city + " " + t.nickname).PadRight(25) +
                          " | Budget: $" + t.budget + "M");
            }
        }
    }

    // -------------------------------------------------------
    // DIFFICULTY SETTINGS
    // -------------------------------------------------------
    public string GetDifficultyName(int difficulty)
    {
        switch (difficulty)
        {
            case 0:  return "Easy";
            case 1:  return "Normal";
            case 2:  return "Hard";
            default: return "Normal";
        }
    }

    public float GetDifficultyBudgetModifier(int difficulty)
    {
        switch (difficulty)
        {
            case 0:  return 1.3f;  // Easy — 30% more budget
            case 1:  return 1.0f;  // Normal
            case 2:  return 0.8f;  // Hard — 20% less budget
            default: return 1.0f;
        }
    }

    public bool GetDifficultyTradeHelp(int difficulty)
    {
        // Easy mode shows trade value hints
        return difficulty == 0;
    }
}
