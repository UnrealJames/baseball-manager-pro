using UnityEngine;
using System.Collections.Generic;

public class RosterBuilder_NLCentral : MonoBehaviour
{
    public void BuildAllRosters(List<Team> allTeams)
    {
        foreach (Team t in allTeams)
        {
            if (t.abbreviation == "CHW") BuildCHW(t);
            if (t.abbreviation == "CNR") BuildCNR(t);
            if (t.abbreviation == "MWB") BuildMWB(t);
            if (t.abbreviation == "PGI") BuildPGI(t);
            if (t.abbreviation == "SLA") BuildSLA(t);
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
    // CHICAGO WINDS — 2026 Cubs
    // -------------------------------------------------------
    void BuildCHW(Team t)
    {
        t.roster = new List<Player>();

        // ROTATION
        t.roster.Add(P(521, "Justin",   "Steeles",       "SP", 29, 86, 0, 0, 55, 62, 62, 86, 80,  6.0f, 3, "CHW", "L", "L"));
        t.roster.Add(P(522, "Jameson",  "Taillons",      "SP", 34, 80, 0, 0, 50, 58, 58, 80, 76, 17.0f, 2, "CHW", "R", "R"));
        t.roster.Add(P(523, "Jordan",   "Wiemers",       "SP", 28, 76, 0, 0, 48, 55, 55, 76, 72,  2.0f, 2, "CHW", "R", "R"));
        t.roster.Add(P(524, "Kyle",     "Hendrickss",    "SP", 36, 74, 0, 0, 46, 52, 52, 74, 70, 16.0f, 1, "CHW", "R", "R"));
        t.roster.Add(P(525, "Hayden",   "Wesneskis",     "SP", 27, 78, 0, 0, 50, 58, 58, 78, 74,  1.0f, 2, "CHW", "R", "R"));

        // BULLPEN
        Player chwCL = P(526, "Hector", "Neriass",       "RP", 35, 82, 0, 0, 50, 60, 60, 82, 72,  6.0f, 1, "CHW", "R", "R");
        chwCL.bullpenRole = "CL"; t.roster.Add(chwCL);

        Player chwSU = P(527, "Brandon","Hughes",        "RP", 29, 78, 0, 0, 48, 55, 55, 78, 68,  2.0f, 2, "CHW", "L", "L");
        chwSU.bullpenRole = "SU"; t.roster.Add(chwSU);

        Player chwMR1 = P(528, "Michael","Rucker",       "RP", 31, 74, 0, 0, 44, 50, 50, 74, 62,  2.0f, 1, "CHW", "R", "R");
        chwMR1.bullpenRole = "MR"; t.roster.Add(chwMR1);

        Player chwMR2 = P(529, "Adbert","Alzolay",       "RP", 30, 72, 0, 0, 42, 48, 48, 72, 60,  2.0f, 1, "CHW", "R", "R");
        chwMR2.bullpenRole = "MR"; t.roster.Add(chwMR2);

        Player chwMR3 = P(530, "Julian","Merryweathers", "RP", 31, 70, 0, 0, 40, 46, 46, 70, 58,  2.0f, 1, "CHW", "R", "R");
        chwMR3.bullpenRole = "MR"; t.roster.Add(chwMR3);

        Player chwRP1 = P(531, "Mark",  "Leiterss",      "RP", 28, 68, 0, 0, 38, 44, 44, 68, 55,  1.0f, 1, "CHW", "L", "L");
        chwRP1.bullpenRole = "MR"; t.roster.Add(chwRP1);

        Player chwRP2 = P(532, "Javier","Assad",         "RP", 28, 70, 0, 0, 40, 46, 46, 70, 58,  1.0f, 1, "CHW", "R", "R");
        chwRP2.bullpenRole = "MR"; t.roster.Add(chwRP2);

        // CATCHERS
        t.roster.Add(P(533, "Miguel",   "Amaya",         "C",  25, 72, 68, 68, 52, 62, 70, 0, 0,  1.0f, 2, "CHW", "R", "R"));
        t.roster.Add(P(534, "Tucker",   "Barnharts",     "C",  35, 65, 62, 55, 48, 58, 65, 0, 0,  4.0f, 1, "CHW", "L", "S"));

        // INFIELDERS
        t.roster.Add(P(535, "Michael",  "Busch",         "1B", 27, 78, 75, 78, 58, 65, 70, 0, 0,  1.0f, 2, "CHW", "L", "L"));
        t.roster.Add(P(536, "Nico",     "Hoerners",      "2B", 28, 80, 82, 68, 72, 72, 78, 0, 0,  5.0f, 3, "CHW", "R", "R"));
        t.roster.Add(P(537, "Dansby",   "Swansonn",      "SS", 32, 80, 78, 72, 72, 75, 80, 0, 0, 23.0f, 5, "CHW", "R", "R"));
        t.roster.Add(P(538, "Patrick",  "Wisdoms",       "3B", 33, 72, 65, 78, 58, 65, 65, 0, 0,  3.0f, 1, "CHW", "R", "R"));
        t.roster.Add(P(539, "Christopher","Morells",     "2B", 25, 74, 70, 72, 70, 68, 70, 0, 0,  1.0f, 2, "CHW", "R", "R"));
        t.roster.Add(P(540, "Miles",    "Mastrobouonos", "SS", 29, 68, 65, 60, 65, 62, 68, 0, 0,  2.0f, 1, "CHW", "R", "R"));

        // OUTFIELDERS
        t.roster.Add(P(541, "Ian",      "Happs",         "LF", 31, 82, 78, 82, 70, 72, 74, 0, 0, 17.0f, 3, "CHW", "R", "S"));
        t.roster.Add(P(542, "Cody",     "Bellinger",     "CF", 31, 82, 78, 80, 78, 76, 80, 0, 0, 17.0f, 1, "CHW", "L", "L"));
        t.roster.Add(P(543, "Seiya",    "Suzukis",       "RF", 30, 82, 80, 78, 72, 72, 76, 0, 0, 17.0f, 3, "CHW", "R", "R"));
        t.roster.Add(P(544, "Alexander","Canarios",      "RF", 25, 70, 65, 72, 68, 65, 68, 0, 0,  1.0f, 1, "CHW", "R", "R"));

        // DH / BENCH
        t.roster.Add(P(545, "Trey",     "Mancinis",      "DH", 32, 72, 70, 72, 55, 60, 65, 0, 0,  3.0f, 1, "CHW", "R", "R"));
        t.roster.Add(P(546, "Nick",     "Madrigals",     "2B", 29, 68, 72, 52, 62, 60, 68, 0, 0,  4.0f, 1, "CHW", "R", "R"));
    }

    // -------------------------------------------------------
    // CINCINNATI RIVERMEN — 2026 Reds
    // -------------------------------------------------------
    void BuildCNR(Team t)
    {
        t.roster = new List<Player>();

        // ROTATION
        t.roster.Add(P(547, "Hunter",   "Greenes",       "SP", 26, 86, 0, 0, 58, 65, 65, 86, 80,  3.0f, 3, "CNR", "R", "R"));
        t.roster.Add(P(548, "Frankie",  "Montass",       "SP", 27, 80, 0, 0, 52, 60, 60, 80, 76,  2.0f, 2, "CNR", "R", "R"));
        t.roster.Add(P(549, "Graham",   "Ashcrafts",     "SP", 28, 76, 0, 0, 48, 55, 55, 76, 72,  3.0f, 2, "CNR", "R", "R"));
        t.roster.Add(P(550, "Andrew",   "Abbotts",       "SP", 27, 74, 0, 0, 46, 52, 52, 74, 70,  1.0f, 1, "CNR", "L", "L"));
        t.roster.Add(P(551, "Nick",     "Lodolo",        "SP", 27, 78, 0, 0, 50, 58, 58, 78, 74,  2.0f, 2, "CNR", "L", "L"));

        // BULLPEN
        Player cnrCL = P(552, "Alexis", "Diazz",         "RP", 27, 84, 0, 0, 50, 60, 60, 84, 74,  3.0f, 2, "CNR", "R", "R");
        cnrCL.bullpenRole = "CL"; t.roster.Add(cnrCL);

        Player cnrSU = P(553, "Buck",   "Farmers",       "RP", 34, 76, 0, 0, 48, 55, 55, 76, 66,  4.0f, 1, "CNR", "R", "R");
        cnrSU.bullpenRole = "SU"; t.roster.Add(cnrSU);

        Player cnrMR1 = P(554, "Ian",   "Gibaults",      "RP", 29, 72, 0, 0, 44, 50, 50, 72, 62,  1.5f, 1, "CNR", "L", "L");
        cnrMR1.bullpenRole = "MR"; t.roster.Add(cnrMR1);

        Player cnrMR2 = P(555, "Tony",  "Santillanss",   "RP", 28, 70, 0, 0, 42, 48, 48, 70, 60,  1.0f, 1, "CNR", "R", "R");
        cnrMR2.bullpenRole = "MR"; t.roster.Add(cnrMR2);

        Player cnrMR3 = P(556, "Lucas", "Sims",          "RP", 31, 68, 0, 0, 40, 46, 46, 68, 58,  2.0f, 1, "CNR", "R", "R");
        cnrMR3.bullpenRole = "MR"; t.roster.Add(cnrMR3);

        Player cnrRP1 = P(557, "Fernando","Cruzballs",   "RP", 29, 66, 0, 0, 38, 44, 44, 66, 55,  1.0f, 1, "CNR", "R", "R");
        cnrRP1.bullpenRole = "MR"; t.roster.Add(cnrRP1);

        Player cnrRP2 = P(558, "Justin","Wilsons",       "RP", 38, 64, 0, 0, 36, 42, 42, 64, 52,  2.0f, 1, "CNR", "L", "L");
        cnrRP2.bullpenRole = "MR"; t.roster.Add(cnrRP2);

        // CATCHERS
        t.roster.Add(P(559, "Luke",     "Mauers",        "C",  29, 76, 72, 72, 52, 62, 72, 0, 0,  3.0f, 2, "CNR", "R", "R"));
        t.roster.Add(P(560, "Tyler",    "Stephensons",   "C",  28, 74, 72, 70, 50, 60, 70, 0, 0,  5.0f, 2, "CNR", "R", "R"));

        // INFIELDERS
        t.roster.Add(P(561, "Joey",     "Vottos",        "1B", 42, 68, 72, 65, 48, 58, 68, 0, 0, 25.0f, 1, "CNR", "L", "L"));
        t.roster.Add(P(562, "Jonathan", "Indias",        "2B", 29, 80, 80, 72, 72, 70, 75, 0, 0,  5.0f, 2, "CNR", "R", "R"));
        t.roster.Add(P(563, "Elly",     "DeLeGalloss",   "SS", 23, 84, 78, 78, 88, 78, 80, 0, 0,  1.0f, 3, "CNR", "S", "S"));
        t.roster.Add(P(564, "Jeimer",   "Candelarioss",  "3B", 31, 76, 74, 74, 62, 65, 70, 0, 0,  5.0f, 2, "CNR", "S", "S"));
        t.roster.Add(P(565, "Matt",     "McLains",       "SS", 25, 76, 72, 70, 72, 68, 72, 0, 0,  1.0f, 2, "CNR", "L", "L"));
        t.roster.Add(P(566, "Kevin",    "Newmans",       "2B", 31, 65, 63, 55, 62, 62, 67, 0, 0,  3.0f, 1, "CNR", "R", "R"));

        // OUTFIELDERS
        t.roster.Add(P(567, "TJ",       "Friedls",       "CF", 29, 74, 70, 65, 82, 72, 76, 0, 0,  1.0f, 2, "CNR", "L", "L"));
        t.roster.Add(P(568, "Jake",     "Fraley",        "LF", 30, 70, 68, 65, 70, 65, 68, 0, 0,  2.0f, 1, "CNR", "L", "L"));
        t.roster.Add(P(569, "Spencer",  "Steer",         "RF", 27, 76, 72, 74, 65, 65, 68, 0, 0,  1.0f, 2, "CNR", "R", "R"));
        t.roster.Add(P(570, "Nick",     "Senzel",        "CF", 29, 68, 65, 62, 70, 62, 66, 0, 0,  3.0f, 1, "CNR", "R", "R"));

        // DH / BENCH
        t.roster.Add(P(571, "Will",     "Bensons",       "DH", 25, 70, 68, 68, 68, 62, 66, 0, 0,  1.0f, 1, "CNR", "L", "L"));
        t.roster.Add(P(572, "Christian","Encarnacion-Strands","RF",23,72,68,72,68,65,68, 0, 0, 1.0f, 1, "CNR", "R", "R"));
    }

    // -------------------------------------------------------
    // MILWAUKEE BADGERS — 2026 Brewers
    // -------------------------------------------------------
    void BuildMWB(Team t)
    {
        t.roster = new List<Player>();

        // ROTATION
        t.roster.Add(P(573, "Freddy",   "Peraltass",     "SP", 27, 84, 0, 0, 55, 62, 62, 84, 78,  2.0f, 3, "MWB", "R", "R"));
        t.roster.Add(P(574, "Colin",    "Raids",         "SP", 27, 82, 0, 0, 52, 60, 60, 82, 76,  2.0f, 2, "MWB", "R", "R"));
        t.roster.Add(P(575, "DL",       "Halls",         "SP", 26, 78, 0, 0, 50, 58, 58, 78, 74,  1.0f, 2, "MWB", "R", "R"));
        t.roster.Add(P(576, "Aaron",    "Civales",       "SP", 30, 74, 0, 0, 46, 52, 52, 74, 70,  5.0f, 1, "MWB", "R", "R"));
        t.roster.Add(P(577, "Joe",      "Rossss",        "SP", 30, 72, 0, 0, 44, 50, 50, 72, 68,  3.0f, 1, "MWB", "R", "R"));

        // BULLPEN
        Player mwbCL = P(578, "Devin",  "Williams",      "RP", 30, 90, 0, 0, 55, 65, 65, 90, 80,  5.0f, 3, "MWB", "R", "R");
        mwbCL.bullpenRole = "CL"; t.roster.Add(mwbCL);

        Player mwbSU = P(579, "Joel",   "Payampes",      "RP", 30, 80, 0, 0, 50, 58, 58, 80, 70,  4.0f, 2, "MWB", "R", "R");
        mwbSU.bullpenRole = "SU"; t.roster.Add(mwbSU);

        Player mwbMR1 = P(580, "Elvis", "Pegueros",      "RP", 28, 74, 0, 0, 44, 50, 50, 74, 62,  1.5f, 1, "MWB", "R", "R");
        mwbMR1.bullpenRole = "MR"; t.roster.Add(mwbMR1);

        Player mwbMR2 = P(581, "Thyago","Vieirass",      "RP", 32, 70, 0, 0, 42, 48, 48, 70, 60,  1.0f, 1, "MWB", "R", "R");
        mwbMR2.bullpenRole = "MR"; t.roster.Add(mwbMR2);

        Player mwbMR3 = P(582, "Hoby",  "Milners",       "RP", 32, 68, 0, 0, 40, 46, 46, 68, 58,  2.0f, 1, "MWB", "L", "L");
        mwbMR3.bullpenRole = "MR"; t.roster.Add(mwbMR3);

        Player mwbRP1 = P(583, "Trevor","Megills",       "RP", 30, 66, 0, 0, 38, 44, 44, 66, 55,  1.0f, 1, "MWB", "R", "R");
        mwbRP1.bullpenRole = "MR"; t.roster.Add(mwbRP1);

        Player mwbRP2 = P(584, "Jake",  "Cousins",       "RP", 31, 64, 0, 0, 36, 42, 42, 64, 52,  1.0f, 1, "MWB", "R", "R");
        mwbRP2.bullpenRole = "MR"; t.roster.Add(mwbRP2);

        // CATCHERS
        t.roster.Add(P(585, "William",  "Contreras",     "C",  27, 82, 76, 80, 52, 65, 72, 0, 0,  5.0f, 3, "MWB", "R", "R"));
        t.roster.Add(P(586, "Eric",     "Haases",        "C",  33, 65, 60, 62, 48, 58, 62, 0, 0,  6.0f, 1, "MWB", "R", "R"));

        // INFIELDERS
        t.roster.Add(P(587, "Carlos",   "Santanass",     "1B", 39, 72, 72, 70, 48, 58, 66, 0, 0, 14.0f, 1, "MWB", "S", "S"));
        t.roster.Add(P(588, "Brice",    "Tuffys",        "2B", 27, 74, 72, 68, 68, 68, 72, 0, 0,  1.0f, 2, "MWB", "R", "R"));
        t.roster.Add(P(589, "Willy",    "Adames",        "SS", 30, 84, 78, 82, 72, 74, 78, 0, 0, 18.0f, 4, "MWB", "R", "R"));
        t.roster.Add(P(590, "Joey",     "Wiemers",       "3B", 27, 74, 68, 72, 65, 65, 70, 0, 0,  1.0f, 2, "MWB", "R", "R"));
        t.roster.Add(P(591, "Abraham",  "Toros",         "2B", 28, 65, 63, 60, 62, 60, 65, 0, 0,  2.0f, 1, "MWB", "R", "R"));
        t.roster.Add(P(592, "Oliver",   "Duranos",       "SS", 25, 68, 65, 62, 68, 65, 68, 0, 0,  1.0f, 1, "MWB", "R", "R"));

        // OUTFIELDERS
        t.roster.Add(P(593, "Christian","Yealichs",      "LF", 33, 80, 78, 78, 72, 70, 74, 0, 0, 22.0f, 2, "MWB", "L", "L"));
        t.roster.Add(P(594, "Joey",     "Wiemers",       "CF", 27, 74, 70, 70, 72, 68, 72, 0, 0,  1.0f, 2, "MWB", "R", "R"));
        t.roster.Add(P(595, "Sal",      "Frelicks",      "RF", 26, 72, 70, 62, 78, 68, 74, 0, 0,  1.0f, 2, "MWB", "R", "R"));
        t.roster.Add(P(596, "Tyrone",   "Taylors",       "CF", 30, 68, 65, 65, 68, 62, 66, 0, 0,  2.0f, 1, "MWB", "R", "R"));

        // DH / BENCH
        t.roster.Add(P(597, "Rowdy",    "Tellez",        "DH", 30, 74, 70, 78, 48, 58, 64, 0, 0,  4.0f, 1, "MWB", "L", "L"));
        t.roster.Add(P(598, "Blake",    "Perkinss",      "LF", 29, 65, 63, 62, 62, 58, 62, 0, 0,  1.0f, 1, "MWB", "L", "L"));
    }

    // -------------------------------------------------------
    // PITTSBURGH IRONMEN — 2026 Pirates
    // -------------------------------------------------------
    void BuildPGI(Team t)
    {
        t.roster = new List<Player>();

        // ROTATION
        t.roster.Add(P(599,  "Paul",    "Skeeness",      "SP", 24, 88, 0, 0, 58, 65, 65, 88, 82,  1.0f, 3, "PGI", "R", "R"));
        t.roster.Add(P(600,  "Mitch",   "Kellers",       "SP", 29, 82, 0, 0, 52, 60, 60, 82, 78,  5.0f, 3, "PGI", "R", "R"));
        t.roster.Add(P(601,  "Martin",  "Perezz",        "SP", 34, 74, 0, 0, 46, 52, 52, 74, 70,  8.0f, 1, "PGI", "L", "L"));
        t.roster.Add(P(602,  "Quinn",   "Primms",        "SP", 26, 72, 0, 0, 44, 50, 50, 72, 68,  1.0f, 1, "PGI", "R", "R"));
        t.roster.Add(P(603,  "Luis",    "Ortizz",        "SP", 26, 70, 0, 0, 42, 48, 48, 70, 66,  1.0f, 1, "PGI", "R", "R"));

        // BULLPEN
        Player pgiCL = P(604, "David",  "Bedfords",      "RP", 29, 82, 0, 0, 50, 60, 60, 82, 72,  3.0f, 2, "PGI", "R", "R");
        pgiCL.bullpenRole = "CL"; t.roster.Add(pgiCL);

        Player pgiSU = P(605, "Colin",  "Holdermans",    "RP", 28, 76, 0, 0, 48, 55, 55, 76, 66,  1.5f, 2, "PGI", "L", "L");
        pgiSU.bullpenRole = "SU"; t.roster.Add(pgiSU);

        Player pgiMR1 = P(606, "Kyle",  "Nicolas",       "RP", 25, 72, 0, 0, 44, 50, 50, 72, 62,  1.0f, 1, "PGI", "R", "R");
        pgiMR1.bullpenRole = "MR"; t.roster.Add(pgiMR1);

        Player pgiMR2 = P(607, "Ryan",  "Borucki",       "RP", 31, 70, 0, 0, 42, 48, 48, 70, 60,  1.5f, 1, "PGI", "L", "L");
        pgiMR2.bullpenRole = "MR"; t.roster.Add(pgiMR2);

        Player pgiMR3 = P(608, "Dauri", "Moretass",      "RP", 26, 68, 0, 0, 40, 46, 46, 68, 58,  1.0f, 1, "PGI", "R", "R");
        pgiMR3.bullpenRole = "MR"; t.roster.Add(pgiMR3);

        Player pgiRP1 = P(609, "Angel", "Perdomo",       "RP", 28, 66, 0, 0, 38, 44, 44, 66, 55,  1.0f, 1, "PGI", "L", "L");
        pgiRP1.bullpenRole = "MR"; t.roster.Add(pgiRP1);

        Player pgiRP2 = P(610, "Eric",  "Stouts",        "RP", 29, 64, 0, 0, 36, 42, 42, 64, 52,  1.0f, 1, "PGI", "R", "R");
        pgiRP2.bullpenRole = "MR"; t.roster.Add(pgiRP2);

        // CATCHERS
        t.roster.Add(P(611, "Henry",    "Daviss",        "C",  24, 74, 68, 72, 52, 62, 70, 0, 0,  1.0f, 2, "PGI", "R", "R"));
        t.roster.Add(P(612, "Austin",   "Allens",        "C",  29, 62, 58, 58, 48, 56, 62, 0, 0,  1.0f, 1, "PGI", "R", "R"));

        // INFIELDERS
        t.roster.Add(P(613, "Rowdy",    "Tellezz",       "1B", 30, 74, 70, 78, 48, 58, 64, 0, 0,  4.0f, 1, "PGI", "L", "L"));
        t.roster.Add(P(614, "Tucupita", "Marcanos",      "2B", 25, 72, 70, 65, 68, 65, 70, 0, 0,  1.0f, 2, "PGI", "L", "L"));
        t.roster.Add(P(615, "Oneil",    "Cruzzes",       "SS", 26, 80, 72, 80, 72, 74, 74, 0, 0,  1.0f, 3, "PGI", "S", "S"));
        t.roster.Add(P(616, "Ke'Bryan", "Hayess",        "3B", 28, 82, 78, 72, 72, 76, 82, 0, 0,  4.0f, 4, "PGI", "R", "R"));
        t.roster.Add(P(617, "Ji Hwan", "Baes",           "2B", 25, 68, 65, 60, 70, 62, 66, 0, 0,  1.0f, 1, "PGI", "R", "R"));
        t.roster.Add(P(618, "Connor",   "Joes",          "SS", 31, 65, 62, 58, 62, 60, 65, 0, 0,  2.0f, 1, "PGI", "R", "R"));

        // OUTFIELDERS
        t.roster.Add(P(619, "Bryan",    "Reynolds",      "CF", 30, 84, 80, 80, 74, 72, 76, 0, 0,  4.0f, 4, "PGI", "S", "S"));
        t.roster.Add(P(620, "Jack",     "Suwinski",      "LF", 27, 74, 68, 78, 68, 65, 68, 0, 0,  1.0f, 2, "PGI", "L", "L"));
        t.roster.Add(P(621, "Edward",   "Olivares",      "RF", 29, 70, 67, 68, 68, 62, 66, 0, 0,  1.0f, 1, "PGI", "R", "R"));
        t.roster.Add(P(622, "Ji Hwan",  "Baess",         "RF", 25, 68, 65, 62, 70, 62, 66, 0, 0,  1.0f, 1, "PGI", "R", "R"));

        // DH / BENCH
        t.roster.Add(P(623, "Andrew",   "McCutchens",    "DH", 39, 70, 70, 68, 62, 62, 65, 0, 0,  7.0f, 1, "PGI", "R", "R"));
        t.roster.Add(P(624, "Carlos",   "Santanasss",    "1B", 39, 65, 65, 62, 45, 55, 62, 0, 0,  5.0f, 1, "PGI", "S", "S"));
    }

    // -------------------------------------------------------
    // ST. LOUIS ARCHMEN — 2026 Cardinals
    // -------------------------------------------------------
    void BuildSLA(Team t)
    {
        t.roster = new List<Player>();

        // ROTATION
        t.roster.Add(P(625, "Miles",    "Mikolas",       "SP", 38, 78, 0, 0, 50, 58, 58, 78, 74, 17.0f, 2, "SLA", "R", "R"));
        t.roster.Add(P(626, "Andre",    "Pallantes",     "SP", 27, 74, 0, 0, 48, 55, 55, 74, 70,  1.0f, 2, "SLA", "R", "R"));
        t.roster.Add(P(627, "Kyle",     "Gibsons",       "SP", 38, 70, 0, 0, 44, 50, 50, 70, 66, 10.0f, 1, "SLA", "R", "R"));
        t.roster.Add(P(628, "Matthew",  "Liberatores",   "SP", 27, 76, 0, 0, 48, 55, 55, 76, 72,  2.0f, 2, "SLA", "L", "L"));
        t.roster.Add(P(629, "Erick",    "Feddes",        "SP", 27, 72, 0, 0, 44, 50, 50, 72, 68,  3.0f, 1, "SLA", "R", "R"));

        // BULLPEN
        Player slaCL = P(630, "Ryan",   "Helsley",       "RP", 31, 90, 0, 0, 55, 65, 65, 90, 80,  6.0f, 3, "SLA", "R", "R");
        slaCL.bullpenRole = "CL"; t.roster.Add(slaCL);

        Player slaSU = P(631, "JoJo",   "Romeros",       "RP", 29, 78, 0, 0, 48, 55, 55, 78, 68,  2.0f, 2, "SLA", "L", "L");
        slaSU.bullpenRole = "SU"; t.roster.Add(slaSU);

        Player slaMR1 = P(632, "Chris", "Strattons",     "RP", 34, 74, 0, 0, 44, 50, 50, 74, 62,  2.5f, 1, "SLA", "R", "R");
        slaMR1.bullpenRole = "MR"; t.roster.Add(slaMR1);

        Player slaMR2 = P(633, "Giovanny","Gallegos",    "RP", 33, 72, 0, 0, 42, 48, 48, 72, 60,  4.0f, 1, "SLA", "R", "R");
        slaMR2.bullpenRole = "MR"; t.roster.Add(slaMR2);

        Player slaMR3 = P(634, "Drew",  "VerHagenn",     "RP", 32, 70, 0, 0, 40, 46, 46, 70, 58,  3.0f, 1, "SLA", "R", "R");
        slaMR3.bullpenRole = "MR"; t.roster.Add(slaMR3);

        Player slaRP1 = P(635, "Nick",  "Robertsons",    "RP", 30, 68, 0, 0, 38, 44, 44, 68, 55,  1.5f, 1, "SLA", "R", "R");
        slaRP1.bullpenRole = "MR"; t.roster.Add(slaRP1);

        Player slaRP2 = P(636, "Jake",  "Woodfords",     "RP", 29, 66, 0, 0, 36, 42, 42, 66, 52,  1.0f, 1, "SLA", "R", "R");
        slaRP2.bullpenRole = "MR"; t.roster.Add(slaRP2);

        // CATCHERS
        t.roster.Add(P(637, "Willson",  "Contrerass",    "C",  34, 76, 72, 74, 50, 62, 70, 0, 0, 17.0f, 1, "SLA", "R", "R"));
        t.roster.Add(P(638, "Pedro",    "Pages",         "C",  25, 65, 60, 62, 48, 58, 62, 0, 0,  1.0f, 1, "SLA", "R", "R"));

        // INFIELDERS
        t.roster.Add(P(639, "Paul",     "Goldschmidt",   "1B", 38, 78, 78, 78, 55, 65, 76, 0, 0, 26.0f, 1, "SLA", "R", "R"));
        t.roster.Add(P(640, "Nolan",    "Gorman",        "2B", 26, 78, 72, 82, 65, 65, 68, 0, 0,  1.0f, 2, "SLA", "L", "L"));
        t.roster.Add(P(641, "Masyn",    "Winns",         "SS", 23, 78, 74, 70, 75, 72, 76, 0, 0,  1.0f, 3, "SLA", "R", "R"));
        t.roster.Add(P(642, "Nolan",    "Arenados",      "3B", 35, 88, 82, 84, 65, 78, 86, 0, 0, 35.0f, 3, "SLA", "R", "R"));
        t.roster.Add(P(643, "Jose",     "Ferrers",       "2B", 28, 68, 65, 60, 65, 62, 66, 0, 0,  2.0f, 1, "SLA", "R", "R"));
        t.roster.Add(P(644, "Tommy",    "Edmons",        "SS", 29, 65, 63, 58, 62, 60, 65, 0, 0,  1.0f, 1, "SLA", "R", "R"));

        // OUTFIELDERS
        t.roster.Add(P(645, "Lars",     "Nootbaars",     "CF", 28, 78, 74, 72, 78, 72, 76, 0, 0,  3.0f, 3, "SLA", "L", "L"));
        t.roster.Add(P(646, "Brendan",  "Donovans",      "LF", 30, 76, 76, 65, 68, 68, 72, 0, 0,  8.0f, 3, "SLA", "L", "L"));
        t.roster.Add(P(647, "Jordan",   "Walkers",       "RF", 23, 80, 74, 80, 72, 70, 72, 0, 0,  1.0f, 3, "SLA", "R", "R"));
        t.roster.Add(P(648, "Dylan",    "Carletons",     "CF", 28, 68, 65, 62, 68, 62, 66, 0, 0,  2.0f, 1, "SLA", "R", "R"));

        // DH / BENCH
        t.roster.Add(P(649, "Alec",     "Burleson",      "DH", 26, 74, 70, 74, 62, 62, 66, 0, 0,  1.0f, 2, "SLA", "L", "L"));
        t.roster.Add(P(650, "Victor",   "Scotts",        "CF", 24, 68, 65, 62, 75, 62, 66, 0, 0,  1.0f, 1, "SLA", "R", "R"));
    }
}
