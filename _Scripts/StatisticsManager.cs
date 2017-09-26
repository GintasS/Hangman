using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Facebook.MiniJSON;
using Facebook.Unity;

public class StatisticsManager : MonoBehaviour {

    // Script that handles statistical part of the game.
    // Including Facebook LeaderBoards.

    #region [StatisticsManager]: Variables
    // Statistical variables.
    [Header("Statistics score")]
    public int StatsScore;
    public int StatsScoreFromWord;
    public int StatsFindingWordTimeInSeconds;

    // Varaibles for local store.
    [Header("Variables for local score")]
    [Range(1,10000)] public int LocalStatsScoreGivenPerWord;
    [Range(1, 100)]  public int LocalStatsPenaltyForSecondSpent;
    public int LocalStatsSuccessRateMinimum;
    public int LocalStatsSuccessRateHigh;

    public string LocalStatsSuccessMinimumWord,
              LocalStatsSuccessMediumWord,
              LocalStatsSuccessHighWord;

    // Data of the equiped Leader.
    [Header("Data of the equiped Leader")]
    public int CurrentEquipedLeaderHealthAdvantage;
    public int CurrentEquipedLeaderTimeAdvantage;
    public int CurrentEquipedLeaderGuessedWordAdvantage;
    public int CurrentWordRestartedTimes;

    public Sprite CurrentEquipedLeaderSprite,
                  DefaultLeaderSprite;

    // Variables for Facebook leaderboard.
    [Header("Facebook Leaderboard's variables")]
    public int FacebookStatsFriendsCount;
    public int FacebookPhotoIndex;
    public int FacebookStatsScoresCount;

    public List<string> CurrentUserFacebookFriendsIds,
                        CurrentUserFacebookFriendsName,
                        CurrentUserFacebookFriendsScores;

    public List<Texture2D> CurrentUserFacebookFriendsPhotos;

    [SerializeField] private GameObject StatisticsValueObject,
                                        StatisticsPageMain;

    [SerializeField] private RectTransform StatsSCrollViewContent;

    // Various bools.
    [Header("Various bool values")]
    [SerializeField] private bool FBStatisticsWasLoaded;
    public bool DataWasSaved;
    [SerializeField] private bool ResetData;

    // Other objects.
    [Header("Other objects")]
    public Image GuillotineSacrifice;

    // References for scripts.
    [Header("Script references")]
    [SerializeField] private FacebookScript FacebookS;
    [SerializeField] private Shop ShopS;
    [SerializeField] private ServerScoreboard ServerS;
    [SerializeField] private CustomLogger Cus;
    #endregion

    #region [StatisticsManager]: Start function

    void Start ()
    {
        StatsFindingWordTimeInSeconds = 0;
        FacebookStatsFriendsCount = 0;
        CurrentEquipedLeaderHealthAdvantage = 0;
        CurrentEquipedLeaderTimeAdvantage = 1;
        CurrentEquipedLeaderGuessedWordAdvantage=0;
        CurrentEquipedLeaderSprite = DefaultLeaderSprite;
        StatsScore = PlayerPrefs.GetInt("TotalScore");

        CurrentWordRestartedTimes = 0;
        FacebookPhotoIndex = 0;
        FacebookStatsScoresCount = 0;

        ReadPlayerPrefs();

        DataWasSaved          = false;
        ResetData             = false;
        FBStatisticsWasLoaded = false;
    }
    #endregion

    #region [StatisticsManager]: Update function
    void Update()
    {
        if ( ResetData == true)
        {
            ResetAllStatistics();
            ResetData = false;
        }
        if ( FBStatisticsWasLoaded == false && FacebookPhotoIndex == FacebookStatsFriendsCount && FacebookStatsScoresCount == FacebookStatsFriendsCount && (FacebookPhotoIndex != 0 && FacebookStatsScoresCount != 0))
        {
            FBStatisticsWasLoaded = true;
            DisplayFacebookStatistics();
        }

    }
    #endregion

    #region [StatisticsManager]: Actions with player local statistics
    // Function which updates statistics after each question.
    public void UpdateStatistics(int value,string target)
    {
        Cus.LogAMessage("[Statistics]: Updating local player data...\n");

        StatsScoreFromWord = value;

        if (target == "update_score")
        {
            StatsScore += value;
            PlayerPrefs.SetInt("TotalScore", StatsScore);
        }
    }
    // Function which saves statistics to playerprefs after the player is dead.
    public void SaveStatistics()
    {
        DataWasSaved = true;
    }
    // Function which displays statistics in Statistics Info Box.

