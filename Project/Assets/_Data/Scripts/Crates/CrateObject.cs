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

    bool collect;
    string startingPromptText;
    ContextualPromptSource promptSource;
    MaterialPropertyBlock block;
    PhysicsPickup pickup;
    TextMeshPro[] textObjects;

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
