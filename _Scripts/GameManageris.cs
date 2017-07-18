using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManageris : MonoBehaviour {

    public GameObject MainMenu,
                      GameArea,
                      WordArea,
                      UnderScore;                

    private PrepareData PrepData;

    public bool IsPlayerDead;

    public Image Hangman;

    private int WordIndex;

    // Use this for initialization
    void Start () {
        PrepData= GameObject.Find("Data").GetComponent<PrepareData>();
        PrepData.ReadDataFromFile();
        DisplayWord();
        WordIndex = 0;
        IsPlayerDead = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (IsPlayerDead == false)
        {
            if (Hangman.sprite.name.ToString() == "Hangman_full")
                IsPlayerDead = true;
        }
    }
    private void DisplayWord()
    {
        GameObject[] all_objects = new GameObject[PrepData.Words[WordIndex].Length];
        int index = 1;

        for ( int i=0 ;i < PrepData.Words[WordIndex].Length;i++)
        {
            all_objects[index] = Instantiate(UnderScore,WordArea.transform) as GameObject;
            RectTransform rt = all_objects[i].GetComponent<RectTransform>();
            rt.anchorMax = new Vector2(0,1);
            rt.anchorMin = new Vector2(0,1);
            index++;
        }
        WordIndex++;
    }
}
