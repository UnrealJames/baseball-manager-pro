using UnityEngine;
using System.Collections.Generic;

public class InningSimulator : MonoBehaviour
{
    private AtBatCalculator atBatCalculator;

    void Start()
    {
        atBatCalculator = GetComponent<AtBatCalculator>();
    }

    public int SimulateInning(List<Player> battingLineup, Player pitcher,
                              int inningNumber, ref int batterIndex,
                              bool isExtraInning = false, bool isPostseason = false)
    {
        if (battingLineup == null || battingLineup.Count == 0)
        {
            Debug.LogError("No batters found for inning " + inningNumber);
            return 0;
        }

        int outs = 0;
        int runs = 0;

        bool firstBase  = false;
        bool secondBase = false;
        bool thirdBase  = false;

        // MLB extra innings rule — free runner on second
        // Regular season only, not postseason
        if (isExtraInning && !isPostseason)
        {
            secondBase = true;
            batterIndex = Mathf.Max(0, batterIndex - 1);
            Player freeRunner = battingLineup[batterIndex % battingLineup.Count];
            batterIndex++;
            Debug.Log("Extra innings — " + freeRunner.FullName() +
                      " starts on second base!");
        }

        while (outs < 3)
        {
            Player batter = battingLineup[batterIndex % battingLineup.Count];
            batterIndex++;

            string result = atBatCalculator.SimulateAtBat(
                batter, pitcher, pitcher.inningsPitched);

            Debug.Log(batter.FullName() + ": " + result +
                      " | AVG: " + batter.BattingAverage().ToString("F3"));

            int runsScored = 0;

            if (result == "HOME RUN")
            {
                if (thirdBase)  { runsScored++; pitcher.earnedRuns++; }
                if (secondBase) { runsScored++; pitcher.earnedRuns++; }
                if (firstBase)  { runsScored++; pitcher.earnedRuns++; }
                runsScored++;
                pitcher.earnedRuns++;
                batter.runs++;
                firstBase  = false;
                secondBase = false;
                thirdBase  = false;
            }
            else if (result == "TRIPLE")
            {
                if (thirdBase)  { runsScored++; pitcher.earnedRuns++; }
                if (secondBase) { runsScored++; pitcher.earnedRuns++; }
                if (firstBase)  { runsScored++; pitcher.earnedRuns++; }
                firstBase  = false;
                secondBase = false;
                thirdBase  = true;
            }
            else if (result == "DOUBLE")
            {
                if (thirdBase)  { runsScored++; pitcher.earnedRuns++; }
                if (secondBase) { runsScored++; pitcher.earnedRuns++; }
                thirdBase  = firstBase;
                secondBase = true;
                firstBase  = false;
            }
            else if (result == "SINGLE" || result == "WALK")
            {
                if (thirdBase) { runsScored++; pitcher.earnedRuns++; }
                thirdBase  = secondBase;
                secondBase = firstBase;
                firstBase  = true;
            }
            else
            {
                outs++;
            }

            batter.rbi += runsScored;
            runs       += runsScored;

            pitcher.inningsPitched = outs / 3;

            if (batterIndex > 200) break;
        }

        // Full inning completed
        pitcher.inningsPitched++;

        Debug.Log("--- End of Inning " + inningNumber +
                  " | Runs scored: " + runs + " ---");
        return runs;
    }
}
