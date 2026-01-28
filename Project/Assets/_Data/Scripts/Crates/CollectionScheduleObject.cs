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
}