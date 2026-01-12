using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct EnemyData
{
    public GameObject prefab;
    public int count;
}

[CreateAssetMenu(menuName = "Wave Data", fileName = "WaveData")]
public class WaveData : ScriptableObject
{
    public string waveName;

    [Tooltip("Optional explicit counts per prefab. Length should match the EnemySpawner's enemyPrefabs length. If non-empty and sum>0, this will be used exactly.")]
    public List<EnemyData> enemies = new();
}