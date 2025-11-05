using UnityEngine;


/// <summary>
///  This class is mostly for demonstration.
///  Represents a crate that can be collected.
/// </summary>
public class CrateObject : MonoBehaviour, ICollectable
{
    [SerializeField]
    float score;

    public float Score
    {
        get { return score; }
        set { score = value; }
    }

    public GameObject GameObject { get => gameObject; }
}
