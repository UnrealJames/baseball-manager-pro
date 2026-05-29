using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    private DataLoader        dataLoader;
    private GameSimulator     gameSimulator;
    private SeasonScheduler   seasonScheduler;
    private SeasonSimulator   seasonSimulator;
    private FranchiseManager  franchiseManager;


    // Pitching rotation tracking
    Dictionary<string, int> teamRotationIndex =
        new Dictionary<string, int>();

    public void AdvanceRotation(string teamAbbr)
    {
        if (!teamRotationIndex.ContainsKey(teamAbbr))
            teamRotationIndex[teamAbbr] = 0;
        else
            teamRotationIndex[teamAbbr]++;
    }

    public int GetStartingPitcherIndex(string teamAbbr)
    {
        if (!teamRotationIndex.ContainsKey(teamAbbr))
            teamRotationIndex[teamAbbr] = 0;
        return teamRotationIndex[teamAbbr];
    }


    public List<Player> GenerateDraftClass()
    {
        DraftSystem draft =
            gameObject.GetComponent<DraftSystem>();
        if (draft == null) return new List<Player>();

        // Generate 30 prospects (round 1 picks)
        List<Player> draftClass =
            draft.GenerateDraftClass(
                franchiseManager.franchise.currentSeason, 1);

        // Sort by overall
        draftClass.Sort((a, b) =>
            b.overall.CompareTo(a.overall));

        Debug.Log("Draft class generated: " +
                  draftClass.Count + " prospects");
        return draftClass;
    }


    public float GetTradeValue(Player p)
    {
        TradeSystem ts = gameObject.GetComponent<TradeSystem>();
        return ts != null ? ts.GetTradeValue(p) : 0f;
    }

    public bool ProposeTrade(Team myTeam, Player myPlayer,
                              Team theirTeam, Player theirPlayer)
    {
        TradeSystem ts = gameObject.GetComponent<TradeSystem>();
        if (ts == null) return false;

        return ts.ProposeTrade(
            myTeam,
            new List<Player> { myPlayer },
            theirTeam,
            new List<Player> { theirPlayer });
    }


    public string GetSavedTeamAbbr()
    {
        return franchiseManager.franchise
               .playerTeamAbbreviation;
    }


    public void SaveGame()
    {
        SaveSystem saveSystem =
            gameObject.GetComponent<SaveSystem>();
        if (saveSystem != null)
            saveSystem.SaveGame(
                franchiseManager, dataLoader.allTeams);
    }

        public void RecordLiveGameResult(
        string homeAbbr, int homeScore,
        string awayAbbr, int awayScore)
    {
        Team homeTeam = dataLoader.allTeams.Find(
            t => t.abbreviation == homeAbbr);
        Team awayTeam = dataLoader.allTeams.Find(
            t => t.abbreviation == awayAbbr);

        if (homeTeam == null || awayTeam == null) return;

        // Record W/L
        if (homeScore > awayScore)
        {
            homeTeam.wins++;
            awayTeam.losses++;
            Debug.Log("WIN: " + homeAbbr + " " +
                      homeScore + " — " + awayAbbr +
                      " " + awayScore);
        }
        else
        {
            awayTeam.wins++;
            homeTeam.losses++;
            Debug.Log("WIN: " + awayAbbr + " " +
                      awayScore + " — " + homeAbbr +
                      " " + homeScore);
        }

        // Simulate one day of CPU games
        SimulateCPUGamesForDay();
    }

    void SimulateCPUGamesForDay()
    {
        // Get all teams except player's team
        List<Team> cpuTeams = dataLoader.allTeams.FindAll(
            t => t.abbreviation !=
                 franchiseManager.franchise
                 .playerTeamAbbreviation);

        // Shuffle for random matchups
        for (int i = cpuTeams.Count - 1; i > 0; i--)
        {
            int j       = Random.Range(0, i + 1);
            Team tmp    = cpuTeams[i];
            cpuTeams[i] = cpuTeams[j];
            cpuTeams[j] = tmp;
        }

        // Simulate games in pairs using quick math
        int gamesPlayed = 0;
        for (int i = 0; i + 1 < cpuTeams.Count; i += 2)
        {
            Team home = cpuTeams[i];
            Team away = cpuTeams[i + 1];

            // Quick sim based on team overall rating
            float homeStr = GetTeamStrength(home);
            float awayStr = GetTeamStrength(away);
            float total   = homeStr + awayStr;
            float roll    = Random.Range(0f, total);

            // Home team advantage
            float homeAdv = total * 0.54f;

            // Advance rotation for both teams
            AdvanceRotation(home.abbreviation);
            AdvanceRotation(away.abbreviation);

            if (roll < homeAdv)
            {
                home.wins++;
                away.losses++;
            }
            else
            {
                away.wins++;
                home.losses++;
            }

            gamesPlayed++;
        }

        Debug.Log("CPU games simulated: " + gamesPlayed);
    }

    float GetTeamStrength(Team t)
    {
        if (t.roster == null || t.roster.Count == 0)
            return 50f;

        float total = 0f;
        foreach (Player p in t.roster)
            total += p.overall;
        return total / t.roster.Count;
    }


    public bool LoadGame()
    {
        SaveSystem saveSystem =
            gameObject.GetComponent<SaveSystem>();
        if (saveSystem == null) return false;

        bool loaded = saveSystem.LoadGame(
            franchiseManager, dataLoader.allTeams);

        if (loaded)
            Debug.Log("Franchise loaded successfully!");

        return loaded;
    }

    public bool HasSaveFile()
    {
        SaveSystem saveSystem =
            gameObject.GetComponent<SaveSystem>();
        return saveSystem != null &&
               saveSystem.HasSaveFile();
    }

    public string GetSaveInfo()
    {
        SaveSystem saveSystem =
            gameObject.GetComponent<SaveSystem>();
        return saveSystem != null ?
               saveSystem.GetSaveInfo() : "";
    }

       public void SimulateOneSeason()
    {
        Team playerTeam = dataLoader.allTeams.Find(
            t => t.abbreviation ==
                 franchiseManager.franchise.playerTeamAbbreviation);

        if (playerTeam == null)
        {
            Debug.LogError("Player team not found!");
            return;
        }

        int year = franchiseManager.franchise.currentSeason;
        Debug.Log("\n==========================================");
        Debug.Log("     SIMULATING " + year + " SEASON");
        Debug.Log("==========================================");

        // Set lineup and rotation
        LineupEditor lineupEditor =
            gameObject.GetComponent<LineupEditor>();
        if (lineupEditor != null)
        {
            playerTeam.lineup   = lineupEditor
                .BuildOptimalLineup(playerTeam);
            playerTeam.rotation = lineupEditor
                .BuildOptimalRotation(playerTeam);
        }

        // Generate schedule
        SeasonSchedule schedule = seasonScheduler
            .GenerateSchedule(dataLoader.allTeams);

        // Simulate season
        seasonSimulator.SimulateSeason(
            schedule, dataLoader.allTeams);

        // Get results
        string wsWinner     = GetWorldSeriesWinner();
        string playerFinish = GetPlayerTeamFinish(
            dataLoader.allTeams,
            franchiseManager.franchise.playerTeamAbbreviation);

        Debug.Log("Your finish: " + playerFinish);
        Debug.Log("World Series: " + wsWinner);

                // Save final standings before offseason resets them
        SaveFinalStandings();

        // Run offseason
        OffseasonManager offseason =
            gameObject.GetComponent<OffseasonManager>();
        if (offseason != null)
            offseason.RunOffseason(
                dataLoader.allTeams,
                playerTeam,
                wsWinner,
                playerFinish);


        Debug.Log("Season " + year + " complete!");
        Debug.Log("Now in " +
                  franchiseManager.franchise.currentSeason);
    
            // Auto save
        SaveGame();
        Debug.Log("Auto-saved!");

    
    }

    // Store final standings for UI display
    public Dictionary<string, int> finalWins   =
        new Dictionary<string, int>();
    public Dictionary<string, int> finalLosses =
        new Dictionary<string, int>();

    public void SaveFinalStandings()
    {
        finalWins.Clear();
        finalLosses.Clear();

        foreach (Team t in dataLoader.allTeams)
        {
            finalWins[t.abbreviation]   = t.wins;
            finalLosses[t.abbreviation] = t.losses;
        }

        Debug.Log("Final standings saved!");
    }


    public int GetCurrentSeason()
    {
        return franchiseManager.franchise.currentSeason;
    }


    public void StartFranchise(string teamAbbr, string gmName)
    {
        franchiseManager.StartNewFranchise(
            teamAbbr, gmName, 1);

        Team playerTeam = dataLoader.allTeams.Find(
            t => t.abbreviation == teamAbbr);

        if (playerTeam == null) return;

        Debug.Log("=== FRANCHISE STARTED ===");
        Debug.Log("GM: "    + gmName);
        Debug.Log("Team: "  + playerTeam.city +
                  " "       + playerTeam.nickname);
        Debug.Log("Budget: $" + playerTeam.budget + "M");
        Debug.Log("Roster: " + playerTeam.roster.Count +
                  " players");
    }

        void Start()
    {
        dataLoader      = FindFirstObjectByType<DataLoader>();
        gameSimulator   = gameObject.AddComponent<GameSimulator>();
        seasonScheduler = gameObject.AddComponent<SeasonScheduler>();
        seasonSimulator = gameObject.AddComponent<SeasonSimulator>();
        franchiseManager = gameObject.AddComponent<FranchiseManager>();
        gameObject.AddComponent<LineupEditor>();
        gameObject.AddComponent<ContractSystem>();
        gameObject.AddComponent<FreeAgencySystem>();
        gameObject.AddComponent<TradeSystem>();
        gameObject.AddComponent<DraftSystem>();
        gameObject.AddComponent<OffseasonManager>();
        gameObject.AddComponent<AwardsSystem>();
        gameObject.AddComponent<PostseasonSimulator>();
        gameObject.AddComponent<MiLBGenerator>();
        gameObject.AddComponent<InjurySystem>();

        // Add roster builders upfront
        gameObject.AddComponent<RosterBuilder_ALEast>();
        gameObject.AddComponent<RosterBuilder_ALCentral>();
        gameObject.AddComponent<RosterBuilder_ALWest>();
        gameObject.AddComponent<RosterBuilder_NLEast>();
        gameObject.AddComponent<RosterBuilder_NLCentral>();
        gameObject.AddComponent<RosterBuilder_NLWest>();


        gameObject.AddComponent<SaveSystem>();


        Invoke("RunSeason", 0.1f);
    }

        // Run Season Method
                void RunSeason()
    {
        // Build rosters FIRST
        BuildAllRosters();

        // Generate minor leagues
        MiLBGenerator milb = gameObject.GetComponent<MiLBGenerator>();
        if (milb != null)
            milb.GenerateAllMinorLeagues(dataLoader.allTeams);

        // Start franchise
        franchiseManager.StartNewFranchise("NYA", "James", 1);

        Debug.Log("Rosters ready! Total teams: " +
                  dataLoader.allTeams.Count);

        // Log all roster counts
        foreach (Team t in dataLoader.allTeams)
            Debug.Log(t.abbreviation + " roster: " +
                      t.roster.Count);
    }


    string GetWorldSeriesWinner()
    {
        return "TBD";
    }

    string GetPlayerTeamFinish(List<Team> allTeams, string abbr)
    {
        Team t = allTeams.FirstOrDefault(
            tm => tm.abbreviation == abbr);
        if (t == null) return "Unknown";

        List<Team> divTeams = allTeams
            .Where(tm => tm.division == t.division)
            .OrderByDescending(tm => tm.wins)
            .ToList();

        if (divTeams[0].abbreviation == abbr)
            return t.division + " Champions";

        var divWinners = allTeams
            .Where(tm => tm.league == t.league)
            .GroupBy(tm => tm.division)
            .Select(g => g.OrderByDescending(
                tm => tm.wins).First().abbreviation)
            .ToList();

        List<Team> leagueTeams = allTeams
            .Where(tm => tm.league == t.league)
            .OrderByDescending(tm => tm.wins)
            .ToList();

        var wcTeams = leagueTeams
            .Where(tm => !divWinners.Contains(tm.abbreviation))
            .Take(3)
            .Select(tm => tm.abbreviation)
            .ToList();

        if (wcTeams.Contains(abbr))
            return "Wild Card";

        int rank = leagueTeams
            .FindIndex(tm => tm.abbreviation == abbr) + 1;
        return "Finished " + rank + " in " + t.league;
    }




    // -------------------------------------------------------
    // BUILD ALL ROSTERS
    // -------------------------------------------------------
               void BuildAllRosters()
    {
        // Step 1 — Empty rosters
        foreach (Team t in dataLoader.allTeams)
            t.roster = new List<Player>();

        // Step 2 — Build real rosters using pre-loaded builders
        gameObject.GetComponent<RosterBuilder_ALEast>()
            .BuildAllRosters(dataLoader.allTeams);

        gameObject.GetComponent<RosterBuilder_ALCentral>()
            .BuildAllRosters(dataLoader.allTeams);

        gameObject.GetComponent<RosterBuilder_ALWest>()
            .BuildAllRosters(dataLoader.allTeams);

        gameObject.GetComponent<RosterBuilder_NLEast>()
            .BuildAllRosters(dataLoader.allTeams);

        gameObject.GetComponent<RosterBuilder_NLCentral>()
            .BuildAllRosters(dataLoader.allTeams);

        gameObject.GetComponent<RosterBuilder_NLWest>()
            .BuildAllRosters(dataLoader.allTeams);

        // Step 3 — Generate minor leagues
        MiLBGenerator milb =
            gameObject.GetComponent<MiLBGenerator>();
        if (milb != null)
            milb.GenerateAllMinorLeagues(dataLoader.allTeams);

        // Step 4 — Log counts
        foreach (Team t in dataLoader.allTeams)
            Debug.Log(t.city + " " + t.nickname +
                      " roster: " + t.roster.Count + " players");

        Debug.Log("Rosters ready!");
    }





    void GeneratePlaceholderRoster(Team t)
    {
        // Starting pitcher
        Player sp = GeneratePitcher(
            t.abbreviation + "_SP", "SP", t.abbreviation, 70, 85);
        t.roster.Add(sp);

        // Bullpen
        Player su = GeneratePitcher(
            t.abbreviation + "_SU", "RP", t.abbreviation, 65, 80);
        su.bullpenRole = "SU"; t.roster.Add(su);

        Player cl = GeneratePitcher(
            t.abbreviation + "_CL", "RP", t.abbreviation, 70, 85);
        cl.bullpenRole = "CL"; t.roster.Add(cl);

        Player mr1 = GeneratePitcher(
            t.abbreviation + "_MR1", "RP", t.abbreviation, 60, 75);
        mr1.bullpenRole = "MR"; t.roster.Add(mr1);

        Player mr2 = GeneratePitcher(
            t.abbreviation + "_MR2", "RP", t.abbreviation, 60, 75);
        mr2.bullpenRole = "MR"; t.roster.Add(mr2);

        // 9 position players
        string[] positions = new string[]
        { "C", "1B", "2B", "3B", "SS",
          "LF", "CF", "RF", "DH" };

        string[] hands = new string[]
        { "R", "R", "R", "L", "R",
          "L", "R", "R", "L" };

        for (int i = 0; i < positions.Length; i++)
        {
            Player p = GenerateBatter(
                t.abbreviation + "_" + positions[i],
                positions[i], t.abbreviation,
                60, 85, hands[i]);
            t.roster.Add(p);
        }
    }

    // -------------------------------------------------------
    // PLAYER GENERATORS
    // -------------------------------------------------------
    Player GeneratePitcher(string id, string pos, string team,
                            int minRating, int maxRating)
    {
        Player p      = new Player();
        p.firstName   = team;
        p.lastName    = pos;
        p.position    = pos;
        p.team        = team;
        p.throwingArm = Random.value > 0.3f ? "R" : "L";
        p.battingHand = "R";
        p.pitching    = Random.Range(minRating, maxRating);
        p.stamina     = Random.Range(minRating, maxRating);
        p.overall     = p.pitching;
        p.confidence  = 50f + (p.pitching - 50) * 0.5f;
        p.age         = Random.Range(24, 36);
        return p;
    }

    Player GenerateBatter(string id, string pos, string team,
                           int minRating, int maxRating, string hand)
    {
        Player p      = new Player();
        p.firstName   = team;
        p.lastName    = pos;
        p.position    = pos;
        p.team        = team;
        p.battingHand = hand;
        p.throwingArm = "R";
        p.contact     = Random.Range(minRating, maxRating);
        p.power       = Random.Range(minRating, maxRating);
        p.speed       = Random.Range(minRating, maxRating);
        p.arm         = Random.Range(minRating, maxRating);
        p.fielding    = Random.Range(minRating, maxRating);
        p.overall     = (p.contact + p.power + p.speed +
                         p.arm + p.fielding) / 5;
        p.age         = Random.Range(22, 38);
        return p;
    }

    // -------------------------------------------------------
    // CREATE PLAYER HELPER
    // -------------------------------------------------------
    public Player CreatePlayer(int id, string first, string last,
                               string pos, int age, int overall,
                               int contact, int power, int speed,
                               int arm, int fielding, int pitching,
                               int stamina, float salary,
                               int contractYears, string team,
                               string throwingArm = "R",
                               string battingHand = "R")
    {
        Player p        = new Player();
        p.id            = id;
        p.firstName     = first;
        p.lastName      = last;
        p.position      = pos;
        p.age           = age;
        p.overall       = overall;
        p.contact       = contact;
        p.power         = power;
        p.speed         = speed;
        p.arm           = arm;
        p.fielding      = fielding;
        p.pitching      = pitching;
        p.stamina       = stamina;
        p.salary        = salary;
        p.contractYears = contractYears;
        p.team          = team;
        p.throwingArm   = throwingArm;
        p.battingHand   = battingHand;
        p.isInjured     = false;
        return p;
    }

        public List<Player> GetFreeAgents()
    {
        List<Player> fas = new List<Player>();

        foreach (Team t in dataLoader.allTeams)
        {
            if (t.roster == null) continue;
            foreach (Player p in t.roster)
                if (p.team == "FA") fas.Add(p);
        }

        if (fas.Count == 0)
        {
            Debug.Log("Generating FA pool...");
            fas = GenerateFAPool();
        }

        Debug.Log("FA pool: " + fas.Count + " players");
        return fas;
    }

    List<Player> GenerateFAPool()
    {
        List<Player> pool = new List<Player>();

        string[] positions = new string[]
        {
            "SP", "SP", "RP", "RP",
            "C", "1B", "2B", "3B", "SS",
            "LF", "CF", "RF", "DH"
        };

        string[] firstNames = new string[]
        {
            "Carlos", "Jose", "Miguel", "Alex", "Ryan",
            "Tyler", "Jake", "Kyle", "Chase", "Hunter",
            "Marcus", "Andre", "Kevin", "Derek", "Luis"
        };

        string[] lastNames = new string[]
        {
            "Garcias", "Martinezz", "Rodrigues", "Wilsons",
            "Andersons", "Thomass", "Harriss", "Martins",
            "Moores", "Taylors", "Lees", "Perezz", "Scotts",
            "Adamss", "Nelsons"
        };

        for (int i = 0; i < 20; i++)
        {
            Player p      = new Player();
            p.id          = Random.Range(80000, 89999);
            p.firstName   = firstNames[
                Random.Range(0, firstNames.Length)];
            p.lastName    = lastNames[
                Random.Range(0, lastNames.Length)];
            p.position    = positions[
                Random.Range(0, positions.Length)];
            p.age         = Random.Range(26, 38);
            p.team        = "FA";
            p.contractYears = 0;
            p.salary      = 0f;
            p.isInjured   = false;
            p.bullpenRole = "";
            p.confidence  = 50f;
            p.throwingArm = Random.value > 0.3f ? "R" : "L";
            p.battingHand = Random.value > 0.5f ? "R" : "L";

            bool isPitcher = p.position == "SP" ||
                             p.position == "RP";
            if (isPitcher)
            {
                p.pitching = Random.Range(58, 82);
                p.stamina  = Random.Range(58, 82);
                p.overall  = p.pitching;
            }
            else
            {
                p.contact  = Random.Range(58, 82);
                p.power    = Random.Range(58, 82);
                p.speed    = Random.Range(55, 80);
                p.arm      = Random.Range(55, 80);
                p.fielding = Random.Range(55, 80);
                p.overall  = (p.contact + p.power +
                              p.speed + p.arm +
                              p.fielding) / 5;
            }

            pool.Add(p);
        }

        return pool;
    }

    public float GetMarketValue(Player p)
    {
        ContractSystem cs =
            gameObject.GetComponent<ContractSystem>();
        return cs != null ?
            cs.GetMarketValue(p) : p.salary;
    }

}
