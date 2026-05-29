using UnityEngine;
using System.Collections.Generic;

public class RosterBuilder_ALWest : MonoBehaviour
{
    public void BuildAllRosters(List<Team> allTeams)
    {
        foreach (Team t in allTeams)
        {
            if (t.abbreviation == "HST") BuildHST(t);
            if (t.abbreviation == "LAC") BuildLAC(t);
            if (t.abbreviation == "OKP") BuildOKP(t);
            if (t.abbreviation == "SET") BuildSET(t);
            if (t.abbreviation == "TXL") BuildTXL(t);
        }
    }

    Player P(int id, string first, string last, string pos,
             int age, int overall, int contact, int power,
             int speed, int arm, int fielding, int pitching,
             int stamina, float salary, int contractYears,
             string team, string throwArm = "R", string batHand = "R")
    {
        Player p        = new Player();
        p.id            = id;
        p.firstName     = first;
        p.lastName      = last;
        p.position      = pos;
        p.age           = age;
        p.overall       = overall;
        p.contact       = contact;
        p.power         = power;
        p.speed         = speed;
        p.arm           = arm;
        p.fielding      = fielding;
        p.pitching      = pitching;
        p.stamina       = stamina;
        p.salary        = salary;
        p.contractYears = contractYears;
        p.team          = team;
        p.throwingArm   = throwArm;
        p.battingHand   = batHand;
        p.isInjured     = false;
        return p;
    }

