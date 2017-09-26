using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text;

public class LocalGameManageris : MonoBehaviour {

    // Main Script that is responsible for managing the game, it's states and all other related stuff.

    #region [LocalGameManager]: Variables
    // Game objects for turning on/off the MainMenu, GameArea, etc.
    [Header("Game Manager's gameobjects")]
    [SerializeField] private GameObject MainMenu;
    public GameObject GameArea;
    public GameObject WordArea;
    [SerializeField] private GameObject UnderScore;
    public GameObject MessageBoxMain;
    public GameObject MessageBoxWon;
    public GameObject MessageBoxLost;
    public GameObject MessageBoxPause;
    [SerializeField] private GameObject MessageBoxWonNormalButtons;
    [SerializeField] private GameObject MessageBoxWonTwoPlayersButtons;

    // Bools which are responsible for various game states.
    [Header("Various bools")]
    [SerializeField] private bool ValuesAreInitialized;
    [SerializeField] private bool WordWasDecyphered;
    [SerializeField] private bool IsPlayerDead;
    public bool GameIsPaused;
    [SerializeField] private bool WordIsShown;
    [SerializeField] private bool MessageWindowIsDisplayed;
    [SerializeField] private bool TimerForSecondsCounterIsEnded;

    // Ints that are holding important information, like the index for Words.
    [Header("Main variables")]
    [SerializeField] private int WordIndex;
    [SerializeField] private int LivesLeft;
    [SerializeField] private int FoundLettersCount;

    [SerializeField] [Range(1, 50)] private int[] DifficultyLivesModifier;

    public string TwoPlayersModeCustomWord;

    [SerializeField] [Range(1, 50)] private int TwoPlayersModeLives;

    // Timers.
    [Header("Timer variables")]
    [SerializeField] private float TimerSecondsCounter;
    [SerializeField][Range(1,100)] private float TimerSecondsCounterDefault;

    [SerializeField] private Text TimeSpentCounter;

    // Text components for changing Lives/Seconds and etc.
    [Header("Message variables")]
    [SerializeField] private Text MessageBoxWonGuessedWord;
    [SerializeField] private Text MessageBoxSuccessRateInWords;
    [SerializeField] private Text MessageBoxTextScoreInWords;

    [SerializeField] private GameObject[] AllStars;

    [Header("Guillotine variables")]
    // Variables for managing guillotine.
    [SerializeField] private RectTransform Guillotine;
    [SerializeField] private RectTransform GuillotineTop;
    [SerializeField] private RectTransform GuillotineSacrificeRect;

    private Vector2 GuillotineDefaultPositionY,
                    GuillotineTopY,
                    GuillotineSacrificeDefaultPositon;

    private float GuillotineLowerValue;

    [Header("Other objects")]
    [SerializeField] private Font CustomFont;
    // Array which is responsible for holding all the current word's letters.
    [SerializeField] private GameObject[] CurrentWordAllLetters;

    // All the letters that the User can press.
    [SerializeField] private Image[] AllLetters;

    // Event System for capturing User inputs.
    [SerializeField] private EventSystem evs;

    // References for scripts.
    [Header("Script references")]
    [SerializeField] private PrepareData PrepData;
    [SerializeField] private SoundManager SoundM;
    [SerializeField] private StatisticsManager StatsM;
    [SerializeField] private MainMenu MainM;
    [SerializeField] private AnimController AnimC;
    [SerializeField] private AdsScriptas AdsMan;
    [SerializeField] private ErrorManageris ErrorM;
    [SerializeField] private CustomLogger Cus;
    #endregion

    #region [LocalGameManager]: Start function
    // Initialize bool that responsible for all the other values to be initialized before the word is displayed.
    void Start ()
    {
        ValuesAreInitialized = false;

        Cus.LogAMessage("[Game]: Starting LocalGameManager script...\n");
    }
    #endregion

