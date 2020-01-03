using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Multiple instances of " + this + "found");
        }

        instance = this;
    }
    #endregion

    AudioManager audioManager;
    BulletManager bulletManager;
    DuckManager duckManager;    // accessors to every manager in the game...
    JuiceManager juiceManager;
    TimeManager timeManager;

    Announcer announcer;
    Shooter shooter;    // and objects too!
    Dog dog;

    public GameObject[] menuPanels;     // gameObjects of the ui visible in the menu
    public GameObject[] gamePanels;     // gameObjects of the ui visible in-game
    public TextMeshProUGUI scoreGUI;    // GUI of the score

    public Round[] rounds;  // array of the game rounds (index through these)
    int roundIndex; // current round index
    int waveIndex;  // current wave index
    int score;  // the player's current score
    bool gameRunning;   // is the game running, or in the menu

    void Start()
    {
        audioManager = AudioManager.instance;
        bulletManager = BulletManager.instance;
        duckManager = DuckManager.instance;
        juiceManager = JuiceManager.instance;
        timeManager = TimeManager.instance;     // sets up singletons

        announcer = Announcer.instance;
        shooter = Shooter.instance;
        dog = Dog.instance;

        LoadMenu(); // loads the main menu
    }

    private void Update()
    {
        ShootDucks();
        Cursor.visible = !gameRunning;  // set the cursor to be visible in the menu, or invisible during the game
    }

    void LoadMenu() // loads the mainMenu
    {
        gameRunning = false;    // set the game to not be running
        shooter.Deactivate();   // deactivate the shooter
        audioManager.PlaySound("menu"); // starts playing the menu music
        dog.StartLoopingAnimation("Pacing");    // make start the dog pacing

        foreach (GameObject panel in menuPanels)
        {
            panel.SetActive(true);  // activate all of the menu panels
        }

        foreach (GameObject panel in gamePanels)
        {
            panel.SetActive(false); // deactivate all of the game panels
        }

        StopAllCoroutines();
    }

    public void LoadGame(bool isNightMode)  // this function is connected to the start buttons,
    {                       // the day button inputted false, while the night button inputs true
        StopAllCoroutines();    // stop any game coroutines that might be running in the background
        duckManager.RemoveAllDucks();   // remove any ducks and all ducks
        audioManager.StopSound("menu"); // stop the menu music

        foreach (GameObject panel in menuPanels)
        {
            panel.SetActive(false); // deactivates the menu panels
        }

        foreach (GameObject panel in gamePanels)
        {
            panel.SetActive(true);  // activates the game panels
        }

        if (isNightMode)
        {    // if the player selected nightMode, apply the night colours to the world
            timeManager.DarkenWorld();
        }
        else
        {   // otherwise, apply the day colours
            timeManager.BrightenWorld();
        }

        score = 0;
        roundIndex = 0; // set the score, roundIndex, and waveIndex to 0
        waveIndex = 0;
        UpdateScoreGUI();   // update the scoreGUI (clears the score)
        StartCoroutine(StartGame());    // start the game!
    }

    void ShootDucks()   // handles the different components of shooting, and hitting ducks
    {
        if (bulletManager.shotsLeft > 0)
        {   // if the player still has bullets left
            if (shooter.Shoot())    // check to see if the player is shooting
            {  
                bulletManager.UseBullet();   // if the player shot, use a bullet 

                if (shooter.lastHit != null)    // and check to see if it hit anything
                {
                    IncreaseScore(shooter.lastHit); // increase the score usimg the hitbox that was hit,
                    juiceManager.SpawnPointPopup(shooter.lastHit);  // spawn a point popup,
                    juiceManager.FlashScreen(); // and flash the screen
                }
            }
        }
    }

    void IncreaseScore(Hitbox targetHit)    // increase the score based off the inputted hitbox point value
    {
        score += targetHit.value;

        UpdateScoreGUI();   // updates the score visual
    }


    void UpdateScoreGUI()   // updates the GUI to match the score
    {
        string scoreText = score.ToString("000000");    // creates a string based of the score, padded to be 6 digits long
        scoreGUI.text = scoreText;  // sets the textGUI string to the newly created strings
    }

    void UpdateWaveCounter()    // updates the wave and round counter to play the correct next wave
    {
        waveIndex++;
        if (waveIndex >= rounds[roundIndex].waves.Length)   // if the this round doesn't have a wave for this index,
        {
            Debug.Log("Round complete");
            waveIndex = 0;  // move to the next round
            roundIndex++;

            if (roundIndex >= rounds.Length)    // if there are no more rounds
            {
                Debug.Log("game finished");
                GameWon();  // end this function and return that the player won
                return;
            }

            StartCoroutine(StartRound());   // if there are rounds left, start the new round
            return;
        }

        StartWave();    // if it isn't a new round, start the next wave
    }

    void GameWon()  // declares that the player has won the game
    {
        StopAllCoroutines();    // shorthand for starting this true coroutine
        StartCoroutine(EndGame(true));  
    }

    public void GameLost()  // called when the game is lost
    {
        StopAllCoroutines(); // shorthand for starting this false coroutine
        StartCoroutine(EndGame(false));;
    }

    public IEnumerator EndWave()    // this function plays the end of wave animations and sound, then starts a new wave
    {
        if (gameRunning)    // if the game is running, the proccess can continue
        {
            Debug.Log("Wave complete");
            shooter.Deactivate();   // deactivate the shooter
            yield return new WaitForSecondsRealtime(0.2f);
            dog.PlayAnimation("Happy");
            audioManager.PlaySound("end wave");
            yield return new WaitForSecondsRealtime(1.3f);  // wait for the sound and animation to end
            UpdateWaveCounter();    // starts a new wave, round, or ends the game
        }
    }

    void StartWave()    // this function starts a new wave
    {
        Debug.Log("Starting round " + (roundIndex + 1) + ", wave " + (waveIndex + 1));
        Wave nextWave = rounds[roundIndex].waves[waveIndex];    // sets var to what the next wave is
        bulletManager.ResetBulletAmount(nextWave.bullets);  // refill the bullets to the bullet wave amount
        StartCoroutine(duckManager.SpawnDucks(nextWave));   // spawn ducks with the new wave
        shooter.Activate(); // activate the shooter
    }

    IEnumerator StartRound()    // starts a new game round
    {
        announcer.Message("Round " + (roundIndex + 1), 2f);   // plays a message for 1.5 seconds, announcing the new round
        audioManager.PlaySound("next round");
        dog.PlayAnimation("Jumping");       // plays new round animations and sound, waits until they are done,
        yield return new WaitForSecondsRealtime(1.4f);
        audioManager.PlaySound("bark"); // more sounds
        yield return new WaitForSecondsRealtime(2f);
        StartWave();    // then starts a new wave
    }

    IEnumerator StartGame() // starts a fresh new game
    {
        gameRunning = true;

        dog.StopAllAnimations();    // resets the dog (to stop it from pacing)

        announcer.Message("Round 1", 5.5f);
        audioManager.PlaySound("start round");
        dog.PlayAnimation("Hunting");   // plays a message, animation, and sound
        yield return new WaitForSecondsRealtime(4.8f);
        audioManager.PlaySound("bark"); // more sounds
        yield return new WaitForSecondsRealtime(1.2f);
        StartWave();    // then starts a new wave
    }

    IEnumerator EndGame(bool gameWon)   // ends the game, victoriously if true, or laughibly if false
    {
        shooter.Deactivate();
        if (gameWon)    // if they ended the gam by winning
        {
            yield return new WaitForSecondsRealtime(1f);
            audioManager.PlaySound("win");
            announcer.Message("You Win!", 3.8f);
            dog.PlayAnimation("Win");   // plays congratulating noise, animation, and message
            yield return new WaitForSecondsRealtime(4.6f);
        }
        else
        {
            yield return new WaitForSecondsRealtime(2f);    // play a sad noise and message
            audioManager.PlaySound("lose");
            announcer.Message("Game Over", 5f);
            duckManager.RemoveAllDucks();

            yield return new WaitForSecondsRealtime(1.9f);
            audioManager.PlaySound("laugh");
            yield return new WaitForSecondsRealtime(0.1f);    // have the dog laugh at you
            dog.PlayAnimation("Laughing");
            yield return new WaitForSecondsRealtime(3f);
        }

        LoadMenu(); // load in the main menu
    }
}