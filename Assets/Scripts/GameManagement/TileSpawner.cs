using System.Collections.Generic;
using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    [Header("Tile Prefabs")]
    public GameObject[] tilePrefabs;

    [Header("Collectibles")]
    public GameObject[] collectibles;

    [Header("Obstacles")]
    public GameObject[] obstacles;

    [Header("Spawner Settings")]
    public Transform player;
    public int tilesOnScreen = 5;
    public float tileLength = 40f;

    private readonly List<GameObject> activeTiles = new List<GameObject>();
    private float spawnZ = 0f;

    void Start()
    {
        GenerateInitialTrack();
    }

    void Update()
    {
        HandleDynamicStreaming();
    }

    //Tracking & Layout Setup
    private void GenerateInitialTrack()
    {
        for (int i = 0; i < tilesOnScreen; i++)
        {
            SpawnTile();
        }
    }

    private void HandleDynamicStreaming()
    {
        if (player == null) return;

        //Check if new tile creation is required
        if (player.position.z > spawnZ - (tilesOnScreen * tileLength))
        {
            SpawnTile();
        }

        //Clean up passed modules
        if (activeTiles.Count > 0)
        {
            GameObject oldestTile = activeTiles[0];
            float tilePivotOffset = tileLength / 2f;
            float tileThresholdZ = oldestTile.transform.position.z + tilePivotOffset;

            if (player.position.z > tileThresholdZ)
            {
                RemoveOldestTile();
            }
        }
    }

    private void SpawnTile()
    {
        if (tilePrefabs.Length == 0) return;

        int index = Random.Range(0, tilePrefabs.Length);
        float spawnOffset = tileLength / 2f;
        Vector3 spawnPosition = new Vector3(0f, 0f, spawnZ + spawnOffset);

        GameObject tileInstance = Instantiate(tilePrefabs[index], spawnPosition, Quaternion.identity);
        activeTiles.Add(tileInstance);

        PopulateTileContent(tileInstance);

        spawnZ += tileLength;
    }

    private void RemoveOldestTile()
    {
        if (activeTiles.Count == 0) return;

        Destroy(activeTiles[0]);
        activeTiles.RemoveAt(0);
    }

    //Content Population Distribution
    private void PopulateTileContent(GameObject tile)
    {
        Transform spawnPoints = tile.transform.Find("SpawnPoints");
        Transform collectiblePoints = tile.transform.Find("CollectiblePoints");
        Transform collectiblesParent = tile.transform.Find("Collectibles");
        Transform obstaclesParent = tile.transform.Find("Obstacles");

        if (spawnPoints != null)
        {
            ProcessMixedSpawnPoints(spawnPoints, obstaclesParent, collectiblesParent);
        }

        if (collectiblePoints != null)
        {
            ProcessDedicatedCollectiblePoints(collectiblePoints, collectiblesParent);
        }
    }

    private void ProcessMixedSpawnPoints(Transform spawnPoints, Transform obstacleGroup, Transform collectibleGroup)
    {
        int targetObstacleSlotIndex = Random.Range(0, spawnPoints.childCount);
        int randomObstaclePrefabIndex = obstacles.Length > 0 ? Random.Range(0, obstacles.Length) : -1;

        for (int i = 0; i < spawnPoints.childCount; i++)
        {
            Transform spawnAnchor = spawnPoints.GetChild(i);

            if (i == targetObstacleSlotIndex)
            {
                TrySpawnObstacle(spawnAnchor, randomObstaclePrefabIndex, obstacleGroup);
            }
            else
            {
                //75% spawn rate chance configuration for primary lanes
                TrySpawnRandomAsset(spawnAnchor, collectibleGroup, 0.75f);
            }
        }
    }

    private void ProcessDedicatedCollectiblePoints(Transform collectiblePoints, Transform collectibleGroup)
    {
        for (int i = 0; i < collectiblePoints.childCount; i++)
        {
            Transform spawnAnchor = collectiblePoints.GetChild(i);

            //50% spawn rate chance configuration for auxiliary side lanes
            TrySpawnRandomAsset(spawnAnchor, collectibleGroup, 0.50f);
        }
    }

    //Low-level Utility Spawning Handlers
    private void TrySpawnObstacle(Transform anchor, int itemIndex, Transform parentGroup)
    {
        if (itemIndex < 0 || obstacles.Length == 0) return;

        Instantiate(obstacles[itemIndex], anchor.position, Quaternion.identity, parentGroup);
    }

    private void TrySpawnRandomAsset(Transform anchor, Transform parentGroup, float probabilityWeight)
    {
        if (collectibles.Length == 0 || Random.value > probabilityWeight) return;

        int randomCollectibleIndex = Random.Range(0, collectibles.Length);
        Instantiate(collectibles[randomCollectibleIndex], anchor.position, Quaternion.identity, parentGroup);
    }
}