
using System.Collections.Generic;
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

    [SerializeField, Tooltip("Spawns one crate at each point")]
    List<Transform> points;

    // Key is where the spawned crate came from
    // Very important nothing directly indexes this otherwise bad things will happen
    Dictionary<Transform, GameObject> spawnedObjects;
    float timer = 0f;

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
        List<Transform> modified = new List<Transform>();

        // Get transforms to spawn
        foreach (var pair in spawnedObjects)
        {
            if (pair.Value == null)
            {
                modified.Add(pair.Key);
            }
        }

        // Actual spawning
        foreach (Transform t in modified)
        {
            spawnedObjects[t] = Instantiate(cratePrefab, t);
        }
    }
}
