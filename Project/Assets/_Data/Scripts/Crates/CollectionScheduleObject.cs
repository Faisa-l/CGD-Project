using UnityEngine;
using static CrateExtensions;

/// <summary>
/// Contains the schedule for how a collector should collect crates
/// </summary>
[CreateAssetMenu(fileName = "CollectionScheduleObject", menuName = "Scriptable Objects/CollectionScheduleObject")]
public class CollectionScheduleObject : ScriptableObject
{
    [SerializeField]
    TimedCrateRequirement[] collectionSchedule;

    public TimedCrateRequirement[] CollectionSchedule => collectionSchedule;

    // This should use some special attribute so it's not editable in the inspector
    [SerializeField]
    float totalTime = 0f;

    public float TotalTime => totalTime;

    private void OnValidate()
    {
        float t = 0f;
        foreach (var r in collectionSchedule) t += r.timeLimit;
        totalTime = t;
    }
}