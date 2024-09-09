using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.MiniJSON;
using System.Security.Cryptography;
using System.Text;
using System;

public class ServerScoreboard : MonoBehaviour {

    // Script that is responsible for 'talking' with a custom server.

    #region [ServerScoreboard]: Variables
    // Hash values for the authentication.
    [Header("Hash values")]
    public string CustomHashValueWritingScore;
    public string CustomHashValueSearchForScore;
    public string CustomHashValuePublishScore;

    // Random hash values to add for the authentication.
    [Header("Random integer bounds to add for hash")]
    [SerializeField] [Range(0,1000)] private int HashAdditionRandomIntMin;
    [SerializeField] [Range(0,1000)] private int HashAdditionRandomIntMax;

    // Url bases for server requests.
    [Header("Url bases")]
    [SerializeField] private string WriteScoreToServerUrlBase;
    [SerializeField] private string SearchForScoreServerUrlBase;

    // References for scripts.
    [Header("Script references")]
    [SerializeField] private StatisticsManager StatsM;
    [SerializeField] private ErrorManageris ErrorM;
    [SerializeField] private CustomLogger Cus;
    #endregion

    #region [ServerScoreboard]: Interactions with a server: reading and writing
    public IEnumerator WriteScoreToServer(int score,string facebookid)
    {
        // Writing score to the server and waiting for the callback with the data.
        // Callback is then checked to see whetver the server returned error or not.

        Cus.LogAMessage("[ServerScoreboard]: Trying to write the score to the Server DB... \n");

        WWW www = new WWW(url);

        yield return www;

        if (www.isDone == true)
        {
            if (!string.IsNullOrEmpty(www.error) || www.text.Contains("[ERROR]") == true)
            {
                if (www.error != null && www.error != String.Empty && www.error != "")
                    Cus.LogAMessage("[ServerScoreboard]: Error while trying to open/load the website DB, because: " + www.error.ToString());
                else
                    Cus.LogAMessage("[ServerScoreboard]: Error while trying to open/load the website DB, because: " + www.text);
            }
            else
            {
                Cus.LogAMessage("[ServerScoreboard]: Successfully uploaded the score to the server DB...\n");
                PlayerPrefs.SetInt("highest_player_score", StatsM.StatsScoreFromWord);
                int b = PlayerPrefs.GetInt("highest_player_score")
;               Cus.LogAMessage("[ServerScoreboard]: Highest player score set to:" + b);
            }
        }
    }
    public IEnumerator SearchForScoreInServerDB(string facebookid)
    {
        // Searching for the score in server DB and waiting for the callback with the data.
        // Callback is then checked to see whetver the server returned error or not.

        Cus.LogAMessage("[ServerScoreboard]: Searching through database for facebook id record.. \n");

        WWW www = new WWW(url);

        yield return www;

        if (www.isDone == true)
        {
            if (!string.IsNullOrEmpty(www.error))
            {
                Cus.LogAMessage("[ServerScoreboard]: Error while trying to search in Server DB, because: " + www.error.ToString());
                StatsM.FacebookStatsScoresCount--;
            }
            else
            {
                Cus.LogAMessage("[ServerScoreboard]: Successfully got the data from the Server DB..\n");

                if (www.text != null && www.text != "" && www.text != string.Empty && www.text.StartsWith("{") == true)
                {
                    ParseServerDBdata(www.text);
                }
                else
                {
                    StatsM.CurrentUserFacebookFriendsScores.Add("-");
                    StatsM.FacebookStatsScoresCount++;
                }
            }               
        }
    }
    #endregion

    #region [ServerScoreboard]: Interactions with a server: creating a hash and parsing read data
    public string GetHashForAuthenticaiton(string text)
    {
        // Generate the main hash that will be used with the hash addition to authenticate the request.

        Cus.LogAMessage("[ServerScoreboard]: Generating the main hash for authentication.. \n");

        byte[] bytes = Encoding.UTF8.GetBytes(text);
        SHA256Managed hashstring = new SHA256Managed();
        byte[] hash = hashstring.ComputeHash(bytes);
        string hashString = string.Empty;

        foreach (byte x in hash)
            hashString += String.Format("{0:x2}", x);

        return hashString;
    }
    public int GetRandomHashAddition()
    {
        // Generate random hash addition for extra protection.

        Cus.LogAMessage("[ServerScoreboard]: Generating random hash addition for authentication.. \n");



        return random_int;
    }

    private void ParseServerDBdata(string data)
    {
        // Parse the data from the server and add it to the list.


    }
    #endregion
}
