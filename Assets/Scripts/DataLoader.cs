using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PlayerList
{
    public List<Player> players;
}

[System.Serializable]
public class TeamList
{
    public List<Team> teams;
}

public class DataLoader : MonoBehaviour
{
    public List<Player> allPlayers = new List<Player>();
    public List<Team> allTeams = new List<Team>();

    void Start()
    {
        LoadPlayers();
        LoadTeams();
        LinkPlayersToTeams();
    }

    void LoadPlayers()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("players");

        if (jsonFile == null)
        {
            Debug.LogError("Could not find players.json!");
            return;
        }

        PlayerList loadedData = JsonUtility.FromJson<PlayerList>(jsonFile.text);
        allPlayers = loadedData.players;
    }

    void LoadTeams()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("teams");

        if (jsonFile == null)
        {
            Debug.LogError("Could not find teams.json!");
            return;
        }

        TeamList loadedData = JsonUtility.FromJson<TeamList>(jsonFile.text);
        allTeams = loadedData.teams;
        Debug.Log("Teams loaded: " + allTeams.Count);
    }

            void LinkPlayersToTeams()
    {
        // Don't reset rosters here
        // Real rosters are built by RosterBuilders
        // This just links any JSON players
        foreach (Player p in allPlayers)
        {
            Team team = allTeams.Find(
                t => t.abbreviation == p.team);
            if (team != null && team.roster != null)
                if (!team.roster.Contains(p))
                    team.roster.Add(p);
        }
    }


}