    #region [LocalGameManager]: Timer and game state updates
    // Update function for timers and to check for game states.
    void Update ()
    {
        if (ValuesAreInitialized == true)
        {
            // Spent seconds counter which is used to show how long does the actual decyphering for the current word is taking place.
            if (TimerSecondsCounter > 0 && TimerForSecondsCounterIsEnded == false && (IsPlayerDead == false && WordWasDecyphered == false && GameIsPaused == false) && AdsMan.InterstitialAdIsDisplaying != 1)
                TimerSecondsCounter -= Time.deltaTime;
            else if (TimerSecondsCounter < 0 && TimerForSecondsCounterIsEnded == false)
            {
                // If the seconds counter timer is less than 0 and the counter is active, reset it back to the default value.

                TimerSecondsCounter = TimerSecondsCounterDefault * StatsM.CurrentEquipedLeaderTimeAdvantage;
                StatsM.StatsFindingWordTimeInSeconds++;

                if (StatsM.StatsFindingWordTimeInSeconds <= 999)
                    TimeSpentCounter.text = StatsM.StatsFindingWordTimeInSeconds.ToString();
                else if (StatsM.StatsFindingWordTimeInSeconds > 999)
                    TimeSpentCounter.text = "999+";

            }

            // Stop the timer if one of the below conditions are met.
            if (GameArea.activeSelf == false || IsPlayerDead == true || WordWasDecyphered == true || GameIsPaused == true || AdsMan.InterstitialAdIsDisplaying == 1 || AdsMan.InterstitialAdIsLoaded == 1 || AdsMan.InterstitialAdTryingToLoad == 1)
                TimerForSecondsCounterIsEnded = true;
            else
                TimerForSecondsCounterIsEnded = false;

            // Display the message if the player is Dead and the message wasn't displayed yet.
            if (IsPlayerDead == false && LivesLeft == 0)
            {
                IsPlayerDead = true;
                Cus.LogAMessage("[Game]: Player is Dead...\n");
                AnimC.PlayGameAreaAnim(0, 0, 0);

            }
            else if (IsPlayerDead == true && AnimC.GameAreaHeadCutIsEnded == true && MessageWindowIsDisplayed == false)
            {
                // If all the animations are done playing, open message window.
                DisplayMessage(1);
                AnimC.PlayMessageAnim(3, 2, 1);
            }
        }
    }
    #endregion

    #region [LocalGameManager]: Start the game by initializing values
    // Actual function that initializes values.
    private void StartValues()
    {
        // Determine the starting lives by the game mode.
        if (MainM.CurrentGameMode == "SinglePlayer")
        {
            // Determine the starting lives by the difficulty level.
            if (MainM.CurrentDifficultyLevel == "Easy")
            {
                LivesLeft += DifficultyLivesModifier[0];
                WordIndex = PrepData.EasyIndex;
            }
            else if (MainM.CurrentDifficultyLevel == "Medium")
            {
                LivesLeft += DifficultyLivesModifier[1];
                WordIndex = PrepData.MediumIndex;
            }
            else if (MainM.CurrentDifficultyLevel == "Hard")
            {
                LivesLeft += DifficultyLivesModifier[2];
                WordIndex = PrepData.HardIndex;
            }
            else if (MainM.CurrentDifficultyLevel == "VeryHard")
            {
                LivesLeft += DifficultyLivesModifier[3];
                WordIndex = PrepData.VeryHardIndex;
            }
        }
        else if (MainM.CurrentGameMode == "TwoPlayers")
            LivesLeft = TwoPlayersModeLives;

        GuillotineTopY = GuillotineTop.anchoredPosition;
        GuillotineDefaultPositionY = Guillotine.anchoredPosition;

        LivesLeft += StatsM.CurrentEquipedLeaderHealthAdvantage;

        GuillotineLowerValue = Vector2.Distance(GuillotineTopY, GuillotineDefaultPositionY) / LivesLeft;

        GuillotineSacrificeDefaultPositon = GuillotineSacrificeRect.anchoredPosition;

        IsPlayerDead = false;
        TimerForSecondsCounterIsEnded = true;
        WordWasDecyphered = false;
        GameIsPaused = false;
        MessageWindowIsDisplayed = false;

        ValuesAreInitialized = true;
        Cus.LogAMessage("[Game]: Game values were successfully initialized!\n");
    }
    #endregion