    public void ResetAllStatistics()
    {
        Cus.LogAMessage("[Statistics]: Resetting local player data...\n");

        PlayerPrefs.SetInt("TotalScore", 0);
        PlayerPrefs.SetInt("highest_player_score", 0);

        for (int i = 0; i < ShopS.LeaderKeyStrings.Length; i++)
            PlayerPrefs.SetString(ShopS.LeaderKeyStrings[i], "nenupirktas");
    }
    public void ReadPlayerPrefs()
    {
        Cus.LogAMessage("[Statistics]: Reading local player data...\n");

        if (PlayerPrefs.HasKey("highest_player_score") == false)
            PlayerPrefs.SetInt("highest_player_score", 0);

        for ( int i = 0; i < ShopS.LeadersCount;i++)
        {
            if (PlayerPrefs.HasKey(ShopS.LeaderKeyStrings[i]) == true)
                ShopS.LeaderStates[i] = PlayerPrefs.GetString(ShopS.LeaderKeyStrings[i]);
            else if (PlayerPrefs.HasKey(ShopS.LeaderKeyStrings[i]) == false)
            {
                PlayerPrefs.SetString(ShopS.LeaderKeyStrings[i], "nenupirktas");
                ShopS.LeaderStates[i] = PlayerPrefs.GetString(ShopS.LeaderKeyStrings[i]);
            }

            if (ShopS.LeaderStates[i] == "nupirktas")
            {
                CurrentEquipedLeaderHealthAdvantage = ShopS.LeaderAdvantagesHealth[i];
                CurrentEquipedLeaderTimeAdvantage = ShopS.LeaderAdvantagesTime[i];
                CurrentEquipedLeaderGuessedWordAdvantage = ShopS.LeaderAdvantagesScorePerWord[i];
                CurrentEquipedLeaderSprite = ShopS.LeaderSprites[i];
            }
        }
        GuillotineSacrifice.sprite = CurrentEquipedLeaderSprite;
    }
    public void SetPlayerPrefString(string keyy,string valuee)
    {
        PlayerPrefs.SetString(keyy, valuee);
    }
    #endregion

    #region [StatisticsManager]: Actions with Facebook Leaderboard statistics
    public void FacebookStatisticsCenter()
    {
        // Before displaying the statistical data, delete previous objects.
        GameObject[] gameobjects = GameObject.FindGameObjectsWithTag("StatisticsValueTag");
        foreach (GameObject go in gameobjects)
        {
            Destroy(go);
        }
        FBStatisticsWasLoaded = false;
        FacebookPhotoIndex = 0;
        FacebookStatsFriendsCount = 0;
        FacebookStatsScoresCount = 0;

        CurrentUserFacebookFriendsIds.Clear();
        CurrentUserFacebookFriendsName.Clear();
        CurrentUserFacebookFriendsPhotos.Clear();
        CurrentUserFacebookFriendsScores.Clear();

        FacebookS.LogIntoFBScoreboard();
    }

    public void ReadPlayerFBStatistics(IGraphResult result)
    {
        Cus.LogAMessage("[Statistics]: Reading Facebook data...\n");
        if (result != null)
        {           
            var dic = Json.Deserialize(result.RawResult) as Dictionary<string, object>;

            string usermainname = dic["name"] as string;
            string usermainid = dic["id"] as string;

            CurrentUserFacebookFriendsName.Add(usermainname);
            CurrentUserFacebookFriendsIds.Add(usermainid);

            FacebookS.GetPlayerPhotoURL(usermainid);

            FacebookStatsFriendsCount++;

            var datalistas = dic["friends"] as Dictionary<string, object>;
            var dataList = datalistas["data"] as List<object>;

            for (int i = 0; i < dataList.Count; i++)
            {
                var dataDict = dataList[i] as Dictionary<string, object>;

                string username = dataDict["name"] as string;
                string userid   = dataDict["id"] as string;

                CurrentUserFacebookFriendsName.Add(username);
                CurrentUserFacebookFriendsIds.Add(userid);

                FacebookS.GetPlayerPhotoURL(userid);

                FacebookStatsFriendsCount++;
            }

        }
        for ( int i = 0; i < FacebookStatsFriendsCount;i++)
            StartCoroutine(ServerS.SearchForScoreInServerDB(CurrentUserFacebookFriendsIds[i]));
    }
    public void DisplayFacebookStatistics()
    {
        Cus.LogAMessage("[Statistics]: Displaying the Facebook ScoreBoard...\n");
        // Create gameobject array based on the count of playerprefs data.
        GameObject[] all_stats_value_objects = new GameObject[FacebookStatsFriendsCount];

        for (int i = 0; i < FacebookStatsFriendsCount; i++)
        {
            // Instantiate one statistical object which holds one username and it's score.
            all_stats_value_objects[i] = Instantiate(StatisticsValueObject, StatisticsPageMain.transform) as GameObject;

            RectTransform rt = all_stats_value_objects[i].GetComponent<RectTransform>();

            // Renaming the gameobject's name because it can be deleted easier and also could've be seen better in inspector.
            all_stats_value_objects[i].name = "StatisticsValue" + i;
            if (i != 0)
            {
                RectTransform rtas = all_stats_value_objects[i - 1].GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rtas.anchoredPosition.y - rtas.rect.height);
            }
            // For each tranform ( both for username and score ) find the tranforms and edit their texts with the data from the playerprefs.
            foreach (Transform trs in all_stats_value_objects[i].transform)
            {
                Text text_object = trs.gameObject.GetComponent<Text>();

                if (trs.gameObject.name.ToString() == "ColumnPlayerName")
                    text_object.text = CurrentUserFacebookFriendsName[i];
                else if (trs.gameObject.name.ToString() == "ColumnPlayerScore")
                    text_object.text = CurrentUserFacebookFriendsScores[i];
                else if (trs.gameObject.name.ToString() == "ColumnPlayerImage")
                {
                    if (CurrentUserFacebookFriendsPhotos[i] != null)
                    {
                        Image img = trs.GetComponent<Image>();
                        img.sprite = Sprite.Create(CurrentUserFacebookFriendsPhotos[i], new Rect(0, 0, CurrentUserFacebookFriendsPhotos[i].width, CurrentUserFacebookFriendsPhotos[i].height), new Vector2(0, 0), 1f);
                    }
                }
            }
        }

    }
    #endregion
}