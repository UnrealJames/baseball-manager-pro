using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class OffseasonManager : MonoBehaviour
{
    private ContractSystem   contractSystem;
    private FreeAgencySystem freeAgencySystem;
    private TradeSystem      tradeSystem;
    private DraftSystem      draftSystem;
    private FranchiseManager franchiseManager;

    void Start()
    {
        contractSystem   = GetComponent<ContractSystem>();
        freeAgencySystem = GetComponent<FreeAgencySystem>();
        tradeSystem      = GetComponent<TradeSystem>();
        draftSystem      = GetComponent<DraftSystem>();
        franchiseManager = GetComponent<FranchiseManager>();
    }

    // -------------------------------------------------------
    // RUN FULL OFFSEASON
    // Call this after World Series ends
    // -------------------------------------------------------
    public void RunOffseason(List<Team> allTeams,
                              Team playerTeam,
                              string worldSeriesWinner,
                              string playerFinish)
    {
        int year = franchiseManager.franchise.currentSeason;

        Debug.Log("\n\n==========================================");
        Debug.Log("       " + year + " OFFSEASON BEGINS");
        Debug.Log("==========================================");
        Debug.Log("World Series Champion: " + worldSeriesWinner);
        Debug.Log("Your finish: " + playerFinish);

        // -------------------------------------------------------
        // PHASE 1 — CONTRACT PROCESSING
        // -------------------------------------------------------
        Debug.Log("\n====== PHASE 1: CONTRACTS ======");

        List<Player> freeAgentPool = new List<Player>();

        // Process arbitration first
        contractSystem.ProcessArbitration(allTeams);

        // Then expire contracts
        contractSystem.ProcessEndOfSeasonContracts(
            allTeams, freeAgentPool);

        // Print player team payroll
        contractSystem.PrintPayroll(playerTeam);

        // -------------------------------------------------------
        // PHASE 2 — CPU TRADES
        // -------------------------------------------------------
        Debug.Log("\n====== PHASE 2: TRADES ======");

        tradeSystem.RunCPUTrades(allTeams, playerTeam);

        // Show incoming trade offers to player
        tradeSystem.RunCPUTradeInitiations(allTeams, playerTeam);

        // -------------------------------------------------------
        // PHASE 3 — FREE AGENCY
        // -------------------------------------------------------
        Debug.Log("\n====== PHASE 3: FREE AGENCY ======");

        freeAgencySystem.RunFreeAgency(allTeams, freeAgentPool);
        freeAgencySystem.PrintSigningSummary(allTeams, freeAgentPool);

        // -------------------------------------------------------
        // PHASE 4 — AMATEUR DRAFT
        // -------------------------------------------------------
        Debug.Log("\n====== PHASE 4: AMATEUR DRAFT ======");

        draftSystem.RunDraft(allTeams, playerTeam, year, 5);
        draftSystem.PromoteProspects(allTeams);

        // -------------------------------------------------------
        // PHASE 5 — PLAYER DEVELOPMENT
        // -------------------------------------------------------
        Debug.Log("\n====== PHASE 5: PLAYER DEVELOPMENT ======");

        franchiseManager.AdvanceToNextSeason(
            allTeams, worldSeriesWinner, playerFinish);

        // -------------------------------------------------------
        // PHASE 6 — ROSTER VALIDATION
        // -------------------------------------------------------
        Debug.Log("\n====== PHASE 6: ROSTER VALIDATION ======");

        ValidateAllRosters(allTeams, freeAgentPool);

        // -------------------------------------------------------
        // OFFSEASON COMPLETE
        // -------------------------------------------------------
        Debug.Log("\n==========================================");
        Debug.Log("    " +
                  franchiseManager.franchise.currentSeason +
                  " SPRING TRAINING BEGINS!");
        Debug.Log("==========================================");

        PrintOffseasonSummary(playerTeam);
    }

    // -------------------------------------------------------
    // VALIDATE ALL ROSTERS
    // Make sure every team has enough players
    // -------------------------------------------------------
        void ValidateAllRosters(List<Team> allTeams,
                             List<Player> freeAgentPool)
    {
        Debug.Log("\n--- Roster Validation ---");

        foreach (Team t in allTeams)
        {
            if (t.roster == null)
                t.roster = new List<Player>();

            // Remove dead/retired players
            t.roster.RemoveAll(p => p == null);

            // Retire players over 42
            List<Player> retired = t.roster
                .FindAll(p => p.age >= 42);
            foreach (Player p in retired)
            {
                t.roster.Remove(p);
                Debug.Log("RETIRED: " + p.FullName() +
                          " (age " + p.age + ")");
            }

            // Count by position type
            int spCount = t.roster.Count(
                p => p.position == "SP");
            int rpCount = t.roster.Count(
                p => p.position == "RP");
            int batCount = t.roster.Count(
                p => p.position != "SP" &&
                     p.position != "RP");

            // Fill SP slots — need at least 5
            while (spCount < 5)
            {
                Player sp = GetFreeAgentOrGenerate(
                    freeAgentPool, "SP", t.abbreviation);
                t.roster.Add(sp);
                spCount++;
                Debug.Log(t.abbreviation +
                          " added SP: " + sp.FullName());
            }

            // Fill RP slots — need at least 6
            while (rpCount < 6)
            {
                Player rp = GetFreeAgentOrGenerate(
                    freeAgentPool, "RP", t.abbreviation);
                rp.bullpenRole = "MR";
                t.roster.Add(rp);
                rpCount++;
                Debug.Log(t.abbreviation +
                          " added RP: " + rp.FullName());
            }

            // Fill position players — need at least 9
            string[] positions = new string[]
            {
                "C", "1B", "2B", "3B",
                "SS", "LF", "CF", "RF", "DH"
            };

            foreach (string pos in positions)
            {
                int posCount = t.roster.Count(
                    p => p.position == pos);

                if (posCount < 1)
                {
                    Player batter = GetFreeAgentOrGenerate(
                        freeAgentPool, pos, t.abbreviation);
                    t.roster.Add(batter);
                    batCount++;
                    Debug.Log(t.abbreviation +
                              " added " + pos + ": " +
                              batter.FullName());
                }
            }

            // Update payroll
            contractSystem.UpdatePayroll(t);

            Debug.Log(t.abbreviation + " final roster: " +
                      t.roster.Count + " players");
        }

        Debug.Log("Roster validation complete!");
    }

    Player GetFreeAgentOrGenerate(
        List<Player> freeAgents, string position,
        string team)
    {
        // Try to find a free agent at this position
        Player fa = freeAgents.FirstOrDefault(
            p => p.position == position);

        if (fa != null)
        {
            freeAgents.Remove(fa);
            fa.team          = team;
            fa.contractYears = Random.Range(1, 3);
            fa.salary        = Random.Range(1f, 5f);
            return fa;
        }

        // Generate a replacement player
        Player p       = new Player();
        p.id           = Random.Range(90000, 99999);
        p.firstName    = GetRandomFirstName();
        p.lastName     = GetRandomLastName();
        p.position     = position;
        p.team         = team;
        p.age          = Random.Range(22, 30);
        p.contractYears = Random.Range(1, 3);
        p.salary       = Random.Range(1f, 4f);
        p.isInjured    = false;
        p.bullpenRole  = "";
        p.confidence   = 50f;

        bool isPitcher = position == "SP" ||
                         position == "RP";

        if (isPitcher)
        {
            p.pitching    = Random.Range(55, 72);
            p.stamina     = Random.Range(55, 72);
            p.overall     = p.pitching;
            p.throwingArm = Random.value > 0.3f ? "R" : "L";
            p.battingHand = "R";
        }
        else
        {
            p.contact     = Random.Range(55, 72);
            p.power       = Random.Range(55, 72);
            p.speed       = Random.Range(55, 72);
            p.arm         = Random.Range(55, 72);
            p.fielding    = Random.Range(55, 72);
            p.overall     = (p.contact + p.power +
                             p.speed + p.arm +
                             p.fielding) / 5;
            p.battingHand = Random.value > 0.5f ? "R" : "L";
            p.throwingArm = "R";
        }

        return p;
    }

    string GetRandomFirstName()
    {
        string[] names = new string[]
        {
            "Carlos", "Jose", "Miguel", "Juan", "Luis",
            "Alex", "Ryan", "Tyler", "Jake", "Kyle",
            "Dylan", "Chase", "Hunter", "Blake", "Cole",
            "Drew", "Logan", "Mason", "Noah", "Ethan",
            "Marcus", "Andre", "Kevin", "Derek", "Mario"
        };
        return names[Random.Range(0, names.Length)];
    }

    string GetRandomLastName()
    {
        string[] names = new string[]
        {
            "Garcias", "Martinezz", "Rodrigues", "Lopezz",
            "Wilsons", "Andersons", "Thomass", "Jacksons",
            "Harriss", "Martins", "Thompsons", "Moores",
            "Taylors", "Lees", "Perezz", "Gonzalezz",
            "Greens", "Halls", "Youngs", "Allens", "Scotts",
            "Adamss", "Nelsons", "Carters", "Collinss"
        };
        return names[Random.Range(0, names.Length)];
    }

    // -------------------------------------------------------
    // OFFSEASON SUMMARY FOR PLAYER TEAM
    // -------------------------------------------------------
    void PrintOffseasonSummary(Team playerTeam)
    {
        Debug.Log("\n=== YOUR TEAM OFFSEASON SUMMARY ===");
        Debug.Log(playerTeam.city + " " + playerTeam.nickname);
        Debug.Log("Roster size: " + playerTeam.roster.Count);
        Debug.Log("Payroll: $" +
                  playerTeam.payroll.ToString("F1") + "M" +
                  " / $" + playerTeam.budget + "M");
        Debug.Log("Cap space: $" +
                  (playerTeam.budget - playerTeam.payroll)
                  .ToString("F1") + "M");

        // Count by position
        int sp  = playerTeam.roster.Count(p => p.position == "SP");
        int rp  = playerTeam.roster.Count(p => p.position == "RP");
        int pos = playerTeam.roster.Count(
            p => p.position != "SP" && p.position != "RP");

        Debug.Log("Starters: "         + sp  +
                  " | Relievers: "     + rp  +
                  " | Position: "      + pos);

        // AAA depth
        int aaaCount = playerTeam.aaaRoster != null
            ? playerTeam.aaaRoster.Count : 0;
        Debug.Log("AAA depth: " + aaaCount + " players");
    }
}
