using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimController : MonoBehaviour {

    // Script that is responsible for managing animations.

    #region [AnimController]: Variables
    // Animations that will be played.
    [Header("Animation clips")]
    [SerializeField] private AnimationClip MoveWindow;
    [SerializeField] private AnimationClip ComeBack;

    [SerializeField] private  AnimationClip[] GameAreaAnimationClips;
    // Animatable objects on whom animations will be played on.
    [Header("Animatable objects")]
    [SerializeField] public GameObject[] Windows;
    [SerializeField] public GameObject[] GameAreaAnimationObjects;

    // Animation indexes which trigger animations.
    [Header("Animation indexes which trigger animations")]
    [SerializeField] private int CurrentAnimationIndexOpen;
    [SerializeField] private int CurrentAnimationIndexClose;
    [SerializeField] private int CurrentMessageWindowModeIndex;

    public int CurrentGameAreaAnimationTransition;

    // Animation objects to whom animations will be assigned.
    [Header("Animation objects")]
    [SerializeField] private Animation AnimationWindow;
    [SerializeField] private Animation AnimationGame;
    [SerializeField] private Animation AnimationMessage;

    // Bool to determine if head was cut.
    [Header("Other objects")]
    public bool GameAreaHeadCutIsEnded;

    // References for scripts.
    [Header("Script references")]
    [SerializeField] private LocalGameManageris lgm;
    [SerializeField] private AdsScriptas adsas;
    [SerializeField] private MainMenu mm;
    [SerializeField] private FacebookScript fbs;
    [SerializeField] private StatisticsManager sm;
    #endregion

    #region [AnimController]: Start function
    // Initializing the values.
    void Start ()
    {
        GameAreaHeadCutIsEnded = false;
        CurrentAnimationIndexClose = -1;
        CurrentAnimationIndexOpen = -1;
        CurrentMessageWindowModeIndex = -1;
        CurrentGameAreaAnimationTransition = -1;
    }
    #endregion

    #region [AnimController]: Play Forward/Back window animations
    // Play animation of window moving forward.
    public void PlayAnimForward(int index)
    {
       AnimationWindow = Windows[index].GetComponent<Animation>();
       AnimationWindow.clip= MoveWindow;
       AnimationWindow.Play();
       CurrentAnimationIndexOpen = index;        
    }
    // Play animation of window moving back.
    public void PlayAnimBack(int index)
    {
        AnimationWindow = Windows[index].GetComponent<Animation>();
        AnimationWindow.clip = ComeBack;
        AnimationWindow.Play();
        CurrentAnimationIndexClose = index;
    }
    #endregion

    #region [AnimController]: Play Game animations
    // Play animation that plays directly inside the game area.
    public void PlayGameAreaAnim(int clip_index,int object_index,int transitionas)
    {
        AnimationGame = GameAreaAnimationObjects[object_index].GetComponent<Animation>();
        AnimationGame.clip = GameAreaAnimationClips[clip_index];
        AnimationGame.Play();
        CurrentGameAreaAnimationTransition = transitionas;
    }
    #endregion

    #region [AnimController]: Play Message window animations
    // Play animation of the message window.
    public void PlayMessageAnim(int clip_index,int object_index,int message_mode)
    {
        AnimationMessage = GameAreaAnimationObjects[object_index].GetComponent<Animation>();
        AnimationMessage.clip = GameAreaAnimationClips[clip_index];
        AnimationMessage.Play();
        CurrentMessageWindowModeIndex = message_mode;
    }
    #endregion

    #region [AnimController]: Reset all animation data
    // Reset the animation data.
    public void ResetAnimationData()
    {
        CurrentAnimationIndexClose = -1;
        CurrentAnimationIndexOpen = -1;
        CurrentMessageWindowModeIndex = -1;
        CurrentGameAreaAnimationTransition = -1;

        AnimationWindow = null;
        AnimationMessage = null;

        GameAreaHeadCutIsEnded = false;
    }
    #endregion

    void Update ()
    {
      #region [AnimController]: Window animations

        // PlayAnimForward(int index).
        // Window animations that will be played each time user opens a new window.
        if ( AnimationWindow != null && AnimationWindow.isPlaying == false)
        {
            if (CurrentAnimationIndexOpen == 6 && mm.CurrentGameMode == "SinglePlayer")
            {
                // From Difficulty info box to Game Area.
                // Moving Difficulty info box back at the same time.
                lgm.DisplayWord();
                CurrentAnimationIndexOpen = -1;
                PlayAnimBack(0);
                PlayAnimBack(1);
            }
            else if ( CurrentAnimationIndexOpen == 6 && mm.CurrentGameMode == "TwoPlayers" )
            {
                // From Two players info box to Game Area.
                // Moving Two players info box back at the same time.
                lgm.DisplayWord();
                CurrentAnimationIndexOpen = -1;
                PlayAnimBack(2);
                PlayAnimBack(0);
            }
            else if  (CurrentAnimationIndexOpen == 8)
            {   
                // From Main Menu to Statistics info box.
                sm.FacebookStatisticsCenter();
                CurrentAnimationIndexOpen = -1;
            }

            if ( CurrentAnimationIndexClose == 6 )
            {
                // At the same time user is returning to the Main Menu, close Message window.
                PlayMessageAnim(4, 2, 4);
                CurrentAnimationIndexClose = -1;
            }
        }

        #endregion

      #region [AnimController]: Game object animations

        // PlayGameAreaAnim (clip_index,object_index,transition_index).
        // Game animations that will be played inside Game Area.
        if ( AnimationGame != null && AnimationGame.isPlaying == false && lgm.GameIsPaused == false)
        {
            if (CurrentGameAreaAnimationTransition != -1)
            {
                if (CurrentGameAreaAnimationTransition == 0) // Move head after the guillotine dropped.
                    PlayGameAreaAnim(1, 1, 1);
                else if (CurrentGameAreaAnimationTransition == 1 && GameAreaHeadCutIsEnded == false) // Set the bool so that 'Move head' animation would not repeat.
                    GameAreaHeadCutIsEnded = true;
            }
        }

        #endregion

      #region [AnimController]: Interstitial Ad response animation

        // PlayMessageAnim(int clip_index,int object_index,int message_mode).
        // Interstitial ad animation that will be played after 'Forward' or 'Restart' event.
        if ((adsas.InterstitialAdIsDisplaying == 0 && adsas.InterstitialAdIsLoaded == 1) || adsas.InterstitialAdIsLoaded == 0 )
        {
            if (adsas.InterstitialOriginatedFrom == "Forward" || adsas.InterstitialOriginatedFrom == "Restart")
            {
                PlayMessageAnim(4, 2, 5);
                adsas.InterstitialAdIsDisplaying = -1;
                adsas.InterstitialAdIsLoaded     = -1;
                adsas.InterstitialAdTryingToLoad = -1;
                adsas.InterstitialOriginatedFrom = "";
            }
        }
        #endregion

      #region [AnimController]: Message window animations
        // PlayMessageAnim (clip_index,object_index,message mode).
        // Message window animations that will be played inside Game Area.
        if ( AnimationMessage != null && AnimationMessage.isPlaying == false && CurrentMessageWindowModeIndex != -1)
        {
            if (CurrentMessageWindowModeIndex == 0) // From Game Area to Message => MessageWindowsWon.
            {
                lgm.DisplayMessage(0);
                
                // If the game mode is not 'Two Players', try to save score to the custom server.
                if ( mm.CurrentGameMode != "TwoPlayers")
                    fbs.LogIntoFBWriteAScore();
            }
            else if (CurrentMessageWindowModeIndex == 3) // From MessageWindowsPaused to Game Area.
                lgm.MessageBoxPause.SetActive(false);
            else if (CurrentMessageWindowModeIndex == 4) // Close Message Window (restart) and set Game Area active to false.
                lgm.GameArea.SetActive(false);
            else if (CurrentMessageWindowModeIndex == 5) // After displaying the ad, get another word from the list.
                lgm.DisplayWord();
            else if (CurrentMessageWindowModeIndex == 7) // Set all buttons to active after the Share status animation is done.
                fbs.ChangeAllMessageButtonsState(true);

            CurrentMessageWindowModeIndex = -1;
        }
        #endregion
    }
}
