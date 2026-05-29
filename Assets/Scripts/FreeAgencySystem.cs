using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class FreeAgencySystem : MonoBehaviour
{
    private ContractSystem contractSystem;

    void Start()
    {
        contractSystem = GetComponent<ContractSystem>();
    }

    // -------------------------------------------------------
    // MAIN ENTRY POINT
    // Run full free agency period
    // -------------------------------------------------------
    public void RunFreeAgency(List<Team> allTeams,
                               List<Player> freeAgentPool)
    {
        Debug.Log("\n========== FREE AGENCY OPENS ==========");
        Debug.Log("Players available: " + freeAgentPool.Count);

        // Sort by overall rating
        freeAgentPool = freeAgentPool
            .OrderByDescending(p => p.overall)
            .ToList();

        // CPU teams sign free agents
        RunCPUSignings(allTeams, freeAgentPool);

        // Print remaining free agents
        Debug.Log("\n========== REMAINING FREE AGENTS ==========");
        Debug.Log("Unsigned players: " + freeAgentPool.Count);

        if (freeAgentPool.Count > 0)
            contractSystem.PrintFreeAgents(freeAgentPool);
    }

    // -------------------------------------------------------
    // CPU TEAM SIGNINGS
    // Each CPU team tries to fill roster needs
    // -------------------------------------------------------
    void RunCPUSignings(List<Team> allTeams,
                         List<Player> freeAgentPool)
    {
        Debug.Log("\n--- CPU Team Signings ---");

        // Each team gets multiple signing attempts
        int signingRounds = 5;

        for (int round = 0; round < signingRounds; round++)
        {
            // Shuffle teams for fairness
            List<Team> shuffled = allTeams
                .OrderBy(t => Random.value)
                .ToList();

            foreach (Team team in shuffled)
            {
                if (freeAgentPool.Count == 0) break;

                // Skip if over budget
                float space = team.budget - team.payroll;
                if (space < 0.75f) continue;

                // Find roster needs
                string neededPos = GetBiggestNeed(team);
                if (neededPos == "NONE") continue;

                // Find best available at that position
                Player target = freeAgentPool
                    .Where(p => p.position == neededPos &&
                                contractSystem.GetMarketValue(p) <= space)
                    .OrderByDescending(p => p.overall)
                    .FirstOrDefault();

                // If no exact match try any position
                if (target == null)
                {
                    target = freeAgentPool
                        .Where(p => contractSystem
                                    .GetMarketValue(p) <= space)
                        .OrderByDescending(p => p.overall)
                        .FirstOrDefault();
                }

                if (target == null) continue;

                // CPU offers market value
                float salary = contractSystem.GetMarketValue(target);
                int   years  = GetContractLength(target);

                // Sign the player
                bool signed = contractSystem.SignPlayer(
                    team, target, salary, years);

                if (signed)
                    freeAgentPool.Remove(target);
            }
        }
    }

    // -------------------------------------------------------
    // PLAYER SIGNS WITH TEAM (Human player action)
    // -------------------------------------------------------
    public bool SignFreeAgent(Team playerTeam, Player target,
                               float offerSalary, int offerYears,
                               List<Player> freeAgentPool)
    {
        if (!freeAgentPool.Contains(target))
        {
            Debug.LogError(target.FullName() +
                           " is no longer a free agent!");
            return false;
        }

        // Check if offer is competitive
        float marketValue = contractSystem.GetMarketValue(target);

        if (offerSalary < marketValue * 0.8f)
        {
            Debug.LogWarning(target.FullName() +
                             " rejected offer — too low! " +
                             "Market value: $" +
                             marketValue.ToString("F1") + "M");
            return false;
        }

        // Sign the player
        bool signed = contractSystem.SignPlayer(
            playerTeam, target, offerSalary, offerYears);

        if (signed)
        {
            freeAgentPool.Remove(target);
            return true;
        }

        return false;
    }

    // -------------------------------------------------------
    // GET BIGGEST ROSTER NEED
    // -------------------------------------------------------
    string GetBiggestNeed(Team team)
    {
        if (team.roster == null) return "SP";

        int spCount = team.roster.Count(p => p.position == "SP"
                                          && !p.isInjured);
        int rpCount = team.roster.Count(p => p.position == "RP"
                                          && !p.isInjured);
        int cCount  = team.roster.Count(p => p.position == "C"
                                          && !p.isInjured);
        int ifCount = team.roster.Count(p =>
            (p.position == "1B" || p.position == "2B" ||
             p.position == "3B" || p.position == "SS")
            && !p.isInjured);
        int ofCount = team.roster.Count(p =>
            (p.position == "LF" || p.position == "CF" ||
             p.position == "RF")
            && !p.isInjured);

        // Minimum requirements
        if (spCount < 5)  return "SP";
        if (rpCount < 4)  return "RP";
        if (cCount  < 1)  return "C";
        if (ifCount < 4)  return GetNeededIF(team);
        if (ofCount < 3)  return GetNeededOF(team);

        // Team is full
        if (team.roster.Count >= 26) return "NONE";

        // Fill bench
        return "DH";
    }

    string GetNeededIF(Team team)
    {
        string[] ifPositions = { "1B", "2B", "3B", "SS" };
        foreach (string pos in ifPositions)
        {
            int count = team.roster.Count(
                p => p.position == pos && !p.isInjured);
            if (count < 1) return pos;
        }
        return "1B";
    }

    string GetNeededOF(Team team)
    {
        string[] ofPositions = { "LF", "CF", "RF" };
        foreach (string pos in ofPositions)
        {
            int count = team.roster.Count(
                p => p.position == pos && !p.isInjured);
            if (count < 1) return pos;
        }
        return "CF";
    }

    // -------------------------------------------------------
    // CONTRACT LENGTH BASED ON AGE AND RATING
    // -------------------------------------------------------
    int GetContractLength(Player p)
    {
        if (p.overall >= 90 && p.age <= 28) return Random.Range(5, 8);
        if (p.overall >= 85 && p.age <= 30) return Random.Range(3, 5);
        if (p.overall >= 80 && p.age <= 32) return Random.Range(2, 4);
        if (p.overall >= 75 && p.age <= 33) return Random.Range(1, 3);
        if (p.age >= 36)                    return 1;
        return Random.Range(1, 3);
    }

    // -------------------------------------------------------
    // PRINT TEAM NEEDS
    // -------------------------------------------------------
    public void PrintTeamNeeds(List<Team> allTeams)
    {
        Debug.Log("\n========== TEAM NEEDS ==========");
        foreach (Team t in allTeams)
        {
            string need  = GetBiggestNeed(t);
            float  space = t.budget - t.payroll;
            if (need != "NONE")
                Debug.Log(t.abbreviation.PadRight(5) +
                          " | Need: " + need.PadRight(3) +
                          " | Space: $" +
                          space.ToString("F1") + "M");
        }
    }

    // -------------------------------------------------------
    // PRINT SIGNING SUMMARY
    // -------------------------------------------------------
    public void PrintSigningSummary(List<Team> allTeams,
                                     List<Player> remaining)
    {
        Debug.Log("\n========== FREE AGENCY SUMMARY ==========");
        Debug.Log("Unsigned players: " + remaining.Count);

        // Top unsigned players
        if (remaining.Count > 0)
        {
            Debug.Log("\nTop unsigned free agents:");
            foreach (Player p in remaining.Take(10))
            {
                Debug.Log(p.FullName().PadRight(22) +
                          " | " + p.position.PadRight(3) +
                          " | OVR: " + p.overall +
                          " | Age: " + p.age);
            }
        }
    }
}