    #region [LocalGameManager]: Displaying a word/Checking User selected letter
    // Function that shows the actual word.
    public void DisplayWord()
    {
        StartValues();

        if (MainM.CurrentGameMode == "SinglePlayer" || MainM.CurrentGameMode == "TwoPlayers")
        {
            string word = null;
            int wordtotalcount = 0;

            if (MainM.CurrentGameMode == "SinglePlayer")
                wordtotalcount = PrepData.WordsChosenList.Length;
            else if (MainM.CurrentGameMode == "TwoPlayers")
                wordtotalcount = 1;

            // If the index is less than the count of the chosen word list, proceed further.
            if (WordIndex < wordtotalcount)
            {
                if (MainM.CurrentGameMode == "SinglePlayer")
                    word = PrepData.WordsChosenList[WordIndex];
                else if (MainM.CurrentGameMode == "TwoPlayers")
                    word = TwoPlayersModeCustomWord;

                Cus.LogAMessage("[Game]: Displaying a word: " + word + "\n");

                // Create the array for the current word.
                CurrentWordAllLetters = new GameObject[word.Length];

                // For loop in which we instantiate game object for every letter in the current word.
                for (int i = 0; i < word.Length; i++)
                {
                    // If the letter is not empty, proceed further.
                    if (word.Substring(i, 1) != string.Empty)
                    {
                        // Instantiate the letter gameobject.
                        CurrentWordAllLetters[i] = Instantiate(UnderScore, WordArea.transform) as GameObject;

                        // Set ReactTransform for the current letter.
                        RectTransform rt = CurrentWordAllLetters[i].GetComponent<RectTransform>();

                        /// Change the game object's name to something more suitable.
                        CurrentWordAllLetters[i].name = "word_letter" + i;

                        // Set the text component of the gameobject.
                        Text TextOfCurrentWord = CurrentWordAllLetters[i].GetComponent<Text>();


                        // If the current letter is space, make that gameobjects letter text to "" and add one to foundletterscount;
                        if (word.Substring(i, 1) == " ")
                        {
                            TextOfCurrentWord.text = "";
                            FoundLettersCount++;
                        }
                        // If the letter is not first one to be added, find the previous letter and add it's width and height to the current letter's position.
                        if (i != 0)
                        {
                            RectTransform rtas = CurrentWordAllLetters[i - 1].GetComponent<RectTransform>();
                            RectTransform WordAreaRt = WordArea.GetComponent<RectTransform>();

                            if (rtas.rect.width * 2 + rtas.anchoredPosition.x <= WordAreaRt.rect.width)
                                rt.anchoredPosition = new Vector2(rtas.rect.width + rtas.anchoredPosition.x, rtas.anchoredPosition.y);
                            else
                                rt.anchoredPosition = new Vector2(0, rt.anchoredPosition.y + rtas.anchoredPosition.y);
                        }
                    }
                }
                WordIsShown = true;
            }
            else
            {
                // If the Index is higher then the current word list count, reset the index and restart the game.
                WordIndex = 0;
                RestartTheGame(0);
            }
        }
    }


