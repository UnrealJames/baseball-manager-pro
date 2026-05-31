using UnityEngine;
using System.Collections.Generic;

public class SaveSystem : MonoBehaviour
{
    // -------------------------------------------------------
    // SAVE KEYS — 3 slots for multiple franchises
    // -------------------------------------------------------
    string[] saveKeys = new string[]
    {
        "BMP_Save_1",
        "BMP_Save_2",
        "BMP_Save_3"
    };

    // -------------------------------------------------------
    // SAVE DATA STRUCTURES
    // -------------------------------------------------------
    [System.Serializable]
    public class SaveData
    {
        public string gmName                 = "";
        public string playerTeamAbbreviation = "";
        public string difficulty             = "NORMAL";
        public int    currentSeason          = 1;
        public int    totalSeasons           = 0;

        // Schedule progress
        public int  totalGamesPlayed   = 0;
        public int  currentSeriesIndex = 0;
        public bool seasonComplete     = false;

        // Current series mid-progress
        public int currentSeriesGamesPlayed = 0;
        public int currentSeriesHomeWins    = 0;
        public int currentSeriesAwayWins    = 0;

        // Recent series results (persists across save/load)
        public List<SeriesResultData> recentResults =
            new List<SeriesResultData>();

        public List<TeamSaveData>   teams   =
            new List<TeamSaveData>();
        public List<PlayerSaveData> players =
            new List<PlayerSaveData>();
    }

    [System.Serializable]
    public class SeriesResultData
    {
        public string homeTeam = "";
        public string awayTeam = "";
        public int    homeWins = 0;
        public int    awayWins = 0;
        public int    numGames = 0;
        public string month    = "";
        public int    dayStart = 0;
    }

    [System.Serializable]
    public class TeamSaveData
    {
        public string abbreviation = "";
        public int    wins         = 0;
        public int    losses       = 0;
        public int    runsScored   = 0;
        public int    runsAllowed  = 0;
        public float  payroll      = 0f;
        public float  budget       = 0f;
    }

    [System.Serializable]
    public class PlayerSaveData
    {
        public int    id               = 0;
        public string firstName        = "";
        public string lastName         = "";
        public string position         = "";
        public int    age              = 0;
        public int    overall          = 0;
        public int    contact          = 0;
        public int    power            = 0;
        public int    speed            = 0;
        public int    arm              = 0;
        public int    fielding         = 0;
        public int    pitching         = 0;
        public int    stamina          = 0;
        public float  salary           = 0f;
        public int    contractYears    = 0;
        public string team             = "";
        public bool   isInjured        = false;
        public string injuryType       = "";
        public string injuryStatus     = "";
        public int    injuryDays       = 0;
        public int    injuryDaysTotal  = 0;
        public string battingHand      = "R";
        public string throwingArm      = "R";
        public string bullpenRole      = "";
        public float  confidence       = 50f;
        public string minorLeagueLevel = "";

        // Season stats
        public int seasonAtBats           = 0;
        public int seasonHits             = 0;
        public int seasonHomeRuns         = 0;
        public int seasonRbi              = 0;
        public int seasonWalks            = 0;
        public int seasonStrikeouts       = 0;
        public int seasonSingles          = 0;
        public int seasonDoubles          = 0;
        public int seasonTriples          = 0;
        public int seasonInningsPitched   = 0;
        public int seasonEarnedRuns       = 0;
        public int seasonWins             = 0;
        public int seasonLosses           = 0;
        public int seasonStrikeoutsThrown = 0;
        public int seasonHitsAllowed      = 0;
        public int seasonWalksAllowed     = 0;
    }

