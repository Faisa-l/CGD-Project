using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// MonoBehaviour which handles spawning collectable crates.
/// </summary>
public class CrateSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject cratePrefab;

    [SerializeField, Range(0f, 30f)]
    float spawnInterval = 10f;

    [SerializeField, Range(0, 100)]
    int spawnExtra = 2;

    [SerializeField, Tooltip("Spawns one crate at each point")]
    List<Transform> points;

    // I have the potential to do something incredibly funny here to get the quota
    // Really it should be a value thats practically globally accessible so instead this will reference the crate collector
    // The alternative would be to either make a singleton that holds this type of data or a ScriptableObject which holds the value
    [SerializeField]
    CrateCollector collector;

    // Key is where the spawned crate came from
    // Very important nothing directly indexes this otherwise bad things will happen
    Dictionary<Transform, GameObject> spawnedObjects;
    float timer = 0f;

    int Quota => collector ? collector.Quota : 10;

    private void OnValidate()
    {
        // Valid prefab
        if(cratePrefab == null || !cratePrefab.TryGetComponent<ICollectable>(out ICollectable _))
        {
            Debug.LogWarning("Spawner does not have correct crate prefab.");
        }

        // Valid range
        if (spawnInterval < 0f)
        {
            spawnInterval = 0f;
        }

        // Auto-populate points
        int count = transform.childCount;

        for (int i = 0; i < count; i++)
        {
            Transform child = transform.GetChild(i);
            if (!points.Contains(child))
            {
                points.Add(child);
            }
        }

    }

    // Populates spawnedObjects
    void Initalise()
    {
        spawnedObjects = new Dictionary<Transform, GameObject>();

        foreach (Transform t in points)
        {
            spawnedObjects.Add(t, null);
        }
    }

    private void Awake()
    {
        Initalise();
    }

    private void Update()
    {
        bool ready = UpdateTimer();

        if (ready)
        {
            // Spawn crates
            TrySpawnCrates();
        }
    }

    bool UpdateTimer()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            return true;
        }
        return false;
    }

    // Attempts to spawn a crate at each point if its mapped GameObject is null
    void TrySpawnCrates()
    {
        List<Transform> spawnPoints = new List<Transform>();
        int spawned = 0;

        // Get transforms to spawn
        foreach (var pair in spawnedObjects)
        {
            if (pair.Value == null)
            {
                spawnPoints.Add(pair.Key);
            }
            else
            {
                spawned++;
            }
        }
        ShuffleList<Transform>(spawnPoints);
        
        // Only spawn enough crates to meet quota (truncate spawnPoints)
        int diff = Quota - spawned + spawnExtra;
        if (diff > 0)
        {
            diff = math.clamp(diff, 0, spawnPoints.Count);
            spawnPoints = new List<Transform>(spawnPoints).GetRange(0, diff);

            // Actual spawning
            foreach (Transform t in spawnPoints)
            {
                spawnedObjects[t] = Instantiate(cratePrefab, t);
            }
        }
    }

    // Randomise spawnable transforms (Fisher-Yates shuffle I found on stack overflow)
    // Partition list from 0 to pointer to end -> Select random element -> swap with pointer element -> decrement pointer
    static void ShuffleList<T>(List<T> list)
    {
        var rnd = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rnd.Next(0, n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }
}