    // -------------------------------------------------------
    // HOUSTON STALLIONS — 2026 Astros
    // -------------------------------------------------------
    void BuildHST(Team t)
    {
        t.roster = new List<Player>();

        // ROTATION
        t.roster.Add(P(261, "Framber",  "Valdezz",       "SP", 30, 88, 0, 0, 55, 65, 65, 88, 82, 16.0f, 4, "HST", "L", "L"));
        t.roster.Add(P(262, "Hunter",   "Brownn",        "SP", 27, 84, 0, 0, 52, 62, 62, 84, 80,  5.0f, 3, "HST", "R", "R"));
        t.roster.Add(P(263, "Cristian", "Javierr",       "SP", 28, 80, 0, 0, 50, 58, 58, 80, 76,  7.0f, 3, "HST", "R", "R"));
        t.roster.Add(P(264, "Bennett",  "Sousa",         "SP", 27, 76, 0, 0, 48, 55, 55, 76, 72,  1.0f, 2, "HST", "R", "R"));
        t.roster.Add(P(265, "JP",       "France",        "SP", 31, 74, 0, 0, 46, 52, 52, 74, 70,  3.0f, 1, "HST", "R", "R"));

        // BULLPEN
        Player hstCL = P(266, "Ryan",   "Pressly",       "RP", 36, 82, 0, 0, 50, 60, 60, 82, 72,  8.0f, 1, "HST", "R", "R");
        hstCL.bullpenRole = "CL"; t.roster.Add(hstCL);

        Player hstSU = P(267, "Phil",   "Maton",         "RP", 31, 76, 0, 0, 48, 55, 55, 76, 66,  3.0f, 1, "HST", "R", "R");
        hstSU.bullpenRole = "SU"; t.roster.Add(hstSU);

        Player hstMR1 = P(268, "Rafael","Monteross",     "RP", 29, 74, 0, 0, 44, 50, 50, 74, 62,  2.0f, 1, "HST", "R", "R");
        hstMR1.bullpenRole = "MR"; t.roster.Add(hstMR1);

        Player hstMR2 = P(269, "Bryan", "Abrahamss",     "RP", 28, 72, 0, 0, 42, 48, 48, 72, 60,  1.5f, 1, "HST", "R", "R");
        hstMR2.bullpenRole = "MR"; t.roster.Add(hstMR2);

        Player hstMR3 = P(270, "Hector","Neris",         "RP", 35, 70, 0, 0, 40, 46, 46, 70, 58,  3.0f, 1, "HST", "R", "R");
        hstMR3.bullpenRole = "MR"; t.roster.Add(hstMR3);

        Player hstRP1 = P(271, "Parker","Mushinski",     "RP", 29, 68, 0, 0, 38, 44, 44, 68, 55,  1.0f, 1, "HST", "L", "L");
        hstRP1.bullpenRole = "MR"; t.roster.Add(hstRP1);

        Player hstRP2 = P(272, "Tayler","Scotts",        "RP", 30, 66, 0, 0, 36, 42, 42, 66, 52,  1.0f, 1, "HST", "R", "R");
        hstRP2.bullpenRole = "MR"; t.roster.Add(hstRP2);

        // CATCHERS
        t.roster.Add(P(273, "Yainer",   "Diazz",         "C",  26, 80, 75, 78, 52, 65, 75, 0, 0,  1.0f, 3, "HST", "R", "R"));
        t.roster.Add(P(274, "Victor",   "Caratinis",     "C",  32, 68, 65, 60, 50, 62, 70, 0, 0,  4.0f, 1, "HST", "S", "S"));

        // INFIELDERS
        t.roster.Add(P(275, "Jose",     "Abreus",        "1B", 39, 75, 72, 78, 48, 60, 68, 0, 0, 19.0f, 1, "HST", "R", "R"));
        t.roster.Add(P(276, "Jose",     "Altuvess",      "2B", 36, 86, 88, 75, 75, 78, 82, 0, 0, 29.0f, 3, "HST", "R", "R"));
        t.roster.Add(P(277, "Jeremy",   "Penass",        "SS", 27, 80, 75, 75, 72, 75, 80, 0, 0,  1.0f, 3, "HST", "R", "R"));
        t.roster.Add(P(278, "Alex",     "Bregmans",      "3B", 32, 88, 82, 82, 68, 78, 84, 0, 0, 22.0f, 2, "HST", "R", "R"));
        t.roster.Add(P(279, "Mauricio", "Dubons",        "2B", 30, 70, 68, 65, 70, 68, 72, 0, 0,  4.0f, 1, "HST", "R", "R"));
        t.roster.Add(P(280, "Grae",     "Kessengers",    "SS", 27, 68, 65, 60, 65, 65, 70, 0, 0,  1.0f, 1, "HST", "R", "R"));

        // OUTFIELDERS
        t.roster.Add(P(281, "Yordan",   "Alvarezz",      "LF", 28, 96, 88, 96, 60, 72, 76, 0, 0, 26.0f, 5, "HST", "L", "L"));
        t.roster.Add(P(282, "Kyle",     "Tuckers",       "RF", 28, 90, 85, 85, 78, 80, 82, 0, 0, 13.0f, 1, "HST", "L", "L"));
        t.roster.Add(P(283, "Jake",     "Meyerss",       "CF", 28, 74, 70, 68, 78, 72, 76, 0, 0,  1.0f, 2, "HST", "R", "R"));
        t.roster.Add(P(284, "Joey",     "Loperfidos",    "CF", 25, 72, 68, 68, 78, 68, 72, 0, 0,  1.0f, 1, "HST", "L", "L"));

        // DH / BENCH
        t.roster.Add(P(285, "Michael",  "Brantleys",     "DH", 39, 70, 72, 65, 55, 62, 68, 0, 0,  5.0f, 1, "HST", "L", "L"));
        t.roster.Add(P(286, "Jon",      "Singlesons",    "RF", 34, 70, 70, 65, 68, 65, 70, 0, 0,  5.0f, 1, "HST", "R", "R"));
    }

