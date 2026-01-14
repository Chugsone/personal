using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;

    public Transform startPoint;
    public Transform[] path;


    [Header("SpawnPoints")]
    [SerializeField] private bool useSpawnPoints = true;
    [SerializeField] private SpawnPoints[] Waves = new SpawnPoints[12];

    public int currency;

    private void Awake()
    {
        main = this;

        // Ensure Waves array entries are not null so they can hold spawn points at runtime
        if (Waves == null)
            Waves = new SpawnPoints[0];

        for (int i = 0; i < Waves.Length; i++)
        {
            if (Waves[i] == null)
                Waves[i] = new SpawnPoints();
        }
    }

    private void Start()
    {
        currency = 500;
    }

    public void AddCurrency(int amount)
    {
        currency += amount;
    }

    public bool SpendCurrency(int amount)
    {
        if (amount <= currency)
        {
            currency -= amount;
            return true;
        }

        Debug.Log("you broke cuh");
        return false;
    }

    // Add a single spawn point to a specific wave (runtime)
    public void AddSpawnPointToWave(int waveIndex, Transform spawnPoint)
    {
        if (!useSpawnPoints) return;
        if (waveIndex < 0 || waveIndex >= Waves.Length)
        {
            Debug.LogWarning($"Invalid waveIndex: {waveIndex}");
            return;
        }

        if (spawnPoint == null) return;
        Waves[waveIndex].spawnPoints.Add(spawnPoint);
    }

    // Add multiple spawn points to a specific wave (runtime)
    public void AddSpawnPointsToWave(int waveIndex, IEnumerable<Transform> spawnPoints)
    {
        if (!useSpawnPoints) return;
        if (waveIndex < 0 || waveIndex >= Waves.Length)
        {
            Debug.LogWarning($"Invalid waveIndex: {waveIndex}");
            return;
        }

        foreach (var t in spawnPoints)
        {
            if (t != null)
                Waves[waveIndex].spawnPoints.Add(t);
        }
    }

    // Get spawn points for a wave (safe copy)
    public IReadOnlyList<Transform> GetSpawnPointsForWave(int waveIndex)
    {
        if (waveIndex < 0 || waveIndex >= Waves.Length) return new Transform[0];
        return Waves[waveIndex].spawnPoints.AsReadOnly();
    }

    [System.Serializable]
    public class SpawnPoints
    {
        // Visible and editable in the Inspector. Use the inspector to drop Transforms,
        // or call AddSpawnPointToWave/AddSpawnPointsToWave at runtime.
        public List<Transform> spawnPoints = new List<Transform>();
    }
}