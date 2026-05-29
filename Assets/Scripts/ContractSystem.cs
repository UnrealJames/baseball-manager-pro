using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ContractSystem : MonoBehaviour
{
    // -------------------------------------------------------
    // PROCESS END OF SEASON CONTRACTS
    // Call this during offseason
    // -------------------------------------------------------
    public void ProcessEndOfSeasonContracts(List<Team> allTeams,
                                             List<Player> freeAgentPool)
    {
        Debug.Log("\n========== CONTRACT PROCESSING ==========");

        foreach (Team t in allTeams)
        {
            if (t.roster == null) continue;

            List<Player> expiring = new List<Player>();

            foreach (Player p in t.roster)
            {
                // Reduce contract years
                p.contractYears--;

                if (p.contractYears <= 0)
                    expiring.Add(p);
            }

            // Move expired contracts to free agency
            foreach (Player p in expiring)
            {
                t.roster.Remove(p);
                p.team          = "FA";
                p.contractYears = 0;
                p.salary        = 0f;
                freeAgentPool.Add(p);

                Debug.Log("FA: " + p.FullName() +
                          " (" + t.abbreviation + ") — contract expired");
            }

            if (expiring.Count > 0)
                Debug.Log(t.city + " " + t.nickname +
                          " — " + expiring.Count + " players hit FA");

            // Update team payroll
            UpdatePayroll(t);
        }

        Debug.Log("\nTotal free agents: " + freeAgentPool.Count);
    }

    // -------------------------------------------------------
    // SIGN PLAYER
    // -------------------------------------------------------
    public bool SignPlayer(Team team, Player player,
                            float salary, int years)
    {
        // Check budget
        float projectedPayroll = team.payroll + salary;
        if (projectedPayroll > team.budget)
        {
            Debug.LogWarning("Cannot sign " + player.FullName() +
                             " — over budget! " +
                             "Payroll: $" + projectedPayroll +
                             "M / Budget: $" + team.budget + "M");
            return false;
        }

        // Validate salary makes sense for player rating
        float minSalary = GetMinimumSalary(player.overall);
        float maxSalary = GetMaximumSalary(player.overall);

        if (salary < minSalary)
        {
            Debug.LogWarning("Offer too low for " +
                             player.FullName() +
                             " — minimum $" + minSalary + "M");
            return false;
        }

        // Sign the player
        player.salary        = salary;
        player.contractYears = years;
        player.team          = team.abbreviation;

        // Add to roster if not already there
        if (!team.roster.Contains(player))
            team.roster.Add(player);

        UpdatePayroll(team);

        Debug.Log("SIGNED: " + player.FullName() +
                  " — $" + salary + "M x " + years + " years" +
                  " (" + team.city + " " + team.nickname + ")");
        return true;
    }

    // -------------------------------------------------------
    // RELEASE PLAYER
    // -------------------------------------------------------
    public void ReleasePlayer(Team team, Player player,
                               List<Player> freeAgentPool)
    {
        if (!team.roster.Contains(player))
        {
            Debug.LogError(player.FullName() + " not on roster!");
            return;
        }

        team.roster.Remove(player);
        player.team          = "FA";
        player.contractYears = 0;
        freeAgentPool.Add(player);

        UpdatePayroll(team);

        Debug.Log("RELEASED: " + player.FullName() +
                  " by " + team.city + " " + team.nickname);
    }

    // -------------------------------------------------------
    // EXTEND PLAYER CONTRACT
    // -------------------------------------------------------
    public bool ExtendContract(Team team, Player player,
                                float salary, int years)
    {
        if (!team.roster.Contains(player))
        {
            Debug.LogError(player.FullName() + " not on roster!");
            return false;
        }

        // Check budget difference
        float salaryDiff      = salary - player.salary;
        float projectedPayroll = team.payroll + salaryDiff;

        if (projectedPayroll > team.budget)
        {
            Debug.LogWarning("Cannot extend " + player.FullName() +
                             " — over budget!");
            return false;
        }

        float oldSalary       = player.salary;
        int   oldYears        = player.contractYears;
        player.salary         = salary;
        player.contractYears += years;

        UpdatePayroll(team);

        Debug.Log("EXTENDED: " + player.FullName() +
                  " — was $" + oldSalary + "M x " + oldYears +
                  "y remaining" +
                  " → now $" + salary + "M x " +
                  player.contractYears + "y remaining");
        return true;
    }

    // -------------------------------------------------------
    // UPDATE TEAM PAYROLL
    // -------------------------------------------------------
    public void UpdatePayroll(Team team)
    {
        if (team.roster == null) { team.payroll = 0f; return; }
        team.payroll = team.roster.Sum(p => p.salary);
    }

    // -------------------------------------------------------
    // PRINT PAYROLL
    // -------------------------------------------------------
    public void PrintPayroll(Team team)
    {
        Debug.Log("\n=== " + team.city + " " +
                  team.nickname + " PAYROLL ===");
        Debug.Log("Budget:  $" + team.budget + "M");
        Debug.Log("Payroll: $" + team.payroll.ToString("F1") + "M");
        Debug.Log("Space:   $" +
                  (team.budget - team.payroll).ToString("F1") + "M");
        Debug.Log("\nCONTRACTS:");

        if (team.roster == null) return;

        var sorted = team.roster
            .OrderByDescending(p => p.salary)
            .ToList();

        foreach (Player p in sorted)
        {
            if (p.salary <= 0) continue;
            Debug.Log(p.FullName().PadRight(22) +
                      " | " + p.position.PadRight(3) +
                      " | $" + p.salary.ToString("F1") +
                      "M x " + p.contractYears + "y" +
                      " | OVR: " + p.overall);
        }
    }

    // -------------------------------------------------------
    // PRINT FREE AGENT POOL
    // -------------------------------------------------------
    public void PrintFreeAgents(List<Player> freeAgents,
                                 string position = "ALL")
    {
        Debug.Log("\n========== FREE AGENTS ==========");

        var pool = freeAgents.AsEnumerable();

        if (position != "ALL")
            pool = pool.Where(p => p.position == position);

        pool = pool.OrderByDescending(p => p.overall);

        Debug.Log("NAME                 | POS | OVR | AGE | " +
                  "MIN SALARY");
        Debug.Log("--------------------------------------------------");

        foreach (Player p in pool)
        {
            Debug.Log(p.FullName().PadRight(20) +
                      " | " + p.position.PadRight(3) +
                      " | " + p.overall.ToString().PadRight(3) +
                      " | " + p.age.ToString().PadRight(3) +
                      " | $" +
                      GetMinimumSalary(p.overall).ToString("F1") + "M");
        }
    }

    // -------------------------------------------------------
    // SALARY HELPERS
    // Based on player overall rating
    // -------------------------------------------------------
    public float GetMinimumSalary(int overall)
    {
        if (overall >= 90) return 20.0f;
        if (overall >= 85) return 12.0f;
        if (overall >= 80) return  7.0f;
        if (overall >= 75) return  4.0f;
        if (overall >= 70) return  2.0f;
        if (overall >= 65) return  1.0f;
        return 0.75f;
    }

    public float GetMaximumSalary(int overall)
    {
        if (overall >= 95) return 55.0f;
        if (overall >= 90) return 35.0f;
        if (overall >= 85) return 22.0f;
        if (overall >= 80) return 14.0f;
        if (overall >= 75) return  8.0f;
        if (overall >= 70) return  4.0f;
        if (overall >= 65) return  2.0f;
        return 1.0f;
    }

    public float GetMarketValue(Player p)
    {
        // Estimate fair market value based on age and overall
        float base_val = GetMinimumSalary(p.overall);
        float max_val  = GetMaximumSalary(p.overall);

        // Prime age players get closer to max
        float ageMod = 1.0f;
        if      (p.age <= 25) ageMod = 0.7f; // Young — team control
        else if (p.age <= 29) ageMod = 1.0f; // Prime
        else if (p.age <= 32) ageMod = 0.85f;
        else if (p.age <= 35) ageMod = 0.65f;
        else                  ageMod = 0.45f;

        return Mathf.Lerp(base_val, max_val, ageMod);
    }

    // -------------------------------------------------------
    // ARBITRATION
    // Players with 3+ years service get salary bumps
    // -------------------------------------------------------
    public void ProcessArbitration(List<Team> allTeams)
    {
        Debug.Log("\n--- ARBITRATION ---");

        foreach (Team t in allTeams)
        {
            if (t.roster == null) continue;

            // Players making less than market value
            // and under team control get raises
            foreach (Player p in t.roster)
            {
                float market = GetMarketValue(p);

                // Only process if underpaid
                if (p.salary < market * 0.5f && p.age >= 25)
                {
                    float newSalary = market * Random.Range(0.5f, 0.75f);
                    float raise     = newSalary - p.salary;
                    p.salary        = newSalary;

                    Debug.Log("ARB: " + p.FullName() +
                              " (" + t.abbreviation + ")" +
                              " raised to $" +
                              newSalary.ToString("F1") + "M" +
                              " (+" + raise.ToString("F1") + "M)");
                }
            }

            UpdatePayroll(t);
        }
    }
}