    // Function that checks the User chosen letter, whether the current word has that letter or not.
    public void CheckPlayerSelectedLetter()
    {
        // If the Word is Active, and if the player is not dead and the world hasn't been decpyhered yet, proceed further.
        if (WordIsShown == true && IsPlayerDead == false && WordWasDecyphered == false)
        {
            // For all User available letters, check which one was pressed and act accordingly.
            for (int i = 0; i < AllLetters.Length; i++)
            {
                // If The Letter seleced is the one, proceed.
                if (evs.currentSelectedGameObject.gameObject.ToString() == AllLetters[i].gameObject.ToString())
                {
                    int index = 0;
                    bool LetterWasFound = false;
                    // Find letter[s] in the word and show the found letter[s] on the screen by the letter's location in the actual word.
                    string word = null;

                    if (MainM.CurrentGameMode == "SinglePlayer")
                        word = PrepData.WordsChosenList[WordIndex];
                    else if (MainM.CurrentGameMode == "TwoPlayers")
                        word = TwoPlayersModeCustomWord;

                    foreach (char c in word)
                    {
                        Text PlayerChosenLetterText = CurrentWordAllLetters[index].GetComponent<Text>();
                        string CurrentWordLetter = c.ToString();

                        // If the letter was found, show it, paint the letter and play the sound.
                        if (CurrentWordLetter.ToUpper() == AllLetters[i].gameObject.name.ToString())
                        {
                            PlayerChosenLetterText.text = CurrentWordLetter.ToUpper();
                            LetterWasFound = true;
                            AllLetters[i].color = Color.green;
                            FoundLettersCount++;
                            SoundM.PlaySuccessSound();
                        }
                        index++;
                    }
                    // If the letter was not found, paint it red, play the sound, etc.
                    if (LetterWasFound == false)
                    {
                        AllLetters[i].color = Color.red;
                        LivesLeft--;
                        Guillotine.anchoredPosition= new Vector2(Guillotine.anchoredPosition.x, Guillotine.anchoredPosition.y + GuillotineLowerValue);
                        SoundM.PlayFailedSound();
                    }
                    // If the count of found letters is equal to the length of the word, then the player has decyphered the word.
                    if (FoundLettersCount == CurrentWordAllLetters.Length)
                    {
                        MessageBoxWonGuessedWord.text = word.ToUpper();
                        ScoreInfoGuessedWord();

                        if (MainM.CurrentGameMode == "SinglePlayer")
                        {
                            MessageBoxWonNormalButtons.SetActive(true);

                            // Increment the list index of the decyphered word.
                            if (MainM.CurrentDifficultyLevel == "Easy")
                                PrepData.EasyIndex++;
                            else if (MainM.CurrentDifficultyLevel == "Medium")
                                PrepData.MediumIndex++;
                            else if (MainM.CurrentDifficultyLevel == "Hard")
                                PrepData.HardIndex++;
                            else if (MainM.CurrentDifficultyLevel == "VeryHard")
                                PrepData.VeryHardIndex++;

                            Cus.LogAMessage("EASY: " + PrepData.EasyIndex + " medium: " + PrepData.MediumIndex + " hard: " + PrepData.HardIndex + " very hard: " + PrepData.VeryHardIndex);
                        }
                        else if (MainM.CurrentGameMode == "TwoPlayers")
                            MessageBoxWonTwoPlayersButtons.SetActive(true);

                        Cus.LogAMessage("[Game]: Player SUCCESSFULLY decyphered the word!\n");


                        DisplayMessage(0);
                        AnimC.PlayMessageAnim(3, 2, 0);
                        WordWasDecyphered = true;
                        break;
                    }
                    // Disable the button for the letter, despite the result.
                    GameObject gm = AllLetters[i].gameObject;
                    Button bts = gm.GetComponent<Button>();
                    bts.enabled = false;

                }
            }
            // Reset the object that has been selected by the event system.
            evs.SetSelectedGameObject(null);
        }
    }
    #endregion

