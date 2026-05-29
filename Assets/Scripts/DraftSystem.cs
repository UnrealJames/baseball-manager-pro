using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DraftSystem : MonoBehaviour
{
    // Draft prospect name pools
    private string[] firstNames = new string[]
    {
        "Jackson", "Cade", "Brady", "Cole", "Tanner",
        "Hunter", "Blake", "Chase", "Bryce", "Drew",
        "Wyatt", "Logan", "Mason", "Dylan", "Tyler",
        "Zach", "Kyle", "Ryan", "Jake", "Austin",
        "Carter", "Luke", "Noah", "Liam", "Ethan",
        "Aiden", "Connor", "Gavin", "Owen", "Evan",
        "Carlos", "Miguel", "Jose", "Juan", "Luis",
        "Diego", "Marco", "Rafael", "Eduardo", "Pedro",
        "Yuki", "Kenji", "Hiroshi", "Ryu", "Sota",
        "Ji", "Hyun", "Sung", "Kang", "Young"
    };

    private string[] lastNames = new string[]
    {
        "Walkers", "Millers", "Daviss", "Wilsons", "Moores",
        "Taylors", "Andersons", "Thomass", "Jacksons", "Whites",
        "Harriss", "Martins", "Thompsons", "Garcias", "Martinezz",
        "Rodrigues", "Lopezz", "Gonzalezz", "Perezz", "Sanchez",
        "Greens", "Halls", "Youngs", "Allens", "Scotts",
        "Adamss", "Nelsons", "Carters", "Mitchells", "Roberts",
        "Turners", "Phillipss", "Campbells", "Parkers", "Edwardss",
        "Collinss", "Stewarts", "Morriss", "Rogerss", "Reedss",
        "Coopers", "Baileys", "Bellss", "Murphys", "Riveras",
        "Yamamoto", "Tanaka", "Suzukis", "Watanabe", "Nakamura"
    };

    private string[] positions = new string[]
    {
        "SP", "SP", "SP",           // Pitchers most common
        "RP", "RP",
        "C", "1B", "2B", "3B", "SS",
        "LF", "CF", "RF"
    };

    private int draftIdCounter = 50000;

    // -------------------------------------------------------
    // GENERATE DRAFT CLASS
    // Creates prospects for each round
    // -------------------------------------------------------
    public List<Player> GenerateDraftClass(int year, int rounds)
    {
        List<Player> draftClass = new List<Player>();

        for (int round = 1; round <= rounds; round++)
        {
            // 30 picks per round (one per team)
            for (int pick = 1; pick <= 30; pick++)
            {
                Player prospect = GenerateProspect(round, pick, year);
                draftClass.Add(prospect);
            }
        }

        Debug.Log(year + " Draft Class generated: " +
                  draftClass.Count + " prospects");
        return draftClass;
    }

    Player GenerateProspect(int round, int pick, int year)
    {
        Player p = new Player();
        p.id     = draftIdCounter++;

        // Name
        p.firstName = firstNames[
            Random.Range(0, firstNames.Length)];
        p.lastName  = lastNames[
            Random.Range(0, lastNames.Length)];

        // Position
        p.position = positions[
            Random.Range(0, positions.Length)];

        // Age — draft picks are 18-22
        p.age = Random.Range(18, 23);

        // Ratings based on round
        int minRating, maxRating;
        if (round == 1)
        {
            minRating = 45; maxRating = 65;
        }
        else if (round <= 3)
        {
            minRating = 38; maxRating = 55;
        }
        else if (round <= 10)
        {
            minRating = 32; maxRating = 48;
        }
        else
        {
            minRating = 28; maxRating = 42;
        }

        bool isPitcher = p.position == "SP" || p.position == "RP";

        if (isPitcher)
        {
            p.pitching  = Random.Range(minRating, maxRating);
            p.stamina   = Random.Range(minRating, maxRating);
            p.overall   = p.pitching;
            p.throwingArm = Random.value > 0.25f ? "R" : "L";
            p.battingHand = "R";

            if (p.position == "RP")
                p.bullpenRole = "MR";
        }
        else
        {
            p.contact   = Random.Range(minRating, maxRating);
            p.power     = Random.Range(minRating, maxRating);
            p.speed     = Random.Range(minRating, maxRating);
            p.arm       = Random.Range(minRating, maxRating);
            p.fielding  = Random.Range(minRating, maxRating);
            p.overall   = (p.contact + p.power + p.speed +
                           p.arm + p.fielding) / 5;

            float handRoll = Random.value;
            p.battingHand = handRoll < 0.65f ? "R" :
                            handRoll < 0.88f ? "L" : "S";
            p.throwingArm = "R";
        }

        // Draft picks start on minimum salary
        p.salary          = 0.72f;
        p.contractYears   = 6; // 6 years team control
        p.minorLeagueLevel = "A"; // Start in A ball
        p.isInjured       = false;
        p.confidence      = 50f;

        return p;
    }

    // -------------------------------------------------------
    // RUN THE DRAFT
    // Simulates all 30 rounds
    // -------------------------------------------------------
    public void RunDraft(List<Team> allTeams,
                          Team playerTeam,
                          int year,
                          int rounds = 5)
    {
        Debug.Log("\n========== " + year +
                  " AMATEUR DRAFT ==========");

        // Generate draft class
        List<Player> draftClass = GenerateDraftClass(year, rounds);

        // Determine draft order (worst record first)
        List<Team> draftOrder = allTeams
            .OrderBy(t => t.wins)
            .ThenByDescending(t => t.losses)
            .ToList();

        int pickIndex = 0;

        for (int round = 1; round <= rounds; round++)
        {
            Debug.Log("\n--- Round " + round + " ---");

            foreach (Team team in draftOrder)
            {
                if (pickIndex >= draftClass.Count) break;

                Player prospect = draftClass[pickIndex];
                pickIndex++;

                // Assign to team
                prospect.team = team.abbreviation;

                // Add to A roster
                if (team.aRoster == null)
                    team.aRoster = new List<Player>();
                team.aRoster.Add(prospect);

                // Log player team picks
                if (team == playerTeam)
                {
                    Debug.Log("⭐ YOUR PICK — Round " + round +
                              ": " + prospect.FullName() +
                              " | " + prospect.position +
                              " | Age: " + prospect.age +
                              " | OVR: " + prospect.overall);
                }
                else
                {
                    Debug.Log("Round " + round +
                              " Pick " + (pickIndex % 30 == 0
                                ? 30 : pickIndex % 30) +
                              ": " + team.abbreviation +
                              " selects " + prospect.FullName() +
                              " (" + prospect.position + ")");
                }
            }
        }

        Debug.Log("\nDraft complete! " +
                  "All picks assigned to A ball rosters.");
    }

    // -------------------------------------------------------
    // PROMOTE PROSPECTS
    // Move players up through minor league system
    // -------------------------------------------------------
    public void PromoteProspects(List<Team> allTeams)
    {
        Debug.Log("\n--- Prospect Promotions ---");
        int promoted = 0;

        foreach (Team t in allTeams)
        {
            // A → AA: overall 50+ and age 21+
            if (t.aRoster != null)
            {
                List<Player> readyForAA = t.aRoster
                    .Where(p => p.overall >= 50 && p.age >= 21)
                    .ToList();

                foreach (Player p in readyForAA)
                {
                    t.aRoster.Remove(p);
                    if (t.aaRoster == null)
                        t.aaRoster = new List<Player>();
                    p.minorLeagueLevel = "AA";
                    t.aaRoster.Add(p);
                    promoted++;
                }
            }

            // AA → AAA: overall 60+ and age 23+
            if (t.aaRoster != null)
            {
                List<Player> readyForAAA = t.aaRoster
                    .Where(p => p.overall >= 60 && p.age >= 23)
                    .ToList();

                foreach (Player p in readyForAAA)
                {
                    t.aaRoster.Remove(p);
                    if (t.aaaRoster == null)
                        t.aaaRoster = new List<Player>();
                    p.minorLeagueLevel = "AAA";
                    t.aaaRoster.Add(p);
                    promoted++;
                }
            }

            // AAA → MLB: overall 68+ and age 24+
            if (t.aaaRoster != null)
            {
                List<Player> readyForMLB = t.aaaRoster
                    .Where(p => p.overall >= 68 &&
                                p.age >= 24 &&
                                t.roster != null &&
                                t.roster.Count < 26)
                    .ToList();

                foreach (Player p in readyForMLB)
                {
                    t.aaaRoster.Remove(p);
                    if (t.roster == null)
                        t.roster = new List<Player>();
                    p.minorLeagueLevel = "";
                    t.roster.Add(p);
                    promoted++;

                    Debug.Log("MLB READY: " + p.FullName() +
                              " promoted to " +
                              t.city + " " + t.nickname +
                              " (OVR: " + p.overall + ")");
                }
            }
        }

        Debug.Log("Total promotions: " + promoted);
    }

    // -------------------------------------------------------
    // PRINT TEAM DRAFT PICKS
    // -------------------------------------------------------
    public void PrintTeamProspects(Team team)
    {
        Debug.Log("\n=== " + team.city + " " +
                  team.nickname + " PROSPECTS ===");

        Debug.Log("\n-- AAA --");
        if (team.aaaRoster != null)
        {
            var sorted = team.aaaRoster
                .OrderByDescending(p => p.overall).ToList();
            foreach (Player p in sorted)
                Debug.Log(p.FullName().PadRight(22) +
                          " | " + p.position.PadRight(3) +
                          " | OVR: " + p.overall +
                          " | Age: " + p.age);
        }

        Debug.Log("\n-- AA --");
        if (team.aaRoster != null)
        {
            var sorted = team.aaRoster
                .OrderByDescending(p => p.overall).ToList();
            foreach (Player p in sorted)
                Debug.Log(p.FullName().PadRight(22) +
                          " | " + p.position.PadRight(3) +
                          " | OVR: " + p.overall +
                          " | Age: " + p.age);
        }

        Debug.Log("\n-- A --");
        if (team.aRoster != null)
        {
            var sorted = team.aRoster
                .OrderByDescending(p => p.overall).ToList();
            foreach (Player p in sorted)
                Debug.Log(p.FullName().PadRight(22) +
                          " | " + p.position.PadRight(3) +
                          " | OVR: " + p.overall +
                          " | Age: " + p.age);
        }
    }

    // -------------------------------------------------------
    // PRINT DRAFT LEADERBOARD
    // Shows best available prospects
    // -------------------------------------------------------
    public void PrintDraftBoard(List<Player> draftClass,
                                 int topN = 30)
    {
        Debug.Log("\n========== " +
                  "DRAFT BOARD (Top " + topN + ") ==========");
        Debug.Log("RANK | NAME                 | POS | " +
                  "OVR | AGE | HAND");
        Debug.Log("---------------------------------------------");

        var top = draftClass
            .OrderByDescending(p => p.overall)
            .Take(topN)
            .ToList();

        for (int i = 0; i < top.Count; i++)
        {
            Player p = top[i];
            Debug.Log((i + 1).ToString().PadRight(5) +
                      p.FullName().PadRight(22) +
                      " | " + p.position.PadRight(3) +
                      " | " + p.overall.ToString().PadRight(3) +
                      " | " + p.age.ToString().PadRight(3) +
                      " | " + p.battingHand);
        }
    }
}
