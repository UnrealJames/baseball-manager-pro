using UnityEngine;
using System.Collections.Generic;

public class RosterBuilder_NLWest : MonoBehaviour
{
    public void BuildAllRosters(List<Team> allTeams)
    {
        foreach (Team t in allTeams)
        {
            if (t.abbreviation == "AZS") BuildAZS(t);
            if (t.abbreviation == "COP") BuildCOP(t);
            if (t.abbreviation == "LAB") BuildLAB(t);
            if (t.abbreviation == "SDS") BuildSDS(t);
            if (t.abbreviation == "SFF") BuildSFF(t);
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
    // ARIZONA SCORPIONS — 2026 Diamondbacks
    // -------------------------------------------------------
    void BuildAZS(Team t)
    {
        t.roster = new List<Player>();

        // ROTATION
        t.roster.Add(P(651, "Zac",      "Galens",        "SP", 30, 88, 0, 0, 55, 65, 65, 88, 82, 12.0f, 4, "AZS", "R", "R"));
        t.roster.Add(P(652, "Merrill",  "Kellys",        "SP", 36, 80, 0, 0, 50, 58, 58, 80, 76, 14.0f, 2, "AZS", "R", "R"));
        t.roster.Add(P(653, "Brandon",  "Pfeaadts",      "SP", 28, 78, 0, 0, 48, 55, 55, 78, 74,  3.0f, 2, "AZS", "R", "R"));
        t.roster.Add(P(654, "Eduardo",  "Rodriguezz",    "SP", 33, 76, 0, 0, 46, 52, 52, 76, 72, 20.0f, 2, "AZS", "L", "L"));
        t.roster.Add(P(655, "Ryne",     "Nelsons",       "SP", 27, 74, 0, 0, 44, 50, 50, 74, 70,  1.0f, 1, "AZS", "R", "R"));

        // BULLPEN
        Player azsCL = P(656, "Paul",   "Seamss",        "RP", 31, 86, 0, 0, 52, 62, 62, 86, 76,  4.0f, 2, "AZS", "R", "R");
        azsCL.bullpenRole = "CL"; t.roster.Add(azsCL);

        Player azsSU = P(657, "Kevin",  "Ginkel",        "RP", 31, 78, 0, 0, 48, 55, 55, 78, 68,  3.0f, 2, "AZS", "R", "R");
        azsSU.bullpenRole = "SU"; t.roster.Add(azsSU);

        Player azsMR1 = P(658, "Joe",   "Mantiply",      "RP", 34, 74, 0, 0, 44, 50, 50, 74, 62,  2.0f, 1, "AZS", "L", "L");
        azsMR1.bullpenRole = "MR"; t.roster.Add(azsMR1);

        Player azsMR2 = P(659, "Miguel","Castros",       "RP", 31, 72, 0, 0, 42, 48, 48, 72, 60,  3.0f, 1, "AZS", "R", "R");
        azsMR2.bullpenRole = "MR"; t.roster.Add(azsMR2);

        Player azsMR3 = P(660, "Scott", "McGoughs",      "RP", 35, 70, 0, 0, 40, 46, 46, 70, 58,  2.0f, 1, "AZS", "R", "R");
        azsMR3.bullpenRole = "MR"; t.roster.Add(azsMR3);

        Player azsRP1 = P(661, "Kyle",  "Nelsons",       "RP", 29, 68, 0, 0, 38, 44, 44, 68, 55,  1.0f, 1, "AZS", "R", "R");
        azsRP1.bullpenRole = "MR"; t.roster.Add(azsRP1);

        Player azsRP2 = P(662, "Justin","Martinezz",     "RP", 25, 70, 0, 0, 40, 46, 46, 70, 58,  1.0f, 1, "AZS", "R", "R");
        azsRP2.bullpenRole = "MR"; t.roster.Add(azsRP2);

        // CATCHERS
        t.roster.Add(P(663, "Gabriel",  "Morenos",       "C",  24, 78, 72, 75, 55, 65, 74, 0, 0,  1.0f, 3, "AZS", "R", "R"));
        t.roster.Add(P(664, "Tucker",   "Barnhartss",    "C",  35, 62, 58, 55, 48, 56, 62, 0, 0,  4.0f, 1, "AZS", "L", "S"));

        // INFIELDERS
        t.roster.Add(P(665, "Christian","Walkers",       "1B", 34, 84, 80, 84, 60, 70, 76, 0, 0, 20.0f, 3, "AZS", "R", "R"));
        t.roster.Add(P(666, "Ketel",    "Martes",        "2B", 31, 88, 86, 80, 72, 74, 80, 0, 0, 14.0f, 5, "AZS", "S", "S"));
        t.roster.Add(P(667, "Geraldo",  "Perdomos",      "SS", 24, 72, 68, 62, 72, 68, 72, 0, 0,  1.0f, 2, "AZS", "S", "S"));
        t.roster.Add(P(668, "Eugenio",  "Suarezz",       "3B", 35, 74, 68, 78, 55, 62, 66, 0, 0,  9.0f, 1, "AZS", "R", "R"));
        t.roster.Add(P(669, "Jace",     "Petersonss",    "2B", 30, 65, 63, 58, 65, 62, 66, 0, 0,  2.0f, 1, "AZS", "L", "L"));
        t.roster.Add(P(670, "Blaze",    "Alexanderss",   "SS", 27, 65, 62, 58, 65, 60, 65, 0, 0,  1.0f, 1, "AZS", "R", "R"));

        // OUTFIELDERS
        t.roster.Add(P(671, "Corbin",   "Carrolls",      "CF", 24, 84, 80, 76, 84, 78, 82, 0, 0,  1.0f, 3, "AZS", "L", "L"));
        t.roster.Add(P(672, "Lourdes",  "Gurriel",       "LF", 32, 78, 76, 76, 68, 70, 72, 0, 0, 11.0f, 2, "AZS", "R", "R"));
        t.roster.Add(P(673, "Jake",     "McCarthys",     "RF", 29, 72, 70, 68, 74, 68, 72, 0, 0,  1.0f, 2, "AZS", "L", "L"));
        t.roster.Add(P(674, "Daulton",  "Varshos",       "RF", 28, 74, 70, 72, 76, 70, 74, 0, 0,  7.0f, 1, "AZS", "L", "L"));

        // DH / BENCH
        t.roster.Add(P(675, "Joc",      "Pedersons",     "DH", 34, 76, 72, 78, 65, 65, 68, 0, 0,  6.0f, 1, "AZS", "L", "L"));
        t.roster.Add(P(676, "Alek",     "Thomass",       "CF", 27, 70, 68, 65, 70, 65, 68, 0, 0,  1.0f, 1, "AZS", "R", "R"));
    }

    // -------------------------------------------------------
    // COLORADO PEAKS — 2026 Rockies
    // -------------------------------------------------------
    void BuildCOP(Team t)
    {
        t.roster = new List<Player>();

        // ROTATION
        t.roster.Add(P(677, "Kyle",     "Freeland",      "SP", 33, 74, 0, 0, 48, 55, 55, 74, 70,  8.0f, 1, "COP", "L", "L"));
        t.roster.Add(P(678, "Austin",   "Gombers",       "SP", 31, 72, 0, 0, 46, 52, 52, 72, 68,  9.0f, 1, "COP", "L", "L"));
        t.roster.Add(P(679, "Cal",      "Quantrills",    "SP", 31, 70, 0, 0, 44, 50, 50, 70, 66,  8.0f, 1, "COP", "R", "R"));
        t.roster.Add(P(680, "Ryan",     "Feltners",      "SP", 28, 68, 0, 0, 42, 48, 48, 68, 64,  2.0f, 1, "COP", "R", "R"));
        t.roster.Add(P(681, "Chase",    "Andersons",     "SP", 34, 65, 0, 0, 40, 46, 46, 65, 62,  4.0f, 1, "COP", "R", "R"));

        // BULLPEN
        Player copCL = P(682, "Daniel", "Bards",         "RP", 41, 72, 0, 0, 48, 55, 55, 72, 62,  4.0f, 1, "COP", "R", "R");
        copCL.bullpenRole = "CL"; t.roster.Add(copCL);

        Player copSU = P(683, "Justin", "Lawrences",     "RP", 28, 76, 0, 0, 48, 55, 55, 76, 66,  2.0f, 2, "COP", "R", "R");
        copSU.bullpenRole = "SU"; t.roster.Add(copSU);

        Player copMR1 = P(684, "Yency", "Almonte",       "RP", 31, 70, 0, 0, 42, 48, 48, 70, 60,  2.0f, 1, "COP", "R", "R");
        copMR1.bullpenRole = "MR"; t.roster.Add(copMR1);

        Player copMR2 = P(685, "Jake",  "Birss",         "RP", 27, 68, 0, 0, 40, 46, 46, 68, 58,  1.0f, 1, "COP", "R", "R");
        copMR2.bullpenRole = "MR"; t.roster.Add(copMR2);

        Player copMR3 = P(686, "Ty",    "Blach",         "RP", 34, 66, 0, 0, 38, 44, 44, 66, 55,  2.0f, 1, "COP", "L", "L");
        copMR3.bullpenRole = "MR"; t.roster.Add(copMR3);

        Player copRP1 = P(687, "Lucas", "Gilbreath",     "RP", 29, 64, 0, 0, 36, 42, 42, 64, 52,  1.0f, 1, "COP", "L", "L");
        copRP1.bullpenRole = "MR"; t.roster.Add(copRP1);

        Player copRP2 = P(688, "Gavin", "Hollowell",     "RP", 27, 62, 0, 0, 34, 40, 40, 62, 50,  1.0f, 1, "COP", "R", "R");
        copRP2.bullpenRole = "MR"; t.roster.Add(copRP2);

        // CATCHERS
        t.roster.Add(P(689, "Elias",    "Diazz",         "C",  33, 70, 65, 65, 48, 60, 68, 0, 0,  4.0f, 1, "COP", "R", "R"));
        t.roster.Add(P(690, "Hunter",   "Goodmans",      "C",  31, 60, 56, 55, 46, 54, 60, 0, 0,  2.0f, 1, "COP", "R", "R"));

        // INFIELDERS
        t.roster.Add(P(691, "Michael",  "Toglia",        "1B", 27, 70, 65, 74, 55, 60, 64, 0, 0,  1.0f, 1, "COP", "L", "L"));
        t.roster.Add(P(692, "Brendan",  "Rodgerss",      "2B", 29, 72, 70, 65, 65, 65, 70, 0, 0,  5.0f, 1, "COP", "R", "R"));
        t.roster.Add(P(693, "Ezequiel", "Tovarss",       "SS", 28, 74, 72, 65, 72, 68, 72, 0, 0,  3.0f, 2, "COP", "R", "R"));
        t.roster.Add(P(694, "Ryan",     "McMahon",       "3B", 30, 76, 72, 74, 65, 68, 72, 0, 0, 12.0f, 3, "COP", "L", "L"));
        t.roster.Add(P(695, "Alan",     "Triejo",        "2B", 26, 65, 63, 58, 62, 60, 65, 0, 0,  1.0f, 1, "COP", "R", "R"));
        t.roster.Add(P(696, "Harold",   "Castros",       "SS", 28, 62, 60, 55, 60, 58, 62, 0, 0,  1.0f, 1, "COP", "R", "R"));

        // OUTFIELDERS
        t.roster.Add(P(697, "Charlie",  "Blackmons",     "RF", 39, 70, 70, 68, 65, 62, 65, 0, 0, 21.0f, 1, "COP", "L", "L"));
        t.roster.Add(P(698, "Randal",   "Grichuk",       "CF", 35, 68, 65, 68, 65, 62, 65, 0, 0,  6.0f, 1, "COP", "R", "R"));
        t.roster.Add(P(699, "Brenton",  "Doyles",        "LF", 24, 74, 70, 68, 72, 65, 70, 0, 0,  1.0f, 2, "COP", "R", "R"));
        t.roster.Add(P(700, "Nolan",    "Joness",        "LF", 25, 68, 65, 65, 68, 60, 64, 0, 0,  1.0f, 1, "COP", "R", "R"));

        // DH / BENCH
        t.roster.Add(P(701, "Kris",     "Bryants",       "DH", 34, 72, 70, 72, 62, 65, 68, 0, 0, 28.0f, 1, "COP", "R", "R"));
        t.roster.Add(P(702, "Sean",     "Bouchard",      "1B", 29, 65, 63, 63, 58, 58, 62, 0, 0,  1.0f, 1, "COP", "R", "R"));
    }

    // -------------------------------------------------------
    // LOS ANGELES BILLIONAIRES — 2026 Dodgers
    // -------------------------------------------------------
    void BuildLAB(Team t)
    {
        t.roster = new List<Player>();

        // ROTATION
        t.roster.Add(P(703, "Yoshinobu","Yamamotos",     "SP", 27, 94, 0, 0, 62, 70, 70, 94, 88, 32.0f, 9, "LAB", "R", "R"));
        t.roster.Add(P(704, "Tyler",    "Glasnows",      "SP", 32, 90, 0, 0, 58, 65, 65, 90, 84, 24.0f, 5, "LAB", "R", "R"));
        t.roster.Add(P(705, "Jack",     "Flahertys",     "SP", 30, 82, 0, 0, 52, 60, 60, 82, 78, 19.0f, 2, "LAB", "R", "R"));
        t.roster.Add(P(706, "Dustin",   "Mays",          "SP", 27, 80, 0, 0, 50, 58, 58, 80, 76,  1.0f, 2, "LAB", "R", "R"));
        t.roster.Add(P(707, "Justin",   "Wrobleskis",    "SP", 27, 76, 0, 0, 48, 55, 55, 76, 72,  1.0f, 1, "LAB", "R", "R"));

        // BULLPEN
        Player labCL = P(708, "Evan",   "Phillips",      "RP", 31, 86, 0, 0, 52, 62, 62, 86, 76,  4.0f, 2, "LAB", "R", "R");
        labCL.bullpenRole = "CL"; t.roster.Add(labCL);

        Player labSU = P(709, "Blake",  "Treinen",       "RP", 37, 80, 0, 0, 50, 58, 58, 80, 70,  8.0f, 1, "LAB", "R", "R");
        labSU.bullpenRole = "SU"; t.roster.Add(labSU);

        Player labMR1 = P(710, "Alex",  "Vessia",        "RP", 27, 76, 0, 0, 44, 50, 50, 76, 64,  1.0f, 1, "LAB", "R", "R");
        labMR1.bullpenRole = "MR"; t.roster.Add(labMR1);

        Player labMR2 = P(711, "Anthony","Bandass",      "RP", 29, 74, 0, 0, 42, 48, 48, 74, 62,  2.0f, 1, "LAB", "R", "R");
        labMR2.bullpenRole = "MR"; t.roster.Add(labMR2);

        Player labMR3 = P(712, "Brusdar","Grateroless",  "RP", 29, 78, 0, 0, 46, 52, 52, 78, 68,  5.0f, 2, "LAB", "R", "R");
        labMR3.bullpenRole = "MR"; t.roster.Add(labMR3);

        Player labRP1 = P(713, "Yohan", "Ramirezzs",     "RP", 27, 72, 0, 0, 40, 46, 46, 72, 60,  1.0f, 1, "LAB", "R", "R");
        labRP1.bullpenRole = "MR"; t.roster.Add(labRP1);

        Player labRP2 = P(714, "Ryan",  "Brasiers",      "RP", 36, 68, 0, 0, 38, 44, 44, 68, 55,  1.0f, 1, "LAB", "R", "R");
        labRP2.bullpenRole = "MR"; t.roster.Add(labRP2);

        // CATCHERS
        t.roster.Add(P(715, "Will",     "Smithss",       "C",  31, 84, 78, 80, 55, 68, 78, 0, 0, 17.0f, 4, "LAB", "R", "R"));
        t.roster.Add(P(716, "Austin",   "Barnss",        "C",  35, 65, 62, 55, 48, 58, 65, 0, 0,  6.0f, 1, "LAB", "R", "R"));

        // INFIELDERS
        t.roster.Add(P(717, "Freddie",  "Fremonts",      "1B", 37, 90, 88, 82, 60, 70, 80, 0, 0, 27.0f, 5, "LAB", "L", "L"));
        t.roster.Add(P(718, "Gavin",    "Luxs",          "2B", 28, 80, 78, 72, 72, 72, 76, 0, 0,  8.0f, 3, "LAB", "R", "R"));
        t.roster.Add(P(719, "Miguel",   "Rojas",         "SS", 36, 68, 65, 58, 62, 65, 70, 0, 0,  5.0f, 1, "LAB", "R", "R"));
        t.roster.Add(P(720, "Max",      "Muncy",         "3B", 35, 78, 74, 80, 58, 65, 68, 0, 0, 13.0f, 1, "LAB", "L", "L"));
        t.roster.Add(P(721, "Chris",    "Taylors",       "2B", 35, 68, 65, 62, 65, 62, 66, 0, 0,  7.0f, 1, "LAB", "R", "R"));
        t.roster.Add(P(722, "Kiké",     "Hernandezss",   "SS", 34, 65, 63, 60, 65, 62, 66, 0, 0,  9.0f, 1, "LAB", "R", "R"));

        // OUTFIELDERS
        t.roster.Add(P(723, "Mookie",   "Bettzs",        "RF", 34, 92, 88, 82, 85, 84, 88, 0, 0, 30.0f, 9, "LAB", "R", "R"));
        t.roster.Add(P(724, "Teoscar",  "Hernandezss",   "LF", 33, 78, 74, 80, 68, 70, 72, 0, 0, 23.0f, 2, "LAB", "R", "R"));
        t.roster.Add(P(725, "Andy",     "Pages",         "CF", 24, 74, 68, 74, 72, 68, 70, 0, 0,  1.0f, 2, "LAB", "R", "R"));
        t.roster.Add(P(726, "James",    "Outmans",       "LF", 28, 72, 68, 68, 74, 66, 70, 0, 0,  1.0f, 2, "LAB", "L", "L"));

        // DH / BENCH
        t.roster.Add(P(727, "Shohei",   "Otanis",        "DH", 32, 99, 92, 99, 82, 85, 85, 0, 0, 46.0f, 9, "LAB", "R", "L"));
        t.roster.Add(P(728, "Miguel",   "Vargas",        "1B", 25, 70, 68, 68, 62, 60, 64, 0, 0,  1.0f, 1, "LAB", "R", "R"));
    }

    // -------------------------------------------------------
    // SAN DIEGO SURFERS — 2026 Padres
    // -------------------------------------------------------
    void BuildSDS(Team t)
    {
        t.roster = new List<Player>();

        // ROTATION
        t.roster.Add(P(729, "Dylan",    "Ceases",        "SP", 31, 88, 0, 0, 55, 65, 65, 88, 82, 18.0f, 3, "SDS", "R", "R"));
        t.roster.Add(P(730, "Michael",  "Kings",         "SP", 29, 82, 0, 0, 52, 60, 60, 82, 78,  5.0f, 3, "SDS", "R", "R"));
        t.roster.Add(P(731, "Joe",      "Musgroves",     "SP", 34, 80, 0, 0, 50, 58, 58, 80, 76, 20.0f, 3, "SDS", "R", "R"));
        t.roster.Add(P(732, "Randy",    "Vasquezz",      "SP", 26, 76, 0, 0, 48, 55, 55, 76, 72,  1.0f, 2, "SDS", "R", "R"));
        t.roster.Add(P(733, "Matt",     "Waldrons",      "SP", 28, 74, 0, 0, 46, 52, 52, 74, 70,  2.0f, 1, "SDS", "L", "L"));

        // BULLPEN
        Player sdsCL = P(734, "Robert", "Suarezz",       "RP", 33, 86, 0, 0, 52, 62, 62, 86, 76,  4.0f, 2, "SDS", "R", "R");
        sdsCL.bullpenRole = "CL"; t.roster.Add(sdsCL);

        Player sdsSU = P(735, "Jhony",  "Brahos",        "RP", 26, 80, 0, 0, 50, 58, 58, 80, 70,  1.0f, 2, "SDS", "R", "R");
        sdsSU.bullpenRole = "SU"; t.roster.Add(sdsSU);

        Player sdsMR1 = P(736, "Yuki",  "Matsuis",       "RP", 28, 76, 0, 0, 44, 50, 50, 76, 64,  3.0f, 2, "SDS", "R", "R");
        sdsMR1.bullpenRole = "MR"; t.roster.Add(sdsMR1);

        Player sdsMR2 = P(737, "Steven","Wilsons",       "RP", 31, 72, 0, 0, 42, 48, 48, 72, 60,  2.0f, 1, "SDS", "R", "R");
        sdsMR2.bullpenRole = "MR"; t.roster.Add(sdsMR2);

        Player sdsMR3 = P(738, "Tom",   "Cossgroves",    "RP", 32, 70, 0, 0, 40, 46, 46, 70, 58,  2.0f, 1, "SDS", "L", "L");
        sdsMR3.bullpenRole = "MR"; t.roster.Add(sdsMR3);

        Player sdsRP1 = P(739, "Adrian","Morejon",       "RP", 26, 68, 0, 0, 38, 44, 44, 68, 55,  1.0f, 1, "SDS", "L", "L");
        sdsRP1.bullpenRole = "MR"; t.roster.Add(sdsRP1);

        Player sdsRP2 = P(740, "Enyel", "DeLosSantosss", "RP", 29, 66, 0, 0, 36, 42, 42, 66, 52,  1.5f, 1, "SDS", "R", "R");
        sdsRP2.bullpenRole = "MR"; t.roster.Add(sdsRP2);

        // CATCHERS
        t.roster.Add(P(741, "Kyle",     "Higashiokas",   "C",  34, 65, 60, 60, 48, 58, 65, 0, 0,  1.0f, 1, "SDS", "R", "R"));
        t.roster.Add(P(742, "Luis",     "Campusanos",    "C",  26, 68, 65, 65, 52, 60, 66, 0, 0,  1.0f, 1, "SDS", "R", "R"));

        // INFIELDERS
        t.roster.Add(P(743, "Jake",     "Croneworths",   "1B", 31, 80, 78, 74, 65, 68, 74, 0, 0,  9.0f, 3, "SDS", "L", "L"));
        t.roster.Add(P(744, "Xander",   "Bogaertss",     "2B", 33, 80, 80, 74, 70, 72, 76, 0, 0, 20.0f, 5, "SDS", "R", "R"));
        t.roster.Add(P(745, "Ha-Seong", "Kims",          "SS", 30, 82, 78, 72, 75, 76, 80, 0, 0,  7.0f, 3, "SDS", "R", "R"));
        t.roster.Add(P(746, "Manny",    "Machado",       "3B", 34, 90, 84, 86, 65, 76, 84, 0, 0, 32.0f, 7, "SDS", "R", "R"));
        t.roster.Add(P(747, "Tyler",    "Wadds",         "2B", 29, 68, 65, 62, 62, 62, 67, 0, 0,  2.0f, 1, "SDS", "R", "R"));
        t.roster.Add(P(748, "Matthew",  "Barreras",      "SS", 27, 65, 62, 58, 60, 60, 64, 0, 0,  1.0f, 1, "SDS", "R", "R"));

        // OUTFIELDERS
        t.roster.Add(P(749, "Fernando", "Tatiss",        "CF", 27, 94, 86, 90, 88, 84, 86, 0, 0, 14.0f, 9, "SDS", "R", "R"));
        t.roster.Add(P(750, "Jackson",  "Mercers",       "LF", 27, 80, 76, 75, 74, 72, 75, 0, 0,  5.0f, 3, "SDS", "R", "R"));
        t.roster.Add(P(751, "Jurickson","Profars",       "RF", 33, 74, 72, 68, 68, 65, 70, 0, 0,  1.0f, 1, "SDS", "L", "L"));
        t.roster.Add(P(752, "Jose",     "Azobars",       "CF", 28, 70, 68, 65, 72, 65, 68, 0, 0,  3.0f, 2, "SDS", "R", "R"));

        // DH / BENCH
        t.roster.Add(P(753, "David",    "Peralta",       "DH", 37, 68, 66, 66, 62, 60, 64, 0, 0,  3.0f, 1, "SDS", "L", "L"));
        t.roster.Add(P(754, "Taylor",   "Kohlweys",      "1B", 33, 65, 62, 62, 58, 58, 62, 0, 0,  2.0f, 1, "SDS", "L", "L"));
    }

    // -------------------------------------------------------
    // SAN FRANCISCO FOG — 2026 Giants
    // -------------------------------------------------------
    void BuildSFF(Team t)
    {
        t.roster = new List<Player>();

        // ROTATION
        t.roster.Add(P(755, "Logan",    "Webbs",         "SP", 29, 92, 0, 0, 58, 65, 65, 92, 86, 15.0f, 5, "SFF", "R", "R"));
        t.roster.Add(P(756, "Robbie",   "Rays",          "SP", 33, 82, 0, 0, 52, 60, 60, 82, 78, 10.0f, 2, "SFF", "L", "L"));
        t.roster.Add(P(757, "Alex",     "Coobs",         "SP", 33, 80, 0, 0, 50, 58, 58, 80, 76,  9.0f, 2, "SFF", "R", "R"));
        t.roster.Add(P(758, "Keaton",   "Winn",          "SP", 27, 76, 0, 0, 48, 55, 55, 76, 72,  1.0f, 2, "SFF", "R", "R"));
        t.roster.Add(P(759, "Jordan",   "Hicks",         "SP", 29, 74, 0, 0, 46, 52, 52, 74, 70,  4.0f, 2, "SFF", "R", "R"));

        // BULLPEN
        Player sffCL = P(760, "Camilo", "Doval",         "RP", 28, 88, 0, 0, 52, 62, 62, 88, 78,  5.0f, 3, "SFF", "R", "R");
        sffCL.bullpenRole = "CL"; t.roster.Add(sffCL);

        Player sffSU = P(761, "Tyler",  "Rogersss",      "RP", 32, 80, 0, 0, 50, 58, 58, 80, 70,  4.0f, 2, "SFF", "R", "R");
        sffSU.bullpenRole = "SU"; t.roster.Add(sffSU);

        Player sffMR1 = P(762, "John",  "Brebebias",     "RP", 31, 76, 0, 0, 44, 50, 50, 76, 64,  3.0f, 1, "SFF", "R", "R");
        sffMR1.bullpenRole = "MR"; t.roster.Add(sffMR1);

        Player sffMR2 = P(763, "Ryan",  "Walkers",       "RP", 29, 74, 0, 0, 42, 48, 48, 74, 62,  2.0f, 1, "SFF", "R", "R");
        sffMR2.bullpenRole = "MR"; t.roster.Add(sffMR2);

        Player sffMR3 = P(764, "Sean",  "Hjelles",       "RP", 29, 70, 0, 0, 40, 46, 46, 70, 58,  1.5f, 1, "SFF", "R", "R");
        sffMR3.bullpenRole = "MR"; t.roster.Add(sffMR3);

        Player sffRP1 = P(765, "Tristan","Beckss",       "RP", 27, 68, 0, 0, 38, 44, 44, 68, 55,  1.0f, 1, "SFF", "R", "R");
        sffRP1.bullpenRole = "MR"; t.roster.Add(sffRP1);

        Player sffRP2 = P(766, "Luke",  "Jacksons",      "RP", 33, 66, 0, 0, 36, 42, 42, 66, 52,  2.0f, 1, "SFF", "R", "R");
        sffRP2.bullpenRole = "MR"; t.roster.Add(sffRP2);

        // CATCHERS
        t.roster.Add(P(767, "Patrick",  "Baileys",       "C",  27, 80, 72, 70, 55, 68, 78, 0, 0,  1.0f, 3, "SFF", "R", "R"));
        t.roster.Add(P(768, "Blake",    "Savares",       "C",  32, 62, 58, 58, 48, 56, 62, 0, 0,  3.0f, 1, "SFF", "R", "R"));

        // INFIELDERS
        t.roster.Add(P(769, "Matt",     "Chapmans",      "3B", 33, 84, 78, 82, 68, 76, 84, 0, 0, 18.0f, 2, "SFF", "R", "R"));
        t.roster.Add(P(770, "Luis",     "Arraezz",       "2B", 29, 84, 92, 65, 65, 68, 76, 0, 0, 18.0f, 3, "SFF", "R", "R"));
        t.roster.Add(P(771, "Willy",    "Adamess",       "SS", 30, 84, 78, 82, 72, 74, 78, 0, 0, 18.0f, 3, "SFF", "R", "R"));
        t.roster.Add(P(772, "Casey",    "Schmitts",      "1B", 27, 72, 68, 70, 62, 65, 70, 0, 0,  1.0f, 2, "SFF", "R", "R"));
        t.roster.Add(P(773, "Thairo",   "Estradas",      "SS", 29, 70, 68, 65, 65, 65, 70, 0, 0,  4.0f, 1, "SFF", "R", "R"));
        t.roster.Add(P(774, "Brett",    "Wisely",        "2B", 27, 66, 63, 60, 65, 60, 65, 0, 0,  1.0f, 1, "SFF", "R", "R"));

        // OUTFIELDERS
        t.roster.Add(P(775, "Jung Hoo", "Lees",          "CF", 27, 84, 82, 72, 80, 78, 82, 0, 0, 26.0f, 5, "SFF", "L", "L"));
        t.roster.Add(P(776, "Heliot",   "Ramoss",        "LF", 25, 76, 72, 74, 72, 68, 72, 0, 0,  1.0f, 2, "SFF", "R", "R"));
        t.roster.Add(P(777, "Rafael",   "Devers",        "RF", 29, 88, 85, 88, 68, 70, 74, 0, 0, 22.0f, 5, "SFF", "R", "L"));
        t.roster.Add(P(778, "Michael",  "Confortoss",    "RF", 33, 74, 72, 72, 68, 65, 68, 0, 0,  9.0f, 1, "SFF", "R", "R"));

        // DH / BENCH
        t.roster.Add(P(779, "Joc",      "Pedersons",     "DH", 34, 74, 70, 76, 65, 62, 65, 0, 0,  6.0f, 1, "SFF", "L", "L"));
        t.roster.Add(P(780, "LaMonte",  "Wades",         "LF", 31, 68, 66, 62, 68, 60, 65, 0, 0,  3.0f, 1, "SFF", "L", "L"));
    }
}
