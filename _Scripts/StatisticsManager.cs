using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class StatisticsManager : MonoBehaviour {

    // Script which handles statistics.

    // Statistical variables.
    public int StatsScore,
               StatsFindingWordTimeInSeconds,
               StatsMainIndex;

    // Input field from which we take the player name to submit to playerprefs later.
    public InputField InputF;
 
    public bool DataWasSaved;

    public GameObject StatisticsValueObject,
                     StatisticsPageMain;

	void Start ()
    {
        StatsFindingWordTimeInSeconds = 0;
        StatsScore = 0;
        DataWasSaved = false;
        StatsMainIndex = PlayerPrefs.GetInt("main_index");
    }
    // Function which updates statistics after each question.
    public void UpdateStatistics(int value)
    {
        StatsScore += value;
    }
    // Function which saves statistics to playerprefs after the player is dead.
    public void SaveStatistics()
    {
        // If Data wasn't saved, save it.
        if (DataWasSaved == false)
        {
            PlayerPrefs.SetString( "player_name" + StatsMainIndex.ToString(), InputF.text);
            PlayerPrefs.SetInt("player_name" + StatsMainIndex.ToString() + "_scores", StatsScore);
            StatsMainIndex++;
            PlayerPrefs.SetInt("main_index", StatsMainIndex);
        }
        DataWasSaved = true;
    }
    // Function which displays statistics in Statistics Info Box.
    public void DisplayStatistics()
    {
        // Before displaying the statistical data, delete previous objects.
        GameObject[] gameobjects = GameObject.FindGameObjectsWithTag("StatisticsValueTag");
        foreach (GameObject go in gameobjects)
            Destroy(go);

        // Create gameobject array based on the count of playerprefs data.
        GameObject[] all_stats_value_objects = new GameObject[StatsMainIndex];
        for (int i = 0; i < StatsMainIndex; i++)
        {
            // Instantiate one statistical object which holds one username and it's score.
            all_stats_value_objects[i] = Instantiate(StatisticsValueObject,StatisticsPageMain.transform) as GameObject;

            RectTransform rt = all_stats_value_objects[i].GetComponent<RectTransform>();
            // Renaming the gameobject's name because it can be deleted easier and also could've be seen better in inspector.
            all_stats_value_objects[i].name = "StatisticsValue" + i;
            if (i != 0)
            {
                RectTransform rtas = all_stats_value_objects[i-1].GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rtas.anchoredPosition.y - rtas.rect.height);
            } 
            // For each tranform ( both for username and score ) find the tranforms and edit their texts with the data from the playerprefs.
            foreach( Transform trs in all_stats_value_objects[i].transform)
            {
                Text text_object = trs.gameObject.GetComponent<Text>();

                string PlayerPrefsValuePlayerUsername;
                int PlayerPrefsValuePlayerScore;

                if (trs.gameObject.name.ToString() == "ColumnPlayerName")
                {
                    PlayerPrefsValuePlayerUsername = PlayerPrefs.GetString("player_name" + i.ToString());
                    text_object.text = PlayerPrefsValuePlayerUsername;
                }
                else if (trs.gameObject.name.ToString() == "ColumnPlayerScore" )
                {
                    PlayerPrefsValuePlayerScore = PlayerPrefs.GetInt("player_name" + i.ToString() + "_scores");
                    text_object.text = PlayerPrefsValuePlayerScore.ToString();
                }
            }
        }
    }
}
