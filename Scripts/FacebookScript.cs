using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Facebook.Unity;
using System;
using Facebook.MiniJSON;

public class FacebookScript : MonoBehaviour {

    // Script that is responsible for managing Facebook SDK integrations with the app.

    #region [FacebookScript]: Variables 
    // Game objects that are used for making user informed about the share status outcome.
    [Header("Share status objects")]
    [SerializeField] private GameObject ShareStatusWindow;
    [SerializeField] private Sprite[] ShareStatusIcons;
    [SerializeField] private Image ShareStatusIconHolder;
    [SerializeField] private Text ShareStatusTextObject;
    [SerializeField] private float ShareStatusWindowTimer;
    [SerializeField] [Range(1,10)] private float ShareStatusWindowTimerDefault;
    [SerializeField] private string[] ShareStatusText;
    [SerializeField] private Button[] AllMessageWindowButtons;

    // Strings that are used to track current Facebook processes and log in destinations.
    [Header("Facebook processes/destinations in words")]
    [SerializeField] private string CurrentFacebookProcess;
    [SerializeField] private string FacebookLoginDestination;

    // Bool/s for various Facebook processes.
    [Header("Various bools")]
    [SerializeField] private bool ShareStatusTimerIsActive;

    // References for scripts.
    [Header("Script references")]
    [SerializeField] private StatisticsManager StatsM;
    [SerializeField] private ErrorManageris ErrorM;
    [SerializeField] private AnimController AnimC;
    [SerializeField] private ServerScoreboard ServerS;
    [SerializeField] private CustomLogger Cus;
    #endregion

