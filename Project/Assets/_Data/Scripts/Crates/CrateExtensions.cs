using System;
using UnityEngine;

// You can add 'using static CrateExtensions' to use these things in any other class

/// <summary>
///  Collection of various crate extensions (structs, enums, functions, etc.)
/// </summary>
public static class CrateExtensions
{
    // Struct defining the requirements for spawning crates
    [Serializable]
    public struct SpawnRequirements
    {
        [Tooltip("This must be the game object whose child transforms are used as spawn points.")]
        public Transform parentTransform;
        public CrateTag tag;
        public int spawnCount;
        public int crateScore;
    }

    public enum ColorTag { Red, Blue, Green, Null };

    // A CrateRequirement with a time limit
    [Serializable]
    public struct ScheduleQuota
    {
        public ColorTag requiredColor;
        //public CrateTag requiredTag;
        public float requiredScore;
        [Min(0f)]
        public float timeLimit;
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
    public enum CrateTag { Red1, Green1, Blue1, Red2, Green2, Blue2 }

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
            CrateTag.Red1 => Color.red,
            CrateTag.Green1 => Color.green,
            CrateTag.Blue1 => Color.blue,
            CrateTag.Red2 => Color.red,
            CrateTag.Green2 => Color.green,
            CrateTag.Blue2 => Color.blue,
            _ => Color.white,
        };
    }

    public static ColorTag getColorTag(this CrateTag tag)
    {
        return tag switch
        {
            CrateTag.Red1 => ColorTag.Red,
            CrateTag.Green1 => ColorTag.Green,
            CrateTag.Blue1 => ColorTag.Blue,
            CrateTag.Red2 => ColorTag.Red,
            CrateTag.Green2 => ColorTag.Green,
            CrateTag.Blue2 => ColorTag.Blue,
            _ => ColorTag.Null
        };
    }

}
