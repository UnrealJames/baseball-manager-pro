using UnityEngine;
using System.Collections.Generic;

public class MiLBGenerator : MonoBehaviour
{
    // First names pool
    private string[] firstNames = new string[]
    {
        "Carlos", "Jose", "Miguel", "Juan", "Luis",
        "Alex", "Ryan", "Tyler", "Jake", "Kyle",
        "Dylan", "Zach", "Chase", "Hunter", "Blake",
        "Cody", "Austin", "Brandon", "Trevor", "Jordan",
        "Nathan", "Mason", "Logan", "Caleb", "Ethan",
        "Noah", "Liam", "Owen", "Cole", "Drew",
        "Derek", "Marcus", "Andre", "Kevin", "Jason",
        "Derek", "Mario", "Pedro", "Ramon", "Felix",
        "Eduardo", "Roberto", "Hector", "Victor", "Angel",
        "Diego", "Gabriel", "Emmanuel", "Isaiah", "Malik"
    };

    // Last names pool
    private string[] lastNames = new string[]
    {
        "Garcias", "Martinezz", "Rodrigues", "Lopezz", "Gonzalezz",
        "Wilsons", "Andersons", "Thomass", "Jacksons", "Whites",
        "Harriss", "Martins", "Thompsons", "Moores", "Taylors",
        "Lees", "Perezz", "Thomases", "Lewiss", "Robinsons",
        "Walkers", "Youngs", "Halls", "Allens", "Wrights",
        "Kingss", "Scotts", "Greens", "Bakers", "Adamss",
        "Nelsons", "Carters", "Mitchells", "Parkers", "Collinss",
        "Edwardss", "Stewarts", "Florress", "Morriss", "Murphys",
        "Cooks", "Rogerss", "Morgans", "Coopers", "Reeds",
        "Baileys", "Bells", "Gomezz", "Kellys", "Howards"
    };

    // Position pools
    private string[] pitcherPositions = new string[] { "SP", "RP" };
    private string[] batterPositions  = new string[]
    { "C", "1B", "2B", "3B", "SS", "LF", "CF", "RF", "DH" };

    private int playerIdCounter = 10000;

    public void GenerateAllMinorLeagues(List<Team> allTeams)
    {
        foreach (Team t in allTeams)
        {
            t.aaaRoster = GenerateRoster(t.abbreviation, "AAA", 10, 62, 75);
            t.aaRoster  = GenerateRoster(t.abbreviation, "AA",  10, 52, 65);
            t.aRoster   = GenerateRoster(t.abbreviation, "A",    8, 42, 58);
        }

        Debug.Log("Minor leagues generated for all 30 teams!");
        Debug.Log("Total minor leaguers: " + (allTeams.Count * 28));
    }

    List<Player> GenerateRoster(string team, string level,
                                 int count, int minRating, int maxRating)
    {
        List<Player> roster = new List<Player>();

        // Always generate 3 pitchers first
        for (int i = 0; i < 3; i++)
        {
            roster.Add(GeneratePitcher(team, level, minRating, maxRating));
        }

        // Fill rest with position players
        int battersNeeded = count - 3;
        for (int i = 0; i < battersNeeded; i++)
        {
            roster.Add(GenerateBatter(team, level, minRating,
                                      maxRating, i));
        }

        return roster;
    }

    Player GeneratePitcher(string team, string level,
                            int minRating, int maxRating)
    {
        Player p          = new Player();
        p.id              = playerIdCounter++;
        p.firstName       = firstNames[Random.Range(0, firstNames.Length)];
        p.lastName        = lastNames[Random.Range(0, lastNames.Length)];
        p.position        = Random.value > 0.6f ? "SP" : "RP";
        p.team            = team;
        p.minorLeagueLevel = level;
        p.age             = Random.Range(19, 27);
        p.pitching        = Random.Range(minRating, maxRating);
        p.stamina         = Random.Range(minRating, maxRating);
        p.overall         = p.pitching;
        p.throwingArm     = Random.value > 0.3f ? "R" : "L";
        p.battingHand     = "R";
        p.confidence      = 50f + (p.pitching - 50) * 0.5f;
        p.salary          = Random.Range(1, 5) * 0.1f;
        p.contractYears   = Random.Range(1, 4);
        p.isInjured       = false;

        if (p.position == "RP")
        {
            p.bullpenRole = Random.value > 0.5f ? "MR" : "SU";
        }

        return p;
    }

