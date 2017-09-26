using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{
    // Script that is responsible for managing Main Menu interactions, showing various Info boxes and etc.

    #region [MainMenu]: Variables
    // Game objects for showing Info boxes.
    [Header("Main Menu gameobjects")]
    [SerializeField] private GameObject AboutTheGameBox;
    [SerializeField] private GameObject StatisticsBox;
    [SerializeField] private GameObject DifficultyBox;
    [SerializeField] private GameObject GameModeBox;
    [SerializeField] private GameObject ShopBox;
    [SerializeField] private GameObject TwoPlayersGameModeInfoBox;

    // Button, which after the click, leads to the game zone.
    [SerializeField] private Button TwoPlayersGameModeConfirmButton;

    [Header("Difficulty level variables ")]
    // String that holds current difficulty level in words.
    public string CurrentDifficultyLevel;

    // Strings that are holding game mode data in words.
    [Header("Game mode variables")]
    public string CurrentGameMode;
    public string MostRecentGameMode;

    // Strings that are holding Two Players game mode data.
    [Header("Two Player game mode variables")]
    [SerializeField] private InputField CustomWordInput;
    [SerializeField] private string[] TwoPlayersGameModeIllegalLetters;
    [SerializeField] [Range(1, 30)] private int CustomWordMinLength,
                                                CustomWordMaxLength;

    // References for scripts.
    [Header("Script references")]
    [SerializeField] private LocalGameManageris LocalM;
    [SerializeField] private SoundManager SoundM;
    [SerializeField] private StatisticsManager StatsM;
    [SerializeField] private PrepareData PrepD;
    [SerializeField] private AnimController Anc;
    [SerializeField] private CustomLogger Cus;
    #endregion

    #region [MainMenu]: Open and close events
    // Function for Play button.
    public void PlayButtonClicked()
    {
        SoundM.PlayButtonClickedSound();
        Anc.PlayAnimForward(0);
    }
    // Function for About Game button.
    public void AboutTheGameClicked()
    {
        SoundM.PlayButtonClickedSound();
        Anc.PlayAnimForward(3);
    }
    // Function for About Game exit button.
    public void AboutTheGameBoxExit()
    {
        SoundM.PlayButtonClickedSound();
        Anc.PlayAnimBack(3);
    }
    // Function for Statistics button.
    public void StatisticsBoxClicked()
    {
        SoundM.PlayButtonClickedSound();
        Anc.PlayAnimForward(8);         
    }
    // Function for Statistics exit button.
    public void StatisticsBoxExit()
    {
        SoundM.PlayButtonClickedSound();
        Anc.PlayAnimBack(8);     
    }
    // Function for Main Shop page button.
    public void ShopBoxClicked()
    {
        SoundM.PlayButtonClickedSound();
        Anc.PlayAnimForward(4);
    }
    // Function for Main Shop page exit button.
    public void ShopBoxExit()
    {
        SoundM.PlayButtonClickedSound();
        Anc.PlayAnimBack(4);
    }
    // Function for Game exit button.
    public void ExitClicked()
    {
        SoundM.PlayButtonClickedSound();
        Application.Quit();
    }

    #endregion

    #region [MainMenu]: Selecting a game mode
    // Function for selecting Single Player game mode.
    public void GameModeSinglePlayer()
    {
        SoundM.PlayButtonClickedSound();
        Anc.PlayAnimForward(1);
        CurrentGameMode = "SinglePlayer";
        GameModeWasSelected();
    }
    // Function for selecting Two Players game mode.
    public void GameModeTwoPlayers()
    {
        SoundM.PlayButtonClickedSound();
        CurrentGameMode = "TwoPlayers";
        GameModeWasSelected();
    }
    // Function for confirming selected game mode.
    public void GameModeWasSelected()
    {
        if (CurrentGameMode == "SinglePlayer")
            Anc.PlayAnimForward(1);
        else if (CurrentGameMode == "TwoPlayers")
        {
            CustomWordInput.text = "Įveskite žodį";
            Anc.PlayAnimForward(2);
        }
        Cus.LogAMessage("[Game]: Game mode was selected: " + CurrentGameMode + "\n");
        MostRecentGameMode = CurrentGameMode;
    }
    // Function for Game mode Info box exit button.
    public void GameModeExit()
    {
        SoundM.PlayButtonClickedSound();
        Anc.PlayAnimBack(0);
        CurrentGameMode = "";
        LocalM.enabled = false;
    }
    // Function for Two players Info box exit button.
    public void TwoPlayersGameModeExit()
    {
        SoundM.PlayButtonClickedSound();
        Anc.PlayAnimBack(2);
        TwoPlayersGameModeConfirmButton.interactable = false;
        CustomWordInput.text = "Įveskite žodį";
    }

    #endregion

    #region [MainMenu]: Selecting a difficulty
    // Function for selecting difficulty level: easy.
    public void SelectedDifficultyLevelEasy()
    {
        SoundM.PlayButtonClickedSound();
        PrepD.WordsChosenList = PrepD.WordsEasy;
        CurrentDifficultyLevel = "Easy";
        DifficultyLevelWasSelected();
    }
    // Function for selecting difficulty level: medium.
    public void SelectedDifficultyLevelMedium()
    {
        SoundM.PlayButtonClickedSound();
        PrepD.WordsChosenList = PrepD.WordsMedium;
        CurrentDifficultyLevel = "Medium";
        DifficultyLevelWasSelected();
    }
    // Function for selecting difficulty level: hard.
    public void SelectedDifficultyLevelHard()
    {
        SoundM.PlayButtonClickedSound();
        PrepD.WordsChosenList = PrepD.WordsHard;     
        CurrentDifficultyLevel = "Hard";
        DifficultyLevelWasSelected();
    }
    // Function for selecting difficulty level: very hard (expert).
    public void SelectedDifficultyLevelVeryHard()
    {
        SoundM.PlayButtonClickedSound();
        PrepD.WordsChosenList = PrepD.WordsVeryHard;

        CurrentDifficultyLevel = "VeryHard";
        DifficultyLevelWasSelected();
    }
    // Function for confirming selected difficulty level.
    public void DifficultyLevelWasSelected()
    {
        Cus.LogAMessage("[Game]: Difficulty was selected: " + CurrentDifficultyLevel + "\n");

        LocalM.enabled = true;

        LocalM.GameArea.SetActive(true);
        Anc.PlayAnimForward(6); 
    }
    // Function for exiting difficulty level info box.
    public void DifficultyLevelExit()
    {
        SoundM.PlayButtonClickedSound();
        CurrentDifficultyLevel = "";
        Anc.PlayAnimBack(1);
    }

    #endregion

    #region [MainMenu]: Two players game mode custom word editing
    // Function for confirming Two player's game mode custom word and moving to play area.
    public void TwoPlayersGameModeWordWasSelected()
    {
        SoundM.PlayButtonClickedSound();
        LocalM.enabled = true;
        LocalM.TwoPlayersModeCustomWord = CustomWordInput.text;

        LocalM.GameArea.SetActive(true);
        Anc.PlayAnimForward(6);
    }
    // Function for making a custom word apply to the main rules.
    public void TwoPlayersGameModeOnStringEditEnd()
    {
        if (CustomWordInput.text.Length >= CustomWordMinLength && CustomWordInput.text.Length <= CustomWordMaxLength)
            TwoPlayersGameModeConfirmButton.interactable = true;
        else if (CustomWordInput.text == null || CustomWordInput.text.Length < CustomWordMinLength || CustomWordInput.text.Length > CustomWordMaxLength)
            TwoPlayersGameModeConfirmButton.interactable = false;
    }
    // Function for finding if the custom word contains banned values.
    public void TwoPlayersGameModeOnStringEditChange()
    {
        for ( int i = 0; i < TwoPlayersGameModeIllegalLetters.Length;i++)
        {
            if (CustomWordInput.text.IndexOf(TwoPlayersGameModeIllegalLetters[i]) != -1)
                CustomWordInput.text = CustomWordInput.text.Replace(TwoPlayersGameModeIllegalLetters[i], "");
        }
    }
    #endregion

}
