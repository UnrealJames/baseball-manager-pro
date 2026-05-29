using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TradeSystem : MonoBehaviour
{
    private ContractSystem contractSystem;

    void Start()
    {
        contractSystem = GetComponent<ContractSystem>();
    }

    // -------------------------------------------------------
    // EVALUATE TRADE VALUE
    // Returns a numerical value for a player
    // -------------------------------------------------------
    public float GetTradeValue(Player p)
    {
        // Base value from overall rating
        float value = 0f;

        if (p.overall >= 95) value = 100f;
        else if (p.overall >= 90) value = 80f;
        else if (p.overall >= 85) value = 60f;
        else if (p.overall >= 80) value = 45f;
        else if (p.overall >= 75) value = 30f;
        else if (p.overall >= 70) value = 18f;
        else if (p.overall >= 65) value = 10f;
        else                      value =  5f;

        // Age modifier
        float ageMod = 1.0f;
        if      (p.age <= 23) ageMod = 1.4f;  // Young star
        else if (p.age <= 26) ageMod = 1.2f;  // Prime approaching
        else if (p.age <= 29) ageMod = 1.0f;  // Peak
        else if (p.age <= 32) ageMod = 0.85f;
        else if (p.age <= 35) ageMod = 0.65f;
        else                  ageMod = 0.40f;

        // Contract modifier — cheap contracts are valuable
        float marketValue   = contractSystem != null
            ? contractSystem.GetMarketValue(p) : p.salary;
        float contractMod   = 1.0f;

        if (p.salary > 0 && marketValue > 0)
        {
            float ratio = p.salary / marketValue;
            if      (ratio < 0.3f) contractMod = 1.4f; // Very cheap
            else if (ratio < 0.6f) contractMod = 1.2f; // Cheap
            else if (ratio < 0.9f) contractMod = 1.0f; // Fair
            else if (ratio < 1.2f) contractMod = 0.9f; // Slightly overpaid
            else                   contractMod = 0.75f; // Overpaid
        }

        // Years remaining modifier
        if      (p.contractYears >= 5) contractMod *= 1.1f;
        else if (p.contractYears == 1) contractMod *= 0.85f;
        else if (p.contractYears == 0) contractMod *= 0.5f;

        // Injured players worth less
        if (p.isInjured) value *= 0.6f;

        return value * ageMod * contractMod;
    }

    // -------------------------------------------------------
    // EVALUATE TRADE PACKAGE
    // Returns total value of a list of players
    // -------------------------------------------------------
    public float GetPackageValue(List<Player> players)
    {
        return players.Sum(p => GetTradeValue(p));
    }

    // -------------------------------------------------------
    // CPU EVALUATES TRADE OFFER
    // Returns true if CPU accepts
    // -------------------------------------------------------
    public bool CPUEvaluateTrade(Team cpuTeam,
                                  List<Player> playersOffered,
                                  List<Player> playersRequested)
    {
        float offerValue    = GetPackageValue(playersOffered);
        float requestValue  = GetPackageValue(playersRequested);

        // CPU needs to receive fair value
        float ratio = offerValue / Mathf.Max(requestValue, 1f);

        // CPU accepts if getting at least 85% value
        // Small random factor for realism
        float threshold = Random.Range(0.82f, 0.92f);

        bool accepts = ratio >= threshold;

        Debug.Log("\n=== TRADE EVALUATION ===");
        Debug.Log(cpuTeam.city + " " + cpuTeam.nickname +
                  " receives: $" + offerValue.ToString("F1") +
                  " value");
        Debug.Log(cpuTeam.city + " " + cpuTeam.nickname +
                  " gives up: $" + requestValue.ToString("F1") +
                  " value");
        Debug.Log("Value ratio: " + ratio.ToString("F2") +
                  " (need " + threshold.ToString("F2") + ")");
        Debug.Log("Decision: " + (accepts ? "ACCEPTED ✓" : "REJECTED ✗"));

        return accepts;
    }

    // -------------------------------------------------------
    // EXECUTE TRADE
    // -------------------------------------------------------
    public bool ExecuteTrade(Team team1, List<Player> team1Gives,
                              Team team2, List<Player> team2Gives)
    {
        // Validate all players are on correct rosters
        foreach (Player p in team1Gives)
        {
            if (!team1.roster.Contains(p))
            {
                Debug.LogError(p.FullName() +
                               " not on " + team1.abbreviation +
                               " roster!");
                return false;
            }
        }

        foreach (Player p in team2Gives)
        {
            if (!team2.roster.Contains(p))
            {
                Debug.LogError(p.FullName() +
                               " not on " + team2.abbreviation +
                               " roster!");
                return false;
            }
        }

        // Execute the trade
        Debug.Log("\n=== TRADE EXECUTED ===");
        Debug.Log(team1.city + " " + team1.nickname +
                  " trades to " +
                  team2.city + " " + team2.nickname + ":");

        foreach (Player p in team1Gives)
        {
            team1.roster.Remove(p);
            team2.roster.Add(p);
            p.team = team2.abbreviation;
            Debug.Log("  → " + p.FullName() +
                      " (OVR: " + p.overall + ")");
        }

        Debug.Log(team2.city + " " + team2.nickname +
                  " trades to " +
                  team1.city + " " + team1.nickname + ":");

        foreach (Player p in team2Gives)
        {
            team2.roster.Remove(p);
            team1.roster.Add(p);
            p.team = team1.abbreviation;
            Debug.Log("  → " + p.FullName() +
                      " (OVR: " + p.overall + ")");
        }

        // Update payrolls
        if (contractSystem != null)
        {
            contractSystem.UpdatePayroll(team1);
            contractSystem.UpdatePayroll(team2);
        }

        return true;
    }

    // -------------------------------------------------------
    // PROPOSE TRADE TO CPU
    // Human player proposes a trade
    // -------------------------------------------------------
    public bool ProposeTrade(Team playerTeam,
                              List<Player> playersOffered,
                              Team cpuTeam,
                              List<Player> playersRequested)
    {
        Debug.Log("\n=== TRADE PROPOSAL ===");
        Debug.Log(playerTeam.city + " " + playerTeam.nickname +
                  " offers:");
        foreach (Player p in playersOffered)
            Debug.Log("  " + p.FullName() +
                      " (OVR: " + p.overall +
                      ", Value: " +
                      GetTradeValue(p).ToString("F1") + ")");

        Debug.Log("For from " + cpuTeam.city + " " +
                  cpuTeam.nickname + ":");
        foreach (Player p in playersRequested)
            Debug.Log("  " + p.FullName() +
                      " (OVR: " + p.overall +
                      ", Value: " +
                      GetTradeValue(p).ToString("F1") + ")");

        // CPU evaluates
        bool accepted = CPUEvaluateTrade(
            cpuTeam, playersOffered, playersRequested);

        if (accepted)
        {
            return ExecuteTrade(
                playerTeam, playersOffered,
                cpuTeam,    playersRequested);
        }

        // CPU counter offer if close
        float offerVal   = GetPackageValue(playersOffered);
        float requestVal = GetPackageValue(playersRequested);
        float ratio      = offerVal / Mathf.Max(requestVal, 1f);

        if (ratio >= 0.7f)
        {
            Debug.Log("\nCOUNTER OFFER: " + cpuTeam.nickname +
                      " wants more value.");
            Debug.Log("Try adding a prospect or " +
                      "removing a requested player.");
        }
        else
        {
            Debug.Log("\n" + cpuTeam.nickname +
                      " is not interested in this trade.");
        }

        return false;
    }

    // -------------------------------------------------------
    // CPU INITIATES TRADE WITH PLAYER
    // CPU teams occasionally propose trades
    // -------------------------------------------------------
    public void RunCPUTradeInitiations(List<Team> allTeams,
                                        Team playerTeam)
    {
        Debug.Log("\n--- CPU Trade Activity ---");

        int tradesProposed = 0;

        foreach (Team cpuTeam in allTeams)
        {
            if (cpuTeam == playerTeam) continue;

            // 15% chance each team proposes something
            if (Random.value > 0.15f) continue;

            // Find player on CPU team to offer
            Player toOffer = cpuTeam.roster
                .Where(p => p.overall >= 75 &&
                            p.contractYears >= 2)
                .OrderBy(p => Random.value)
                .FirstOrDefault();

            if (toOffer == null) continue;

            // Find player on player team CPU wants
            Player toRequest = playerTeam.roster
                .Where(p => p.overall >= 70 &&
                            p.overall <= toOffer.overall + 5)
                .OrderBy(p => Random.value)
                .FirstOrDefault();

            if (toRequest == null) continue;

            Debug.Log("\n📨 TRADE OFFER from " +
                      cpuTeam.city + " " + cpuTeam.nickname + ":");
            Debug.Log("They offer: " + toOffer.FullName() +
                      " (OVR: " + toOffer.overall + ")");
            Debug.Log("They want: " + toRequest.FullName() +
                      " (OVR: " + toRequest.overall + ")");
            Debug.Log("(In UI you will Accept or Reject this)");

            tradesProposed++;
            if (tradesProposed >= 3) break; // Max 3 proposals
        }

        if (tradesProposed == 0)
            Debug.Log("No trade proposals this period.");
    }

    // -------------------------------------------------------
    // PRINT TRADE BLOCK
    // Shows players available for trade on a team
    // -------------------------------------------------------
    public void PrintTradeBlock(Team team)
    {
        Debug.Log("\n=== " + team.city + " " +
                  team.nickname + " TRADE BLOCK ===");

        // Players likely available — older or overpaid
        var available = team.roster
            .Where(p => p.age >= 32 ||
                        (p.salary > contractSystem
                            .GetMarketValue(p) * 1.1f))
            .OrderByDescending(p => p.overall)
            .ToList();

        if (available.Count == 0)
        {
            Debug.Log("No players on trade block");
            return;
        }

        foreach (Player p in available)
        {
            Debug.Log(p.FullName().PadRight(22) +
                      " | " + p.position.PadRight(3) +
                      " | OVR: " + p.overall +
                      " | Age: " + p.age +
                      " | $" + p.salary.ToString("F1") +
                      "M | Value: " +
                      GetTradeValue(p).ToString("F1"));
        }
    }

    // -------------------------------------------------------
    // CPU VS CPU TRADES
    // Simulate trades between CPU teams during offseason
    // -------------------------------------------------------
    public void RunCPUTrades(List<Team> allTeams, Team playerTeam)
    {
        Debug.Log("\n--- CPU Trade Period ---");

        int tradesCompleted = 0;
        int maxTrades       = 8;

        List<Team> cpuTeams = allTeams
            .Where(t => t != playerTeam)
            .OrderBy(t => Random.value)
            .ToList();

        for (int i = 0; i < cpuTeams.Count - 1; i++)
        {
            if (tradesCompleted >= maxTrades) break;

            Team buyer  = cpuTeams[i];
            Team seller = cpuTeams[i + 1];

            // Find players to swap
            Player buyerOffer = buyer.roster
                .Where(p => p.overall >= 65 &&
                            p.overall <= 82 &&
                            p.contractYears >= 1)
                .OrderBy(p => Random.value)
                .FirstOrDefault();

            Player sellerOffer = seller.roster
                .Where(p => p.overall >= 65 &&
                            p.overall <= 82 &&
                            p.contractYears >= 1)
                .OrderBy(p => Random.value)
                .FirstOrDefault();

            if (buyerOffer == null || sellerOffer == null) continue;

            // Check if values are close enough
            float buyVal  = GetTradeValue(buyerOffer);
            float sellVal = GetTradeValue(sellerOffer);
            float ratio   = Mathf.Min(buyVal, sellVal) /
                            Mathf.Max(buyVal, sellVal);

            if (ratio < 0.75f) continue;

            // Execute trade
            ExecuteTrade(
                buyer,  new List<Player> { buyerOffer },
                seller, new List<Player> { sellerOffer });

            tradesCompleted++;
        }

        Debug.Log("CPU trades completed: " + tradesCompleted);
    }
}
