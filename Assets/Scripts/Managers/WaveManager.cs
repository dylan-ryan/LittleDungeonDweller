using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;  // For managing the list of active enemies
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class WaveManager : MonoBehaviour
{
    // Prefabs for each enemy type
    public GameObject wolfPrefab;
    public GameObject goblinPrefab;
    public GameObject ogrePrefab;
    private GameManager gameManager;
    private LevelManager levelManager;

    // Reference to the player GameObject and its Transform
    private GameObject player;
    private Transform playerTransform;

    // Reference to progress bar and wave counter
    private Slider progressBar;
    private TextMeshProUGUI waveCounter;
    private RectTransform arrowUI;

    // Radius around the player where enemies can spawn
    public float spawnRadius = 10f;
    // Distance used to sample the NavMesh for valid spawn positions
    public float navMeshSampleDistance = 10f;

    // Maximum number of attempts to find a valid spawn position
    private const int maxSpawnAttempts = 5;

    // Timing variables for the wave system
    private int waveCount = 1;         // Current wave count
    private int enemiesInWave;         // Total enemies in current wave

    // List to track all active enemies
    private List<GameObject> activeEnemies = new List<GameObject>();

    private void Awake()
    {
        levelManager = FindAnyObjectByType<LevelManager>();
        // Find the player object in the scene and store its Transform
        gameManager = FindAnyObjectByType<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.transform;
        // Find the slider and wave counter
        progressBar = GameObject.Find("ProgressBar")?.GetComponent<Slider>();
        waveCounter = GameObject.Find("WaveCounter")?.GetComponent<TextMeshProUGUI>();
        arrowUI = GameObject.Find("ArrowUI")?.GetComponent<RectTransform>();
    }

    void Start()
    {
        // Start spawning waves after all previous enemies are dead
        StartCoroutine(SpawnWaves());
    }
    void Update()
    {
        //if (activeEnemies.Count > 0)
            //PointArrowToNearestEnemy();
    }

    // Coroutine to manage spawning waves of enemies
    IEnumerator SpawnWaves()
    {
        // Continue spawning waves as long as the game is running
        while (true)
        {
            // Check for victory condition at wave 20
            if (waveCount >= 21)
            {
                TriggerVictory();
                yield break;
            }

            // Spawn the next wave
            SpawnWave();

            // Set up the UI for the current wave
            waveCounter.text = "Wave: " + waveCount + " / 20";
            progressBar.maxValue = enemiesInWave;
            progressBar.value = enemiesInWave;

            // Wait until all enemies in the current wave are killed
            yield return new WaitUntil(() => activeEnemies.Count == 0);
            yield return new WaitForSeconds(5f);

            // Move to the next wave
            waveCount++;
        }
    }

    // Method to trigger victory UI
    void TriggerVictory()
    {
        // Assuming you have a method in your UIManager to display Victory screen
        UIManager.manager.ButtonSwitchScreen("Victory");
        Debug.Log("Victory! You've reached wave 20.");
    }

    // Method to spawn a wave of enemies
    void SpawnWave()
    {
        // Clear any remaining enemies from previous waves
        activeEnemies.Clear();

        // Determine how many of each enemy to spawn based on the wave count
        int wolfCount = Mathf.Min(waveCount);           // Wolves
        int goblinCount = Mathf.Max(0, waveCount - 5);     // Goblins increase after wave 5
        int ogreCount = Mathf.Max(0, waveCount - 10);       // Ogres increase after wave 10

        // Calculate total enemies in this wave
        enemiesInWave = wolfCount + goblinCount + ogreCount;

        // Spawn the enemies of each type
        SpawnEnemies(wolfPrefab, wolfCount);
        SpawnEnemies(goblinPrefab, goblinCount);
        SpawnEnemies(ogrePrefab, ogreCount);

        player.GetComponent<HealthSystem>().health = levelManager.starterHealth + gameManager.health;
    }


    // Method to spawn a specified number of a given enemy prefab
    void SpawnEnemies(GameObject enemyPrefab, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPosition;
            // Try to find a valid spawn position for the enemy
            if (TryGetNavMeshSpawnPosition(out spawnPosition))
            {
                GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity); // Spawn the enemy at the valid position
                activeEnemies.Add(enemy); // Add the enemy to the active list for tracking
            }
            else
            {
                Debug.LogWarning("Failed to find a NavMesh position for enemy"); // Log a warning if no valid position was found
            }
        }
    }

    // Method to attempt to get a valid NavMesh spawn position
    bool TryGetNavMeshSpawnPosition(out Vector3 spawnPosition)
    {
        spawnPosition = Vector3.zero; // Default to zero position if not found

        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            Vector3 randomPosition = GetRandomSpawnPosition(); // Get a random position around the player
            // Check if the random position is on the NavMesh
            if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, navMeshSampleDistance, NavMesh.AllAreas))
            {
                spawnPosition = hit.position; // Use the valid hit position
                return true; // Return success
            }
        }
        return false; // Return failure after max attempts
    }

    // Method to generate a random spawn position around the player
    // Method to generate a random spawn position exactly on the spawn radius around the player
    Vector3 GetRandomSpawnPosition()
    {
        float angle = Random.Range(0, 2 * Mathf.PI); // Random angle for circular spawning
        float distance = spawnRadius; // Set the distance to exactly the spawn radius
                                      // Calculate the offset position based on angle and fixed distance
        Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * distance;
        return playerTransform.position + offset; // Return the final spawn position
    }


    // Call this method when an enemy dies
    public void OnEnemyDeath(GameObject enemy)
    {
        // Remove the enemy from the active list
        activeEnemies.Remove(enemy);
        progressBar.value = activeEnemies.Count;
        gameManager.enemiesKilledLastRun++;
    }
    /*private void PointArrowToNearestEnemy()
    {
        if (arrowUI == null || Camera.main == null || playerTransform == null) return;

        // Find the nearest enemy
        GameObject nearestEnemy = activeEnemies
            .Where(enemy => enemy != null)
            .OrderBy(enemy => Vector3.Distance(playerTransform.position, enemy.transform.position))
            .FirstOrDefault();

        if (nearestEnemy == null)
        {
            arrowUI.gameObject.SetActive(false);
            return;
        }

        // Calculate direction from the player to the enemy in world space
        Vector3 directionToEnemy = nearestEnemy.transform.position - playerTransform.position;

        // Handle cases where the enemy is behind the player
        Vector3 cameraForward = Camera.main.transform.forward;
        if (Vector3.Dot(cameraForward, directionToEnemy) < 0)
        {
            directionToEnemy = -directionToEnemy; // Flip the direction
        }

        // Convert the direction to screen space
        Vector3 enemyScreenPos = Camera.main.WorldToScreenPoint(playerTransform.position + directionToEnemy);
        bool isOnScreen = enemyScreenPos.z > 0 &&
                          enemyScreenPos.x > 0 && enemyScreenPos.x < Screen.width &&
                          enemyScreenPos.y > 0 && enemyScreenPos.y < Screen.height;

        if (isOnScreen)
        {
            arrowUI.gameObject.SetActive(false);
            return;
        }

        // Show the arrow and position it at the screen edge
        arrowUI.gameObject.SetActive(true);
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Vector3 screenDirection = (enemyScreenPos - screenCenter).normalized;

        float edgeDistance = Mathf.Min(Screen.width, Screen.height) / 2 - 50; // Adjust for padding
        Vector3 edgePos = screenCenter + screenDirection * edgeDistance;
        edgePos.x = Mathf.Clamp(edgePos.x, 0, Screen.width);
        edgePos.y = Mathf.Clamp(edgePos.y, 0, Screen.height);

        arrowUI.position = edgePos;
        arrowUI.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(screenDirection.y, screenDirection.x) * Mathf.Rad2Deg);
    }*/




}
