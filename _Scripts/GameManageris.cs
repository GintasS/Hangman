using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text;

public class GameManageris : MonoBehaviour {

    // Main Script responsible for managing the game, it's states and all other related stuff.

    // GameObjects for displaying on/off the MainMenu, GameArea etc
    public GameObject MainMenu,
                      GameArea,
                      WordArea,
                      UnderScore,
                      SubmitScoreBox,
                      MessageBox;
    // Bools which are responsible for various game states.
    public bool WordWasDecyphered,
                IsPlayerDead,
                WordIsShown,
                TimerIsEnded,
                TimerForSecondsCounterIsEnded;

    // Int's that hold important information, like the index for Words.
    public int WordIndex,
               LivesLeft,
               HangmanIndex,
               FoundLettersCount;

    // Event System for capturing User Inputs.
    public EventSystem evs;

    // Timers.
    public float Timer,
                 TimerDefault,
                 TimerSecondsCounter,
                 TimerSecondsCounterDefault;

    // Array which is responsible for holding all the current word's letter gameobjects.
    public GameObject[] CurrentWordAllLetters;

    // All the letters the User can press.
    public Text[] AllLetters;

    // Text components for changing Lives/Seconds and etc.
    public Text LivesLeftCounter,
                TimeSpentCounter,
                MessageBoxText;

    // All the hangman sprites.
    public Sprite[] AllHangmanSprites;

    // Actual Image component to whom the sprites are.
    public Image Hangman;
    
    // Reference to scripts.
    private PrepareData PrepData;

    private SoundManager SoundM;

    private StatisticsManager StatsM;

    void Start ()
    {
        PrepData = GameObject.Find("Data").GetComponent<PrepareData>();
        SoundM  = GameObject.Find("SoundManagerScript").GetComponent<SoundManager>();
        StatsM = GameObject.Find("StatisticsManagerScript").GetComponent<StatisticsManager>();
        PrepData.ReadDataFromFile();
        IsPlayerDead = false;
        TimerIsEnded = false;
        TimerForSecondsCounterIsEnded = true;
        WordWasDecyphered = false;
    }

