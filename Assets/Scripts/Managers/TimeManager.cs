using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeManager : MonoBehaviour
{
    #region Singleton
    public static TimeManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Multiple instances of " + this + "found");
        }

        instance = this;
    }
    #endregion

    public Color nightTint; // color of the night

    [HideInInspector]
    public Color uiNightTint;   // color of the night for UI elements (needs to be modified slightly)

    public GameObject[] uiPanels;   // array of all uiPanels (that will be affected by color changes)
    public GameObject darkLayer;    // layer of darkness that overlaps the scene, making the gameObjects have a dark tint

    public bool isNight;

    Dog dog;    // needs to inform the dog of what time it is, so it needs to be able to access the dog

    private void Start()
    {
        dog = Dog.instance; // setup singleton
        BrightenWorld();    // set default time to day
    }

    public void BrightenWorld() // this function makes everything it's normal colors
    {
        foreach (GameObject uiPanel in uiPanels)
        {   // for every ui panel, the function looks for sprite renderers, images, and text attached to them, and sets their colour to white
            foreach (SpriteRenderer spr in uiPanel.GetComponentsInChildren<SpriteRenderer>())
            {
                spr.color = Color.white;
            }

            foreach (TextMeshProUGUI label in uiPanel.GetComponentsInChildren<TextMeshProUGUI>())
            {
                label.color = Color.white;
            }

            foreach (Image image in uiPanel.GetComponentsInChildren<Image>())
            {
                image.color = Color.white;
            }
        }

        darkLayer.SetActive(false); // it deactivates the darkLayer

        isNight = false;    // and makes sure everthing is aware that it isn't night
        dog.isNight = false;
    }

    public void DarkenWorld()   // this function applies the nightTint to everything, making the scene darker
    {
        uiNightTint = MixColours(nightTint, Color.white);   // calculates the uiNightTint by mixing white and the nightTint together

        foreach (GameObject uiPanel in uiPanels)
        {   // then, for every ui panel, the function looks for sprite renderers, images, and text attached to them, and sets their colour to the newly calculated uiNightTint
            foreach (SpriteRenderer spr in uiPanel.GetComponentsInChildren<SpriteRenderer>())
            {
                spr.color = uiNightTint;
            }

            foreach (TextMeshProUGUI label in uiPanel.GetComponentsInChildren<TextMeshProUGUI>())
            {
                label.color = uiNightTint;
            }

            foreach (Image image in uiPanel.GetComponentsInChildren<Image>())
            {
                image.color = uiNightTint;
            }
        }

        darkLayer.GetComponent<SpriteRenderer>().color = nightTint; // change the color of the darkLayer to the nightTint...
        darkLayer.SetActive(true);  // and activate the darkLayer gameObject

        isNight = true; // set the timeManager to night
        //dog.isNight = true;   // *** NOT IMPLEMENTED *** the dog doesn't have night animations, so this line is skipped
    }

    Color MixColours(Color colourA, Color colourB)  // this func takes two colours and mixes them together
    {
        return new Color
        {
            r = colourA.r + colourB.r / 2,
            g = colourA.g + colourB.g / 2,  // it mixes the colours by averageing each channel, 
            b = colourA.b + colourB.b / 2,  // and then returns the new colour
            a = 1   // it doesn't mix the alpha, because we don't want see through panels it's set to 1
        };
    }
}
