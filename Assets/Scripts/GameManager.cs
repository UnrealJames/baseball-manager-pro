using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class ScheduledSeries
{
    public string homeTeam;
    public string awayTeam;
    public int    numGames;
    public int    gamesPlayed;
    public int    homeWins;
    public int    awayWins;
    public bool   isComplete;
    public string month;
    public int    dayStart;
}


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

    int currentSaveSlot = 0;

    public Dictionary<string, int> finalWins   =
        new Dictionary<string, int>();
    public Dictionary<string, int> finalLosses =
        new Dictionary<string, int>();

    Dictionary<string, int> teamRotationIndex =
        new Dictionary<string, int>();

    // Schedule tracking
    public int  totalGamesPlayed   = 0;
    public int  maxGamesPerSeason  = 162;
    public bool seasonComplete     = false;
    public bool postseasonStarted  = false;

    public string currentSeriesOpponent = "";
    public int    currentSeriesGame     = 0;
    public int    currentSeriesLength   = 0;
    public int    seriesHomeWins        = 0;
    public int    seriesAwayWins        = 0;

    public List<ScheduledSeries> schedule =
        new List<ScheduledSeries>();
    public int currentSeriesIndex = 0;


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

        gameObject.AddComponent<RosterBuilder_ALEast>();
        gameObject.AddComponent<RosterBuilder_ALCentral>();
        gameObject.AddComponent<RosterBuilder_ALWest>();
        gameObject.AddComponent<RosterBuilder_NLEast>();
        gameObject.AddComponent<RosterBuilder_NLCentral>();
        gameObject.AddComponent<RosterBuilder_NLWest>();

        Invoke("RunSeason", 0.1f);
    }

    void RunSeason()
    {
        BuildAllRosters();
        franchiseManager.StartNewFranchise("NYA", "James", 1);
        // Don't generate schedule here —
        // wait until player picks their team

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
        foreach (Team t in dataLoader.allTeams)
            t.roster = new List<Player>();

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

        MiLBGenerator milb =
            gameObject.GetComponent<MiLBGenerator>();
        if (milb != null)
            milb.GenerateAllMinorLeagues(dataLoader.allTeams);

        foreach (Team t in dataLoader.allTeams)
            Debug.Log(t.city + " " + t.nickname +
                      " roster: " + t.roster.Count + " players");

        Debug.Log("Rosters ready!");
    }

    // -------------------------------------------------------
    // START FRANCHISE
    // -------------------------------------------------------
    public void StartFranchise(string teamAbbr, string gmName)
    {
        // Clear old standings from previous save
        finalWins.Clear();
        finalLosses.Clear();

        // Reset all team records to 0-0
        if (dataLoader?.allTeams != null)
        {
            foreach (Team t in dataLoader.allTeams)
            {
                t.wins   = 0;
                t.losses = 0;
            }
        }

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

        GenerateSeasonSchedule();
    }

    // -------------------------------------------------------
    // SCHEDULE GENERATOR
    // -------------------------------------------------------
    public void GenerateSeasonSchedule()
    {
        schedule.Clear();
        currentSeriesIndex = 0;
        totalGamesPlayed   = 0;
        seasonComplete     = false;
        postseasonStarted  = false;

        if (dataLoader?.allTeams == null) return;

        List<Team> allTeams = dataLoader.allTeams;

        Team playerTeam = allTeams.Find(
            t => t.abbreviation ==
                 franchiseManager.franchise
                     .playerTeamAbbreviation);
        if (playerTeam == null) return;

        List<Team> divRivals = allTeams.FindAll(
            t => t.division == playerTeam.division &&
                 t.abbreviation !=
                     playerTeam.abbreviation);

        List<Team> leagueOpponents = allTeams.FindAll(
            t => t.league == playerTeam.league &&
                 t.division != playerTeam.division);

        List<Team> interleague = allTeams.FindAll(
            t => t.league != playerTeam.league);

        string[] months = {
            "MAR","APR","MAY","JUN",
            "JUL","AUG","SEP" };
        int monthIdx = 0;
        int day      = 25;

        void AddSeries(Team home, Team away, int games)
        {
            schedule.Add(new ScheduledSeries
            {
                homeTeam    = home.abbreviation,
                awayTeam    = away.abbreviation,
                numGames    = games,
                gamesPlayed = 0,
                homeWins    = 0,
                awayWins    = 0,
                isComplete  = false,
                month       = months[Mathf.Clamp(
                    monthIdx, 0, months.Length - 1)],
                dayStart    = day
            });

            day += games + 1;
            if (day > 28)
            {
                day = 1;
                monthIdx = Mathf.Min(
                    monthIdx + 1, months.Length - 1);
            }

            if (months[Mathf.Clamp(monthIdx, 0,
                months.Length - 1)] == "JUL" &&
                day >= 11 && day <= 16)
                day = 17;
        }

        // Division rivals — 13 games each (3+3+4+3)
        foreach (Team rival in divRivals)
        {
            AddSeries(playerTeam, rival,     3);
            AddSeries(rival,      playerTeam, 3);
            AddSeries(playerTeam, rival,     4);
            AddSeries(rival,      playerTeam, 3);
        }

        // League opponents — 6 or 7 games each
        int lgIdx = 0;
        foreach (Team opp in leagueOpponents)
        {
            bool isSeven = lgIdx % 5 == 0;
            if (isSeven)
            {
                AddSeries(playerTeam, opp, 3);
                AddSeries(opp, playerTeam, 4);
            }
            else
            {
                AddSeries(playerTeam, opp, 3);
                AddSeries(opp, playerTeam, 3);
            }
            lgIdx++;
        }

        // Interleague — 3 games each
        foreach (Team opp in interleague)
        {
            if (interleague.IndexOf(opp) % 2 == 0)
                AddSeries(playerTeam, opp, 3);
            else
                AddSeries(opp, playerTeam, 3);
        }

        InterleaveSeries();

        Debug.Log("Schedule generated: " +
            schedule.Count + " series for " +
            playerTeam.abbreviation);
    }

    void InterleaveSeries()
    {
        if (schedule.Count == 0) return;

        List<ScheduledSeries> divSeries =
            new List<ScheduledSeries>();
        List<ScheduledSeries> lgSeries  =
            new List<ScheduledSeries>();
        List<ScheduledSeries> ilSeries  =
            new List<ScheduledSeries>();

        Team playerTeam = dataLoader.allTeams.Find(
            t => t.abbreviation ==
                 franchiseManager.franchise
                     .playerTeamAbbreviation);

        if (playerTeam == null) return;

        foreach (ScheduledSeries s in schedule)
        {
            string oppAbbr =
                s.homeTeam == playerTeam.abbreviation
                    ? s.awayTeam : s.homeTeam;

            Team opp = dataLoader.allTeams.Find(
                t => t.abbreviation == oppAbbr);

            if (opp == null) continue;

            if (opp.division == playerTeam.division)
                divSeries.Add(s);
            else if (opp.league == playerTeam.league)
                lgSeries.Add(s);
            else
                ilSeries.Add(s);
        }

        schedule.Clear();

        int d = 0, l = 0, il = 0;

        while (d < divSeries.Count ||
               l < lgSeries.Count  ||
               il < ilSeries.Count)
        {
            for (int i = 0; i < 2 && d < divSeries.Count; i++)
                schedule.Add(divSeries[d++]);

            if (l < lgSeries.Count)
                schedule.Add(lgSeries[l++]);

            if (il < ilSeries.Count)
                schedule.Add(ilSeries[il++]);

            if (l < lgSeries.Count)
                schedule.Add(lgSeries[l++]);
        }

        // Re-assign dates
        string[] months = {
            "MAR","APR","MAY","JUN",
            "JUL","AUG","SEP" };
        int monthIdx = 0;
        int day      = 25;

        for (int i = 0; i < schedule.Count; i++)
        {
            schedule[i].month =
                months[Mathf.Clamp(
                    monthIdx, 0, months.Length - 1)];
            schedule[i].dayStart = day;

            day += schedule[i].numGames + 1;
            if (day > 28)
            {
                day = 1;
                monthIdx = Mathf.Min(
                    monthIdx + 1, months.Length - 1);
            }

            if (monthIdx < months.Length &&
                months[monthIdx] == "JUL" &&
                day >= 11 && day <= 16)
                day = 17;
        }

        Debug.Log("Schedule interleaved: " +
            divSeries.Count + " div, " +
            lgSeries.Count  + " league, " +
            ilSeries.Count  + " interleague series");
    }

    // -------------------------------------------------------
    // POSTSEASON
    // -------------------------------------------------------
    public void TriggerPostseason()
    {
        if (postseasonStarted) return;
        postseasonStarted = true;

        Debug.Log("SEASON COMPLETE — " +
            totalGamesPlayed + " games played.");
        Debug.Log("POSTSEASON STARTING!");

        SaveFinalStandings();

        List<Team> playoffTeams = GetPlayoffTeams();
        string msg = "PLAYOFF TEAMS: ";
        foreach (Team t in playoffTeams)
            msg += t.abbreviation + " ";
        Debug.Log(msg);
    }

    public List<Team> GetPlayoffTeams()
    {
        List<Team> playoffs = new List<Team>();

        string[] leagues = { "AL", "NL" };
        string[] divs    = { "East", "Central", "West" };

        foreach (string league in leagues)
        {
            List<Team> wildcards = new List<Team>();

            foreach (string div in divs)
            {
                string fullDiv = league + " " + div;
                List<Team> divTeams =
                    dataLoader.allTeams.FindAll(
                        t => t.division == fullDiv);

                divTeams = divTeams
                    .OrderByDescending(t => t.wins)
                    .ThenBy(t => t.losses)
                    .ToList();

                if (divTeams.Count > 0)
                {
                    playoffs.Add(divTeams[0]);
                    for (int i = 1; i < divTeams.Count; i++)
                        wildcards.Add(divTeams[i]);
                }
            }

            wildcards = wildcards
                .OrderByDescending(t => t.wins)
                .ThenBy(t => t.losses)
                .ToList();

            for (int i = 0; i < 3 && i < wildcards.Count; i++)
                playoffs.Add(wildcards[i]);
        }

        return playoffs;
    }

    public bool PlayerMadePlayoffs()
    {
        List<Team> teams = GetPlayoffTeams();
        return teams.Exists(
            t => t.abbreviation ==
                 franchiseManager.franchise
                     .playerTeamAbbreviation);
    }

    public int GamesRemaining()
    {
        return Mathf.Max(0,
            maxGamesPerSeason - totalGamesPlayed);
    }

    public ScheduledSeries GetCurrentSeries()
    {
        if (schedule == null ||
            currentSeriesIndex >= schedule.Count)
            return null;
        return schedule[currentSeriesIndex];
    }

    public List<ScheduledSeries> GetUpcomingSeries(int n)
    {
        List<ScheduledSeries> upcoming =
            new List<ScheduledSeries>();
        for (int i = currentSeriesIndex;
             i < schedule.Count &&
             upcoming.Count < n; i++)
        {
            // Include current AND future series
            upcoming.Add(schedule[i]);
        }
        return upcoming;
    }

    public List<ScheduledSeries> GetRecentResults(int n)
    {
        List<ScheduledSeries> results =
            new List<ScheduledSeries>();
        for (int i = currentSeriesIndex - 1;
             i >= 0 && results.Count < n; i--)
        {
            if (schedule[i].isComplete)
                results.Add(schedule[i]);
        }
        return results;
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

        LineupEditor lineupEditor =
            gameObject.GetComponent<LineupEditor>();
        if (lineupEditor != null)
        {
            playerTeam.lineup   =
                lineupEditor.BuildOptimalLineup(playerTeam);
            playerTeam.rotation =
                lineupEditor.BuildOptimalRotation(playerTeam);
        }

        // Reset season tracking
        totalGamesPlayed  = 0;
        seasonComplete    = false;
        postseasonStarted = false;
        GenerateSeasonSchedule();

        SeasonSchedule schedule =
            seasonScheduler.GenerateSchedule(
                dataLoader.allTeams);

        seasonSimulator.SimulateSeason(
            schedule, dataLoader.allTeams);

        SaveFinalStandings();

        string wsWinner     = GetWorldSeriesWinner();
        string playerFinish = GetPlayerTeamFinish(
            dataLoader.allTeams,
            franchiseManager.franchise
            .playerTeamAbbreviation);

        Debug.Log("Your finish: "  + playerFinish);
        Debug.Log("World Series: " + wsWinner);

        OffseasonManager offseason =
            gameObject.GetComponent<OffseasonManager>();
        if (offseason != null)
            offseason.RunOffseason(
                dataLoader.allTeams,
                playerTeam,
                wsWinner,
                playerFinish);

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
        {
            Debug.Log("Loaded slot " + slot);

            // Regenerate schedule after load
            // so SCHEDULE screen has data
            GenerateSeasonSchedule();

            Debug.Log("Schedule regenerated after load: " +
                schedule.Count + " series");
        }
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

    public int GetCurrentSaveSlot() { return currentSaveSlot; }

    public void SetSaveSlot(int slot) { currentSaveSlot = slot; }

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
        bool homeWon = homeScore > awayScore;
        string playerAbbr =
            franchiseManager.franchise
                .playerTeamAbbreviation;

        Team homeTeam = dataLoader.allTeams.Find(
            t => t.abbreviation == homeAbbr);
        Team awayTeam = dataLoader.allTeams.Find(
            t => t.abbreviation == awayAbbr);

        if (homeTeam != null && awayTeam != null)
        {
            if (homeWon)
            {
                homeTeam.wins++;
                awayTeam.losses++;
            }
            else
            {
                awayTeam.wins++;
                homeTeam.losses++;
            }
        }

        bool playerIsHome = homeAbbr == playerAbbr;
        bool playerWon    =
            (playerIsHome && homeWon) ||
            (!playerIsHome && !homeWon);

        RecordSeriesGame(playerWon, playerIsHome);

        // *** FIXED: only called ONCE ***
        SimulateCPUGamesForDay();

        Debug.Log("WIN: " + homeAbbr +
                  " " + homeScore + " — " +
                  awayAbbr + " " + awayScore);
    }

    public void RecordSeriesGame(
        bool playerWon, bool playerIsHome)
    {
        if (schedule == null ||
            currentSeriesIndex >= schedule.Count)
        {
            totalGamesPlayed++;
            if (totalGamesPlayed >= maxGamesPerSeason)
            {
                seasonComplete = true;
                TriggerPostseason();
            }
            return;
        }

        ScheduledSeries series =
            schedule[currentSeriesIndex];
        series.gamesPlayed++;
        totalGamesPlayed++;

        if (playerWon)
        {
            if (playerIsHome) series.homeWins++;
            else              series.awayWins++;
        }
        else
        {
            if (playerIsHome) series.awayWins++;
            else              series.homeWins++;
        }

        if (series.gamesPlayed >= series.numGames)
        {
            series.isComplete   = true;
            currentSeriesIndex++;
            Debug.Log("Series complete. Next: " +
                currentSeriesIndex + " / " +
                schedule.Count);
        }

        if (totalGamesPlayed >= maxGamesPerSeason ||
            currentSeriesIndex >= schedule.Count)
        {
            seasonComplete = true;
            TriggerPostseason();
        }

        Debug.Log("Games played: " +
            totalGamesPlayed + " / " +
            maxGamesPerSeason);
    }

    // -------------------------------------------------------
    // CPU GAME SIMULATION — called ONCE per player game
    // -------------------------------------------------------
    public void SimulateCPUGamesForDay()
    {
        if (dataLoader?.allTeams == null) return;

        string playerAbbr =
            franchiseManager.franchise
                .playerTeamAbbreviation;

        // All non-player teams sorted by fewest games played
        List<Team> cpuTeams = dataLoader.allTeams
            .FindAll(t => t.abbreviation != playerAbbr);

        cpuTeams.Sort((a, b) =>
            (a.wins + a.losses).CompareTo(b.wins + b.losses));

        HashSet<string> playedToday =
            new HashSet<string>();

        int gamesSimmed = 0;

        for (int i = 0; i < cpuTeams.Count; i++)
        {
            Team home = cpuTeams[i];
            if (playedToday.Contains(home.abbreviation))
                continue;

            // Find opponent with fewest games played
            Team away     = null;
            int  fewest   = int.MaxValue;

            for (int j = i + 1; j < cpuTeams.Count; j++)
            {
                Team candidate = cpuTeams[j];
                if (playedToday.Contains(
                    candidate.abbreviation)) continue;

                int gp = candidate.wins + candidate.losses;
                if (gp < fewest)
                {
                    fewest = gp;
                    away   = candidate;
                }
            }

            if (away == null) continue;

            playedToday.Add(home.abbreviation);
            playedToday.Add(away.abbreviation);

            float homeStr =
                home.wins + home.losses > 0
                    ? (float)home.wins /
                      (home.wins + home.losses)
                    : 0.5f;
            float awayStr =
                away.wins + away.losses > 0
                    ? (float)away.wins /
                      (away.wins + away.losses)
                    : 0.5f;

            float winPct =
                (homeStr + 0.04f) /
                (homeStr + awayStr + 0.04f);

            if (Random.value < winPct)
            {
                home.wins++;
                away.losses++;
                CreditCPUPitcherStats(home, away, true);
            }
            else
            {
                away.wins++;
                home.losses++;
                CreditCPUPitcherStats(away, home, false);
            }

            gamesSimmed++;
        }

        Debug.Log("CPU games simulated: " +
            gamesSimmed + " (" +
            playedToday.Count + " teams played)");
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

    void CreditCPUPitcherStats(
        Team winTeam, Team loseTeam, bool homeWon)
    {
        Player winSP  = null;
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

        if (winSP != null)
        {
            int ip = Random.Range(5, 8);
            int er = Random.Range(0, 3);
            winSP.seasonInningsPitched   += ip;
            winSP.seasonEarnedRuns       += er;
            winSP.seasonWins++;
            winSP.wins++;

            int k = Mathf.RoundToInt(
                ip * (winSP.pitching / 99f) * 1.5f) +
                Random.Range(0, 3);
            winSP.seasonStrikeoutsThrown += k;

            int h = Random.Range(3, ip + 2);
            winSP.seasonHitsAllowed += h;
        }

        if (loseSP != null)
        {
            int ip = Random.Range(4, 7);
            int er = Random.Range(2, 6);
            loseSP.seasonInningsPitched   += ip;
            loseSP.seasonEarnedRuns       += er;
            loseSP.seasonLosses++;
            loseSP.losses++;

            int k = Mathf.RoundToInt(
                ip * (loseSP.pitching / 99f) * 1.2f) +
                Random.Range(0, 2);
            loseSP.seasonStrikeoutsThrown += k;

            int h = Random.Range(ip, ip + 4);
            loseSP.seasonHitsAllowed += h;
        }

        SimulateCPUBatterStats(winTeam,  true);
        SimulateCPUBatterStats(loseTeam, false);
    }

    void SimulateCPUBatterStats(Team team, bool won)
    {
        if (team?.roster == null) return;

        List<Player> batters = team.roster.FindAll(
            p => p.position != "SP" &&
                 p.position != "RP");

        foreach (Player b in batters)
        {
            b.seasonAtBats += Random.Range(3, 5);

            float hitChance =
                (b.contact / 99f) * 0.35f + 0.15f;
            if (Random.value < hitChance)
            {
                b.seasonHits++;
                b.seasonSingles++;

                if (Random.value <
                    b.power / 99f * 0.08f)
                {
                    b.seasonHomeRuns++;
                    b.seasonHits++;
                    b.seasonAtBats++;
                }
            }
        }

        int teamRuns = won
            ? Random.Range(2, 8)
            : Random.Range(0, 4);

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

        string[] positions = {
            "SP", "SP", "RP", "RP",
            "C", "1B", "2B", "3B", "SS",
            "LF", "CF", "RF", "DH" };

        string[] firstNames = {
            "Carlos", "Jose", "Miguel", "Alex", "Ryan",
            "Tyler", "Jake", "Kyle", "Chase", "Hunter",
            "Marcus", "Andre", "Kevin", "Derek", "Luis" };

        string[] lastNames = {
            "Garcias", "Martinezz", "Rodrigues", "Wilsons",
            "Andersons", "Thomass", "Harriss", "Martins",
            "Moores", "Taylors", "Lees", "Perezz", "Scotts",
            "Adamss", "Nelsons" };

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
