using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup : MonoBehaviour  // this script is attached to the little popups that appear, such as when you shoot the duck and the points appear
{
    public float waitTime;  // time the popup waits before appearing
    public float lifeTime;  // length of time the popup is visible for
    float timer;   // how long has the popup been at the current stage?
    bool visible;   // is the popup visible?

    private void Start()
    {
        GetComponent<SpriteRenderer>().color = Color.clear; // set the popup to be invisble
        visible = false;
    }

    private void Update()
    {
        if (visible)    // if the popup is visible
        {
            if (timer >= lifeTime)
            {   // if the timer has passed the life limit,
                Destroy(gameObject);    // destroy this popup
            }
        }
        else    // if it's still invisble
        {
            if (timer >= waitTime)
            {   // if the timer has passed it's wait time
                GetComponent<SpriteRenderer>().color = Color.white; // makes the popup visible
                timer = 0; // resets the timer
                visible = true;
            }
        }

        timer += Time.deltaTime;    // increase the timer by the amount of time that passes between the last two frames
    }
}
