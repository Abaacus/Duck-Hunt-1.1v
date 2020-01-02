using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : MonoBehaviour    // scripts for dog (used to play animations)
{
    #region Singleton
    public static Dog instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Multiple instances of " + this + "found");
        }

        instance = this;
    }
    #endregion

    Animator animator;  // the dogs animator. This holds animation clips, which scripts can play by inputting the animation name into animator.Play("");

    public bool isNight;    // is it night? (the dog was supposed to have day and night animations, but I don't have the time to draw them in right now)
    bool canAnimate;    // is the dog allowed to animate? (used in earlier versions of the game, still useful if I need to disable the dog, so it's left implemented)

    private void Start()
    {
        animator = GetComponent<Animator>();    // gets the animator attached to the dog so we can call it's animations
        canAnimate = true;
    }

    string AddAnimationSuffix(ref string animationName) // adds modifies to the inputted string based off of the dog's current state
    {
        if (isNight)    // if it is night, we add the night modifier (playing the appropriate night version of the animation)
        {
            animationName += "_Night";  // *** NOTE *** isNight is always equal to false, as it isn't set to true in the timeManager. This is because
        }                               //  none of the dog's night animations have been implemented, so only the day animations can (and will) be played
        else
        {
            animationName += "_Day";    // otherwise, it will add the day modifier (playing the corresponding day version of the animation)
        }

        return animationName;   // the appropriate modifiers have been made, so return the new string
    }

    public void PlayAnimation(string animationName)    // other scripts call this function to animate the dog, based off what string they inputted. The function adds the appropriate modifers, and then plays the animation
    {
        if (canAnimate) // if the dog has it's animation ability activated...
        {  
            AddAnimationSuffix(ref animationName);  // the script modifies the animationName 
            animator.Play(animationName);   // and plays the animation with the same name as this string
        }
    }

    public void StartLoopingAnimation(string animationName)    // the pacing animation works differently, since it infinetly loops, so I have a coroutine run looping animation it instead of the PlayAnimation function
    {
        if (canAnimate) // most of this is the exact same as PlayAnimation, except for the end
        {
            AddAnimationSuffix(ref animationName);
            StartCoroutine(LoopingAnimation(animationName));    // this is where it's different. instead of calling animator.Play() directly, a coroutine is started to run it repetitively
        }
    }

    public void StopAllAnimations()  // this stops any animation playing and sets the animator into standby
    {
        StopAllCoroutines();    // stops any loop coroutines
        animator.Play("Standby");   // sets the animator in stand by
    }

    IEnumerator LoopingAnimation(string animationName)  // this coroutine continuously loops the inputted animation
    {
        while (true) // while true is true (AKA, forever)
        {
            animator.Play(animationName);   // play the animation, and wait for a number of seconds equal to the length of the animation
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length - animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        }   // repeat! An outside script must be used to stop the coroutine and loop, such as StopLoopingAnimation()
    }
}
