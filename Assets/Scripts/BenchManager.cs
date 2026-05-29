using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BenchManager : MonoBehaviour
{
    private List<int> usedPinchHitters = new List<int>();
    private List<int> usedPinchRunners = new List<int>();

    public void ResetForNewGame()
    {
        usedPinchHitters.Clear();
        usedPinchRunners.Clear();
    }

    // -------------------------------------------------------
    // PINCH HITTER LOGIC
    // -------------------------------------------------------
    public Player ShouldPinchHit(Player currentBatter, Team team,
                                  Player opposingPitcher, int inning,
                                  int outs, bool runnersOnBase)
    {
        if (inning < 6) return null;
        if (currentBatter.overall >= 75) return null;

        float pinchHitChance = 0f;

        if (inning >= 9)      pinchHitChance = 0.6f;
        else if (inning >= 7) pinchHitChance = 0.35f;
        else if (inning >= 6) pinchHitChance = 0.15f;

        if (runnersOnBase) pinchHitChance += 0.2f;

        if (HasPlatoonDisadvantage(currentBatter, opposingPitcher))
            pinchHitChance += 0.2f;

        if (Random.value > pinchHitChance) return null;

        Player pinchHitter = GetBestPinchHitter(team, opposingPitcher, currentBatter);

        if (pinchHitter != null)
        {
            Debug.Log("PINCH HIT: " + pinchHitter.FullName() +
                      " hits for " + currentBatter.FullName() +
                      " | " + pinchHitter.battingHand +
                      " vs " + opposingPitcher.throwingArm + "HP");
        }

        return pinchHitter;
    }

    Player GetBestPinchHitter(Team team, Player opposingPitcher, Player currentBatter)
    {
        List<Player> bench = team.roster
            .Where(p => p.position != "SP" &&
                        p.position != "RP" &&
                        p.overall > currentBatter.overall &&
                        !usedPinchHitters.Contains(p.id))
            .OrderByDescending(p => p.overall)
            .ToList();

        Player advantagePH = bench
            .FirstOrDefault(p => HasPlatoonAdvantage(p, opposingPitcher));

        if (advantagePH != null)
        {
            usedPinchHitters.Add(advantagePH.id);
            return advantagePH;
        }

        Player bestPH = bench.FirstOrDefault();
        if (bestPH != null)
            usedPinchHitters.Add(bestPH.id);

        return bestPH;
    }

    // -------------------------------------------------------
    // PINCH RUNNER LOGIC
    // -------------------------------------------------------
    public Player ShouldPinchRun(Player currentRunner, Team team,
                                  int inning, bool isCloseGame, int score)
    {
        if (inning < 7) return null;
        if (currentRunner.speed >= 70) return null;
        if (!isCloseGame) return null;

        float pinchRunChance = 0.3f;
        if (inning >= 9)      pinchRunChance = 0.6f;
        else if (inning >= 8) pinchRunChance = 0.45f;

        if (Random.value > pinchRunChance) return null;

        Player pinchRunner = GetFastestPinchRunner(team, currentRunner);

        if (pinchRunner != null)
        {
            Debug.Log("PINCH RUN: " + pinchRunner.FullName() +
                      " runs for " + currentRunner.FullName() +
                      " | Speed: " + pinchRunner.speed);
        }

        return pinchRunner;
    }

    Player GetFastestPinchRunner(Team team, Player currentRunner)
    {
        Player fastest = team.roster
            .Where(p => p.position != "SP" &&
                        p.position != "RP" &&
                        p.speed > currentRunner.speed &&
                        !usedPinchRunners.Contains(p.id))
            .OrderByDescending(p => p.speed)
            .FirstOrDefault();

        if (fastest != null)
            usedPinchRunners.Add(fastest.id);

        return fastest;
    }

    // -------------------------------------------------------
    // PULL PITCHER LOGIC
    // -------------------------------------------------------
    public bool ShouldPullPitcher(Player pitcher, int inning,
                                   int earnedRunsThisGame, int teamRunDifference)
    {
        if (inning < 4) return false;

        if (pitcher.IsTired(pitcher.inningsPitched))
        {
            Debug.Log("PITCHING CHANGE: " + pitcher.FullName() +
                      " is exhausted after " + pitcher.inningsPitched + " innings!");
            return true;
        }

        if (earnedRunsThisGame >= 5)
        {
            Debug.Log("PITCHING CHANGE: " + pitcher.FullName() +
                      " is getting shelled — " + earnedRunsThisGame + " ER!");
            return true;
        }

        if (inning >= 7 && pitcher.confidence < 35f)
        {
            Debug.Log("PITCHING CHANGE: " + pitcher.FullName() +
                      " has lost confidence!");
            return true;
        }

        if (inning >= 9 && teamRunDifference > 0 && teamRunDifference <= 3)
        {
            Debug.Log("PITCHING CHANGE: Bringing in closer in the 9th!");
            return true;
        }

        return false;
    }

    // -------------------------------------------------------
    // HELPERS
    // -------------------------------------------------------
    bool HasPlatoonAdvantage(Player batter, Player pitcher)
    {
        if (batter.battingHand == "S") return true;
        if (batter.battingHand == "L" && pitcher.throwingArm == "R") return true;
        if (batter.battingHand == "R" && pitcher.throwingArm == "L") return true;
        return false;
    }

    bool HasPlatoonDisadvantage(Player batter, Player pitcher)
    {
        return !HasPlatoonAdvantage(batter, pitcher);
    }
}
