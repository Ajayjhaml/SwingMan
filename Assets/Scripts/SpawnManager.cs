using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Prefabs to Spawn")]
    public GameObject coinPrefab;
    public GameObject heartPrefab;
    public GameObject diamondPrefab;
    public GameObject skullPrefab;
    public GameObject bomb;

    [Header("Spawn Settings")]
    public float spawnInterval = 2f;          // Time between spawns
    public float objectLifetime = 6f;         // How long before objects disappear
    public int maxObjectsAtOnce = 10;         // Prevent too many items

    [Header("Camera Settings")]
    public Camera mainCamera;                 // Reference to the main camera

    private List<GameObject> spawnedObjects = new List<GameObject>();

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            // Prevent overcrowding
            CleanupSpawnedObjects();

            if (spawnedObjects.Count < maxObjectsAtOnce)
                SpawnRandomObject();
        }
    }

    void SpawnRandomObject()
    {
        if (mainCamera == null)
            return;

        // Get visible camera bounds
        Vector3 camPos = mainCamera.transform.position;
        float camHeight = 2f * mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;

        // Generate random position inside camera view
        float randomX = Random.Range(camPos.x - camWidth / 2f, camPos.x + camWidth / 2f);
        float randomY = Random.Range(camPos.y - camHeight / 2f, camPos.y + camHeight / 2f);
        Vector3 spawnPos = new Vector3(randomX, randomY, 0f);

        // Randomly select one prefab
        GameObject prefabToSpawn = GetRandomPrefab();

        if (prefabToSpawn != null)
        {
            GameObject obj = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
            spawnedObjects.Add(obj);

            // Start disappearance countdown
            StartCoroutine(DestroyAfterTime(obj, objectLifetime));
        }
    }

    GameObject GetRandomPrefab()
    {
        int random = Random.Range(0, 4); // 0=coin, 1=heart, 2=diamond, 3=skull

        switch (random)
        {
            case 0: return coinPrefab;
            case 1: return heartPrefab;
            case 2: return diamondPrefab;
            case 3: return skullPrefab;
            default: return null;
        }
    }

    IEnumerator DestroyAfterTime(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);

        if (obj != null)
        {
            spawnedObjects.Remove(obj);
            Destroy(obj);
        }
    }

    void CleanupSpawnedObjects()
    {
        spawnedObjects.RemoveAll(o => o == null);
    }

    // Optional: Draw the spawn area for debugging in Editor
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null) return;

        Vector3 camPos = mainCamera.transform.position;
        float camHeight = 2f * mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(camPos, new Vector3(camWidth, camHeight, 0f));
    }
#endif
}
