using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Announcer : MonoBehaviour  // this code is the status box that pops up to give the player information
{
    #region Singleton
    public static Announcer instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Multiple instances of " + this + "found");
        }

        instance = this;
    }
    #endregion

    TimeManager timeManager;    // accessor for the timeManager

    public TextMeshProUGUI announcerTextGUI;    // textGUI object on the announcer box
    public Image panelImage;    // background image of the announcer 

    public const float messageDuration = 3f;    // default value the message box appears for

    void Start()
    {
        timeManager = TimeManager.instance; // set up accessor to singleton

        panelImage = GetComponent<Image>(); // sets panelImage to this objects Image component...
        announcerTextGUI = GetComponentInChildren<TextMeshProUGUI>();   // and the textGUI to the textGUI attached to this object

        Message("", 0.01f); // play an empty message for less than a frame (quickly resets the message box)
    }

    void Show() // this code makes the message box appear
    {
        if (timeManager.isNight)
        {   // if it's night, set the two colors to the uiNightTint (explained in the timeManager)
            panelImage.color = timeManager.uiNightTint;
            announcerTextGUI.color = timeManager.uiNightTint;
        }
        else
        {   // otherwise, just set the colors to white
            panelImage.color = Color.white;
            announcerTextGUI.color = Color.white;
        }
    }

    void Hide() // this func hides the message box
    {   // sets both colors to be clear (making them invisible)
        panelImage.color = Color.clear;
        announcerTextGUI.color = Color.clear;
    }

    public void Message(string message, float duration = messageDuration)   // this function is called by other scripts that want to display a message. the duration (if not provided) defaults to 3 seconds
    {
        StopAllCoroutines();    // stops any DisplayMessage coroutine that may be running in the background
        StartCoroutine(DisplayMessage(message, duration));  // runs the display message coroutine, transfering the information over inputted over
    }

    IEnumerator DisplayMessage(string message, float duration)
    {
        announcerTextGUI.text = message;    // sets the textGUI's text to the inputted message
        Show(); // shows the announcer
        yield return new WaitForSecondsRealtime(duration);
        Hide(); // hides the announcer
    }
}
