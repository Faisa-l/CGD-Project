using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static CrateExtensions;

/// <summary>
/// Schedules the collection requirements for a given CrateCollector.
/// </summary>
[RequireComponent (typeof(Timer))]
public class CollectorScheduler : MonoBehaviour
{
    [SerializeField]
    Timer timer;

    [SerializeField]
    CollectionScheduleObject scheduleObject;

    [Tooltip("Events fired while the scheduler is running")]
    public UnityEvent SchedulerStarted, SchedulerEnded, SchedulerUpdated;

    bool isRunning;

    public Queue<TimedCrateRequirement> Schedule { get; private set; }
    public TimedCrateRequirement CurrentRequirement { get; private set; }
    public bool Running => isRunning;


    private void OnValidate()
    {
        if (!TryGetComponent(out timer))
        {
            Debug.LogWarning("CollectorScheduler cannot find its timer.");
        }
        else
        {
            timer.autoStart = false;
            timer.repeat = false;
        }

        if (scheduleObject == null)
        {
            Debug.LogWarning("CollectorScheduler does not have a schedule set in the inspector.");
        }
    }

    private void Awake()
    {
        isRunning = false;
        timer.repeat = false;
        timer.autoStart = false;
        timer.timeout.AddListener(UpdateSchedule);
    }

    private void OnDestroy()
    {
        timer.timeout.RemoveListener(UpdateSchedule);
    }

    /// <summary>
    /// Begin running through the collection schedule.
    /// </summary>
    public void StartScheduler()
    {
        isRunning = true;
        SchedulerStarted.Invoke();
        SetSchedule();
        UpdateSchedule();
    }

    void SetSchedule()
    {
        if (scheduleObject == null) return;

        Schedule = new Queue<TimedCrateRequirement>();
        foreach (var req in scheduleObject.CollectionSchedule)
        {
            Schedule.Enqueue(req);
        }
    }

    // Proceed through the schedule and handle what happens if the schedule is empty
    void UpdateSchedule()
    {
        if (TryGetNextInSchedule(out TimedCrateRequirement req))
        {
            // Process next item in schedule
            CurrentRequirement = req;
            timer.Restart();
            timer.duration = CurrentRequirement.timeLimit;
            timer.paused = false;
            SchedulerUpdated.Invoke();
        }
        else
        {
            // Completed the schedule
            isRunning = false;
            SchedulerEnded.Invoke();
        }
    }

    // Attempts to get a new requirement from the stack
    bool TryGetNextInSchedule(out TimedCrateRequirement requirement)
    {
        requirement = new TimedCrateRequirement();

        // Stack is empty (No more items in the schedule)
        if (Schedule.Count == 0) return false;

        requirement = Schedule.Dequeue();
        return true;
    }

}