	void Update ()
    {
        // Timer which is used to move to the next word after the successfull decypher.
        if ( WordWasDecyphered == true)
        {
            if (Timer > 0 && TimerIsEnded == false)
                Timer -= Time.deltaTime;
            else if (Timer < 0 && TimerIsEnded == false)
            {
                // If the Timer is below 0 and it's not stopped, stop it and reset it.
                Timer = TimerDefault;
                TimerIsEnded = true;
            }
        }
        
        // Spent seconds counter which is used to show for how long does the actual decyphering for the current word is taking place.
        if (TimerSecondsCounter > 0 && TimerForSecondsCounterIsEnded == false && ( IsPlayerDead == false && WordWasDecyphered == false))
            TimerSecondsCounter -= Time.deltaTime;
        else if ( TimerSecondsCounter < 0 && TimerForSecondsCounterIsEnded == false)
        {
            // If the Seconds counter timer is less than 0 and the counter is active, reset it back to the default value.

            TimerSecondsCounter = TimerSecondsCounterDefault;
            StatsM.Stats_FindingWordTimeInSeconds++;
            TimeSpentCounter.text = StatsM.Stats_FindingWordTimeInSeconds + " s";
        }

        // Stop the timer if one of the below conditions are met.
        if (GameArea.activeSelf == false || IsPlayerDead == true || WordWasDecyphered == true)
            TimerForSecondsCounterIsEnded = true;
        else
            TimerForSecondsCounterIsEnded = false;

        // Display the Message if the player is Dead and the message wasn't displayed yet.
        if (IsPlayerDead == false && LivesLeft == 0)
        {
            IsPlayerDead = true;
            DisplayMessage("Žaidimą pralaimėjote!" + "\n" + "\n" + "Žodis buvo: " + PrepData.WordsChosenList[WordIndex - 1] + "\n", 1);
        }
        else if (IsPlayerDead == true && StatsM.DataWasSaved == true)
        {
            // If Player Is Dead and Data was saved, restart the game.
            RestartTheGame(0);
        }
        else if (IsPlayerDead == false && TimerIsEnded == true && WordWasDecyphered == true)
        {
            // If Player is not Dead and the Timer Is Ended and the Word was decyphered -> restart the game.
            RestartTheGame(1);
        }
    }
    // Function which checks the User pressed letter, whether the current word has that letter or not.
    public void CheckPlayerSelectedLetter()
    {
        // If the Word is Active, and if the player is not dead and the world is not decpyhered, proceed further.
        if (WordIsShown == true && IsPlayerDead == false && WordWasDecyphered == false)
        {
            // For all User Available letters, check which was pressed and act accordingly.
            for (int i = 0; i < AllLetters.Length; i++)
            {
                // If The Letter seleced is the one, proceed.
                if (evs.currentSelectedGameObject.gameObject.ToString() == AllLetters[i].gameObject.ToString())
                {
                    int index = 0;
                    bool LetterWasFound = false;
                    // Find letter[s] in the word and show the found letter on screen by that letter location in the actual word.
                    foreach (char c in PrepData.WordsChosenList[WordIndex - 1])
                    {
                        Text PlayerChosenLetterText = CurrentWordAllLetters[index].GetComponent<Text>();
                        string CurrentWordLetter = c.ToString();

                        // If the letter was found, show it, paint the letter and play the sound.
                        if (CurrentWordLetter.ToUpper() == AllLetters[i].text)
                        {
                            PlayerChosenLetterText.text = CurrentWordLetter.ToUpper();
                            LetterWasFound = true;
                            AllLetters[i].color = Color.green;
                            FoundLettersCount++;
                            SoundM.PlaySuccessSound();
                        }
                        index++;
                    }
                    // If the letter is not found, paint it red, play the sound, etc.
                    if (LetterWasFound == false)
                    {
                        AllLetters[i].color = Color.red;
                        LivesLeft--;
                        LivesLeftCounter.text = LivesLeft.ToString();
                        Hangman.sprite = AllHangmanSprites[HangmanIndex];
                        HangmanIndex++;
                        SoundM.PlayFailedSound();
                    }
                    // If the count of found letters is equal to the length of the word, then the player decyphered the word.
                    if (FoundLettersCount == CurrentWordAllLetters.Length)
                    {
                        DisplayMessage("Atspėjote žodį!",0);
                        StatsM.UpdateStatistics(1000);
                        WordWasDecyphered = true;
                        break;
                    }
                    // Disable the button for the letter, despite the result.
                    GameObject gm = AllLetters[i].gameObject;
                    Button bts = gm.GetComponent<Button>();
                    bts.enabled = false;

                }
            }
            // Reset the event system selected object.
            evs.SetSelectedGameObject(null);
        }
    }
    // Function which shows the word.
    public void DisplayWord()
    {
        // If the index is less than the count of the chosen word list, proceed further.
        if (WordIndex  < PrepData.WordsChosenList.Count)
        {
            Debug.Log(PrepData.WordsChosenList[WordIndex]);
            // Get the array of the current word.
            CurrentWordAllLetters = new GameObject[PrepData.WordsChosenList[WordIndex].Length];

            // For loop in which we instantiate gameobject for every letter in the current word.
            for (int i = 0; i < PrepData.WordsChosenList[WordIndex].Length; i++)
            {
                // If the letter is not empty, proceed further.
                if (PrepData.WordsChosenList[WordIndex].Substring(i, 1) != string.Empty)
                {
                    // Instantiate the letter gameobject.
                    CurrentWordAllLetters[i] = Instantiate(UnderScore, WordArea.transform) as GameObject;

                    // ReactTransform of the current letter.
                    RectTransform rt = CurrentWordAllLetters[i].GetComponent<RectTransform>();

                    /// Change the gameobjects name to something more suitable.
                    CurrentWordAllLetters[i].name = "word_letter" + i;
                    
                    // Get the text component of the gameobject.
                    Text TextOfCurrentWord = CurrentWordAllLetters[i].GetComponent<Text>();

                    // If the current letter is space, make that gameobjects letter text to "" and add one to foundletterscount;
                    if (PrepData.WordsChosenList[WordIndex].Substring(i,1) == " " )
                    {
                        TextOfCurrentWord.text = "";
                        FoundLettersCount++;
                    }
                    // If the letter is not first, find the previous letter and add it's width and last position to the current letter.
                    if (i != 0)
                    {
                        RectTransform rtas = CurrentWordAllLetters[i - 1].GetComponent<RectTransform>();
                        RectTransform WordAreaRt = WordArea.GetComponent<RectTransform>();

                        if (rtas.rect.width * 2 + rtas.anchoredPosition.x  <= WordAreaRt.rect.width)
                            rt.anchoredPosition = new Vector2(rtas.rect.width + rtas.anchoredPosition.x, rtas.anchoredPosition.y);
                        else
                            rt.anchoredPosition = new Vector2(0, rt.anchoredPosition.y + rtas.anchoredPosition.y);
                    }
                }
            }      
            WordIndex++;
            WordIsShown = true;
        }
        else
        {
            // If the Index is higher then the current word list count, reset the index and restart the game.
            WordIndex = 0;
            RestartTheGame(0);
        }
    }
    // Function which displays the message to the player.Parameter 'mode' is for submit box to come on then the player is Dead.
    public void DisplayMessage(string message,int mode)
    {
        MessageBox.SetActive(true);
        MessageBoxText.text = message;
        if (mode == 1)
            SubmitScoreBox.SetActive(true);
    }
    // Function which restarts the game.Parameter 'status' is for determining what settings should be nulled if the player is Dead or he successfully decyphered the word.
    private void RestartTheGame(int status)
    {
        MessageBox.SetActive(false);
        Hangman.sprite = AllHangmanSprites[0];
        IsPlayerDead = false;
        TimerIsEnded = false;
        WordWasDecyphered = false;
        Timer        = TimerDefault;
        LivesLeft    = 8;
        HangmanIndex = 1;
        FoundLettersCount = 0;
        StatsM.Stats_FindingWordTimeInSeconds = 0;
        TimerSecondsCounter = TimerSecondsCounterDefault;

        DestroyAllLetterObjects();

        ResetAllPlayerLettersColor();

        LivesLeftCounter.text = "8";
        TimeSpentCounter.text = "0 s";

        if (status == 0)
        {
            StatsM.Stats_Score = 0;
            GameArea.SetActive(false);
            MainMenu.SetActive(true);
            SubmitScoreBox.SetActive(false);
            StatsM.DataWasSaved = false;
        }
        else if ( status == 1)
        {
            DisplayWord();
        }
    }
    // Function which resets all the letters colour back to black;
    private void ResetAllPlayerLettersColor()
    {
        for (int i = 0; i < AllLetters.Length; i++)
        {
            AllLetters[i].color = Color.black;
            GameObject gm = AllLetters[i].gameObject;
            Button bts = gm.GetComponent<Button>();
            bts.enabled = true;
        }
    }
    // Function which destroys all the letter objects.
    private void DestroyAllLetterObjects()
    {
        GameObject[] gameobjects = GameObject.FindGameObjectsWithTag("LetterSpace");
        foreach (GameObject go in gameobjects)
            Destroy(go);
    }
}