    // -------------------------------------------------------
    // SAVE GAME — saves to a specific slot
    // -------------------------------------------------------
    public void SaveGame(FranchiseManager franchise,
                          List<Team> allTeams,
                          int slot = 0)
    {
        SaveData data = new SaveData();

        // Save franchise info
        data.gmName =
            franchise.franchise.gmName;
        data.playerTeamAbbreviation =
            franchise.franchise.playerTeamAbbreviation;
        data.difficulty =
            franchise.franchise.difficulty.ToString();
        data.currentSeason =
            franchise.franchise.currentSeason;
        data.totalSeasons =
            franchise.franchise.totalSeasons;

        // Save schedule progress + current series + recents
        GameManager gm =
            FindFirstObjectByType<GameManager>();
        if (gm != null)
        {
            data.totalGamesPlayed   = gm.totalGamesPlayed;
            data.currentSeriesIndex = gm.currentSeriesIndex;
            data.seasonComplete     = gm.seasonComplete;

            // Save current series mid-game progress
            if (gm.schedule != null &&
                gm.currentSeriesIndex < gm.schedule.Count)
            {
                ScheduledSeries cur =
                    gm.schedule[gm.currentSeriesIndex];
                data.currentSeriesGamesPlayed = cur.gamesPlayed;
                data.currentSeriesHomeWins    = cur.homeWins;
                data.currentSeriesAwayWins    = cur.awayWins;
            }

            // Save up to 4 most recent completed series
            List<ScheduledSeries> recent =
                gm.GetRecentResults(4);
            foreach (ScheduledSeries s in recent)
            {
                data.recentResults.Add(
                    new SeriesResultData
                    {
                        homeTeam = s.homeTeam,
                        awayTeam = s.awayTeam,
                        homeWins = s.homeWins,
                        awayWins = s.awayWins,
                        numGames = s.numGames,
                        month    = s.month,
                        dayStart = s.dayStart
                    });
            }
        }

        // Save all teams and their rosters
        foreach (Team t in allTeams)
        {
            TeamSaveData td  = new TeamSaveData();
            td.abbreviation  = t.abbreviation;
            td.wins          = t.wins;
            td.losses        = t.losses;
            td.runsScored    = t.runsScored;
            td.runsAllowed   = t.runsAllowed;
            td.payroll       = t.payroll;
            td.budget        = t.budget;
            data.teams.Add(td);

            if (t.roster != null)
                foreach (Player p in t.roster)
                    data.players.Add(PlayerToSaveData(p));
            if (t.aaaRoster != null)
                foreach (Player p in t.aaaRoster)
                    data.players.Add(PlayerToSaveData(p));
            if (t.aaRoster != null)
                foreach (Player p in t.aaRoster)
                    data.players.Add(PlayerToSaveData(p));
            if (t.aRoster != null)
                foreach (Player p in t.aRoster)
                    data.players.Add(PlayerToSaveData(p));
        }

        // Write to correct slot
        string key  = saveKeys[Mathf.Clamp(slot, 0, 2)];
        string json = JsonUtility.ToJson(data, true);
        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();

        Debug.Log("Saved to slot " + slot +
                  " — Games: "   + data.totalGamesPlayed +
                  " Series: "    + data.currentSeriesIndex +
                  " SeriesGame: "+ data.currentSeriesGamesPlayed +
                  " Recent: "    + data.recentResults.Count +
                  " Teams: "     + data.teams.Count +
                  " Players: "   + data.players.Count);
    }

