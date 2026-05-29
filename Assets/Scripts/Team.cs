using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Team
{
    public int id;
    public string city;
    public string nickname;
    public string abbreviation;
    public string division;
    public string league;
    public int wins;
    public int losses;
    public float payroll;
    public float budget;
    public List<Player> roster;
    public List<Player> aaaRoster;
    public List<Player> aaRoster;
    public List<Player> aRoster;
    public List<int> lineup;
    public List<int> rotation;

    // Season record
    public int runsScored;
    public int runsAllowed;

    public float WinPercentage()
    {
        int games = wins + losses;
        if (games == 0) return 0f;
        return (float)wins / games;
    }

    public string Record()
    {
        return wins + "-" + losses;
    }

    public Player GetCloser()
    {
        if (roster == null) return null;
        foreach (Player p in roster)
            if (p.bullpenRole == "CL")
                return p;
        return null;
    }

    public Player GetSetupMan()
    {
        if (roster == null) return null;
        foreach (Player p in roster)
            if (p.bullpenRole == "SU")
                return p;
        return null;
    }

    public List<Player> GetMiddleRelievers()
    {
        List<Player> relievers = new List<Player>();
        if (roster == null) return relievers;
        foreach (Player p in roster)
            if (p.bullpenRole == "MR")
                relievers.Add(p);
        return relievers;
    }

    public Player GetStartingPitcher()
    {
        if (roster == null) return null;
        foreach (Player p in roster)
            if (p.position == "SP")
                return p;
        return null;
    }
}