    Player GenerateBatter(string team, string level,
                           int minRating, int maxRating, int index)
    {
        // Assign positions evenly across the roster
        string[] positionOrder = new string[]
        { "C", "1B", "2B", "3B", "SS", "LF", "CF", "RF" };

        string pos = positionOrder[index % positionOrder.Length];

        // Batting hand distribution — roughly 70% R, 20% L, 10% S
        string hand;
        float roll = Random.value;
        if (roll < 0.7f)       hand = "R";
        else if (roll < 0.9f)  hand = "L";
        else                   hand = "S";

        Player p          = new Player();
        p.id              = playerIdCounter++;
        p.firstName       = firstNames[Random.Range(0, firstNames.Length)];
        p.lastName        = lastNames[Random.Range(0, lastNames.Length)];
        p.position        = pos;
        p.team            = team;
        p.minorLeagueLevel = level;
        p.age             = Random.Range(18, 26);
        p.contact         = Random.Range(minRating, maxRating);
        p.power           = Random.Range(minRating, maxRating);
        p.speed           = Random.Range(minRating, maxRating);
        p.arm             = Random.Range(minRating, maxRating);
        p.fielding        = Random.Range(minRating, maxRating);
        p.overall         = (p.contact + p.power + p.speed +
                             p.arm + p.fielding) / 5;
        p.battingHand     = hand;
        p.throwingArm     = "R";
        p.salary          = Random.Range(1, 3) * 0.1f;
        p.contractYears   = Random.Range(1, 5);
        p.isInjured       = false;

        return p;
    }

    // -------------------------------------------------------
    // HELPER — Get best AAA player at a position for call up
    // -------------------------------------------------------
    public Player GetCallUp(Team team, string position)
    {
        if (team.aaaRoster == null) return null;

        // Find best available player at position
        Player best     = null;
        int    bestRating = 0;

        foreach (Player p in team.aaaRoster)
        {
            if (p.position == position && !p.isInjured)
            {
                int rating = p.position == "SP" || p.position == "RP"
                    ? p.pitching : p.overall;

                if (rating > bestRating)
                {
                    bestRating = rating;
                    best       = p;
                }
            }
        }

        // If no exact position match try any pitcher or any batter
        if (best == null)
        {
            foreach (Player p in team.aaaRoster)
            {
                if (!p.isInjured)
                {
                    int rating = p.pitching > 0 ? p.pitching : p.overall;
                    if (rating > bestRating)
                    {
                        bestRating = rating;
                        best       = p;
                    }
                }
            }
        }

        return best;
    }

    // -------------------------------------------------------
    // CALL UP — Move player from AAA to MLB roster
    // -------------------------------------------------------
    public void CallUp(Team team, Player player)
    {
        if (team.aaaRoster == null) return;
        if (!team.aaaRoster.Contains(player)) return;

        // Remove from AAA
        team.aaaRoster.Remove(player);

        // Add to MLB roster
        player.minorLeagueLevel = "";
        team.roster.Add(player);

        Debug.Log("CALL UP: " + player.FullName() +
                  " called up to " + team.city + " " + team.nickname +
                  " from AAA!");
    }

    // -------------------------------------------------------
    // SEND DOWN — Move player from MLB to AAA
    // -------------------------------------------------------
    public void SendDown(Team team, Player player)
    {
        if (!team.roster.Contains(player)) return;

        // Remove from MLB
        team.roster.Remove(player);

        // Add to AAA
        if (team.aaaRoster == null)
            team.aaaRoster = new List<Player>();

        player.minorLeagueLevel = "AAA";
        team.aaaRoster.Add(player);

        Debug.Log("OPTION: " + player.FullName() +
                  " sent down to AAA by " + team.city +
                  " " + team.nickname + "!");
    }

    // -------------------------------------------------------
    // PRINT MINOR LEAGUE ROSTER
    // -------------------------------------------------------
    public void PrintMinorLeagues(Team team)
    {
        Debug.Log("\n=== " + team.city + " " + team.nickname +
                  " MINOR LEAGUES ===");

        Debug.Log("\n-- AAA --");
        if (team.aaaRoster != null)
            foreach (Player p in team.aaaRoster)
                Debug.Log(p.FullName().PadRight(20) +
                          " | " + p.position.PadRight(3) +
                          " | OVR: " + p.overall +
                          " | Age: " + p.age);

        Debug.Log("\n-- AA --");
        if (team.aaRoster != null)
            foreach (Player p in team.aaRoster)
                Debug.Log(p.FullName().PadRight(20) +
                          " | " + p.position.PadRight(3) +
                          " | OVR: " + p.overall +
                          " | Age: " + p.age);

        Debug.Log("\n-- A --");
        if (team.aRoster != null)
            foreach (Player p in team.aRoster)
                Debug.Log(p.FullName().PadRight(20) +
                          " | " + p.position.PadRight(3) +
                          " | OVR: " + p.overall +
                          " | Age: " + p.age);
    }
}
