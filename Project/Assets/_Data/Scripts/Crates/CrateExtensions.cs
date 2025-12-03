using System;
using UnityEngine;

// You can add 'using static CrateExtensions' to use these things in any other class

/// <summary>
///  Collection of various crate extensions (structs, enums, functions, etc.)
/// </summary>
public static class CrateExtensions
{
    // Struct defining any type of spawn requirement
    [Serializable]
    public struct CrateRequirement
    {
        public CrateTag requiredTag;
        public int requiredCount;
    }

    // Struct defining a spawn node; a transform for where to spawn and a tag for its spawned object
    // This class also uses the transform's children as points
    [Serializable]
    public struct SpawnNode
    {
        public Transform transform;
        public CrateTag tag;
    }

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
    // FOR THOSE WHO DON'T KNOW you can call this from an instance of a CrateTag (makes calling this function less painfu
    public static Color GetColourFromTag(this CrateTag tag)
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
