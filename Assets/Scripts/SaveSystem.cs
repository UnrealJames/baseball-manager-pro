using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public string gmName;
    public string playerTeamAbbreviation;
    public int    difficulty;
    public int    currentSeason;
    public int    totalSeasons;
    public List<TeamSaveData>   teams   = new List<TeamSaveData>();
    public List<PlayerSaveData> players = new List<PlayerSaveData>();
}

[System.Serializable]
public class TeamSaveData
{
    public string abbreviation;
    public int    wins;
    public int    losses;
    public int    runsScored;
    public int    runsAllowed;
    public float  payroll;
    public float  budget;
}

[System.Serializable]
public class PlayerSaveData
{
    public int    id;
    public string firstName;
    public string lastName;
    public string position;
    public string team;
    public int    age;
    public int    overall;
    public int    contact;
    public int    power;
    public int    speed;
    public int    arm;
    public int    fielding;
    public int    pitching;
    public int    stamina;
    public float  salary;
    public int    contractYears;
    public string throwingArm;
    public string battingHand;
    public string bullpenRole;
    public bool   isInjured;
    public string minorLeagueLevel;
}

public class SaveSystem : MonoBehaviour
{
    const string SAVE_KEY = "BMP_SaveData";

    // -------------------------------------------------------
    // SAVE GAME
    // -------------------------------------------------------
    public void SaveGame(FranchiseManager franchise,
                          List<Team> allTeams)
    {
        SaveData data = new SaveData();

        // Franchise info
        data.gmName                  =
            franchise.franchise.gmName;
        data.playerTeamAbbreviation  =
            franchise.franchise.playerTeamAbbreviation;
        data.difficulty              =
            franchise.franchise.difficulty;
        data.currentSeason           =
            franchise.franchise.currentSeason;
        data.totalSeasons            =
            franchise.franchise.totalSeasons;

        // Save all teams
        foreach (Team t in allTeams)
        {
            TeamSaveData td    = new TeamSaveData();
            td.abbreviation    = t.abbreviation;
            td.wins            = t.wins;
            td.losses          = t.losses;
            td.runsScored      = t.runsScored;
            td.runsAllowed     = t.runsAllowed;
            td.payroll         = t.payroll;
            td.budget          = t.budget;
            data.teams.Add(td);

            // Save MLB roster
            if (t.roster != null)
                foreach (Player p in t.roster)
                    data.players.Add(PlayerToSaveData(p));

            // Save AAA roster
            if (t.aaaRoster != null)
                foreach (Player p in t.aaaRoster)
                    data.players.Add(PlayerToSaveData(p));

            // Save AA roster
            if (t.aaRoster != null)
                foreach (Player p in t.aaRoster)
                    data.players.Add(PlayerToSaveData(p));

            // Save A roster
            if (t.aRoster != null)
                foreach (Player p in t.aRoster)
                    data.players.Add(PlayerToSaveData(p));
        }

        // Serialize to JSON
        string json = JsonUtility.ToJson(data, true);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();

        Debug.Log("Game saved! " +
                  data.players.Count + " players saved.");
        Debug.Log("Season: " + data.currentSeason +
                  " | GM: " + data.gmName);
    }

    // -------------------------------------------------------
    // LOAD GAME
    // -------------------------------------------------------
    public bool LoadGame(FranchiseManager franchise,
                          List<Team> allTeams)
    {
        if (!PlayerPrefs.HasKey(SAVE_KEY))
        {
            Debug.Log("No save file found.");
            return false;
        }

        string json   = PlayerPrefs.GetString(SAVE_KEY);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        if (data == null)
        {
            Debug.LogError("Failed to parse save data!");
            return false;
        }

        // Restore franchise info
        franchise.franchise.gmName                 =
            data.gmName;
        franchise.franchise.playerTeamAbbreviation =
            data.playerTeamAbbreviation;
        franchise.franchise.difficulty             =
            data.difficulty;
        franchise.franchise.currentSeason          =
            data.currentSeason;
        franchise.franchise.totalSeasons           =
            data.totalSeasons;
        franchise.franchise.franchiseStarted       = true;

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

        // Clear all rosters
        foreach (Team t in allTeams)
        {
            t.roster    = new List<Player>();
            t.aaaRoster = new List<Player>();
            t.aaRoster  = new List<Player>();
            t.aRoster   = new List<Player>();
        }

        // Restore players
        foreach (PlayerSaveData pd in data.players)
        {
            Player p = SaveDataToPlayer(pd);

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

        Debug.Log("Game loaded! Season: " +
                  data.currentSeason +
                  " | GM: " + data.gmName);
        Debug.Log("Players loaded: " + data.players.Count);

        return true;
    }

    // -------------------------------------------------------
    // CHECK IF SAVE EXISTS
    // -------------------------------------------------------
    public bool HasSaveFile()
    {
        return PlayerPrefs.HasKey(SAVE_KEY);
    }

    // -------------------------------------------------------
    // DELETE SAVE
    // -------------------------------------------------------
    public void DeleteSave()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.Save();
        Debug.Log("Save deleted!");
    }

    // -------------------------------------------------------
    // GET SAVE INFO (for continue button)
    // -------------------------------------------------------
    public string GetSaveInfo()
    {
        if (!PlayerPrefs.HasKey(SAVE_KEY))
            return "";

        string json   = PlayerPrefs.GetString(SAVE_KEY);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        if (data == null) return "";

        return data.gmName + " — " +
               data.playerTeamAbbreviation +
               " — Season " + data.currentSeason;
    }

    // -------------------------------------------------------
    // HELPERS
    // -------------------------------------------------------
    PlayerSaveData PlayerToSaveData(Player p)
    {
        PlayerSaveData pd    = new PlayerSaveData();
        pd.id                = p.id;
        pd.firstName         = p.firstName;
        pd.lastName          = p.lastName;
        pd.position          = p.position;
        pd.team              = p.team;
        pd.age               = p.age;
        pd.overall           = p.overall;
        pd.contact           = p.contact;
        pd.power             = p.power;
        pd.speed             = p.speed;
        pd.arm               = p.arm;
        pd.fielding          = p.fielding;
        pd.pitching          = p.pitching;
        pd.stamina           = p.stamina;
        pd.salary            = p.salary;
        pd.contractYears     = p.contractYears;
        pd.throwingArm       = p.throwingArm;
        pd.battingHand       = p.battingHand;
        pd.bullpenRole       = p.bullpenRole;
        pd.isInjured         = p.isInjured;
        pd.minorLeagueLevel  = p.minorLeagueLevel;
        return pd;
    }

    Player SaveDataToPlayer(PlayerSaveData pd)
    {
        Player p         = new Player();
        p.id             = pd.id;
        p.firstName      = pd.firstName;
        p.lastName       = pd.lastName;
        p.position       = pd.position;
        p.team           = pd.team;
        p.age            = pd.age;
        p.overall        = pd.overall;
        p.contact        = pd.contact;
        p.power          = pd.power;
        p.speed          = pd.speed;
        p.arm            = pd.arm;
        p.fielding       = pd.fielding;
        p.pitching       = pd.pitching;
        p.stamina        = pd.stamina;
        p.salary         = pd.salary;
        p.contractYears  = pd.contractYears;
        p.throwingArm    = pd.throwingArm;
        p.battingHand    = pd.battingHand;
        p.bullpenRole    = pd.bullpenRole;
        p.isInjured      = pd.isInjured;
        p.minorLeagueLevel = pd.minorLeagueLevel;
        p.confidence     = 50f;
        return p;
    }
}
