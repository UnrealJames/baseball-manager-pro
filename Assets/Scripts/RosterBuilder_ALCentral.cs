using UnityEngine;
using System.Collections.Generic;

public class RosterBuilder_ALCentral : MonoBehaviour
{
    public void BuildAllRosters(List<Team> allTeams)
    {
        foreach (Team t in allTeams)
        {
            if (t.abbreviation == "CHH") BuildCHH(t);
            if (t.abbreviation == "CLN") BuildCLN(t);
            if (t.abbreviation == "DTE") BuildDTE(t);
            if (t.abbreviation == "KCP") BuildKCP(t);
            if (t.abbreviation == "MNV") BuildMNV(t);
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
    // CHICAGO HOUNDS — 2026 White Sox
    // -------------------------------------------------------
    void BuildCHH(Team t)
    {
        t.roster = new List<Player>();

        // ROTATION
        t.roster.Add(P(131, "Chris",    "Salzburg",         "SP", 37, 82, 0, 0, 52, 60, 60, 82, 78, 20.0f, 2, "CHH", "L", "L"));
        t.roster.Add(P(132, "Garrett",  "Cohens",       "SP", 26, 80, 0, 0, 55, 62, 62, 80, 76,  5.0f, 2, "CHH", "L", "L"));
        t.roster.Add(P(133, "Erick",    "Freddrick",        "SP", 27, 74, 0, 0, 48, 55, 55, 74, 70,  3.0f, 2, "CHH", "R", "R"));
        t.roster.Add(P(134, "Jonathan", "Carellibino",        "SP", 26, 72, 0, 0, 46, 52, 52, 72, 68,  1.0f, 1, "CHH", "R", "R"));
        t.roster.Add(P(135, "Drew",     "Lorenzsky",       "SP", 25, 70, 0, 0, 44, 50, 50, 70, 66,  1.0f, 1, "CHH", "L", "L"));

        // BULLPEN
        Player chhCL = P(136, "Michael","Kopechs",       "RP", 29, 82, 0, 0, 52, 60, 60, 82, 72,  6.0f, 2, "CHH", "R", "R");
        chhCL.bullpenRole = "CL"; t.roster.Add(chhCL);

        Player chhSU = P(137, "Keynan", "Middletons",    "RP", 30, 76, 0, 0, 48, 55, 55, 76, 66,  3.0f, 1, "CHH", "R", "R");
        chhSU.bullpenRole = "SU"; t.roster.Add(chhSU);

        Player chhMR1 = P(138, "Gregory","Santoss",      "RP", 29, 72, 0, 0, 44, 50, 50, 72, 62,  2.0f, 1, "CHH", "R", "R");
        chhMR1.bullpenRole = "MR"; t.roster.Add(chhMR1);

        Player chhMR2 = P(139, "Tanner","Banks",         "RP", 29, 70, 0, 0, 42, 48, 48, 70, 60,  1.5f, 1, "CHH", "L", "L");
        chhMR2.bullpenRole = "MR"; t.roster.Add(chhMR2);

        Player chhMR3 = P(140, "Joe",   "Kellys",        "RP", 37, 68, 0, 0, 40, 46, 46, 68, 58,  3.0f, 1, "CHH", "R", "R");
        chhMR3.bullpenRole = "MR"; t.roster.Add(chhMR3);

        Player chhRP1 = P(141, "Davis", "Martins",       "RP", 28, 66, 0, 0, 38, 44, 44, 66, 55,  1.0f, 1, "CHH", "R", "R");
        chhRP1.bullpenRole = "MR"; t.roster.Add(chhRP1);

        Player chhRP2 = P(142, "Wes",   "Benjamins",     "RP", 28, 64, 0, 0, 36, 42, 42, 64, 52,  1.0f, 1, "CHH", "R", "R");
        chhRP2.bullpenRole = "MR"; t.roster.Add(chhRP2);

        // CATCHERS
        t.roster.Add(P(143, "Martin",   "Maldonados",    "C",  37, 65, 60, 55, 45, 60, 70, 0, 0,  3.0f, 1, "CHH", "R", "R"));
        t.roster.Add(P(144, "Korey",    "Lees",          "C",  25, 62, 58, 55, 48, 58, 65, 0, 0,  1.0f, 1, "CHH", "R", "R"));

        // INFIELDERS
        t.roster.Add(P(145, "Andrew",   "Vaughns",       "1B", 27, 78, 75, 80, 55, 65, 72, 0, 0,  5.0f, 3, "CHH", "R", "R"));
        t.roster.Add(P(146, "Lenyn",    "Sosas",         "2B", 24, 68, 65, 62, 65, 62, 68, 0, 0,  1.0f, 2, "CHH", "R", "R"));
        t.roster.Add(P(147, "Tim",      "Andersons",     "SS", 31, 72, 72, 65, 70, 68, 72, 0, 0,  12.0f,2, "CHH", "R", "R"));
        t.roster.Add(P(148, "Jake",     "Burleson",      "3B", 24, 70, 68, 68, 62, 65, 68, 0, 0,  1.0f, 2, "CHH", "R", "R"));
        t.roster.Add(P(149, "Hanser",   "Albertos",      "2B", 32, 65, 65, 58, 62, 62, 68, 0, 0,  2.0f, 1, "CHH", "R", "R"));
        t.roster.Add(P(150, "Elvis",    "Andrus",        "SS", 36, 65, 65, 55, 65, 65, 70, 0, 0,  3.0f, 1, "CHH", "R", "R"));

        // OUTFIELDERS
        t.roster.Add(P(151, "Eloy",     "Jimenezz",      "LF", 28, 78, 75, 82, 55, 65, 68, 0, 0,  14.0f,3, "CHH", "R", "R"));
        t.roster.Add(P(152, "Luis",     "Robertss",      "CF", 27, 80, 75, 78, 82, 78, 82, 0, 0,  12.0f,3, "CHH", "R", "R"));
        t.roster.Add(P(153, "Gavin",    "Sheets",        "RF", 28, 70, 68, 72, 58, 62, 68, 0, 0,  1.0f, 2, "CHH", "L", "L"));
        t.roster.Add(P(154, "Oscar",    "Colases",       "RF", 24, 72, 68, 70, 70, 68, 72, 0, 0,  1.0f, 2, "CHH", "R", "R"));

        // DH / BENCH
        t.roster.Add(P(155, "Yasmani",  "Grandalss",     "DH", 36, 72, 68, 72, 45, 58, 65, 0, 0,  5.0f, 1, "CHH", "R", "R"));
        t.roster.Add(P(156, "Zach",     "Remillards",    "2B", 29, 62, 62, 55, 62, 58, 65, 0, 0,  1.0f, 1, "CHH", "R", "R"));
    }

    // -------------------------------------------------------
    // CLEVELAND NAVIGATORS — 2026 Guardians
    // -------------------------------------------------------
    void BuildCLN(Team t)
    {
        t.roster = new List<Player>();

        // ROTATION
        t.roster.Add(P(157, "Tanner",   "Bibees",        "SP", 27, 84, 0, 0, 55, 62, 62, 84, 80,  5.0f, 3, "CLN", "R", "R"));
        t.roster.Add(P(158, "Gavin",    "Williamss",     "SP", 25, 80, 0, 0, 52, 60, 60, 80, 76,  3.0f, 2, "CLN", "R", "R"));
        t.roster.Add(P(159, "Slade",    "Cecconis",      "SP", 26, 76, 0, 0, 50, 58, 58, 76, 72,  2.0f, 2, "CLN", "R", "R"));
        t.roster.Add(P(160, "Joey",     "Cantillos",     "SP", 26, 74, 0, 0, 48, 55, 55, 74, 70,  1.0f, 1, "CLN", "L", "L"));
        t.roster.Add(P(161, "Parker",   "Messicks",      "SP", 25, 72, 0, 0, 46, 52, 52, 72, 68,  1.0f, 1, "CLN", "L", "L"));

        // BULLPEN
        Player clnCL = P(162, "Emmanuel","Clasee",        "RP", 28, 88, 0, 0, 52, 62, 62, 88, 78,  7.0f, 3, "CLN", "R", "R");
        clnCL.bullpenRole = "CL"; t.roster.Add(clnCL);

        Player clnSU = P(163, "Trevor",  "Stephanns",    "RP", 31, 80, 0, 0, 50, 58, 58, 80, 70,  5.0f, 2, "CLN", "R", "R");
        clnSU.bullpenRole = "SU"; t.roster.Add(clnSU);

        Player clnMR1 = P(164, "Nick",  "Sandlins",      "RP", 28, 76, 0, 0, 45, 52, 52, 76, 64,  2.0f, 2, "CLN", "R", "R");
        clnMR1.bullpenRole = "MR"; t.roster.Add(clnMR1);

        Player clnMR2 = P(165, "James", "Karinchaks",    "RP", 30, 74, 0, 0, 42, 50, 50, 74, 62,  3.0f, 1, "CLN", "R", "R");
        clnMR2.bullpenRole = "MR"; t.roster.Add(clnMR2);

        Player clnMR3 = P(166, "Enyel", "DeLosSantoss",  "RP", 29, 70, 0, 0, 40, 46, 46, 70, 58,  1.5f, 1, "CLN", "R", "R");
        clnMR3.bullpenRole = "MR"; t.roster.Add(clnMR3);

        Player clnRP1 = P(167, "Tim",   "Herins",        "RP", 29, 68, 0, 0, 38, 44, 44, 68, 55,  1.0f, 1, "CLN", "R", "R");
        clnRP1.bullpenRole = "MR"; t.roster.Add(clnRP1);

        Player clnRP2 = P(168, "Logan", "Allens",        "RP", 26, 66, 0, 0, 36, 42, 42, 66, 52,  1.0f, 1, "CLN", "L", "L");
        clnRP2.bullpenRole = "MR"; t.roster.Add(clnRP2);

        // CATCHERS
        t.roster.Add(P(169, "Bo",       "Naylors",       "C",  25, 76, 72, 72, 55, 65, 75, 0, 0,  2.0f, 3, "CLN", "L", "L"));
        t.roster.Add(P(170, "Austin",   "Hedgess",       "C",  31, 65, 60, 55, 50, 62, 72, 0, 0,  3.0f, 1, "CLN", "R", "R"));

        // INFIELDERS
        t.roster.Add(P(171, "Kyle",     "Manzardos",     "1B", 25, 78, 75, 78, 55, 62, 72, 0, 0,  1.0f, 2, "CLN", "L", "L"));
        t.roster.Add(P(172, "Brayan",   "Rocchios",      "2B", 25, 72, 70, 65, 70, 68, 72, 0, 0,  1.0f, 2, "CLN", "S", "S"));
        t.roster.Add(P(173, "Gabriel",  "Ariass",        "SS", 25, 70, 68, 65, 68, 68, 72, 0, 0,  1.0f, 2, "CLN", "R", "R"));
        t.roster.Add(P(174, "Jose",     "Ramirezz",      "3B", 32, 95, 88, 90, 75, 80, 85, 0, 0, 16.0f, 4, "CLN", "S", "S"));
        t.roster.Add(P(175, "Chase",    "DeLauters",     "RF", 23, 76, 72, 75, 70, 70, 74, 0, 0,  1.0f, 2, "CLN", "L", "L"));
        t.roster.Add(P(176, "Tyler",    "Freemans",      "SS", 27, 68, 65, 60, 65, 65, 70, 0, 0,  1.0f, 1, "CLN", "R", "R"));

        // OUTFIELDERS
        t.roster.Add(P(177, "Steven",   "Kwans",         "CF", 27, 84, 85, 70, 82, 78, 84, 0, 0,  3.0f, 3, "CLN", "L", "L"));
        t.roster.Add(P(178, "Angel",    "Martinezz",     "LF", 23, 70, 68, 65, 68, 65, 70, 0, 0,  1.0f, 2, "CLN", "S", "S"));
        t.roster.Add(P(179, "Jhonkensy","Nolas",         "RF", 24, 74, 68, 80, 55, 62, 65, 0, 0,  1.0f, 2, "CLN", "R", "R"));
        t.roster.Add(P(180, "George",   "Valeras",       "LF", 25, 70, 68, 68, 70, 65, 70, 0, 0,  1.0f, 1, "CLN", "L", "L"));

        // DH / BENCH
        t.roster.Add(P(181, "Rhys",     "Hoskinss",      "DH", 33, 80, 78, 82, 55, 65, 70, 0, 0, 15.0f, 2, "CLN", "R", "R"));
        t.roster.Add(P(182, "Will",     "Bensons",       "LF", 24, 68, 65, 65, 68, 62, 68, 0, 0,  1.0f, 1, "CLN", "L", "L"));
    }

    // -------------------------------------------------------
    // DETROIT ENGINES — 2026 Tigers
    // -------------------------------------------------------
    void BuildDTE(Team t)
    {
        t.roster = new List<Player>();

        // ROTATION
        t.roster.Add(P(183, "Tarik",    "Skubals",       "SP", 28, 90, 0, 0, 58, 65, 65, 90, 84, 10.0f, 4, "DTE", "L", "L"));
        t.roster.Add(P(184, "Jack",     "Flahertys",     "SP", 30, 84, 0, 0, 55, 62, 62, 84, 80, 14.0f, 2, "DTE", "R", "R"));
        t.roster.Add(P(185, "Reese",    "Olsons",        "SP", 26, 78, 0, 0, 50, 58, 58, 78, 74,  2.0f, 2, "DTE", "R", "R"));
        t.roster.Add(P(186, "Casey",    "Mizes",         "SP", 28, 74, 0, 0, 48, 55, 55, 74, 70,  5.0f, 2, "DTE", "R", "R"));
        t.roster.Add(P(187, "Beau",     "Brieske",       "SP", 27, 72, 0, 0, 46, 52, 52, 72, 68,  1.0f, 1, "DTE", "R", "R"));

        // BULLPEN
        Player dteCL = P(188, "Jason",  "Foley",         "RP", 30, 84, 0, 0, 50, 60, 60, 84, 74,  4.0f, 2, "DTE", "R", "R");
        dteCL.bullpenRole = "CL"; t.roster.Add(dteCL);

        Player dteSU = P(189, "Will",   "Vests",         "RP", 29, 78, 0, 0, 48, 55, 55, 78, 68,  3.0f, 2, "DTE", "R", "R");
        dteSU.bullpenRole = "SU"; t.roster.Add(dteSU);

        Player dteMR1 = P(190, "Alex",  "Faedos",        "RP", 30, 74, 0, 0, 44, 50, 50, 74, 62,  2.0f, 1, "DTE", "R", "R");
        dteMR1.bullpenRole = "MR"; t.roster.Add(dteMR1);

        Player dteMR2 = P(191, "Tyler", "Holtons",       "RP", 28, 72, 0, 0, 42, 48, 48, 72, 60,  1.5f, 1, "DTE", "L", "L");
        dteMR2.bullpenRole = "MR"; t.roster.Add(dteMR2);

        Player dteMR3 = P(192, "Jose",  "Cisneros",      "RP", 32, 70, 0, 0, 40, 46, 46, 70, 58,  2.0f, 1, "DTE", "R", "R");
        dteMR3.bullpenRole = "MR"; t.roster.Add(dteMR3);

        Player dteRP1 = P(193, "Chasen","Shreves",       "RP", 30, 68, 0, 0, 38, 44, 44, 68, 55,  1.0f, 1, "DTE", "L", "L");
        dteRP1.bullpenRole = "MR"; t.roster.Add(dteRP1);

        Player dteRP2 = P(194, "Mason", "Englerts",      "RP", 29, 66, 0, 0, 36, 42, 42, 66, 52,  1.0f, 1, "DTE", "R", "R");
        dteRP2.bullpenRole = "MR"; t.roster.Add(dteRP2);

        // CATCHERS
        t.roster.Add(P(195, "Jake",     "Rogerss",       "C",  29, 75, 70, 72, 55, 65, 74, 0, 0,  3.0f, 2, "DTE", "R", "R"));
        t.roster.Add(P(196, "Eric",     "Haases",        "C",  32, 65, 60, 62, 48, 58, 65, 0, 0,  6.0f, 1, "DTE", "R", "R"));

        // INFIELDERS
        t.roster.Add(P(197, "Spencer",  "Torrkelson",    "1B", 26, 80, 75, 85, 55, 65, 72, 0, 0,  1.0f, 3, "DTE", "R", "R"));
        t.roster.Add(P(198, "Colt",     "Keiths",        "2B", 23, 76, 72, 70, 70, 68, 74, 0, 0,  1.0f, 2, "DTE", "L", "L"));
        t.roster.Add(P(199, "Trey",     "Sweeney",       "SS", 24, 72, 68, 68, 68, 68, 72, 0, 0,  1.0f, 2, "DTE", "R", "R"));
        t.roster.Add(P(200, "Matt",     "Vierlings",     "3B", 28, 74, 72, 70, 68, 68, 72, 0, 0,  2.0f, 2, "DTE", "R", "R"));
        t.roster.Add(P(201, "Gio",      "Urshelas",      "3B", 32, 72, 70, 68, 65, 68, 72, 0, 0,  5.0f, 1, "DTE", "R", "R"));
        t.roster.Add(P(202, "Andy",     "Ibaness",       "SS", 29, 68, 66, 60, 65, 65, 70, 0, 0,  3.0f, 1, "DTE", "R", "R"));

        // OUTFIELDERS
        t.roster.Add(P(203, "Riley",    "Greenes",       "CF", 24, 82, 78, 78, 80, 75, 80, 0, 0,  1.0f, 3, "DTE", "L", "L"));
        t.roster.Add(P(204, "Parker",   "Meadowss",      "LF", 29, 76, 72, 75, 72, 70, 74, 0, 0,  7.0f, 2, "DTE", "R", "R"));
        t.roster.Add(P(205, "Wenceel",  "Perezz",        "RF", 24, 72, 70, 65, 70, 65, 70, 0, 0,  1.0f, 2, "DTE", "S", "S"));
        t.roster.Add(P(206, "Justyn-Henry","Malloys",    "RF", 25, 68, 65, 65, 68, 62, 68, 0, 0,  1.0f, 1, "DTE", "R", "R"));

        // DH / BENCH
        t.roster.Add(P(207, "Mark",     "Canass",        "DH", 30, 78, 75, 80, 55, 65, 70, 0, 0,  5.0f, 2, "DTE", "R", "R"));
        t.roster.Add(P(208, "Zach",     "McKinstry",     "2B", 30, 65, 63, 60, 65, 62, 67, 0, 0,  2.0f, 1, "DTE", "L", "L"));
    }

    // -------------------------------------------------------
    // KANSAS CITY PIONEERS — 2026 Royals
    // -------------------------------------------------------
    void BuildKCP(Team t)
    {
        t.roster = new List<Player>();

        // ROTATION
        t.roster.Add(P(209, "Cole",     "Raganss",       "SP", 28, 88, 0, 0, 58, 65, 65, 88, 82, 12.0f, 4, "KCP", "L", "L"));
        t.roster.Add(P(210, "Michael",  "Wachas",        "SP", 35, 80, 0, 0, 52, 60, 60, 80, 76, 10.0f, 2, "KCP", "R", "R"));
        t.roster.Add(P(211, "Seth",     "Lugos",         "SP", 34, 78, 0, 0, 50, 58, 58, 78, 74,  9.0f, 2, "KCP", "R", "R"));
        t.roster.Add(P(212, "Kris",     "Bubics",        "SP", 28, 74, 0, 0, 48, 55, 55, 74, 70,  3.0f, 2, "KCP", "L", "L"));
        t.roster.Add(P(213, "Noah",     "Camerons",      "SP", 25, 72, 0, 0, 46, 52, 52, 72, 68,  1.0f, 1, "KCP", "L", "L"));

        // BULLPEN
        Player kcpCL = P(214, "James",  "McArthurs",     "RP", 30, 86, 0, 0, 52, 62, 62, 86, 76,  5.0f, 2, "KCP", "R", "R");
        kcpCL.bullpenRole = "CL"; t.roster.Add(kcpCL);

        Player kcpSU = P(215, "Carlos", "Hernandezz",    "RP", 27, 78, 0, 0, 48, 55, 55, 78, 68,  3.0f, 2, "KCP", "R", "R");
        kcpSU.bullpenRole = "SU"; t.roster.Add(kcpSU);

        Player kcpMR1 = P(216, "Scott", "Barlow",        "RP", 31, 74, 0, 0, 44, 50, 50, 74, 62,  4.0f, 1, "KCP", "R", "R");
        kcpMR1.bullpenRole = "MR"; t.roster.Add(kcpMR1);

        Player kcpMR2 = P(217, "Josh",  "Staumonts",     "RP", 30, 72, 0, 0, 42, 48, 48, 72, 60,  2.0f, 1, "KCP", "R", "R");
        kcpMR2.bullpenRole = "MR"; t.roster.Add(kcpMR2);

        Player kcpMR3 = P(218, "Taylor","Clarkes",       "RP", 29, 70, 0, 0, 40, 46, 46, 70, 58,  1.5f, 1, "KCP", "R", "R");
        kcpMR3.bullpenRole = "MR"; t.roster.Add(kcpMR3);

        Player kcpRP1 = P(219, "Bailey","Falters",       "RP", 27, 68, 0, 0, 38, 44, 44, 68, 55,  1.0f, 1, "KCP", "L", "L");
        kcpRP1.bullpenRole = "MR"; t.roster.Add(kcpRP1);

        Player kcpRP2 = P(220, "Dylan", "Colemans",      "RP", 28, 66, 0, 0, 36, 42, 42, 66, 52,  1.0f, 1, "KCP", "R", "R");
        kcpRP2.bullpenRole = "MR"; t.roster.Add(kcpRP2);

        // CATCHERS
        t.roster.Add(P(221, "Salvador", "Perezz",        "C",  36, 85, 78, 82, 48, 68, 78, 0, 0, 14.0f, 2, "KCP", "R", "R"));
        t.roster.Add(P(222, "Carter",   "Jensens",       "C",  23, 68, 65, 65, 52, 62, 68, 0, 0,  1.0f, 2, "KCP", "R", "R"));

        // INFIELDERS
        t.roster.Add(P(223, "Vinnie",   "Pasquantinos",  "1B", 27, 84, 82, 80, 55, 65, 74, 0, 0,  3.0f, 3, "KCP", "L", "L"));
        t.roster.Add(P(224, "Jonathan", "Indias",        "2B", 29, 78, 78, 70, 70, 68, 74, 0, 0,  5.0f, 2, "KCP", "R", "R"));
        t.roster.Add(P(225, "Bobby",    "Witts",         "SS", 24, 94, 88, 88, 85, 82, 86, 0, 0,  9.0f, 5, "KCP", "R", "R"));
        t.roster.Add(P(226, "Maikel",   "Garcias",       "3B", 27, 76, 74, 70, 72, 70, 74, 0, 0,  3.0f, 3, "KCP", "R", "R"));
        t.roster.Add(P(227, "Michael",  "Masseys",       "2B", 27, 70, 68, 65, 65, 65, 70, 0, 0,  1.0f, 1, "KCP", "L", "L"));
        t.roster.Add(P(228, "Matt",     "Duffy",         "3B", 34, 65, 65, 58, 62, 62, 68, 0, 0,  2.0f, 1, "KCP", "R", "R"));

        // OUTFIELDERS
        t.roster.Add(P(229, "Starling", "Martes",        "RF", 32, 78, 75, 75, 78, 72, 76, 0, 0,  6.0f, 2, "KCP", "R", "R"));
        t.roster.Add(P(230, "Lane",     "Thomass",       "CF", 29, 74, 72, 68, 78, 70, 74, 0, 0,  4.0f, 2, "KCP", "R", "R"));
        t.roster.Add(P(231, "Isaac",    "Collinss",      "LF", 27, 72, 70, 68, 70, 65, 70, 0, 0,  1.0f, 2, "KCP", "L", "L"));
        t.roster.Add(P(232, "Nelson",   "Velazquezz",    "RF", 28, 68, 65, 65, 68, 62, 68, 0, 0,  1.0f, 1, "KCP", "R", "R"));

        // DH / BENCH
        t.roster.Add(P(233, "Hunter",   "Renfroes",      "DH", 32, 74, 70, 75, 68, 68, 70, 0, 0,  4.0f, 1, "KCP", "R", "R"));
        t.roster.Add(P(234, "Freddy",   "Fermin",        "C",  28, 65, 63, 58, 52, 60, 66, 0, 0,  1.0f, 1, "KCP", "R", "R"));
    }

    // -------------------------------------------------------
    // MINNESOTA VOYAGERS — 2026 Twins
    // -------------------------------------------------------
    void BuildMNV(Team t)
    {
        t.roster = new List<Player>();

        // ROTATION
        t.roster.Add(P(235, "Pablo",    "Lopez",         "SP", 29, 88, 0, 0, 58, 65, 65, 88, 82, 14.0f, 4, "MNV", "R", "R"));
        t.roster.Add(P(236, "Joe",      "Ryans",         "SP", 27, 84, 0, 0, 55, 62, 62, 84, 80,  3.0f, 3, "MNV", "R", "R"));
        t.roster.Add(P(237, "Simeon",   "Woods-Richardsons","SP",24,78,0, 0, 50, 58, 58, 78, 74,  1.0f, 2, "MNV", "R", "R"));
        t.roster.Add(P(238, "Bailey",   "Ober",          "SP", 28, 76, 0, 0, 48, 55, 55, 76, 72,  3.0f, 2, "MNV", "R", "R"));
        t.roster.Add(P(239, "David",    "Festas",        "SP", 26, 74, 0, 0, 46, 52, 52, 74, 70,  1.0f, 1, "MNV", "R", "R"));

        // BULLPEN
        Player mnvCL = P(240, "Jhoan",  "Durans",        "RP", 27, 90, 0, 0, 55, 65, 65, 90, 80,  5.0f, 3, "MNV", "R", "R");
        mnvCL.bullpenRole = "CL"; t.roster.Add(mnvCL);

        Player mnvSU = P(241, "Griffin","Jaxs",          "RP", 28, 80, 0, 0, 50, 58, 58, 80, 70,  4.0f, 2, "MNV", "R", "R");
        mnvSU.bullpenRole = "SU"; t.roster.Add(mnvSU);

        Player mnvMR1 = P(242, "Caleb", "Thielbars",     "RP", 37, 74, 0, 0, 44, 50, 50, 74, 62,  3.0f, 1, "MNV", "L", "L");
        mnvMR1.bullpenRole = "MR"; t.roster.Add(mnvMR1);

        Player mnvMR2 = P(243, "Brock", "Stewarts",      "RP", 32, 72, 0, 0, 42, 48, 48, 72, 60,  2.0f, 1, "MNV", "L", "L");
        mnvMR2.bullpenRole = "MR"; t.roster.Add(mnvMR2);

        Player mnvMR3 = P(244, "Jorge", "Alcalass",      "RP", 28, 70, 0, 0, 40, 46, 46, 70, 58,  1.5f, 1, "MNV", "R", "R");
        mnvMR3.bullpenRole = "MR"; t.roster.Add(mnvMR3);

        Player mnvRP1 = P(245, "Josh",  "Winders",       "RP", 28, 68, 0, 0, 38, 44, 44, 68, 55,  1.0f, 1, "MNV", "R", "R");
        mnvRP1.bullpenRole = "MR"; t.roster.Add(mnvRP1);

        Player mnvRP2 = P(246, "Ronny", "Henriquezz",    "RP", 25, 66, 0, 0, 36, 42, 42, 66, 52,  1.0f, 1, "MNV", "R", "R");
        mnvRP2.bullpenRole = "MR"; t.roster.Add(mnvRP2);

        // CATCHERS
        t.roster.Add(P(247, "Bryan",     "Jeff",      "C",  27, 75, 70, 70, 55, 65, 74, 0, 0,  2.0f, 2, "MNV", "R", "R"));
        t.roster.Add(P(248, "Christian","Vazquezz",      "C",  34, 65, 62, 58, 52, 62, 70, 0, 0,  7.0f, 1, "MNV", "R", "R"));

        // INFIELDERS
        t.roster.Add(P(249, "Carlos",   "Samsons",      "1B", 39, 75, 75, 72, 48, 60, 70, 0, 0, 14.0f, 1, "MNV", "S", "S"));
        t.roster.Add(P(250, "Edouard",  "Juliano",       "2B", 25, 76, 74, 72, 70, 68, 72, 0, 0,  1.0f, 2, "MNV", "L", "L"));
        t.roster.Add(P(251, "Carlos",   "Calebs",       "SS", 31, 82, 78, 80, 68, 75, 80, 0, 0, 35.0f, 4, "MNV", "R", "R"));
        t.roster.Add(P(252, "Royce",    "Lewinsky",        "3B", 25, 80, 76, 76, 75, 72, 76, 0, 0,  1.0f, 3, "MNV", "R", "R"));
        t.roster.Add(P(253, "Brooky",   "Leon",          "SS", 26, 70, 68, 62, 68, 65, 70, 0, 0,  1.0f, 1, "MNV", "R", "R"));
        t.roster.Add(P(254, "Kylor",     "Farmerss",      "2B", 33, 65, 63, 58, 62, 62, 68, 0, 0,  3.0f, 1, "MNV", "R", "R"));

        // OUTFIELDERS
        t.roster.Add(P(255, "Byron",    "Buxton",        "CF", 33, 82, 75, 80, 88, 82, 85, 0, 0, 10.0f, 3, "MNV", "R", "R"));
        t.roster.Add(P(256, "Trevor",   "Larnachs",      "LF", 28, 74, 72, 72, 68, 65, 70, 0, 0,  1.0f, 2, "MNV", "L", "L"));
        t.roster.Add(P(257, "Alex",     "Kirilloffs",    "RF", 27, 80, 78, 78, 68, 70, 74, 0, 0,  9.0f, 4, "MNV", "L", "L"));
        t.roster.Add(P(258, "Nicky",     "Gordons",       "LF", 29, 68, 65, 62, 68, 62, 68, 0, 0,  2.0f, 1, "MNV", "L", "L"));

        // DH / BENCH
        t.roster.Add(P(259, "Max",      "Klement",        "DH", 34, 74, 72, 72, 65, 65, 70, 0, 0,  10.0f,1, "MNV", "L", "L"));
        t.roster.Add(P(260, "Matt",     "Wally",      "RF", 25, 68, 65, 68, 65, 62, 66, 0, 0,  1.0f, 1, "MNV", "L", "L"));
    }
}
