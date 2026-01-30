using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static CrateExtensions;

/// <summary>
/// MonoBehaviour that uses the GameObject's collider to detect and collect crates.
/// </summary>
[RequireComponent(typeof(CollectorScheduler))]
public class CrateCollector : MonoBehaviour
{
    [SerializeField]
    GameObject marker;

    [SerializeField]
    ScoreObject scoreObject;

    [SerializeField]
    CollectorScheduler scheduler;

    [Space, Header("Settings")]

    [SerializeField]
    Color activeColor = Color.green;

    [SerializeField]
    Color inactiveColor = Color.red;

    [Space, Header("Event Bindings")]

    [SerializeField]
    UnityEvent<float> onCollection;

    [SerializeField]
    UnityEvent<TimedCrateRequirement> onRequirementUpdate;

    [SerializeField]
    UnityEvent<float> onScoreUpdated, onItemsForCollectionChanged;

    [SerializeField]
    UnityEvent onCollectionPeriodStarted, onCollectionPeriodEnded, onQuotaMet;

    [SerializeField]
    UnityEvent<bool> onEvaluatedRequirement;

    float collectionScore = 0f;
    float currentCollectionScore = 0f;
    bool canCollect = false;
    bool wasStarted = false;
    List<ICollectable> forCollection;
    TimedCrateRequirement collectionRequirement;
    Material markerMaterial;

    public int Quota => collectionRequirement.requiredScore;
    public CrateTag RequiredTag => collectionRequirement.requiredTag;

    void Initialise()
    {
        if (!TryGetComponent(out Collider collectorCollider))
        {
            Debug.LogWarning("Collector is missing a collider.");
        }

        if (marker.TryGetComponent(out Renderer renderer))
        {
            markerMaterial = renderer.sharedMaterial;
            markerMaterial.SetColor("_BaseColor", activeColor);
        }

        forCollection = new List<ICollectable>();
        scheduler.SchedulerStarted.AddListener(DoCollect);
        scheduler.SchedulerUpdated.AddListener(UpdateFromSchedule);
        scheduler.SchedulerEnded.AddListener(NoCollect);
        scheduler.SchedulerEnded.AddListener(EvaluateRequirement);
    }

    private void Awake()
    {
        Initialise();
        StartCollector();
        onScoreUpdated.Invoke(collectionScore);
    }

    private void OnDestroy()
    {
        scheduler.SchedulerStarted.RemoveListener(DoCollect);
        scheduler.SchedulerUpdated.RemoveListener(UpdateFromSchedule);
        scheduler.SchedulerEnded.RemoveListener(NoCollect);
        scheduler.SchedulerEnded.RemoveListener(EvaluateRequirement);
    }

    // If other is a collectable add it to list
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ICollectable collectable))
        {
            TryCollect(collectable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out ICollectable collectable))
        {
            RemoveCollectable(collectable);
        }
    }

    // Will attempt to collect the given collectable
    void TryCollect(ICollectable collectable)
    {
        if (canCollect && collectable.CanCollect && collectable.Tag == collectionRequirement.requiredTag && !forCollection.Contains(collectable))
        {
            forCollection.Add(collectable);
            onItemsForCollectionChanged.Invoke(GetScoreWaitingInCollection());
        }
    }

    // Remove collectable from the list
    void RemoveCollectable(ICollectable collectable)
    {
        if (canCollect && forCollection.Contains(collectable))
        {
            forCollection.Remove(collectable);
            onItemsForCollectionChanged.Invoke(GetScoreWaitingInCollection());
        }
    }

    // Starts the collector for collecting
    public void StartCollector()
    {
        wasStarted = true;
        scheduler.StartScheduler();
    }

    // Set the current collection requirement to what is currently scheduled
    void UpdateRequirement()
    {
        if (!scheduler.Running) return;

        collectionRequirement = scheduler.CurrentRequirement;
        onRequirementUpdate.Invoke(collectionRequirement);
    }

    // Collects the crate (removes the object and adds some score)
    private void CollectCrate(ICollectable collectable)
    {
        collectionScore += collectable.Score;
        currentCollectionScore += collectable.Score;
        scoreObject.currentScore = collectionScore;
        Destroy(collectable.GameObject);
    }

    // Check if the current requirement was met
    void EvaluateRequirement()
    {
        // Collect everything that should be collected
        foreach(var c in forCollection) CollectCrate(c);

        bool isSuccess = (currentCollectionScore >= collectionRequirement.requiredScore);
        onEvaluatedRequirement.Invoke(isSuccess);
                                                            // These mainly invoke:
        onCollection.Invoke(currentCollectionScore);        // Panel to show how much was collected for this scheduled requirement
        onScoreUpdated.Invoke(collectionScore);             // Update panel which displays the total score
        onQuotaMet.Invoke();                                // Audio
        currentCollectionScore = 0f;

        forCollection.Clear();
    }

    // For invocation whenever the schedule changes the current collection requirement
    void UpdateFromSchedule()
    {
        // Call UpdateRequirement after evaluating the current requirement
        // If this was called when the collector first started, don't evaluate (nothing to check)
        if (!wasStarted)
        {
            EvaluateRequirement();
        }
        else
        {
            wasStarted = false;
        }
        UpdateRequirement();
    }

    // Make this collect or not
    internal void SetCollection(bool can)
    {
        canCollect = can;
        AdjustMaterial(can);

        if (can)
        {
            onCollectionPeriodStarted.Invoke();
        }
        else
        {
            onCollectionPeriodEnded.Invoke();
        }
    }

    // Variants of above but always true/false
    void DoCollect() => SetCollection(true);
    void NoCollect() => SetCollection(false);

    // Change material on object based on canCollect state
    private void AdjustMaterial(bool toActive)
    {
        if (toActive)
        {
            markerMaterial.SetColor("_BaseColor", activeColor);
        }
        else
        {
            markerMaterial.SetColor("_BaseColor", inactiveColor);
        }
    }

    // Returns predicted score
    float GetScoreWaitingInCollection()
    {
        float s = 0f;
        foreach(var item in forCollection)
        {
            s += item.Score;
        }
        return s;
    }

    /*
    // If other is a collectable remove it from list
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<ICollectable>(out ICollectable collectable))
        {

            RemoveCollectableFromList(collectable);
        }
    }
    */

    /*
    private void Update()
    {
        UpdateTimer();
        AdjustMaterial();
        HandleCollection();  
    }
     */

    /*
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
     */



    /*
    void OnCollectionStarted()
    {
        // Show requirement text
        onCollectionPeriodStarted.Invoke();

        // if (randomiseRequirementOnCollection) UpdateRequirement();
    }

    void OnCollectionEnded()
    {
        // Hide requirement text
        onCollectionPeriodEnded.Invoke();
    }
     */

    /*
    // Add the collectable to the toCollect list
    void AddCollectableToList(ICollectable collectable)
    {
        if (toCollect.Contains(collectable)) return;
        if (!requireCorrectCrateTag || collectable.Tag != collectionRequirement.requiredTag) return;

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
     */



}
