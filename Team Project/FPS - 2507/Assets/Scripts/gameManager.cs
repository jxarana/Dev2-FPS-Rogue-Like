using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuUnlocks;
    public  TMP_Text goldCount;
    public  TMP_Text unlockCount;

    public TMP_Text inMagCount;
    public TMP_Text currAmmoCount;




    public GameObject menuShop;

    public Image ammoBar;
    public Image playerHPBar;
    public GameObject playerDamagePanel;

    public bool isPaused;
    public GameObject player;
    public playerController playerScript;

    float timeScaleOrig;

    int gameGoalCount;
    int levelCount;
    

    // Spawn point for the player
    public Transform playerSpawnPoint;
    public GameObject playerPrefab;


    // Randomized spawn for the enemy
    public GameObject enemyPrefab;
    public int numberOfEnemiesToSpawn = 5;
    public List<Transform> enemySpawnPoints;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;

        player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            if (playerPrefab != null && playerSpawnPoint != null)
            {
                player = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
                player.tag = "Player";
            }
        }
        if (player != null)
        {
            playerScript = player.GetComponent<playerController>();
            timeScaleOrig = Time.timeScale;
        }
    }

    private void Start()
    {
        SpawnEnemies();

        gameGoalCount = numberOfEnemiesToSpawn;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if (menuActive == menuPause)
            {
                stateUnpause();
            }
        }
    }

    void SpawnEnemies()
    {
        if (enemyPrefab == null)
        {
            return;
        }

        if (enemySpawnPoints == null || enemySpawnPoints.Count == 0)
        {
            return;
        }

        List<Transform> availableSpawnPoints = new List<Transform>(enemySpawnPoints);

        for (int i = 0; i < numberOfEnemiesToSpawn; i++)
        {
            if (availableSpawnPoints.Count == 0)
            {
                availableSpawnPoints = new List<Transform>(enemySpawnPoints);

                if (availableSpawnPoints.Count == 0)
                {
                    break;
                }
            }


            int randomIndex = Random.Range(0, availableSpawnPoints.Count);
            Transform spawnLocation = availableSpawnPoints[randomIndex];

            GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnLocation.position, spawnLocation.rotation);

            availableSpawnPoints.RemoveAt(randomIndex);
        }
    }

    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    public void updateGameGoal(int amount)
    {
        gameGoalCount += amount;

        if (gameGoalCount <= 0)
        {
            // you win!
            statePause();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }
    }

    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void openShop()
    {
        statePause();
        menuActive = menuShop;
        menuActive.SetActive(true);
    }

}
