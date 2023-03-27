using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;

    Resolution[] resolutions;

    public TMPro.TMP_Text VolText;

    public TMPro.TMP_Dropdown resolutionDropdown;

    void Start()
    {
        // Grab all resolutions the player can run the game in
        resolutions = Screen.resolutions;

        // Clear placeholder options
        resolutionDropdown.ClearOptions();

        int currentResolutionIndex = 0;

        // Convert the array of resolutions to a list of strings
        List<string> options = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height + " @ " + resolutions[i].refreshRate + "hz";
            options.Add(option);

            // Get the index of the player's actual resolution within the list of resolutions
            if(resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        // Add the resolutions list to the dropdown menu
        resolutionDropdown.AddOptions(options);

        // Set the dropdown menu's default value to the player's current resolution
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    // Called whenever the Master Volume slider changes value
    public void SetVolume(float volume)
    {
        // Set the volume of the mixer to the slider value
        audioMixer.SetFloat("Volume", volume);
        VolText.text = ((int)(((volume + 80) / 80) * 100)).ToString("000");
    }

    // Called whenever the Fullscreen toggle is clicked
    public void SetFullscreen(bool isFullscreen)
    {
        // Set fullscreen to true/false based on the value of the toggle
        Screen.fullScreen = isFullscreen;
    }

    // Called whenever the dropdown menu changes value
    public void SetResolution(int resolutionIndex)
    {
        // Set the screen to the chosen resolution
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}
