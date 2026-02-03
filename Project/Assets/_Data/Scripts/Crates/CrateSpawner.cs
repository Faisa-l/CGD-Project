using System.Collections.Generic;
using UnityEngine;
using static CrateExtensions;

/// <summary>
/// MonoBehaviour which handles spawning collectable crates.
/// </summary>
[RequireComponent(typeof(Timer))]
public class CrateSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject cratePrefab;

    [SerializeField, Tooltip("Timeout event is assigned at runtime.")]
    Timer timer;

    [SerializeField, Tooltip("How many objects should be spawned for a given tag.")]
    List<SpawnRequirements> spawnRequirements;

    // Maps a spawn point to its spawned object
    // This shouldn't be resizing in gameplay; its size should be predetermined in Initalise()
    Dictionary<SpawnNode, ICollectable> spawnedObjects;
    

    private void OnValidate()
    {
        // Valid prefab
        if (cratePrefab == null || !cratePrefab.TryGetComponent(out ICollectable _))
        {
            Debug.LogWarning("Spawner does not have correct crate prefab.");
        }

        if (TryGetComponent(out timer))
        {
            timer.repeat = true;
            timer.autoStart = false;
        }
    }

    void Initialise()
    {
        // Populate spawnedObjects with nodes built from each spawn requirement
        spawnedObjects = new Dictionary<SpawnNode, ICollectable>();
        foreach (var req in spawnRequirements)
        {
            foreach (var t in req.parentTransform.GetComponentsInChildren<Transform>())
            {
                var node = new SpawnNode()
                {
                    tag = req.tag,
                    transform = t,
                };
                spawnedObjects.Add(node, null);
            }
        }
    }

    private void Awake()
    {
        Initialise();
    }

    private void OnEnable()
    {
        if (timer != null)
        {
            timer.timeout.AddListener(TrySpawnCrates);
        }
    }

    private void OnDisable()
    {
        if (timer != null)
        {
            timer.timeout.RemoveListener(TrySpawnCrates);
        }
    }

    public void StartSpawner() => timer.paused = false;
    public void StopSpawner() => timer.paused = true;

    // Attempts to spawn a crate at each point if its mapped GameObject is null
    protected void TrySpawnCrates()
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

        // Spawn a crate at each spawn point
        // Will only spawn in crates to fulfil a spawn requirement - ignores node that already meets requirements
        foreach (var node in spawnPoints)
        {
            if (TryGetRequirementFromTag(node.tag, out SpawnRequirements requirement))
            {
                if (WasRequirementMet(requirement)) continue;
                SpawnCrate(node, requirement);
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

    // Spawns a crate and set its data based on its requirement. Instantiate within spawnedObjects
    void SpawnCrate(in SpawnNode node, in SpawnRequirements requirement)
    {
        spawnedObjects[node] = Instantiate(cratePrefab, node.transform).GetComponent<ICollectable>();
        spawnedObjects[node].Tag = node.tag;
        spawnedObjects[node].Score = requirement.crateScore;
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
    bool WasRequirementMet(SpawnRequirements requirement)
    {
        int i = 0;
        foreach (ICollectable item in spawnedObjects.Values)
        {
            if (item == null) continue;
            if (item.Tag == requirement.tag) i++;
            if (i == requirement.spawnCount) break;
        }

        return (i == requirement.spawnCount);
    }

    // Gets a requirement from a given tag and outputs whether this requirement exists or not
    bool TryGetRequirementFromTag(CrateTag tag, out SpawnRequirements requirement)
    {
        requirement = new();

        foreach (var req in spawnRequirements)
        {
            if (req.tag == tag)
            {
                requirement = req;
                return true;
            }
        }
        return false;
    }

}
