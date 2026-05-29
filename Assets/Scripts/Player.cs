using System;
using UnityEngine;

[Serializable]
public class Player
{
    // Identity
    public int id;
    public string firstName;
    public string lastName;
    public string position;      // SP, RP, C, 1B, 2B, 3B, SS, LF, CF, RF, DH
    public int age;
    public int overall;          // 0-99
    public string throwingArm;   // "L" or "R"
    public string battingHand;   // "L", "R", or "S" (switch hitter)
    public string bullpenRole;   // "MR" middle relief, "SU" setup, "CL" closer
    public string minorLeagueLevel; // "AAA", "AA", "A", "" for MLB

    // Attributes
    public int contact;          // 0-99
    public int power;            // 0-99
    public int speed;            // 0-99
    public int arm;              // 0-99
    public int fielding;         // 0-99
    public int pitching;         // 0-99 (pitchers only)
    public int stamina;          // 0-99 (pitchers only)

    // Contract
    public float salary;         // in millions
    public int contractYears;
    public string team;

    // Health
    public bool isInjured;
    public int injuryDaysRemaining;
    public int injuryDaysTotal;
    public string injuryType;      // "Hamstring", "Shoulder", etc
    public string injuryStatus;    // "Day-to-Day", "10-Day IL", "60-Day IL"
    public bool isOnIL;

    // Game Stats (resets every game)
    public int gamesPlayed;
    public int atBats;
    public int hits;
    public int singles;
    public int doubles;
    public int triples;
    public int homeRuns;
    public int rbi;
    public int runs;
    public int walks;
    public int strikeouts;

    // Game Pitching Stats (resets every game)
    public int inningsPitched;
    public int earnedRuns;
    public int hitsAllowed;
    public int walksAllowed;
    public int strikeoutsThrown;
    public int wins;
    public int losses;

    // Season Stats (accumulates all season)
    public int seasonGamesPlayed;
    public int seasonAtBats;
    public int seasonHits;
    public int seasonSingles;
    public int seasonDoubles;
    public int seasonTriples;
    public int seasonHomeRuns;
    public int seasonRbi;
    public int seasonRuns;
    public int seasonWalks;
    public int seasonStrikeouts;

    // Season Pitching Stats
    public int seasonInningsPitched;
    public int seasonEarnedRuns;
    public int seasonHitsAllowed;
    public int seasonWalksAllowed;
    public int seasonStrikeoutsThrown;
    public int seasonWins;
    public int seasonLosses;

    // Pitching Mental & Physical State
    public float confidence;
    public float currentStamina;
    public int consecutiveBadGames;

    public void InitializePitcher()
    {
        if (confidence == 0)
            confidence = 50f + (pitching - 50) * 0.5f;
        currentStamina = stamina;
    }

    public float GetFatigueMultiplier(int inningsPitchedToday)
    {
        float staminaThreshold = 20f + (stamina * 0.6f);
        float fatigue = 1f - (inningsPitchedToday / staminaThreshold);
        return Mathf.Clamp(fatigue, 0f, 1f);
    }

    public bool IsTired(int inningsPitchedToday)
    {
        float threshold = 2f + (stamina / 99f) * 5f;
        return inningsPitchedToday >= threshold;
    }

    public void UpdateConfidenceAfterGame(int earnedRunsAllowed, int inningsPitchedToday)
    {
        float era = inningsPitchedToday > 0 ?
                    ((float)earnedRunsAllowed / inningsPitchedToday) * 9f : 99f;

        if (era < 2.0f)
        {
            confidence += 8f;
            consecutiveBadGames = 0;
        }
        else if (era < 3.5f)
        {
            confidence += 4f;
            consecutiveBadGames = 0;
        }
        else if (era < 5.0f)
        {
            confidence -= 2f;
        }
        else if (era < 7.0f)
        {
            confidence -= 8f;
            consecutiveBadGames++;
        }
        else
        {
            confidence -= 15f;
            consecutiveBadGames++;
        }

        confidence = Mathf.Clamp(confidence, 10f, 95f);
    }

    // Game batting average
    public float BattingAverage()
    {
        if (atBats == 0) return 0f;
        return (float)hits / atBats;
    }

    // Season batting average
    public float SeasonBattingAverage()
    {
        if (seasonAtBats == 0) return 0f;
        return (float)seasonHits / seasonAtBats;
    }

    public float SeasonOBP()
    {
        int pa = seasonAtBats + seasonWalks;
        if (pa == 0) return 0f;
        return (float)(seasonHits + seasonWalks) / pa;
    }

    public float SeasonSlugging()
    {
        if (seasonAtBats == 0) return 0f;
        int totalBases = seasonSingles + (seasonDoubles * 2) +
                        (seasonTriples * 3) + (seasonHomeRuns * 4);
        return (float)totalBases / seasonAtBats;
    }

    public float SeasonOPS()
    {
        return SeasonOBP() + SeasonSlugging();
    }

    public float SeasonwOBA()
    {
        float walkWeight    = 0.696f;
        float singleWeight  = 0.888f;
        float doubleWeight  = 1.271f;
        float tripleWeight  = 1.616f;
        float homeRunWeight = 2.101f;

        int pa = seasonAtBats + seasonWalks;
        if (pa == 0) return 0f;

        float numerator = (walkWeight * seasonWalks) +
                         (singleWeight * seasonSingles) +
                         (doubleWeight * seasonDoubles) +
                         (tripleWeight * seasonTriples) +
                         (homeRunWeight * seasonHomeRuns);

        return numerator / pa;
    }

    public float SeasonERA()
    {
        if (seasonInningsPitched == 0) return 0f;
        return ((float)seasonEarnedRuns / seasonInningsPitched) * 9f;
    }

    public float ERA()
    {
        if (inningsPitched == 0) return 0f;
        return ((float)earnedRuns / inningsPitched) * 9f;
    }

    public string FullName()
    {
        return firstName + " " + lastName;
    }
}
