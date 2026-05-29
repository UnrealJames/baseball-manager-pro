using UnityEngine;
using System.Collections.Generic;

public class RosterBuilder_NLEast : MonoBehaviour
{
    public void BuildAllRosters(List<Team> allTeams)
    {
        foreach (Team t in allTeams)
        {
            if (t.abbreviation == "ATB") BuildATB(t);
            if (t.abbreviation == "MMP") BuildMMP(t);
            if (t.abbreviation == "NYC") BuildNYC(t);
            if (t.abbreviation == "PHF") BuildPHF(t);
            if (t.abbreviation == "WAS") BuildWAS(t);
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
    // ATLANTA BRAWLERS — 2026 Braves
    // -------------------------------------------------------
    void BuildATB(Team t)
    {
        t.roster = new List<Player>();

        // ROTATION
        t.roster.Add(P(391, "Chris",    "Sales",         "SP", 37, 86, 0, 0, 55, 65, 65, 86, 80, 20.0f, 2, "ATB", "L", "L"));
        t.roster.Add(P(392, "Spencer",  "Striders",      "SP", 27, 90, 0, 0, 58, 65, 65, 90, 84, 18.0f, 4, "ATB", "R", "R"));
        t.roster.Add(P(393, "Reynaldo", "Lopezz",        "SP", 31, 80, 0, 0, 52, 60, 60, 80, 76,  8.0f, 2, "ATB", "R", "R"));
        t.roster.Add(P(394, "Charlie",  "Mortons",       "SP", 42, 74, 0, 0, 46, 52, 52, 74, 70, 15.0f, 1, "ATB", "R", "R"));
        t.roster.Add(P(395, "AJ",       "Smiths",        "SP", 26, 76, 0, 0, 48, 55, 55, 76, 72,  1.0f, 2, "ATB", "R", "R"));

        // BULLPEN
        Player atbCL = P(396, "Raisel", "Iglesiass",     "RP", 35, 84, 0, 0, 50, 60, 60, 84, 74,  7.0f, 2, "ATB", "R", "R");
        atbCL.bullpenRole = "CL"; t.roster.Add(atbCL);

        Player atbSU = P(397, "Joe",    "Jimenezz",      "RP", 29, 78, 0, 0, 48, 55, 55, 78, 68,  3.0f, 2, "ATB", "R", "R");
        atbSU.bullpenRole = "SU"; t.roster.Add(atbSU);

        Player atbMR1 = P(398, "Pierce","Johnsons",      "RP", 32, 74, 0, 0, 44, 50, 50, 74, 62,  2.0f, 1, "ATB", "R", "R");
        atbMR1.bullpenRole = "MR"; t.roster.Add(atbMR1);

        Player atbMR2 = P(399, "Dylan", "Lees",          "RP", 29, 72, 0, 0, 42, 48, 48, 72, 60,  2.0f, 1, "ATB", "L", "L");
        atbMR2.bullpenRole = "MR"; t.roster.Add(atbMR2);

        Player atbMR3 = P(400, "Nick",  "Andersons",     "RP", 34, 70, 0, 0, 40, 46, 46, 70, 58,  3.0f, 1, "ATB", "R", "R");
        atbMR3.bullpenRole = "MR"; t.roster.Add(atbMR3);

        Player atbRP1 = P(401, "Jesse", "Chavezz",       "RP", 40, 68, 0, 0, 38, 44, 44, 68, 55,  2.0f, 1, "ATB", "R", "R");
        atbRP1.bullpenRole = "MR"; t.roster.Add(atbRP1);

        Player atbRP2 = P(402, "Kirby", "Yatess",        "RP", 31, 72, 0, 0, 42, 48, 48, 72, 62,  4.0f, 1, "ATB", "R", "R");
        atbRP2.bullpenRole = "MR"; t.roster.Add(atbRP2);

        // CATCHERS
        t.roster.Add(P(403, "Sean",     "Murphyss",      "C",  30, 82, 75, 78, 55, 68, 78, 0, 0,  5.0f, 3, "ATB", "R", "R"));
        t.roster.Add(P(404, "Travis",   "dArnauds",      "C",  37, 68, 65, 65, 50, 62, 68, 0, 0,  8.0f, 1, "ATB", "R", "R"));

        // INFIELDERS
        t.roster.Add(P(405, "Matt",     "Olmstedds",     "1B", 24, 78, 74, 80, 55, 65, 70, 0, 0,  1.0f, 2, "ATB", "R", "R"));
        t.roster.Add(P(406, "Ozzie",    "Albies",        "2B", 29, 86, 84, 80, 78, 78, 82, 0, 0,  7.0f, 5, "ATB", "S", "S"));
        t.roster.Add(P(407, "Orlando",  "Arcias",        "SS", 32, 74, 72, 68, 68, 70, 75, 0, 0,  7.0f, 2, "ATB", "R", "R"));
        t.roster.Add(P(408, "Austin",   "Rileye",        "3B", 29, 90, 85, 88, 68, 75, 80, 0, 0,  7.0f, 5, "ATB", "R", "R"));
        t.roster.Add(P(409, "Whit",     "Merrifields",   "2B", 36, 68, 68, 60, 72, 65, 70, 0, 0,  3.0f, 1, "ATB", "R", "R"));
        t.roster.Add(P(410, "Vaughn",   "Grissom",       "SS", 24, 70, 68, 65, 68, 65, 70, 0, 0,  1.0f, 1, "ATB", "R", "R"));

        // OUTFIELDERS
        t.roster.Add(P(411, "Ronald",   "Acunass",       "RF", 28, 97, 88, 92, 90, 85, 88, 0, 0, 24.0f, 7, "ATB", "R", "R"));
        t.roster.Add(P(412, "Michael",  "Harriss",       "CF", 25, 82, 78, 78, 82, 78, 82, 0, 0,  1.0f, 3, "ATB", "R", "R"));
        t.roster.Add(P(413, "Marcell",  "Ozouna",        "LF", 34, 76, 72, 78, 65, 68, 70, 0, 0, 16.0f, 1, "ATB", "R", "R"));
        t.roster.Add(P(414, "Eddie",    "Rosarios",      "LF", 32, 72, 70, 70, 68, 65, 70, 0, 0,  7.0f, 1, "ATB", "L", "L"));

        // DH / BENCH
        t.roster.Add(P(415, "Jarred",   "Kelenic",       "DH", 27, 74, 70, 74, 70, 65, 70, 0, 0,  1.0f, 2, "ATB", "L", "L"));
        t.roster.Add(P(416, "Matt",     "Salazar",       "C",  29, 62, 60, 58, 52, 58, 64, 0, 0,  1.0f, 1, "ATB", "R", "R"));
    }

    // -------------------------------------------------------
    // MIAMI PIRANHAS — 2026 Marlins
    // -------------------------------------------------------
    void BuildMMP(Team t)
    {
        t.roster = new List<Player>();

        // ROTATION
        t.roster.Add(P(417, "Sandy",    "Alcantarass",   "SP", 30, 86, 0, 0, 55, 62, 62, 86, 80, 14.0f, 3, "MMP", "R", "R"));
        t.roster.Add(P(418, "Braxton",  "Garrets",       "SP", 27, 80, 0, 0, 52, 58, 58, 80, 76,  3.0f, 2, "MMP", "R", "R"));
        t.roster.Add(P(419, "Trevor",   "Rogerss",       "SP", 28, 76, 0, 0, 50, 55, 55, 76, 72,  3.0f, 2, "MMP", "L", "L"));
        t.roster.Add(P(420, "Jesus",    "Luzardoss",     "SP", 27, 74, 0, 0, 48, 52, 52, 74, 70,  1.0f, 2, "MMP", "L", "L"));
        t.roster.Add(P(421, "Ryan",     "Weatherss",     "SP", 26, 72, 0, 0, 46, 50, 50, 72, 68,  2.0f, 1, "MMP", "L", "L"));

        // BULLPEN
        Player mmpCL = P(422, "Dylan",  "Flemals",       "RP", 30, 80, 0, 0, 50, 58, 58, 80, 70,  3.0f, 2, "MMP", "L", "L");
        mmpCL.bullpenRole = "CL"; t.roster.Add(mmpCL);

        Player mmpSU = P(423, "Tanner", "Scotts",        "RP", 30, 76, 0, 0, 48, 55, 55, 76, 66,  2.0f, 2, "MMP", "R", "R");
        mmpSU.bullpenRole = "SU"; t.roster.Add(mmpSU);

        Player mmpMR1 = P(424, "Andrew","Nardis",        "RP", 28, 72, 0, 0, 44, 50, 50, 72, 62,  1.5f, 1, "MMP", "R", "R");
        mmpMR1.bullpenRole = "MR"; t.roster.Add(mmpMR1);

        Player mmpMR2 = P(425, "Huascar","Ynoas",        "RP", 28, 70, 0, 0, 42, 48, 48, 70, 60,  1.0f, 1, "MMP", "R", "R");
        mmpMR2.bullpenRole = "MR"; t.roster.Add(mmpMR2);

        Player mmpMR3 = P(426, "Anthony","Bantas",       "RP", 29, 68, 0, 0, 40, 46, 46, 68, 58,  1.0f, 1, "MMP", "R", "R");
        mmpMR3.bullpenRole = "MR"; t.roster.Add(mmpMR3);

        Player mmpRP1 = P(427, "Steven", "Okerts",       "RP", 30, 66, 0, 0, 38, 44, 44, 66, 55,  1.0f, 1, "MMP", "R", "R");
        mmpRP1.bullpenRole = "MR"; t.roster.Add(mmpRP1);

        Player mmpRP2 = P(428, "JT",    "Chargois",      "RP", 34, 64, 0, 0, 36, 42, 42, 64, 52,  1.5f, 1, "MMP", "R", "R");
        mmpRP2.bullpenRole = "MR"; t.roster.Add(mmpRP2);

        // CATCHERS
        t.roster.Add(P(429, "Nick",     "Fortezz",       "C",  30, 68, 62, 60, 52, 60, 68, 0, 0,  2.0f, 1, "MMP", "R", "R"));
        t.roster.Add(P(430, "Jacob",    "Stallingss",    "C",  32, 65, 60, 58, 48, 58, 65, 0, 0,  3.0f, 1, "MMP", "R", "R"));

        // INFIELDERS
        t.roster.Add(P(431, "Josh",     "Bells",         "1B", 32, 74, 72, 74, 52, 62, 68, 0, 0,  6.0f, 1, "MMP", "S", "S"));
        t.roster.Add(P(432, "Luis",     "Arraezz",       "2B", 29, 84, 92, 65, 65, 68, 76, 0, 0, 18.0f, 4, "MMP", "R", "R"));
        t.roster.Add(P(433, "Jon",      "Betancourts",   "SS", 32, 70, 68, 65, 68, 65, 70, 0, 0,  5.0f, 1, "MMP", "R", "R"));
        t.roster.Add(P(434, "Joey",     "Wendless",      "3B", 27, 72, 68, 70, 62, 65, 68, 0, 0,  1.0f, 2, "MMP", "R", "R"));
        t.roster.Add(P(435, "Otto",     "Lopezz",        "2B", 25, 68, 65, 60, 68, 62, 66, 0, 0,  1.0f, 1, "MMP", "R", "R"));
        t.roster.Add(P(436, "Griffin",  "Connss",        "SS", 26, 65, 62, 58, 62, 60, 65, 0, 0,  1.0f, 1, "MMP", "R", "R"));

        // OUTFIELDERS
        t.roster.Add(P(437, "Jazz",     "Chisholmss",    "CF", 28, 82, 76, 80, 82, 75, 78, 0, 0,  8.0f, 2, "MMP", "L", "S"));
        t.roster.Add(P(438, "Bryan",    "DeLeGalloss",   "LF", 26, 74, 70, 72, 70, 68, 70, 0, 0,  1.0f, 2, "MMP", "R", "R"));
        t.roster.Add(P(439, "Peyton",   "Burdicks",      "RF", 26, 70, 68, 68, 68, 65, 68, 0, 0,  1.0f, 2, "MMP", "R", "R"));
        t.roster.Add(P(440, "Jesus",    "Sanchezz",      "RF", 28, 72, 68, 74, 65, 65, 68, 0, 0,  3.0f, 2, "MMP", "R", "R"));

        // DH / BENCH
        t.roster.Add(P(441, "Jorge",    "Solars",        "DH", 32, 70, 68, 72, 55, 60, 65, 0, 0,  3.0f, 1, "MMP", "R", "R"));
        t.roster.Add(P(442, "Garrett",  "Hampson",       "2B", 31, 62, 60, 55, 70, 58, 62, 0, 0,  2.0f, 1, "MMP", "R", "R"));
    }

    // -------------------------------------------------------
    // NEW YORK COPS — 2026 Mets
    // -------------------------------------------------------
    void BuildNYC(Team t)
    {
        t.roster = new List<Player>();

        // ROTATION
        t.roster.Add(P(443, "Kodai",    "Senga",         "SP", 32, 88, 0, 0, 55, 65, 65, 88, 82, 15.0f, 3, "NYC", "R", "R"));
        t.roster.Add(P(444, "Sean",     "Manaeass",      "SP", 33, 86, 0, 0, 52, 62, 62, 86, 80, 14.0f, 2, "NYC", "R", "R"));
        t.roster.Add(P(445, "David",    "Petersons",     "SP", 30, 78, 0, 0, 50, 58, 58, 78, 74,  5.0f, 2, "NYC", "L", "L"));
        t.roster.Add(P(446, "Jose",     "Quintanas",     "SP", 37, 74, 0, 0, 46, 52, 52, 74, 70, 13.0f, 1, "NYC", "L", "L"));
        t.roster.Add(P(447, "Tylor",    "Megills",       "SP", 29, 72, 0, 0, 44, 50, 50, 72, 68,  2.0f, 1, "NYC", "R", "R"));

        // BULLPEN
        Player nycCL = P(448, "Edwin",  "Diazz",         "RP", 31, 92, 0, 0, 55, 65, 65, 92, 82,  8.0f, 3, "NYC", "R", "R");
        nycCL.bullpenRole = "CL"; t.roster.Add(nycCL);

        Player nycSU = P(449, "Adam",   "Ottavinos",     "RP", 40, 78, 0, 0, 48, 55, 55, 78, 68,  6.0f, 1, "NYC", "R", "R");
        nycSU.bullpenRole = "SU"; t.roster.Add(nycSU);

        Player nycMR1 = P(450, "Drew",  "Smithss",       "RP", 30, 74, 0, 0, 44, 50, 50, 74, 62,  2.0f, 1, "NYC", "R", "R");
        nycMR1.bullpenRole = "MR"; t.roster.Add(nycMR1);

        Player nycMR2 = P(451, "Brooks","Raelyy",        "RP", 29, 72, 0, 0, 42, 48, 48, 72, 60,  2.0f, 1, "NYC", "R", "R");
        nycMR2.bullpenRole = "MR"; t.roster.Add(nycMR2);

        Player nycMR3 = P(452, "Jake",  "Dyness",        "RP", 37, 70, 0, 0, 40, 46, 46, 70, 58,  5.0f, 1, "NYC", "R", "R");
        nycMR3.bullpenRole = "MR"; t.roster.Add(nycMR3);

        Player nycRP1 = P(453, "Elieser","Hernandezss",  "RP", 29, 68, 0, 0, 38, 44, 44, 68, 55,  1.5f, 1, "NYC", "R", "R");
        nycRP1.bullpenRole = "MR"; t.roster.Add(nycRP1);

        Player nycRP2 = P(454, "Jeff",  "Brigham",       "RP", 33, 66, 0, 0, 36, 42, 42, 66, 52,  1.0f, 1, "NYC", "R", "R");
        nycRP2.bullpenRole = "MR"; t.roster.Add(nycRP2);

        // CATCHERS
        t.roster.Add(P(455, "Francisco","Alvarez",       "C",  23, 84, 75, 85, 52, 65, 74, 0, 0,  1.0f, 3, "NYC", "R", "R"));
        t.roster.Add(P(456, "Omar",     "Narvaezz",      "C",  33, 65, 62, 58, 48, 58, 65, 0, 0,  4.0f, 1, "NYC", "L", "L"));

        // INFIELDERS
        t.roster.Add(P(457, "Pete",     "Alonzos",       "1B", 31, 90, 82, 92, 55, 68, 76, 0, 0, 20.0f, 2, "NYC", "R", "R"));
        t.roster.Add(P(458, "Jeff",     "McNeil",        "2B", 33, 82, 88, 68, 70, 72, 78, 0, 0,  8.0f, 3, "NYC", "L", "L"));
        t.roster.Add(P(459, "Francisco","Lindors",       "SS", 32, 90, 85, 82, 78, 80, 85, 0, 0, 34.0f, 6, "NYC", "S", "S"));
        t.roster.Add(P(460, "Mark",     "Vientos",       "3B", 25, 78, 72, 82, 58, 65, 68, 0, 0,  1.0f, 2, "NYC", "R", "R"));
        t.roster.Add(P(461, "DJ",       "McSpades",      "2B", 36, 70, 72, 62, 58, 65, 72, 0, 0,  6.0f, 1, "NYC", "R", "R"));
        t.roster.Add(P(462, "Jose",     "Iglesiass",     "SS", 35, 65, 63, 55, 62, 62, 68, 0, 0,  3.0f, 1, "NYC", "R", "R"));

        // OUTFIELDERS
        t.roster.Add(P(463, "Juan",     "Sotoss",        "LF", 26, 96, 90, 92, 75, 80, 82, 0, 0, 31.0f, 1, "NYC", "L", "L"));
        t.roster.Add(P(464, "Brandon",  "Nimitss",       "CF", 30, 78, 74, 72, 78, 72, 76, 0, 0,  7.0f, 2, "NYC", "R", "R"));
        t.roster.Add(P(465, "Starling", "Martes",        "RF", 32, 76, 72, 74, 76, 70, 74, 0, 0,  6.0f, 2, "NYC", "R", "R"));
        t.roster.Add(P(466, "Tyrone",   "Taylors",       "RF", 30, 68, 65, 65, 68, 62, 66, 0, 0,  2.0f, 1, "NYC", "R", "R"));

        // DH / BENCH
        t.roster.Add(P(467, "J.D.",     "Daviss",        "DH", 31, 76, 72, 78, 58, 62, 65, 0, 0,  5.0f, 2, "NYC", "R", "R"));
        t.roster.Add(P(468, "Darin",    "Ruffs",         "1B", 31, 68, 65, 70, 50, 58, 62, 0, 0,  3.0f, 1, "NYC", "L", "L"));
    }

    // -------------------------------------------------------
    // PHILADELPHIA FOUNDERS — 2026 Phillies
    // -------------------------------------------------------
    void BuildPHF(Team t)
    {
        t.roster = new List<Player>();

        // ROTATION
        t.roster.Add(P(469, "Zack",     "Wheelers",      "SP", 36, 90, 0, 0, 58, 65, 65, 90, 84, 23.0f, 3, "PHF", "R", "R"));
        t.roster.Add(P(470, "Aaron",    "Nolas",         "SP", 33, 88, 0, 0, 55, 62, 62, 88, 82, 16.0f, 4, "PHF", "R", "R"));
        t.roster.Add(P(471, "Ranger",   "Suarezz",       "SP", 30, 82, 0, 0, 52, 60, 60, 82, 78, 17.0f, 3, "PHF", "L", "L"));
        t.roster.Add(P(472, "Cristopher","Sanchez",      "SP", 29, 78, 0, 0, 50, 58, 58, 78, 74,  3.0f, 2, "PHF", "L", "L"));
        t.roster.Add(P(473, "Michael",  "Lorenzen",      "SP", 34, 74, 0, 0, 46, 52, 52, 74, 70,  5.0f, 1, "PHF", "R", "R"));

        // BULLPEN
        Player phfCL = P(474, "Craig",  "Kimbrel",       "RP", 36, 82, 0, 0, 50, 60, 60, 82, 72,  8.0f, 1, "PHF", "R", "R");
        phfCL.bullpenRole = "CL"; t.roster.Add(phfCL);

        Player phfSU = P(475, "Gregory","Soto",          "RP", 29, 78, 0, 0, 48, 55, 55, 78, 68,  4.0f, 2, "PHF", "L", "L");
        phfSU.bullpenRole = "SU"; t.roster.Add(phfSU);

        Player phfMR1 = P(476, "Matt",  "Strahms",       "RP", 32, 76, 0, 0, 44, 50, 50, 76, 64,  5.0f, 2, "PHF", "L", "L");
        phfMR1.bullpenRole = "MR"; t.roster.Add(phfMR1);

        Player phfMR2 = P(477, "Seranthony","Dominquezz","RP", 29, 74, 0, 0, 42, 48, 48, 74, 62,  3.0f, 1, "PHF", "R", "R");
        phfMR2.bullpenRole = "MR"; t.roster.Add(phfMR2);

        Player phfMR3 = P(478, "Jose",  "Alvaradoss",    "RP", 30, 72, 0, 0, 40, 46, 46, 72, 60,  4.0f, 1, "PHF", "L", "L");
        phfMR3.bullpenRole = "MR"; t.roster.Add(phfMR3);

        Player phfRP1 = P(479, "Yunior","Manoss",        "RP", 27, 70, 0, 0, 38, 44, 44, 70, 58,  1.0f, 1, "PHF", "R", "R");
        phfRP1.bullpenRole = "MR"; t.roster.Add(phfRP1);

        Player phfRP2 = P(480, "Andrew","Painter",       "RP", 23, 74, 0, 0, 42, 50, 50, 74, 68,  1.0f, 1, "PHF", "R", "R");
        phfRP2.bullpenRole = "MR"; t.roster.Add(phfRP2);

        // CATCHERS
        t.roster.Add(P(481, "JT",       "Realmutos",     "C",  35, 84, 78, 78, 60, 72, 82, 0, 0, 24.0f, 2, "PHF", "R", "R"));
        t.roster.Add(P(482, "Garrett",  "Stubbs",        "C",  31, 62, 58, 55, 52, 58, 62, 0, 0,  1.0f, 1, "PHF", "R", "R"));

        // INFIELDERS
        t.roster.Add(P(483, "Bryce",    "Harpers",       "1B", 33, 94, 88, 92, 68, 75, 78, 0, 0, 26.0f, 6, "PHF", "L", "L"));
        t.roster.Add(P(484, "Bryson",   "Stotts",        "2B", 25, 82, 78, 78, 75, 75, 80, 0, 0,  1.0f, 3, "PHF", "R", "R"));
        t.roster.Add(P(485, "Trea",     "Turners",       "SS", 33, 88, 85, 80, 85, 80, 84, 0, 0, 30.0f, 7, "PHF", "R", "R"));
        t.roster.Add(P(486, "Alec",     "Bohms",         "3B", 29, 82, 80, 78, 65, 70, 76, 0, 0,  5.0f, 3, "PHF", "R", "R"));
        t.roster.Add(P(487, "Edmundo",  "Sosas",         "SS", 29, 65, 63, 58, 65, 62, 68, 0, 0,  2.0f, 1, "PHF", "R", "R"));
        t.roster.Add(P(488, "Kody",     "Clemens",       "2B", 29, 62, 60, 60, 60, 58, 63, 0, 0,  1.0f, 1, "PHF", "L", "L"));

        // OUTFIELDERS
        t.roster.Add(P(489, "Kyle",     "Schwarbzers",   "LF", 32, 88, 80, 92, 65, 68, 70, 0, 0, 22.0f, 3, "PHF", "L", "L"));
        t.roster.Add(P(490, "Brandon",  "Marshs",        "CF", 28, 78, 74, 74, 78, 74, 78, 0, 0,  2.0f, 2, "PHF", "L", "L"));
        t.roster.Add(P(491, "Nick",     "Castellanos",   "RF", 34, 80, 78, 80, 65, 68, 70, 0, 0, 20.0f, 3, "PHF", "R", "R"));
        t.roster.Add(P(492, "Johan",    "Rohass",        "RF", 28, 70, 68, 68, 68, 65, 68, 0, 0,  2.0f, 2, "PHF", "R", "R"));

        // DH / BENCH
        t.roster.Add(P(493, "Whit",     "Merrifield",    "DH", 36, 68, 68, 60, 72, 62, 68, 0, 0,  3.0f, 1, "PHF", "R", "R"));
        t.roster.Add(P(494, "Cal",      "Stevensons",    "LF", 30, 68, 68, 60, 72, 62, 68, 0, 0,  3.0f, 1, "PHF", "L", "L"));
    }

    // -------------------------------------------------------
    // WASHINGTON SOUTHPAWS — 2026 Nationals
    // -------------------------------------------------------
    void BuildWAS(Team t)
    {
        t.roster = new List<Player>();

        // ROTATION
        t.roster.Add(P(495, "MacKenzie","Gores",         "SP", 29, 82, 0, 0, 52, 60, 60, 82, 78,  5.0f, 3, "WAS", "L", "L"));
        t.roster.Add(P(496, "Patrick",  "Corbins",       "SP", 36, 72, 0, 0, 46, 52, 52, 72, 68, 23.0f, 1, "WAS", "L", "L"));
        t.roster.Add(P(497, "Trevor",   "Williamss",     "SP", 33, 70, 0, 0, 44, 50, 50, 70, 66,  4.0f, 1, "WAS", "R", "R"));
        t.roster.Add(P(498, "Jake",     "Irvin",         "SP", 28, 74, 0, 0, 48, 55, 55, 74, 70,  1.0f, 2, "WAS", "R", "R"));
        t.roster.Add(P(499, "DJ",       "Herzes",        "SP", 28, 72, 0, 0, 46, 52, 52, 72, 68,  1.0f, 1, "WAS", "R", "R"));

        // BULLPEN
        Player wasCL = P(500, "Kyle",   "Finnegans",     "RP", 32, 80, 0, 0, 50, 58, 58, 80, 70,  3.0f, 2, "WAS", "L", "L");
        wasCL.bullpenRole = "CL"; t.roster.Add(wasCL);

        Player wasSU = P(501, "Hunter", "Harveys",       "RP", 30, 76, 0, 0, 48, 55, 55, 76, 66,  2.0f, 2, "WAS", "R", "R");
        wasSU.bullpenRole = "SU"; t.roster.Add(wasSU);

        Player wasMR1 = P(502, "Carl",  "Edwards",       "RP", 35, 70, 0, 0, 42, 48, 48, 70, 60,  2.0f, 1, "WAS", "R", "R");
        wasMR1.bullpenRole = "MR"; t.roster.Add(wasMR1);

        Player wasMR2 = P(503, "Derek", "Lawss",         "RP", 29, 68, 0, 0, 40, 46, 46, 68, 58,  1.5f, 1, "WAS", "R", "R");
        wasMR2.bullpenRole = "MR"; t.roster.Add(wasMR2);

        Player wasMR3 = P(504, "Mason", "Thompsons",     "RP", 27, 66, 0, 0, 38, 44, 44, 66, 55,  1.0f, 1, "WAS", "R", "R");
        wasMR3.bullpenRole = "MR"; t.roster.Add(wasMR3);

        Player wasRP1 = P(505, "Erasmo","Ramirezz",      "RP", 36, 64, 0, 0, 36, 42, 42, 64, 52,  2.0f, 1, "WAS", "R", "R");
        wasRP1.bullpenRole = "MR"; t.roster.Add(wasRP1);

        Player wasRP2 = P(506, "Chad",  "Kuhlss",        "RP", 31, 62, 0, 0, 34, 40, 40, 62, 50,  2.0f, 1, "WAS", "R", "R");
        wasRP2.bullpenRole = "MR"; t.roster.Add(wasRP2);

        // CATCHERS
        t.roster.Add(P(507, "Keibert",  "Ruizz",         "C",  26, 76, 72, 70, 52, 62, 72, 0, 0,  7.0f, 4, "WAS", "S", "S"));
        t.roster.Add(P(508, "Riley",    "Adams",         "C",  28, 62, 58, 58, 48, 56, 62, 0, 0,  1.0f, 1, "WAS", "R", "R"));

        // INFIELDERS
        t.roster.Add(P(509, "Joey",     "Meneses",       "1B", 33, 74, 72, 74, 52, 60, 68, 0, 0,  2.0f, 2, "WAS", "R", "R"));
        t.roster.Add(P(510, "Luis",     "Garcia",        "2B", 24, 74, 70, 68, 72, 68, 72, 0, 0,  1.0f, 2, "WAS", "L", "L"));
        t.roster.Add(P(511, "CJ",       "Abroms",        "SS", 25, 76, 72, 70, 70, 68, 72, 0, 0,  1.0f, 2, "WAS", "R", "R"));
        t.roster.Add(P(512, "Jeimer",   "Candelarios",   "3B", 31, 76, 74, 74, 62, 65, 70, 0, 0,  5.0f, 2, "WAS", "S", "S"));
        t.roster.Add(P(513, "Ildemaro", "Vargas",        "2B", 33, 62, 60, 55, 60, 58, 64, 0, 0,  1.0f, 1, "WAS", "R", "R"));
        t.roster.Add(P(514, "Trey",     "Lipscomb",      "3B", 25, 65, 62, 63, 60, 60, 64, 0, 0,  1.0f, 1, "WAS", "R", "R"));

        // OUTFIELDERS
        t.roster.Add(P(515, "Jesse",    "Winkerss",      "LF", 31, 74, 74, 70, 60, 62, 66, 0, 0, 21.0f, 1, "WAS", "L", "L"));
        t.roster.Add(P(516, "Victor",   "Robles",        "CF", 27, 70, 65, 60, 80, 70, 75, 0, 0,  4.0f, 1, "WAS", "R", "R"));
        t.roster.Add(P(517, "Lane",     "Thomasss",      "RF", 29, 72, 68, 68, 72, 65, 70, 0, 0,  4.0f, 2, "WAS", "R", "R"));
        t.roster.Add(P(518, "Stone",    "Garretts",      "CF", 24, 68, 65, 62, 70, 62, 66, 0, 0,  1.0f, 1, "WAS", "R", "R"));

        // DH / BENCH
        t.roster.Add(P(519, "Dominic",  "Smithss",       "DH", 30, 68, 66, 68, 55, 58, 62, 0, 0,  2.0f, 1, "WAS", "L", "L"));
        t.roster.Add(P(520, "Corey",    "Dickersons",    "LF", 36, 65, 65, 65, 62, 58, 62, 0, 0,  3.0f, 1, "WAS", "L", "L"));
    }
}
