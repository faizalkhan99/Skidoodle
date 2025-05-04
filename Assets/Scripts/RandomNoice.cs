using System.Collections.Generic;
using UnityEngine;

public class RandomNoice : MonoBehaviour
{
    [Header("Prefabs (Fallback)")]
    [SerializeField] private GameObject _treePrefab;
    [SerializeField] private GameObject _waterPrefab;
    [SerializeField] private GameObject _rockPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private int _objectCount = 500;

    [Header("Terrain Settings")]
    [SerializeField] private float _terrainSize = 512;
    [SerializeField] private float _terrainHeight = 50;
    [SerializeField] private float _noiseScale = 0.01f;
    [SerializeField] private PhysicsMaterial terrainPhysicMaterial;

    private Terrain terrain;
    private TerrainData _terrainData;

    [Header("Biome Settings")]
    [SerializeField] private float biomeScale = 0.005f;
    [SerializeField] private List<Biome> biomes;

    private void Start()
    {
        GenerateGround();
        PlaceObjects();
    }

    private void GenerateGround()
    {
        int _heightMapRes = 513;
        _terrainData = new TerrainData
        {
            heightmapResolution = _heightMapRes,
            size = new Vector3(_terrainSize, _terrainHeight, _terrainSize)
        };

        float[,] heights = new float[_heightMapRes, _heightMapRes];

        for (int i = 0; i < _heightMapRes; i++)
        {
            for (int j = 0; j < _heightMapRes; j++)
            {
                float xCoord = i * _noiseScale;
                float yCoord = j * _noiseScale;

                float height = Mathf.PerlinNoise(xCoord, yCoord);
                heights[i, j] = height;
            }
        }

        _terrainData.SetHeights(0, 0, heights);

        // Assign terrain textures from biomes
        AssignTerrainTextures();
        AddBiomeTextures();

        // Create terrain object
        GameObject terrainGO = Terrain.CreateTerrainGameObject(_terrainData);
        terrainGO.transform.position = Vector3.zero;
        terrain = terrainGO.GetComponent<Terrain>();

        // Apply physic material
        TerrainCollider terrainCollider = terrainGO.GetComponent<TerrainCollider>();
        if (terrainCollider != null && terrainPhysicMaterial != null)
        {
            terrainCollider.material = terrainPhysicMaterial;
        }
    }

    private void AssignTerrainTextures()
    {
        List<TerrainLayer> layers = new();

        foreach (var biome in biomes)
        {
            if (biome.terrainTexture != null && !layers.Contains(biome.terrainTexture))
            {
                layers.Add(biome.terrainTexture);
            }
        }

        _terrainData.terrainLayers = layers.ToArray();
    }


    private void AddBiomeTextures()
    {
        int res = _terrainData.alphamapResolution;
        float[,,] splatmap = new float[res, res, biomes.Count];

        for (int x = 0; x < res; x++)
        {
            for (int z = 0; z < res; z++)
            {
                float normX = (float)x / res;
                float normZ = (float)z / res;

                float worldX = normX * _terrainSize;
                float worldZ = normZ * _terrainSize;

                float noiseVal = Mathf.PerlinNoise(worldX * biomeScale, worldZ * biomeScale);

                for (int i = 0; i < biomes.Count; i++)
                {
                    if (biomes[i].Contains(noiseVal))
                    {
                        splatmap[z, x, i] = 1f;
                        break;
                    }
                }
            }
        }
        if (_terrainData.terrainLayers.Length != biomes.Count)
        {
            Debug.LogError("Mismatch: Number of terrain layers and biomes must match!");
            return;
        }

        _terrainData.SetAlphamaps(0, 0, splatmap);
    }

    private void PlaceObjects()
    {
        if (terrain == null || _terrainData == null) return;

        for (int i = 0; i < _objectCount; i++)
        {
            float x = Random.Range(0, _terrainSize);
            float z = Random.Range(0, _terrainSize);
            float y = terrain.SampleHeight(new Vector3(x, 0, z)) + terrain.GetPosition().y;
            Vector3 pos = new(x, y, z);

            float noiseVal = Mathf.PerlinNoise(x * biomeScale, z * biomeScale);
            Biome biome = biomes.Find(b => b.Contains(noiseVal));
            if (biome == null) continue;

            float roll = Random.value;
            GameObject prefab = null;

            if (roll < biome.treeChance)
                prefab = biome.GetRandomTree();
            else if (roll < biome.treeChance + biome.rockChance)
                prefab = biome.GetRandomRock();
            else if (roll < biome.treeChance + biome.rockChance + biome.waterChance)
                prefab = biome.GetRandomWater();

            if (prefab != null)
                Instantiate(prefab, pos, Quaternion.identity, transform);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_terrainData == null || biomes == null) return;

        int step = 20;
        for (int x = 0; x < _terrainSize; x += step)
        {
            for (int z = 0; z < _terrainSize; z += step)
            {
                float noiseVal = Mathf.PerlinNoise(x * biomeScale, z * biomeScale);
                Biome biome = biomes.Find(b => b.Contains(noiseVal));
                if (biome == null) continue;

                float y = terrain != null
                    ? terrain.SampleHeight(new Vector3(x, 0, z)) + terrain.GetPosition().y
                    : 0;

                Vector3 pos = new(x, y + 2f, z);
                Gizmos.color = ColorFromName(biome.biomeName);
                Gizmos.DrawSphere(pos, 1f);
            }
        }
    }

    private Color ColorFromName(string name)
    {
        int hash = name.GetHashCode();
        float r = ((hash >> 16) & 0xFF) / 255f;
        float g = ((hash >> 8) & 0xFF) / 255f;
        float b = (hash & 0xFF) / 255f;
        return new Color(r, g, b);
    }
#endif
}