    // -------------------------------------------------------
    // LOS ANGELES CONDORS — 2026 Angels
    // -------------------------------------------------------
    void BuildLAC(Team t)
    {
        t.roster = new List<Player>();

        // ROTATION
        t.roster.Add(P(287, "Reid",     "Detmerss",      "SP", 27, 78, 0, 0, 50, 58, 58, 78, 74,  3.0f, 2, "LAC", "L", "L"));
        t.roster.Add(P(288, "Tyler",    "Andersons",     "SP", 30, 76, 0, 0, 48, 55, 55, 76, 72,  8.0f, 2, "LAC", "R", "R"));
        t.roster.Add(P(289, "Patrick",  "Sandovals",     "SP", 29, 74, 0, 0, 46, 52, 52, 74, 70,  3.0f, 1, "LAC", "L", "L"));
        t.roster.Add(P(290, "Chase",    "Silseth",       "SP", 26, 72, 0, 0, 44, 50, 50, 72, 68,  1.0f, 1, "LAC", "R", "R"));
        t.roster.Add(P(291, "Jose",     "Soriano",       "SP", 27, 70, 0, 0, 42, 48, 48, 70, 66,  1.0f, 1, "LAC", "R", "R"));

        // BULLPEN
        Player lacCL = P(292, "Carlos", "Estevezz",      "RP", 30, 84, 0, 0, 50, 60, 60, 84, 74,  5.0f, 2, "LAC", "R", "R");
        lacCL.bullpenRole = "CL"; t.roster.Add(lacCL);

        Player lacSU = P(293, "Matt",   "Moores",        "RP", 35, 76, 0, 0, 48, 55, 55, 76, 66,  4.0f, 1, "LAC", "R", "R");
        lacSU.bullpenRole = "SU"; t.roster.Add(lacSU);

        Player lacMR1 = P(294, "Kolby", "Allards",       "RP", 27, 72, 0, 0, 44, 50, 50, 72, 62,  1.5f, 1, "LAC", "L", "L");
        lacMR1.bullpenRole = "MR"; t.roster.Add(lacMR1);

        Player lacMR2 = P(295, "Ben",   "Joycee",        "RP", 28, 70, 0, 0, 42, 48, 48, 70, 60,  1.0f, 1, "LAC", "R", "R");
        lacMR2.bullpenRole = "MR"; t.roster.Add(lacMR2);

        Player lacMR3 = P(296, "Kenyon","Middletons",    "RP", 29, 68, 0, 0, 40, 46, 46, 68, 58,  1.5f, 1, "LAC", "R", "R");
        lacMR3.bullpenRole = "MR"; t.roster.Add(lacMR3);

        Player lacRP1 = P(297, "Adam",  "Kolareks",      "RP", 35, 66, 0, 0, 38, 44, 44, 66, 55,  2.0f, 1, "LAC", "L", "L");
        lacRP1.bullpenRole = "MR"; t.roster.Add(lacRP1);

        Player lacRP2 = P(298, "Jimmy", "Hergets",       "RP", 33, 64, 0, 0, 36, 42, 42, 64, 52,  1.0f, 1, "LAC", "R", "R");
        lacRP2.bullpenRole = "MR"; t.roster.Add(lacRP2);

        // CATCHERS
        t.roster.Add(P(299, "Logan",    "OHoppes",       "C",  25, 78, 72, 75, 52, 65, 74, 0, 0,  1.0f, 3, "LAC", "R", "R"));
        t.roster.Add(P(300, "Chad",     "Wallachs",      "C",  29, 62, 60, 55, 48, 58, 65, 0, 0,  1.0f, 1, "LAC", "R", "R"));

        // INFIELDERS
        t.roster.Add(P(301, "Nolan",    "Schanuels",     "1B", 25, 82, 78, 80, 58, 68, 74, 0, 0,  1.0f, 3, "LAC", "R", "R"));
        t.roster.Add(P(302, "Brandon",  "Drurys",        "2B", 33, 72, 70, 70, 62, 65, 70, 0, 0,  8.0f, 1, "LAC", "R", "R"));
        t.roster.Add(P(303, "Zach",     "Netos",         "SS", 28, 74, 70, 70, 72, 70, 74, 0, 0,  3.0f, 2, "LAC", "R", "R"));
        t.roster.Add(P(304, "Anthony",  "Rendons",       "3B", 36, 68, 68, 65, 58, 65, 72, 0, 0, 38.0f, 1, "LAC", "R", "R"));
        t.roster.Add(P(305, "Luis",     "Rengifo",       "2B", 27, 70, 68, 65, 68, 65, 70, 0, 0,  1.0f, 2, "LAC", "S", "S"));
        t.roster.Add(P(306, "David",    "Fletchers",     "SS", 30, 65, 65, 55, 65, 62, 68, 0, 0,  3.0f, 1, "LAC", "R", "R"));

        // OUTFIELDERS
        t.roster.Add(P(307, "Mike",     "Troutts",       "CF", 35, 88, 85, 88, 80, 82, 85, 0, 0, 37.0f, 2, "LAC", "R", "R"));
        t.roster.Add(P(308, "Taylor",   "Wards",         "RF", 32, 78, 75, 78, 70, 72, 75, 0, 0,  9.0f, 2, "LAC", "R", "R"));
        t.roster.Add(P(309, "Mickey",   "Moniak",        "LF", 27, 72, 68, 70, 72, 68, 70, 0, 0,  1.0f, 2, "LAC", "L", "L"));
        t.roster.Add(P(310, "Hunter",   "Renfroes",      "RF", 32, 70, 68, 72, 65, 65, 68, 0, 0,  3.0f, 1, "LAC", "R", "R"));

        // DH / BENCH
        t.roster.Add(P(311, "Shohei",   "Otani",         "DH", 32, 99, 92, 99, 82, 85, 85, 0, 0, 46.0f, 9, "LAC", "R", "L"));
        t.roster.Add(P(312, "Matt",     "Thaiss",        "1B", 30, 65, 62, 62, 55, 58, 65, 0, 0,  1.0f, 1, "LAC", "L", "L"));
    }

