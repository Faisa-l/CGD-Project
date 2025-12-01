using UnityEngine;


/// <summary>
/// Component representing a point of interest on the minimap.
/// Add as a component to any game object, add the sprite and colour, and this should automatically register
/// itself as a MinimapPointOfInterest for the MinimapPanel.
/// </summary>
public class MinimapPOI : MonoBehaviour
{
    [Tooltip("Sprite to display on the minimap.")]
    public Sprite sprite;

    [Tooltip("Color to apply on sprite.")]
    public Color color = Color.white;

    [Tooltip("Whether this should register as the object enters the scene.")]
    public bool registerOnAwake = false;

    bool registered = false;

    // Add this object as a MinimapPointOfInterest to all MinimapPanel instances
    public void RegisterPOI()
    {
        if (registered) return;

        // Pass extra stuff here eventually
        var poi = new MinimapPanel.MinimapPointOfInterest
        {
            sprite = sprite,
            color = color,
            gameObject = gameObject,
        };

        MinimapPanel.RegisterPointOfInterest(poi);
        registered = true;
    }

    private void Awake()
    {
        if (!registerOnAwake) return;
        RegisterPOI();
    }
}
