using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System;

public class PrepareData : MonoBehaviour {

    public string[] Words;

    public TextAsset DataFile;

    public void ReadDataFromFile()
    {
        try
        {  
            Words = DataFile.text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);                
        }
        catch(Exception e)
        {
            if (e.Source != null)
                Debug.Log("IOException source: {0}" + e);
            throw;
        }
        if (Words != null)
            RandomizeData();

    }
    private void RandomizeData()
    {
        System.Random rnd = new System.Random();
        for ( int i = 0; i < Words.Length;i++)
        {
            int random_sk = rnd.Next(0,Words.Length-1);

            string laikinas = Words[random_sk];
            Words[random_sk] = Words[i];
            Words[i] = laikinas;

        }
    }

}
