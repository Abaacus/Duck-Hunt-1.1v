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
    DuckManager duckManager;
    JuiceManager juiceManager;
    TimeManager timeManager;

    Announcer announcer;
    Shooter shooter;
    Dog dog;

    public GameObject[] menuButtons;
    public GameObject[] uiPanels;
    public TextMeshProUGUI scoreGUI;
    public Round[] rounds;
    int roundIndex;
    int waveIndex;
    int score;
    bool gameRunning;

    void Start()
    {
        audioManager = AudioManager.instance;
        bulletManager = BulletManager.instance;
        duckManager = DuckManager.instance;
        juiceManager = JuiceManager.instance;
        timeManager = TimeManager.instance;

        announcer = Announcer.instance;
        shooter = Shooter.instance;
        dog = Dog.instance;

        LoadMenu();
    }

    private void Update()
    {
        BulletShot();
    }

    void LoadMenu()
    {
        Cursor.visible = true;
        shooter.Deactivate();
        audioManager.PlaySound("menu");
        dog.StartLoopingAnimation("Pacing");

        foreach (GameObject button in menuButtons)
        {
            button.SetActive(true);
        }

        foreach (GameObject uiPanel in uiPanels)
        {
            uiPanel.SetActive(false);
        }

        StopAllCoroutines();
    }

    public void LoadGame(bool isNightMode)
    {
        StopAllCoroutines();
        duckManager.RemoveAllDucks();
        audioManager.StopSound("menu");

        foreach (GameObject button in menuButtons)
        {
            button.SetActive(false);
        }

        foreach (GameObject uiPanel in uiPanels)
        {
            uiPanel.SetActive(true);
        }

        if (isNightMode)
        {
            timeManager.DarkenWorld();
        }
        else
        {
            timeManager.BrightenWorld();
        }

        Cursor.visible = false;

        score = 0;
        roundIndex = 0;
        waveIndex = 0;
        UpdateScoreGUI();
        StartCoroutine(StartGame());
    }

    void BulletShot()
    {
        if (bulletManager.shotsLeft > 0)
        {
            if (shooter.Shoot())
            {
                bulletManager.UseBullet();

                if (shooter.lastHit != null)
                {
                    IncreaseScore(shooter.lastHit);
                    juiceManager.SpawnPointPopup(shooter.lastHit);
                    juiceManager.FlashScreen();
                }
            }
        }
    }

    void IncreaseScore(Hitbox targetHit)
    {
        score += targetHit.pointValue;

        UpdateScoreGUI();
    }


    void UpdateScoreGUI()
    {
        string scoreText = score.ToString("000000");
        scoreGUI.text = scoreText;
    }

    void UpdateWaveCounter()
    {
        waveIndex++;
        if (waveIndex >= rounds[roundIndex].waves.Length)
        {
            Debug.Log("Round complete");
            waveIndex = 0;
            roundIndex++;

            if (roundIndex >= rounds.Length)
            {
                Debug.Log("game finished");
                StartCoroutine(EndGame(true));
                return;
            }

            StartCoroutine(StartRound());
            return;
        }

        StartWave();
    }

    public void GameLost()
    {
        StartCoroutine(EndGame(false));;
    }

    public IEnumerator EndWave()
    {
        if (gameRunning)
        {
            Debug.Log("Wave complete");
            Cursor.visible = false;
            shooter.Deactivate();
            yield return new WaitForSecondsRealtime(0.2f);
            dog.PlayAnimation("Happy");
            audioManager.PlaySound("end wave");
            yield return new WaitForSecondsRealtime(1.3f);
            UpdateWaveCounter();
        }
    }

    void StartWave()
    {
        Debug.Log("Starting round " + (roundIndex + 1) + ", wave " + (waveIndex + 1));
        Wave nextWave = rounds[roundIndex].waves[waveIndex];
        bulletManager.ResetBulletAmount(nextWave.bullets);
        StartCoroutine(duckManager.SpawnDucks(nextWave));
        shooter.Activate();
    }

    IEnumerator StartGame()
    {
        gameRunning = true;

        dog.StopAllAnimations();

        announcer.Message("Round 1", 5.5f);
        audioManager.PlaySound("start round");
        dog.PlayAnimation("Hunting");
        yield return new WaitForSecondsRealtime(4.8f);
        audioManager.PlaySound("bark");
        yield return new WaitForSecondsRealtime(1.2f);
        StartWave();
    }

    IEnumerator StartRound()
    {
        announcer.Message("Round " + (roundIndex + 1), 1.5f);
        audioManager.PlaySound("next round");
        dog.PlayAnimation("Jumping");
        yield return new WaitForSecondsRealtime(3f);
        StartWave();
    }

    IEnumerator EndGame(bool gameWon)
    {
        gameRunning = false;
        if (gameWon)
        {
            dog.PlayAnimation("Win");
            audioManager.PlaySound("win");
            yield return new WaitForSecondsRealtime(5f);
        }
        else
        {
            audioManager.PlaySound("lose");
            yield return new WaitForSecondsRealtime(2f);
            announcer.Message("Game Over");
            duckManager.RemoveAllDucks();

            yield return new WaitForSecondsRealtime(1.1f);
            audioManager.PlaySound("laugh");
            yield return new WaitForSecondsRealtime(2.1f);
            dog.PlayAnimation("Laughing"); 
            yield return new WaitForSecondsRealtime(2.9f);
        }

        LoadMenu();
    }
}