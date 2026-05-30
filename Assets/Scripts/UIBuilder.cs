using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class UIBuilder : MonoBehaviour
{
    // -------------------------------------------------------
    // COLOR SCHEME
    // -------------------------------------------------------
    static Color BG      = Hex("#080e1a");
    static Color SURFACE = Hex("#0f1926");
    static Color RED     = Hex("#e8192c");
    static Color GOLD    = Hex("#f5c842");
    static Color GREEN   = Hex("#00e676");
    static Color TEXT    = Hex("#e8edf5");
    static Color SUBTEXT = Hex("#8899aa");
    static Color BORDER  = Hex("#1a2a3a");

    // -------------------------------------------------------
    // SCREENS
    // -------------------------------------------------------
    GameObject mainMenuScreen;
    GameObject continueScreen;
    GameObject teamSelectScreen;
    GameObject gmNameScreen;
    GameObject teamScreen;
    GameObject standingsScreen;
    GameObject tradeScreen;
    GameObject draftScreen;
    GameObject faScreen;
    GameObject liveGameScreen;
    GameObject boxScoreScreen;
    GameObject currentScreen;
    GameObject preGameScreen;


    // Data
    DataLoader dataLoader;

    // GM info
    string gmName       = "";
    string selectedTeam = "";

    // Save slot management
    int          currentSaveSlot = 0;
    int          saveSlotIndex   = 0;
    List<string> saveInfos       = new List<string>();

    // Team select
    int        currentTeamIndex = 0;
    List<Team> allTeamsList     = new List<Team>();

    // Team screen
    int  currentTab  = 0;
    Team currentTeam = null;

    // Roster browsing
    List<Player> currentRoster = new List<Player>();
    int          rosterIndex   = 0;

    // Standings
    int      currentDivision = 0;
    string[] divisionNames   = new string[]
    {
        "AL East", "AL Central", "AL West",
        "NL East", "NL Central", "NL West"
    };

    // Trade screen
    Team         tradeTeam2      = null;
    Team         tradeTeam3      = null;
    bool         isThreeTeam     = false;
    int          tradeTeam2Index = 0;
    int          tradeTeam3Index = 1;
    List<Player> myOffer         = new List<Player>();
    List<Player> team2Offer      = new List<Player>();
    List<Player> team3Offer      = new List<Player>();
    int          myBrowseIndex   = 0;
    int          t2BrowseIndex   = 0;
    int          t3BrowseIndex   = 0;

    // Draft screen
    List<Player> draftClass     = new List<Player>();
    int          draftPickIndex = 0;
    int          myDraftPick    = 0;
    bool         draftStarted   = false;
    int          draftRound     = 1;
    int          totalRounds    = 10;

    // FA screen
    List<Player> faPool      = new List<Player>();
    int          faIndex     = 0;
    int          offerYears  = 2;
    float        offerSalary = 5.0f;

    // Live game state
    Team         homeTeam         = null;
    Team         awayTeam         = null;
    int          homeScore        = 0;
    int          awayScore        = 0;
    int          currentInning    = 1;
    int          outs             = 0;
    int          balls            = 0;
    int          strikes          = 0;
    bool         isTopInning      = true;
    bool         gameOnBase1      = false;
    bool         gameOnBase2      = false;
    bool         gameOnBase3      = false;
    List<string> playByPlay       = new List<string>();
    int          homePitcherIndex = 0;
    int          awayPitcherIndex = 0;
    int          homeBatterIndex  = 0;
    int          awayBatterIndex  = 0;
    bool         gameInProgress   = false;
    bool         gameOver         = false;
    List<Player> usedRelievers    = new List<Player>();

    // Scoreboard tracking
    int[]         homeInningRuns  = new int[12];
    int[]         awayInningRuns  = new int[12];
    int           homeHits        = 0;
    int           awayHits        = 0;
    int           homeErrors      = 0;
    int           awayErrors      = 0;
    RectTransform CurInnHighlight = null;
    float         sbColStart      = -118f;
    float         sbColW          = 26f;


    // Pre-game
    Team         pgHomeTeam      = null;
    Team         pgAwayTeam      = null;
    int          pgHomeLineupIdx = 0;
    int          pgAwayLineupIdx = 0;
    bool         pgShowingHome   = true;
    int          pgGameNumber    = 1;
    List<Player> pgHomeLineup    = new List<Player>();
    List<Player> pgAwayLineup    = new List<Player>();


    // Box score
    bool boxScoreShowingHome = true;

    // Track pitcher of record
    Player lastHomePitcher  = null;
    Player lastAwayPitcher  = null;


    // -------------------------------------------------------
    // START
    // -------------------------------------------------------
    void Start()
    {
        dataLoader = FindFirstObjectByType<DataLoader>();
        Invoke("BuildUI", 1.5f);
    }

    void BuildUI()
    {
        Canvas canvas    = CreateCanvas();
        mainMenuScreen   = BuildMainMenu(canvas.gameObject);
        gmNameScreen     = BuildGMNameScreen(canvas.gameObject);
        teamSelectScreen = BuildTeamSelectScreen(canvas.gameObject);
        teamScreen       = BuildTeamScreen(canvas.gameObject);
        standingsScreen  = BuildStandingsScreen(canvas.gameObject);
        tradeScreen      = BuildTradeScreen(canvas.gameObject);
        draftScreen      = BuildDraftScreen(canvas.gameObject);
        faScreen         = BuildFAScreen(canvas.gameObject);
        liveGameScreen   = BuildLiveGameScreen(canvas.gameObject);
        continueScreen   = BuildContinueScreen(canvas.gameObject);
        boxScoreScreen   = BuildBoxScoreScreen(canvas.gameObject);
        preGameScreen    = BuildPreGameScreen(canvas.gameObject);
        ShowScreen(mainMenuScreen);
    }

    // -------------------------------------------------------
    // CANVAS
    // -------------------------------------------------------
    Canvas CreateCanvas()
    {
        GameObject obj     = new GameObject("GameCanvas");
        Canvas canvas      = obj.AddComponent<Canvas>();
        canvas.renderMode  = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;

        CanvasScaler cs        = obj.AddComponent<CanvasScaler>();
        cs.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(390, 844);
        cs.matchWidthOrHeight  = 0.5f;

        obj.AddComponent<GraphicRaycaster>();

        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        return canvas;
    }

    // -------------------------------------------------------
    // MAIN MENU
    // -------------------------------------------------------
    GameObject BuildMainMenu(GameObject canvas)
    {
        GameObject screen = CreateScreen(canvas, "MainMenu");

        BuildImageBackground(screen, "background");
        AddImage(screen, "Overlay",
            new Color(0.03f, 0.05f, 0.10f, 0.75f),
            Vector2.zero, new Vector2(390, 844));

        AddText(screen, "Studio",
            "DUGOUT ENTERTAINMENT",
            14, GOLD, new Vector2(0, 370),
            new Vector2(350, 30));

        AddText(screen, "Title",
            "BASEBALL\nMANAGER PRO",
            52, TEXT, new Vector2(0, 240),
            new Vector2(370, 140), FontStyles.Bold);

        AddImage(screen, "TitleLine", RED,
            new Vector2(0, 155), new Vector2(200, 3));

        AddText(screen, "Tagline",
            "Built by fans, played by fans",
            16, GOLD, new Vector2(0, 125),
            new Vector2(350, 30));

        GameObject newBtn = CreateButton(screen,
            "NEW SEASON", RED, TEXT,
            new Vector2(0, 20),
            new Vector2(300, 62), 20);
        GetButton(newBtn).onClick.AddListener(() =>
            ShowScreen(gmNameScreen));

        GameObject contBtn = CreateButton(screen,
            "CONTINUE", SURFACE, SUBTEXT,
            new Vector2(0, -60),
            new Vector2(300, 62), 20);
        AddBorder(contBtn, BORDER, 2);
        GetButton(contBtn).onClick.AddListener(() =>
        {
            GameManager gm =
                FindFirstObjectByType<GameManager>();
            if (gm != null && gm.HasAnySave())
            {
                ShowScreen(continueScreen);
                PopulateContinueScreen();
            }
            else
                Debug.Log("No save files found!");
        });

        GameObject settBtn = CreateButton(screen,
            "SETTINGS", SURFACE, SUBTEXT,
            new Vector2(0, -135),
            new Vector2(300, 62), 18);
        AddBorder(settBtn, BORDER, 2);

        AddText(screen, "Version",
            "v0.1.0  •  Season 2026",
            12, SUBTEXT, new Vector2(0, -370),
            new Vector2(350, 24));

        screen.SetActive(false);
        return screen;
    }

    // -------------------------------------------------------
    // CONTINUE SCREEN
    // -------------------------------------------------------
    GameObject BuildContinueScreen(GameObject canvas)
    {
        GameObject screen =
            CreateScreen(canvas, "Continue");

        BuildImageBackground(screen, "background");
        AddImage(screen, "Overlay",
            new Color(0.03f, 0.05f, 0.10f, 0.88f),
            Vector2.zero, new Vector2(390, 844));

        AddImage(screen, "Header",
            new Color(0.06f, 0.12f, 0.20f, 1f),
            new Vector2(0, 370), new Vector2(390, 100));

        AddImage(screen, "HeaderBorder", RED,
            new Vector2(0, 322), new Vector2(390, 3));

        AddText(screen, "Title",
            "CONTINUE FRANCHISE",
            22, TEXT, new Vector2(0, 378),
            new Vector2(340, 44), FontStyles.Bold);

        AddText(screen, "Sub",
            "Select your save file",
            13, GOLD, new Vector2(0, 350),
            new Vector2(300, 24));

        GameObject backBtn = CreateButton(screen,
            "BACK", SURFACE, GOLD,
            new Vector2(-140, 378),
            new Vector2(80, 44), 14);
        GetButton(backBtn).onClick.AddListener(() =>
            ShowScreen(mainMenuScreen));

        AddImage(screen, "SlotCard",
            new Color(0.06f, 0.12f, 0.20f, 0.97f),
            new Vector2(0, 80), new Vector2(340, 300));

        AddImage(screen, "SlotCardTop", RED,
            new Vector2(0, 228), new Vector2(340, 5));

        AddImage(screen, "SlotBadge", RED,
            new Vector2(-130, 210), new Vector2(80, 30));
        AddText(screen, "SlotNumber",
            "SLOT 1", 12, TEXT,
            new Vector2(-130, 210),
            new Vector2(80, 30), FontStyles.Bold);

        AddText(screen, "SlotTeam",
            "", 64, RED,
            new Vector2(0, 145),
            new Vector2(340, 90), FontStyles.Bold);

        AddText(screen, "SlotGM",
            "", 16, SUBTEXT,
            new Vector2(0, 78),
            new Vector2(300, 28));

        AddText(screen, "SlotSeason",
            "", 22, TEXT,
            new Vector2(0, 45),
            new Vector2(300, 36), FontStyles.Bold);

        AddImage(screen, "SlotDiv", BORDER,
            new Vector2(0, 18), new Vector2(300, 1));

        AddText(screen, "SlotStatus",
            "EMPTY SLOT", 16, SUBTEXT,
            new Vector2(0, -5),
            new Vector2(300, 30));

        AddText(screen, "SlotCounter",
            "1 / 3", 13, SUBTEXT,
            new Vector2(0, -115),
            new Vector2(200, 28));

        GameObject prevBtn = CreateButton(screen,
            "<", SURFACE, TEXT,
            new Vector2(-150, 80),
            new Vector2(36, 36), 22);
        AddBorder(prevBtn, BORDER, 2);
        GetButton(prevBtn).onClick.AddListener(() =>
        {
            saveSlotIndex--;
            if (saveSlotIndex < 0) saveSlotIndex = 2;
            RefreshSlotCard();
        });

        GameObject nextBtn = CreateButton(screen,
            ">", SURFACE, TEXT,
            new Vector2(150, 80),
            new Vector2(36, 36), 22);
        AddBorder(nextBtn, BORDER, 2);
        GetButton(nextBtn).onClick.AddListener(() =>
        {
            saveSlotIndex++;
            if (saveSlotIndex > 2) saveSlotIndex = 0;
            RefreshSlotCard();
        });

        GameObject loadBtn = CreateButton(screen,
            "LOAD THIS SAVE", RED, TEXT,
            new Vector2(0, -185),
            new Vector2(280, 54), 17);
        loadBtn.name = "LoadBtn";
        GetButton(loadBtn).onClick.AddListener(() =>
            OnLoadSave());

        GameObject deleteBtn = CreateButton(screen,
            "DELETE", SURFACE, RED,
            new Vector2(0, -248),
            new Vector2(140, 36), 12);
        AddBorder(deleteBtn, BORDER, 1);
        deleteBtn.name = "DeleteBtn";
        GetButton(deleteBtn).onClick.AddListener(() =>
            OnDeleteSave());

        screen.SetActive(false);
        return screen;
    }

    void PopulateContinueScreen()
    {
        saveSlotIndex = 0;
        GameManager gm =
            FindFirstObjectByType<GameManager>();
        if (gm != null)
            saveInfos = gm.GetAllSaveInfos();
        else
            saveInfos = new List<string> { "", "", "" };

        for (int i = 0; i < saveInfos.Count; i++)
        {
            if (saveInfos[i] != "")
            {
                saveSlotIndex = i;
                break;
            }
        }

        RefreshSlotCard();
    }

    void RefreshSlotCard()
    {
        if (continueScreen == null) return;

        bool hasData =
            saveSlotIndex < saveInfos.Count &&
            saveInfos[saveSlotIndex] != "";

        SetContinueText("SlotNumber",
            "SLOT " + (saveSlotIndex + 1));
        SetContinueText("SlotCounter",
            (saveSlotIndex + 1) + " / 3");

        if (hasData)
        {
            string info    = saveInfos[saveSlotIndex];
            string[] parts = info.Split(
                new string[] { " — " },
                System.StringSplitOptions.None);

            string gm     = parts.Length > 0 ? parts[0] : "";
            string team   = parts.Length > 1 ? parts[1] : "";
            string season = parts.Length > 2 ? parts[2] : "";

            SetContinueText("SlotTeam",   team);
            SetContinueText("SlotGM",     "GM: " + gm);
            SetContinueText("SlotSeason", season);
            SetContinueText("SlotStatus", "");

            Transform loadBtn =
                continueScreen.transform.Find("LoadBtn");
            if (loadBtn != null)
                loadBtn.GetComponent<Button>()
                    .interactable = true;

            Transform deleteBtn =
                continueScreen.transform.Find("DeleteBtn");
            if (deleteBtn != null)
                deleteBtn.gameObject.SetActive(true);
        }
        else
        {
            SetContinueText("SlotTeam",   "");
            SetContinueText("SlotGM",     "");
            SetContinueText("SlotSeason", "");
            SetContinueText("SlotStatus", "EMPTY SLOT");

            Transform loadBtn =
                continueScreen.transform.Find("LoadBtn");
            if (loadBtn != null)
                loadBtn.GetComponent<Button>()
                    .interactable = false;

            Transform deleteBtn =
                continueScreen.transform.Find("DeleteBtn");
            if (deleteBtn != null)
                deleteBtn.gameObject.SetActive(false);
        }
    }

    void OnLoadSave()
    {
        GameManager gm =
            FindFirstObjectByType<GameManager>();
        if (gm == null) return;

        bool loaded = gm.LoadGame(saveSlotIndex);
        if (!loaded) return;

        gm.SetSaveSlot(saveSlotIndex);
        currentSaveSlot = saveSlotIndex;
        selectedTeam    = gm.GetSavedTeamAbbr();

        Team savedTeam = dataLoader.allTeams.Find(
            t => t.abbreviation == selectedTeam);

        if (savedTeam != null)
        {
            ShowScreen(teamScreen);
            PopulateTeamScreen(savedTeam);
        }
    }

    void OnDeleteSave()
    {
        GameManager gm =
            FindFirstObjectByType<GameManager>();
        if (gm == null) return;

        SaveSystem ss =
            FindFirstObjectByType<SaveSystem>();
        if (ss != null) ss.DeleteSave(saveSlotIndex);

        saveInfos = gm.GetAllSaveInfos();
        RefreshSlotCard();
        Debug.Log("Deleted slot " + saveSlotIndex);
    }

    void SetContinueText(string objName, string value)
    {
        if (continueScreen == null) return;
        Transform t =
            continueScreen.transform.Find(objName);
        if (t == null) return;
        TextMeshProUGUI tmp =
            t.GetComponent<TextMeshProUGUI>();
        if (tmp != null) tmp.text = value;
    }

    // -------------------------------------------------------
    // EXIT DIALOG
    // -------------------------------------------------------
    void ShowExitDialog()
    {
        Transform old =
            teamScreen.transform.Find("ExitDialog");
        if (old != null) Destroy(old.gameObject);

        GameObject dialog = new GameObject("ExitDialog");
        dialog.transform.SetParent(
            teamScreen.transform, false);
        RectTransform dRT =
            dialog.AddComponent<RectTransform>();
        dRT.anchoredPosition = Vector2.zero;
        dRT.sizeDelta         = new Vector2(390, 844);
        dialog.AddComponent<Image>().color =
            new Color(0f, 0f, 0f, 0.88f);

        AddText(dialog, "ExitTitle",
            "EXIT GAME", 22, TEXT,
            new Vector2(0, 80),
            new Vector2(300, 44), FontStyles.Bold);

        AddText(dialog, "ExitSub",
            "What would you like to do?",
            14, SUBTEXT,
            new Vector2(0, 40),
            new Vector2(300, 30));

        GameObject saveQuitBtn = CreateButton(dialog,
            "SAVE AND QUIT", GREEN, BG,
            new Vector2(0, -20),
            new Vector2(280, 56), 16);
        GetButton(saveQuitBtn).onClick.AddListener(() =>
        {
            GameManager gm =
                FindFirstObjectByType<GameManager>();
            if (gm != null)
                gm.SaveGame(currentSaveSlot);
            Destroy(dialog);
            ShowScreen(mainMenuScreen);
        });

        GameObject quitBtn = CreateButton(dialog,
            "QUIT WITHOUT SAVING", SURFACE, RED,
            new Vector2(0, -88),
            new Vector2(280, 56), 14);
        AddBorder(quitBtn, BORDER, 2);
        GetButton(quitBtn).onClick.AddListener(() =>
        {
            Destroy(dialog);
            ShowScreen(mainMenuScreen);
        });

        GameObject cancelBtn = CreateButton(dialog,
            "CANCEL", SURFACE, SUBTEXT,
            new Vector2(0, -155),
            new Vector2(180, 44), 13);
        AddBorder(cancelBtn, BORDER, 2);
        GetButton(cancelBtn).onClick.AddListener(() =>
            Destroy(dialog));
    }

    // -------------------------------------------------------
    // GM NAME SCREEN
    // -------------------------------------------------------
    GameObject BuildGMNameScreen(GameObject canvas)
    {
        GameObject screen = CreateScreen(canvas, "GMName");

        BuildImageBackground(screen, "background");
        AddImage(screen, "Overlay",
            new Color(0.03f, 0.05f, 0.10f, 0.85f),
            Vector2.zero, new Vector2(390, 844));

        AddImage(screen, "Header", SURFACE,
            new Vector2(0, 380), new Vector2(390, 88));

        GameObject backBtn = CreateButton(screen,
            "BACK", SURFACE, GOLD,
            new Vector2(-140, 380),
            new Vector2(80, 44), 14);
        GetButton(backBtn).onClick.AddListener(() =>
            ShowScreen(mainMenuScreen));

        AddText(screen, "Title",
            "YOUR GM NAME", 22, TEXT,
            new Vector2(0, 380),
            new Vector2(300, 44), FontStyles.Bold);

        AddText(screen, "Sub",
            "What should the league call you?",
            15, SUBTEXT, new Vector2(0, 260),
            new Vector2(320, 30));

        AddImage(screen, "InputBG", SURFACE,
            new Vector2(0, 180), new Vector2(320, 62));

        GameObject inputObj  = new GameObject("NameInput");
        inputObj.transform.SetParent(screen.transform, false);
        RectTransform rt     =
            inputObj.AddComponent<RectTransform>();
        rt.anchoredPosition   = new Vector2(0, 180);
        rt.sizeDelta          = new Vector2(300, 56);
        TMP_InputField input  =
            inputObj.AddComponent<TMP_InputField>();

        GameObject ph         = new GameObject("Placeholder");
        ph.transform.SetParent(inputObj.transform, false);
        TextMeshProUGUI phT   =
            ph.AddComponent<TextMeshProUGUI>();
        phT.text              = "Enter your name...";
        phT.color             = SUBTEXT;
        phT.fontSize          = 18;
        phT.alignment         = TextAlignmentOptions.Center;
        RectTransform phRT    =
            ph.GetComponent<RectTransform>();
        phRT.anchorMin         = Vector2.zero;
        phRT.anchorMax         = Vector2.one;
        phRT.offsetMin         = Vector2.zero;
        phRT.offsetMax         = Vector2.zero;

        GameObject txt        = new GameObject("Text");
        txt.transform.SetParent(inputObj.transform, false);
        TextMeshProUGUI txtT  =
            txt.AddComponent<TextMeshProUGUI>();
        txtT.color            = TEXT;
        txtT.fontSize         = 18;
        txtT.alignment        = TextAlignmentOptions.Center;
        RectTransform txtRT   =
            txt.GetComponent<RectTransform>();
        txtRT.anchorMin        = Vector2.zero;
        txtRT.anchorMax        = Vector2.one;
        txtRT.offsetMin        = Vector2.zero;
        txtRT.offsetMax        = Vector2.zero;

        input.textComponent   = txtT;
        input.placeholder     = phT;
        input.characterLimit  = 20;

        GameObject confirmBtn = CreateButton(screen,
            "LETS PLAY BALL", RED, TEXT,
            new Vector2(0, 60),
            new Vector2(300, 62), 20);
        GetButton(confirmBtn).onClick.AddListener(() =>
        {
            if (input.text.Length > 0)
            {
                gmName = input.text;
                ShowScreen(teamSelectScreen);
                PopulateTeamSelect();
            }
            else
            {
                Transform inputBG =
                    screen.transform.Find("InputBG");
                if (inputBG != null)
                    inputBG.GetComponent<Image>().color = RED;
            }
        });

        AddText(screen, "DiffLabel",
            "DIFFICULTY", 13, GOLD,
            new Vector2(0, -40),
            new Vector2(300, 28));

        string[] diffs    = { "EASY", "NORMAL", "HARD" };
        Color[]  diffCols = { GREEN, GOLD, RED };
        float[]  xPos     = { -105f, 0f, 105f };

        for (int i = 0; i < 3; i++)
        {
            GameObject db = CreateButton(screen,
                diffs[i], SURFACE, diffCols[i],
                new Vector2(xPos[i], -100),
                new Vector2(90, 44), 14);
            AddBorder(db, diffCols[i], 2);
        }

        screen.SetActive(false);
        return screen;
    }

    // -------------------------------------------------------
    // TEAM SELECT SCREEN
    // -------------------------------------------------------
    GameObject BuildTeamSelectScreen(GameObject canvas)
    {
        GameObject screen =
            CreateScreen(canvas, "TeamSelect");

        BuildImageBackground(screen, "background");
        AddImage(screen, "Overlay",
            new Color(0.03f, 0.05f, 0.10f, 0.88f),
            Vector2.zero, new Vector2(390, 844));

        AddImage(screen, "Header",
            new Color(0.06f, 0.12f, 0.20f, 1f),
            new Vector2(0, 370), new Vector2(390, 100));

        AddImage(screen, "HeaderBorder", RED,
            new Vector2(0, 322), new Vector2(390, 3));

        AddText(screen, "Title",
            "SELECT YOUR TEAM",
            24, TEXT, new Vector2(0, 378),
            new Vector2(340, 44), FontStyles.Bold);

        AddText(screen, "Sub",
            "Choose wisely, GM",
            13, GOLD, new Vector2(0, 350),
            new Vector2(300, 24));

        GameObject backBtn = CreateButton(screen,
            "BACK", SURFACE, GOLD,
            new Vector2(-140, 378),
            new Vector2(80, 44), 14);
        GetButton(backBtn).onClick.AddListener(() =>
            ShowScreen(gmNameScreen));

        AddText(screen, "DivLabel",
            "", 13, GOLD,
            new Vector2(0, 295),
            new Vector2(340, 28));

        AddImage(screen, "TeamCard",
            new Color(0.06f, 0.12f, 0.20f, 0.97f),
            new Vector2(0, 100), new Vector2(340, 280));

        AddImage(screen, "CardTop", RED,
            new Vector2(0, 238), new Vector2(340, 5));

        AddText(screen, "TeamAbbr",
            "", 64, RED,
            new Vector2(0, 155),
            new Vector2(340, 90), FontStyles.Bold);

        AddText(screen, "TeamCity",
            "", 18, SUBTEXT,
            new Vector2(0, 85),
            new Vector2(320, 32));

        AddText(screen, "TeamNick",
            "", 28, TEXT,
            new Vector2(0, 50),
            new Vector2(320, 42), FontStyles.Bold);

        AddText(screen, "TeamDiv",
            "", 13, SUBTEXT,
            new Vector2(0, 10),
            new Vector2(320, 28));

        AddText(screen, "TeamBudget",
            "", 16, GOLD,
            new Vector2(0, -30),
            new Vector2(320, 30));

        AddText(screen, "Counter",
            "1 / 30", 13, SUBTEXT,
            new Vector2(0, -115),
            new Vector2(200, 28));

        GameObject prevBtn = CreateButton(screen,
            "<", SURFACE, TEXT,
            new Vector2(-150, 100),
            new Vector2(36, 36), 22);
        AddBorder(prevBtn, BORDER, 2);
        GetButton(prevBtn).onClick.AddListener(() =>
        {
            currentTeamIndex--;
            if (currentTeamIndex < 0)
                currentTeamIndex = allTeamsList.Count - 1;
            RefreshTeamCard();
        });

        GameObject nextBtn = CreateButton(screen,
            ">", SURFACE, TEXT,
            new Vector2(150, 100),
            new Vector2(36, 36), 22);
        AddBorder(nextBtn, BORDER, 2);
        GetButton(nextBtn).onClick.AddListener(() =>
        {
            currentTeamIndex++;
            if (currentTeamIndex >= allTeamsList.Count)
                currentTeamIndex = 0;
            RefreshTeamCard();
        });

        GameObject selectBtn = CreateButton(screen,
            "SELECT THIS TEAM", RED, TEXT,
            new Vector2(0, -190),
            new Vector2(300, 62), 18);
        GetButton(selectBtn).onClick.AddListener(() =>
        {
            if (allTeamsList.Count > 0)
            {
                Team chosen  = allTeamsList[currentTeamIndex];
                selectedTeam = chosen.abbreviation;

                Team freshTeam = dataLoader.allTeams.Find(
                    t => t.abbreviation ==
                         chosen.abbreviation);
                if (freshTeam == null) return;

                GameManager gm =
                    FindFirstObjectByType<GameManager>();
                if (gm != null)
                {
                    gm.StartFranchise(
                        chosen.abbreviation, gmName);

                    for (int s = 0; s < 3; s++)
                    {
                        if (!gm.HasSaveFile(s))
                        {
                            currentSaveSlot = s;
                            gm.SetSaveSlot(s);
                            break;
                        }
                    }

                    gm.SaveGame(currentSaveSlot);
                }

                ShowScreen(teamScreen);
                PopulateTeamScreen(freshTeam);
            }
        });

        screen.SetActive(false);
        return screen;
    }

    void PopulateTeamSelect()
    {
        if (dataLoader == null) return;
        if (dataLoader.allTeams == null) return;

        allTeamsList.Clear();
        string[] divisions = new string[]
        {
            "AL East", "AL Central", "AL West",
            "NL East", "NL Central", "NL West"
        };

        foreach (string div in divisions)
        {
            List<Team> divTeams = dataLoader.allTeams
                .FindAll(t => t.division == div);
            allTeamsList.AddRange(divTeams);
        }

        currentTeamIndex = 0;
        RefreshTeamCard();
    }

    void RefreshTeamCard()
    {
        if (allTeamsList.Count == 0) return;
        Team t = allTeamsList[currentTeamIndex];
        SetText("DivLabel",   t.division.ToUpper());
        SetText("TeamAbbr",   t.abbreviation);
        SetText("TeamCity",   t.city.ToUpper());
        SetText("TeamNick",   t.nickname.ToUpper());
        SetText("TeamDiv",    t.league + " — " + t.division);
        SetText("TeamBudget", "BUDGET: $" + t.budget + "M");
        SetText("Counter",
            (currentTeamIndex + 1) + " / " +
            allTeamsList.Count);
    }

    void SetText(string objName, string value)
    {
        Transform t =
            teamSelectScreen.transform.Find(objName);
        if (t == null) return;
        TextMeshProUGUI tmp =
            t.GetComponent<TextMeshProUGUI>();
        if (tmp != null) tmp.text = value;
    }

    // -------------------------------------------------------
    // TEAM SCREEN
    // -------------------------------------------------------
    GameObject BuildTeamScreen(GameObject canvas)
    {
        GameObject screen =
            CreateScreen(canvas, "TeamScreen");

        BuildImageBackground(screen, "background");
        AddImage(screen, "Overlay",
            new Color(0.03f, 0.05f, 0.10f, 0.88f),
            Vector2.zero, new Vector2(390, 844));

        AddImage(screen, "Header", SURFACE,
            new Vector2(0, 380), new Vector2(390, 88));

        // EXIT button
        GameObject exitBtn = CreateButton(screen,
            "EXIT", SURFACE, RED,
            new Vector2(-150, 380),
            new Vector2(56, 36), 11);
        AddBorder(exitBtn, BORDER, 2);
        GetButton(exitBtn).onClick.AddListener(() =>
            ShowExitDialog());
        exitBtn.name = "ExitBtn";

        AddText(screen, "TeamName",
            "TEAM NAME", 20, TEXT,
            new Vector2(20, 385),
            new Vector2(220, 44), FontStyles.Bold);

        AddText(screen, "Record",
            "0-0", 13, GOLD,
            new Vector2(20, 357),
            new Vector2(200, 24));

        AddImage(screen, "TabBar", SURFACE,
            new Vector2(0, 313), new Vector2(390, 40));

        string[] tabs = { "BATTERS", "ROTATION", "BULLPEN" };
        float[]  tabX = { -120f, 0f, 120f };
        for (int i = 0; i < tabs.Length; i++)
        {
            int idx = i;
            GameObject tab = CreateButton(screen,
                tabs[i], SURFACE, SUBTEXT,
                new Vector2(tabX[i], 313),
                new Vector2(110, 40), 11);
            GetButton(tab).onClick.AddListener(() =>
                OnTabSelected(idx));
            tab.name = "Tab_" + tabs[i];
        }

        AddImage(screen, "TabLine", RED,
            new Vector2(-120, 294), new Vector2(110, 3));

        AddImage(screen, "PlayerCard",
            new Color(0.06f, 0.12f, 0.20f, 0.97f),
            new Vector2(0, 90), new Vector2(340, 300));

        AddImage(screen, "CardTop", RED,
            new Vector2(0, 238), new Vector2(340, 4));

        AddImage(screen, "PosBadge", RED,
            new Vector2(-130, 210), new Vector2(60, 28));
        AddText(screen, "PlayerPos",
            "", 13, TEXT,
            new Vector2(-130, 210),
            new Vector2(60, 28), FontStyles.Bold);

        AddImage(screen, "OvrBadge", SURFACE,
            new Vector2(130, 210), new Vector2(60, 28));
        AddText(screen, "PlayerOvr",
            "", 13, GOLD,
            new Vector2(130, 210),
            new Vector2(60, 28), FontStyles.Bold);

        AddText(screen, "PlayerName",
            "", 22, TEXT,
            new Vector2(0, 160),
            new Vector2(320, 44), FontStyles.Bold);

        AddImage(screen, "StatsBar1", BORDER,
            new Vector2(0, 110), new Vector2(320, 1));

        AddText(screen, "Stat1Label", "AGE",
            10, SUBTEXT, new Vector2(-100, 90),
            new Vector2(80, 24));
        AddText(screen, "Stat1Val", "",
            16, TEXT, new Vector2(-100, 68),
            new Vector2(80, 28), FontStyles.Bold);

        AddText(screen, "Stat2Label", "HAND",
            10, SUBTEXT, new Vector2(0, 90),
            new Vector2(80, 24));
        AddText(screen, "Stat2Val", "",
            16, TEXT, new Vector2(0, 68),
            new Vector2(80, 28), FontStyles.Bold);

        AddText(screen, "Stat3Label", "SALARY",
            10, SUBTEXT, new Vector2(100, 90),
            new Vector2(80, 24));
        AddText(screen, "Stat3Val", "",
            16, TEXT, new Vector2(100, 68),
            new Vector2(80, 28), FontStyles.Bold);

        AddImage(screen, "StatsBar2", BORDER,
            new Vector2(0, 50), new Vector2(320, 1));

        AddText(screen, "Attr1Label", "", 10, SUBTEXT,
            new Vector2(-130, 30),  new Vector2(80, 20));
        AddText(screen, "Attr1Val",   "", 10, GOLD,
            new Vector2(130, 30),   new Vector2(60, 20));

        AddText(screen, "Attr2Label", "", 10, SUBTEXT,
            new Vector2(-130, 10),  new Vector2(80, 20));
        AddText(screen, "Attr2Val",   "", 10, GOLD,
            new Vector2(130, 10),   new Vector2(60, 20));

        AddText(screen, "Attr3Label", "", 10, SUBTEXT,
            new Vector2(-130, -10), new Vector2(80, 20));
        AddText(screen, "Attr3Val",   "", 10, GOLD,
            new Vector2(130, -10),  new Vector2(60, 20));

        AddText(screen, "Attr4Label", "", 10, SUBTEXT,
            new Vector2(-130, -30), new Vector2(80, 20));
        AddText(screen, "Attr4Val",   "", 10, GOLD,
            new Vector2(130, -30),  new Vector2(60, 20));

        AddText(screen, "Attr5Label", "", 10, SUBTEXT,
            new Vector2(-130, -50), new Vector2(80, 20));
        AddText(screen, "Attr5Val",   "", 10, GOLD,
            new Vector2(130, -50),  new Vector2(60, 20));

        AddText(screen, "InjuryBadge",
            "", 11, RED,
            new Vector2(0, -75),
            new Vector2(300, 24));

        AddText(screen, "PlayerCounter",
            "1 / 26", 12, SUBTEXT,
            new Vector2(0, -120),
            new Vector2(200, 24));

        GameObject prevBtn = CreateButton(screen,
            "<", SURFACE, TEXT,
            new Vector2(-150, 90),
            new Vector2(36, 36), 22);
        AddBorder(prevBtn, BORDER, 2);
        GetButton(prevBtn).onClick.AddListener(() =>
        {
            rosterIndex--;
            if (rosterIndex < 0)
                rosterIndex = currentRoster.Count - 1;
            RefreshPlayerCard();
        });

        GameObject nextBtn = CreateButton(screen,
            ">", SURFACE, TEXT,
            new Vector2(150, 90),
            new Vector2(36, 36), 22);
        AddBorder(nextBtn, BORDER, 2);
        GetButton(nextBtn).onClick.AddListener(() =>
        {
            rosterIndex++;
            if (rosterIndex >= currentRoster.Count)
                rosterIndex = 0;
            RefreshPlayerCard();
        });

        GameObject simBtn = CreateButton(screen,
            "SIMULATE SEASON", GREEN, BG,
            new Vector2(0, -270),
            new Vector2(200, 44), 14);
        GetButton(simBtn).onClick.AddListener(() =>
            OnSimulateSeason());
        simBtn.name = "SimBtn";

        GameObject liveBtn = CreateButton(screen,
            "PLAY LIVE GAME", RED, TEXT,
            new Vector2(0, -320),
            new Vector2(200, 44), 14);
        GetButton(liveBtn).onClick.AddListener(() =>
            OnPlayLiveGame());

        AddText(screen, "RosterHint",
            "Select a team to view roster",
            15, SUBTEXT, new Vector2(0, 0),
            new Vector2(300, 40));

        BuildBottomNav(screen);
        screen.SetActive(false);
        return screen;
    }

    void OnTabSelected(int tab)
    {
        currentTab = tab;

        float[] tabX = { -120f, 0f, 120f };
        Transform tabLine =
            teamScreen.transform.Find("TabLine");
        if (tabLine != null)
        {
            RectTransform rt =
                tabLine.GetComponent<RectTransform>();
            rt.anchoredPosition =
                new Vector2(tabX[tab], 294);
        }

        string[] tabs = { "BATTERS", "ROTATION", "BULLPEN" };
        for (int i = 0; i < tabs.Length; i++)
        {
            Transform t = teamScreen.transform
                .Find("Tab_" + tabs[i]);
            if (t == null) continue;
            TextMeshProUGUI txt =
                t.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null)
                txt.color = i == tab ? GOLD : SUBTEXT;
        }

        if (currentTeam != null)
            PopulateTeamScreen(currentTeam);
    }

    void PopulateTeamScreen(Team team)
    {
        if (team == null) return;
        currentTeam = team;

        SetTeamText("TeamName",
            team.city.ToUpper() + " " +
            team.nickname.ToUpper());
        SetTeamText("Record",
            team.wins + " - " + team.losses);

        Transform hint =
            teamScreen.transform.Find("RosterHint");
        if (hint != null) hint.gameObject.SetActive(false);

        currentRoster.Clear();
        if (currentTab == 0)
            currentRoster = team.roster.FindAll(p =>
                p.position == "C"  || p.position == "1B" ||
                p.position == "2B" || p.position == "3B" ||
                p.position == "SS" || p.position == "LF" ||
                p.position == "CF" || p.position == "RF" ||
                p.position == "DH");
        else if (currentTab == 1)
            currentRoster = team.roster.FindAll(p =>
                p.position == "SP");
        else if (currentTab == 2)
            currentRoster = team.roster.FindAll(p =>
                p.position == "RP");

        rosterIndex = 0;
        RefreshPlayerCard();
    }

    void RefreshPlayerCard()
    {
        if (currentRoster == null ||
            currentRoster.Count == 0)
        {
            SetTeamText("PlayerName",    "No players");
            SetTeamText("PlayerCounter", "0 / 0");
            return;
        }

        Player p       = currentRoster[rosterIndex];
        bool isPitcher = p.position == "SP" ||
                         p.position == "RP";

        SetTeamText("PlayerPos",  p.position);
        SetTeamText("PlayerOvr",  "OVR " + p.overall);
        SetTeamText("PlayerName", p.FullName());
        SetTeamText("Stat1Val",   p.age.ToString());
        SetTeamText("Stat2Val",
            isPitcher ? p.throwingArm + "HP" :
                        p.battingHand + "HB");
        SetTeamText("Stat3Val",
            "$" + p.salary.ToString("F1") + "M");

        if (isPitcher)
        {
            // Show season stats if pitcher has thrown
            if (p.seasonInningsPitched > 0)
            {
                SetTeamText("Attr1Label", "SZN ERA");
                SetTeamText("Attr1Val",
                    p.SeasonERA().ToString("F2"));
                SetTeamText("Attr2Label", "SZN K");
                SetTeamText("Attr2Val",
                    p.seasonStrikeoutsThrown.ToString());
                SetTeamText("Attr3Label", "SZN IP");
                SetTeamText("Attr3Val",
                    p.seasonInningsPitched.ToString());
                SetTeamText("Attr4Label", "CONFIDENCE");
                SetTeamText("Attr4Val",
                    p.confidence.ToString("F0"));
                SetTeamText("Attr5Label", "ROLE");
                SetTeamText("Attr5Val",
                    p.bullpenRole != "" ?
                    p.bullpenRole : p.position);
            }
            else
            {
                SetTeamText("Attr1Label", "PITCHING");
                SetTeamText("Attr1Val",
                    p.pitching.ToString());
                SetTeamText("Attr2Label", "STAMINA");
                SetTeamText("Attr2Val",
                    p.stamina.ToString());
                SetTeamText("Attr3Label", "CONFIDENCE");
                SetTeamText("Attr3Val",
                    p.confidence.ToString("F0"));
                SetTeamText("Attr4Label", "CONTRACT");
                SetTeamText("Attr4Val",
                    p.contractYears + " YRS");
                SetTeamText("Attr5Label", "ROLE");
                SetTeamText("Attr5Val",
                    p.bullpenRole != "" ?
                    p.bullpenRole : p.position);
            }
        }
        else
        {
            // Show season stats if batter has ABs
            if (p.seasonAtBats > 0)
            {
                SetTeamText("Attr1Label", "SZN AVG");
                SetTeamText("Attr1Val",
                    p.SeasonBattingAverage()
                     .ToString("F3"));
                SetTeamText("Attr2Label", "SZN HR");
                SetTeamText("Attr2Val",
                    p.seasonHomeRuns.ToString());
                SetTeamText("Attr3Label", "SZN RBI");
                SetTeamText("Attr3Val",
                    p.seasonRbi.ToString());
                SetTeamText("Attr4Label", "SZN OPS");
                SetTeamText("Attr4Val",
                    p.SeasonOPS().ToString("F3"));
                SetTeamText("Attr5Label", "SZN AB");
                SetTeamText("Attr5Val",
                    p.seasonAtBats.ToString());
            }
            else
            {
                SetTeamText("Attr1Label", "CONTACT");
                SetTeamText("Attr1Val",
                    p.contact.ToString());
                SetTeamText("Attr2Label", "POWER");
                SetTeamText("Attr2Val",
                    p.power.ToString());
                SetTeamText("Attr3Label", "SPEED");
                SetTeamText("Attr3Val",
                    p.speed.ToString());
                SetTeamText("Attr4Label", "FIELDING");
                SetTeamText("Attr4Val",
                    p.fielding.ToString());
                SetTeamText("Attr5Label", "ARM");
                SetTeamText("Attr5Val",
                    p.arm.ToString());
            }
        }

        SetTeamText("InjuryBadge",
            p.isInjured
                ? "INJURED: " + p.injuryType +
                  " — " + p.injuryStatus
                : "");

        string contractInfo = p.contractYears > 0
            ? p.contractYears + "yr left" : "FREE AGENT";
        string ageTrend = p.age <= 25 ? " UP" :
                          p.age >= 33 ? " DOWN" : "";

        SetTeamText("PlayerCounter",
            (rosterIndex + 1) + " / " +
            currentRoster.Count +
            "   |   " + contractInfo + ageTrend);
    }

    void SetTeamText(string objName, string value)
    {
        Transform t = teamScreen.transform.Find(objName);
        if (t == null) return;
        TextMeshProUGUI tmp =
            t.GetComponent<TextMeshProUGUI>();
        if (tmp != null) tmp.text = value;
    }

    void OnSimulateSeason()
    {
        Transform simBtn =
            teamScreen.transform.Find("SimBtn");
        if (simBtn != null)
        {
            Button btn = simBtn.GetComponent<Button>();
            if (btn != null) btn.interactable = false;
            TextMeshProUGUI btnText =
                simBtn.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null)
                btnText.text = "SIMULATING...";
        }

        GameManager gm =
            FindFirstObjectByType<GameManager>();
        if (gm != null) gm.SimulateOneSeason();

        if (currentTeam != null)
            PopulateTeamScreen(currentTeam);

        if (simBtn != null)
        {
            Button btn = simBtn.GetComponent<Button>();
            if (btn != null) btn.interactable = true;
            TextMeshProUGUI btnText =
                simBtn.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null)
                btnText.text = "NEXT SEASON";
        }

        ShowScreen(standingsScreen);

        Transform sub =
            standingsScreen.transform.Find("Sub");
        if (sub != null)
        {
            TextMeshProUGUI subTmp =
                sub.GetComponent<TextMeshProUGUI>();
            if (subTmp != null)
            {
                GameManager gm2 =
                    FindFirstObjectByType<GameManager>();
                if (gm2 != null)
                    subTmp.text =
                        (gm2.GetCurrentSeason() - 1) +
                        " SEASON RESULTS";
            }
        }

        ShowDivision(GetPlayerDivisionIndex());

        draftStarted = false;
        draftRound   = 1;
        draftClass.Clear();

        SetStandingsNotification(
            "Draft is now open! Tap DRAFT to pick.");
    }

    void SetStandingsNotification(string msg)
    {
        Transform note =
            standingsScreen.transform.Find("PlayoffNote");
        if (note != null)
        {
            TextMeshProUGUI tmp =
                note.GetComponent<TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.text  = msg;
                tmp.color = GOLD;
            }
        }
    }

    int GetPlayerDivisionIndex()
    {
        if (selectedTeam == "") return 0;
        Team t = dataLoader.allTeams.Find(
            tm => tm.abbreviation == selectedTeam);
        if (t == null) return 0;
        for (int i = 0; i < divisionNames.Length; i++)
            if (divisionNames[i] == t.division) return i;
        return 0;
    }

      void OnPlayLiveGame()
    {
        Team myTeam = GetMyTeam();
        if (myTeam == null) return;

        List<Team> opponents = dataLoader.allTeams.FindAll(
            t => t.abbreviation != selectedTeam);
        if (opponents.Count == 0) return;

        Team opponent =
            opponents[Random.Range(0, opponents.Count)];

        // Show pre-game screen first
        ShowPreGame(myTeam, opponent);
    }

    // -------------------------------------------------------
    // STANDINGS SCREEN
    // -------------------------------------------------------
    GameObject BuildStandingsScreen(GameObject canvas)
    {
        GameObject screen = CreateScreen(canvas, "Standings");

        BuildImageBackground(screen, "background");
        AddImage(screen, "Overlay",
            new Color(0.03f, 0.05f, 0.10f, 0.92f),
            Vector2.zero, new Vector2(390, 844));

        AddImage(screen, "Header", SURFACE,
            new Vector2(0, 380), new Vector2(390, 88));

        AddText(screen, "Title",
            "STANDINGS", 24, TEXT,
            new Vector2(0, 385),
            new Vector2(300, 44), FontStyles.Bold);

        AddText(screen, "Sub",
            "2026 SEASON", 13, GOLD,
            new Vector2(0, 355),
            new Vector2(300, 24));

        AddImage(screen, "DivTabBar", SURFACE,
            new Vector2(0, 313), new Vector2(390, 40));

        string[] divTabs = {
            "ALE","ALC","ALW","NLE","NLC","NLW" };
        float startX = -155f;
        float tabW   = 62f;

        for (int i = 0; i < divTabs.Length; i++)
        {
            int idx = i;
            GameObject tab = CreateButton(screen,
                divTabs[i], SURFACE, SUBTEXT,
                new Vector2(startX + (i * tabW), 313),
                new Vector2(58, 40), 10);
            GetButton(tab).onClick.AddListener(() =>
                ShowDivision(idx));
            tab.name = "DivTab_" + i;
        }

        AddImage(screen, "DivTabLine", RED,
            new Vector2(startX, 294),
            new Vector2(58, 3));

        AddImage(screen, "ColHeaders", BORDER,
            new Vector2(0, 275), new Vector2(374, 26));

        AddText(screen, "ColTeam", "TEAM", 10, GOLD,
            new Vector2(-120, 275), new Vector2(120, 26));
        AddText(screen, "ColW",   "W",   10, GOLD,
            new Vector2(40, 275),  new Vector2(40, 26));
        AddText(screen, "ColL",   "L",   10, GOLD,
            new Vector2(80, 275),  new Vector2(40, 26));
        AddText(screen, "ColPCT", "PCT", 10, GOLD,
            new Vector2(125, 275), new Vector2(50, 26));
        AddText(screen, "ColGB",  "GB",  10, GOLD,
            new Vector2(165, 275), new Vector2(40, 26));

        for (int i = 0; i < 5; i++)
        {
            float rowY = 245f - (i * 46f);
            Color rowColor = i % 2 == 0
                ? new Color(0.05f, 0.10f, 0.18f, 0.97f)
                : new Color(0.04f, 0.08f, 0.15f, 0.97f);

            GameObject row =
                new GameObject("StandRow_" + i);
            row.transform.SetParent(screen.transform, false);
            RectTransform rRT =
                row.AddComponent<RectTransform>();
            rRT.anchoredPosition = new Vector2(0, rowY);
            rRT.sizeDelta        = new Vector2(374, 44);
            row.AddComponent<Image>().color = rowColor;

            if (i == 2)
                AddImage(screen, "WCLine", RED,
                    new Vector2(0, rowY - 22),
                    new Vector2(374, 1));

            AddTextToParent(row, "RowTeam", "",
                12f, TEXT, new Vector2(-70f, 0f),
                new Vector2(180f, 44f),
                TextAlignmentOptions.MidlineLeft);

            AddTextToParent(row, "RowW", "",
                13f, TEXT, new Vector2(40f, 0f),
                new Vector2(40f, 44f),
                TextAlignmentOptions.Midline);

            AddTextToParent(row, "RowL", "",
                13f, TEXT, new Vector2(80f, 0f),
                new Vector2(40f, 44f),
                TextAlignmentOptions.Midline);

            AddTextToParent(row, "RowPCT", "",
                13f, TEXT, new Vector2(125f, 0f),
                new Vector2(50f, 44f),
                TextAlignmentOptions.Midline);

            AddTextToParent(row, "RowGB", "",
                13f, SUBTEXT, new Vector2(165f, 0f),
                new Vector2(40f, 44f),
                TextAlignmentOptions.Midline);
        }

        AddText(screen, "PlayoffNote",
            "— Wild Card Line —",
            10, RED, new Vector2(0, 18),
            new Vector2(300, 20));

        BuildBottomNav(screen);
        screen.SetActive(false);
        return screen;
    }

    void ShowDivision(int divIndex)
    {
        currentDivision = divIndex;

        float startX = -155f;
        float tabW   = 62f;
        Transform tabLine =
            standingsScreen.transform.Find("DivTabLine");
        if (tabLine != null)
        {
            RectTransform rt =
                tabLine.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(
                startX + (divIndex * tabW), 294);
        }

        for (int i = 0; i < 6; i++)
        {
            Transform t = standingsScreen.transform
                .Find("DivTab_" + i);
            if (t == null) continue;
            TextMeshProUGUI txt =
                t.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null)
                txt.color = i == divIndex ? GOLD : SUBTEXT;
        }

        string divName = divisionNames[divIndex];
        GameManager gm =
            FindFirstObjectByType<GameManager>();

        List<Team> divTeams = dataLoader.allTeams
            .FindAll(t => t.division == divName);

        if (gm != null && gm.finalWins.Count > 0)
        {
            foreach (Team t in divTeams)
            {
                if (gm.finalWins.ContainsKey(t.abbreviation))
                    t.wins = gm.finalWins[t.abbreviation];
                if (gm.finalLosses.ContainsKey(t.abbreviation))
                    t.losses = gm.finalLosses[t.abbreviation];
            }
        }

        divTeams = divTeams
            .OrderByDescending(t => t.wins)
            .ThenBy(t => t.losses)
            .ToList();

        float leaderWins   = divTeams.Count > 0
            ? divTeams[0].wins : 0;
        float leaderLosses = divTeams.Count > 0
            ? divTeams[0].losses : 0;

        for (int i = 0; i < 5; i++)
        {
            Transform row = standingsScreen.transform
                .Find("StandRow_" + i);
            if (row == null) continue;

            if (i >= divTeams.Count)
            {
                SetStandText(row, "RowTeam", "");
                SetStandText(row, "RowW",    "");
                SetStandText(row, "RowL",    "");
                SetStandText(row, "RowPCT",  "");
                SetStandText(row, "RowGB",   "");
                continue;
            }

            Team t     = divTeams[i];
            float pct  = (t.wins + t.losses) > 0
                ? (float)t.wins /
                  (t.wins + t.losses) : 0f;
            float gb   = ((leaderWins - t.wins) +
                          (t.losses - leaderLosses)) / 2f;
            string gbStr = gb == 0 ? "--" :
                           gb.ToString("F1");

            bool isPlayerTeam =
                selectedTeam == t.abbreviation;

            Transform teamTxt = row.Find("RowTeam");
            if (teamTxt != null)
            {
                TextMeshProUGUI tmp =
                    teamTxt.GetComponent<TextMeshProUGUI>();
                if (tmp != null)
                {
                    tmp.text  = t.city + " " + t.nickname;
                    tmp.color = isPlayerTeam ? GOLD : TEXT;
                }
            }

            SetStandText(row, "RowW",   t.wins.ToString());
            SetStandText(row, "RowL",   t.losses.ToString());
            SetStandText(row, "RowPCT", pct.ToString("F3"));
            SetStandText(row, "RowGB",  gbStr);
        }
    }

    void SetStandText(Transform row, string name,
                       string value)
    {
        Transform t = row.Find(name);
        if (t == null) return;
        TextMeshProUGUI tmp =
            t.GetComponent<TextMeshProUGUI>();
        if (tmp != null) tmp.text = value;
    }

    // -------------------------------------------------------
    // TRADE SCREEN
    // -------------------------------------------------------
    GameObject BuildTradeScreen(GameObject canvas)
    {
        GameObject screen = CreateScreen(canvas, "Trade");

        BuildImageBackground(screen, "background");
        AddImage(screen, "Overlay",
            new Color(0.03f, 0.05f, 0.10f, 0.92f),
            Vector2.zero, new Vector2(390, 844));

        AddImage(screen, "Header", SURFACE,
            new Vector2(0, 380), new Vector2(390, 88));

        AddText(screen, "Title",
            "TRADE CENTER", 22, TEXT,
            new Vector2(0, 388),
            new Vector2(300, 36), FontStyles.Bold);

        GameObject threeTeamBtn = CreateButton(screen,
            "3-TEAM: OFF", SURFACE, SUBTEXT,
            new Vector2(140, 360),
            new Vector2(90, 28), 10);
        AddBorder(threeTeamBtn, BORDER, 1);
        threeTeamBtn.name = "ThreeTeamBtn";
        GetButton(threeTeamBtn).onClick.AddListener(() =>
        {
            isThreeTeam = !isThreeTeam;
            Transform t3Panel =
                tradeScreen.transform.Find("Team3Panel");
            if (t3Panel != null)
                t3Panel.gameObject.SetActive(isThreeTeam);

            Transform btn =
                tradeScreen.transform.Find("ThreeTeamBtn");
            if (btn != null)
            {
                TextMeshProUGUI txt =
                    btn.GetComponentInChildren<TextMeshProUGUI>();
                if (txt != null)
                    txt.text = isThreeTeam ?
                        "3-TEAM: ON" : "3-TEAM: OFF";
                btn.GetComponent<Image>().color =
                    isThreeTeam ? RED : SURFACE;
            }
        });

        BuildTradePanel(screen, "MyPanel",
            new Vector2(0, 248), "YOUR OFFER", true);
        BuildTradePanel(screen, "Team2Panel",
            new Vector2(0, 100), "TEAM 2 OFFER", false);
        GameObject t3Panel2 = BuildTradePanel(screen,
            "Team3Panel",
            new Vector2(0, -48), "TEAM 3 OFFER", false);
        t3Panel2.SetActive(false);

        AddImage(screen, "ValueDivider", BORDER,
            new Vector2(0, -118), new Vector2(374, 1));

        AddText(screen, "ValueCompare",
            "Add players to compare value",
            11, SUBTEXT, new Vector2(0, -132),
            new Vector2(340, 22));

        GameObject proposeBtn = CreateButton(screen,
            "PROPOSE TRADE", RED, TEXT,
            new Vector2(0, -168),
            new Vector2(300, 48), 15);
        GetButton(proposeBtn).onClick.AddListener(
            OnProposeMultiTrade);
        proposeBtn.name = "ProposeBtn";

        AddText(screen, "TradeResult",
            "", 12, GREEN,
            new Vector2(0, -218),
            new Vector2(340, 44));

        BuildBottomNav(screen);
        screen.SetActive(false);
        return screen;
    }

    GameObject BuildTradePanel(GameObject parent,
                                string panelName,
                                Vector2 pos,
                                string label,
                                bool isMyTeam)
    {
        GameObject panel = new GameObject(panelName);
        panel.transform.SetParent(parent.transform, false);
        RectTransform pRT =
            panel.AddComponent<RectTransform>();
        pRT.anchoredPosition = pos;
        pRT.sizeDelta         = new Vector2(374, 130);
        panel.AddComponent<Image>().color =
            isMyTeam
                ? new Color(0.15f, 0.05f, 0.05f, 1f)
                : new Color(0.05f, 0.10f, 0.20f, 1f);

        GameObject lbl = new GameObject("PanelLabel");
        lbl.transform.SetParent(panel.transform, false);
        TextMeshProUGUI lblT =
            lbl.AddComponent<TextMeshProUGUI>();
        lblT.text      = label;
        lblT.fontSize  = 10f;
        lblT.color     = SUBTEXT;
        lblT.alignment = TextAlignmentOptions.Center;
        RectTransform lblRT =
            lbl.GetComponent<RectTransform>();
        lblRT.anchoredPosition = new Vector2(0, 55);
        lblRT.sizeDelta         = new Vector2(340, 20);

        if (!isMyTeam)
        {
            GameObject prevT = new GameObject("PrevTeam");
            prevT.transform.SetParent(panel.transform, false);
            RectTransform ptRT =
                prevT.AddComponent<RectTransform>();
            ptRT.anchoredPosition = new Vector2(-160, 42);
            ptRT.sizeDelta         = new Vector2(24, 24);
            prevT.AddComponent<Image>().color = SURFACE;
            Button ptBtn = prevT.AddComponent<Button>();
            GameObject ptTxt = new GameObject("T");
            ptTxt.transform.SetParent(prevT.transform, false);
            TextMeshProUGUI ptT =
                ptTxt.AddComponent<TextMeshProUGUI>();
            ptT.text = "<"; ptT.fontSize = 14f;
            ptT.color = TEXT;
            ptT.alignment = TextAlignmentOptions.Center;
            RectTransform ptTRT =
                ptTxt.GetComponent<RectTransform>();
            ptTRT.anchorMin = Vector2.zero;
            ptTRT.anchorMax = Vector2.one;
            ptTRT.offsetMin = Vector2.zero;
            ptTRT.offsetMax = Vector2.zero;
            string pn = panelName;
            ptBtn.onClick.AddListener(() =>
                CycleTradeTeam(pn, -1));

            GameObject tn = new GameObject("TeamName");
            tn.transform.SetParent(panel.transform, false);
            TextMeshProUGUI tnT =
                tn.AddComponent<TextMeshProUGUI>();
            tnT.text      = "Select Team";
            tnT.fontSize  = 12f; tnT.color = GOLD;
            tnT.fontStyle = FontStyles.Bold;
            tnT.alignment = TextAlignmentOptions.Center;
            RectTransform tnRT =
                tn.GetComponent<RectTransform>();
            tnRT.anchoredPosition = new Vector2(0, 42);
            tnRT.sizeDelta         = new Vector2(220, 24);

            GameObject nextT = new GameObject("NextTeam");
            nextT.transform.SetParent(panel.transform, false);
            RectTransform ntRT =
                nextT.AddComponent<RectTransform>();
            ntRT.anchoredPosition = new Vector2(160, 42);
            ntRT.sizeDelta         = new Vector2(24, 24);
            nextT.AddComponent<Image>().color = SURFACE;
            Button ntBtn = nextT.AddComponent<Button>();
            GameObject ntTxt = new GameObject("T");
            ntTxt.transform.SetParent(nextT.transform, false);
            TextMeshProUGUI ntT =
                ntTxt.AddComponent<TextMeshProUGUI>();
            ntT.text = ">"; ntT.fontSize = 14f;
            ntT.color = TEXT;
            ntT.alignment = TextAlignmentOptions.Center;
            RectTransform ntTRT =
                ntTxt.GetComponent<RectTransform>();
            ntTRT.anchorMin = Vector2.zero;
            ntTRT.anchorMax = Vector2.one;
            ntTRT.offsetMin = Vector2.zero;
            ntTRT.offsetMax = Vector2.zero;
            ntBtn.onClick.AddListener(() =>
                CycleTradeTeam(pn, 1));
        }
        else
        {
            GameObject tn = new GameObject("TeamName");
            tn.transform.SetParent(panel.transform, false);
            TextMeshProUGUI tnT =
                tn.AddComponent<TextMeshProUGUI>();
            tnT.text      = selectedTeam;
            tnT.fontSize  = 12f; tnT.color = GOLD;
            tnT.fontStyle = FontStyles.Bold;
            tnT.alignment = TextAlignmentOptions.Center;
            RectTransform tnRT =
                tn.GetComponent<RectTransform>();
            tnRT.anchoredPosition = new Vector2(0, 42);
            tnRT.sizeDelta         = new Vector2(340, 24);
        }

        float slotStartX  = -115f;
        float slotSpacing = 115f;

        for (int i = 0; i < 3; i++)
        {
            int   slotIdx = i;
            float sx      = slotStartX + (i * slotSpacing);

            GameObject slot = new GameObject("Slot_" + i);
            slot.transform.SetParent(panel.transform, false);
            RectTransform sRT =
                slot.AddComponent<RectTransform>();
            sRT.anchoredPosition = new Vector2(sx, -10);
            sRT.sizeDelta         = new Vector2(105, 70);
            slot.AddComponent<Image>().color =
                new Color(0.04f, 0.08f, 0.14f, 1f);

            Button slotBtn = slot.AddComponent<Button>();
            string pn2     = panelName;
            slotBtn.onClick.AddListener(() =>
                OnSlotTapped(pn2, slotIdx));

            GameObject sName = new GameObject("SlotName");
            sName.transform.SetParent(slot.transform, false);
            TextMeshProUGUI sNameT =
                sName.AddComponent<TextMeshProUGUI>();
            sNameT.text      = "+ ADD";
            sNameT.fontSize  = 10f;
            sNameT.color     = SUBTEXT;
            sNameT.alignment = TextAlignmentOptions.Center;
            RectTransform sNameRT =
                sName.GetComponent<RectTransform>();
            sNameRT.anchorMin = Vector2.zero;
            sNameRT.anchorMax = Vector2.one;
            sNameRT.offsetMin = new Vector2(2, 2);
            sNameRT.offsetMax = new Vector2(-2, -2);

            GameObject sOvr = new GameObject("SlotOvr");
            sOvr.transform.SetParent(slot.transform, false);
            TextMeshProUGUI sOvrT =
                sOvr.AddComponent<TextMeshProUGUI>();
            sOvrT.text      = "";
            sOvrT.fontSize  = 9f; sOvrT.color = GOLD;
            sOvrT.alignment = TextAlignmentOptions.BottomRight;
            RectTransform sOvrRT =
                sOvr.GetComponent<RectTransform>();
            sOvrRT.anchorMin = Vector2.zero;
            sOvrRT.anchorMax = Vector2.one;
            sOvrRT.offsetMin = new Vector2(2, 2);
            sOvrRT.offsetMax = new Vector2(-2, -2);
        }

        return panel;
    }

    void OnSlotTapped(string panelName, int slotIndex)
    {
        List<Player> offerList = GetOfferList(panelName);
        Team team              = GetPanelTeam(panelName);

        if (team == null || team.roster == null) return;

        if (slotIndex < offerList.Count)
        {
            Player removed = offerList[slotIndex];
            offerList.RemoveAt(slotIndex);
            Debug.Log("Removed: " + removed.FullName());
        }
        else
        {
            if (offerList.Count >= 3) return;

            int browse = panelName == "MyPanel"
                ? myBrowseIndex
                : panelName == "Team2Panel"
                    ? t2BrowseIndex : t3BrowseIndex;

            Player toAdd = null;
            for (int i = 0; i < team.roster.Count; i++)
            {
                int    idx = (browse + i) %
                             team.roster.Count;
                Player p   = team.roster[idx];
                if (!offerList.Contains(p))
                {
                    toAdd = p;
                    if (panelName == "MyPanel")
                        myBrowseIndex = (idx + 1) %
                            team.roster.Count;
                    else if (panelName == "Team2Panel")
                        t2BrowseIndex = (idx + 1) %
                            team.roster.Count;
                    else
                        t3BrowseIndex = (idx + 1) %
                            team.roster.Count;
                    break;
                }
            }

            if (toAdd != null) offerList.Add(toAdd);
        }

        RefreshTradePanel(panelName);
        UpdateTradeValue();
    }

    void CycleTradeTeam(string panelName, int dir)
    {
        List<Team> cpuTeams = dataLoader.allTeams.FindAll(
            t => t.abbreviation != selectedTeam);

        if (panelName == "Team2Panel")
        {
            tradeTeam2Index =
                (tradeTeam2Index + dir + cpuTeams.Count) %
                cpuTeams.Count;
            tradeTeam2 = cpuTeams[tradeTeam2Index];
            team2Offer.Clear(); t2BrowseIndex = 0;
        }
        else if (panelName == "Team3Panel")
        {
            tradeTeam3Index =
                (tradeTeam3Index + dir + cpuTeams.Count) %
                cpuTeams.Count;
            tradeTeam3 = cpuTeams[tradeTeam3Index];
            team3Offer.Clear(); t3BrowseIndex = 0;
        }

        RefreshTradePanel(panelName);
        UpdateTradeValue();
    }

    void RefreshTradePanel(string panelName)
    {
        Transform panel =
            tradeScreen.transform.Find(panelName);
        if (panel == null) return;

        Team team = GetPanelTeam(panelName);
        Transform tn = panel.Find("TeamName");
        if (tn != null)
        {
            TextMeshProUGUI tnT =
                tn.GetComponent<TextMeshProUGUI>();
            if (tnT != null)
                tnT.text = team != null
                    ? team.city + " " + team.nickname
                    : "Select Team";
        }

        List<Player> offerList = GetOfferList(panelName);

        for (int i = 0; i < 3; i++)
        {
            Transform slot = panel.Find("Slot_" + i);
            if (slot == null) continue;

            Transform sName = slot.Find("SlotName");
            Transform sOvr  = slot.Find("SlotOvr");
            TextMeshProUGUI nameT =
                sName?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI ovrT  =
                sOvr?.GetComponent<TextMeshProUGUI>();

            if (i < offerList.Count)
            {
                Player p = offerList[i];
                if (nameT != null)
                {
                    nameT.text  = p.FullName();
                    nameT.color = TEXT;
                }
                if (ovrT != null)
                    ovrT.text = p.position + " " + p.overall;
                slot.GetComponent<Image>().color =
                    new Color(0.08f, 0.16f, 0.28f, 1f);
            }
            else
            {
                if (nameT != null)
                {
                    nameT.text  = "+ ADD";
                    nameT.color = SUBTEXT;
                }
                if (ovrT != null) ovrT.text = "";
                slot.GetComponent<Image>().color =
                    new Color(0.04f, 0.08f, 0.14f, 1f);
            }
        }
    }

    void UpdateTradeValue()
    {
        GameManager gm =
            FindFirstObjectByType<GameManager>();
        if (gm == null) return;

        float myVal = myOffer.Count > 0
            ? myOffer.Sum(p => gm.GetTradeValue(p)) : 0;
        float t2Val = team2Offer.Count > 0
            ? team2Offer.Sum(p => gm.GetTradeValue(p)) : 0;
        float t3Val = isThreeTeam && team3Offer.Count > 0
            ? team3Offer.Sum(p => gm.GetTradeValue(p)) : 0;

        string msg = "MY: " + myVal.ToString("F0") +
                     "  T2: " + t2Val.ToString("F0");
        if (isThreeTeam)
            msg += "  T3: " + t3Val.ToString("F0");
        SetTradeText("ValueCompare", msg);
    }

    void OnProposeMultiTrade()
    {
        Team myTeam = GetMyTeam();
        if (myTeam == null || tradeTeam2 == null)
        {
            SetTradeTextColor("TradeResult",
                "Select a team first!", RED);
            return;
        }
        if (myOffer.Count == 0 || team2Offer.Count == 0)
        {
            SetTradeTextColor("TradeResult",
                "Add players to trade!", RED);
            return;
        }

        GameManager gm =
            FindFirstObjectByType<GameManager>();
        if (gm == null) return;

        float myVal   = myOffer.Sum(p => gm.GetTradeValue(p));
        float t2Val   = team2Offer.Sum(p => gm.GetTradeValue(p));
        float ratio   = myVal / Mathf.Max(t2Val, 1f);
        bool accepted =
            (ratio >= 0.82f && ratio <= 1.25f) ||
            Random.value < 0.15f;

        if (accepted)
        {
            foreach (Player p in myOffer)
            {
                myTeam.roster.Remove(p);
                tradeTeam2.roster.Add(p);
                p.team = tradeTeam2.abbreviation;
            }
            foreach (Player p in team2Offer)
            {
                tradeTeam2.roster.Remove(p);
                myTeam.roster.Add(p);
                p.team = myTeam.abbreviation;
            }
            if (isThreeTeam && tradeTeam3 != null &&
                team3Offer.Count > 0)
            {
                foreach (Player p in team3Offer)
                {
                    tradeTeam3.roster.Remove(p);
                    myTeam.roster.Add(p);
                    p.team = myTeam.abbreviation;
                }
            }

            string result = "TRADE ACCEPTED! " +
                myTeam.abbreviation + " receives: " +
                string.Join(", ",
                    team2Offer.ConvertAll(p => p.FullName()));
            if (isThreeTeam && team3Offer.Count > 0)
                result += " + " + string.Join(", ",
                    team3Offer.ConvertAll(p => p.FullName()));

            SetTradeTextColor("TradeResult", result, GREEN);

            myOffer.Clear(); team2Offer.Clear();
            team3Offer.Clear();
            myBrowseIndex = 0; t2BrowseIndex = 0;
            t3BrowseIndex = 0;
            RefreshTradePanel("MyPanel");
            RefreshTradePanel("Team2Panel");
            if (isThreeTeam)
                RefreshTradePanel("Team3Panel");
            UpdateTradeValue();
            PopulateTeamScreen(myTeam);
        }
        else
        {
            float needed = t2Val * 0.85f;
            SetTradeTextColor("TradeResult",
                "REJECTED — Need: " +
                needed.ToString("F0") +
                " (offered: " +
                myVal.ToString("F0") + ")", RED);
        }
    }

    List<Player> GetOfferList(string panelName)
    {
        if (panelName == "MyPanel")    return myOffer;
        if (panelName == "Team2Panel") return team2Offer;
        return team3Offer;
    }

    Team GetPanelTeam(string panelName)
    {
        if (panelName == "MyPanel")    return GetMyTeam();
        if (panelName == "Team2Panel") return tradeTeam2;
        return tradeTeam3;
    }

    Team GetMyTeam()
    {
        if (dataLoader == null) return null;
        return dataLoader.allTeams.Find(
            t => t.abbreviation == selectedTeam);
    }

    void SetTradeText(string objName, string value)
    {
        if (tradeScreen == null) return;
        Transform t = tradeScreen.transform.Find(objName);
        if (t == null) return;
        TextMeshProUGUI tmp =
            t.GetComponent<TextMeshProUGUI>();
        if (tmp != null) tmp.text = value;
    }

    void SetTradeTextColor(string objName,
                            string value, Color color)
    {
        if (tradeScreen == null) return;
        Transform t = tradeScreen.transform.Find(objName);
        if (t == null) return;
        TextMeshProUGUI tmp =
            t.GetComponent<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.text  = value;
            tmp.color = color;
        }
    }

    void InitTradeScreen()
    {
        List<Team> cpuTeams = dataLoader.allTeams.FindAll(
            t => t.abbreviation != selectedTeam);

        if (cpuTeams.Count > 0)
        {
            tradeTeam2      = cpuTeams[0];
            tradeTeam2Index = 0;
        }
        if (cpuTeams.Count > 1)
        {
            tradeTeam3      = cpuTeams[1];
            tradeTeam3Index = 1;
        }

        myOffer.Clear(); team2Offer.Clear();
        team3Offer.Clear();
        myBrowseIndex = 0; t2BrowseIndex = 0;
        t3BrowseIndex = 0;
        isThreeTeam   = false;

        Team myTeam = GetMyTeam();
        Transform myPanel =
            tradeScreen.transform.Find("MyPanel");
        if (myPanel != null)
        {
            Transform tn = myPanel.Find("TeamName");
            if (tn != null)
            {
                TextMeshProUGUI tnT =
                    tn.GetComponent<TextMeshProUGUI>();
                if (tnT != null && myTeam != null)
                    tnT.text = myTeam.city + " " +
                               myTeam.nickname;
            }
        }

        Transform t3Panel =
            tradeScreen.transform.Find("Team3Panel");
        if (t3Panel != null)
            t3Panel.gameObject.SetActive(false);

        Transform threeBtn =
            tradeScreen.transform.Find("ThreeTeamBtn");
        if (threeBtn != null)
        {
            TextMeshProUGUI txt =
                threeBtn.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null) txt.text = "3-TEAM: OFF";
            threeBtn.GetComponent<Image>().color = SURFACE;
        }

        SetTradeText("TradeResult", "");
        SetTradeText("ValueCompare", "Tap slots to add players");
        RefreshTradePanel("MyPanel");
        RefreshTradePanel("Team2Panel");
        RefreshTradePanel("Team3Panel");
    }

    // -------------------------------------------------------
    // DRAFT SCREEN
    // -------------------------------------------------------
    GameObject BuildDraftScreen(GameObject canvas)
    {
        GameObject screen = CreateScreen(canvas, "Draft");

        BuildImageBackground(screen, "background");
        AddImage(screen, "Overlay",
            new Color(0.03f, 0.05f, 0.10f, 0.92f),
            Vector2.zero, new Vector2(390, 844));

        AddImage(screen, "Header", SURFACE,
            new Vector2(0, 380), new Vector2(390, 88));

        AddText(screen, "Title",
            "AMATEUR DRAFT", 22, TEXT,
            new Vector2(0, 388),
            new Vector2(300, 36), FontStyles.Bold);

        AddText(screen, "DraftSub",
            "Round 1 — Pick 1",
            13, GOLD, new Vector2(0, 358),
            new Vector2(300, 24));

        AddImage(screen, "BoardHeader",
            new Color(0.08f, 0.18f, 0.30f, 1f),
            new Vector2(0, 310), new Vector2(374, 36));

        AddText(screen, "BoardLabel",
            "DRAFT BOARD", 12, GOLD,
            new Vector2(0, 310),
            new Vector2(374, 36), FontStyles.Bold);

        AddImage(screen, "ProspectCard",
            new Color(0.06f, 0.12f, 0.20f, 0.97f),
            new Vector2(0, 130), new Vector2(340, 260));

        AddImage(screen, "CardTop", RED,
            new Vector2(0, 258), new Vector2(340, 4));

        AddImage(screen, "RankBadge", RED,
            new Vector2(-130, 240), new Vector2(60, 28));
        AddText(screen, "ProspectRank",
            "#1", 14, TEXT,
            new Vector2(-130, 240),
            new Vector2(60, 28), FontStyles.Bold);

        AddImage(screen, "GradeBadge", SURFACE,
            new Vector2(130, 240), new Vector2(60, 28));
        AddText(screen, "ProspectGrade",
            "A", 14, GOLD,
            new Vector2(130, 240),
            new Vector2(60, 28), FontStyles.Bold);

        AddText(screen, "ProspectName",
            "Loading...", 22, TEXT,
            new Vector2(0, 190),
            new Vector2(320, 44), FontStyles.Bold);

        AddText(screen, "ProspectPos",
            "", 14, SUBTEXT,
            new Vector2(0, 160),
            new Vector2(320, 28));

        AddImage(screen, "CardDiv1", BORDER,
            new Vector2(0, 138), new Vector2(300, 1));

        AddText(screen, "PStat1Label", "OVERALL",
            10, SUBTEXT, new Vector2(-100, 120),
            new Vector2(100, 22));
        AddText(screen, "PStat1Val", "",
            18, GOLD, new Vector2(-100, 98),
            new Vector2(100, 28), FontStyles.Bold);

        AddText(screen, "PStat2Label", "AGE",
            10, SUBTEXT, new Vector2(0, 120),
            new Vector2(80, 22));
        AddText(screen, "PStat2Val", "",
            18, TEXT, new Vector2(0, 98),
            new Vector2(80, 28), FontStyles.Bold);

        AddText(screen, "PStat3Label", "HAND",
            10, SUBTEXT, new Vector2(100, 120),
            new Vector2(80, 22));
        AddText(screen, "PStat3Val", "",
            18, TEXT, new Vector2(100, 98),
            new Vector2(80, 28), FontStyles.Bold);

        AddImage(screen, "CardDiv2", BORDER,
            new Vector2(0, 78), new Vector2(300, 1));

        AddText(screen, "PAttr1Label", "", 10, SUBTEXT,
            new Vector2(-130, 60), new Vector2(100, 20));
        AddText(screen, "PAttr1Val",   "", 10, GOLD,
            new Vector2(130, 60),  new Vector2(60, 20));

        AddText(screen, "PAttr2Label", "", 10, SUBTEXT,
            new Vector2(-130, 40), new Vector2(100, 20));
        AddText(screen, "PAttr2Val",   "", 10, GOLD,
            new Vector2(130, 40),  new Vector2(60, 20));

        AddText(screen, "PAttr3Label", "", 10, SUBTEXT,
            new Vector2(-130, 20), new Vector2(100, 20));
        AddText(screen, "PAttr3Val",   "", 10, GOLD,
            new Vector2(130, 20),  new Vector2(60, 20));

        AddText(screen, "PAttr4Label", "", 10, SUBTEXT,
            new Vector2(-130, 0),  new Vector2(100, 20));
        AddText(screen, "PAttr4Val",   "", 10, GOLD,
            new Vector2(130, 0),   new Vector2(60, 20));

        AddText(screen, "PPotential",
            "", 11, GREEN,
            new Vector2(0, -20),
            new Vector2(300, 22));

        GameObject prevProspect = CreateButton(screen,
            "<", SURFACE, TEXT,
            new Vector2(-155, 130),
            new Vector2(36, 36), 22);
        AddBorder(prevProspect, BORDER, 2);
        GetButton(prevProspect).onClick.AddListener(() =>
        {
            draftPickIndex--;
            if (draftPickIndex < 0)
                draftPickIndex = draftClass.Count - 1;
            RefreshProspectCard();
        });

        GameObject nextProspect = CreateButton(screen,
            ">", SURFACE, TEXT,
            new Vector2(155, 130),
            new Vector2(36, 36), 22);
        AddBorder(nextProspect, BORDER, 2);
        GetButton(nextProspect).onClick.AddListener(() =>
        {
            draftPickIndex++;
            if (draftPickIndex >= draftClass.Count)
                draftPickIndex = 0;
            RefreshProspectCard();
        });

        AddText(screen, "ProspectCounter",
            "1 / 30", 12, SUBTEXT,
            new Vector2(0, -50),
            new Vector2(200, 24));

        GameObject draftBtn = CreateButton(screen,
            "DRAFT THIS PLAYER", RED, TEXT,
            new Vector2(0, -120),
            new Vector2(300, 52), 15);
        GetButton(draftBtn).onClick.AddListener(OnDraftPlayer);
        draftBtn.name = "DraftBtn";

        GameObject skipBtn = CreateButton(screen,
            "SKIP — AUTO PICK", SURFACE, SUBTEXT,
            new Vector2(0, -180),
            new Vector2(200, 38), 12);
        AddBorder(skipBtn, BORDER, 2);
        GetButton(skipBtn).onClick.AddListener(OnSkipPick);
        skipBtn.name = "Btn_SKIP - AUTO PICK";

        AddText(screen, "DraftResult",
            "", 12, GREEN,
            new Vector2(0, -228),
            new Vector2(340, 36));

        BuildBottomNav(screen);
        screen.SetActive(false);
        return screen;
    }

    void InitDraftScreen()
    {
        if (draftStarted && draftRound <= totalRounds &&
            draftClass.Count > 0)
        {
            RefreshProspectCard();
            return;
        }

        draftClass.Clear();
        draftPickIndex = 0;
        draftRound     = 1;
        draftStarted   = true;

        Transform btn =
            draftScreen.transform.Find("DraftBtn");
        if (btn != null)
            btn.GetComponent<Button>().interactable = true;

        GameManager gm =
            FindFirstObjectByType<GameManager>();
        if (gm != null)
            draftClass = gm.GenerateDraftClass();

        Team myTeam = GetMyTeam();
        if (myTeam != null)
        {
            List<Team> sorted = dataLoader.allTeams
                .OrderBy(t => t.wins).ToList();
            myDraftPick = sorted.FindIndex(
                t => t.abbreviation == selectedTeam) + 1;
        }

        SetDraftText("DraftSub",
            "Round 1 — Your pick: #" + myDraftPick);
        RefreshProspectCard();
    }

    void RefreshProspectCard()
    {
        if (draftClass.Count == 0) return;
        if (draftPickIndex >= draftClass.Count)
            draftPickIndex = 0;

        Player p       = draftClass[draftPickIndex];
        bool isPitcher = p.position == "SP" ||
                         p.position == "RP";

        SetDraftText("ProspectRank",
            "#" + (draftPickIndex + 1));
        string grade = p.overall >= 60 ? "A" :
                       p.overall >= 52 ? "B" :
                       p.overall >= 45 ? "C" : "D";
        SetDraftText("ProspectGrade", grade);
        SetDraftText("ProspectName",  p.FullName());
        SetDraftText("ProspectPos",
            p.position + " | " +
            (isPitcher ? p.throwingArm + "HP" :
             p.battingHand + "HB") +
            " | Age " + p.age);

        SetDraftText("PStat1Val", p.overall.ToString());
        SetDraftText("PStat2Val", p.age.ToString());
        SetDraftText("PStat3Val",
            isPitcher ? p.throwingArm : p.battingHand);

        if (isPitcher)
        {
            SetDraftText("PAttr1Label", "PITCHING");
            SetDraftText("PAttr1Val",   p.pitching.ToString());
            SetDraftText("PAttr2Label", "STAMINA");
            SetDraftText("PAttr2Val",   p.stamina.ToString());
            SetDraftText("PAttr3Label", "POTENTIAL");
            SetDraftText("PAttr3Val",
                p.age <= 19 ? "HIGH" : "MED");
            SetDraftText("PAttr4Label", "ETA");
            SetDraftText("PAttr4Val",
                p.age <= 19 ? "3-4 YRS" : "2-3 YRS");
        }
        else
        {
            SetDraftText("PAttr1Label", "CONTACT");
            SetDraftText("PAttr1Val",   p.contact.ToString());
            SetDraftText("PAttr2Label", "POWER");
            SetDraftText("PAttr2Val",   p.power.ToString());
            SetDraftText("PAttr3Label", "SPEED");
            SetDraftText("PAttr3Val",   p.speed.ToString());
            SetDraftText("PAttr4Label", "ETA");
            SetDraftText("PAttr4Val",
                p.age <= 19 ? "3-4 YRS" : "2-3 YRS");
        }

        string potential =
            p.overall >= 60
                ? "TOP PROSPECT — Future Star" :
            p.overall >= 52
                ? "HIGH UPSIDE — Solid Starter" :
            p.overall >= 45
                ? "AVERAGE Prospect" : "Depth Piece";
        SetDraftText("PPotential", potential);
        SetDraftText("ProspectCounter",
            (draftPickIndex + 1) + " / " +
            draftClass.Count +
            "  |  Round " + draftRound +
            " of " + totalRounds);
        SetDraftText("DraftResult", "");
    }

    void OnDraftPlayer()
    {
        if (draftRound > totalRounds)
        {
            SetDraftTextColor("DraftResult",
                "Draft is complete!", GOLD);
            return;
        }
        if (draftClass.Count == 0) return;

        Player pick = draftClass[draftPickIndex];
        Team myTeam = GetMyTeam();
        if (myTeam == null) return;

        if (myTeam.aRoster == null)
            myTeam.aRoster = new List<Player>();

        pick.team             = selectedTeam;
        pick.minorLeagueLevel = "A";
        pick.contractYears    = 6;
        pick.salary           = 0.72f;

        myTeam.aRoster.Add(pick);
        draftClass.RemoveAt(draftPickIndex);
        if (draftPickIndex >= draftClass.Count)
            draftPickIndex = 0;

        int justPickedRound = draftRound;
        draftRound++;

        SetDraftTextColor("DraftResult",
            "Rd " + justPickedRound + ": DRAFTED " +
            pick.FullName() + " (" + pick.position +
            ") — A ball!", GREEN);

        if (draftRound <= totalRounds)
        {
            SetDraftText("DraftSub",
                "Round " + draftRound + " of " +
                totalRounds +
                " — Your pick: #" + myDraftPick);
            RefreshProspectCard();
        }
        else
        {
            SetDraftText("DraftSub", "Draft Complete!");
            Transform draftBtn =
                draftScreen.transform.Find("DraftBtn");
            if (draftBtn != null)
                draftBtn.GetComponent<Button>()
                    .interactable = false;
        }
    }

    void OnSkipPick()
    {
        if (draftRound > totalRounds)
        {
            SetDraftTextColor("DraftResult",
                "Draft is complete!", GOLD);
            return;
        }
        if (draftClass.Count == 0) return;

        Team myTeam = GetMyTeam();
        if (myTeam == null) return;

        Player autoPick = draftClass[0];
        if (myTeam.aRoster == null)
            myTeam.aRoster = new List<Player>();

        autoPick.team             = selectedTeam;
        autoPick.minorLeagueLevel = "A";
        autoPick.contractYears    = 6;
        autoPick.salary           = 0.72f;

        myTeam.aRoster.Add(autoPick);
        draftClass.RemoveAt(0);

        int justPickedRound = draftRound;
        draftRound++;
        draftPickIndex = 0;

        SetDraftTextColor("DraftResult",
            "Rd " + justPickedRound + ": Auto-picked " +
            autoPick.FullName() + " (" +
            autoPick.position + ")", GOLD);

        if (draftRound <= totalRounds)
        {
            SetDraftText("DraftSub",
                "Round " + draftRound + " of " +
                totalRounds +
                " — Your pick: #" + myDraftPick);
            RefreshProspectCard();
        }
        else
        {
            SetDraftText("DraftSub", "Draft Complete!");
            Transform draftBtn =
                draftScreen.transform.Find("DraftBtn");
            if (draftBtn != null)
                draftBtn.GetComponent<Button>()
                    .interactable = false;
            Transform skipBtn = draftScreen.transform
                .Find("Btn_SKIP - AUTO PICK");
            if (skipBtn != null)
                skipBtn.GetComponent<Button>()
                    .interactable = false;
        }
    }

    void SetDraftText(string objName, string value)
    {
        if (draftScreen == null) return;
        Transform t = draftScreen.transform.Find(objName);
        if (t == null) return;
        TextMeshProUGUI tmp =
            t.GetComponent<TextMeshProUGUI>();
        if (tmp != null) tmp.text = value;
    }

    void SetDraftTextColor(string objName,
                            string value, Color color)
    {
        if (draftScreen == null) return;
        Transform t = draftScreen.transform.Find(objName);
        if (t == null) return;
        TextMeshProUGUI tmp =
            t.GetComponent<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.text  = value;
            tmp.color = color;
        }
    }

    // -------------------------------------------------------
    // FREE AGENCY SCREEN
    // -------------------------------------------------------
    GameObject BuildFAScreen(GameObject canvas)
    {
        GameObject screen = CreateScreen(canvas, "FA");

        BuildImageBackground(screen, "background");
        AddImage(screen, "Overlay",
            new Color(0.03f, 0.05f, 0.10f, 0.92f),
            Vector2.zero, new Vector2(390, 844));

        AddImage(screen, "Header", SURFACE,
            new Vector2(0, 380), new Vector2(390, 88));

        AddText(screen, "Title",
            "FREE AGENCY", 22, TEXT,
            new Vector2(0, 388),
            new Vector2(300, 36), FontStyles.Bold);

        AddText(screen, "FASub",
            "Browse available players",
            13, GOLD, new Vector2(0, 358),
            new Vector2(300, 24));

        AddImage(screen, "BudgetBar",
            new Color(0.08f, 0.18f, 0.30f, 1f),
            new Vector2(0, 315), new Vector2(374, 36));

        AddText(screen, "BudgetText",
            "Budget: $0M | Payroll: $0M | Space: $0M",
            11, GOLD, new Vector2(0, 315),
            new Vector2(360, 36));

        AddImage(screen, "FACard",
            new Color(0.06f, 0.12f, 0.20f, 0.97f),
            new Vector2(0, 148), new Vector2(340, 240));

        AddImage(screen, "CardTop", RED,
            new Vector2(0, 266), new Vector2(340, 4));

        AddImage(screen, "FAPosBadge", RED,
            new Vector2(-130, 250), new Vector2(60, 28));
        AddText(screen, "FAPos",
            "", 13, TEXT,
            new Vector2(-130, 250),
            new Vector2(60, 28), FontStyles.Bold);

        AddImage(screen, "FAOvrBadge", SURFACE,
            new Vector2(130, 250), new Vector2(60, 28));
        AddText(screen, "FAOvr",
            "", 13, GOLD,
            new Vector2(130, 250),
            new Vector2(60, 28), FontStyles.Bold);

        AddText(screen, "FAName",
            "Loading...", 20, TEXT,
            new Vector2(0, 210),
            new Vector2(320, 40), FontStyles.Bold);

        AddText(screen, "FAInfo",
            "", 13, SUBTEXT,
            new Vector2(0, 182),
            new Vector2(320, 26));

        AddImage(screen, "FADiv1", BORDER,
            new Vector2(0, 164), new Vector2(300, 1));

        AddText(screen, "FAStat1L", "CONTACT",
            9, SUBTEXT, new Vector2(-130, 148),
            new Vector2(80, 20));
        AddText(screen, "FAStat1V", "",
            14, TEXT, new Vector2(-130, 130),
            new Vector2(80, 24), FontStyles.Bold);

        AddText(screen, "FAStat2L", "POWER",
            9, SUBTEXT, new Vector2(-43, 148),
            new Vector2(80, 20));
        AddText(screen, "FAStat2V", "",
            14, TEXT, new Vector2(-43, 130),
            new Vector2(80, 24), FontStyles.Bold);

        AddText(screen, "FAStat3L", "SPEED",
            9, SUBTEXT, new Vector2(43, 148),
            new Vector2(80, 20));
        AddText(screen, "FAStat3V", "",
            14, TEXT, new Vector2(43, 130),
            new Vector2(80, 24), FontStyles.Bold);

        AddText(screen, "FAStat4L", "MARKET",
            9, SUBTEXT, new Vector2(130, 148),
            new Vector2(80, 20));
        AddText(screen, "FAStat4V", "",
            14, GOLD, new Vector2(130, 130),
            new Vector2(80, 24), FontStyles.Bold);

        AddImage(screen, "FADiv2", BORDER,
            new Vector2(0, 112), new Vector2(300, 1));

        AddText(screen, "FAMarket",
            "Market Value: $0M",
            11, SUBTEXT, new Vector2(0, 96),
            new Vector2(300, 22));

        AddText(screen, "FACounter",
            "1 / 0", 11, SUBTEXT,
            new Vector2(0, 72),
            new Vector2(200, 22));

        GameObject prevFA = CreateButton(screen,
            "<", SURFACE, TEXT,
            new Vector2(-155, 220),
            new Vector2(32, 32), 16);
        AddBorder(prevFA, BORDER, 2);
        GetButton(prevFA).onClick.AddListener(() =>
        {
            faIndex--;
            if (faIndex < 0) faIndex = faPool.Count - 1;
            RefreshFACard();
        });

        GameObject nextFA = CreateButton(screen,
            ">", SURFACE, TEXT,
            new Vector2(155, 220),
            new Vector2(32, 32), 16);
        AddBorder(nextFA, BORDER, 2);
        GetButton(nextFA).onClick.AddListener(() =>
        {
            faIndex++;
            if (faIndex >= faPool.Count) faIndex = 0;
            RefreshFACard();
        });

        AddImage(screen, "OfferBG", SURFACE,
            new Vector2(0, -10), new Vector2(374, 110));

        AddText(screen, "OfferLabel",
            "YOUR OFFER", 10, SUBTEXT,
            new Vector2(0, 34), new Vector2(340, 20));

        AddText(screen, "SalLabel",
            "SALARY / YR", 9, SUBTEXT,
            new Vector2(-80, 14), new Vector2(100, 18));

        GameObject salDown = CreateButton(screen,
            "-", BORDER, TEXT,
            new Vector2(-110, -8),
            new Vector2(30, 30), 16);
        GetButton(salDown).onClick.AddListener(() =>
        {
            offerSalary = Mathf.Max(0.72f, offerSalary - 0.5f);
            RefreshOfferDisplay();
        });

        AddText(screen, "OfferSalary",
            "$5.0M", 14, GOLD,
            new Vector2(-68, -8),
            new Vector2(80, 30), FontStyles.Bold);

        GameObject salUp = CreateButton(screen,
            "+", BORDER, TEXT,
            new Vector2(-22, -8),
            new Vector2(30, 30), 16);
        GetButton(salUp).onClick.AddListener(() =>
        {
            offerSalary += 0.5f;
            RefreshOfferDisplay();
        });

        AddText(screen, "YrLabel",
            "YEARS", 9, SUBTEXT,
            new Vector2(80, 14), new Vector2(80, 18));

        GameObject yrDown = CreateButton(screen,
            "-", BORDER, TEXT,
            new Vector2(40, -8),
            new Vector2(30, 30), 16);
        GetButton(yrDown).onClick.AddListener(() =>
        {
            offerYears = Mathf.Max(1, offerYears - 1);
            RefreshOfferDisplay();
        });

        AddText(screen, "OfferYears",
            "2 YRS", 14, TEXT,
            new Vector2(82, -8),
            new Vector2(60, 30), FontStyles.Bold);

        GameObject yrUp = CreateButton(screen,
            "+", BORDER, TEXT,
            new Vector2(120, -8),
            new Vector2(30, 30), 16);
        GetButton(yrUp).onClick.AddListener(() =>
        {
            offerYears = Mathf.Min(10, offerYears + 1);
            RefreshOfferDisplay();
        });

        GameObject signBtn = CreateButton(screen,
            "SIGN PLAYER", RED, TEXT,
            new Vector2(0, -55),
            new Vector2(300, 44), 15);
        GetButton(signBtn).onClick.AddListener(OnSignPlayer);
        signBtn.name = "SignBtn";

        AddText(screen, "FAResult",
            "", 12, GREEN,
            new Vector2(0, -110),
            new Vector2(340, 36));

        BuildBottomNav(screen);
        screen.SetActive(false);
        return screen;
    }

    void InitFAScreen()
    {
        faPool.Clear();
        faIndex = 0;

        GameManager gm =
            FindFirstObjectByType<GameManager>();
        if (gm != null) faPool = gm.GetFreeAgents();
        faPool.Sort((a, b) =>
            b.overall.CompareTo(a.overall));

        Team myTeam = GetMyTeam();
        if (myTeam != null)
        {
            float space = myTeam.budget - myTeam.payroll;
            SetFAText("BudgetText",
                "Budget: $" + myTeam.budget + "M" +
                "  Payroll: $" +
                myTeam.payroll.ToString("F1") + "M" +
                "  Space: $" +
                space.ToString("F1") + "M");
        }

        offerSalary = 5.0f;
        offerYears  = 2;

        if (faPool.Count == 0)
        {
            SetFAText("FAName",    "No free agents");
            SetFAText("FACounter", "0 / 0");
            return;
        }

        RefreshFACard();
        RefreshOfferDisplay();
    }

    void RefreshFACard()
    {
        if (faPool.Count == 0) return;
        if (faIndex >= faPool.Count) faIndex = 0;

        Player p       = faPool[faIndex];
        bool isPitcher = p.position == "SP" ||
                         p.position == "RP";

        SetFAText("FAPos",  p.position);
        SetFAText("FAOvr",  "OVR " + p.overall);
        SetFAText("FAName", p.FullName());
        SetFAText("FAInfo",
            "Age: " + p.age + "  |  " +
            (isPitcher ? p.throwingArm + "HP" :
             p.battingHand + "HB"));

        if (isPitcher)
        {
            SetFAText("FAStat1L", "PITCHING");
            SetFAText("FAStat1V", p.pitching.ToString());
            SetFAText("FAStat2L", "STAMINA");
            SetFAText("FAStat2V", p.stamina.ToString());
            SetFAText("FAStat3L", "AGE");
            SetFAText("FAStat3V", p.age.ToString());
        }
        else
        {
            SetFAText("FAStat1L", "CONTACT");
            SetFAText("FAStat1V", p.contact.ToString());
            SetFAText("FAStat2L", "POWER");
            SetFAText("FAStat2V", p.power.ToString());
            SetFAText("FAStat3L", "SPEED");
            SetFAText("FAStat3V", p.speed.ToString());
        }

        GameManager gm =
            FindFirstObjectByType<GameManager>();
        float market = gm != null ?
            gm.GetMarketValue(p) : p.salary;

        SetFAText("FAStat4L", "MARKET");
        SetFAText("FAStat4V",
            "$" + market.ToString("F1") + "M");
        SetFAText("FAMarket",
            "Asking: $" + market.ToString("F1") +
            "M/yr  Min: $" +
            (market * 0.8f).ToString("F1") + "M/yr");
        SetFAText("FACounter",
            (faIndex + 1) + " / " + faPool.Count);
        SetFAText("FAResult", "");

        offerSalary = Mathf.Round(market * 2) / 2f;
        RefreshOfferDisplay();
    }

    void RefreshOfferDisplay()
    {
        SetFAText("OfferSalary",
            "$" + offerSalary.ToString("F1") + "M");
        SetFAText("OfferYears", offerYears + " YRS");
    }

    void OnSignPlayer()
    {
        if (faPool.Count == 0) return;

        Player p    = faPool[faIndex];
        Team myTeam = GetMyTeam();
        if (myTeam == null) return;

        GameManager gm =
            FindFirstObjectByType<GameManager>();
        if (gm == null) return;

        float space = myTeam.budget - myTeam.payroll;
        if (offerSalary > space)
        {
            SetFATextColor("FAResult",
                "Not enough cap space! Need $" +
                offerSalary.ToString("F1") +
                "M, have $" +
                space.ToString("F1") + "M", RED);
            return;
        }

        float market = gm.GetMarketValue(p);
        if (offerSalary < market * 0.8f)
        {
            SetFATextColor("FAResult",
                p.FullName() + " rejected — try $" +
                (market * 0.8f).ToString("F1") + "M+", RED);
            return;
        }

        p.team          = selectedTeam;
        p.salary        = offerSalary;
        p.contractYears = offerYears;

        if (myTeam.roster == null)
            myTeam.roster = new List<Player>();
        myTeam.roster.Add(p);
        myTeam.payroll += offerSalary;

        faPool.RemoveAt(faIndex);
        if (faIndex >= faPool.Count) faIndex = 0;

        SetFATextColor("FAResult",
            "SIGNED: " + p.FullName() + " — $" +
            offerSalary.ToString("F1") + "M x " +
            offerYears + " years!", GREEN);

        float newSpace = myTeam.budget - myTeam.payroll;
        SetFAText("BudgetText",
            "Budget: $" + myTeam.budget + "M" +
            "  Payroll: $" +
            myTeam.payroll.ToString("F1") + "M" +
            "  Space: $" +
            newSpace.ToString("F1") + "M");

        if (faPool.Count > 0) RefreshFACard();
        else SetFAText("FAName", "No more free agents");

        PopulateTeamScreen(myTeam);
    }

    void SetFAText(string objName, string value)
    {
        if (faScreen == null) return;
        Transform t = faScreen.transform.Find(objName);
        if (t == null) return;
        TextMeshProUGUI tmp =
            t.GetComponent<TextMeshProUGUI>();
        if (tmp != null) tmp.text = value;
    }

    void SetFATextColor(string objName,
                         string value, Color color)
    {
        if (faScreen == null) return;
        Transform t = faScreen.transform.Find(objName);
        if (t == null) return;
        TextMeshProUGUI tmp =
            t.GetComponent<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.text  = value;
            tmp.color = color;
        }
    }

    // -------------------------------------------------------
    // LIVE GAME SCREEN
    // -------------------------------------------------------
    GameObject BuildLiveGameScreen(GameObject canvas)
    {
        GameObject screen = CreateScreen(canvas, "LiveGame");

        AddImage(screen, "BG", BG,
            Vector2.zero, new Vector2(390, 844));

        // Scoreboard
        AddImage(screen, "Scoreboard",
            new Color(0.02f, 0.05f, 0.10f, 1f),
            new Vector2(0, 358), new Vector2(390, 104));

        AddImage(screen, "ScoreboardBorder", RED,
            new Vector2(0, 308), new Vector2(390, 3));

        AddImage(screen, "TeamCol",
            new Color(0.06f, 0.12f, 0.20f, 1f),
            new Vector2(-163, 358), new Vector2(50, 104));

        AddText(screen, "AwayAbbr",
            "AWY", 11, GOLD,
            new Vector2(-163, 378),
            new Vector2(50, 22), FontStyles.Bold);

        AddText(screen, "HomeAbbr",
            "HME", 11, GOLD,
            new Vector2(-163, 350),
            new Vector2(50, 22), FontStyles.Bold);

        // Inning columns 1-12
        for (int i = 0; i < 12; i++)
        {
            float cx = sbColStart + (i * sbColW);

            AddText(screen, "InnHdr_" + i,
                (i + 1).ToString(), 9, SUBTEXT,
                new Vector2(cx, 397),
                new Vector2(22, 16));

            AddText(screen, "InnAway_" + i,
                "-", 11, TEXT,
                new Vector2(cx, 378),
                new Vector2(22, 20));

            AddText(screen, "InnHome_" + i,
                "-", 11, TEXT,
                new Vector2(cx, 350),
                new Vector2(22, 20));

            if (i == 2 || i == 5 || i == 8)
                AddImage(screen, "InnDiv_" + i,
                    new Color(0.15f, 0.25f, 0.40f, 1f),
                    new Vector2(cx + 12f, 358),
                    new Vector2(1, 80));
        }

        // R H E
        AddImage(screen, "RHEBg",
            new Color(0.06f, 0.12f, 0.20f, 1f),
            new Vector2(152, 358), new Vector2(78, 104));

        AddText(screen, "RHdr", "R", 9, GOLD,
            new Vector2(130, 397), new Vector2(22, 16));
        AddText(screen, "HHdr", "H", 9, GOLD,
            new Vector2(152, 397), new Vector2(22, 16));
        AddText(screen, "EHdr", "E", 9, GOLD,
            new Vector2(174, 397), new Vector2(22, 16));

        AddText(screen, "AwayScore",  "0", 13, TEXT,
            new Vector2(130, 378), new Vector2(22, 20),
            FontStyles.Bold);
        AddText(screen, "AwayHits",   "0", 13, SUBTEXT,
            new Vector2(152, 378), new Vector2(22, 20));
        AddText(screen, "AwayErrors", "0", 13, SUBTEXT,
            new Vector2(174, 378), new Vector2(22, 20));

        AddText(screen, "HomeScore",  "0", 13, TEXT,
            new Vector2(130, 350), new Vector2(22, 20),
            FontStyles.Bold);
        AddText(screen, "HomeHits",   "0", 13, SUBTEXT,
            new Vector2(152, 350), new Vector2(22, 20));
        AddText(screen, "HomeErrors", "0", 13, SUBTEXT,
            new Vector2(174, 350), new Vector2(22, 20));

        // Inning label on ballpark
        AddText(screen, "InningLabel",
            "TOP 1ST", 12, GOLD,
            new Vector2(0, 270),
            new Vector2(120, 24));

        // Count display
        AddImage(screen, "CountBG", SURFACE,
            new Vector2(0, 300), new Vector2(390, 28));

        AddText(screen, "BallsLabel", "B", 9, SUBTEXT,
            new Vector2(-100, 300), new Vector2(20, 24));

        for (int i = 0; i < 4; i++)
        {
            GameObject dot = new GameObject("BallDot_" + i);
            dot.transform.SetParent(screen.transform, false);
            RectTransform dRT = dot.AddComponent<RectTransform>();
            dRT.anchoredPosition =
                new Vector2(-82 + (i * 16), 300);
            dRT.sizeDelta = new Vector2(12, 12);
            dot.AddComponent<Image>().color = BORDER;
        }

        AddText(screen, "StrikesLabel", "S", 9, SUBTEXT,
            new Vector2(0, 300), new Vector2(20, 24));

        for (int i = 0; i < 3; i++)
        {
            GameObject dot = new GameObject("StrikeDot_" + i);
            dot.transform.SetParent(screen.transform, false);
            RectTransform dRT = dot.AddComponent<RectTransform>();
            dRT.anchoredPosition =
                new Vector2(14 + (i * 16), 300);
            dRT.sizeDelta = new Vector2(12, 12);
            dot.AddComponent<Image>().color = BORDER;
        }

        AddText(screen, "OutsLabel", "O", 9, SUBTEXT,
            new Vector2(90, 300), new Vector2(20, 24));

        for (int i = 0; i < 3; i++)
        {
            GameObject dot = new GameObject("OutDot_" + i);
            dot.transform.SetParent(screen.transform, false);
            RectTransform dRT = dot.AddComponent<RectTransform>();
            dRT.anchoredPosition =
                new Vector2(104 + (i * 16), 300);
            dRT.sizeDelta = new Vector2(12, 12);
            dot.AddComponent<Image>().color = BORDER;
        }

        // Ballpark
        AddImage(screen, "OutfieldGrass",
            new Color(0.04f, 0.22f, 0.08f, 1f),
            new Vector2(0, 165), new Vector2(390, 200));

        for (int i = 0; i < 7; i++)
        {
            Color stripeCol = i % 2 == 0
                ? new Color(0.04f, 0.22f, 0.08f, 1f)
                : new Color(0.05f, 0.26f, 0.10f, 1f);
            AddImage(screen, "Stripe_" + i, stripeCol,
                new Vector2(-150 + (i * 50), 165),
                new Vector2(44, 200));
        }

        AddImage(screen, "InfieldDirt",
            new Color(0.42f, 0.26f, 0.10f, 1f),
            new Vector2(0, 140), new Vector2(180, 120));

        AddImage(screen, "InfieldGrass",
            new Color(0.05f, 0.26f, 0.10f, 1f),
            new Vector2(0, 150), new Vector2(110, 80));

        AddImage(screen, "Mound",
            new Color(0.50f, 0.32f, 0.14f, 1f),
            new Vector2(0, 140), new Vector2(20, 12));

        AddImage(screen, "FirstBase",
            new Color(0.9f, 0.9f, 0.9f, 1f),
            new Vector2(55, 140), new Vector2(10, 10));
        AddImage(screen, "SecondBase",
            new Color(0.9f, 0.9f, 0.9f, 1f),
            new Vector2(0, 185), new Vector2(10, 10));
        AddImage(screen, "ThirdBase",
            new Color(0.9f, 0.9f, 0.9f, 1f),
            new Vector2(-55, 140), new Vector2(10, 10));
        AddImage(screen, "HomePlate",
            new Color(0.9f, 0.9f, 0.9f, 1f),
            new Vector2(0, 95), new Vector2(10, 10));

        AddImage(screen, "Runner1",
            new Color(1f, 0.84f, 0f, 0f),
            new Vector2(55, 140), new Vector2(14, 14));
        AddImage(screen, "Runner2",
            new Color(1f, 0.84f, 0f, 0f),
            new Vector2(0, 185), new Vector2(14, 14));
        AddImage(screen, "Runner3",
            new Color(1f, 0.84f, 0f, 0f),
            new Vector2(-55, 140), new Vector2(14, 14));

        AddImage(screen, "WallLeft",
            new Color(0.06f, 0.16f, 0.08f, 1f),
            new Vector2(-140, 238), new Vector2(120, 14));
        AddImage(screen, "WallCenter",
            new Color(0.06f, 0.16f, 0.08f, 1f),
            new Vector2(0, 248), new Vector2(140, 14));
        AddImage(screen, "WallRight",
            new Color(0.06f, 0.16f, 0.08f, 1f),
            new Vector2(140, 238), new Vector2(120, 14));

        AddImage(screen, "WallTrimL", RED,
            new Vector2(-140, 244), new Vector2(120, 3));
        AddImage(screen, "WallTrimC", RED,
            new Vector2(0, 254), new Vector2(140, 3));
        AddImage(screen, "WallTrimR", RED,
            new Vector2(140, 244), new Vector2(120, 3));

        AddImage(screen, "FoulPoleL", GOLD,
            new Vector2(-188, 220), new Vector2(3, 80));
        AddImage(screen, "FoulPoleR", GOLD,
            new Vector2(188, 220), new Vector2(3, 80));

        // Matchup
        AddImage(screen, "MatchupBG", SURFACE,
            new Vector2(0, 50), new Vector2(390, 58));

        AddText(screen, "BatterName",
            "BATTER", 13, TEXT,
            new Vector2(-80, 60),
            new Vector2(160, 24), FontStyles.Bold);

        AddText(screen, "BatterInfo",
            "", 10, SUBTEXT,
            new Vector2(-80, 42),
            new Vector2(160, 20));

        AddText(screen, "MatchupVS",
            "VS", 10, SUBTEXT,
            new Vector2(0, 50),
            new Vector2(30, 20));

        AddText(screen, "PitcherName",
            "PITCHER", 13, TEXT,
            new Vector2(80, 60),
            new Vector2(160, 24), FontStyles.Bold);

        AddText(screen, "PitcherInfo",
            "", 10, SUBTEXT,
            new Vector2(80, 42),
            new Vector2(160, 20));

        // Play by play
        AddImage(screen, "PBPBg",
            new Color(0.04f, 0.08f, 0.14f, 1f),
            new Vector2(0, -30), new Vector2(390, 60));

        AddText(screen, "PBP1",
            "Game starting...", 11, TEXT,
            new Vector2(0, -18),
            new Vector2(370, 22));

        AddText(screen, "PBP2",
            "", 11, SUBTEXT,
            new Vector2(0, -38),
            new Vector2(370, 22));

        // Decision buttons
        AddImage(screen, "DecisionBG", SURFACE,
            new Vector2(0, -100), new Vector2(390, 80));

        GameObject pitchBtn = CreateButton(screen,
            "PITCH", RED, TEXT,
            new Vector2(0, -100),
            new Vector2(120, 54), 16);
        GetButton(pitchBtn).onClick.AddListener(OnPitch);
        pitchBtn.name = "PitchBtn";

        GameObject pullBtn = CreateButton(screen,
            "PULL\nPITCHER", SURFACE, TEXT,
            new Vector2(-130, -100),
            new Vector2(80, 54), 11);
        AddBorder(pullBtn, BORDER, 2);
        GetButton(pullBtn).onClick.AddListener(OnPullPitcher);
        pullBtn.name = "PullBtn";

        GameObject ibbBtn = CreateButton(screen,
            "IBB", SURFACE, TEXT,
            new Vector2(130, -100),
            new Vector2(80, 54), 11);
        AddBorder(ibbBtn, BORDER, 2);
        GetButton(ibbBtn).onClick.AddListener(OnIBB);
        ibbBtn.name = "IBBBtn";

        GameObject swingBtn = CreateButton(screen,
            "SWING", GREEN, BG,
            new Vector2(0, -100),
            new Vector2(120, 54), 16);
        GetButton(swingBtn).onClick.AddListener(OnPitch);
        swingBtn.name = "SwingBtn";

        GameObject takeBtn = CreateButton(screen,
            "TAKE", SURFACE, TEXT,
            new Vector2(-130, -100),
            new Vector2(80, 54), 11);
        AddBorder(takeBtn, BORDER, 2);
        GetButton(takeBtn).onClick.AddListener(OnTakePitch);
        takeBtn.name = "TakeBtn";

        GameObject buntBtn = CreateButton(screen,
            "BUNT", SURFACE, TEXT,
            new Vector2(130, -100),
            new Vector2(80, 54), 11);
        AddBorder(buntBtn, BORDER, 2);
        GetButton(buntBtn).onClick.AddListener(OnBunt);
        buntBtn.name = "BuntBtn";

        GameObject autoBtn = CreateButton(screen,
            "SIM REST", SURFACE, SUBTEXT,
            new Vector2(-90, -165),
            new Vector2(130, 38), 12);
        AddBorder(autoBtn, BORDER, 2);
        GetButton(autoBtn).onClick.AddListener(OnSimRestOfGame);

        GameObject exitBtn2 = CreateButton(screen,
            "EXIT GAME", SURFACE, SUBTEXT,
            new Vector2(90, -165),
            new Vector2(130, 38), 12);
        AddBorder(exitBtn2, BORDER, 2);
        GetButton(exitBtn2).onClick.AddListener(() =>
        {
            gameInProgress = false;
            ShowScreen(teamScreen);
        });

        // Game over overlay
        GameObject gameOverPanel =
            new GameObject("GameOverPanel");
        gameOverPanel.transform.SetParent(
            screen.transform, false);
        RectTransform gopRT =
            gameOverPanel.AddComponent<RectTransform>();
        gopRT.anchoredPosition = Vector2.zero;
        gopRT.sizeDelta        = new Vector2(390, 844);
        gameOverPanel.AddComponent<Image>().color =
            new Color(0f, 0f, 0f, 0.85f);

        AddText(gameOverPanel, "GOTitle",
            "FINAL SCORE", 18, GOLD,
            new Vector2(0, 60),
            new Vector2(300, 36), FontStyles.Bold);

        AddText(gameOverPanel, "GOScore",
            "0 - 0", 52, TEXT,
            new Vector2(0, 0),
            new Vector2(300, 80), FontStyles.Bold);

        AddText(gameOverPanel, "GOResult",
            "", 16, GREEN,
            new Vector2(0, -60),
            new Vector2(300, 36));

        GameObject goBtn = CreateButton(gameOverPanel,
            "CONTINUE", RED, TEXT,
            new Vector2(0, -120),
            new Vector2(200, 52), 16);
        GetButton(goBtn).onClick.AddListener(() =>
        {
            gameInProgress = false;
            ShowScreen(teamScreen);
        });

        gameOverPanel.SetActive(false);
        screen.SetActive(false);
        return screen;
    }

    // Reset per-game stats for a team
    void ResetGameStats(Team team)
    {
        if (team?.roster == null) return;
        foreach (Player p in team.roster)
        {
            p.atBats           = 0;
            p.hits             = 0;
            p.singles          = 0;
            p.doubles          = 0;
            p.triples          = 0;
            p.homeRuns         = 0;
            p.rbi              = 0;
            p.runs             = 0;
            p.walks            = 0;
            p.strikeouts       = 0;
            p.inningsPitched   = 0;
            p.earnedRuns       = 0;
            p.hitsAllowed      = 0;
            p.walksAllowed     = 0;
            p.strikeoutsThrown = 0;
        }
    }

    // Start a live game
    public void StartLiveGame(Team home, Team away)
    {
        homeTeam       = home;
        awayTeam       = away;
        homeScore      = 0;
        awayScore      = 0;
        currentInning  = 1;
        outs           = 0;
        balls          = 0;
        strikes        = 0;
        isTopInning    = true;
        gameOnBase1    = false;
        gameOnBase2    = false;
        gameOnBase3    = false;
        playByPlay.Clear();
        homeBatterIndex  = 0;
        awayBatterIndex  = 0;
        gameInProgress   = true;
        gameOver         = false;
        usedRelievers.Clear();
        lastHomePitcher = null;
        lastAwayPitcher = null;

        homeInningRuns = new int[12];
        awayInningRuns = new int[12];
        homeHits       = 0;
        awayHits       = 0;
        homeErrors     = 0;
        awayErrors     = 0;

        // Reset game stats for all players
        ResetGameStats(home);
        ResetGameStats(away);

        // Use pitching rotation
        GameManager gm =
            FindFirstObjectByType<GameManager>();
        if (gm != null)
        {
            List<Player> homeSPs = home.roster.FindAll(
                p => p.position == "SP");
            List<Player> awaySPs = away.roster.FindAll(
                p => p.position == "SP");

            int homeRotIdx = homeSPs.Count > 0
                ? gm.GetStartingPitcherIndex(
                    home.abbreviation) % homeSPs.Count
                : 0;
            int awayRotIdx = awaySPs.Count > 0
                ? gm.GetStartingPitcherIndex(
                    away.abbreviation) % awaySPs.Count
                : 0;

            homePitcherIndex = homeSPs.Count > 0
                ? home.roster.IndexOf(homeSPs[homeRotIdx])
                : 0;
            awayPitcherIndex = awaySPs.Count > 0
                ? away.roster.IndexOf(awaySPs[awayRotIdx])
                : 0;

            gm.AdvanceRotation(home.abbreviation);
            gm.AdvanceRotation(away.abbreviation);
        }
        else
        {
            homePitcherIndex = 0;
            awayPitcherIndex = 0;
        }

        Transform gop = liveGameScreen.transform
            .Find("GameOverPanel");
        if (gop != null) gop.gameObject.SetActive(false);

        ShowScreen(liveGameScreen);
        RefreshGameDisplay();
        AddPlay("FIRST PITCH — " + awayTeam.city +
                " at " + homeTeam.city);
    }

    void RefreshGameDisplay()
    {
        if (liveGameScreen == null) return;

        SetGameText("AwayAbbr",
            awayTeam?.abbreviation ?? "AWY");
        SetGameText("HomeAbbr",
            homeTeam?.abbreviation ?? "HME");

        string half      = isTopInning ? "TOP" : "BOT";
        string[] innings = {
            "1ST","2ND","3RD","4TH","5TH","6TH",
            "7TH","8TH","9TH","10TH","11TH","12TH" };
        int inningIdx = Mathf.Clamp(
            currentInning - 1, 0, innings.Length - 1);
        SetGameText("InningLabel",
            half + " " + innings[inningIdx]);

        UpdateDots("BallDot_",   4, balls,   GREEN);
        UpdateDots("StrikeDot_", 3, strikes, RED);
        UpdateDots("OutDot_",    3, outs,    GOLD);

        UpdateRunner("Runner1", gameOnBase1);
        UpdateRunner("Runner2", gameOnBase2);
        UpdateRunner("Runner3", gameOnBase3);

        Team batting  = isTopInning ? awayTeam : homeTeam;
        Team pitching = isTopInning ? homeTeam : awayTeam;

        if (batting?.roster != null &&
            batting.roster.Count > 0)
        {
            List<Player> batters = batting.roster.FindAll(
                p => p.position != "SP" &&
                     p.position != "RP");
            if (batters.Count > 0)
            {
                int idx = isTopInning
                    ? awayBatterIndex % batters.Count
                    : homeBatterIndex % batters.Count;
                Player bat = batters[idx];
                SetGameText("BatterName", bat.FullName());
                // Show season AVG if has ABs, else rating
                string batStat = bat.seasonAtBats > 0
                    ? bat.SeasonBattingAverage()
                       .ToString("F3")
                    : "." + bat.contact.ToString()
                       .PadLeft(3, '0');
                SetGameText("BatterInfo",
                    bat.position + " | " + batStat);
            }
        }

        if (pitching?.roster != null)
        {
            List<Player> allP = pitching.roster.FindAll(
                p => p.position == "SP" ||
                     p.position == "RP");
            if (allP.Count > 0)
            {
                int idx = isTopInning
                    ? homePitcherIndex % allP.Count
                    : awayPitcherIndex % allP.Count;
                Player pit = allP[idx];
                SetGameText("PitcherName", pit.FullName());
                // Show season ERA if has pitched, else rating
                string wl = pit.seasonWins + "-" +
                            pit.seasonLosses;
                string pitStat = pit.seasonInningsPitched > 0
                    ? wl + " " +
                      pit.SeasonERA().ToString("F2") + " ERA"
                    : pit.pitching + " OVR";
                SetGameText("PitcherInfo",
                    pit.throwingArm + "HP | " + pitStat);
            }
        }

        RefreshScoreboard();
        RefreshDecisionButtons();
    }

    void RefreshScoreboard()
    {
        if (liveGameScreen == null) return;

        float colStart = sbColStart;
        float colW     = sbColW;
        int   offset   = Mathf.Max(0, currentInning - 9);

        for (int i = 0; i < 12; i++)
        {
            string awayVal = i < currentInning - 1
                ? awayInningRuns[i].ToString()
                : i == currentInning - 1 && !isTopInning
                    ? awayInningRuns[i].ToString()
                    : "-";

            string homeVal = i < currentInning - 1
                ? homeInningRuns[i].ToString()
                : "-";

            Color awayCol =
                i == currentInning - 1 ? GOLD : TEXT;
            Color homeCol =
                i == currentInning - 1 ? GOLD : TEXT;

            SetGameTextColor("InnAway_" + i, awayVal, awayCol);
            SetGameTextColor("InnHome_" + i, homeVal, homeCol);

            float cx     = colStart + ((i - offset) * colW);
            bool visible = (i - offset) >= 0 &&
                           (i - offset) < 9;

            Transform hdr = liveGameScreen.transform
                .Find("InnHdr_" + i);
            Transform away = liveGameScreen.transform
                .Find("InnAway_" + i);
            Transform home = liveGameScreen.transform
                .Find("InnHome_" + i);

            if (hdr != null)
            {
                hdr.GetComponent<RectTransform>()
                    .anchoredPosition = new Vector2(cx, 397);
                hdr.gameObject.SetActive(visible);
            }
            if (away != null)
            {
                away.GetComponent<RectTransform>()
                    .anchoredPosition = new Vector2(cx, 378);
                away.gameObject.SetActive(visible);
            }
            if (home != null)
            {
                home.GetComponent<RectTransform>()
                    .anchoredPosition = new Vector2(cx, 350);
                home.gameObject.SetActive(visible);
            }
        }

        SetGameText("AwayScore",  awayScore.ToString());
        SetGameText("HomeScore",  homeScore.ToString());
        SetGameText("AwayHits",   awayHits.ToString());
        SetGameText("HomeHits",   homeHits.ToString());
        SetGameText("AwayErrors", awayErrors.ToString());
        SetGameText("HomeErrors", homeErrors.ToString());
    }

    void RefreshDecisionButtons()
    {
        if (liveGameScreen == null) return;

        bool playerIsFielding =
            (isTopInning  && homeTeam == GetMyTeam()) ||
            (!isTopInning && awayTeam == GetMyTeam());

        Transform pitch = liveGameScreen.transform.Find("PitchBtn");
        Transform pull  = liveGameScreen.transform.Find("PullBtn");
        Transform ibb   = liveGameScreen.transform.Find("IBBBtn");
        Transform swing = liveGameScreen.transform.Find("SwingBtn");
        Transform take  = liveGameScreen.transform.Find("TakeBtn");
        Transform bunt  = liveGameScreen.transform.Find("BuntBtn");

        if (pitch != null) pitch.gameObject.SetActive(playerIsFielding);
        if (pull  != null) pull.gameObject.SetActive(playerIsFielding);
        if (ibb   != null) ibb.gameObject.SetActive(playerIsFielding);
        if (swing != null) swing.gameObject.SetActive(!playerIsFielding);
        if (take  != null) take.gameObject.SetActive(!playerIsFielding);
        if (bunt  != null) bunt.gameObject.SetActive(!playerIsFielding);
    }

    void UpdateDots(string prefix, int total,
                    int filled, Color fillColor)
    {
        for (int i = 0; i < total; i++)
        {
            Transform dot = liveGameScreen.transform
                .Find(prefix + i);
            if (dot == null) continue;
            Image img = dot.GetComponent<Image>();
            if (img == null) continue;
            img.color = i < filled ? fillColor : BORDER;
        }
    }

    void UpdateRunner(string name, bool onBase)
    {
        Transform r = liveGameScreen.transform.Find(name);
        if (r == null) return;
        Image img = r.GetComponent<Image>();
        if (img == null) return;
        Color c = img.color;
        c.a     = onBase ? 1f : 0f;
        img.color = c;
    }

    void AddPlay(string text)
    {
        playByPlay.Insert(0, text);
        if (playByPlay.Count > 10)
            playByPlay.RemoveAt(playByPlay.Count - 1);
        SetGameText("PBP1",
            playByPlay.Count > 0 ? playByPlay[0] : "");
        SetGameText("PBP2",
            playByPlay.Count > 1 ? playByPlay[1] : "");
    }

    // -------------------------------------------------------
    // PITCH
    // -------------------------------------------------------
    void OnPitch()
    {
        if (!gameInProgress || gameOver) return;

        Team batting  = isTopInning ? awayTeam : homeTeam;
        Team pitching = isTopInning ? homeTeam : awayTeam;

        if (batting?.roster  == null) return;
        if (pitching?.roster == null) return;

        List<Player> batters = batting.roster.FindAll(
            p => p.position != "SP" &&
                 p.position != "RP");
        if (batters.Count == 0) return;

        int batterIdx = isTopInning
            ? awayBatterIndex % batters.Count
            : homeBatterIndex % batters.Count;
        Player batter = batters[batterIdx];

        List<Player> allPitchers = pitching.roster.FindAll(
            p => p.position == "SP" ||
                 p.position == "RP");
        if (allPitchers.Count == 0) return;

        int pitcherIdx = isTopInning
            ? homePitcherIndex % allPitchers.Count
            : awayPitcherIndex % allPitchers.Count;
        Player pitcher = allPitchers[pitcherIdx];

        string result = SimulateAtBat(batter, pitcher);
        ProcessAtBatResult(result, batter, batting);

        // Track pitcher game + season strikeouts
        if (result == "STRIKEOUT")
        {
            pitcher.strikeoutsThrown++;
            pitcher.seasonStrikeoutsThrown++;
        }

        // Track pitcher game + season hits allowed
        if (result == "SINGLE"  || result == "DOUBLE" ||
            result == "TRIPLE"  || result == "HOME RUN")
        {
            pitcher.hitsAllowed++;
            pitcher.seasonHitsAllowed++;
        }

        // Track pitcher game + season walks
        if (result == "WALK")
        {
            pitcher.walksAllowed++;
            pitcher.seasonWalksAllowed++;
        }

        // Track who is currently pitching
        // so we know the pitcher of record
        Team pitchingTeam = isTopInning ? homeTeam : awayTeam;
        List<Player> currentPitchers =
            pitchingTeam.roster.FindAll(
                p => p.position == "SP" ||
                     p.position == "RP");
        if (currentPitchers.Count > 0)
        {
            int idx = isTopInning
                ? homePitcherIndex % currentPitchers.Count
                : awayPitcherIndex % currentPitchers.Count;

            if (isTopInning)
                lastHomePitcher = currentPitchers[idx];
            else
                lastAwayPitcher = currentPitchers[idx];
        }


        RefreshGameDisplay();
    }

    string SimulateAtBat(Player batter, Player pitcher)
    {
        float hr  = 3.3f + (batter.power   - 50) / 200f * 2f;
        float tri = 0.6f;
        float dbl = 5.3f + (batter.power   - 50) / 200f;
        float sng = 15f  + (batter.contact - 50) / 200f * 3f;
        float bb  = 8.5f;
        float so  = 22.5f - (batter.contact - 50) / 200f * 3f;

        float pitchMod = (pitcher.pitching - 50) / 200f;
        so  += pitchMod * 3f;
        sng -= pitchMod * 2f;
        hr  -= pitchMod;

        hr  = Mathf.Max(0.5f, hr);
        sng = Mathf.Max(5f,   sng);
        so  = Mathf.Max(5f,   so);

        float roll = Random.Range(0f, 100f);
        float c    = 0f;
        c += hr;  if (roll < c) return "HOME RUN";
        c += tri; if (roll < c) return "TRIPLE";
        c += dbl; if (roll < c) return "DOUBLE";
        c += sng; if (roll < c) return "SINGLE";
        c += bb;  if (roll < c) return "WALK";
        c += so;  if (roll < c) return "STRIKEOUT";
        return "OUT";
    }

    // -------------------------------------------------------
    // AT BAT RESULTS — tracks game + season stats
    // -------------------------------------------------------
    void ProcessAtBatResult(string result,
                             Player batter, Team batting)
    {
        string name = batter.FullName();

        switch (result)
        {
            case "HOME RUN":
                RecordHit(batting);
                batter.atBats++;        batter.seasonAtBats++;
                batter.homeRuns++;      batter.seasonHomeRuns++;
                batter.hits++;          batter.seasonHits++;
                int runs = 1;
                if (gameOnBase1) runs++;
                if (gameOnBase2) runs++;
                if (gameOnBase3) runs++;
                batter.rbi       += runs;
                batter.seasonRbi += runs;
                AddRuns(batting, runs);

                // Charge earned runs to pitcher
                Team pit1 = isTopInning ? homeTeam : awayTeam;
                if (pit1?.roster != null)
                {
                    List<Player> pp = pit1.roster.FindAll(
                        p => p.position == "SP" ||
                             p.position == "RP");
                    if (pp.Count > 0)
                    {
                        int pidx = isTopInning
                            ? homePitcherIndex % pp.Count
                            : awayPitcherIndex % pp.Count;
                        pp[pidx].earnedRuns += runs;
                        pp[pidx].seasonEarnedRuns += runs;
                    }
                }

                gameOnBase1 = gameOnBase2 =
                    gameOnBase3 = false;
                string hrType =
                    runs == 4 ? "GRAND SLAM!" :
                    runs == 3 ? "3-RUN HR!"   :
                    runs == 2 ? "2-RUN HR!"   : "SOLO HR!";
                AddPlay(name + " — " + hrType +
                        " " + runs + " RBI");
                break;

            case "TRIPLE":
                RecordHit(batting);
                batter.atBats++;    batter.seasonAtBats++;
                batter.triples++;   batter.seasonTriples++;
                batter.hits++;      batter.seasonHits++;
                int triRBI = 0;
                if (gameOnBase3) { AddRuns(batting, 1); triRBI++; }
                if (gameOnBase2) { AddRuns(batting, 1); triRBI++; }
                if (gameOnBase1) { AddRuns(batting, 1); triRBI++; }
                batter.rbi       += triRBI;
                batter.seasonRbi += triRBI;
                gameOnBase1 = gameOnBase2 = false;
                gameOnBase3 = true;
                AddPlay(name + (triRBI > 0
                    ? " — RBI TRIPLE! " + triRBI + " RBI"
                    : " — TRIPLE!"));
                break;

            case "DOUBLE":
                RecordHit(batting);
                batter.atBats++;    batter.seasonAtBats++;
                batter.doubles++;   batter.seasonDoubles++;
                batter.hits++;      batter.seasonHits++;
                int dblRBI = 0;
                if (gameOnBase3) { AddRuns(batting, 1); dblRBI++; }
                if (gameOnBase2) { AddRuns(batting, 1); dblRBI++; }
                batter.rbi       += dblRBI;
                batter.seasonRbi += dblRBI;
                gameOnBase3 = gameOnBase1;
                gameOnBase2 = true;
                gameOnBase1 = false;
                AddPlay(name + (dblRBI > 0
                    ? " — RBI DOUBLE! " + dblRBI + " RBI"
                    : " — DOUBLE!"));
                break;

            case "SINGLE":
                RecordHit(batting);
                batter.atBats++;    batter.seasonAtBats++;
                batter.singles++;   batter.seasonSingles++;
                batter.hits++;      batter.seasonHits++;
                int sngRBI = 0;
                if (gameOnBase3) { AddRuns(batting, 1); sngRBI++; }
                batter.rbi       += sngRBI;
                batter.seasonRbi += sngRBI;
                gameOnBase3 = gameOnBase2;
                gameOnBase2 = gameOnBase1;
                gameOnBase1 = true;
                AddPlay(name + (sngRBI > 0
                    ? " — RBI SINGLE! " + sngRBI + " RBI"
                    : " — SINGLE!"));
                break;

            case "WALK":
                batter.walks++;
                batter.seasonWalks++;
                if (gameOnBase1 && gameOnBase2 &&
                    gameOnBase3)
                {
                    AddRuns(batting, 1);
                    batter.rbi++;
                    batter.seasonRbi++;
                }
                gameOnBase3 =
                    (gameOnBase2 && gameOnBase1)
                    ? true : gameOnBase3;
                gameOnBase2 =
                    gameOnBase1 ? true : gameOnBase2;
                gameOnBase1 = true;
                AddPlay(name + " — WALK");
                break;

            case "STRIKEOUT":
                batter.atBats++;        batter.seasonAtBats++;
                batter.strikeouts++;    batter.seasonStrikeouts++;
                outs++;
                AddPlay(name + " — STRIKEOUT");
                AdvanceBatter(batting);
                CheckInningOver();
                return;

            case "OUT":
                batter.atBats++;
                batter.seasonAtBats++;
                outs++;
                AddPlay(name + " — OUT");
                AdvanceBatter(batting);
                CheckInningOver();
                return;
        }

        AdvanceBatter(batting);
        balls   = 0;
        strikes = 0;
    }

    void AddRuns(Team team, int runs)
    {
        if (team == homeTeam) homeScore += runs;
        else                  awayScore += runs;

        for (int i = 0; i < runs; i++)
            RecordInningRun(team);

        CheckWalkOff();
    }

    void RecordHit(Team batting)
    {
        if (batting == awayTeam) awayHits++;
        else                     homeHits++;
    }

    void RecordInningRun(Team batting)
    {
        int idx = Mathf.Clamp(currentInning - 1, 0, 11);
        if (batting == awayTeam) awayInningRuns[idx]++;
        else                     homeInningRuns[idx]++;
    }

    void CheckWalkOff()
    {
        if (isTopInning) return;
        if (currentInning < 9) return;
        if (homeScore > awayScore)
        {
            AddPlay("WALK-OFF! " + homeTeam.city + " WIN!");
            EndGame();
        }
    }

    void AdvanceBatter(Team batting)
    {
        if (batting == awayTeam) awayBatterIndex++;
        else                     homeBatterIndex++;
    }

    // -------------------------------------------------------
    // INNING MANAGEMENT + BULLPEN
    // -------------------------------------------------------

    // Credit inning to active pitcher (game + season)
    void CreditInningToPitcher(Team pitching, bool isHome)
    {
        if (pitching?.roster == null) return;

        List<Player> allPitchers = pitching.roster.FindAll(
            p => p.position == "SP" ||
                 p.position == "RP");
        if (allPitchers.Count == 0) return;

        int idx = isHome
            ? homePitcherIndex % allPitchers.Count
            : awayPitcherIndex % allPitchers.Count;

        allPitchers[idx].inningsPitched++;
        allPitchers[idx].seasonInningsPitched++;
    }

    // Auto pull tired starters and bring in bullpen
    void AutoManageBullpen(Team pitching, bool isHome)
    {
        if (pitching?.roster == null) return;

        List<Player> relievers = pitching.roster.FindAll(
            p => p.position == "RP" &&
                 !usedRelievers.Contains(p) &&
                 !p.isInjured);

        if (relievers.Count == 0) return;

        List<Player> allPitchers = pitching.roster.FindAll(
            p => p.position == "SP" ||
                 p.position == "RP");
        if (allPitchers.Count == 0) return;

        int pitcherIdx = isHome
            ? homePitcherIndex
            : awayPitcherIndex;

        int activeIdx = pitcherIdx % allPitchers.Count;
        Player activePitcher = allPitchers[activeIdx];

        bool isStarter    = activePitcher.position == "SP";
        bool shouldPull   = false;
        string reason     = "";

        if (isStarter)
        {
            // Pull starter after 5 innings
            if (activePitcher.inningsPitched >= 5)
            {
                shouldPull = true;
                reason = "has thrown " +
                    activePitcher.inningsPitched + " innings";
            }
            // Pull if struggling after 3 innings
            if (activePitcher.inningsPitched >= 3 &&
                activePitcher.earnedRuns >= 4)
            {
                shouldPull = true;
                reason = "is struggling";
            }
        }
        else
        {
            // Pull reliever after 2 innings
            if (activePitcher.inningsPitched >= 2)
            {
                shouldPull = true;
                reason = "has thrown " +
                    activePitcher.inningsPitched + " innings";
            }
        }

        if (!shouldPull) return;

        // Pick best reliever for the inning
        Player newPitcher = null;

        if (currentInning >= 9)
            newPitcher = relievers.Find(
                p => p.bullpenRole == "CL");

        if (newPitcher == null && currentInning >= 8)
            newPitcher = relievers.Find(
                p => p.bullpenRole == "SU");

        if (newPitcher == null)
            newPitcher = relievers.Find(
                p => p.bullpenRole == "MR");

        if (newPitcher == null && relievers.Count > 0)
        {
            relievers.Sort((a, b) =>
                b.pitching.CompareTo(a.pitching));
            newPitcher = relievers[0];
        }

        if (newPitcher == null) return;

        usedRelievers.Add(newPitcher);
        int newIdx = pitching.roster.IndexOf(newPitcher);

        if (isHome) homePitcherIndex = newIdx;
        else        awayPitcherIndex = newIdx;

        AddPlay("PITCHING CHANGE: " +
                activePitcher.FullName() +
                " (" + reason + ") → " +
                newPitcher.FullName() +
                (newPitcher.bullpenRole != "" ?
                 " (" + newPitcher.bullpenRole + ")" :
                 " (RP)"));
    }

    void CheckInningOver()
    {
        if (outs < 3) return;

        outs        = 0; balls = 0; strikes = 0;
        gameOnBase1 = false;
        gameOnBase2 = false;
        gameOnBase3 = false;

        if (isTopInning)
        {
            // Credit inning + auto manage bullpen
            CreditInningToPitcher(homeTeam, true);
            AutoManageBullpen(homeTeam, true);

            isTopInning = false;
            AddPlay("--- END TOP " + currentInning + " ---");

            if (currentInning >= 9 &&
                homeScore > awayScore)
            {
                AddPlay("WALK-OFF! " +
                        homeTeam.city + " WIN!");
                EndGame();
                return;
            }
        }
        else
        {
            // Credit inning + auto manage bullpen
            CreditInningToPitcher(awayTeam, false);
            AutoManageBullpen(awayTeam, false);

            isTopInning = true;
            currentInning++;
            AddPlay("--- END INNING " +
                    (currentInning - 1) + " ---");

            if (currentInning > 9 &&
                homeScore != awayScore)
            {
                EndGame();
                return;
            }

            if (currentInning > 9)
            {
                AddPlay("EXTRA INNINGS! Runner on 2nd.");
                gameOnBase2 = true;
            }
        }
    }


    // Credit W or L to the pitcher of record
    void CreditPitcherDecision()
    {
        if (homeScore == awayScore) return;

        bool homeWon = homeScore > awayScore;
        Team winTeam  = homeWon ? homeTeam : awayTeam;
        Team loseTeam = homeWon ? awayTeam : homeTeam;

        // -----------------------------------------------
        // WINNING PITCHER
        // Starter gets W if pitched 5+ innings.
        // Otherwise first reliever who pitched 1+
        // inning for the winning team gets the W.
        // -----------------------------------------------
        Player winPitcher = null;

        Player winStarter = winTeam.roster.Find(
            p => p.position == "SP" &&
                 p.inningsPitched >= 5);

        if (winStarter != null)
        {
            // Starter went 5+ — he gets the W
            winPitcher = winStarter;
        }
        else
        {
            // Find first reliever from winning team
            // who pitched at least 1 inning
            foreach (Player rp in usedRelievers)
            {
                if (winTeam.roster.Contains(rp) &&
                    rp.inningsPitched >= 1)
                {
                    winPitcher = rp;
                    break;
                }
            }

            // Fall back to starter if no reliever qualifies
            if (winPitcher == null)
                winPitcher = winTeam.roster.Find(
                    p => p.position == "SP" &&
                         p.inningsPitched > 0);
        }

        // -----------------------------------------------
        // LOSING PITCHER
        // ONLY give L to pitcher who actually gave up
        // earned runs. No ER = No Decision always.
        // -----------------------------------------------
        Player losePitcher = null;

        // Check ALL pitchers from losing team
        // Only eligible if they gave up earned runs
        List<Player> allLosePitchers = new List<Player>();

        // Add starter if pitched
        Player loseStarter = loseTeam.roster.Find(
            p => p.position == "SP" &&
                 p.inningsPitched > 0);
        if (loseStarter != null)
            allLosePitchers.Add(loseStarter);

        // Add relievers from losing team
        foreach (Player rp in usedRelievers)
            if (loseTeam.roster.Contains(rp) &&
                rp.inningsPitched > 0)
                allLosePitchers.Add(rp);

        // Find pitcher with most earned runs
        // — he is responsible for the loss
        int maxER = 0;
        foreach (Player p in allLosePitchers)
        {
            if (p.earnedRuns > maxER)
            {
                maxER       = p.earnedRuns;
                losePitcher = p;
            }
        }

        // If nobody gave up any earned runs
        // = No Decision for everyone on losing team
        if (maxER == 0)
        {
            losePitcher = null;
            Debug.Log("NO DECISION — losing team" +
                      " gave up 0 earned runs." +
                      " Unearned run loss.");
        }

        // Credit win
        if (winPitcher != null)
        {
            winPitcher.wins++;
            winPitcher.seasonWins++;
            AddPlay("W: " + winPitcher.FullName() +
                    " (" + winPitcher.seasonWins +
                    "-" + winPitcher.seasonLosses + ")");
            Debug.Log("WIN: " + winPitcher.FullName() +
                      " IP:" + winPitcher.inningsPitched +
                      " ER:" + winPitcher.earnedRuns +
                      " ERA:" +
                      winPitcher.SeasonERA().ToString("F2"));
        }

        // Credit loss — no decision if nobody qualifies
        if (losePitcher != null)
        {
            losePitcher.losses++;
            losePitcher.seasonLosses++;
            AddPlay("L: " + losePitcher.FullName() +
                    " (" + losePitcher.seasonWins +
                    "-" + losePitcher.seasonLosses + ")");
            Debug.Log("LOSS: " +
                      losePitcher.FullName() +
                      " IP:" + losePitcher.inningsPitched +
                      " ER:" + losePitcher.earnedRuns +
                      " ERA:" +
                      losePitcher.SeasonERA()
                          .ToString("F2"));
        }
        else
        {
            Debug.Log("No decision — no pitcher" +
                      " clearly responsible for loss");
        }
    }


    void EndGame()
    {
        gameOver       = true;
        gameInProgress = false;

        AddPlay("FINAL: " +
                awayTeam.abbreviation + " " + awayScore +
                " — " +
                homeTeam.abbreviation + " " + homeScore);

        // Credit W/L to pitchers
        CreditPitcherDecision();

        // Record result and auto-save
        GameManager gm =
            FindFirstObjectByType<GameManager>();
        if (gm != null)
        {
            gm.RecordLiveGameResult(
                homeTeam.abbreviation, homeScore,
                awayTeam.abbreviation, awayScore);
            gm.SaveFinalStandings();
            gm.SaveGame(currentSaveSlot);
            Debug.Log("Auto-saved after game!");
        }

        // Show box score
        ShowBoxScore(homeTeam, awayTeam,
                     homeScore, awayScore);
    }

    // -------------------------------------------------------
    // PITCHING DECISIONS
    // -------------------------------------------------------
    void OnPullPitcher()
    {
        Team pitching = isTopInning ? homeTeam : awayTeam;
        if (pitching?.roster == null) return;

        List<Player> relievers = pitching.roster.FindAll(
            p => p.position == "RP" &&
                 !p.isInjured &&
                 !usedRelievers.Contains(p));

        if (relievers.Count == 0)
        {
            AddPlay("No fresh relievers available!");
            return;
        }

        ShowRelieverPicker(pitching, relievers);
    }

    void ShowRelieverPicker(Team pitching,
                             List<Player> relievers)
    {
        Transform old = liveGameScreen.transform
            .Find("RelieverPicker");
        if (old != null) Destroy(old.gameObject);

        GameObject picker = new GameObject("RelieverPicker");
        picker.transform.SetParent(
            liveGameScreen.transform, false);
        RectTransform pRT =
            picker.AddComponent<RectTransform>();
        pRT.anchoredPosition = Vector2.zero;
        pRT.sizeDelta         = new Vector2(390, 844);
        picker.AddComponent<Image>().color =
            new Color(0f, 0f, 0f, 0.88f);

        GameObject title = new GameObject("Title");
        title.transform.SetParent(picker.transform, false);
        TextMeshProUGUI titleT =
            title.AddComponent<TextMeshProUGUI>();
        titleT.text      = "SELECT RELIEVER";
        titleT.fontSize  = 18f;
        titleT.color     = GOLD;
        titleT.fontStyle = FontStyles.Bold;
        titleT.alignment = TextAlignmentOptions.Center;
        RectTransform tRT =
            title.GetComponent<RectTransform>();
        tRT.anchoredPosition = new Vector2(0, 300);
        tRT.sizeDelta         = new Vector2(350, 36);

        GameObject cancelBtn = CreateButton(picker,
            "CANCEL", SURFACE, SUBTEXT,
            new Vector2(0, 250),
            new Vector2(120, 36), 12);
        AddBorder(cancelBtn, BORDER, 2);
        GetButton(cancelBtn).onClick.AddListener(() =>
            Destroy(picker));

        int   count  = Mathf.Min(relievers.Count, 8);
        float startY = 200f;

        for (int i = 0; i < count; i++)
        {
            Player rp = relievers[i];
            float  y  = startY - (i * 52f);

            GameObject row = new GameObject("RP_" + i);
            row.transform.SetParent(picker.transform, false);
            RectTransform rRT =
                row.AddComponent<RectTransform>();
            rRT.anchoredPosition = new Vector2(0, y);
            rRT.sizeDelta         = new Vector2(340, 46);
            row.AddComponent<Image>().color =
                new Color(0.05f, 0.10f, 0.20f, 1f);

            Button rowBtn = row.AddComponent<Button>();

            GameObject nameObj = new GameObject("Name");
            nameObj.transform.SetParent(row.transform, false);
            TextMeshProUGUI nameT =
                nameObj.AddComponent<TextMeshProUGUI>();
            nameT.text      = rp.FullName();
            nameT.fontSize  = 13f;
            nameT.color     = TEXT;
            nameT.alignment = TextAlignmentOptions.MidlineLeft;
            RectTransform nameRT =
                nameObj.GetComponent<RectTransform>();
            nameRT.anchoredPosition = new Vector2(-50, 8);
            nameRT.sizeDelta         = new Vector2(200, 30);

            GameObject roleObj = new GameObject("Role");
            roleObj.transform.SetParent(row.transform, false);
            TextMeshProUGUI roleT =
                roleObj.AddComponent<TextMeshProUGUI>();
            roleT.text      = rp.bullpenRole != "" ?
                rp.bullpenRole : "RP";
            roleT.fontSize  = 11f;
            roleT.color     = SUBTEXT;
            roleT.alignment = TextAlignmentOptions.MidlineLeft;
            RectTransform roleRT =
                roleObj.GetComponent<RectTransform>();
            roleRT.anchoredPosition = new Vector2(-50, -10);
            roleRT.sizeDelta         = new Vector2(100, 22);

            GameObject ovrObj = new GameObject("Ovr");
            ovrObj.transform.SetParent(row.transform, false);
            TextMeshProUGUI ovrT =
                ovrObj.AddComponent<TextMeshProUGUI>();
            ovrT.text      = "OVR " + rp.pitching;
            ovrT.fontSize  = 13f;
            ovrT.color     = GetOverallColor(rp.overall);
            ovrT.fontStyle = FontStyles.Bold;
            ovrT.alignment = TextAlignmentOptions.MidlineRight;
            RectTransform ovrRT =
                ovrObj.GetComponent<RectTransform>();
            ovrRT.anchoredPosition = new Vector2(120, 0);
            ovrRT.sizeDelta         = new Vector2(80, 46);

            rowBtn.onClick.AddListener(() =>
            {
                usedRelievers.Add(rp);

                if (isTopInning)
                    homePitcherIndex =
                        homeTeam.roster.IndexOf(rp);
                else
                    awayPitcherIndex =
                        awayTeam.roster.IndexOf(rp);

                AddPlay("PITCHING CHANGE: " +
                        rp.FullName() + " (" +
                        (rp.bullpenRole != "" ?
                         rp.bullpenRole : "RP") +
                        ") enters the game");

                Destroy(picker);
                RefreshGameDisplay();
            });
        }
    }

    void OnIBB()
    {
        Team batting = isTopInning ? awayTeam : homeTeam;

        if (gameOnBase1 && gameOnBase2 && gameOnBase3)
        {
            AddRuns(batting, 1);
            AddPlay("IBB — RBI WALK! Run scores!");
        }
        else
        {
            gameOnBase3 =
                (gameOnBase2 && gameOnBase1)
                ? true : gameOnBase3;
            gameOnBase2 =
                gameOnBase1 ? true : gameOnBase2;
            gameOnBase1 = true;
        }

        List<Player> batters = batting?.roster?.FindAll(
            p => p.position != "SP" &&
                 p.position != "RP");
        if (batters != null && batters.Count > 0)
        {
            int idx = isTopInning
                ? awayBatterIndex % batters.Count
                : homeBatterIndex % batters.Count;
            AddPlay("INTENTIONAL WALK: " +
                    batters[idx].FullName());
        }

        AdvanceBatter(batting);
        RefreshGameDisplay();
    }

    // -------------------------------------------------------
    // BATTING DECISIONS
    // -------------------------------------------------------
    void OnTakePitch()
    {
        if (!gameInProgress || gameOver) return;

        float roll = Random.value;
        if (roll < 0.45f)
        {
            balls++;
            AddPlay("Ball " + balls);
            if (balls >= 4)
            {
                Team batting =
                    isTopInning ? awayTeam : homeTeam;
                ProcessAtBatResult("WALK",
                    GetCurrentBatter(), batting);
            }
        }
        else
        {
            strikes++;
            AddPlay("Strike " + strikes + " — looking");
            if (strikes >= 3)
            {
                Team batting =
                    isTopInning ? awayTeam : homeTeam;
                ProcessAtBatResult("STRIKEOUT",
                    GetCurrentBatter(), batting);
            }
        }

        RefreshGameDisplay();
    }

    void OnBunt()
    {
        if (!gameInProgress || gameOver) return;

        Team   batting = isTopInning ? awayTeam : homeTeam;
        Player batter  = GetCurrentBatter();
        if (batter == null) return;

        if (Random.value < 0.65f)
        {
            outs++;
            if (gameOnBase2) gameOnBase3 = true;
            if (gameOnBase1) gameOnBase2 = true;
            AddPlay(batter.FullName() + " — SACRIFICE BUNT");
            AdvanceBatter(batting);
            CheckInningOver();
        }
        else
        {
            ProcessAtBatResult("SINGLE", batter, batting);
            AddPlay(batter.FullName() + " — BUNT SINGLE!");
        }

        RefreshGameDisplay();
    }

    Player GetCurrentBatter()
    {
        Team batting = isTopInning ? awayTeam : homeTeam;
        if (batting?.roster == null) return null;

        List<Player> batters = batting.roster.FindAll(
            p => p.position != "SP" &&
                 p.position != "RP");
        if (batters.Count == 0) return null;

        int idx = isTopInning
            ? awayBatterIndex % batters.Count
            : homeBatterIndex % batters.Count;
        return batters[idx];
    }

    void OnSimRestOfGame()
    {
        int maxIterations = 500;
        int iterations    = 0;

        while (!gameOver && gameInProgress &&
               iterations < maxIterations)
        {
            Team batting  = isTopInning ? awayTeam : homeTeam;
            Team pitching = isTopInning ? homeTeam : awayTeam;

            if (batting?.roster == null) break;

            List<Player> batters = batting.roster.FindAll(
                p => p.position != "SP" &&
                     p.position != "RP");
            List<Player> pitchers = pitching.roster.FindAll(
                p => p.position == "SP" ||
                     p.position == "RP");

            if (batters.Count == 0 ||
                pitchers.Count == 0) break;

            Player batter = batters[
                isTopInning
                    ? awayBatterIndex % batters.Count
                    : homeBatterIndex % batters.Count];

            Player pitcher = pitchers[
                isTopInning
                    ? homePitcherIndex % pitchers.Count
                    : awayPitcherIndex % pitchers.Count];

            string result = SimulateAtBat(batter, pitcher);
            ProcessAtBatResult(result, batter, batting);

            if (currentInning > 9 &&
                homeScore != awayScore &&
                !isTopInning)
            {
                EndGame();
                break;
            }

            iterations++;
        }

        if (!gameOver) EndGame();
        RefreshGameDisplay();
    }

    void SetGameText(string objName, string value)
    {
        if (liveGameScreen == null) return;
        Transform t = liveGameScreen.transform.Find(objName);
        if (t == null) return;
        TextMeshProUGUI tmp =
            t.GetComponent<TextMeshProUGUI>();
        if (tmp != null) tmp.text = value;
    }

    void SetGameTextColor(string objName,
                           string value, Color color)
    {
        if (liveGameScreen == null) return;
        Transform t = liveGameScreen.transform.Find(objName);
        if (t == null) return;
        TextMeshProUGUI tmp =
            t.GetComponent<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.text  = value;
            tmp.color = color;
        }
    }

    // -------------------------------------------------------
    // BOX SCORE SCREEN
    // -------------------------------------------------------
    GameObject BuildBoxScoreScreen(GameObject canvas)
    {
        GameObject screen =
            CreateScreen(canvas, "BoxScore");

        BuildImageBackground(screen, "background");
        AddImage(screen, "Overlay",
            new Color(0.03f, 0.05f, 0.10f, 0.95f),
            Vector2.zero, new Vector2(390, 844));

        AddImage(screen, "Header", SURFACE,
            new Vector2(0, 380), new Vector2(390, 88));

        AddText(screen, "BSTitle",
            "BOX SCORE", 22, TEXT,
            new Vector2(0, 388),
            new Vector2(300, 36), FontStyles.Bold);

        // Final score bar
        AddImage(screen, "ScoreBG",
            new Color(0.06f, 0.12f, 0.20f, 1f),
            new Vector2(0, 310), new Vector2(374, 60));

        AddText(screen, "BSAwayTeam",
            "AWY", 12, SUBTEXT,
            new Vector2(-110, 320),
            new Vector2(120, 28));

        AddText(screen, "BSAwayScore",
            "0", 28, TEXT,
            new Vector2(-20, 310),
            new Vector2(50, 48), FontStyles.Bold);

        AddText(screen, "BSVs",
            "—", 16, SUBTEXT,
            new Vector2(0, 310),
            new Vector2(24, 48));

        AddText(screen, "BSHomeScore",
            "0", 28, TEXT,
            new Vector2(20, 310),
            new Vector2(50, 48), FontStyles.Bold);

        AddText(screen, "BSHomeTeam",
            "HME", 12, SUBTEXT,
            new Vector2(110, 320),
            new Vector2(120, 28));

        AddText(screen, "BSResult",
            "", 12, GOLD,
            new Vector2(0, 282),
            new Vector2(300, 22));

        // MY TEAM / OPPONENT tabs
        AddImage(screen, "BSTabBG", SURFACE,
            new Vector2(0, 258), new Vector2(374, 32));

        GameObject homeTab = CreateButton(screen,
            "MY TEAM", SURFACE, GOLD,
            new Vector2(-95, 258),
            new Vector2(170, 32), 11);
        homeTab.name = "BSHomeTab";
        GetButton(homeTab).onClick.AddListener(() =>
        {
            boxScoreShowingHome = true;
            RefreshBoxScoreRows();
            SetBSTabColors();
        });

        GameObject awayTab = CreateButton(screen,
            "OPPONENT", SURFACE, SUBTEXT,
            new Vector2(95, 258),
            new Vector2(170, 32), 11);
        awayTab.name = "BSAwayTab";
        GetButton(awayTab).onClick.AddListener(() =>
        {
            boxScoreShowingHome = false;
            RefreshBoxScoreRows();
            SetBSTabColors();
        });

        AddImage(screen, "BSTabLine", RED,
            new Vector2(-95, 243), new Vector2(170, 3));

        // Batting column headers
        AddImage(screen, "BatHeader",
            new Color(0.08f, 0.18f, 0.30f, 1f),
            new Vector2(0, 228), new Vector2(374, 24));

        AddText(screen, "BatHeaderLabel",
            "BATTING", 10, GOLD,
            new Vector2(-95, 228),
            new Vector2(130, 24), FontStyles.Bold);

        AddText(screen, "BatAB",  "AB",  9, SUBTEXT,
            new Vector2(50,  228), new Vector2(28, 24));
        AddText(screen, "BatH",   "H",   9, SUBTEXT,
            new Vector2(78,  228), new Vector2(28, 24));
        AddText(screen, "BatHR",  "HR",  9, SUBTEXT,
            new Vector2(106, 228), new Vector2(28, 24));
        AddText(screen, "BatRBI", "RBI", 9, SUBTEXT,
            new Vector2(134, 228), new Vector2(28, 24));
        AddText(screen, "BatAVG", "AVG", 9, SUBTEXT,
            new Vector2(162, 228), new Vector2(36, 24));

        // 9 batter rows
        for (int i = 0; i < 9; i++)
        {
            float rowY = 206f - (i * 22f);
            Color rowColor = i % 2 == 0
                ? new Color(0.05f, 0.10f, 0.18f, 0.97f)
                : new Color(0.04f, 0.08f, 0.15f, 0.97f);

            GameObject row =
                new GameObject("BatRow_" + i);
            row.transform.SetParent(
                screen.transform, false);
            RectTransform rRT =
                row.AddComponent<RectTransform>();
            rRT.anchoredPosition = new Vector2(0, rowY);
            rRT.sizeDelta        = new Vector2(374, 20);
            row.AddComponent<Image>().color = rowColor;

            AddTextToParent(row, "Name", "",
                8.5f, TEXT, new Vector2(-85f, 0f),
                new Vector2(155f, 20f),
                TextAlignmentOptions.MidlineLeft);

            AddTextToParent(row, "AB", "",
                9f, TEXT, new Vector2(50f, 0f),
                new Vector2(28f, 20f),
                TextAlignmentOptions.Midline);

            AddTextToParent(row, "H", "",
                9f, TEXT, new Vector2(78f, 0f),
                new Vector2(28f, 20f),
                TextAlignmentOptions.Midline);

            AddTextToParent(row, "HR", "",
                9f, TEXT, new Vector2(106f, 0f),
                new Vector2(28f, 20f),
                TextAlignmentOptions.Midline);

            AddTextToParent(row, "RBI", "",
                9f, TEXT, new Vector2(134f, 0f),
                new Vector2(28f, 20f),
                TextAlignmentOptions.Midline);

            AddTextToParent(row, "AVG", "",
                9f, GOLD, new Vector2(162f, 0f),
                new Vector2(36f, 20f),
                TextAlignmentOptions.Midline);
        }

        // Pitching column headers
        AddImage(screen, "PitHeader",
            new Color(0.08f, 0.18f, 0.30f, 1f),
            new Vector2(0, -2), new Vector2(374, 24));

        AddText(screen, "PitHeaderLabel",
            "PITCHING", 10, GOLD,
            new Vector2(-95, -2),
            new Vector2(130, 24), FontStyles.Bold);

        AddText(screen, "PitIP", "IP", 9, SUBTEXT,
            new Vector2(50,  -2), new Vector2(28, 24));
        AddText(screen, "PitH",  "H",  9, SUBTEXT,
            new Vector2(78,  -2), new Vector2(28, 24));
        AddText(screen, "PitER", "ER", 9, SUBTEXT,
            new Vector2(106, -2), new Vector2(28, 24));
        AddText(screen, "PitBB", "BB", 9, SUBTEXT,
            new Vector2(134, -2), new Vector2(28, 24));
        AddText(screen, "PitK",  "K",  9, SUBTEXT,
            new Vector2(162, -2), new Vector2(28, 24));

        // 3 pitcher rows
        for (int i = 0; i < 3; i++)
        {
            float rowY = -24f - (i * 22f);
            Color rowColor = i % 2 == 0
                ? new Color(0.05f, 0.10f, 0.18f, 0.97f)
                : new Color(0.04f, 0.08f, 0.15f, 0.97f);

            GameObject row =
                new GameObject("PitRow_" + i);
            row.transform.SetParent(
                screen.transform, false);
            RectTransform rRT =
                row.AddComponent<RectTransform>();
            rRT.anchoredPosition = new Vector2(0, rowY);
            rRT.sizeDelta        = new Vector2(374, 20);
            row.AddComponent<Image>().color = rowColor;

            AddTextToParent(row, "Name", "",
                8.5f, TEXT, new Vector2(-85f, 0f),
                new Vector2(155f, 20f),
                TextAlignmentOptions.MidlineLeft);

            AddTextToParent(row, "IP", "",
                9f, TEXT, new Vector2(50f, 0f),
                new Vector2(28f, 20f),
                TextAlignmentOptions.Midline);

            AddTextToParent(row, "H", "",
                9f, TEXT, new Vector2(78f, 0f),
                new Vector2(28f, 20f),
                TextAlignmentOptions.Midline);

            AddTextToParent(row, "ER", "",
                9f, TEXT, new Vector2(106f, 0f),
                new Vector2(28f, 20f),
                TextAlignmentOptions.Midline);

            AddTextToParent(row, "BB", "",
                9f, TEXT, new Vector2(134f, 0f),
                new Vector2(28f, 20f),
                TextAlignmentOptions.Midline);

            AddTextToParent(row, "K", "",
                9f, TEXT, new Vector2(162f, 0f),
                new Vector2(28f, 20f),
                TextAlignmentOptions.Midline);
        }

        // HR log
        AddImage(screen, "HRBg", SURFACE,
            new Vector2(0, -100), new Vector2(374, 24));
        AddText(screen, "HRLog",
            "", 10, GOLD,
            new Vector2(0, -100),
            new Vector2(360, 24));

        // Continue button
        GameObject contBtn = CreateButton(screen,
            "CONTINUE", RED, TEXT,
            new Vector2(0, -155),
            new Vector2(280, 52), 16);
        GetButton(contBtn).onClick.AddListener(() =>
        {
            ShowScreen(teamScreen);
            if (currentTeam != null)
                PopulateTeamScreen(currentTeam);
        });

        screen.SetActive(false);
        return screen;
    }

    // Set tab highlight colors
    void SetBSTabColors()
    {
        Transform homeTab =
            boxScoreScreen.transform.Find("BSHomeTab");
        Transform awayTab =
            boxScoreScreen.transform.Find("BSAwayTab");
        Transform tabLine =
            boxScoreScreen.transform.Find("BSTabLine");

        if (homeTab != null)
        {
            TextMeshProUGUI t =
                homeTab.GetComponentInChildren<TextMeshProUGUI>();
            if (t != null)
                t.color = boxScoreShowingHome ? GOLD : SUBTEXT;
        }
        if (awayTab != null)
        {
            TextMeshProUGUI t =
                awayTab.GetComponentInChildren<TextMeshProUGUI>();
            if (t != null)
                t.color = boxScoreShowingHome ? SUBTEXT : GOLD;
        }
        if (tabLine != null)
        {
            RectTransform rt =
                tabLine.GetComponent<RectTransform>();
            if (rt != null)
                rt.anchoredPosition = new Vector2(
                    boxScoreShowingHome ? -95f : 95f, 243);
        }
    }

    // Refresh batting and pitching rows for selected team
    void RefreshBoxScoreRows()
    {
        Team showTeam = boxScoreShowingHome
            ? homeTeam : awayTeam;

        // Batters
        List<Player> batters = new List<Player>();
        if (showTeam?.roster != null)
        {
            batters = showTeam.roster.FindAll(
                p => p.position != "SP" &&
                     p.position != "RP" &&
                     p.atBats > 0);
            batters.Sort((a, b) =>
                b.hits.CompareTo(a.hits));
        }

        string hrLog = "";

        for (int i = 0; i < 9; i++)
        {
            Transform row = boxScoreScreen.transform
                .Find("BatRow_" + i);
            if (row == null) continue;

            if (i < batters.Count)
            {
                Player p = batters[i];
                // Show W/L next to pitcher name
                string pitRecord = "";
                if (p.wins > 0 || p.losses > 0)
                    pitRecord = p.wins > p.losses
                        ? " (W " + p.seasonWins + "-" +
                          p.seasonLosses + ")"
                        : " (L " + p.seasonWins + "-" +
                          p.seasonLosses + ")";
                SetBSRowText(row, "Name",
                    p.FullName() + pitRecord);
                SetBSRowText(row, "AB",
                    p.atBats.ToString());
                SetBSRowText(row, "H",
                    p.hits.ToString());
                SetBSRowText(row, "HR",
                    p.homeRuns.ToString());
                SetBSRowText(row, "RBI",
                    p.rbi.ToString());
                SetBSRowText(row, "AVG",
                    p.atBats > 0
                        ? ((float)p.hits / p.atBats)
                           .ToString("F3")
                        : ".000");

                if (p.homeRuns > 0)
                {
                    if (hrLog != "") hrLog += "  ";
                    hrLog += p.lastName +
                             " (" + p.homeRuns + ")";
                }
            }
            else
            {
                SetBSRowText(row, "Name", "");
                SetBSRowText(row, "AB",   "");
                SetBSRowText(row, "H",    "");
                SetBSRowText(row, "HR",   "");
                SetBSRowText(row, "RBI",  "");
                SetBSRowText(row, "AVG",  "");
            }
        }

        SetBSText("HRLog",
            hrLog != "" ? "HR: " + hrLog : "");

        // Pitchers
        List<Player> pitchers = new List<Player>();
        if (showTeam?.roster != null)
        {
            pitchers = showTeam.roster.FindAll(
                p => (p.position == "SP" ||
                      p.position == "RP") &&
                     p.inningsPitched > 0);
        }

        for (int i = 0; i < 3; i++)
        {
            Transform row = boxScoreScreen.transform
                .Find("PitRow_" + i);
            if (row == null) continue;

            if (i < pitchers.Count)
            {
                Player p = pitchers[i];
                SetBSRowText(row, "Name", p.FullName());
                SetBSRowText(row, "IP",
                    p.inningsPitched.ToString());
                SetBSRowText(row, "H",
                    p.hitsAllowed.ToString());
                SetBSRowText(row, "ER",
                    p.earnedRuns.ToString());
                SetBSRowText(row, "BB",
                    p.walksAllowed.ToString());
                SetBSRowText(row, "K",
                    p.strikeoutsThrown.ToString());
            }
            else
            {
                SetBSRowText(row, "Name", "");
                SetBSRowText(row, "IP",   "");
                SetBSRowText(row, "H",    "");
                SetBSRowText(row, "ER",   "");
                SetBSRowText(row, "BB",   "");
                SetBSRowText(row, "K",    "");
            }
        }
    }

    // Show box score after game ends
    public void ShowBoxScore(
        Team home, Team away,
        int homeScoreFinal, int awayScoreFinal)
    {
        if (boxScoreScreen == null) return;

        SetBSText("BSAwayTeam",
            away.city + " " + away.nickname);
        SetBSText("BSHomeTeam",
            home.city + " " + home.nickname);
        SetBSText("BSAwayScore",
            awayScoreFinal.ToString());
        SetBSText("BSHomeScore",
            homeScoreFinal.ToString());

        bool playerWon =
            (home == GetMyTeam() &&
             homeScoreFinal > awayScoreFinal) ||
            (away == GetMyTeam() &&
             awayScoreFinal > homeScoreFinal);

        SetBSText("BSResult",
            playerWon ? "YOU WIN!" : "YOU LOSE");

        Transform resultT =
            boxScoreScreen.transform.Find("BSResult");
        if (resultT != null)
        {
            TextMeshProUGUI tmp =
                resultT.GetComponent<TextMeshProUGUI>();
            if (tmp != null)
                tmp.color = playerWon ? GREEN : RED;
        }

        // Default to player's team
        boxScoreShowingHome = (home == GetMyTeam());
        RefreshBoxScoreRows();
        SetBSTabColors();

        ShowScreen(boxScoreScreen);
    }

    void SetBSText(string objName, string value)
    {
        if (boxScoreScreen == null) return;
        Transform t =
            boxScoreScreen.transform.Find(objName);
        if (t == null) return;
        TextMeshProUGUI tmp =
            t.GetComponent<TextMeshProUGUI>();
        if (tmp != null) tmp.text = value;
    }

    void SetBSRowText(Transform row, string name,
                       string value)
    {
        Transform t = row.Find(name);
        if (t == null) return;
        TextMeshProUGUI tmp =
            t.GetComponent<TextMeshProUGUI>();
        if (tmp != null) tmp.text = value;
    }


    // -------------------------------------------------------
    // PRE-GAME SCREEN
    // -------------------------------------------------------
    GameObject BuildPreGameScreen(GameObject canvas)
    {
        GameObject screen =
            CreateScreen(canvas, "PreGame");

        BuildImageBackground(screen, "background");
        AddImage(screen, "Overlay",
            new Color(0.03f, 0.05f, 0.10f, 0.92f),
            Vector2.zero, new Vector2(390, 844));

        // Header
        AddImage(screen, "Header", SURFACE,
            new Vector2(0, 380), new Vector2(390, 88));

        AddText(screen, "PGMatchup",
            "NYA vs TOR", 20, TEXT,
            new Vector2(0, 390),
            new Vector2(340, 36), FontStyles.Bold);

        AddText(screen, "PGGameNum",
            "GAME 1", 13, GOLD,
            new Vector2(0, 358),
            new Vector2(300, 24));

        // Team tabs
        AddImage(screen, "PGTabBG", SURFACE,
            new Vector2(0, 318), new Vector2(390, 36));

        GameObject homeTab = CreateButton(screen,
            "HOME LINEUP", SURFACE, GOLD,
            new Vector2(-97, 318),
            new Vector2(178, 36), 11);
        homeTab.name = "PGHomeTab";
        GetButton(homeTab).onClick.AddListener(() =>
        {
            pgShowingHome = true;
            RefreshPreGameLineup();
            SetPGTabColors();
        });

        GameObject awayTab = CreateButton(screen,
            "AWAY LINEUP", SURFACE, SUBTEXT,
            new Vector2(97, 318),
            new Vector2(178, 36), 11);
        awayTab.name = "PGAwayTab";
        GetButton(awayTab).onClick.AddListener(() =>
        {
            pgShowingHome = false;
            RefreshPreGameLineup();
            SetPGTabColors();
        });

        AddImage(screen, "PGTabLine", RED,
            new Vector2(-97, 300), new Vector2(178, 3));

        // Starter display
        AddImage(screen, "PGStarterBG",
            new Color(0.06f, 0.12f, 0.20f, 1f),
            new Vector2(0, 270), new Vector2(374, 48));

        AddText(screen, "PGStarterLabel",
            "STARTING PITCHER", 9, SUBTEXT,
            new Vector2(-80, 283),
            new Vector2(160, 18));

        AddText(screen, "PGStarterName",
            "", 14, TEXT,
            new Vector2(-80, 264),
            new Vector2(220, 24), FontStyles.Bold);

        AddText(screen, "PGStarterStats",
            "", 11, GOLD,
            new Vector2(130, 270),
            new Vector2(120, 48));

        // Column headers
        AddImage(screen, "PGColHeader",
            new Color(0.08f, 0.18f, 0.30f, 1f),
            new Vector2(0, 243), new Vector2(374, 24));

        AddText(screen, "PGColPos", "#  POS", 9, GOLD,
            new Vector2(-130, 243), new Vector2(80, 24));
        AddText(screen, "PGColName", "PLAYER", 9, GOLD,
            new Vector2(-30, 243), new Vector2(120, 24));
        AddText(screen, "PGColOVR", "OVR", 9, GOLD,
            new Vector2(110, 243), new Vector2(40, 24));
        AddText(screen, "PGColAVG", "AVG", 9, GOLD,
            new Vector2(155, 243), new Vector2(40, 24));

        // 9 lineup rows
        for (int i = 0; i < 9; i++)
        {
            float rowY = 221f - (i * 22f);
            Color rowColor = i % 2 == 0
                ? new Color(0.05f, 0.10f, 0.18f, 0.97f)
                : new Color(0.04f, 0.08f, 0.15f, 0.97f);

            GameObject row =
                new GameObject("PGRow_" + i);
            row.transform.SetParent(
                screen.transform, false);
            RectTransform rRT =
                row.AddComponent<RectTransform>();
            rRT.anchoredPosition = new Vector2(0, rowY);
            rRT.sizeDelta        = new Vector2(374, 20);
            row.AddComponent<Image>().color = rowColor;

            // Batting order number
            AddTextToParent(row, "Num",
                (i + 1) + ".",
                9f, SUBTEXT, new Vector2(-168f, 0f),
                new Vector2(24f, 20f),
                TextAlignmentOptions.Midline);

            // Position
            AddTextToParent(row, "Pos", "",
                9f, RED, new Vector2(-145f, 0f),
                new Vector2(36f, 20f),
                TextAlignmentOptions.Midline);

            // Player name
            AddTextToParent(row, "Name", "",
                9f, TEXT, new Vector2(-30f, 0f),
                new Vector2(160f, 20f),
                TextAlignmentOptions.MidlineLeft);

            // Overall
            AddTextToParent(row, "OVR", "",
                9f, SUBTEXT, new Vector2(110f, 0f),
                new Vector2(40f, 20f),
                TextAlignmentOptions.Midline);

            // Season AVG
            AddTextToParent(row, "AVG", "",
                9f, GOLD, new Vector2(155f, 0f),
                new Vector2(40f, 20f),
                TextAlignmentOptions.Midline);
        }

        // Weather / conditions bar
        AddImage(screen, "PGCondBG", SURFACE,
            new Vector2(0, 15), new Vector2(374, 30));

        AddText(screen, "PGConditions",
            "⚾  CLEAR  •  72°F  •  Wind: 5mph OUT",
            10, SUBTEXT,
            new Vector2(0, 15),
            new Vector2(360, 30));

        // Start game button
        GameObject startBtn = CreateButton(screen,
            "PLAY BALL!", RED, TEXT,
            new Vector2(0, -50),
            new Vector2(300, 62), 22);
        GetButton(startBtn).onClick.AddListener(() =>
            OnStartPreGame());
        startBtn.name = "PGStartBtn";

        // Back button
        GameObject backBtn = CreateButton(screen,
            "BACK", SURFACE, SUBTEXT,
            new Vector2(0, -130),
            new Vector2(160, 40), 13);
        AddBorder(backBtn, BORDER, 2);
        GetButton(backBtn).onClick.AddListener(() =>
            ShowScreen(teamScreen));

        screen.SetActive(false);
        return screen;
    }

    // Show pre-game screen before a live game
    public void ShowPreGame(Team home, Team away)
    {
        pgHomeTeam      = home;
        pgAwayTeam      = away;
        pgShowingHome   = true;
        pgGameNumber++;

        // Build optimal lineups
        pgHomeLineup = BuildOptimalLineup(home);
        pgAwayLineup = BuildOptimalLineup(away);

        // Set matchup header
        SetPGText("PGMatchup",
            away.abbreviation + "  vs  " +
            home.abbreviation);
        SetPGText("PGGameNum",
            "GAME " + pgGameNumber);

        // Random weather
        string[] conditions =
        {
            "CLEAR  •  72F  •  Wind: 5mph OUT",
            "CLOUDY  •  65F  •  Wind: 8mph IN",
            "SUNNY  •  81F  •  Calm winds",
            "OVERCAST  •  59F  •  Wind: 12mph OUT",
            "PARTLY CLOUDY  •  74F  •  Wind: 3mph"
        };
        SetPGText("PGConditions",
            conditions[Random.Range(
                0, conditions.Length)]);

        RefreshPreGameLineup();
        SetPGTabColors();
        ShowScreen(preGameScreen);
    }

    // Build a simple optimal batting lineup
    List<Player> BuildOptimalLineup(Team team)
    {
        if (team?.roster == null)
            return new List<Player>();

        List<Player> batters = team.roster.FindAll(
            p => p.position != "SP" &&
                 p.position != "RP");

        // Sort by overall rating
        batters.Sort((a, b) =>
            b.overall.CompareTo(a.overall));

        // Take top 9
        List<Player> lineup = new List<Player>();
        for (int i = 0; i < 9 && i < batters.Count; i++)
            lineup.Add(batters[i]);

        return lineup;
    }

    // Refresh the lineup rows
    void RefreshPreGameLineup()
    {
        if (preGameScreen == null) return;

        Team showTeam   = pgShowingHome ?
            pgHomeTeam : pgAwayTeam;
        List<Player> lu = pgShowingHome ?
            pgHomeLineup : pgAwayLineup;

        // Show starting pitcher
        if (showTeam?.roster != null)
        {
            GameManager gm =
                FindFirstObjectByType<GameManager>();
            List<Player> sps = showTeam.roster.FindAll(
                p => p.position == "SP");

            if (sps.Count > 0 && gm != null)
            {
                int rotIdx =
                    gm.GetStartingPitcherIndex(
                        showTeam.abbreviation) %
                    sps.Count;
                Player starter = sps[rotIdx];

                SetPGText("PGStarterName",
                    starter.FullName());

                string wl = starter.seasonWins + "-" +
                            starter.seasonLosses;
                string era = starter.seasonInningsPitched > 0
                    ? starter.SeasonERA().ToString("F2")
                    : "-.--";
                SetPGText("PGStarterStats",
                    wl + "  " + era + " ERA\n" +
                    starter.throwingArm + "HP  " +
                    starter.pitching + " OVR");
            }
        }

        // Fill lineup rows
        for (int i = 0; i < 9; i++)
        {
            Transform row = preGameScreen.transform
                .Find("PGRow_" + i);
            if (row == null) continue;

            if (i < lu.Count)
            {
                Player p = lu[i];
                SetPGRowText(row, "Pos", p.position);
                SetPGRowText(row, "Name", p.FullName());
                SetPGRowText(row, "OVR",
                    p.overall.ToString());
                SetPGRowText(row, "AVG",
                    p.seasonAtBats > 0
                        ? p.SeasonBattingAverage()
                           .ToString("F3")
                        : "---");
            }
            else
            {
                SetPGRowText(row, "Pos",  "");
                SetPGRowText(row, "Name", "");
                SetPGRowText(row, "OVR",  "");
                SetPGRowText(row, "AVG",  "");
            }
        }
    }

    // Set pre-game tab colors
    void SetPGTabColors()
    {
        Transform homeTab =
            preGameScreen.transform.Find("PGHomeTab");
        Transform awayTab =
            preGameScreen.transform.Find("PGAwayTab");
        Transform tabLine =
            preGameScreen.transform.Find("PGTabLine");

        if (homeTab != null)
        {
            TextMeshProUGUI t =
                homeTab.GetComponentInChildren
                    <TextMeshProUGUI>();
            if (t != null)
                t.color = pgShowingHome ? GOLD : SUBTEXT;
        }
        if (awayTab != null)
        {
            TextMeshProUGUI t =
                awayTab.GetComponentInChildren
                    <TextMeshProUGUI>();
            if (t != null)
                t.color = pgShowingHome ? SUBTEXT : GOLD;
        }
        if (tabLine != null)
        {
            RectTransform rt =
                tabLine.GetComponent<RectTransform>();
            if (rt != null)
                rt.anchoredPosition = new Vector2(
                    pgShowingHome ? -97f : 97f, 300);
        }
    }

    // Start the actual live game from pre-game screen
    void OnStartPreGame()
    {
        if (pgHomeTeam == null || pgAwayTeam == null)
            return;
        StartLiveGame(pgHomeTeam, pgAwayTeam);
    }

    void SetPGText(string objName, string value)
    {
        if (preGameScreen == null) return;
        Transform t =
            preGameScreen.transform.Find(objName);
        if (t == null) return;
        TextMeshProUGUI tmp =
            t.GetComponent<TextMeshProUGUI>();
        if (tmp != null) tmp.text = value;
    }

    void SetPGRowText(Transform row, string name,
                       string value)
    {
        Transform t = row.Find(name);
        if (t == null) return;
        TextMeshProUGUI tmp =
            t.GetComponent<TextMeshProUGUI>();
        if (tmp != null) tmp.text = value;
    }



    // -------------------------------------------------------
    // BOTTOM NAV
    // -------------------------------------------------------
    void BuildBottomNav(GameObject screen)
    {
        GameObject nav = new GameObject("BottomNav");
        nav.transform.SetParent(screen.transform, false);
        RectTransform navRT =
            nav.AddComponent<RectTransform>();
        navRT.anchoredPosition = new Vector2(0, -390);
        navRT.sizeDelta        = new Vector2(390, 80);
        nav.AddComponent<Image>().color = SURFACE;

        AddImage(nav, "Border", BORDER,
            new Vector2(0, 38), new Vector2(390, 1));

        string[] labels = {
            "MY TEAM", "STAND", "TRADE", "DRAFT", "FA" };
        float[] xPos = { -155f, -78f, 0f, 78f, 155f };

        for (int i = 0; i < labels.Length; i++)
        {
            int idx = i;
            GameObject nb =
                new GameObject("Nav_" + labels[i]);
            nb.transform.SetParent(nav.transform, false);
            RectTransform nRT =
                nb.AddComponent<RectTransform>();
            nRT.anchoredPosition = new Vector2(xPos[i], 0);
            nRT.sizeDelta        = new Vector2(60, 70);
            nb.AddComponent<Image>().color = Color.clear;
            Button nBtn = nb.AddComponent<Button>();

            GameObject nt = new GameObject("T");
            nt.transform.SetParent(nb.transform, false);
            TextMeshProUGUI nTMP =
                nt.AddComponent<TextMeshProUGUI>();
            nTMP.text      = labels[i];
            nTMP.fontSize  = 9;
            nTMP.color     = idx == 0 ? GOLD : SUBTEXT;
            nTMP.fontStyle = FontStyles.Bold;
            nTMP.alignment = TextAlignmentOptions.Midline;
            RectTransform nTRT =
                nt.GetComponent<RectTransform>();
            nTRT.anchorMin = Vector2.zero;
            nTRT.anchorMax = Vector2.one;
            nTRT.offsetMin = Vector2.zero;
            nTRT.offsetMax = Vector2.zero;

            nBtn.onClick.AddListener(() =>
            {
                if (idx == 0) ShowScreen(teamScreen);
                if (idx == 1)
                {
                    ShowScreen(standingsScreen);
                    ShowDivision(0);
                }
                if (idx == 2)
                {
                    ShowScreen(tradeScreen);
                    InitTradeScreen();
                }
                if (idx == 3)
                {
                    ShowScreen(draftScreen);
                    InitDraftScreen();
                }
                if (idx == 4)
                {
                    ShowScreen(faScreen);
                    InitFAScreen();
                }
            });
        }
    }

    // -------------------------------------------------------
    // SCREEN MANAGEMENT
    // -------------------------------------------------------
    void ShowScreen(GameObject screen)
    {
        if (mainMenuScreen   != null)
            mainMenuScreen.SetActive(false);
        if (gmNameScreen     != null)
            gmNameScreen.SetActive(false);
        if (continueScreen   != null)
            continueScreen.SetActive(false);
        if (teamSelectScreen != null)
            teamSelectScreen.SetActive(false);
        if (teamScreen       != null)
            teamScreen.SetActive(false);
        if (standingsScreen  != null)
            standingsScreen.SetActive(false);
        if (tradeScreen      != null)
            tradeScreen.SetActive(false);
        if (draftScreen      != null)
            draftScreen.SetActive(false);
        if (faScreen         != null)
            faScreen.SetActive(false);
        if (liveGameScreen   != null)
            liveGameScreen.SetActive(false);
        if (boxScoreScreen   != null)
            boxScoreScreen.SetActive(false);
        if (preGameScreen    != null)
            preGameScreen.SetActive(false);
        if (screen           != null)
            screen.SetActive(true);
        currentScreen = screen;
    }

    // -------------------------------------------------------
    // IMAGE BACKGROUND
    // -------------------------------------------------------
    void BuildImageBackground(GameObject parent,
                               string imageName)
    {
        Sprite bg = Resources.Load<Sprite>(imageName);

        if (bg == null)
        {
            Debug.LogError("Could not find: " + imageName);
            AddImage(parent, "BG", BG,
                Vector2.zero, new Vector2(390, 844));
            return;
        }

        GameObject bgObj = new GameObject("BG");
        bgObj.transform.SetParent(parent.transform, false);
        RectTransform rt =
            bgObj.AddComponent<RectTransform>();
        rt.anchorMin      = Vector2.zero;
        rt.anchorMax      = Vector2.one;
        rt.offsetMin      = Vector2.zero;
        rt.offsetMax      = Vector2.zero;
        Image img         = bgObj.AddComponent<Image>();
        img.sprite        = bg;
        img.preserveAspect = false;

        AddImage(parent, "Overlay", Hex("#080e1aCC"),
            Vector2.zero, new Vector2(390, 844));
    }

    // -------------------------------------------------------
    // UI HELPERS
    // -------------------------------------------------------
    GameObject CreateScreen(GameObject parent, string name)
    {
        GameObject s     = new GameObject(name);
        s.transform.SetParent(parent.transform, false);
        RectTransform rt = s.AddComponent<RectTransform>();
        rt.anchorMin      = Vector2.zero;
        rt.anchorMax      = Vector2.one;
        rt.offsetMin      = Vector2.zero;
        rt.offsetMax      = Vector2.zero;
        return s;
    }

    GameObject AddImage(GameObject parent, string name,
                         Color color, Vector2 pos,
                         Vector2 size,
                         bool circle = false)
    {
        GameObject obj   = new GameObject(name);
        obj.transform.SetParent(parent.transform, false);
        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta        = size;
        obj.AddComponent<Image>().color = color;
        return obj;
    }

    GameObject AddText(GameObject parent, string name,
                        string content, float fontSize,
                        Color color, Vector2 pos,
                        Vector2 size,
                        FontStyles style = FontStyles.Normal)
    {
        GameObject obj   = new GameObject(name);
        obj.transform.SetParent(parent.transform, false);
        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta        = size;
        TextMeshProUGUI tmp =
            obj.AddComponent<TextMeshProUGUI>();
        tmp.text      = content;
        tmp.fontSize  = fontSize;
        tmp.color     = color;
        tmp.fontStyle = style;
        tmp.alignment = TextAlignmentOptions.Center;
        return obj;
    }

    void AddTextToParent(GameObject parent, string name,
                          string content, float fontSize,
                          Color color, Vector2 pos,
                          Vector2 size,
                          TextAlignmentOptions align)
    {
        GameObject obj   = new GameObject(name);
        obj.transform.SetParent(parent.transform, false);
        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta        = size;
        TextMeshProUGUI tmp =
            obj.AddComponent<TextMeshProUGUI>();
        tmp.text      = content;
        tmp.fontSize  = fontSize;
        tmp.color     = color;
        tmp.alignment = align;
    }

    GameObject CreateButton(GameObject parent,
                             string label,
                             Color bgColor,
                             Color textColor,
                             Vector2 pos, Vector2 size,
                             float fontSize)
    {
        GameObject btn   = new GameObject("Btn_" + label);
        btn.transform.SetParent(parent.transform, false);
        RectTransform rt = btn.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta        = size;
        btn.AddComponent<Image>().color = bgColor;
        btn.AddComponent<Button>();

        GameObject t     = new GameObject("T");
        t.transform.SetParent(btn.transform, false);
        TextMeshProUGUI tmp =
            t.AddComponent<TextMeshProUGUI>();
        tmp.text      = label;
        tmp.fontSize  = fontSize;
        tmp.color     = textColor;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        RectTransform tRT =
            t.GetComponent<RectTransform>();
        tRT.anchorMin = Vector2.zero;
        tRT.anchorMax = Vector2.one;
        tRT.offsetMin = Vector2.zero;
        tRT.offsetMax = Vector2.zero;

        return btn;
    }

    void AddBorder(GameObject obj, Color color,
                    float width)
    {
        Outline o        = obj.AddComponent<Outline>();
        o.effectColor    = color;
        o.effectDistance = new Vector2(width, width);
    }

    Button GetButton(GameObject obj)
    {
        return obj.GetComponent<Button>();
    }

    Color GetOverallColor(int overall)
    {
        if (overall >= 90) return Hex("#f5c842");
        if (overall >= 80) return Hex("#00e676");
        if (overall >= 70) return Hex("#e8edf5");
        if (overall >= 60) return Hex("#8899aa");
        return Hex("#e8192c");
    }

    static Color Hex(string hex)
    {
        Color c;
        ColorUtility.TryParseHtmlString(hex, out c);
        return c;
    }
}
