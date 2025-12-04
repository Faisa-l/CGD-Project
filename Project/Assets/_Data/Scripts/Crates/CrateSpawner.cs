using System;
using System.Collections.Generic;
using UnityEngine;
using static CrateExtensions;

/// <summary>
/// MonoBehaviour which handles spawning collectable crates.
/// </summary>
public class CrateSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject cratePrefab;

    [SerializeField, Range(0f, 30f)]
    float spawnInterval = 10f;

    [SerializeField, Tooltip("Spawns one crate at each of the transform's children with the provided tag.")]
    List<SpawnNode> spawnGroups;

    [SerializeField, Tooltip("How many objects should be spawned for a given tag.")]
    List<CrateRequirement> spawnRequirements;

    // Maps a spawn point to its spawned object
    // This shouldn't be resizing in gameplay; its size should be predetermined in Initalise()
    Dictionary<SpawnNode, ICollectable> spawnedObjects;
    float timer = 0f;

    private void OnValidate()
    {
        // Valid prefab
        if (cratePrefab == null || !cratePrefab.TryGetComponent<ICollectable>(out ICollectable _))
        {
            Debug.LogWarning("Spawner does not have correct crate prefab.");
        }

        // Valid range
        if (spawnInterval < 0f)
        {
            spawnInterval = 0f;
        }
    }

    // Populates spawnedObjects
    void Initalise()
    {
        spawnedObjects = new Dictionary<SpawnNode, ICollectable>();

        // Get the individual transforms in the spawn groups 
        foreach (var group in spawnGroups)
        {
            foreach (Transform t in group.transform.GetComponentInChildren<Transform>())
            {
                var n = new SpawnNode()
                {
                    tag = group.tag,
                    transform = t
                };
                spawnedObjects.Add(n, null);
            }
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
        List<SpawnNode> spawnPoints = new();

        // Get nodes to spawn and randomise the order to spawn
        foreach (var pair in spawnedObjects)
        {
            if (pair.Value == null)
            {
                spawnPoints.Add(pair.Key);
            }
        }
        ShuffleList(spawnPoints);

        // Spawn a crate at each Spawn point.
        // Will only spawn in crates to fulfil a spawn requirement - ignores node that already meets requirements
        foreach (SpawnNode node in spawnPoints)
        {
            CrateTag tag = node.tag;
            if (GetRequirementFromTag(tag, out CrateRequirement requirement))
            {
                if (HasMatchedRequirement(requirement)) continue;

                // Spawn crate and set its tag
                spawnedObjects[node] = Instantiate(cratePrefab, node.transform).GetComponent<ICollectable>();
                spawnedObjects[node].Tag = tag;
            }
        }
        /* Keeping this code here incase we want to revert back to quota based spawning
        // Only spawn enough crates to meet quota (truncate spawnPoints)
        int diff = Quota - spawned + spawnExtra;
        if (diff > 0)
        {
            diff = math.clamp(diff, 0, spawnPoints.Count);
            spawnPoints = new List<SpawnNode>(spawnPoints).GetRange(0, diff);

            // Actual spawning
            foreach (SpawnNode t in spawnPoints)
            {
                spawnedObjects[t] = Instantiate(cratePrefab, t.transform);
            }
        }
        */
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

    // Returns whether a given requirement is met
    bool HasMatchedRequirement(CrateRequirement requirement)
    {
        int i = 0;
        foreach (var pair in spawnedObjects)
        {
            ICollectable item = pair.Value;

            if (item == null) continue;
            if (item.Tag == requirement.requiredTag) i++;
            if (i == requirement.requiredCount) break;
        }

        return (i == requirement.requiredCount);
    }

    // Gets a requirement from a given tag and outputs whether this requirement exists or not
    bool GetRequirementFromTag(CrateTag tag, out CrateRequirement requirement)
    {
        requirement = new();

        foreach (var req in spawnRequirements)
        {
            if (req.requiredTag == tag)
            {
                requirement = req;
                return true;
            }
        }
        return false;
    }


}
