using UnityEngine;

public class AtBatCalculator : MonoBehaviour
{
    public string SimulateAtBat(Player batter, Player pitcher, int inningsPitchedToday)
    {
        // Base probabilities calibrated to 2025 MLB averages
        float homeRunChance   = 3.3f;
        float tripleChance    = 0.6f;
        float doubleChance    = 5.3f;
        float singleChance    = 15.0f;
        float walkChance      = 8.5f;
        float strikeoutChance = 22.5f;

        // --- BATTER ADJUSTMENTS ---
        float powerModifier   = (batter.power - 50) / 200f;
        homeRunChance += powerModifier * 2.0f;
        doubleChance  += powerModifier * 1.0f;

        float contactModifier = (batter.contact - 50) / 200f;
        singleChance    += contactModifier * 3.0f;
        strikeoutChance -= contactModifier * 3.0f;

        // --- PLATOON SPLIT ADJUSTMENTS ---
        bool batterHasAdvantage  = false;
        bool pitcherHasAdvantage = false;

        if (batter.battingHand == "S")
        {
            batterHasAdvantage = true;
        }
        else if (batter.battingHand == "L" && pitcher.throwingArm == "R")
        {
            batterHasAdvantage = true;
        }
        else if (batter.battingHand == "R" && pitcher.throwingArm == "L")
        {
            batterHasAdvantage = true;
        }
        else
        {
            pitcherHasAdvantage = true;
        }

        if (batterHasAdvantage)
        {
            singleChance    += 2.5f;
            doubleChance    += 1.0f;
            homeRunChance   += 0.8f;
            strikeoutChance -= 3.0f;
            walkChance      += 1.0f;
        }
        else if (pitcherHasAdvantage)
        {
            strikeoutChance += 3.0f;
            singleChance    -= 2.0f;
            homeRunChance   -= 0.8f;
            walkChance      -= 0.5f;
        }

        // --- PITCHER BASE SKILL ADJUSTMENTS ---
        float pitchingModifier = (pitcher.pitching - 50) / 200f;
        strikeoutChance -= pitchingModifier * 3.0f;
        singleChance    += pitchingModifier * 2.0f;
        homeRunChance   += pitchingModifier * 1.0f;
        walkChance      += pitchingModifier * 1.0f;

        // --- FATIGUE ADJUSTMENTS ---
        float fatigue        = pitcher.GetFatigueMultiplier(inningsPitchedToday);
        float fatiguePenalty = 1f - fatigue;

        homeRunChance   += fatiguePenalty * 6.0f;
        singleChance    += fatiguePenalty * 5.0f;
        doubleChance    += fatiguePenalty * 3.0f;
        strikeoutChance -= fatiguePenalty * 8.0f;
        walkChance      += fatiguePenalty * 4.0f;

        if (fatiguePenalty > 0.6f)
            Debug.Log(pitcher.FullName() + " is EXHAUSTED — throwing meatballs!");
        else if (fatiguePenalty > 0.3f)
            Debug.Log(pitcher.FullName() + " is tiring...");

        // --- CONFIDENCE ADJUSTMENTS ---
        float confidenceModifier = (pitcher.confidence - 50f) / 100f;
        strikeoutChance += confidenceModifier * 4.0f;
        walkChance      -= confidenceModifier * 2.0f;
        homeRunChance   -= confidenceModifier * 2.0f;

        if (pitcher.consecutiveBadGames >= 3)
        {
            Debug.Log(pitcher.FullName() + " is in a rough patch!");
            homeRunChance += 2.0f;
            singleChance  += 2.0f;
            walkChance    += 2.0f;
        }

        // --- CLAMP ALL VALUES ---
        homeRunChance   = Mathf.Max(0.5f,  homeRunChance);
        tripleChance    = Mathf.Max(0.1f,  tripleChance);
        doubleChance    = Mathf.Max(1.0f,  doubleChance);
        singleChance    = Mathf.Max(5.0f,  singleChance);
        walkChance      = Mathf.Max(2.0f,  walkChance);
        strikeoutChance = Mathf.Max(5.0f,  strikeoutChance);

        // --- ROLL THE DICE ---
        float roll       = Random.Range(0f, 100f);
        string result    = "OUT";
        float cumulative = 0f;

        cumulative += homeRunChance;
        if (roll < cumulative) result = "HOME RUN";
        else
        {
            cumulative += tripleChance;
            if (roll < cumulative) result = "TRIPLE";
            else
            {
                cumulative += doubleChance;
                if (roll < cumulative) result = "DOUBLE";
                else
                {
                    cumulative += singleChance;
                    if (roll < cumulative) result = "SINGLE";
                    else
                    {
                        cumulative += walkChance;
                        if (roll < cumulative) result = "WALK";
                        else
                        {
                            cumulative += strikeoutChance;
                            if (roll < cumulative) result = "STRIKEOUT";
                            else result = "OUT";
                        }
                    }
                }
            }
        }

        // --- TRACK BATTER STATS ---
        if (result != "WALK")
            batter.atBats++;

        if (result == "HOME RUN")       { batter.hits++; batter.homeRuns++; }
        else if (result == "TRIPLE")    { batter.hits++; batter.triples++;  }
        else if (result == "DOUBLE")    { batter.hits++; batter.doubles++;  }
        else if (result == "SINGLE")    { batter.hits++; batter.singles++;  }
        else if (result == "WALK")      { batter.walks++;                   }
        else if (result == "STRIKEOUT") { batter.strikeouts++;              }

        // --- TRACK PITCHER STATS ---
        pitcher.strikeoutsThrown += (result == "STRIKEOUT") ? 1 : 0;
        pitcher.walksAllowed     += (result == "WALK")      ? 1 : 0;
        pitcher.hitsAllowed      += (result == "SINGLE"  || 
                                     result == "DOUBLE"  ||
                                     result == "TRIPLE"  || 
                                     result == "HOME RUN") ? 1 : 0;

        return result;
    }
}