    #region [FacebookScript]: Awake function
    // Awake function for Facebook SDK initialization.
    void Awake()
    {
        ShareStatusTimerIsActive = false;
        ShareStatusWindowTimer = ShareStatusWindowTimerDefault;

        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK.
            Cus.LogAMessage("[Facebook SDK]: Initializing FACEBOOK SDK...\n");
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event.
            FB.ActivateApp();
        }
    }
    #endregion

    #region [FacebookScript]: Update function
    // Update function that is used only for resetting/updating share status timer.
    void Update()
    {
        if (ShareStatusTimerIsActive == true && ShareStatusWindowTimer >= 0)
        {
            ShareStatusWindowTimer -= Time.deltaTime * 1;
        }
        else if (ShareStatusWindowTimer <= 0 && ShareStatusTimerIsActive == true)
        {
            ShareStatusTimerIsActive = false;
            ShareStatusWindowTimer = ShareStatusWindowTimerDefault;

            ShareStatusTextObject.text = "";
        }
    }
    #endregion

    #region [FacebookScript]: Logging into FB with different tasks to do

    // Log into Facebook with different destinations.

    // Log into Facebook with the purpose of getting Facebook scoreboard.
    public void LogIntoFBScoreboard()
    {
        FacebookLoginDestination = "ScoreBoard";
        LogIntoFB_Read();
    }
    // Log into Facebook with the purpose of Sharing the score with your friends via Open graph stories.
    public void LogIntoFBShareAScore()
    {
        FacebookLoginDestination = "ShareAScore";
        LogIntoFB_Read();
    }
    // Log into Facebook with the purpose of Writing the score to the custom server.
    public void LogIntoFBWriteAScore()
    {
        FacebookLoginDestination = "WriteAScore";

        int didziausias_score = 0;


        if (PlayerPrefs.HasKey("highest_player_score") == true)
            didziausias_score = PlayerPrefs.GetInt("highest_player_score");

        // If the highest player score is smaller than the previous word's score, try to upload it to custom server.
        if (StatsM.StatsScoreFromWord > didziausias_score)
            LogIntoFB_Read();
    }
    
    // Actual function for logging into Facebook.
    private void LogIntoFB_Read()
    {
        Cus.LogAMessage("[Facebook SDK]: Starting a login proccess with read permissions...\n");     
        FB.LogInWithReadPermissions(new List<string>() { "public_profile", "user_games_activity", "user_friends" }, AuthCallback_Login);
    }
    #endregion

    #region [FacebookScript]: Different actions/tasks what we will be doing

    // Different possible Facebook actions/tasks to do with this application.

    // Sharing a google play store link.
    public void ShareFB()
    {
        // Not implemented at all.

        Cus.LogAMessage("[Facebook SDK]: Trying to Share a Link...\n");

    }
    // Share an Open Graph story with your Facebook friends.
    public void ShareAScore()
    {
        CurrentFacebookProcess = "ShareAScore";

        ChangeAllMessageButtonsState(false);

    }
    // Open Facebook fan page directly with Facebook app.
    public void OpenFBUrl()
    {
        Cus.LogAMessage("[Facebook SDK]: Opening FB page URL...\n");

    }
    // Get Facebook user photo to fill the scoreboard.
    public void GetPlayerPhotoURL(string playerFacebookId)
    {
        Cus.LogAMessage("[Facebook SDK]: Trying to get Player Photo by URL...\n");
        CurrentFacebookProcess = "UserPhoto";


        WWWForm wwwForm = new WWWForm();
        FB.API(url, HttpMethod.GET, FacebookProcessCallBack, wwwForm);
    }
    // Get the actual user friends and use that information for filling the Facebook scoreboard.
    public void GetFacebookFriendsScoreBoard()
    {
        Cus.LogAMessage("[Facebook SDK]: Trying to get FB friends Score...\n");

        CurrentFacebookProcess = "GetFBScoreBoard";
    

        FB.API(url, HttpMethod.GET, FacebookProcessCallBack, wwwForm);
    }
    // Buttons to disable during the sharing process.
    public void ChangeAllMessageButtonsState(bool state)
    {
        for (int i = 0; i < AllMessageWindowButtons.Length; i++)
            AllMessageWindowButtons[i].interactable = state;
    }

    #endregion

    #region [FacebookScript]: Callbacks from different actions/tasks
    // Callback from Facebook SDK initialization.
    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // If the Facebook SDK was successfully initialized, try to activate the app.
            Cus.LogAMessage("[Facebook SDK]: Facebook SDK was successfully initialized, now activating the APP...\n");

            if (FB.IsLoggedIn == true)
            {
                Cus.LogAMessage("[Facebook SDK]: Already logged in , loggin out...\n");
                FB.LogOut();
            }
            else if (FB.IsLoggedIn == false)
                Cus.LogAMessage("[Facebook SDK]: User is not logged in...\n");

            FB.ActivateApp();
        }
        else
        {
            // Failed to initialize the Facebook SDK.
            Cus.LogAMessage("[Facebook SDK]: Failed to initialize the Facebook SDK...\n");
            ErrorM.ShowErrorArea("Nepavyko paleisti 'Facebook' SDK, todėl nebus galima naudotis " + Environment.NewLine + " 'Facebook' socialinio tinklo galimybėmis" +Environment.NewLine +"su šiuo žaidimu...",2);
        }
    }
    // Function that hides Facebook dialog UI.
    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide.
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again.
            Time.timeScale = 1;
        }
    }
    // Callback from the Facebook SDK log in process.
    private void AuthCallback_Login(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            // After the user is logged in, determine which process to activate.

            Cus.LogAMessage("[Facebook SDK]: Successfully logged in with READ permissions...\n");

            if (FacebookLoginDestination == "ScoreBoard")
                GetFacebookFriendsScoreBoard();
            else if (FacebookLoginDestination == "ShareAScore")
                ShareAScore();
            else if (FacebookLoginDestination == "WriteAScore")
                StartCoroutine(ServerS.WriteScoreToServer(StatsM.StatsScoreFromWord, result.AccessToken.UserId.ToString()));

        }
        else
        {
            // Failed to log in to Facebook SDK.
            ErrorM.ShowErrorArea("Nepavyko prisijungti prie " + Environment.NewLine + "Facebook serverių!",-1);
            Cus.LogAMessage("[Facebook SDK]: Failed to login with READ permissions...\n");
        }
    }
    // Callback from sharing google play store link.
    private void ShareCallBackProcess(IShareResult result)
    {
        if (!String.IsNullOrEmpty(result.PostId))
        {
            // Print post identifier of the shared content.
            Cus.LogAMessage("[Facebook SDK]: Successfully Shared a Link, post id: \n" + result.PostId);
        }
        else
        {
            // Share succeeded without postID.
            Cus.LogAMessage("[Facebook SDK]: Failed to Share a Link, because: \n" + result.Error);
        }
    }
    // More universal callback function, which works on mostly of the processes.
    // Does not work with the link sharing and score sharing.
    private void FacebookProcessCallBack(IGraphResult resultas)
    {
        if (!string.IsNullOrEmpty(resultas.RawResult)) // Success
        {
            if (CurrentFacebookProcess == "UserPhoto")
            {
                Cus.LogAMessage("[Facebook SDK]: Successfully got the UserPhoto!\n");
                StatsM.CurrentUserFacebookFriendsPhotos.Add(resultas.Texture);
                StatsM.FacebookPhotoIndex++;
            }
            else if (CurrentFacebookProcess == "GetFBScoreBoard")
            {
                Cus.LogAMessage("[Facebook SDK]: Successfully read the ScoreBoard!");
                StatsM.ReadPlayerFBStatistics(resultas);
            }
        }
        else
        {
            if (CurrentFacebookProcess == "UserPhoto")
            {
                Cus.LogAMessage("[Facebook SDK]: Failed to get UserPhoto, because: \n" + resultas.Error);
                StatsM.FacebookPhotoIndex++;
            }
            else if (CurrentFacebookProcess == "GetFBScoreBoard")
            {
                Cus.LogAMessage("[Facebook SDK]: Failed to get Score data from FB, because: \n" + resultas.Error);
                ErrorM.ShowErrorArea("Nepavyko nuskaityti duomenų iš " + Environment.NewLine + "Facebook serverių!",-1);
            }
        }
    }
    // Callback from sharing the score with the Facebook friends.
    public void ShareFBcallback(IShareResult resultas)
    {
        if (CurrentFacebookProcess == "ShareAScore")
            ShareStatusTimerIsActive = true;

        if (resultas.Cancelled == true || !String.IsNullOrEmpty(resultas.Error))
        {
            if (CurrentFacebookProcess == "ShareAScore")
            {
                if (!String.IsNullOrEmpty(resultas.Error))
                    Cus.LogAMessage("[Facebook SDK]: Error while trying to share the score, reason:\n" + resultas.Error.ToString() + "\n");
                else if (resultas.Cancelled == true)
                    Cus.LogAMessage("[Facebook SDK]: Error while trying to share the score, reason: " + " the sharing process was cancelled by the player\n");

                ShowShareScoreInfo(1);
            }
        }
        else if (!String.IsNullOrEmpty(resultas.PostId))
        {
            if (CurrentFacebookProcess == "ShareAScore")
            {
                Cus.LogAMessage("[Facebook SDK]: Successfully shared the score on FB!\n");
                ShowShareScoreInfo(0);
            }
        }
        else
        {
            if (CurrentFacebookProcess == "ShareAScore")
            {
                Cus.LogAMessage("[Facebook SDK]: Successfully shared the score on FB!\n");
                ShowShareScoreInfo(0);
            }
        }
    }

    // Display message with the process(callback) outcome.
    private void ShowShareScoreInfo(int mode)
    {
        if (mode == 0)
        {
            ShareStatusTextObject.text = ShareStatusText[0];
            ShareStatusIconHolder.sprite = ShareStatusIcons[0];
            ShareStatusIconHolder.preserveAspect = true;
        }
        else if (mode == 1)
        {
            ShareStatusTextObject.text = ShareStatusText[1];
            ShareStatusIconHolder.sprite = ShareStatusIcons[1];
            ShareStatusIconHolder.preserveAspect = true;
        }

        AnimC.PlayMessageAnim(7, 4, 7);
    }
    #endregion
}