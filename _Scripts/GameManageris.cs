using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameManageris : MonoBehaviour {

    public GameObject MainMenu,
                      GameArea,
                      WordArea,
                      UnderScore,
                      MessageBox;

    public GameObject[] Current_word_all_letters;

    public Text[] All_letters;

    public Text LivesLeftCounter,
                MessageBoxText;

    public Sprite[] AllHangmanSprites;

    public Image Hangman;

    private PrepareData PrepData;

    public bool WordWasDecyphered,
                IsPlayerDead,
                WordIsShown,
                TimerIsEnded;

    public int WordIndex,
               LivesLeft,
               SecondsSpent,
               HangmanIndex,
               FoundLettersCount;

    

    public EventSystem evs;

    public float Timer,
                 Timer_default;

    // Use this for initialization
    void Start ()
    {
        PrepData= GameObject.Find("Data").GetComponent<PrepareData>();
        PrepData.ReadDataFromFile();
        DisplayWord();
        IsPlayerDead = false;
        TimerIsEnded = false;
        WordWasDecyphered = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (WordWasDecyphered == true && IsPlayerDead == false)
        {
            if ( Timer > 0 && TimerIsEnded == false)
                Timer -= Time.deltaTime;
            else if ( Timer < 0 && TimerIsEnded == false)
            {
                Timer = Timer_default;
                TimerIsEnded = true;
            }

        }
        if (IsPlayerDead == false && TimerIsEnded == true && WordWasDecyphered == true)
        {
            RestartTheGame(1);
        }


        if (IsPlayerDead == false && LivesLeft == 0)
        {
            IsPlayerDead = true;
            DisplayMessage("Žaidimą pralaimėjote!" + "\n"  + "\n" + "Žodis buvo: " + PrepData.Words[WordIndex-1]);
        }
        else if ( IsPlayerDead == true && TimerIsEnded == false)
        {
            if ( Timer > 0 && TimerIsEnded == false)
                Timer -= Time.deltaTime;
            else if ( Timer < 0 && TimerIsEnded == false)
            {
                Timer = Timer_default;
                TimerIsEnded = true;
            }
        }
        else if ( IsPlayerDead == true && TimerIsEnded == true)
        {
            RestartTheGame(0);
        }

        if (evs.currentSelectedGameObject != null && PrepData.DataActionsAreFinished == true && WordIsShown == true && IsPlayerDead == false && WordWasDecyphered == false)
        {
            for (int i = 0; i < All_letters.Length; i++)
            {
                if (evs.currentSelectedGameObject.gameObject.ToString() == All_letters[i].gameObject.ToString())
                {
                    int index = 0;
                    bool letter_was_found = false;
                    foreach ( char c in PrepData.Words[WordIndex-1])
                    {
                        Text cur_word_text = Current_word_all_letters[index].GetComponent<Text>();
                        string e_converted = c.ToString();

                        if (e_converted.ToUpper() == All_letters[i].text)
                        {
                            cur_word_text.text = e_converted.ToUpper();
                            letter_was_found = true;
                            All_letters[i].color = Color.green;
                            FoundLettersCount++;
                        }
                        index++;
                    }
                    if (letter_was_found == false)
                    {
                        All_letters[i].color = Color.red;
                        LivesLeft--;
                        LivesLeftCounter.text = LivesLeft.ToString();
                        Hangman.sprite = AllHangmanSprites[HangmanIndex];
                        HangmanIndex++;
                    }
                    if (FoundLettersCount == Current_word_all_letters.Length)
                    {
                        DisplayMessage("Atspėjote žodį!");
                        WordWasDecyphered = true;
                        break;
                    }

                }
            }

            


            evs.SetSelectedGameObject(null);
        }
    }
    private void DisplayWord()
    {
        Current_word_all_letters = new GameObject[PrepData.Words[WordIndex].Length];
        Debug.Log(PrepData.Words[WordIndex]);
        for (int i = 0; i < PrepData.Words[WordIndex].Length; i++)
        {
            Current_word_all_letters[i] = Instantiate(UnderScore, WordArea.transform) as GameObject;
            RectTransform rt = Current_word_all_letters[i].GetComponent<RectTransform>();
            Current_word_all_letters[i].name = "word_letter" + i;
            rt.anchorMax = new Vector2(0, 1);
            rt.anchorMin = new Vector2(0, 1);

            if (i != 0)
            {
                RectTransform rtas         = Current_word_all_letters[i - 1].GetComponent<RectTransform>();
                RectTransform word_area_rt = WordArea.GetComponent<RectTransform>();

                float x = rtas.rect.width + rtas.anchoredPosition.x;
                float y = rtas.anchoredPosition.y;

                if ( x + rtas.rect.width <= word_area_rt.rect.width)
                  rt.anchoredPosition = new Vector2(x,y);
                else
                  rt.anchoredPosition = new Vector2(0,rt.anchoredPosition.y+y);

            }            
        }
        WordIndex++;
        WordIsShown = true;
    }
    private void DisplayMessage(string message)
    {
        MessageBox.SetActive(true);
        MessageBoxText.text = message;
    }
    private void RestartTheGame(int status)
    {   
        MessageBox.SetActive(false);
        Hangman.sprite = AllHangmanSprites[0];
        IsPlayerDead = false;
        TimerIsEnded = false;
        WordWasDecyphered = false;
        Timer        = Timer_default;
        LivesLeft    = 8;
        SecondsSpent = 0;
        HangmanIndex = 1;
        FoundLettersCount = 0;

        DestroyAllLetterObjects();

        ResetAllPlayerLettersColor();

        LivesLeftCounter.text = "8";

        DisplayWord();
        if (status == 0)
        {
            GameArea.SetActive(false);
            MainMenu.SetActive(true);
        }
    }
    private void ResetAllPlayerLettersColor()
    {
        for (int i = 0; i < All_letters.Length; i++)
            All_letters[i].color = Color.black;
    }

    private void DestroyAllLetterObjects()
    {
        GameObject[] gameobjects = GameObject.FindGameObjectsWithTag("LetterSpace");
        foreach (GameObject go in gameobjects)
            Destroy(go);
    }

}