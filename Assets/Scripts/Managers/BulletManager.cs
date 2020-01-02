using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletManager : MonoBehaviour  // keeps ntrack of how many bullets the player has, and shares that information with the player
{
    #region Singleton
    public static BulletManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Multiple instances of " + this + "found");
        }

        instance = this;
    }
    #endregion

    TimeManager timeManager;    // accessor for timeManager (accesses the nightTint for the bullets)

    public int shotsLeft;   // number of bullets left before gameOver
    public float breakTime; // duration that the bullet is broken before fully dissapearing
    public Sprite[] bulletSprites = new Sprite[2];  // two bullet images, a whole bullet and a broken one
    public Image[] bulletGUIS;  // array of bulletGUIS, the images used to represent how many bullets the player has
    public bool outOfBullets;   // is the player out of bullets

    private void Start()
    {
        timeManager = TimeManager.instance; // access the timeManager
    }

    public void ResetBulletAmount(int newBullets)
    {   // resets the number of bullets the the interher inputted
        Debug.Log("Refilled bullets to " + newBullets);
        shotsLeft = newBullets; // refill the bullets
        UpdateBulletGUI();    // update the bullets to reflect how many the player has

        outOfBullets = false;   // it can't be out of bullets, it just refilled
    }

    public void UseBullet()
    {
        UpdateBulletGUI();  // updates the bullets, makes sure the right bullets are being
        shotsLeft--;
        StartCoroutine(BreakBullet(shotsLeft)); // breaks the very last bullet in the array
        Debug.Log("Bullets left: " + shotsLeft);
        CheckBulletAmount();    // checks to see if the player is out of bullets and if the game should end
    }

    void UpdateBulletGUI()
    {
        for (int i = 0; i < bulletGUIS.Length; i++)
        {   // looping through every bulletGUI, should that bulletGUI be shown? is it less than or equal to the number of shots left?
            if (i + 1 <= shotsLeft)
            {
                if (timeManager.isNight)
                {   // if it is, set it to the uiNight color if it's night
                    bulletGUIS[i].color = timeManager.uiNightTint;
                }
                else
                {   // or just plain colors if it's day
                    bulletGUIS[i].color = Color.white;
                }
            }
            else
            {   // if we shouldn't display it, make it invisble
                bulletGUIS[i].color = Color.clear;
            }
        }
    }

    void CheckBulletAmount()
    {
        if (shotsLeft <= 0)
        {   // if the player has 0 or less bullets (some how), declare that it's out of bullets
            outOfBullets = true;
        }
    }

    IEnumerator BreakBullet(int bulletIndex)
    {
        //Debug.Log("Breaking Bullet: " + bulletIndex);
        bulletGUIS[bulletIndex].sprite = bulletSprites[1];  // sets the inputted bullet index to broken bullet image
        yield return new WaitForSecondsRealtime(breakTime); // waits to display the broken bullet for a bit

        bulletGUIS[bulletIndex].color = Color.clear;    // hides the bullet and resets it sprite
        bulletGUIS[bulletIndex].sprite = bulletSprites[0];
    }
}
