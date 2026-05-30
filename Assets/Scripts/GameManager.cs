using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // -------------------------------------------------------
    // REFERENCES
    // -------------------------------------------------------
    DataLoader       dataLoader;
    GameSimulator    gameSimulator;
    SeasonScheduler  seasonScheduler;
    SeasonSimulator  seasonSimulator;
    FranchiseManager franchiseManager;

    // Current save slot
    int currentSaveSlot = 0;

    // Final standings saved before offseason resets
    public Dictionary<string, int> finalWins   =
        new Dictionary<string, int>();
    public Dictionary<string, int> finalLosses =
        new Dictionary<string, int>();

    // Pitching rotation tracking per team
    Dictionary<string, int> teamRotationIndex =
        new Dictionary<string, int>();

    // -------------------------------------------------------
    // START
    // -------------------------------------------------------
    void Start()
    {
        dataLoader       = FindFirstObjectByType<DataLoader>();
        gameSimulator    = gameObject.AddComponent<GameSimulator>();
        seasonScheduler  = gameObject.AddComponent<SeasonScheduler>();
        seasonSimulator  = gameObject.AddComponent<SeasonSimulator>();
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
        gameObject.AddComponent<SaveSystem>();

        // Add roster builders upfront
        gameObject.AddComponent<RosterBuilder_ALEast>();
        gameObject.AddComponent<RosterBuilder_ALCentral>();
        gameObject.AddComponent<RosterBuilder_ALWest>();
        gameObject.AddComponent<RosterBuilder_NLEast>();
        gameObject.AddComponent<RosterBuilder_NLCentral>();
        gameObject.AddComponent<RosterBuilder_NLWest>();

        Invoke("RunSeason", 0.1f);
    }

    // -------------------------------------------------------
    // RUN SEASON — called on start
    // -------------------------------------------------------
    void RunSeason()
    {
        // Build rosters first
        BuildAllRosters();

        // Start franchise
        franchiseManager.StartNewFranchise("NYA", "James", 1);

        Debug.Log("Rosters ready! Total teams: " +
                  dataLoader.allTeams.Count);

        foreach (Team t in dataLoader.allTeams)
            Debug.Log(t.abbreviation + " roster: " +
                      t.roster.Count);
    }

    // -------------------------------------------------------
    // BUILD ALL ROSTERS
    // -------------------------------------------------------
    void BuildAllRosters()
    {
        // Step 1 — Empty all rosters
        foreach (Team t in dataLoader.allTeams)
            t.roster = new List<Player>();

        // Step 2 — Build real rosters
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

    // -------------------------------------------------------
    // START FRANCHISE — called by UI when team is selected
    // -------------------------------------------------------
    public void StartFranchise(string teamAbbr, string gmName)
    {
        franchiseManager.StartNewFranchise(teamAbbr, gmName, 1);

        Team playerTeam = dataLoader.allTeams.Find(
            t => t.abbreviation == teamAbbr);

        if (playerTeam == null) return;

        Debug.Log("=== FRANCHISE STARTED ===");
        Debug.Log("GM: "     + gmName);
        Debug.Log("Team: "   + playerTeam.city +
                  " "        + playerTeam.nickname);
        Debug.Log("Budget: $" + playerTeam.budget + "M");
        Debug.Log("Roster: " + playerTeam.roster.Count +
                  " players");
    }

    // -------------------------------------------------------
    // SIMULATE ONE SEASON
    // -------------------------------------------------------
    public void SimulateOneSeason()
    {
        Team playerTeam = dataLoader.allTeams.Find(
            t => t.abbreviation ==
                 franchiseManager.franchise
                 .playerTeamAbbreviation);

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
            playerTeam.lineup   =
                lineupEditor.BuildOptimalLineup(playerTeam);
            playerTeam.rotation =
                lineupEditor.BuildOptimalRotation(playerTeam);
        }

        // Generate schedule
        SeasonSchedule schedule =
            seasonScheduler.GenerateSchedule(
                dataLoader.allTeams);

        // Simulate season
        seasonSimulator.SimulateSeason(
            schedule, dataLoader.allTeams);

        // Save standings before offseason resets
        SaveFinalStandings();

        // Get results
        string wsWinner     = GetWorldSeriesWinner();
        string playerFinish = GetPlayerTeamFinish(
            dataLoader.allTeams,
            franchiseManager.franchise
            .playerTeamAbbreviation);

        Debug.Log("Your finish: "   + playerFinish);
        Debug.Log("World Series: "  + wsWinner);

        // Run offseason
        OffseasonManager offseason =
            gameObject.GetComponent<OffseasonManager>();
        if (offseason != null)
            offseason.RunOffseason(
                dataLoader.allTeams,
                playerTeam,
                wsWinner,
                playerFinish);

        // Auto save to current slot
        SaveGame(currentSaveSlot);
        Debug.Log("Auto-saved to slot " + currentSaveSlot);

        Debug.Log("Season " + year + " complete!");
        Debug.Log("Now in " +
                  franchiseManager.franchise.currentSeason);
    }

    // -------------------------------------------------------
    // SAVE / LOAD
    // -------------------------------------------------------
    public void SaveGame(int slot = 0)
    {
        SaveSystem saveSystem =
            gameObject.GetComponent<SaveSystem>();
        if (saveSystem != null)
            saveSystem.SaveGame(
                franchiseManager,
                dataLoader.allTeams,
                slot);
    }

    public bool LoadGame(int slot = 0)
    {
        SaveSystem saveSystem =
            gameObject.GetComponent<SaveSystem>();
        if (saveSystem == null) return false;

        bool loaded = saveSystem.LoadGame(
            franchiseManager,
            dataLoader.allTeams,
            slot);

        if (loaded)
            Debug.Log("Loaded slot " + slot);
        return loaded;
    }

    public bool HasSaveFile(int slot = 0)
    {
        SaveSystem saveSystem =
            gameObject.GetComponent<SaveSystem>();
        return saveSystem != null &&
               saveSystem.HasSaveFile(slot);
    }

    public bool HasAnySave()
    {
        SaveSystem saveSystem =
            gameObject.GetComponent<SaveSystem>();
        return saveSystem != null &&
               saveSystem.HasAnySave();
    }

    public string GetSaveInfo(int slot = 0)
    {
        SaveSystem saveSystem =
            gameObject.GetComponent<SaveSystem>();
        return saveSystem != null ?
               saveSystem.GetSaveInfo(slot) : "";
    }

    public List<string> GetAllSaveInfos()
    {
        SaveSystem saveSystem =
            gameObject.GetComponent<SaveSystem>();
        return saveSystem != null ?
               saveSystem.GetAllSaveInfos() :
               new List<string> { "", "", "" };
    }

    public int GetCurrentSaveSlot()
    {
        return currentSaveSlot;
    }

    public void SetSaveSlot(int slot)
    {
        currentSaveSlot = slot;
    }

    // -------------------------------------------------------
    // STANDINGS
    // -------------------------------------------------------
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

    // -------------------------------------------------------
    // LIVE GAME RESULTS
    // -------------------------------------------------------
    public void RecordLiveGameResult(
        string homeAbbr, int homeScore,
        string awayAbbr, int awayScore)
    {
        Team homeTeam = dataLoader.allTeams.Find(
            t => t.abbreviation == homeAbbr);
        Team awayTeam = dataLoader.allTeams.Find(
            t => t.abbreviation == awayAbbr);

        if (homeTeam == null || awayTeam == null) return;

        if (homeScore > awayScore)
        {
            homeTeam.wins++;
            awayTeam.losses++;
            Debug.Log("WIN: " + homeAbbr + " " +
                      homeScore + " — " +
                      awayAbbr + " " + awayScore);
        }
        else
        {
            awayTeam.wins++;
            homeTeam.losses++;
            Debug.Log("WIN: " + awayAbbr + " " +
                      awayScore + " — " +
                      homeAbbr + " " + homeScore);
        }

        // Simulate CPU games for the day
        SimulateCPUGamesForDay();
    }

    // Simulate one day of CPU games
    void SimulateCPUGamesForDay()
    {
        List<Team> cpuTeams = dataLoader.allTeams.FindAll(
            t => t.abbreviation !=
                 franchiseManager.franchise
                 .playerTeamAbbreviation);

        // Shuffle teams for random matchups
        for (int i = cpuTeams.Count - 1; i > 0; i--)
        {
            int  j      = Random.Range(0, i + 1);
            Team tmp    = cpuTeams[i];
            cpuTeams[i] = cpuTeams[j];
            cpuTeams[j] = tmp;
        }

        int gamesPlayed = 0;
        for (int i = 0; i + 1 < cpuTeams.Count; i += 2)
        {
            Team home = cpuTeams[i];
            Team away = cpuTeams[i + 1];

            float homeStr  = GetTeamStrength(home);
            float awayStr  = GetTeamStrength(away);
            float total    = homeStr + awayStr;
            float homeAdv  = total * 0.54f;
            float roll     = Random.Range(0f, total);

            // Advance rotation for both teams
            AdvanceRotation(home.abbreviation);
            AdvanceRotation(away.abbreviation);

            if (roll < homeAdv)
            {
                home.wins++;
                away.losses++;
                // Credit CPU pitcher stats
                CreditCPUPitcherStats(home, away, true);
            }
            else
            {
                away.wins++;
                home.losses++;
                // Credit CPU pitcher stats
                CreditCPUPitcherStats(away, home, false);
            }

            gamesPlayed++;
        }

        Debug.Log("CPU games simulated: " + gamesPlayed);
    }

    // Get average overall rating for a team
    float GetTeamStrength(Team t)
    {
        if (t.roster == null || t.roster.Count == 0)
            return 50f;

        float total = 0f;
        foreach (Player p in t.roster)
            total += p.overall;
        return total / t.roster.Count;
    }

    // -------------------------------------------------------
    // PITCHING ROTATION
    // -------------------------------------------------------
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

    // Simulate basic pitcher stats for CPU games
    void CreditCPUPitcherStats(
        Team winTeam, Team loseTeam, bool homeWon)
    {
        // Find starters
        Player winSP = null;
        Player loseSP = null;

        if (winTeam.roster != null)
        {
            List<Player> sps = winTeam.roster.FindAll(
                p => p.position == "SP");
            if (sps.Count > 0)
            {
                int idx = GetStartingPitcherIndex(
                    winTeam.abbreviation) % sps.Count;
                winSP = sps[idx];
            }
        }

        if (loseTeam.roster != null)
        {
            List<Player> sps = loseTeam.roster.FindAll(
                p => p.position == "SP");
            if (sps.Count > 0)
            {
                int idx = GetStartingPitcherIndex(
                    loseTeam.abbreviation) % sps.Count;
                loseSP = sps[idx];
            }
        }

        // Simulate realistic game stats
        if (winSP != null)
        {
            // Winner starter: 5-7 IP, 0-2 ER
            int ip = Random.Range(5, 8);
            int er = Random.Range(0, 3);
            winSP.seasonInningsPitched += ip;
            winSP.seasonEarnedRuns     += er;
            winSP.seasonWins++;
            winSP.wins++;

            // K's based on pitching rating
            int k = Mathf.RoundToInt(
                ip * (winSP.pitching / 99f) * 1.5f) +
                Random.Range(0, 3);
            winSP.seasonStrikeoutsThrown += k;

            // Hits allowed
            int h = Random.Range(3, ip + 2);
            winSP.seasonHitsAllowed += h;
        }

        if (loseSP != null)
        {
            // Loser starter: 4-6 IP, 2-5 ER
            int ip = Random.Range(4, 7);
            int er = Random.Range(2, 6);
            loseSP.seasonInningsPitched += ip;
            loseSP.seasonEarnedRuns     += er;
            loseSP.seasonLosses++;
            loseSP.losses++;

            int k = Mathf.RoundToInt(
                ip * (loseSP.pitching / 99f) * 1.2f) +
                Random.Range(0, 2);
            loseSP.seasonStrikeoutsThrown += k;

            int h = Random.Range(ip, ip + 4);
            loseSP.seasonHitsAllowed += h;
        }

        // Simulate batter stats too
        SimulateCPUBatterStats(winTeam,  true);
        SimulateCPUBatterStats(loseTeam, false);
    }

    // Simulate basic batter stats for CPU games
    void SimulateCPUBatterStats(Team team, bool won)
    {
        if (team?.roster == null) return;

        List<Player> batters = team.roster.FindAll(
            p => p.position != "SP" &&
                 p.position != "RP");

        int teamHits = won
            ? Random.Range(6, 13)
            : Random.Range(3, 9);

        int teamRuns = won
            ? Random.Range(2, 8)
            : Random.Range(0, 4);

        // Distribute hits across lineup
        foreach (Player b in batters)
        {
            b.seasonAtBats += Random.Range(3, 5);

            // More hits for better hitters
            float hitChance =
                (b.contact / 99f) * 0.35f + 0.15f;
            if (Random.value < hitChance)
            {
                b.seasonHits++;
                b.seasonSingles++;

                // HR chance
                if (Random.value <
                    b.power / 99f * 0.08f)
                {
                    b.seasonHomeRuns++;
                    b.seasonHits++;
                    b.seasonAtBats++;
                }
            }
        }

        // Distribute RBI to top hitters
        if (batters.Count > 0)
        {
            batters.Sort((a, b2) =>
                b2.power.CompareTo(a.power));
            int rbiLeft = teamRuns;
            for (int i = 0;
                 i < batters.Count && rbiLeft > 0; i++)
            {
                int rbi = Random.Range(0, 2);
                batters[i].seasonRbi += rbi;
                rbiLeft -= rbi;
            }
        }
    }



    // -------------------------------------------------------
    // TRADE SYSTEM
    // -------------------------------------------------------
    public float GetTradeValue(Player p)
    {
        TradeSystem ts =
            gameObject.GetComponent<TradeSystem>();
        return ts != null ? ts.GetTradeValue(p) : 0f;
    }

    public bool ProposeTrade(
        Team myTeam,    Player myPlayer,
        Team theirTeam, Player theirPlayer)
    {
        TradeSystem ts =
            gameObject.GetComponent<TradeSystem>();
        if (ts == null) return false;

        return ts.ProposeTrade(
            myTeam,
            new List<Player> { myPlayer },
            theirTeam,
            new List<Player> { theirPlayer });
    }

    // -------------------------------------------------------
    // FREE AGENCY
    // -------------------------------------------------------
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
            Player p        = new Player();
            p.id            = Random.Range(80000, 89999);
            p.firstName     = firstNames[
                Random.Range(0, firstNames.Length)];
            p.lastName      = lastNames[
                Random.Range(0, lastNames.Length)];
            p.position      = positions[
                Random.Range(0, positions.Length)];
            p.age           = Random.Range(26, 38);
            p.team          = "FA";
            p.contractYears = 0;
            p.salary        = 0f;
            p.isInjured     = false;
            p.bullpenRole   = "";
            p.confidence    = 50f;
            p.throwingArm   =
                Random.value > 0.3f ? "R" : "L";
            p.battingHand   =
                Random.value > 0.5f ? "R" : "L";

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
                               p.speed  + p.arm +
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
        return cs != null ? cs.GetMarketValue(p) : p.salary;
    }

    // -------------------------------------------------------
    // DRAFT
    // -------------------------------------------------------
    public List<Player> GenerateDraftClass()
    {
        DraftSystem draft =
            gameObject.GetComponent<DraftSystem>();
        if (draft == null) return new List<Player>();

        List<Player> draftClass =
            draft.GenerateDraftClass(
                franchiseManager.franchise.currentSeason, 1);

        draftClass.Sort((a, b) =>
            b.overall.CompareTo(a.overall));

        Debug.Log("Draft class: " + draftClass.Count +
                  " prospects");
        return draftClass;
    }

    // -------------------------------------------------------
    // HELPERS
    // -------------------------------------------------------
    public int GetCurrentSeason()
    {
        return franchiseManager.franchise.currentSeason;
    }

    public string GetSavedTeamAbbr()
    {
        return franchiseManager.franchise
               .playerTeamAbbreviation;
    }

    string GetWorldSeriesWinner()
    {
        if (dataLoader.allTeams == null ||
            dataLoader.allTeams.Count == 0)
            return "Unknown";

        Team best = dataLoader.allTeams[0];
        foreach (Team t in dataLoader.allTeams)
            if (t.wins > best.wins) best = t;

        return best.city + " " + best.nickname;
    }

    string GetPlayerTeamFinish(
        List<Team> allTeams, string playerAbbr)
    {
        Team playerTeam = allTeams.Find(
            t => t.abbreviation == playerAbbr);
        if (playerTeam == null) return "Unknown";

        List<Team> divTeams = allTeams.FindAll(
            t => t.division == playerTeam.division);
        divTeams.Sort((a, b) => b.wins.CompareTo(a.wins));

        int place = divTeams.FindIndex(
            t => t.abbreviation == playerAbbr) + 1;

        string suffix =
            place == 1 ? "1st" :
            place == 2 ? "2nd" :
            place == 3 ? "3rd" :
            place + "th";

        return suffix + " in " + playerTeam.division;
    }
}