    // -------------------------------------------------------
    // OAKLAND PROSPECTORS — 2026 Athletics
    // -------------------------------------------------------
    void BuildOKP(Team t)
    {
        t.roster = new List<Player>();

        // ROTATION
        t.roster.Add(P(313, "JP",       "Seares",        "SP", 30, 80, 0, 0, 52, 60, 60, 80, 76,  5.0f, 2, "OKP", "R", "R"));
        t.roster.Add(P(314, "Paul",     "Blackburns",    "SP", 31, 76, 0, 0, 50, 58, 58, 76, 72,  4.0f, 2, "OKP", "R", "R"));
        t.roster.Add(P(315, "Mitch",    "Spences",       "SP", 28, 74, 0, 0, 48, 55, 55, 74, 70,  2.0f, 2, "OKP", "R", "R"));
        t.roster.Add(P(316, "Mason",    "Millers",       "SP", 26, 82, 0, 0, 54, 62, 62, 82, 78,  1.0f, 2, "OKP", "R", "R"));
        t.roster.Add(P(317, "Joey",     "Wentzs",        "SP", 27, 70, 0, 0, 44, 50, 50, 70, 66,  1.0f, 1, "OKP", "R", "R"));

        // BULLPEN
        Player okpCL = P(318, "Dany",   "Jimenezz",      "RP", 30, 80, 0, 0, 50, 58, 58, 80, 70,  3.0f, 2, "OKP", "R", "R");
        okpCL.bullpenRole = "CL"; t.roster.Add(okpCL);

        Player okpSU = P(319, "Trevor", "Gott",          "RP", 32, 74, 0, 0, 46, 52, 52, 74, 64,  2.0f, 1, "OKP", "R", "R");
        okpSU.bullpenRole = "SU"; t.roster.Add(okpSU);

        Player okpMR1 = P(320, "Lucas", "Erceg",         "RP", 29, 72, 0, 0, 44, 50, 50, 72, 62,  1.5f, 1, "OKP", "R", "R");
        okpMR1.bullpenRole = "MR"; t.roster.Add(okpMR1);

        Player okpMR2 = P(321, "Sam",   "Moll",          "RP", 31, 70, 0, 0, 42, 48, 48, 70, 60,  1.0f, 1, "OKP", "L", "L");
        okpMR2.bullpenRole = "MR"; t.roster.Add(okpMR2);

        Player okpMR3 = P(322, "Austin","Adams",         "RP", 34, 68, 0, 0, 40, 46, 46, 68, 58,  2.0f, 1, "OKP", "R", "R");
        okpMR3.bullpenRole = "MR"; t.roster.Add(okpMR3);

        Player okpRP1 = P(323, "Kirby", "Snead",         "RP", 30, 66, 0, 0, 38, 44, 44, 66, 55,  1.0f, 1, "OKP", "L", "L");
        okpRP1.bullpenRole = "MR"; t.roster.Add(okpRP1);

        Player okpRP2 = P(324, "Jared", "Koenigs",       "RP", 28, 64, 0, 0, 36, 42, 42, 64, 52,  1.0f, 1, "OKP", "R", "R");
        okpRP2.bullpenRole = "MR"; t.roster.Add(okpRP2);

        // CATCHERS
        t.roster.Add(P(325, "Shea",     "Langelierss",   "C",  26, 72, 65, 72, 50, 62, 70, 0, 0,  1.0f, 2, "OKP", "R", "R"));
        t.roster.Add(P(326, "Carlos",   "Perezz",        "C",  28, 62, 58, 58, 48, 58, 62, 0, 0,  1.0f, 1, "OKP", "R", "R"));

        // INFIELDERS
        t.roster.Add(P(327, "Ryan",     "Noda",          "1B", 28, 74, 72, 72, 55, 62, 68, 0, 0,  1.0f, 2, "OKP", "L", "L"));
        t.roster.Add(P(328, "Zack",     "Gelofs",        "2B", 25, 76, 72, 74, 68, 68, 72, 0, 0,  1.0f, 2, "OKP", "L", "L"));
        t.roster.Add(P(329, "Nick",     "Allens",        "SS", 26, 68, 65, 58, 70, 65, 70, 0, 0,  1.0f, 2, "OKP", "R", "R"));
        t.roster.Add(P(330, "Jordan",   "Diazz",         "3B", 27, 72, 68, 70, 65, 65, 68, 0, 0,  1.0f, 2, "OKP", "R", "R"));
        t.roster.Add(P(331, "Jace",     "Petersons",     "2B", 30, 65, 63, 58, 65, 62, 66, 0, 0,  2.0f, 1, "OKP", "L", "L"));
        t.roster.Add(P(332, "Abraham",  "Toros",         "3B", 28, 62, 60, 60, 60, 60, 64, 0, 0,  1.0f, 1, "OKP", "R", "R"));

        // OUTFIELDERS
        t.roster.Add(P(333, "Esteury",  "Ruizz",         "CF", 25, 72, 68, 62, 88, 70, 74, 0, 0,  1.0f, 2, "OKP", "R", "R"));
        t.roster.Add(P(334, "JJ",       "Bledays",       "LF", 27, 70, 68, 68, 68, 65, 68, 0, 0,  1.0f, 2, "OKP", "L", "L"));
        t.roster.Add(P(335, "Cody",     "Thomass",       "RF", 29, 68, 65, 65, 65, 62, 66, 0, 0,  2.0f, 1, "OKP", "R", "R"));
        t.roster.Add(P(336, "Lawrence", "Butlers",       "LF", 24, 74, 70, 72, 70, 65, 68, 0, 0,  1.0f, 2, "OKP", "L", "L"));

        // DH / BENCH
        t.roster.Add(P(337, "Brent",    "Rookers",       "DH", 30, 76, 70, 80, 55, 60, 65, 0, 0,  4.0f, 2, "OKP", "R", "R"));
        t.roster.Add(P(338, "Aledmys",  "Diazz",         "SS", 33, 62, 60, 58, 58, 60, 64, 0, 0,  2.0f, 1, "OKP", "R", "R"));
    }

