using UnityEngine;

[System.Serializable]
public class Biome
{
    public string biomeName;

    [Header("Noise Threshold Range")]
    public float minNoiseThreshold;
    public float maxNoiseThreshold;

    [Header("Spawn Chances")]
    [Range(0f, 1f)] public float treeChance;
    [Range(0f, 1f)] public float rockChance;
    [Range(0f, 1f)] public float waterChance;

    [Header("Vegetation Prefabs")]
    public GameObject[] treePrefabs;
    public GameObject[] rockPrefabs;
    public GameObject[] waterPrefabs;

    [Header("Terrain Visuals")]
    public TerrainLayer terrainTexture;

    public bool Contains(float noiseValue)
    {
        return noiseValue >= minNoiseThreshold && noiseValue < maxNoiseThreshold;
    }

    public GameObject GetRandomTree() =>
        treePrefabs != null && treePrefabs.Length > 0
        ? treePrefabs[Random.Range(0, treePrefabs.Length)]
        : null;

    public GameObject GetRandomRock() =>
        rockPrefabs != null && rockPrefabs.Length > 0
        ? rockPrefabs[Random.Range(0, rockPrefabs.Length)]
        : null;

    public GameObject GetRandomWater() =>
        waterPrefabs != null && waterPrefabs.Length > 0
        ? waterPrefabs[Random.Range(0, waterPrefabs.Length)]
        : null;
}
