using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JuiceManager : MonoBehaviour
{
    #region Singleton
    public static JuiceManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Multiple instances of " + this + "found");
        }

        instance = this;
    }
    #endregion

    public GameObject fiveOOPopup;  // gameObject for the 500 point popup
    public GameObject oneOOOPopup;  // gameObject for the 1000 point popup

    public Image flashRenderer; // image for the screen flash
    public float flashDuration;

    private void Start()
    {
        flashRenderer.color = Color.clear;  // make the flash invisible.
        flashRenderer.gameObject.SetActive(false);  // and inactive
    }

    public void FlashScreen()   // this function is called by other scripts to flash the screen
    {
        StartCoroutine(FlashScreen(flashDuration)); // starts the coroutine with a duration of flashDuration
    }

    public void SpawnPointPopup(Hitbox hitbox)  // this func takes in a hitbox and spawns in an appropriate popup for it's point value
    {
        if (hitbox.value == 500)
        {   // if the hitbox has a value of 500, spawn the 500 popup
            Instantiate(fiveOOPopup, hitbox.transform.position, Quaternion.identity);
        }

        else if (hitbox.value == 1000)
        {   // if the hitbox has a value of 1000, spawn the 1000 popup
            Instantiate(oneOOOPopup, hitbox.transform.position, Quaternion.identity);
        }

        else
        {   // if the hitbox didn't have any of those values, debug a warning
            Debug.LogWarning("No popup found for point value of: " + hitbox.value);
        }
    }

    IEnumerator FlashScreen(float duration) // flashes the screen for the inputter amount of time
    {
        flashRenderer.gameObject.SetActive(true);   // activates the flash

        flashRenderer.color = Color.white;  // sets it's colour to white for a tenth of the duration,
        yield return new WaitForSecondsRealtime(duration / 10);
        flashRenderer.color = Color.black;  // then black for the rest
        yield return new WaitForSecondsRealtime(9 * duration / 10);

        flashRenderer.color = Color.clear;  // then it clears the image colour,
        flashRenderer.gameObject.SetActive(false);  // and deactivates it
    }
}