    #region  [LocalGameManager]: Counting the score for the completed word
    private void ScoreInfoGuessedWord()
    {
        // Calculate score for the guessed word.

        Cus.LogAMessage("[Game]: Counting Player Score for current word...\n");

        // Score = lives left after guessing the word * score given per word + the leader advantage from the shop ( default is 0 ).
        int score = ( LivesLeft * StatsM.LocalStatsScoreGivenPerWord ) + StatsM.CurrentEquipedLeaderGuessedWordAdvantage;

        string level = null;

        // Subtract score by the spent seconds * penalty for each second spent.
        score -= StatsM.StatsFindingWordTimeInSeconds * StatsM.LocalStatsPenaltyForSecondSpent;
        // Also subtract the score by how many times you restarted on word by the score given per word.
        score -= StatsM.CurrentWordRestartedTimes * StatsM.LocalStatsScoreGivenPerWord;

        // Multiply score if the user has atleast X lives remaining.
        if (LivesLeft <= 5)
            score = score * 2;

        // Find the success rate by the score.
        if (score <= StatsM.LocalStatsSuccessRateMinimum )
        {
           level = StatsM.LocalStatsSuccessMinimumWord;
           AllStars[0].SetActive(true);
        }
        else if (score > StatsM.LocalStatsSuccessRateMinimum && score < StatsM.LocalStatsSuccessRateHigh)
        {
            level = StatsM.LocalStatsSuccessMediumWord;
            AllStars[1].SetActive(true);
        }
        else if (score >= StatsM.LocalStatsSuccessRateHigh)
        {
            level = StatsM.LocalStatsSuccessHighWord;
            AllStars[2].SetActive(true);
        }
        // Normalize score if the score is below 0.
        if (score <= 0)
            score = 0;

        // Display the score in the message window.
        if (MainM.CurrentGameMode == "SinglePlayer")
        {
            MessageBoxTextScoreInWords.text = "TAŠKAI:" + score.ToString();
            MessageBoxSuccessRateInWords.text = level;
            StatsM.UpdateStatistics(score, "update_score");
        }
        else if ( MainM.CurrentGameMode == "TwoPlayers")
        {
            MessageBoxTextScoreInWords.text = "TAŠKAI "+ score.ToString();
            MessageBoxSuccessRateInWords.text = "LAIMĖJAI!";
            StatsM.UpdateStatistics(score,"show_for_sharing");
        }
    }
    #endregion

    #region [LocalGameManager]: Opening the message window
    // Function that displays the message to the player.Parameter 'mode' is to determine the message state: won/lost/pause, etc.
    public void DisplayMessage(int mode)
    {
        if (mode == 1)
        {
            // Display the message when the user lost the game.
            Cus.LogAMessage("[Game]: Displaying a message: LOST...\n");
            MessageBoxLost.SetActive(true);
            MessageWindowIsDisplayed = true;
        }
        else if (mode == 0)
        {
            // Display the message when the user won the game.
            Cus.LogAMessage("[Game]: Displaying a message: WON...\n");
            MessageBoxWon.SetActive(true);
            MessageWindowIsDisplayed = true;
        }
        else if (mode == 2)
        {
            if (GameIsPaused == false)
            {
                // Pause the game if the user has pressed the pause button.
                Cus.LogAMessage("[Game]: Displaying a message: PAUSING..\n");
                MessageWindowIsDisplayed = true;
                MessageBoxPause.SetActive(true);
                GameIsPaused = true;
                AnimC.PlayMessageAnim(3, 2, 2);
            }
            else if (GameIsPaused == true)
            {
                // Unpause the game if the user has pressed the unpause button.
                Cus.LogAMessage("[Game]: Displaying a message: UNPAUSING THE GAME.\n");
                MessageWindowIsDisplayed = false;
                GameIsPaused = false;
                AnimC.PlayMessageAnim(4, 2, 3);
            }
        }

    }
    #endregion

    #region [LocalGameManager]: Actions by clicking the buttons
    // Restarts the current word.
    public void RestartThePreviousWord()
    {
        SoundM.PlayButtonClickedSound();
        Cus.LogAMessage("[Game]: Restarting current word...\n");
        RestartTheGame(2);
    }
    // Moves to the next word.
    public void NextWordFromButton()
    {
        SoundM.PlayButtonClickedSound();
        Cus.LogAMessage("[Game]: Moving to the next word...\n");
        RestartTheGame(1);
    }
    // Restarts the game and moves to the Main Menu.
    public void ReturnToMainMenuAndRestartGame()
    {
        SoundM.PlayButtonClickedSound();
        Cus.LogAMessage("[Game]: Restarting the game and returning to the Main Menu...\n");
        RestartTheGame(0);
    }
    // Pause/Unpause the game.
    public void PauseTheGame()
    {
        SoundM.PlayButtonClickedSound();
        Cus.LogAMessage("[Game]: Pausing/Unpausing the game...\n");
        DisplayMessage(2);
    }
    #endregion

