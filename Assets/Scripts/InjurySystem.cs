using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class InjurySystem : MonoBehaviour
{
    private MiLBGenerator milbGenerator;

    // Injury types by position type
    private string[] pitcherInjuries = new string[]
    {
        "Elbow Inflammation", "UCL Strain", "Shoulder Fatigue",
        "Forearm Tightness", "Blister", "Back Tightness",
        "Hip Flexor", "Oblique Strain", "Biceps Tendinitis"
    };

    private string[] batterInjuries = new string[]
    {
        "Hamstring Strain", "Quad Strain", "Calf Strain",
        "Oblique Strain", "Wrist Soreness", "Thumb Sprain",
        "Back Tightness", "Knee Soreness", "Ankle Sprain",
        "Shoulder Soreness", "Hamate Fracture", "Groin Strain"
    };

    void Start()
    {
        milbGenerator = GetComponent<MiLBGenerator>();
    }

    // -------------------------------------------------------
    // PROCESS INJURIES FOR ONE GAME DAY
    // Call this once per game simulated
    // -------------------------------------------------------
    public void ProcessGameDay(List<Team> allTeams, int gameNumber)
    {
        foreach (Team t in allTeams)
        {
            // Check each player for injury
            CheckForNewInjuries(t, gameNumber);

            // Update existing injuries
            UpdateExistingInjuries(t, gameNumber);
        }
    }

    // -------------------------------------------------------
    // CHECK FOR NEW INJURIES
    // -------------------------------------------------------
    void CheckForNewInjuries(Team team, int gameNumber)
    {
        if (team.roster == null) return;

        // Only active non-injured players can get hurt
        List<Player> activePlayers = team.roster
            .Where(p => !p.isInjured && !p.isOnIL)
            .ToList();

        foreach (Player p in activePlayers)
        {
            // Base injury chance per game
            // Real MLB: roughly 1-2 injuries per team per week
            // ~162 games, ~25 players, so ~0.05% per player per game
            float injuryChance = 0.003f; // 0.3% per game

            // Pitchers slightly more likely to get hurt
            if (p.position == "SP") injuryChance = 0.005f;
            if (p.position == "RP") injuryChance = 0.004f;

            // Older players more injury prone
            if (p.age >= 35) injuryChance *= 1.5f;
            if (p.age >= 38) injuryChance *= 2.0f;

            // Roll for injury
            if (Random.value < injuryChance)
            {
                InjurePlayer(p, team, gameNumber);
            }
        }
    }

    // -------------------------------------------------------
    // INJURE A PLAYER
    // -------------------------------------------------------
    void InjurePlayer(Player p, Team team, int gameNumber)
    {
        bool isPitcher = p.position == "SP" || p.position == "RP";

        // Pick injury type
        string[] injuryPool = isPitcher ? pitcherInjuries : batterInjuries;
        p.injuryType = injuryPool[Random.Range(0, injuryPool.Length)];

        // Determine severity
        float severityRoll = Random.value;
        int days;

        if (severityRoll < 0.4f)
        {
            // Minor — Day to Day (1-9 days)
            days = Random.Range(1, 10);
            p.injuryStatus = "Day-to-Day";
            p.isOnIL       = false;
        }
        else if (severityRoll < 0.75f)
        {
            // Moderate — 10-Day IL
            days = Random.Range(10, 30);
            p.injuryStatus = "10-Day IL";
            p.isOnIL       = true;
        }
        else if (severityRoll < 0.92f)
        {
            // Serious — 60-Day IL
            days = Random.Range(30, 75);
            p.injuryStatus = "60-Day IL";
            p.isOnIL       = true;
        }
        else
        {
            // Season ending
            days = Random.Range(75, 162);
            p.injuryStatus = "Season-Ending";
            p.isOnIL       = true;
        }

        p.isInjured            = true;
        p.injuryDaysTotal      = days;
        p.injuryDaysRemaining  = days;

        Debug.Log("INJURY: " + p.FullName() +
                  " (" + team.abbreviation + ") — " +
                  p.injuryType + " | " + p.injuryStatus +
                  " | " + days + " days");

        // Auto call up if player goes on IL
        if (p.isOnIL && milbGenerator != null)
        {
            Player callUp = milbGenerator.GetCallUp(team, p.position);
            if (callUp != null)
            {
                milbGenerator.CallUp(team, callUp);
            }
        }
    }

    // -------------------------------------------------------
    // UPDATE EXISTING INJURIES
    // -------------------------------------------------------
    void UpdateExistingInjuries(Team team, int gameNumber)
    {
        if (team.roster == null) return;

        List<Player> injured = team.roster
            .Where(p => p.isInjured)
            .ToList();

        foreach (Player p in injured)
        {
            p.injuryDaysRemaining--;

            if (p.injuryDaysRemaining <= 0)
            {
                // Player has recovered
                RecoverPlayer(p, team);
            }
        }
    }

    // -------------------------------------------------------
    // RECOVER A PLAYER
    // -------------------------------------------------------
    void RecoverPlayer(Player p, Team team)
    {
        p.isInjured           = false;
        p.isOnIL              = false;
        p.injuryDaysRemaining = 0;
        p.injuryDaysTotal     = 0;
        string injury         = p.injuryType;
        p.injuryType          = "";
        p.injuryStatus        = "";

        Debug.Log("RETURN: " + p.FullName() +
                  " (" + team.abbreviation + ") — " +
                  "Activated from IL. Recovered from " + injury);

        // If a call up was made send someone down
        // Find AAA player on MLB roster and send down
        if (team.roster.Count > 26 && milbGenerator != null)
        {
            // Find most recently called up AAA player
            Player sendDown = team.roster
                .Where(r => r.minorLeagueLevel == "" &&
                            r.overall < 70 &&
                            r.id >= 10000) // Generated players have high IDs
                .OrderBy(r => r.overall)
                .FirstOrDefault();

            if (sendDown != null)
                milbGenerator.SendDown(team, sendDown);
        }
    }

    // -------------------------------------------------------
    // INJURY REPORT
    // -------------------------------------------------------
    public void PrintInjuryReport(List<Team> allTeams)
    {
        Debug.Log("\n========== INJURY REPORT ==========");

        int totalInjured = 0;

        foreach (Team t in allTeams)
        {
            if (t.roster == null) continue;

            List<Player> injured = t.roster
                .Where(p => p.isInjured)
                .ToList();

            if (injured.Count == 0) continue;

            totalInjured += injured.Count;

            Debug.Log("\n" + t.city + " " + t.nickname +
                      " (" + injured.Count + " injured):");

            foreach (Player p in injured)
            {
                Debug.Log("  " + p.FullName().PadRight(20) +
                          " | " + p.position.PadRight(3) +
                          " | " + p.injuryType.PadRight(22) +
                          " | " + p.injuryStatus.PadRight(14) +
                          " | " + p.injuryDaysRemaining + " days left");
            }
        }

        Debug.Log("\nTotal injured: " + totalInjured + " players");
    }

    // -------------------------------------------------------
    // SEASON INJURY SUMMARY
    // -------------------------------------------------------
    public void PrintSeasonInjurySummary(List<Team> allTeams)
    {
        Debug.Log("\n========== SEASON INJURY SUMMARY ==========");

        foreach (Team t in allTeams)
        {
            if (t.roster == null) continue;

            int onIL = t.roster.Count(p => p.isOnIL);
            if (onIL > 0)
                Debug.Log(t.city + " " + t.nickname +
                          " — " + onIL + " on IL");
        }
    }
}
