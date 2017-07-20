using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour {

    private GameManageris gm;

    public GameObject AboutTheGameBox;

    public Text PlayText;

	// Use this for initialization
	void Start () {
        gm = GameObject.Find("GameManagerScript").GetComponent<GameManageris>();
	}
	
    public void PlayButtonClicked()
    {
        gm.MainMenu.SetActive(false);
        gm.GameArea.SetActive(true);
    }
    public void AboutTheGameClicked()
    {
        AboutTheGameBox.SetActive(true);
    }
    public void AboutTheGameBoxExit()
    {
        AboutTheGameBox.SetActive(false);
    }
    public void StatisticsClicked()
    {


    }
    public void ReturnToMainMenu()
    {
        PlayText.text = "T Ę S T I";
        gm.MainMenu.SetActive(true);
        gm.GameArea.SetActive(false);

    }
    public void ExitClicked()
    {
        Application.Quit();
    }














}
