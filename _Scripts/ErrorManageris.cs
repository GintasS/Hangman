using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorManageris : MonoBehaviour {

    // Script that is responsible for showing most important errors that require user attention.

    #region [ErrorManageris]: Main variables
    
    // Main game objects.
    [Header("Main game objects")]
    [SerializeField] private GameObject ErrorArea;
    [SerializeField] private Text ErrorTextas;
    [SerializeField] private Button[] ButtonsToDisable;

    // References for scripts.
    [Header("Script references")]
    [SerializeField] private SoundManager SoundM;
    #endregion

    #region [ErrorManageris]: Show/Hide error window
    // Showing error info box with the text.
    public void ShowErrorArea(string textas,int mode)
    {
        // Mode = -1, no buttons to disable.
        // Mode != -1, there are buttons to disable.

        ErrorArea.SetActive(true);
        ErrorTextas.text = textas;
        SoundM.PlayFailedSound();

        if (mode != -1) 
        {
            int start_index_loop = -1,
                 end_index_loop = -1;

            if ( mode == 1) // Error => couldn't load Text assets.
            {
                start_index_loop = 0;
                end_index_loop = 5;
            }
            else if ( mode == 2) // Error => Facebook SDK failed to initialize.
            {
                start_index_loop = 4;
                end_index_loop = 7;

            }
            // Set the buttons to be not interactable by their start and end indexes.
            for (int i = start_index_loop; i < end_index_loop; i++)
                ButtonsToDisable[i].interactable = false;
        }
    }
    // Hide error info box.
    public void HideErrorArea()
    {
        SoundM.PlayButtonClickedSound();
        ErrorArea.SetActive(false);
        ErrorTextas.text = "";
    }
    #endregion
}
