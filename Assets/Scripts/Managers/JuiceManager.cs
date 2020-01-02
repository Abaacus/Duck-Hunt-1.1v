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

    public void SpawnPointPopup(Hitbox hitbox)  // this func takes in a hitbox
    {
        if (hitbox.pointValue == 500)
        {
            Instantiate(fiveOOPopup, hitbox.transform.position, Quaternion.identity);
        }

        else if (hitbox.pointValue == 1000)
        {
            Instantiate(oneOOOPopup, hitbox.transform.position, Quaternion.identity);
        }

        else
        {
            Debug.LogWarning("No popup found for point value of: " + hitbox.pointValue);
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
