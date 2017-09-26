using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour {

    // Script that manages buying the leader and getting free points from video ads.

    #region [Shop]: Variables
    // Shop game objects.
    [Header("Shop's gameobjects")]
    [SerializeField] private GameObject ShopPage1;
    [SerializeField] private GameObject ShopPage2;
    
    // Reward Based Video ad variables.
    [Header("Get free points gameobjects")]
    [SerializeField] private GameObject RewardVideoResultas;
    [SerializeField] private Text RewardVideoResultText;
    [SerializeField] private Image RewardVideoResultImageContainer;
    [SerializeField] private Sprite[] RewardVideoResultSprites;

    // Variables that are holding data about every Leader.
    [Header("Variables that are holding data about every Leader")]
    [Range(1, 50)] public int LeadersCount;

    public int[] LeaderCosts,
                 LeaderAdvantagesHealth,
                 LeaderAdvantagesTime,
                 LeaderAdvantagesScorePerWord;

    public Sprite[] LeaderSprites;


    public string[] LeaderStates,
                    LeaderNames,
                    LeaderKeyStrings;

    [SerializeField] private Sprite[] LeaderIcons;

    // Interactable buttons.
    [Header("Buttons for User interactions")]
    [SerializeField] private Button NextLeaderBTN;
    [SerializeField] private Button PreviousLeaderBTN;
    [SerializeField] private Button BuyALeaderBTN;

    [SerializeField] private Image LeaderIcon;

    // Game objects that are currently holding the data of the selected Leader.
    [Header("Gameobjects that are currently holding data of the selected Leader")]
    [SerializeField] private Text CurrentPlayerScore;
    [SerializeField] private Text CurrentLeaderName;
    [SerializeField] private Text CurrentLeaderCost;
    [SerializeField] private Text CurrentLeaderState;
    [SerializeField] private Text CurrentLeaderAdvantageHealth;
    [SerializeField] private Text CurrentLeaderAdvantageTime;
    [SerializeField] private Text CurrentLeaderAdvantageScorePerWord;

    public int  CurrentLeaderIndex;

    // References for scripts.
    [Header("Script references")]
    [SerializeField] private StatisticsManager sm;
    [SerializeField] private SoundManager SoundsM;
    [SerializeField] private AnimController anc;
    [SerializeField] private AdsScriptas adse;
    #endregion

    #region [Shop]: Start function
    // Use this for initialization.
    void Start () {
        CurrentLeaderIndex = 0;
    }
    #endregion

    #region [Shop]: Update function
    // Update function is used for managing timers and displaying Reward Based Video outcome data.
    void Update ()
    {
        if ( adse.RewardVideoSuccess != -1 )
        {
            RewardVideoResultImageContainer.preserveAspect = true;
            if ((adse.RewardVideoSuccess == 0 || adse.RewardVideoSuccess == 1) && adse.RewardVideoRewardWasGiven == true)
            {
                int skas = adse.PointsFromRewardVideo();
                sm.UpdateStatistics(skas,"update_score");
                RewardVideoResultText.text = "Sėkmingai gavote " + skas.ToString() + " taškus!";
                RewardVideoResultImageContainer.sprite = RewardVideoResultSprites[0];
                adse.RewardVideoRewardWasGiven = false;
            }
            else if (adse.RewardVideoSuccess == 0)
            {
                RewardVideoResultText.text = "Klaida, taškų negavote!";
                RewardVideoResultImageContainer.sprite = RewardVideoResultSprites[1];
            }

            RewardVideoResultas.SetActive(true);
            adse.RewardVideoTimerEnabled = true;
            adse.RewardVideoSuccess = -1;
        }

        if ( adse.RewardVideoTimerEnabled == true && adse.RewardVideoEndTimer > 0)
            adse.RewardVideoEndTimer -= Time.deltaTime * 1;
        else if ( adse.RewardVideoEndTimer <= 0)
        {
            adse.RewardVideoEndTimer = adse.RewardVideoEndTimerDefault;
            adse.RewardVideoTimerEnabled = false;

            RewardVideoResultas.SetActive(false);
            RewardVideoResultText.text = "";
            RewardVideoResultImageContainer.sprite = null;
            anc.PlayAnimBack(7);
        }

    }
    #endregion

    #region [Shop]: Shop actions: opening/closing windows
    // Open the first page of the shop - Leaders.
    public void OpenShopPage1()
    {
        SoundsM.PlayButtonClickedSound();
        CurrentPlayerScore.text = sm.StatsScore.ToString();
        sm.ReadPlayerPrefs();
        ShopPage1ChangeLeadersValues();
        anc.PlayAnimForward(5);
    }
    // Open the second page of the shop - Free points from Reward Based Video.
    public void OpenShopPage2()
    {
        SoundsM.PlayButtonClickedSound();
        anc.PlayAnimForward(7);
        adse.RequestRewardBasedVideo();
    }
    // Close the first page ( Leaders ) .
    public void CloseShopPage1()
    {
        SoundsM.PlayButtonClickedSound();
        CurrentLeaderIndex = 0;
        anc.PlayAnimBack(5);
    }
    #endregion

    #region [Shop]: Shop actions: buying a leader, moving to next, previous leader
    // Move to the next leader if the index is valid.
    public void ShopPage1NextLeader()
    {
        if ( CurrentLeaderIndex + 1 <= LeadersCount-1)
        {
            SoundsM.PlayButtonClickedSound();
            CurrentLeaderIndex++;
            ShopPage1ChangeLeadersValues();
        }
    }
    // Go back to the previous leader if the index is valid.
    public void ShopPage1PreviousLeader()
    {
        if (CurrentLeaderIndex - 1 >= 0)
        {
            SoundsM.PlayButtonClickedSound();
            CurrentLeaderIndex--;
            ShopPage1ChangeLeadersValues();
        }

    }
    // Buy a Leader if the User has the right amount of money and he didn't bought that leader already.
    public void ShopPage1BuyALeader()
    {
        SoundsM.PlaySuccessSound();
        LeaderStates[CurrentLeaderIndex] = "nupirktas";

        sm.SetPlayerPrefString(LeaderKeyStrings[CurrentLeaderIndex], "nupirktas");
        sm.UpdateStatistics(-LeaderCosts[CurrentLeaderIndex],"update_score");

        CurrentPlayerScore.text = sm.StatsScore.ToString();
        CurrentLeaderState.text = "nupirktas";
        CurrentLeaderState.color = Color.green;

        BuyALeaderBTN.interactable = false;

        sm.ReadPlayerPrefs();
    }
    // Display the data of the currently selected Leader.
    private void ShopPage1ChangeLeadersValues()
    {
        // Set the button's color to green or red, detemrined if the leader was bought or not.
        if (LeaderStates[CurrentLeaderIndex] == "nupirktas")
            CurrentLeaderState.color = Color.green;
        else
            CurrentLeaderState.color = Color.red;

        if (LeaderStates[CurrentLeaderIndex] == "nenupirktas" && sm.StatsScore - LeaderCosts[CurrentLeaderIndex] >= 0)
            BuyALeaderBTN.interactable = true;
        else
            BuyALeaderBTN.interactable = false;

        // Load all the data of the Leader.
        LeaderIcon.sprite = LeaderIcons[CurrentLeaderIndex];
        CurrentLeaderName.text = LeaderNames[CurrentLeaderIndex];
        CurrentLeaderCost.text = LeaderCosts[CurrentLeaderIndex].ToString();
        CurrentLeaderState.text = LeaderStates[CurrentLeaderIndex];
        CurrentLeaderAdvantageHealth.text = "+"+LeaderAdvantagesHealth[CurrentLeaderIndex].ToString();
        CurrentLeaderAdvantageTime.text = LeaderAdvantagesTime[CurrentLeaderIndex].ToString()+"x";
        CurrentLeaderAdvantageScorePerWord.text = "+" +LeaderAdvantagesScorePerWord[CurrentLeaderIndex].ToString();

        // Turn buttons to be interactable or not, determined if the next/previous index is valid.
        if (CurrentLeaderIndex - 1 >= 0)
            PreviousLeaderBTN.interactable = true;
        else
            PreviousLeaderBTN.interactable = false;

        if (CurrentLeaderIndex + 1 <= LeadersCount-1)
            NextLeaderBTN.interactable = true;
        else
            NextLeaderBTN.interactable = false;
    }
    #endregion
}