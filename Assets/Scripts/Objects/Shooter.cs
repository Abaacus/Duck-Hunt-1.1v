using System.Collections;
using UnityEngine;

public class Shooter : MonoBehaviour    // this script handles the scope that the player controlls
{
    #region Singleton
    public static Shooter instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Multiple instances of " + this + "found");
        }

        instance = this;
    }
    #endregion

    AudioManager audioManager;  // more singletons

    Camera cam; // the camera is used to make the gameObject follow the player's mouse. Why? look for the camera below, line 56
    Collider2D shootCollider;   // the collided that the shooter will use to 'hit' ducks
    SpriteRenderer spr;
    
    [HideInInspector]
    public Hitbox lastHit;  // saves what the shooter last hit (so when the shooter hits something, other classes can easily access information about what it hit)

    public Vector2 boundSize;   // the sideLengths for the box that is the bounds
    public Vector2 boundOffset; // and the center point
    Boundary bounds;    // the boundary for where the player is allowed to shoot (can't have them wasting bullet by clicking off screen)

    public float shotDelay; // time needed in between shots
    float shotDelayTime; // time since last shot
    bool active;    // is shooting enabled?

    private void Start()
    {
        audioManager = AudioManager.instance;   // set up singleton

        cam = FindObjectOfType<Camera>();
        shootCollider = GetComponent<Collider2D>(); // intialilize important game components
        spr = GetComponent<SpriteRenderer>();

        bounds = new Boundary(boundSize, boundOffset);  // calculate the boundary
    }

    private void FixedUpdate()  // runs every fixedUpdate, to make for smoother shifts
    {
        MoveAim();  // moves the scope to the mouse position
    }

    void MoveAim()  // this makes the scope move the player's cursor
    {   // this function inputs the mouse position on the screen,  and the camera finds where that would be in the game world
        Vector3 newPos = cam.ScreenToWorldPoint(Input.mousePosition);    // (see why the camera is important)
        newPos.z = transform.position.z;    // maintain the z pos (the depth) of the shooter
        transform.position = newPos;    // set the scope to the new position calculated
    }

    public bool Shoot() // check for collisions with the scope, saves info about what it hit, and return whether or not the player actually shot
    {
        if (active && IsInBounds() && shotDelayTime < 0)
        {   // if the shooter is enabled, it's in the shooting boundary, and the delay has been waited, the player can shoot
            if (Input.GetButtonDown("Fire1"))  // if the player can shoot, did they actually press the fire button? (the LMB)
            {
                RaycastHit2D[] hits = new RaycastHit2D[1];  // if the player did shoot, cast the shooter's 
                shootCollider.Cast(Vector3.forward, hits);  // collider into the scene and store the 
                RaycastHit2D hit = hits[0];                 // collider hit in the 'hit' variable

                if (hit.transform != null && hit.transform.TryGetComponent(out Hitbox hitbox))  // if the hit isn't null, and there's a hitbox attached to what was hit
                {
                    Debug.Log("Hit " + hit.transform);  // debug what we hit
                    audioManager.PlaySound("blast");    // play the blast and quack sounds
                    audioManager.PlaySound("duck");
                    hitbox.hit = true;  // set the hitbox as hit

                    lastHit = hitbox;   // save the info about that hitbox
                }
                else    // otherwise, if nothing was hit
                {
                    Debug.Log("Hit Nothing");   // debug that
                    audioManager.PlaySound("miss"); // play the miss sound
                    lastHit = null; // set last hit to nothing
                }

                shotDelayTime = shotDelay; 
                return true;     // the shoot was shot, so reset it's cooldown & return true
            }
        }
        // if the player didn't fire, or couldn't fire,
        shotDelayTime -= Time.deltaTime;    // remove time before next shot
        lastHit = null; // clear last hit
        return false;   // return false (wasn't shot)
    }

    bool IsInBounds()   // returns whether or not the scope is in the shooting bounds (and if the player should be able to shoot)
    {
        if (transform.position.x > bounds.xMin && transform.position.x < bounds.xMax && transform.position.y > bounds.yMin && transform.position.y < bounds.yMax)
        {   // ho boy this is long. if the shoot meets all of these requirements, return true
            return true;
        }

        return false;   // otherwise, the player isn't in bounds, and the player shouldn't shoot
    }

    public void Activate()  // activates the shooter for player use
    {
        spr.color = Color.white;    // make the scope visible,
        active = true;  // and allow the player to shoot
    }

    public void Deactivate()    // stops the player from shooting, and hides the scope
    {
        spr.color = Color.clear;    // make the scope invisble...
        active = false; // and remove the player's ability to shoot
    }

    private void OnDrawGizmosSelected()
    {   // debug draw the boundary of where the player can shoot
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(boundOffset, boundSize);
    }
}
