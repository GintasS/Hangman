using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour {

    // Script for managing Main Menu interactions, showing various Info boxes and etc.

    // GameObjects for showing the InfoBoxes.
    public GameObject AboutTheGameBox,
                      StatisticsBox,
                      DifficultyBox;

    // Array for holding each Difficulty level's word count.
    public int[] DifficultyLevelWordCount;

    // Actual object for displaying each Difficulty level's word count.
    public Text DifficultyLevelWordCountText;

    // Button which after the click leads to the game zone.
    public Button DifficultyConfirmButton;

    // Text component used to change the text after the player paused the game.
    public Text PlayText;

    // Default Text used for displaying the word count.
    public string DifficultyLevelDefaultText;

    // Reference for scripts.
    private GameManageris gm;

    private SoundManager SoundM;

    private StatisticsManager StatsM;

    private PrepareData PrepD;

    void Start ()
    {
        gm     = GameObject.Find("GameManagerScript").GetComponent<GameManageris>();
        SoundM = GameObject.Find("SoundManagerScript").GetComponent<SoundManager>();
        PrepD  = GameObject.Find("Data").GetComponent<PrepareData>();
    }
    // Event functions for the buttons.

    // Play Button event.
    public void PlayButtonClicked()
    {
        SoundM.PlayButtonClickedSound();
        if (PlayText.text != "T Ę S T I")
        DifficultyBox.SetActive(true);
        else
        {
            gm.MainMenu.SetActive(false);
            gm.GameArea.SetActive(true);
        }
    }
    // About the Game Button event.
    public void AboutTheGameClicked()
    {
        SoundM.PlayButtonClickedSound();
        AboutTheGameBox.SetActive(true);
    }
    // About the Game Info Box Exit Button.
    public void AboutTheGameBoxExit()
    {
        SoundM.PlayButtonClickedSound();
        AboutTheGameBox.SetActive(false);
    }
    // Statistics Button event.
    public void StatisticsBoxClicked()
    {
        SoundM.PlayButtonClickedSound();
        StatisticsBox.SetActive(true);
    }
    // Statistics Info Box Exit Button.
    public void StatisticsBoxExit()
    {
        SoundM.PlayButtonClickedSound();
        StatisticsBox.SetActive(false);
    }
    // In-game Return to Main Menu Button.
    public void ReturnToMainMenu()
    {
        SoundM.PlayButtonClickedSound();
        PlayText.text = "T Ę S T I";
        gm.MainMenu.SetActive(true);
        gm.GameArea.SetActive(false);
    }
    // Selecting Difficulty Events.

    // Event for Button responsible for Difficulty Level Easy.
    public void SelectedDifficultyLevelEasy()
    {
        SoundM.PlayButtonClickedSound();
        PrepD.WordsChosenList = PrepD.WordsEasy;
        DifficultyLevelWordCountText.text = DifficultyLevelDefaultText  + "\n" + DifficultyLevelWordCount[0].ToString();
        DifficultyConfirmButton.interactable = true;
    }
    // Event for Button responsible for Difficulty Level Medium.
    public void SelectedDifficultyLevelMedium()
    {
        SoundM.PlayButtonClickedSound();
        PrepD.WordsChosenList = PrepD.WordsMedium;
        DifficultyLevelWordCountText.text = DifficultyLevelDefaultText + "\n" + DifficultyLevelWordCount[1].ToString();
        DifficultyConfirmButton.interactable = true;
    }
    // Event for Button responsible forr Difficulty Level Hard.
    public void SelectedDifficultyLevelHard()
    {
        SoundM.PlayButtonClickedSound();
        PrepD.WordsChosenList = PrepD.WordsHard;
        DifficultyLevelWordCountText.text = DifficultyLevelDefaultText + "\n" + DifficultyLevelWordCount[2].ToString();
        DifficultyConfirmButton.interactable = true;
    }
    // Event for Button responsible for Difficulty Level Very Hard.
    public void SelectedDifficultyLevelVeryHard()
    {
        SoundM.PlayButtonClickedSound();
        PrepD.WordsChosenList = PrepD.WordsVeryHard;
        DifficultyLevelWordCountText.text = DifficultyLevelDefaultText + "\n" + DifficultyLevelWordCount[3].ToString();
        DifficultyConfirmButton.interactable = true;
    }
    // Event for Button which confirms selected difficulty level.
    public void DifficultyLevelWasSelected()
    {
        SoundM.PlayButtonClickedSound();
        DifficultyConfirmButton.interactable = false;
        DifficultyLevelWordCountText.text = DifficultyLevelDefaultText + "\n" + "-";
        DifficultyBox.SetActive(false);
        gm.MainMenu.SetActive(false);
        gm.GameArea.SetActive(true);
        gm.DisplayWord();
    }
    // Event for going back to the Main Menu.
    public void DifficultyLevelExit()
    {
        SoundM.PlayButtonClickedSound();
        DifficultyConfirmButton.interactable = false;
        DifficultyLevelWordCountText.text = DifficultyLevelDefaultText + "\n" + "-";
        DifficultyBox.SetActive(false);
        gm.MainMenu.SetActive(true);
    }
    // Event for Button which exists the application.
    public void ExitClicked()
    {
        SoundM.PlayButtonClickedSound();
        Application.Quit();
    }
}
