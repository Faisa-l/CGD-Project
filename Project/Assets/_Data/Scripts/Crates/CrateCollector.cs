using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// MonoBehaviour that uses the GameObject's collider to detect and collect crates.
/// </summary>
public class CrateCollector : MonoBehaviour
{
    [SerializeField]
    GameObject marker;

    [SerializeField, Range(0f, 60f)]
    float collectionInterval = 30f;

    [SerializeField]
    Color activeColor = Color.green;

    [SerializeField]
    Color inactiveColor = Color.red;

    [SerializeField]
    Vector2Int requirementRange;

    [SerializeField]
    UnityEvent<float> onCollection;

    [SerializeField]
    UnityEvent<int> onRequirementUpdate;

    float timer = 0f;
    bool canCollect = false;
    int collectionRequierment = 1;
    float collectionScore = 0f;
    List<ICollectable> toCollect;
    Material markerMaterial;

    bool RequirementMet => (toCollect.Count >= collectionRequierment);
    void UpdateRequirement()
    {
        collectionRequierment = Random.Range(requirementRange.x, requirementRange.y);
        onRequirementUpdate.Invoke(collectionRequierment);
    }

    void initialise()
    {
        if (!TryGetComponent<Collider>(out Collider collectorCollider))
        {
            Debug.LogWarning("Collector is missing a collider.");
        }

        if (marker.TryGetComponent<Renderer>(out Renderer renderer))
        {
            markerMaterial = renderer.sharedMaterial;
            markerMaterial.SetColor("_BaseColor", activeColor);
        }
        UpdateRequirement();
    }

    private void OnValidate()
    {
        if (requirementRange.x < 0) requirementRange.x = 0;
        if (requirementRange.y < 0) requirementRange.y = 0;
        if (requirementRange.x > requirementRange.y)
        {
            requirementRange.x = requirementRange.y;
        }
        initialise();
    }

    private void Awake()
    {
        initialise();
        toCollect = new List<ICollectable>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // If collider is a 'collectable'
        if (other.TryGetComponent<ICollectable>(out ICollectable collectable))
        {
            // CollectCrate(collectable);
            toCollect.Add(collectable);
            Debug.Log("Added object");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<ICollectable>(out ICollectable collectable))
        {
            if (toCollect.Contains(collectable))
            {
                toCollect.Remove(collectable);
                Debug.Log("removed object");
            }
        }
    }

    private void Update()
    {
        UpdateTimer();
        AdjustMaterial();
        HandleCollection();
    }

    // Collects the crate (removes the object and adds some score)
    private void CollectCrate(ICollectable collectable)
    {
        // Do something with its data
        collectionScore += collectable.Score;
        Destroy(collectable.GameObject);
    }

    // Handle timer
    private bool UpdateTimer()
    {
        timer += Time.deltaTime;
        if (timer < collectionInterval) return false;
        canCollect = true;
        return true;
    }

    // Collects items in toCollect if canCollect is true
    private void HandleCollection()
    {
        if (!canCollect) return;
        if (!RequirementMet) return;
        
        foreach (ICollectable item in toCollect)
        {
            CollectCrate(item);
        }

        toCollect.Clear();
        timer = 0f;
        canCollect = false;
        onCollection.Invoke(collectionScore);
        UpdateRequirement();
    }

    // Change material on object based on canCollect state
    private void AdjustMaterial()
    {
        if (canCollect)
        {
            markerMaterial.SetColor("_BaseColor", activeColor);
        }
        else
        {
            markerMaterial.SetColor("_BaseColor", inactiveColor);
        }
    }
}
