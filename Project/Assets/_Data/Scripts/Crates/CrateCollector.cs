using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static CrateExtensions;

/// <summary>
/// MonoBehaviour that uses the GameObject's collider to detect and collect crates.
/// </summary>
public class CrateCollector : MonoBehaviour
{
    [SerializeField]
    GameObject marker;

    [Space, Header("Settings")]
    [SerializeField, Range(0f, 60f)]
    float collectionInterval = 30f;

    [SerializeField]
    Color activeColor = Color.green;

    [SerializeField]
    Color inactiveColor = Color.red;

    [SerializeField]
    Vector2Int requirementRange;
    
    [SerializeField]
    bool randomiseRequirementOnCollection = false;

    [SerializeField]
    bool requireCorrectCrateTag = false;

    [SerializeField]
    bool randomiseRequiredCorrectCrateTag = true;

    [Space, Header("Event Bindings")]

    [SerializeField]
    UnityEvent<float> onCollection;

    [SerializeField]
    UnityEvent<CrateRequirement> onRequirementUpdate;

    [SerializeField]
    UnityEvent<float> onScoreUpdated;

    [SerializeField]
    UnityEvent onCollectionPeriodStarted;

    [SerializeField]
    UnityEvent onCollectionPeriodEnded;
    
    [SerializeField]
    UnityEvent onQuotaMet;

    float timer = 0f;
    bool canCollect = false;
    CrateRequirement collectionRequierment;
    float collectionScore = 0f;
    float currentCollectionScore = 0f;
    List<ICollectable> toCollect;
    Material markerMaterial;

    public int Quota => collectionRequierment.requiredCount;
    public CrateTag RequiredTag => collectionRequierment.requiredTag;
    bool RequirementMet => (toCollect.Count >= collectionRequierment.requiredCount);

    void UpdateRequirement()
    {
        var req = new CrateRequirement()
        {
            requiredCount = UnityEngine.Random.Range(requirementRange.x, requirementRange.y + 1),
            requiredTag = randomiseRequiredCorrectCrateTag ? GetRandomCrateTag() : CrateTag.Red,
        };

        collectionRequierment = req;
        onRequirementUpdate.Invoke(req);
    }

    void Initialise()
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
        if (requirementRange.x < 0) requirementRange.x = 1; 
        if (requirementRange.y < 0) requirementRange.y = 1;
        if (requirementRange.x > requirementRange.y)
        {
            requirementRange.x = requirementRange.y;
        }
        Initialise();
    }

    private void Awake()
    {
        Initialise();
        toCollect = new List<ICollectable>();
        onScoreUpdated.Invoke(collectionScore);
    }

    // If other is a collectable add it to list
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<ICollectable>(out ICollectable collectable))
        {
            AddCollectableToList(collectable);
        }
    }

    // If other is a collectable remove it from list
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<ICollectable>(out ICollectable collectable))
        {
            RemoveCollectableFromList(collectable);
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
        currentCollectionScore += collectable.Score;
        Destroy(collectable.GameObject);
    }

    // Handle timer
    private void UpdateTimer()
    {
        if (canCollect == true) return;

        timer += Time.deltaTime;
        if (timer < collectionInterval) return;

        canCollect = true;

        OnCollectionStarted();
        return;
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
        onCollection.Invoke(currentCollectionScore);
        onScoreUpdated.Invoke(collectionScore);
        onQuotaMet.Invoke();
        currentCollectionScore = 0f;

        // Hide text
        OnCollectionEnded();
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

    // These functions could eventually be bound to an action so they aren't directly invoked via code
    void OnCollectionStarted()
    {
        // Show requirement text
        onCollectionPeriodStarted.Invoke();

        if (randomiseRequirementOnCollection) UpdateRequirement();
    }

    void OnCollectionEnded()
    {
        // Hide requirement text
        onCollectionPeriodEnded.Invoke();
    }

    // Add the collectable to the toCollect list
    void AddCollectableToList(ICollectable collectable)
    {
        if (toCollect.Contains(collectable)) return;
        if (!requireCorrectCrateTag || collectable.Tag != collectionRequierment.requiredTag) return;

        toCollect.Add(collectable);
        collectable.GameObject.GetComponent<PhysicsPickup>().OnGrabbed += RemoveCollectableFromList;
        Debug.Log("Added object");
    }

    // Removes the object in toCollect if it was picked up
    void RemoveCollectableFromList(ICollectable collectable)
    {
        if (!toCollect.Contains(collectable)) return;

        toCollect.Remove(collectable);
        collectable.GameObject.GetComponent<PhysicsPickup>().OnGrabbed -= RemoveCollectableFromList;
        Debug.Log("Removing object");
    }
}
