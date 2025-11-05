
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MonoBehaviour which handles spawning collectable crates.
/// </summary>
public class CrateSpawner : MonoBehaviour
{
    [Header("Spawns one crate at each point")]

    [SerializeField]
    GameObject cratePrefab;

    [SerializeField, Range(0f, 30f)]
    float spawnInterval = 10f;

    [SerializeField]
    List<Transform> points;

    // Key is where the spawned crate came from
    // Very important nothing directly indexes this otherwise bad things will happen
    Dictionary<Transform, GameObject> spawnedObjects;

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

    }

    private void Awake()
    {
        Initalise();
    }

    // Populates spawnedObjects
    void Initalise()
    {
        foreach (Transform t in points)
        {
            spawnedObjects.Add(t, null);
        }
    }
}
