using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour {

    private GameManageris gm;

    public Text PlayText;

	// Use this for initialization
	void Start () {
        gm = GameObject.Find("GameManagerScript").GetComponent<GameManageris>();
	}
	
    public void PlayButtonClicked()
    {
        PlayText.text = "T Ę S T I";
        gm.MainMenu.SetActive(false);
        gm.GameArea.SetActive(true);
    }
    public void AboutTheGameClicked()
    {


    }
    public void StatisticsClicked()
    {


    }
    public void ReturnToMainMenu()
    {
        gm.MainMenu.SetActive(true);
        gm.GameArea.SetActive(false);

    }
    public void ExitClicked()
    {
        Application.Quit();
    }














}
