using System;
using UnityEngine;


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
            block.SetColor("_BaseColor", GetColourFromTag(value));
            renderer.SetPropertyBlock(block);
        } 
    }

    public GameObject GameObject { get => gameObject; }
 
    // Types of tags a crate can have. Add more to the enum if you want.
    // This can be referenced by calling CrateObject.CrateTag. 
    public enum CrateTag { Red, Green, Blue }

    // Gets a random crate tag
    public static CrateTag GetRandomCrateTag()
    {
        int length = Enum.GetNames(typeof(CrateTag)).Length;
        return (CrateTag)UnityEngine.Random.Range(0, length);
    }

    // Returns a colour for a given colour-named tag
    public static Color GetColourFromTag(CrateTag tag)
    {
        return tag switch
        {
            CrateTag.Red => Color.red,
            CrateTag.Green => Color.green,
            CrateTag.Blue => Color.blue,
            _ => Color.white,
        };
    }
}
