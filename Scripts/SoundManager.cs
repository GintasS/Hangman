using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    // Script that handles sound output.

    #region [SoundManager]: Variables
    // Main audio source from which sound is going to be played.
    [Header("Audio source")]
    [SerializeField] private AudioSource auds;

    // Audio clips which we will use in this application.
    [Header("Audio clips")]
    [SerializeField] private AudioClip SuccessSound;
    [SerializeField] private AudioClip FailedSound;
    [SerializeField] private AudioClip ButtonClickedSound;
    #endregion

    #region [SoundManager]: Sound actions
    // Play sound of success.
    public void PlaySuccessSound()
    {
        auds.clip = SuccessSound;
        auds.Play();
    }
    // Play sound of failure.
    public void PlayFailedSound()
    {
        auds.clip = FailedSound;
        auds.Play();
    }
    // Play button sound.
    public void PlayButtonClickedSound()
    {
        auds.clip = ButtonClickedSound;
        auds.Play();
    }
    #endregion
}
