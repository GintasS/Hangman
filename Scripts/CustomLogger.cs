using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomLogger : MonoBehaviour {

    // Script that is responsible for logging all the logs throughout the app.

    // Bool that either enables/disables the ability to log messages throughout the app.
    [SerializeField] private bool IsAllowedToLogMessages;

    // Function that logs the message with the content.
    public void LogAMessage(string content)
    {
        if (IsAllowedToLogMessages == true)
        Debug.Log(content);
    }

}
