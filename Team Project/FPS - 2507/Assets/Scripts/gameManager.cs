using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuLevelComplete;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuUnlocks;
    [SerializeField] TMP_Text gameGoalCountText;

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
    float timeScaleNew;

    int gameGoalCount;
    int levelCount;

    

    // Spawn point for the player
    public Transform playerSpawnPoint;
    public GameObject playerPrefab;


    // Randomized spawn for the enemy
    public GameObject enemyPrefab;
    public int numberOfEnemiesToSpawn = 5;

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

        SpawnEnemies();
    }

    private void Start()
    {
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
            else if (menuActive == menuPause || menuActive == menuShop)
            {
                stateUnpause();
            }
        }
        
    }

    void SpawnEnemies()
    {
        // Exit early if no enemy prefab is assigned
        if (enemyPrefab == null) return;

        // Find all spawn points in the scene tagged "EnemySpawn"
        GameObject[] spawnObjects = GameObject.FindGameObjectsWithTag("EnemySpawn");
        if (spawnObjects.Length == 0) return;

        // Create a pool of available spawn points to randomize selection without
        List<GameObject> availableSpawns = new List<GameObject>(spawnObjects);

        for (int i = 0; i < numberOfEnemiesToSpawn; i++)
        {
            if (availableSpawns.Count == 0)
            {
                // If we've used up all spawn points, reset the pool so spawns can repeat
                availableSpawns = new List<GameObject>(spawnObjects);
            }


            // Select a random spawn point from the available list
            int randomIndex = Random.Range(0, availableSpawns.Count);
            GameObject spawnPoint = availableSpawns[randomIndex];

            // Instantiate the enemy at the chosen spawn location
            Instantiate(enemyPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);

            // Remove this spawn point to avoid duplicate use until reset
            availableSpawns.RemoveAt(randomIndex);

        }
    }

    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        timeScaleNew = Time.timeScale;
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
        gameGoalCountText.text = gameGoalCount.ToString("F0");
        /*
         if(gameGoalCount <= 0 && levelCount == x)

           statePause();
            menuActive = menuWin;
            menuActive.SetActive(true);
         
         
         
         */
        if (gameGoalCount <= 0)
        {
            // you win!
            statePause();
            int currentIndex = SceneManager.GetActiveScene().buildIndex;
            int finalIndex = SceneManager.sceneCountInBuildSettings - 1;

            if (currentIndex >= finalIndex)
            {
                // End of last level (show true win menu)
                menuActive = menuWin;
            }
            else
            {
                // Intermediate level completed (show level complete screen)
                menuActive = menuLevelComplete;
            }

            menuActive.SetActive(true);
        }

    /*
     *  statePause();   
        menuActive = menuUlocks;
        menuActive.setActive(true);




     */



    }

    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void levelComplete()
    {
        statePause();
        menuActive = menuLevelComplete;
        menuActive.SetActive(true);
    }

    public void openShop()
    {
        statePause();
        goldCount.text = playerScript.goldCount.ToString();
        unlockCount.text = playerScript.upgradePoints.ToString();
        menuActive = menuShop;
        menuActive.SetActive(true);
    }

   


}
