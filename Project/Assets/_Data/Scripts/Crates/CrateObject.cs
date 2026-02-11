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
    MaterialPropertyBlock block;
    PhysicsPickup pickup;
    TextMeshPro[] textObjects;

    private void Awake()
    {
        block = new MaterialPropertyBlock();
        collect = true;
        
        // Bind grabbing event to pickup controller
        if (!TryGetComponent(out pickup))
        {
            Debug.LogWarning("No PhysicsPickup on CrateObject");
        }

        // Get textObjects
        textObjects = GetComponentsInChildren<TextMeshPro>();
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

    // Displays the current score in the textObjects
    void UpdateTextObjects()
    {
        foreach (var tmp in textObjects)
        {
            tmp.SetText(score.ToString());
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
    void OnGrabbed() => CanCollect = false;
    void OnDropped() => CanCollect = true;
}
