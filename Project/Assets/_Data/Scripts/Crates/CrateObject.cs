using TMPro;
using UnityEngine;
using static CrateExtensions;

/// <summary>
///  This class is mostly for demonstration.
///  Represents a crate that can be collected.
/// </summary>
public class CrateObject : MonoBehaviour, ICollectable
{
    [SerializeField]
    float score;

    [SerializeField]
    CrateTag crateTag;

    [SerializeField]
    bool useColouredTags = true;

    [SerializeField, Min(0f)]
    float velocityDamageThreshold = 10f, damageCoefficient = 1f;

    [SerializeField, Tooltip("Values are combined to define the maximum loss from damage.")]
    float maximumScoreLossValue = 10f;

    [SerializeField, Tooltip("Values are combined to define the maximum loss from damage."), Range(0f, 1f)]
    float maximumScoreLossPercentage = 0f;

    float maxScore;
    bool collect;
    string startingPromptText;
    ContextualPromptSource promptSource;
    MaterialPropertyBlock block;
    PhysicsPickup pickup;
    TextMeshPro[] textObjects;

    // As in the minimum score the crate can have
    float MaximumScoreReduction => maxScore * (1 - maximumScoreLossPercentage) - maximumScoreLossValue;

    /// <summary>
    /// Instantiate and initialise a new crate.
    /// </summary>
    /// <param name="prefab">Prefab to use, must have a ICollectable.</param>
    /// <param name="transform">Transform to spawn and parent to.</param>
    /// <param name="crateTag">Tag for the crate.</param>
    /// <param name="score">Starting score of the crate</param>
    /// <returns></returns>
    public static ICollectable Instantiate(GameObject prefab, Transform transform, CrateTag crateTag, float score)
    {
        var collectable = Instantiate(prefab, transform).GetComponent<ICollectable>();
        collectable.Tag = crateTag;
        collectable.Score = collectable.MaxScore = score;
        return collectable;
    }

    private void Awake()
    {
        textObjects = GetComponentsInChildren<TextMeshPro>();
        promptSource = GetComponentInChildren<ContextualPromptSource>();
        block = new MaterialPropertyBlock();
        collect = true;
        startingPromptText = "<sprite name=\"Xbox_Y\">";

        // Bind grabbing event to pickup controller
        if (!TryGetComponent(out pickup))
        {
            Debug.LogWarning("No PhysicsPickup on CrateObject");
        }

    }

    private void OnEnable()
    {
        if (pickup != null)
        {
            pickup.OnGrabbed += OnGrabbed;
            pickup.OnDropped += OnDropped;
        }
    }

    private void OnDisable()
    {
        if (pickup != null)
        {
            pickup.OnGrabbed -= OnGrabbed;
            pickup.OnDropped -= OnDropped;
        }
    }

    // Handles taking damage on collisions if this crate is going fast enough
    private void OnCollisionEnter(Collision collision)
    {
        var relativeVelocity = collision.relativeVelocity;
        if (relativeVelocity.magnitude > velocityDamageThreshold)
        {
            Score = (int)Mathf.Max(MaximumScoreReduction, (int)Score - GetScoreLoss(relativeVelocity));
        }
    }

    public float MaxScore
    {
        get => maxScore;
        set => maxScore = value;
    }

    public float Score
    {
        get => score;
        set
        {
            score = value;
            UpdateTextObjects();
        }
    }

    public CrateTag Tag
    {
        get { return crateTag; }
        set
        {
            crateTag = value;
            if (!useColouredTags) return;

            // Colour this object based on its tag
            var renderer = GetComponent<Renderer>();
            renderer.GetPropertyBlock(block);
            block.SetColor("_BaseColor", value.GetColourFromTag());
            renderer.SetPropertyBlock(block);
        }
    }

    public GameObject GameObject { get => gameObject; }

    public bool CanCollect
    {
        get => collect;
        set => collect = value;
    }

    // Make the object collect-able or not
    void OnGrabbed()
    {
        CanCollect = false;
        UpdatePromptTextToDrop();
    }
    void OnDropped() 
    { 
        CanCollect = true;
        UpdatePromptTextToGrab();
    }

    // Returns how much score would be lost based on the relative velocity of a collision
    float GetScoreLoss(Vector3 relativeVelocity) => (relativeVelocity.magnitude - velocityDamageThreshold) * damageCoefficient;

    // Displays the current score in the textObjects
    void UpdateTextObjects()
    {
        foreach (var tmp in textObjects)
        {
            tmp.SetText(score.ToString());
        }
    }

    // Particularly ugly ways to change the prompt text
    void UpdatePromptTextToGrab()
    {
        if (promptSource == null) return;

        promptSource.message = $"{startingPromptText} Grab";
    }

    void UpdatePromptTextToDrop()
    {
        if (promptSource == null) return;

        promptSource.message = $"{startingPromptText} Drop";
    }
}