    #region  [LocalGameManager]: Restarting the game
    // Function that restarts the game.Parameter 'status' is for determining what settings should be nulled.
    private void RestartTheGame(int status)
    {
        Cus.LogAMessage("[Game]: Restarting the game, mode: " + status.ToString() + "\n");
        // Reset all the game objects.
        MessageBoxWon.SetActive(false);
        MessageBoxLost.SetActive(false);
        MessageBoxPause.SetActive(false);
        MessageBoxWonNormalButtons.SetActive(false);
        MessageBoxWonTwoPlayersButtons.SetActive(false);
        MessageBoxTextScoreInWords.text = "";
        MessageBoxSuccessRateInWords.text = "";

        for (int i = 0; i < AllStars.Length; i++)
            AllStars[i].SetActive(false);


        // Reset all the bools.
        GameIsPaused = false;      
        IsPlayerDead = false;
        WordWasDecyphered = false;
        WordIsShown = false;
        TimerForSecondsCounterIsEnded = false;
        MessageWindowIsDisplayed = false;
        ValuesAreInitialized = false;

        // Reset all the ints.
        LivesLeft = 0;
        FoundLettersCount = 0;
        StatsM.StatsFindingWordTimeInSeconds= 0;
        TimerSecondsCounter = TimerSecondsCounterDefault*StatsM.CurrentEquipedLeaderTimeAdvantage;

        // Reset guillotine values.
        Guillotine.anchoredPosition = new Vector2(Guillotine.anchoredPosition.x,GuillotineDefaultPositionY.y);

        StatsM.GuillotineSacrifice.sprite = StatsM.CurrentEquipedLeaderSprite;

        GuillotineSacrificeRect.anchoredPosition = GuillotineSacrificeDefaultPositon;
        GuillotineSacrificeRect.rotation = Quaternion.Euler(0, 0, 0);

        // Reset letter objects.
        DestroyAllLetterObjects();
        ResetAllPlayerLettersColor();

        // Reset animation data.
        AnimC.ResetAnimationData();

        TimeSpentCounter.text = "0";

        // Reset even more data, that data is determined by the outcome/action of the game.
        if (status == 0)
        {
            // Move to Main Menu.
            StatsM.CurrentWordRestartedTimes = 0;
            AnimC.PlayAnimBack(6);
            
            StatsM.DataWasSaved = false;

            this.GetComponent<LocalGameManageris>().enabled = false;
        }
        else if (status == 1)
        {
            // Move to the next word.
            AdsMan.InterstitialAdTryingToLoad = 1;
            AdsMan.InterstitialOriginatedFrom = "Forward";
            StatsM.CurrentWordRestartedTimes = 0;
            AdsMan.RequestInterstitial();             
        }
        else if (status == 2)
        {            
            // Restart the current word.
            StatsM.CurrentWordRestartedTimes++;

            AdsMan.InterstitialAdTryingToLoad = 1;
            AdsMan.InterstitialOriginatedFrom = "Restart";
            AdsMan.RequestInterstitial();
        }
    }
    // Function that resets color of the letters back to their default color.
    private void ResetAllPlayerLettersColor()
    {
        for (int i = 0; i < AllLetters.Length; i++)
        {
            AllLetters[i].color = new Color(1f, 1f, 1f, 0.27f);
            GameObject gm = AllLetters[i].gameObject;
            Button bts = gm.GetComponent<Button>();
            bts.enabled = true;
        }
    }
    // Function that destroys all the word's letter objects.
    private void DestroyAllLetterObjects()
    {
        GameObject[] gameobjects = GameObject.FindGameObjectsWithTag("LetterSpace");
        foreach (GameObject go in gameobjects)
            Destroy(go);
    }
    #endregion
}