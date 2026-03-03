using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("Attributes")]
    [SerializeField] private float enemiesPerSecond = 0.5f;
    [SerializeField] private float timeBetweenWaves = 2f;

    [Header("Wave Data")]
    [SerializeField] private bool useWaveData = true;
    [SerializeField] private WaveData[] Waves = new WaveData[12];

    [Header("Events")]
    public static UnityEvent onEnemyDestroy = new UnityEvent();

    private int currentWave = 1;
    private float timeSinceLastSpawn;
    private int enemiesAlive;
    private int enemiesLeftToSpawn;
    private bool isSpawning = false;

    private List<GameObject> spawnSequence;
    private int spawnIndex;

    private void Awake()
    {
        onEnemyDestroy.AddListener(EnemyDestroyed);
    }

    private void OnDestroy()
    {
        onEnemyDestroy.RemoveListener(EnemyDestroyed);
    }

    private void Start()
    {
        StartCoroutine(StartWave());
    }

    private void Update()
    {
        if (!isSpawning) return;

        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= (1f / enemiesPerSecond) && enemiesLeftToSpawn > 0)
        {
            SpawnEnemy();
            enemiesLeftToSpawn--;
            timeSinceLastSpawn = 0f;
        }
    }

    private void EnemyDestroyed()
    {
        enemiesAlive--;
        Debug.Log($"Enimies Alive: {enemiesAlive} ---- Enimies Left To Spawn: {enemiesLeftToSpawn}");
        if (enemiesAlive <= 0 && enemiesLeftToSpawn <= 0)
        {
            EndWave();
            currentWave++;
            StartCoroutine(StartWave());
        }
    }

    private IEnumerator StartWave()
    {
        yield return new WaitForSeconds(timeBetweenWaves);
        isSpawning = true;

        enemiesLeftToSpawn = Waves[currentWave - 1].enemies.Sum(item => item.count);
        BuildSpawnSequenceForCurrentWave();
        spawnIndex = 0;
    }

    private void EndWave()
    {
        Debug.Log("tyulihkvkhikvjkvjlbi");
        isSpawning = false;
        timeSinceLastSpawn = 0f;
        spawnSequence = null;
        spawnIndex = 0;
    }

    // create functions to spawn enemies based the current wave using the enemy data
    private void BuildSpawnSequenceForCurrentWave()
    {
        spawnSequence = new List<GameObject>();
        if (useWaveData && currentWave - 1 < Waves.Length && Waves[currentWave - 1] != null)
        {
            WaveData waveData = Waves[currentWave - 1];
            foreach (var enemyData in waveData.enemies)
            {
                for (int i = 0; i < enemyData.count; i++)
                {
                    spawnSequence.Add(enemyData.prefab);
                }
            }
        }

    }

    // Spawn the next enemy in the sequence please copilot I'm begging you
    private void SpawnEnemy()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("EnemySpawner: no enemyPrefabs assigned.");
            return;
        }

        GameObject prefabToSpawn = null;

        // Prefer the prepared spawnSequence
        if (spawnSequence != null && spawnIndex < spawnSequence.Count)
        {
            prefabToSpawn = spawnSequence[spawnIndex];
            spawnIndex++;
        }
        else
        {
            // Fallback: random selection from available prefabs
            int fallbackIndex = Random.Range(0, enemyPrefabs.Length);
            prefabToSpawn = enemyPrefabs[fallbackIndex];
        }

        if (prefabToSpawn == null)
        {
            Debug.LogWarning("EnemySpawner: prefab in spawnSequence is null, skipping spawn.");
            return;
        }

        Instantiate(prefabToSpawn, LevelManager.main.startPoint.position, Quaternion.identity);

        // Track alive enemies so wave end logic can work
        enemiesAlive++;
    }
}