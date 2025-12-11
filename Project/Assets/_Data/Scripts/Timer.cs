using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Simple timer for tracking elapsed time and calling functions when a timer expires.
/// </summary>
public class Timer : MonoBehaviour
{
    [SerializeField, Range(0f, 100f), Tooltip("How long the timer will continue.")]
    float duration = 1f;

    [SerializeField, Tooltip("Make the timer automatically restart once it passes its duration.")]
    bool repeat = false;

    [SerializeField, Tooltip("Make the timer immediately begin on Start")]
    bool autoStart = true;

    [Tooltip("Events to run when the timer expires.")]
    public UnityEvent timeout;

    float currentTime;
    bool running;

    public float ElapsedTime => currentTime;
    public bool IsRunning => running;

    private void Awake()
    {
        Initialise();
    }

    private void Start()
    {
        if (autoStart) Begin();
    }

    // Increments current time each frame
    private void Update()
    {
        if (running)
        {
            currentTime += Time.deltaTime;

            // Is this timer expired
            if (currentTime > duration)
            {
                timeout.Invoke();

                // Restart timer if it should repeat
                if (repeat)
                {
                    Restart();

                }
                else
                {
                    Pause();
                }
            }
        }
    }

    void Initialise()
    {
        currentTime = 0f;
        running = false;
    }

    /// <summary>
    /// Starts the timer.
    /// </summary>
    public void Begin()
    {
        running = true;
    }

    /// <summary>
    /// Stops the timer.
    /// </summary>
    public void Pause()
    {
        running = false;
    }

    /// <summary>
    /// Resets the timer's internally tracked time.
    /// </summary>
    public void Restart()
    {
        currentTime = 0f;
    }

    /// <summary>
    /// Fast forwards the interally tracked time to a new time. Can immediately force a timeout.
    /// </summary>
    /// <param name="newTime"> New time to fast forward to. </param>
    public void ForwardTo(float newTime = math.INFINITY)
    {
        currentTime = newTime;
    }

    private void OnDestroy()
    {
        timeout = null;
    }
}
