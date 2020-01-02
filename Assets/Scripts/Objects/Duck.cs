using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DuckState { alive, hit, falling, dead };    // different states the duck can be in (and different code to run!)

public class Duck : MonoBehaviour   // this monobehaviour manages what the duck should do at what state,
{
    AudioManager audioManager;  // accessor to the audioManager and duckBoundary
    DuckBoundary duckBoundary;

    Animator animator;  // animator for the duck Animations
    Rigidbody2D rb; // physics component for the duck
    Hitbox hitbox;  // the duck hitbox (handles the duck being shot and such)

    public DuckState duckState; // holds what state the duck is in

    public float minSpeed;  // minimum and maximum speed the duck will travel
    public float maxSpeed;
    public static Vector2[] directions = { new Vector2(1, 1), new Vector2(1, -1), new Vector2(-1, -1), new Vector2(-1, 1) };    // the different ordinal directions the duck can travel in (NE, SE, SW, NW)

    float speed;    // the ducks current speed
    Vector2 direction;  // the normalized vector of direction the duck is flying

    public float fallSpeed; // how fast the duck should fall
    public Sound fallSound; // what sound the duck should play while falling
    bool firstFall; // is this the first loop where the duck is falling?

    void Start()
    {
        audioManager = AudioManager.instance;   // set up singletons
        duckBoundary = DuckBoundary.instance;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();   // access the various components
        hitbox = GetComponent<Hitbox>();

        fallSound.InitializeAudioSource(gameObject.AddComponent<AudioSource>());    // initialize the fall sound

        speed = Random.Range(minSpeed, maxSpeed);   // generate a random speed for the duck
        direction = directions[Random.Range(0, directions.Length)]; // set the duck's direction to one of the 4 defined above

        firstFall = true;   // the duck hasn't fallen yet, so the next fall will certainly be it's first

        if (direction.x < 0)
        {   // the animator defaults to the animation "Fly1", which faces right. if the x direction is negative, 
            animator.Play("Fly2");  // the duck will be flying left, so we have to play "FLy2", an animation that faces left
        }

        rb.velocity = direction * speed;    // set the physics velocity to the duck's directional vector multiplied by the duck's speed

        duckState = DuckState.alive;    // make sure the duck is alive
    }

    void Update()
    {   // each frame, run something different depending on what state the duck is in
        switch (duckState)
        {
            default:
            case DuckState.alive:
                Fly();
                break;

            case DuckState.hit:
                Hit();
                break;

            case DuckState.falling:
                Fall();
                break;

            case DuckState.dead:
                Dead();
                break;
        }

        CheckEdge();    // check to see if the duck has reached the edges of the duck boundary, and if it's direction needs to change
    }

    void Fly()  // runs while the duck is flying around and hasn't been shot
    {
        if (hitbox.hit) // if the hitbox has been hit...
        {
            Hit();  // run the duck hit function
        }
    }

    void Hit()  // code for when the duck is shot
    {
        Destroy(GetComponent<Collider2D>());    // destroy it's collider (so it can't be shot more than once)
        animator.Play("Hit");   // the hit animation
        rb.velocity = Vector2.zero; // stop the duck from moving (sets it's velocity to (0,0) )
    }

    void Fall() // this code handles the duck falling *** NOTE: The duck transfer to this stage after the hit animation has played, something that's handled in the animator ***
    {
        if (firstFall)
        {   // if this is the start of the fall, start playing the fall sound...
            fallSound.source.Play();
            firstFall = false;  // and make sure the duck doesn't run this again
        }

        rb.velocity = Vector2.down * fallSpeed; // set the duck's velocity to a vector pointing down multiplied by the duck fallSpeed
    }

    void Dead() // this code deactivates the duck and preps it for DESTRUCTION in the duckManager
    {
        audioManager.PlaySound("drop"); // plays the duck dead noise...
        gameObject.SetActive(false);    // and deactivates the duck's gameObject
    }

    void CheckEdge()    // this code checks if the duck has gone out of the duckBoundary, and what should happen when it does
    {
        switch (duckState)
        {
            case DuckState.alive:   // if the duck is alive, the code will ake sure it bounces around the duckBoundary

                if (Bounce())   // if the duck bounced, we need to run code to update it's velocity and animations
                {   // if it didn't bounce, it would be a waste of resources to run this code
                    rb.velocity = direction * speed;    // and, set it's velocity to the new direction multiplied by the duck's speed

                    if (direction.x > 0)
                    {   // if the direction is positive, make it face right
                        animator.Play("Fly1");
                    }
                    else
                    {   // otherwise, make it face left
                        animator.Play("Fly2");
                    }
                }

                break;

            case DuckState.falling: // if the duck is falling, the code will kill it when it reaches the bottom of the duckBoundary
                if (transform.position.y < duckBoundary.boundary.yMin)
                {   // if the duck's y is less than the boundary's minimum y,
                    duckState = DuckState.dead; // set it as dead,
                    Dead(); // and kill it
                }
                break;
        }
    }

    bool Bounce()   // this code notifies the edge check if the duck bounced, and updates the duck's direction if the duck needs to bounce
    {
        bool directionChanged = false;  // will return whether or not the duck changed direction

        if (transform.position.y < duckBoundary.boundary.yMin)
        {   // if the duck is too low down, sets the direction to be upwards
            direction.y = 1;
            directionChanged = true;
        }

        if (transform.position.y > duckBoundary.boundary.yMax)
        {   // if the duck is too high, set the direction to be downwards
            direction.y = -1;
            directionChanged = true;
        }

        if (transform.position.x < duckBoundary.boundary.xMin)
        {   // if the duck is too far left, sets the direction to be right
            direction.x = 1;
            directionChanged = true;
        }

        if (transform.position.x > duckBoundary.boundary.xMax)
        {   // and if the duck is too far right, set the direction to be left
            direction.x = -1;
            directionChanged = true;
        }

        return directionChanged;    // return whether or not the direction changed, and if the velocity needs to be updated
    }
}