    // -------------------------------------------------------
    // SEATTLE TOTEMS — 2026 Mariners
    // -------------------------------------------------------
    void BuildSET(Team t)
    {
        t.roster = new List<Player>();

        // ROTATION
        t.roster.Add(P(339, "Logan",    "Gilberts",      "SP", 28, 90, 0, 0, 58, 65, 65, 90, 84, 15.0f, 4, "SET", "R", "R"));
        t.roster.Add(P(340, "George",   "Kirby",         "SP", 27, 86, 0, 0, 55, 62, 62, 86, 80,  4.0f, 3, "SET", "R", "R"));
        t.roster.Add(P(341, "Bryce",    "Millers",       "SP", 26, 78, 0, 0, 50, 58, 58, 78, 74,  1.0f, 2, "SET", "R", "R"));
        t.roster.Add(P(342, "Bryan",    "Wongg",         "SP", 34, 74, 0, 0, 48, 55, 55, 74, 70, 13.0f, 1, "SET", "L", "L"));
        t.roster.Add(P(343, "Luis",     "Castillos",     "SP", 31, 82, 0, 0, 52, 60, 60, 82, 78, 15.0f, 2, "SET", "R", "R"));

        // BULLPEN
        Player setCL = P(344, "Andres", "Munozz",        "RP", 25, 88, 0, 0, 52, 62, 62, 88, 78,  2.0f, 3, "SET", "R", "R");
        setCL.bullpenRole = "CL"; t.roster.Add(setCL);

        Player setSU = P(345, "Matt",   "Brash",         "RP", 27, 80, 0, 0, 50, 58, 58, 80, 70,  1.0f, 2, "SET", "R", "R");
        setSU.bullpenRole = "SU"; t.roster.Add(setSU);

        Player setMR1 = P(346, "Tayler","Scottss",       "RP", 31, 74, 0, 0, 44, 50, 50, 74, 62,  2.0f, 1, "SET", "R", "R");
        setMR1.bullpenRole = "MR"; t.roster.Add(setMR1);

        Player setMR2 = P(347, "Gabe",  "Speiers",       "RP", 29, 72, 0, 0, 42, 48, 48, 72, 60,  1.5f, 1, "SET", "L", "L");
        setMR2.bullpenRole = "MR"; t.roster.Add(setMR2);

        Player setMR3 = P(348, "Trent", "Thornttons",    "RP", 28, 70, 0, 0, 40, 46, 46, 70, 58,  1.0f, 1, "SET", "R", "R");
        setMR3.bullpenRole = "MR"; t.roster.Add(setMR3);

        Player setRP1 = P(349, "Isaiah","Campbells",     "RP", 27, 68, 0, 0, 38, 44, 44, 68, 55,  1.0f, 1, "SET", "R", "R");
        setRP1.bullpenRole = "MR"; t.roster.Add(setRP1);

        Player setRP2 = P(350, "Penn",  "Murfee",        "RP", 30, 66, 0, 0, 36, 42, 42, 66, 52,  1.0f, 1, "SET", "R", "R");
        setRP2.bullpenRole = "MR"; t.roster.Add(setRP2);

        // CATCHERS
        t.roster.Add(P(351, "Cal",      "Raleighs",      "C",  28, 84, 75, 82, 55, 68, 78, 0, 0,  7.0f, 4, "SET", "R", "R"));
        t.roster.Add(P(352, "Tom",      "Murphys",       "C",  34, 62, 58, 60, 48, 58, 62, 0, 0,  3.0f, 1, "SET", "R", "R"));

        // INFIELDERS
        t.roster.Add(P(353, "Ty",       "Fransas",       "1B", 29, 76, 72, 75, 58, 65, 70, 0, 0,  4.0f, 2, "SET", "L", "L"));
        t.roster.Add(P(354, "Brendan",  "Donovans",      "2B", 30, 78, 78, 68, 68, 70, 76, 0, 0,  8.0f, 3, "SET", "L", "L"));
        t.roster.Add(P(355, "Cole",     "Youngs",        "SS", 22, 74, 72, 65, 72, 68, 72, 0, 0,  1.0f, 2, "SET", "L", "L"));
        t.roster.Add(P(356, "Josh",     "Rohrs",         "3B", 28, 72, 68, 70, 65, 65, 70, 0, 0,  3.0f, 2, "SET", "R", "R"));
        t.roster.Add(P(357, "Dylan",    "Mooress",       "2B", 27, 70, 68, 65, 65, 65, 70, 0, 0,  3.0f, 1, "SET", "R", "R"));
        t.roster.Add(P(358, "Tim",      "Locastros",     "SS", 32, 62, 60, 55, 72, 60, 65, 0, 0,  2.0f, 1, "SET", "R", "R"));

        // OUTFIELDERS
        t.roster.Add(P(359, "Julio",    "Rodriguezz",    "CF", 24, 92, 85, 88, 88, 82, 86, 0, 0,  7.0f, 5, "SET", "R", "R"));
        t.roster.Add(P(360, "Mitch",    "Hannigers",     "RF", 34, 76, 74, 72, 70, 70, 74, 0, 0, 16.0f, 1, "SET", "R", "R"));
        t.roster.Add(P(361, "Teoscar",  "Hernandezz",    "LF", 33, 78, 74, 80, 68, 70, 72, 0, 0, 23.0f, 2, "SET", "R", "R"));
        t.roster.Add(P(362, "Jesse",    "Winkers",       "LF", 31, 74, 74, 70, 60, 62, 66, 0, 0, 21.0f, 1, "SET", "L", "L"));

        // DH / BENCH
        t.roster.Add(P(363, "Eugenio",  "Suarezz",       "DH", 35, 74, 68, 78, 55, 62, 66, 0, 0,  9.0f, 1, "SET", "R", "R"));
        t.roster.Add(P(364, "Josh",     "Bells",         "1B", 32, 72, 70, 72, 52, 58, 65, 0, 0,  6.0f, 1, "SET", "S", "S"));
    }