    // -------------------------------------------------------
    // LOAD GAME — loads from a specific slot
    // -------------------------------------------------------
    public bool LoadGame(FranchiseManager franchise,
                          List<Team> allTeams,
                          int slot = 0)
    {
        string key = saveKeys[Mathf.Clamp(slot, 0, 2)];

        if (!PlayerPrefs.HasKey(key))
        {
            Debug.Log("No save in slot " + slot);
            return false;
        }

        string json   = PlayerPrefs.GetString(key);
        SaveData data =
            JsonUtility.FromJson<SaveData>(json);
        if (data == null)
        {
            Debug.LogError("Failed to parse save!");
            return false;
        }

        // Restore franchise info
        franchise.franchise.gmName =
            data.gmName;
        franchise.franchise.playerTeamAbbreviation =
            data.playerTeamAbbreviation;
        franchise.franchise.currentSeason =
            data.currentSeason;
        franchise.franchise.totalSeasons =
            data.totalSeasons;
        franchise.franchise.franchiseStarted = true;

        // Restore team records
        foreach (TeamSaveData td in data.teams)
        {
            Team team = allTeams.Find(
                t => t.abbreviation == td.abbreviation);
            if (team == null) continue;
            team.wins        = td.wins;
            team.losses      = td.losses;
            team.runsScored  = td.runsScored;
            team.runsAllowed = td.runsAllowed;
            team.payroll     = td.payroll;
            team.budget      = td.budget;
        }

        // Clear all rosters before restoring
        foreach (Team t in allTeams)
        {
            t.roster    = new List<Player>();
            t.aaaRoster = new List<Player>();
            t.aaRoster  = new List<Player>();
            t.aRoster   = new List<Player>();
        }

        // Restore players to correct teams and levels
        foreach (PlayerSaveData pd in data.players)
        {
            Player p  = SaveDataToPlayer(pd);
            Team team = allTeams.Find(
                t => t.abbreviation == pd.team);
            if (team == null) continue;

            if (pd.minorLeagueLevel == "AAA")
                team.aaaRoster.Add(p);
            else if (pd.minorLeagueLevel == "AA")
                team.aaRoster.Add(p);
            else if (pd.minorLeagueLevel == "A")
                team.aRoster.Add(p);
            else
                team.roster.Add(p);
        }

        // Restore schedule progress + recent results
        // NOTE: schedule index/games restored here so
        // GameManager.LoadGame can read them before
        // GenerateSeasonSchedule wipes them
        GameManager gm =
            FindFirstObjectByType<GameManager>();
        if (gm != null)
        {
            gm.totalGamesPlayed   = data.totalGamesPlayed;
            gm.currentSeriesIndex = data.currentSeriesIndex;
            gm.seasonComplete     = data.seasonComplete;

            // Restore recent results
            gm.savedRecentResults.Clear();
            foreach (SeriesResultData sr in
                     data.recentResults)
            {
                gm.savedRecentResults.Add(
                    new ScheduledSeries
                    {
                        homeTeam   = sr.homeTeam,
                        awayTeam   = sr.awayTeam,
                        homeWins   = sr.homeWins,
                        awayWins   = sr.awayWins,
                        numGames   = sr.numGames,
                        month      = sr.month,
                        dayStart   = sr.dayStart,
                        isComplete = true
                    });
            }

            Debug.Log("Schedule restored: game " +
                gm.totalGamesPlayed +
                " series " + gm.currentSeriesIndex +
                " seriesGame " + data.currentSeriesGamesPlayed +
                " recent " + gm.savedRecentResults.Count +
                " complete=" + gm.seasonComplete);
        }

        Debug.Log("Loaded slot " + slot +
                  " — Season: " + data.currentSeason +
                  " Team: " +
                  data.playerTeamAbbreviation);
        return true;
    }

    // -------------------------------------------------------
    // HAS SAVE — check if a slot has data
    // -------------------------------------------------------
    public bool HasSaveFile(int slot = 0)
    {
        string key = saveKeys[Mathf.Clamp(slot, 0, 2)];
        return PlayerPrefs.HasKey(key);
    }

    public bool HasAnySave()
    {
        foreach (string key in saveKeys)
            if (PlayerPrefs.HasKey(key)) return true;
        return false;
    }

    // -------------------------------------------------------
    // GET SAVE INFO — display string for a slot
    // -------------------------------------------------------
    public string GetSaveInfo(int slot = 0)
    {
        string key = saveKeys[Mathf.Clamp(slot, 0, 2)];
        if (!PlayerPrefs.HasKey(key)) return "";

        string json   = PlayerPrefs.GetString(key);
        SaveData data =
            JsonUtility.FromJson<SaveData>(json);
        if (data == null) return "";

        return data.gmName + " — " +
               data.playerTeamAbbreviation +
               " — Season " + data.currentSeason;
    }

    public List<string> GetAllSaveInfos()
    {
        List<string> infos = new List<string>();
        for (int i = 0; i < saveKeys.Length; i++)
            infos.Add(GetSaveInfo(i));
        return infos;
    }

    // -------------------------------------------------------
    // DELETE SAVE — wipe a specific slot
    // -------------------------------------------------------
    public void DeleteSave(int slot = 0)
    {
        string key = saveKeys[Mathf.Clamp(slot, 0, 2)];
        PlayerPrefs.DeleteKey(key);
        PlayerPrefs.Save();
        Debug.Log("Deleted slot " + slot);
    }

