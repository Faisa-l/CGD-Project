using UnityEngine;

/// <summary>
/// An interface which can designate objects as being collectable (crates and such).
/// If we want collectables to hold some data, implement it here.
/// </summary>
public interface ICollectable
{
    float Score { get; set; }
    GameObject GameObject { get; }
    CrateExtensions.CrateTag Tag { get; set; }

}
