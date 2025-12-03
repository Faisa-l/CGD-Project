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

    MaterialPropertyBlock block;

    private void Awake()
    {
        block = new MaterialPropertyBlock();
    }

    public float Score
    {
        get { return score; }
        set { score = value; }
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
 
    
}
