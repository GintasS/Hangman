using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    // Basic script, which handles sound output.
    
    public AudioSource auds;

    // Audio clips which we will use in this application.
    public AudioClip SuccessSound,
                     FailedSound,
                     ButtonClickedSound;

    // Play sound of success then the player gets the letter or he's decyphered it.
	public void PlaySuccessSound()
    {
        auds.clip = SuccessSound;
        auds.Play();
    }
    // Play sound of failure then the player dies or picks the wrong letter.
    public void PlayFailedSound()
    {
        auds.clip = FailedSound;
        auds.Play();
    }
    // Play button sound for each player interaction with buttons.
    public void PlayButtonClickedSound()
    {
        auds.clip = ButtonClickedSound;
        auds.Play();
    }
}
