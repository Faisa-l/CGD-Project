using System;
using System.Runtime.InteropServices.WindowsRuntime;
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
    CrateTag tag;

    public float Score
    {
        get { return score; }
        set { score = value; }
    }
    public CrateTag Tag
    {
        get { return tag; }
        set { tag = value; } 
    }

    public GameObject GameObject { get => gameObject; }
 
    // Types of tags a crate can have. Add more to the enum if you want.
    // This can be referenced by calling CrateObject.CrateTag. 
    public enum CrateTag { Red, Green, Blue }

    // Gets a random crate tag
    public static CrateTag GetRandomCrateTag()
    {
        int length = Enum.GetNames(typeof(CrateTag)).Length;
        return (CrateTag)UnityEngine.Random.Range(0, length - 1);
    }
}
