using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public class PrepareData : MonoBehaviour {

    // Script that is responsible for preparing data for the app.

    #region [PrepareData]: Variables
    // String arrays that are holding all the words, that are sorted by the difficulty level.
    [Header("Word lists that are holding all the words")]
    public string[] WordsEasy;
    public string[] WordsMedium;
    public string[] WordsHard;
    public string[] WordsVeryHard;
    public string[] WordsChosenList;

    [Header("Text Assets")]
    // Text file assets that are holding all the words.
    [SerializeField] private TextAsset DataFileEasy;
    [SerializeField] private TextAsset DataFileMedium;
    [SerializeField] private TextAsset DataFileHard;
    [SerializeField] private TextAsset DataFileVeryHard;

    [SerializeField] [Range(1, 10)] private int TextAssetCount;
    // Indexes for each of a difficulty, to remove cheating with switching difficulties.
    [Header("Indexes for each difficulty")]
    public int EasyIndex;
    public int MediumIndex;
    public int HardIndex;
    public int VeryHardIndex;

    // Bool for determining if the data preparation process is finished.
    [SerializeField] private bool DataActionsAreFinished;

    // References for scripts.
    [Header("Script references")]
    [SerializeField] private ErrorManageris ErrorM;
    [SerializeField] private CustomLogger Cus;

    #endregion

    #region [PrepareData]: Start function
    // Initialize the values.
    void Start()
    {
        DataActionsAreFinished = false;
        EasyIndex     = 0;
        MediumIndex   = 0;
        HardIndex     = 0;
        VeryHardIndex = 0;

        ReadDataFromFile();
    }
    #endregion

    #region [PrepareData]: Reading and shuffling the data
    // Read the data from the text assets.
    private void ReadDataFromFile()
    {
        for (int i = 0,b = 1; i < TextAssetCount; i++,b++)
        {
            try
            {
                // Parse the text asset by the index.
                if (i == 0)
                    WordsEasy = DataFileEasy.text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                else if (i == 1)
                    WordsMedium = DataFileMedium.text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                else if ( i == 2)
                    WordsHard = DataFileHard.text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                else if ( i == 3)
                    WordsVeryHard = DataFileVeryHard.text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            }
            catch (Exception e)
            {
                if (e.Source != null)
                     ErrorM.ShowErrorArea("Nepavyko nuskaityti " + b.ToString() + " duomenų failo, nes: "+Environment.NewLine  + e.Message + " !",1);
                    
                throw;
            }

        }

        // If everything is OK, proceed to the other stage.
        if (WordsEasy != null && WordsMedium != null && WordsHard != null && WordsVeryHard != null)
            RandomizeData();
        else
            ErrorM.ShowErrorArea("Nepavyko nuskaityti duomenų failų,"+Environment.NewLine+" todėl nebus galima žaisti žaidimo...",1);
    }
    // Randomize data and sort that data into lists by their difficulty levels.
    private void RandomizeData()
    {
        // Seed for generating random values.
        System.Random rnd = new System.Random();

        string[] def_array = null ;
        int random_sk = 0;
        string laikinas;
        for (int b = 0; b < TextAssetCount; b++)
        {
            // Choose the word list.
            if (b == 0)
                def_array = WordsEasy;
            else if (b == 1)
                def_array = WordsMedium;
            else if (b == 2)
                def_array = WordsHard;
            else if (b == 3)
                def_array = WordsVeryHard;

            for (int i = 0; i < def_array.Length; i++)
            {
                // Randomize values in the word list.
                random_sk = rnd.Next(0, def_array.Length - 1);

                def_array[i]         = def_array[i].TrimEnd(new char[] { '\r', '\n' });
                def_array[random_sk] = def_array[random_sk].TrimEnd(new char[] { '\r', '\n' });

                laikinas = def_array[random_sk];
                def_array[random_sk] = def_array[i];
                def_array[i] = laikinas;
            }
        }

        Cus.LogAMessage("EASY: " + WordsEasy.Length);
        Cus.LogAMessage("MED: " + WordsMedium.Length);
        Cus.LogAMessage("HARD: " + WordsHard.Length);
        Cus.LogAMessage("VERYHARD: " + WordsVeryHard.Length);

        DataActionsAreFinished = true;
    }
    #endregion
}