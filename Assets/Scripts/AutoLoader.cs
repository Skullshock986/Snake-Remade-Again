using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoLoader : MonoBehaviour
{
    public static bool initialSceneLoaded;
    [SerializeField] string mainSceneName;

    private void Update() // Runs every frame
    {
        if (initialSceneLoaded)
        {
            // In the case the Initial scene is already loaded, do not run any code
            Debug.LogWarning("The Initial scene has already been loaded. This is not allowed. Cancelling main menu auto-load.");
            return;
        }
        initialSceneLoaded = true; // If Initial is not loaded, set the Loaded boolean to true

        //
        SceneManager.LoadScene(mainSceneName); 
        gameObject.SetActive(false);
    }
}
