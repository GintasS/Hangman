using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System;

public class PrepareData : MonoBehaviour {

    // Script responsible for preparing the data ( words ) from the text file.

    // Main array which holds all the words.
    public string[] Words;

    // Lists who are holding words by their difficulty levels.
    public List<string> WordsEasy,
                         WordsMedium,
                         WordsHard,
                         WordsVeryHard,
                         WordsChosenList;

    // Text file holding all the words.
    public TextAsset DataFile;

    // Bool for determining if the preparation process is finished.
    public bool DataActionsAreFinished;

    // Reference to script.
    private GameManageris gm;

    void Start()
    {
        gm = GameObject.Find("GameManagerScript").GetComponent<GameManageris>();
        DataActionsAreFinished = false;
    }
    // Read data from the file.
    public void ReadDataFromFile()
    {
        try
        {  
            Words = DataFile.text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);                
        }
        catch(Exception e)
        {
            if (e.Source != null)
                gm.DisplayMessage("Nepavyko nuskaityti žodžių, nes: " + e,0);
            throw;
        }
        // If everything is OK, proceed to the other stage.
        if (Words != null)
        RandomizeData();
    }
    // Randomize data and sort that data into lists by their difficulty levels.
    private void RandomizeData()
    {
        // Seed for generating random values.
        System.Random rnd = new System.Random();

        // Loop for swapping the values, making them appear in random order.
        for ( int i = 0; i < Words.Length;i++)
        {
            int random_sk = rnd.Next(0,Words.Length-1);

            string laikinas = Words[random_sk];
            Words[random_sk] = Words[i];
            Words[i] = laikinas;
        }
        DataActionsAreFinished = true;

        // Sort the words into difficulty levels.
        for ( int d = 0; d < Words.Length;d++)
        {
            // Trims the end of the word to remove the carriage character which comes in to play in android platfomr.
            Words[d] = Words[d].TrimEnd(new char[] { '\r', '\n' });

            // Add word to the list by the length.
            if (Words[d].Length > 0 && Words[d].Length <= 5)
                WordsEasy.Add(Words[d]);
            else if (Words[d].Length > 5 && Words[d].Length <= 10)
                WordsMedium.Add(Words[d]);
            else if (Words[d].Length > 10 && Words[d].Length <= 15)
                WordsHard.Add(Words[d]);
            else if (Words[d].Length > 15)
                WordsVeryHard.Add(Words[d]);
        }
    }
}