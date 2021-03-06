﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckManager : MonoBehaviour    // keeps track of the ducks flying around
{
    #region Singleton
    public static DuckManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Multiple instances of " + this + "found");
        }

        instance = this;
    }
    #endregion

    BulletManager bulletManager;    // refers to bullets to check if the player has run out of bullets
    GameManager gameManager;    // gets info from the gameManager and notifies it when the player has lost
    public GameObject duckGPrefab;  // prefab for green ducks
    public GameObject duckPPrefab;  // prefab for purple ducks
    List<Duck> ducks = new List<Duck>();    // list of living ducks

    Boundary spawnBoundary; // boundary for where the ducks spawn
    public Vector2 spawnOffset; // offset for the boundary
    public Vector2 spawnSize;   // x and y scale for the boundary


    public void Start()
    {
        bulletManager = BulletManager.instance;
        gameManager = GameManager.instance; // access singletosns
        spawnBoundary = new Boundary(spawnSize, spawnOffset);   // calculate a new boundary
    }

    public IEnumerator SpawnDucks(Wave wave)    // creates new ducks at the start of a wave, and slowly removes them over time
    {
        for (int i = 0; i < wave.GDucks; i++)   // for the number of green ducks to instantiate...
        {
            Vector3 newPos = new Vector3(Random.Range(spawnBoundary.xMin, spawnBoundary.xMax), Random.Range(spawnBoundary.yMin, spawnBoundary.yMax), transform.position.z); // generates a random posistion within the boundary,
            ducks.Add(Instantiate(duckGPrefab, newPos, Quaternion.identity).GetComponent<Duck>());  // and spawns a new green duck there, keeping track of it in the ducks list
        }

        for (int i = 0; i < wave.PDucks; i++)   // the exact same proccess as green ducks, but for purple ones
        {
            Vector3 newPos = new Vector3(Random.Range(spawnBoundary.xMin, spawnBoundary.xMax), Random.Range(spawnBoundary.yMin, spawnBoundary.yMax), transform.position.z);
            ducks.Add(Instantiate(duckPPrefab, newPos, Quaternion.identity).GetComponent<Duck>());
        }

        while (ducks.Count != 0)    // while there are still ducks alive, we check to see if the player has won, or lost
        {
            foreach (Duck duck in ducks)    // loops through the every duck in the duck list
            {
                if (duck.duckState == DuckState.dead)
                {   // if the duck is dead,
                    Destroy(duck.gameObject);    // destroy the duck gameObject,
                    ducks.Remove(duck); // and removes the duck from the duck list
                    break;  // because we modified the list, we need to break the foreach loop 
                }   // (two ducks can't die in the same frame anyways, so no problems will be created)
            }

            if (bulletManager.outOfBullets) // if the player is out of bullets, they might have lost the game
            {
                foreach (Duck duck in ducks)    // we loop through all remaining ducks to see if they have
                {   // already killed every duck, and are just waiting for them to hit the ground
                    if (duck.duckState == DuckState.alive)
                    {   // if we find a living duck, then the player has lost, as there is no way the player can kill that duck
                        gameManager.GameLost(); // notify the gameManager that the player has lost, and break the loop
                        break;
                    }
                }
            }

            yield return new WaitForEndOfFrame();
        }

        gameManager.StartCoroutine(gameManager.EndWave());  // if there are no ducks left, end the current wave
    }

    public void RemoveAllDucks()    // this function kills all ducks and clears the duck array, it's used at the end of the game to clear the screen before returning to the menu
    {
        StopAllCoroutines(); // first, if there are any spawnDuck coroutines running, stop them

        while (ducks.Count > 0) // then, while there are still ducks in the ducks list...
        {
            Destroy(ducks[0].gameObject);   // destroy the first index's gameObject...
            ducks.Remove(ducks[0]); // then remove the first index in the list
        }   // when it breaks the while loop, there should be no ducks left
    }

    private void OnDrawGizmosSelected() // this is a little debug function that unity calls automatically to draw lines and such in the scene editor
    {   // this particular code draws the area where ducks can spawn
        Gizmos.color = Color.yellow;    // it draws a yellow square...
        Gizmos.DrawWireCube(spawnOffset, spawnSize);    // at the spawn offset, with a scale of the spawn size
    }
}