    // -------------------------------------------------------
    // PLAYER → SAVE DATA
    // -------------------------------------------------------
    PlayerSaveData PlayerToSaveData(Player p)
    {
        PlayerSaveData pd         = new PlayerSaveData();
        pd.id                     = p.id;
        pd.firstName              = p.firstName;
        pd.lastName               = p.lastName;
        pd.position               = p.position;
        pd.age                    = p.age;
        pd.overall                = p.overall;
        pd.contact                = p.contact;
        pd.power                  = p.power;
        pd.speed                  = p.speed;
        pd.arm                    = p.arm;
        pd.fielding               = p.fielding;
        pd.pitching               = p.pitching;
        pd.stamina                = p.stamina;
        pd.salary                 = p.salary;
        pd.contractYears          = p.contractYears;
        pd.team                   = p.team;
        pd.isInjured              = p.isInjured;
        pd.injuryType             = p.injuryType;
        pd.injuryStatus           = p.injuryStatus;
        pd.injuryDays             = p.injuryDaysRemaining;
        pd.injuryDaysTotal        = p.injuryDaysTotal;
        pd.battingHand            = p.battingHand;
        pd.throwingArm            = p.throwingArm;
        pd.bullpenRole            = p.bullpenRole;
        pd.confidence             = p.confidence;
        pd.minorLeagueLevel       = p.minorLeagueLevel;
        pd.seasonAtBats           = p.seasonAtBats;
        pd.seasonHits             = p.seasonHits;
        pd.seasonHomeRuns         = p.seasonHomeRuns;
        pd.seasonRbi              = p.seasonRbi;
        pd.seasonWalks            = p.seasonWalks;
        pd.seasonStrikeouts       = p.seasonStrikeouts;
        pd.seasonSingles          = p.seasonSingles;
        pd.seasonDoubles          = p.seasonDoubles;
        pd.seasonTriples          = p.seasonTriples;
        pd.seasonInningsPitched   = p.seasonInningsPitched;
        pd.seasonEarnedRuns       = p.seasonEarnedRuns;
        pd.seasonWins             = p.seasonWins;
        pd.seasonLosses           = p.seasonLosses;
        pd.seasonStrikeoutsThrown = p.seasonStrikeoutsThrown;
        pd.seasonHitsAllowed      = p.seasonHitsAllowed;
        pd.seasonWalksAllowed     = p.seasonWalksAllowed;
        return pd;
    }

    // -------------------------------------------------------
    // SAVE DATA → PLAYER
    // -------------------------------------------------------
    Player SaveDataToPlayer(PlayerSaveData pd)
    {
        Player p                  = new Player();
        p.id                      = pd.id;
        p.firstName               = pd.firstName;
        p.lastName                = pd.lastName;
        p.position                = pd.position;
        p.age                     = pd.age;
        p.overall                 = pd.overall;
        p.contact                 = pd.contact;
        p.power                   = pd.power;
        p.speed                   = pd.speed;
        p.arm                     = pd.arm;
        p.fielding                = pd.fielding;
        p.pitching                = pd.pitching;
        p.stamina                 = pd.stamina;
        p.salary                  = pd.salary;
        p.contractYears           = pd.contractYears;
        p.team                    = pd.team;
        p.isInjured               = pd.isInjured;
        p.injuryType              = pd.injuryType;
        p.injuryStatus            = pd.injuryStatus;
        p.injuryDaysRemaining     = pd.injuryDays;
        p.injuryDaysTotal         = pd.injuryDaysTotal;
        p.battingHand             = pd.battingHand;
        p.throwingArm             = pd.throwingArm;
        p.bullpenRole             = pd.bullpenRole;
        p.confidence              = pd.confidence;
        p.minorLeagueLevel        = pd.minorLeagueLevel;
        p.seasonAtBats            = pd.seasonAtBats;
        p.seasonHits              = pd.seasonHits;
        p.seasonHomeRuns          = pd.seasonHomeRuns;
        p.seasonRbi               = pd.seasonRbi;
        p.seasonWalks             = pd.seasonWalks;
        p.seasonStrikeouts        = pd.seasonStrikeouts;
        p.seasonSingles           = pd.seasonSingles;
        p.seasonDoubles           = pd.seasonDoubles;
        p.seasonTriples           = pd.seasonTriples;
        p.seasonInningsPitched    = pd.seasonInningsPitched;
        p.seasonEarnedRuns        = pd.seasonEarnedRuns;
        p.seasonWins              = pd.seasonWins;
        p.seasonLosses            = pd.seasonLosses;
        p.seasonStrikeoutsThrown  = pd.seasonStrikeoutsThrown;
        p.seasonHitsAllowed       = pd.seasonHitsAllowed;
        p.seasonWalksAllowed      = pd.seasonWalksAllowed;
        return p;
    }
}