    // -------------------------------------------------------
    // TEXAS LEGENDS — 2026 Rangers
    // -------------------------------------------------------
    void BuildTXL(Team t)
    {
        t.roster = new List<Player>();

        // ROTATION
        t.roster.Add(P(365, "Nathan",   "Eovaldis",      "SP", 36, 82, 0, 0, 52, 60, 60, 82, 78, 18.0f, 2, "TXL", "R", "R"));
        t.roster.Add(P(366, "Tyler",    "Mahles",        "SP", 30, 80, 0, 0, 50, 58, 58, 80, 76, 20.0f, 2, "TXL", "R", "R"));
        t.roster.Add(P(367, "Kumar",    "Rokars",        "SP", 28, 78, 0, 0, 48, 55, 55, 78, 74,  3.0f, 2, "TXL", "R", "R"));
        t.roster.Add(P(368, "Andrew",   "Heaneys",       "SP", 35, 74, 0, 0, 46, 52, 52, 74, 70, 10.0f, 1, "TXL", "L", "L"));
        t.roster.Add(P(369, "Cody",     "Bradfords",     "SP", 27, 72, 0, 0, 44, 50, 50, 72, 68,  1.0f, 1, "TXL", "L", "L"));

        // BULLPEN
        Player txlCL = P(370, "Jose",   "Leculers",      "RP", 31, 86, 0, 0, 52, 62, 62, 86, 76,  5.0f, 2, "TXL", "R", "R");
        txlCL.bullpenRole = "CL"; t.roster.Add(txlCL);

        Player txlSU = P(371, "David",  "Robertsons",    "RP", 41, 78, 0, 0, 48, 55, 55, 78, 68, 10.0f, 1, "TXL", "R", "R");
        txlSU.bullpenRole = "SU"; t.roster.Add(txlSU);

        Player txlMR1 = P(372, "Aroldis","Chapmans",     "RP", 38, 76, 0, 0, 45, 52, 52, 76, 64,  7.0f, 1, "TXL", "L", "L");
        txlMR1.bullpenRole = "MR"; t.roster.Add(txlMR1);

        Player txlMR2 = P(373, "Will",  "Smithss",       "RP", 30, 74, 0, 0, 42, 48, 48, 74, 62,  4.0f, 1, "TXL", "L", "L");
        txlMR2.bullpenRole = "MR"; t.roster.Add(txlMR2);

        Player txlMR3 = P(374, "Jonathan","Hernandezz",  "RP", 29, 72, 0, 0, 40, 46, 46, 72, 60,  2.0f, 1, "TXL", "R", "R");
        txlMR3.bullpenRole = "MR"; t.roster.Add(txlMR3);

        Player txlRP1 = P(375, "Brett", "Martins",       "RP", 32, 70, 0, 0, 38, 44, 44, 70, 58,  3.0f, 1, "TXL", "L", "L");
        txlRP1.bullpenRole = "MR"; t.roster.Add(txlRP1);

        Player txlRP2 = P(376, "Dane",  "Dunnings",      "RP", 29, 68, 0, 0, 36, 42, 42, 68, 55,  1.0f, 1, "TXL", "R", "R");
        txlRP2.bullpenRole = "MR"; t.roster.Add(txlRP2);

        // CATCHERS
        t.roster.Add(P(377, "Jonah",    "Heims",         "C",  29, 72, 68, 68, 52, 62, 70, 0, 0,  1.0f, 2, "TXL", "S", "S"));
        t.roster.Add(P(378, "Mitch",    "Garvers",       "C",  33, 70, 65, 70, 48, 60, 65, 0, 0,  6.0f, 1, "TXL", "R", "R"));

        // INFIELDERS
        t.roster.Add(P(379, "Nathaniel","Lowes",         "1B", 29, 82, 80, 80, 60, 68, 74, 0, 0,  6.0f, 3, "TXL", "L", "L"));
        t.roster.Add(P(380, "Marcus",   "Semienss",      "2B", 36, 78, 76, 72, 70, 72, 76, 0, 0, 18.0f, 2, "TXL", "R", "R"));
        t.roster.Add(P(381, "Corey",    "Seagers",       "SS", 32, 90, 85, 85, 70, 78, 82, 0, 0, 32.0f, 6, "TXL", "L", "L"));
        t.roster.Add(P(382, "Josh",     "Jungs",         "3B", 27, 82, 78, 80, 68, 72, 76, 0, 0,  1.0f, 3, "TXL", "R", "R"));
        t.roster.Add(P(383, "Ezequiel", "Durann",        "2B", 29, 72, 70, 68, 70, 68, 72, 0, 0,  3.0f, 1, "TXL", "R", "R"));
        t.roster.Add(P(384, "Charlie",  "Culbersons",    "SS", 35, 62, 60, 55, 62, 60, 65, 0, 0,  2.0f, 1, "TXL", "R", "R"));

        // OUTFIELDERS
        t.roster.Add(P(385, "Adolis",   "Garcias",       "RF", 32, 82, 75, 85, 72, 78, 78, 0, 0,  3.0f, 3, "TXL", "R", "R"));
        t.roster.Add(P(386, "Leody",    "Taveras",       "CF", 27, 74, 70, 68, 80, 72, 76, 0, 0,  1.0f, 2, "TXL", "S", "S"));
        t.roster.Add(P(387, "Evan",     "Carters",       "LF", 22, 76, 74, 72, 72, 68, 72, 0, 0,  1.0f, 2, "TXL", "L", "L"));
        t.roster.Add(P(388, "Travis",   "Janskowskis",   "LF", 35, 65, 63, 58, 72, 62, 66, 0, 0,  1.0f, 1, "TXL", "L", "L"));

        // DH / BENCH
        t.roster.Add(P(389, "Wyatt",    "Langfords",     "DH", 24, 78, 75, 76, 72, 68, 72, 0, 0,  1.0f, 2, "TXL", "R", "R"));
        t.roster.Add(P(390, "Brad",     "Millers",       "1B", 35, 62, 60, 60, 60, 58, 62, 0, 0,  2.0f, 1, "TXL", "L", "L"));
    }
}
