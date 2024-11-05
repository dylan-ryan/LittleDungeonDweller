using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    // Prefabs for each enemy type
    public GameObject wolfPrefab;
    public GameObject goblinPrefab;
    public GameObject ogrePrefab;

    // Reference to the player GameObject and its Transform
    private GameObject player;
    private Transform playerTransform;

    // Radius around the player where enemies can spawn
    public float spawnRadius = 10f;
    // Distance used to sample the NavMesh for valid spawn positions
    public float navMeshSampleDistance = 10f;

    // Maximum number of attempts to find a valid spawn position
    private const int maxSpawnAttempts = 5;

    // Timing variables for the wave system
    private float startTime;
    private float spawnInterval = 5f; // Time between each wave
    private int waveCount = 1;         // Current wave count

    private void Awake()
    {
        // Find the player object in the scene and store its Transform
        player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.transform;
    }

    void Start()
    {
        // Record the starting time and begin spawning waves
        startTime = Time.time;
        StartCoroutine(SpawnWaves());
    }

    // Coroutine to manage spawning waves of enemies
    IEnumerator SpawnWaves()
    {
        // Continue spawning waves until 5 minutes have passed
        while (Time.time - startTime <= 300f) // 300 seconds long
        {
            SpawnWave();    // Spawn a wave of enemies
            waveCount++;    // Increment the wave count
            yield return new WaitForSeconds(spawnInterval); // Wait for the specified spawn interval
        }
    }

    // Method to spawn a wave of enemies
    void SpawnWave()
    {
        // Determine how many of each enemy to spawn based on the wave count
        int wolfCount = Mathf.Min(waveCount, 5);           // Max 5 wolves
        int goblinCount = Mathf.Max(0, waveCount - 5);     // Goblins increase after wave 5
        int ogreCount = Mathf.Max(0, waveCount - 10);       // Ogres increase after wave 10

        // Spawn the enemies of each type
        SpawnEnemies(wolfPrefab, wolfCount);
        SpawnEnemies(goblinPrefab, goblinCount);
        SpawnEnemies(ogrePrefab, ogreCount);
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
                Instantiate(enemyPrefab, spawnPosition, Quaternion.identity); // Spawn the enemy at the valid position
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
    Vector3 GetRandomSpawnPosition()
    {
        float angle = Random.Range(0, 2 * Mathf.PI); // Random angle for circular spawning
        float distance = Random.Range(5f, spawnRadius); // Random distance within the spawn radius
        // Calculate the offset position based on angle and distance
        Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * distance;
        return playerTransform.position + offset; // Return the final spawn position
    }
}
