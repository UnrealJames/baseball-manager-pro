using UnityEngine;
using System.Collections.Generic;

public class RosterBuilder_ALEast : MonoBehaviour
{
       
           void Add(Team t, Player p)
    {
        if (p != null) t.roster.Add(p);
        else Debug.LogError("Null player skipped for " + t.abbreviation);
    }

       public void BuildAllRosters(List<Team> allTeams)
    {
        foreach (Team t in allTeams)
        {
            if (t.abbreviation == "NYA") { BuildNYA(t); Debug.Log("NYA built: " + t.roster.Count); }
            if (t.abbreviation == "BST") { BuildBST(t); Debug.Log("BST built: " + t.roster.Count); }
            if (t.abbreviation == "TRN") { BuildTRN(t); Debug.Log("TRN built: " + t.roster.Count); }
            if (t.abbreviation == "BLT") { BuildBLT(t); Debug.Log("BLT built: " + t.roster.Count); }
            if (t.abbreviation == "TBS") { BuildTBS(t); Debug.Log("TBS built: " + t.roster.Count); }
        }
    }

        Player P(int id, string first, string last, string pos,
             int age, int overall, int contact, int power,
             int speed, int arm, int fielding, int pitching,
             int stamina, float salary, int contractYears,
             string team, string throwArm = "R", string batHand = "R")
    {
        try
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
            p.bullpenRole   = "";
            p.confidence    = 50f;
            return p;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error creating player ID " + id +
                           " " + first + " " + last +
                           " — " + e.Message);
            return null;
        }
    }

        void BuildNYA(Team t)
    {
        t.roster = new List<Player>();
                Debug.Log("Starting NYA build - testing P() method...");
        Player testP = P(1, "Max", "Freid", "SP", 32, 92, 
                         0, 0, 58, 68, 68, 92, 86, 
                         28.0f, 5, "NYA", "L", "L");
        Debug.Log("Test player: " + (testP == null ? 
                  "NULL!" : testP.FullName()));

        Debug.Log("Building NYA roster...");

        // ROTATION
        t.roster.Add(P(1,  "Max",       "Freid",        "SP", 32, 92, 0, 0, 58, 68, 68, 92, 86, 28.0f, 5, "NYA", "L", "L"));
        t.roster.Add(P(2,  "Cam",       "Schlittlers",  "SP", 24, 78, 0, 0, 52, 60, 60, 78, 74,  1.0f, 1, "NYA", "R", "R"));
        t.roster.Add(P(3,  "Will",      "Warrens",      "SP", 26, 76, 0, 0, 50, 58, 58, 76, 72,  1.0f, 1, "NYA", "R", "R"));
        t.roster.Add(P(4,  "Ryan",      "Weathers",     "SP", 25, 74, 0, 0, 48, 55, 55, 74, 70,  2.0f, 1, "NYA", "L", "L"));
        t.roster.Add(P(5,  "Luis",      "Gill",         "SP", 27, 80, 0, 0, 52, 60, 60, 80, 76,  3.0f, 2, "NYA", "R", "R"));

        // BULLPEN
        Player nyaCL = P(6,  "Clay",   "Holmes",        "RP", 31, 86, 0, 0, 50, 62, 62, 86, 76,  8.0f, 2, "NYA", "R", "R");
        nyaCL.bullpenRole = "CL"; t.roster.Add(nyaCL);

        Player nyaSU = P(7,  "Tommy",  "Kahnle",        "RP", 36, 78, 0, 0, 48, 55, 55, 78, 68,  5.0f, 1, "NYA", "R", "R");
        nyaSU.bullpenRole = "SU"; t.roster.Add(nyaSU);

        Player nyaMR1 = P(8,  "Mark",  "Levines",       "RP", 30, 75, 0, 0, 45, 52, 52, 75, 65,  3.0f, 1, "NYA", "R", "R");
        nyaMR1.bullpenRole = "MR"; t.roster.Add(nyaMR1);

        Player nyaMR2 = P(9,  "Jake",  "Cousinss",      "RP", 30, 72, 0, 0, 42, 50, 50, 72, 62,  2.0f, 1, "NYA", "R", "R");
        nyaMR2.bullpenRole = "MR"; t.roster.Add(nyaMR2);

        Player nyaMR3 = P(10, "Amed",  "Rosarions",     "RP", 32, 70, 0, 0, 40, 48, 48, 70, 58,  3.0f, 1, "NYA", "R", "R");
        nyaMR3.bullpenRole = "MR"; t.roster.Add(nyaMR3);

        Player nyaRP1 = P(11, "Greg",  "Weisserts",     "RP", 30, 68, 0, 0, 38, 45, 45, 68, 55,  1.0f, 1, "NYA", "R", "R");
        nyaRP1.bullpenRole = "MR"; t.roster.Add(nyaRP1);

        Player nyaRP2 = P(12, "Ian",   "Hamiltons",     "RP", 30, 65, 0, 0, 36, 42, 42, 65, 52,  1.0f, 1, "NYA", "R", "R");
        nyaRP2.bullpenRole = "MR"; t.roster.Add(nyaRP2);

        // CATCHERS
        t.roster.Add(P(13, "Austin",    "Wellmans",      "C",  26, 78, 72, 70, 55, 68, 78, 0, 0,  2.0f, 2, "NYA", "R", "R"));
        t.roster.Add(P(14, "Jose",      "Trevinos",      "C",  32, 65, 60, 55, 48, 62, 72, 0, 0,  1.0f, 1, "NYA", "R", "R"));

        // INFIELDERS
        t.roster.Add(P(15, "Ben",       "Rices",         "1B", 25, 78, 75, 80, 55, 65, 72, 0, 0,  1.0f, 2, "NYA", "L", "L"));
        t.roster.Add(P(16, "Jazz",      "Chisholme",     "2B", 28, 88, 80, 85, 85, 78, 80, 0, 0, 10.0f, 3, "NYA", "L", "S"));
        t.roster.Add(P(17, "Jose",      "Caballeros",    "SS", 29, 72, 68, 62, 72, 70, 75, 0, 0,  2.0f, 2, "NYA", "R", "R"));
        t.roster.Add(P(18, "Ryan",      "McMahons",      "3B", 32, 80, 75, 80, 68, 75, 80, 0, 0, 12.0f, 4, "NYA", "L", "L"));
        t.roster.Add(P(19, "Paul",      "Goldschmidts",  "1B", 38, 78, 78, 80, 55, 68, 80, 0, 0, 18.0f, 1, "NYA", "R", "R"));
        t.roster.Add(P(20, "Oswaldo",   "Cabreras",      "2B", 26, 70, 68, 65, 68, 65, 72, 0, 0,  1.0f, 1, "NYA", "R", "S"));

        // OUTFIELDERS
        t.roster.Add(P(21, "Aaron",     "Judkins",       "RF", 34, 97, 85, 99, 70, 88, 85, 0, 0, 40.0f, 3, "NYA", "R", "R"));
        t.roster.Add(P(22, "Cody",      "Bellingers",    "LF", 30, 82, 80, 80, 80, 78, 82, 0, 0, 17.0f, 1, "NYA", "L", "L"));
        t.roster.Add(P(23, "Trent",     "Grishams",      "CF", 30, 75, 72, 68, 80, 75, 82, 0, 0,  5.0f, 2, "NYA", "L", "L"));
        t.roster.Add(P(24, "Giancarlo", "Stantone",      "DH", 35, 85, 72, 95, 52, 65, 70, 0, 0, 32.0f, 1, "NYA", "R", "R"));

        // BENCH
        t.roster.Add(P(25, "Carlos",    "Rodon",         "SP", 33, 82, 0,  0, 50, 60, 60, 82, 78, 22.0f, 2, "NYA", "L", "L"));
        t.roster.Add(P(26, "Anthony",   "Volpes",        "SS", 24, 76, 72, 65, 78, 70, 80, 0,  0,  1.0f, 2, "NYA", "R", "R"));
    
        Debug.Log("NYA final count: " + t.roster.Count);
        foreach (Player p in t.roster)
            Debug.Log("  " + p.id + " " + p.FullName() +
                      " | " + p.position);


    }

    void BuildBST(Team t)
    {
        t.roster = new List<Player>();

        // ROTATION
        t.roster.Add(P(27, "Garrett",   "Crochet",       "SP", 26, 90, 0, 0, 60, 68, 68, 90, 84, 20.0f, 4, "BST", "L", "L"));
        t.roster.Add(P(28, "Ranger",    "Suareez",       "SP", 29, 85, 0, 0, 55, 62, 62, 85, 80, 17.0f, 4, "BST", "L", "L"));
        t.roster.Add(P(29, "Sonny",     "Grays",         "SP", 37, 82, 0, 0, 52, 60, 60, 82, 78, 15.0f, 2, "BST", "R", "R"));
        t.roster.Add(P(30, "Brayan",    "Bellos",        "SP", 25, 78, 0, 0, 50, 58, 58, 78, 74,  3.0f, 2, "BST", "R", "R"));
        t.roster.Add(P(31, "Johan",     "Oviedos",       "SP", 27, 74, 0, 0, 48, 55, 55, 74, 70,  4.0f, 2, "BST", "R", "R"));

        // BULLPEN
        Player bstCL = P(32, "Kenley",  "Janssen",       "RP", 38, 82, 0, 0, 48, 60, 60, 82, 70,  5.0f, 1, "BST", "R", "R");
        bstCL.bullpenRole = "CL"; t.roster.Add(bstCL);

        Player bstSU = P(33, "Kutter",  "Crawfords",     "RP", 29, 78, 0, 0, 50, 58, 58, 78, 68,  2.0f, 2, "BST", "R", "R");
        bstSU.bullpenRole = "SU"; t.roster.Add(bstSU);

        Player bstMR1 = P(34, "Justin", "Slatens",       "RP", 28, 75, 0, 0, 45, 52, 52, 75, 62,  1.0f, 1, "BST", "L", "L");
        bstMR1.bullpenRole = "MR"; t.roster.Add(bstMR1);

        Player bstMR2 = P(35, "Greg",   "Bragmans",      "RP", 32, 72, 0, 0, 42, 50, 50, 72, 60,  2.0f, 1, "BST", "R", "R");
        bstMR2.bullpenRole = "MR"; t.roster.Add(bstMR2);

        Player bstMR3 = P(36, "Patrick","Sandovals",     "RP", 29, 70, 0, 0, 40, 48, 48, 70, 58,  3.0f, 1, "BST", "L", "L");
        bstMR3.bullpenRole = "MR"; t.roster.Add(bstMR3);

        Player bstRP1 = P(37, "Zack",   "Kellys",        "RP", 35, 68, 0, 0, 38, 45, 45, 68, 55,  1.5f, 1, "BST", "R", "R");
        bstRP1.bullpenRole = "MR"; t.roster.Add(bstRP1);

        Player bstRP2 = P(38, "Chris",  "Martins",       "RP", 38, 65, 0, 0, 36, 42, 42, 65, 52,  2.0f, 1, "BST", "R", "R");
        bstRP2.bullpenRole = "MR"; t.roster.Add(bstRP2);

        // CATCHERS
        t.roster.Add(P(39, "Carlos",    "Narvaezz",      "C",  30, 70, 65, 60, 52, 62, 72, 0, 0,  3.0f, 2, "BST", "L", "L"));
        t.roster.Add(P(40, "Connor",    "Wongs",         "C",  33, 68, 65, 58, 55, 60, 70, 0, 0,  4.0f, 1, "BST", "R", "R"));

        // INFIELDERS
        t.roster.Add(P(41, "Willson",   "Contreras",     "1B", 34, 82, 78, 80, 52, 65, 72, 0, 0, 22.0f, 3, "BST", "R", "R"));
        t.roster.Add(P(42, "Marcelo",   "Mayers",        "2B", 23, 78, 75, 70, 70, 70, 75, 0, 0,  1.0f, 2, "BST", "L", "L"));
        t.roster.Add(P(43, "Trevor",    "Storys",        "SS", 33, 78, 75, 75, 75, 72, 78, 0, 0, 15.0f, 2, "BST", "R", "R"));
        t.roster.Add(P(44, "Caleb",     "Durbins",       "3B", 25, 72, 70, 68, 68, 68, 72, 0, 0,  1.0f, 2, "BST", "R", "R"));
        t.roster.Add(P(45, "David",     "Hamiltons",     "2B", 27, 68, 68, 60, 72, 65, 70, 0, 0,  1.0f, 1, "BST", "L", "L"));
        t.roster.Add(P(46, "Enrique",   "Hernandez",     "SS", 33, 65, 62, 58, 65, 62, 68, 0, 0,  5.0f, 1, "BST", "R", "R"));

        // OUTFIELDERS
        t.roster.Add(P(47, "Roman",     "Anthonys",      "LF", 21, 82, 82, 75, 75, 72, 78, 0, 0,  1.0f, 2, "BST", "L", "L"));
        t.roster.Add(P(48, "Jarren",    "Durans",        "CF", 28, 80, 78, 72, 82, 78, 80, 0, 0,  5.0f, 3, "BST", "L", "L"));
        t.roster.Add(P(49, "Wilyer",    "Abreus",        "RF", 26, 75, 72, 70, 72, 70, 75, 0, 0,  1.0f, 2, "BST", "R", "R"));
        t.roster.Add(P(50, "Ceddanne",  "Rafaelas",      "CF", 25, 72, 68, 65, 80, 72, 80, 0, 0,  1.0f, 2, "BST", "R", "R"));

        // DH / BENCH
        t.roster.Add(P(51, "Rafael",    "Devers",        "DH", 29, 90, 85, 88, 68, 70, 75, 0, 0, 18.0f, 7, "BST", "R", "L"));
        t.roster.Add(P(52, "Bobby",     "Dalbecs",       "1B", 29, 65, 60, 70, 52, 58, 65, 0, 0,  1.0f, 1, "BST", "R", "R"));
    }

    void BuildTRN(Team t)
    {
        t.roster = new List<Player>();

        // ROTATION
        t.roster.Add(P(53, "Kevin",     "Gausmans",      "SP", 33, 88, 0, 0, 55, 65, 65, 88, 82, 22.0f, 3, "TRN", "R", "R"));
        t.roster.Add(P(54, "Dylan",     "Ceases",        "SP", 30, 86, 0, 0, 55, 65, 65, 86, 80, 18.0f, 4, "TRN", "R", "R"));
        t.roster.Add(P(55, "Eric",      "Lauers",        "SP", 29, 76, 0, 0, 50, 58, 58, 76, 72,  5.0f, 2, "TRN", "L", "L"));
        t.roster.Add(P(56, "Cody",      "Ponces",        "SP", 29, 72, 0, 0, 48, 55, 55, 72, 68,  3.0f, 1, "TRN", "R", "R"));
        t.roster.Add(P(57, "Max",       "Schertzers",    "SP", 41, 70, 0, 0, 45, 52, 52, 70, 66, 20.0f, 1, "TRN", "R", "R"));

        // BULLPEN
        Player trnCL = P(58, "Jordan",  "Romanos",       "RP", 31, 84, 0, 0, 50, 60, 60, 84, 74,  9.0f, 2, "TRN", "R", "R");
        trnCL.bullpenRole = "CL"; t.roster.Add(trnCL);

        Player trnSU = P(59, "Tim",     "Mayports",      "RP", 29, 78, 0, 0, 48, 55, 55, 78, 68,  4.0f, 2, "TRN", "R", "R");
        trnSU.bullpenRole = "SU"; t.roster.Add(trnSU);

        Player trnMR1 = P(60, "Yimi",   "Garcias",       "RP", 34, 74, 0, 0, 44, 50, 50, 74, 62,  2.5f, 1, "TRN", "R", "R");
        trnMR1.bullpenRole = "MR"; t.roster.Add(trnMR1);

        Player trnMR2 = P(61, "Nate",   "Pearces",       "RP", 27, 72, 0, 0, 42, 48, 48, 72, 60,  1.5f, 1, "TRN", "L", "L");
        trnMR2.bullpenRole = "MR"; t.roster.Add(trnMR2);

        Player trnMR3 = P(62, "Erik",   "Swanbergs",     "RP", 28, 70, 0, 0, 40, 46, 46, 70, 58,  2.0f, 1, "TRN", "R", "R");
        trnMR3.bullpenRole = "MR"; t.roster.Add(trnMR3);

        Player trnRP1 = P(63, "Trevor", "Richards",      "RP", 31, 68, 0, 0, 38, 44, 44, 68, 55,  1.5f, 1, "TRN", "R", "R");
        trnRP1.bullpenRole = "MR"; t.roster.Add(trnRP1);

        Player trnRP2 = P(64, "Adam",   "Cimbers",       "RP", 33, 65, 0, 0, 36, 42, 42, 65, 52,  2.0f, 1, "TRN", "R", "R");
        trnRP2.bullpenRole = "MR"; t.roster.Add(trnRP2);

        // CATCHERS
        t.roster.Add(P(65, "Alejandro", "Kirks",         "C",  28, 80, 78, 72, 48, 65, 78, 0, 0,  5.0f, 3, "TRN", "R", "R"));
        t.roster.Add(P(66, "Gabriel",   "Morenos",       "C",  27, 65, 62, 55, 48, 60, 70, 0, 0,  1.0f, 1, "TRN", "R", "R"));

        // INFIELDERS
        t.roster.Add(P(67, "Vladimir",  "Guerrerio",     "1B", 27, 94, 88, 95, 58, 72, 80, 0, 0, 20.0f, 3, "TRN", "R", "R"));
        t.roster.Add(P(68, "Ernie",     "Clements",      "2B", 29, 68, 68, 60, 68, 65, 70, 0, 0,  1.0f, 1, "TRN", "R", "R"));
        t.roster.Add(P(69, "Andres",    "Gimenezz",      "SS", 26, 78, 75, 68, 72, 72, 78, 0, 0,  5.0f, 2, "TRN", "L", "S"));
        t.roster.Add(P(70, "Kazuma",    "Okamotos",      "3B", 25, 74, 70, 72, 65, 68, 72, 0, 0,  1.0f, 2, "TRN", "R", "R"));
        t.roster.Add(P(71, "Addison",   "Bargers",       "RF", 25, 76, 72, 75, 68, 70, 74, 0, 0,  1.0f, 2, "TRN", "R", "R"));
        t.roster.Add(P(72, "Cavan",     "Biggios",       "2B", 30, 68, 65, 60, 65, 65, 70, 0, 0,  2.5f, 1, "TRN", "L", "S"));

        // OUTFIELDERS
        t.roster.Add(P(73, "George",    "Springers",     "DH", 37, 82, 78, 80, 75, 78, 80, 0, 0, 22.0f, 2, "TRN", "R", "R"));
        t.roster.Add(P(74, "Daulton",   "Varshos",       "CF", 28, 80, 75, 75, 80, 78, 82, 0, 0,  7.0f, 2, "TRN", "L", "L"));
        t.roster.Add(P(75, "Jesus",     "Sanchezz",      "LF", 28, 76, 72, 78, 70, 72, 75, 0, 0,  3.0f, 2, "TRN", "R", "R"));
        t.roster.Add(P(76, "Nathan",    "Lukess",        "LF", 26, 68, 65, 62, 68, 65, 70, 0, 0,  1.0f, 1, "TRN", "L", "L"));

        // BENCH
        t.roster.Add(P(77, "Davis",     "Schneiders",    "RF", 26, 70, 68, 65, 70, 65, 72, 0, 0,  1.0f, 1, "TRN", "R", "R"));
        t.roster.Add(P(78, "Whit",      "Merrifields",   "RF", 36, 68, 68, 60, 75, 68, 72, 0, 0,  3.0f, 1, "TRN", "R", "R"));
    }

    void BuildBLT(Team t)
    {
        t.roster = new List<Player>();

        // ROTATION
        t.roster.Add(P(79,  "Trevor",   "Rogerss",       "SP", 29, 82, 0, 0, 55, 62, 62, 82, 78,  8.0f, 3, "BLT", "L", "L"));
        t.roster.Add(P(80,  "Kyle",     "Bradishs",      "SP", 27, 80, 0, 0, 52, 60, 60, 80, 76,  5.0f, 3, "BLT", "R", "R"));
        t.roster.Add(P(81,  "Shane",    "Bazz",          "SP", 25, 78, 0, 0, 50, 58, 58, 78, 74,  2.0f, 2, "BLT", "R", "R"));
        t.roster.Add(P(82,  "Chris",    "Bassitts",      "SP", 37, 76, 0, 0, 48, 55, 55, 76, 72, 19.0f, 2, "BLT", "R", "R"));
        t.roster.Add(P(83,  "Zach",     "Eflins",        "SP", 31, 74, 0, 0, 45, 52, 52, 74, 70,  8.0f, 2, "BLT", "R", "R"));

        // BULLPEN
        Player bltCL = P(84, "Felix",   "Bautistas",     "RP", 29, 88, 0, 0, 52, 62, 62, 88, 78, 11.0f, 3, "BLT", "R", "R");
        bltCL.bullpenRole = "CL"; t.roster.Add(bltCL);

        Player bltSU = P(85, "Danny",   "Couteses",      "RP", 29, 80, 0, 0, 50, 58, 58, 80, 70,  4.0f, 2, "BLT", "R", "R");
        bltSU.bullpenRole = "SU"; t.roster.Add(bltSU);

        Player bltMR1 = P(86, "Keegan", "Akinss",        "RP", 29, 76, 0, 0, 45, 52, 52, 76, 64,  2.0f, 2, "BLT", "R", "R");
        bltMR1.bullpenRole = "MR"; t.roster.Add(bltMR1);

        Player bltMR2 = P(87, "Dillon", "Tates",         "RP", 30, 72, 0, 0, 42, 48, 48, 72, 60,  2.5f, 1, "BLT", "R", "R");
        bltMR2.bullpenRole = "MR"; t.roster.Add(bltMR2);

        Player bltMR3 = P(88, "Bryan",  "Bakers",        "RP", 28, 70, 0, 0, 40, 46, 46, 70, 58,  1.5f, 1, "BLT", "R", "R");
        bltMR3.bullpenRole = "MR"; t.roster.Add(bltMR3);

        Player bltRP1 = P(89, "Nick",   "Vespis",        "RP", 29, 68, 0, 0, 38, 44, 44, 68, 55,  1.0f, 1, "BLT", "L", "L");
        bltRP1.bullpenRole = "MR"; t.roster.Add(bltRP1);

        Player bltRP2 = P(90, "Yennier","Canos",         "RP", 30, 66, 0, 0, 36, 42, 42, 66, 52,  1.0f, 1, "BLT", "R", "R");
        bltRP2.bullpenRole = "MR"; t.roster.Add(bltRP2);

        // CATCHERS
        t.roster.Add(P(91,  "Adley",    "Rutschmans",    "C",  28, 90, 85, 78, 62, 75, 85, 0, 0,  9.0f, 4, "BLT", "S", "S"));
        t.roster.Add(P(92,  "Samuel",   "Basallos",      "C",  21, 72, 68, 75, 52, 62, 72, 0, 0,  1.0f, 2, "BLT", "R", "R"));

        // INFIELDERS
        t.roster.Add(P(93,  "Pete",     "Alonsos",       "1B", 31, 90, 82, 92, 58, 68, 78, 0, 0, 20.0f, 2, "BLT", "R", "R"));
        t.roster.Add(P(94,  "Blaze",    "Alexanders",    "2B", 27, 68, 65, 60, 68, 65, 70, 0, 0,  1.0f, 1, "BLT", "R", "R"));
        t.roster.Add(P(95,  "Gunnar",   "Hendersons",    "SS", 25, 92, 85, 88, 78, 82, 85, 0, 0,  4.0f, 4, "BLT", "L", "L"));
        t.roster.Add(P(96,  "Coby",     "Mayos",         "3B", 23, 76, 70, 78, 62, 68, 72, 0, 0,  1.0f, 2, "BLT", "R", "R"));
        t.roster.Add(P(97,  "Jackson",  "Hollidays",     "2B", 22, 78, 75, 72, 72, 70, 75, 0, 0,  1.0f, 3, "BLT", "L", "L"));
        t.roster.Add(P(98,  "Jordan",   "Westburgs",     "3B", 27, 74, 70, 70, 68, 68, 72, 0, 0,  1.0f, 2, "BLT", "R", "R"));

        // OUTFIELDERS
        t.roster.Add(P(99,  "Colton",   "Cowsers",       "CF", 25, 78, 75, 72, 78, 75, 80, 0, 0,  1.0f, 2, "BLT", "L", "L"));
        t.roster.Add(P(100, "Tyler",    "ONeills",       "RF", 29, 80, 75, 82, 75, 78, 80, 0, 0, 12.0f, 2, "BLT", "R", "R"));
        t.roster.Add(P(101, "Taylor",   "Wards",         "LF", 32, 78, 75, 78, 70, 72, 75, 0, 0,  9.0f, 2, "BLT", "R", "R"));
        t.roster.Add(P(102, "Cedric",   "Mullinss",      "LF", 32, 72, 70, 65, 78, 72, 76, 0, 0,  6.0f, 2, "BLT", "S", "S"));

        // DH / BENCH
        t.roster.Add(P(103, "Ryan",     "Mountcastles",  "DH", 28, 80, 78, 80, 62, 68, 74, 0, 0,  3.0f, 3, "BLT", "R", "R"));
        t.roster.Add(P(104, "Heston",   "Kjerstads",     "LF", 26, 70, 68, 68, 65, 65, 70, 0, 0,  1.0f, 1, "BLT", "L", "L"));
    }

    void BuildTBS(Team t)
    {
        t.roster = new List<Player>();

        // ROTATION
        t.roster.Add(P(105, "Shane",    "McLanahan",     "SP", 29, 88, 0, 0, 58, 65, 65, 88, 82, 10.0f, 3, "TBS", "L", "L"));
        t.roster.Add(P(106, "Drew",     "Rasmussens",    "SP", 29, 84, 0, 0, 55, 62, 62, 84, 80,  7.0f, 3, "TBS", "R", "R"));
        t.roster.Add(P(107, "Ryan",     "Pepiots",       "SP", 27, 78, 0, 0, 50, 58, 58, 78, 74,  3.0f, 2, "TBS", "R", "R"));
        t.roster.Add(P(108, "Nick",     "Martinezz",     "SP", 36, 76, 0, 0, 48, 55, 55, 76, 72,  9.0f, 2, "TBS", "R", "R"));
        t.roster.Add(P(109, "Steven",   "Matzs",         "SP", 35, 72, 0, 0, 45, 52, 52, 72, 68,  6.0f, 1, "TBS", "L", "L"));

        // BULLPEN
        Player tbsCL = P(110, "Pete",   "Fairbankss",    "RP", 30, 86, 0, 0, 50, 62, 62, 86, 76,  5.0f, 2, "TBS", "R", "R");
        tbsCL.bullpenRole = "CL"; t.roster.Add(tbsCL);

        Player tbsSU = P(111, "Jason",  "Adamss",        "RP", 37, 78, 0, 0, 48, 55, 55, 78, 68,  3.0f, 1, "TBS", "R", "R");
        tbsSU.bullpenRole = "SU"; t.roster.Add(tbsSU);

        Player tbsMR1 = P(112, "Garrett","Cleavingers",  "RP", 30, 75, 0, 0, 44, 50, 50, 75, 62,  2.0f, 2, "TBS", "L", "L");
        tbsMR1.bullpenRole = "MR"; t.roster.Add(tbsMR1);

        Player tbsMR2 = P(113, "Kevin", "Kelleys",       "RP", 34, 72, 0, 0, 42, 48, 48, 72, 60,  2.5f, 1, "TBS", "R", "R");
        tbsMR2.bullpenRole = "MR"; t.roster.Add(tbsMR2);

        Player tbsMR3 = P(114, "Colin", "Poches",        "RP", 32, 70, 0, 0, 40, 46, 46, 70, 58,  2.0f, 1, "TBS", "L", "L");
        tbsMR3.bullpenRole = "MR"; t.roster.Add(tbsMR3);

        Player tbsRP1 = P(115, "Shawn", "Armstrongs",    "RP", 35, 68, 0, 0, 38, 44, 44, 68, 55,  1.5f, 1, "TBS", "R", "R");
        tbsRP1.bullpenRole = "MR"; t.roster.Add(tbsRP1);

        Player tbsRP2 = P(116, "Josh",  "Flemings",      "RP", 29, 65, 0, 0, 36, 42, 42, 65, 52,  1.0f, 1, "TBS", "L", "L");
        tbsRP2.bullpenRole = "MR"; t.roster.Add(tbsRP2);

        // CATCHERS
        t.roster.Add(P(117, "Nick",     "Fortess",       "C",  30, 68, 65, 60, 52, 60, 70, 0, 0,  2.0f, 1, "TBS", "R", "R"));
        t.roster.Add(P(118, "Hunter",   "Feduccias",     "C",  26, 62, 60, 55, 50, 58, 65, 0, 0,  1.0f, 1, "TBS", "R", "R"));

        // INFIELDERS
        t.roster.Add(P(119, "Jonathan", "Arandas",       "1B", 27, 82, 80, 78, 60, 68, 75, 0, 0,  3.0f, 3, "TBS", "L", "L"));
        t.roster.Add(P(120, "Ben",      "Williamsons",   "2B", 24, 74, 70, 70, 68, 68, 72, 0, 0,  1.0f, 2, "TBS", "R", "R"));
        t.roster.Add(P(121, "Carson",   "Williamss",     "SS", 23, 72, 68, 68, 70, 68, 72, 0, 0,  1.0f, 2, "TBS", "R", "R"));
        t.roster.Add(P(122, "Junior",   "Camineros",     "3B", 22, 84, 78, 85, 65, 70, 74, 0, 0,  1.0f, 3, "TBS", "R", "R"));
        t.roster.Add(P(123, "Yandy",    "Diazz",         "DH", 34, 80, 82, 72, 60, 65, 70, 0, 0,  8.0f, 2, "TBS", "R", "R"));
        t.roster.Add(P(124, "Richie",   "Palacioss",     "2B", 29, 68, 66, 60, 68, 65, 68, 0, 0,  1.0f, 1, "TBS", "L", "L"));

        // OUTFIELDERS
        t.roster.Add(P(125, "Cedric",   "Mullinss",      "CF", 32, 75, 72, 68, 80, 75, 78, 0, 0,  6.0f, 2, "TBS", "S", "S"));
        t.roster.Add(P(126, "Jonny",    "DeLucas",       "RF", 27, 72, 68, 70, 72, 68, 72, 0, 0,  1.0f, 2, "TBS", "R", "R"));
        t.roster.Add(P(127, "Chandler", "Simpsons",      "LF", 24, 68, 65, 60, 78, 68, 70, 0, 0,  1.0f, 1, "TBS", "R", "R"));
        t.roster.Add(P(128, "Jake",     "Fraleys",       "LF", 30, 68, 65, 65, 70, 65, 68, 0, 0,  2.0f, 1, "TBS", "L", "L"));

        // BENCH
        t.roster.Add(P(129, "Ryan",     "Vilades",       "LF", 28, 68, 65, 62, 68, 62, 68, 0, 0,  1.0f, 1, "TBS", "R", "R"));
        t.roster.Add(P(130, "Jose",     "Siris",         "CF", 30, 65, 58, 62, 82, 68, 70, 0, 0,  1.0f, 1, "TBS", "R", "R"));
    }
}
