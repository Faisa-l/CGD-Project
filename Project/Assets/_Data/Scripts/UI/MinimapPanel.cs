using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles positioning the minimap relative to how many players are on the screen
/// </summary>
public class MinimapPanel : MonoBehaviour
{
    [SerializeField]
    GameObject iconPrefab;

    [SerializeField]
    Sprite playerIcon;

    [SerializeField]
    RectTransform rectTransform;

    [SerializeField]
    float margin = 15f;

    [SerializeField, Tooltip("Ratio between world distance and map distance.")]
    Vector2 scaleRatio;

    [SerializeField]
    Color[] playerColors;

    [SerializeField, Tooltip("Position the minimap will use to place icons on its map. Represents its centre.")]
    Transform pointOfReference;
    
    float width;
    float height;
    List<MinimapIcon> icons;
    List<MinimapIcon> playerIcons;

    // List of all points of interest this minimap should load
    static readonly List<MinimapPointOfInterest> POIs = new();

    // Any script that wants to register themselves as a POI should call this function 
    public static void RegisterPointOfInterest(MinimapPointOfInterest poi) => POIs.Add(poi);

    private void OnValidate()
    {
        if (!TryGetComponent<RectTransform>(out rectTransform))
        {
            Debug.LogWarning("Minimap panel has no RectTransform (somehow?)");
        }

        if (playerColors.Length < 4)
        {
            Debug.LogWarning("MinimapPanel: playerColors may not have enough values for all players");
        }
    }

    private void Awake()
    {
        icons = new List<MinimapIcon>();
        playerIcons = new List<MinimapIcon>();

        width = rectTransform.rect.width;
        height = rectTransform.rect.height;
        gameObject.SetActive(false);
    }

    // THIS IS TECHNICALLY TEMPORARY SEE THE COMMENT ON Initialise() FOR A REASON
    private void Start()
    {
        Initialise();
    }

    private void FixedUpdate()
    {
        UpdateMinimapPlayerIcons();
    }

    // Ideally something else would call this function (like a hud manager, but I'm scared of editing that)
    public void Initialise()
    {
        AddPointsOfInterest();
    }

    // Repositions the panel based on the player count
    public void RepositionPanel(int playerCount)
    {
        Debug.Log("Repositioning panel for players: " +  playerCount);
        gameObject.SetActive(true);

        switch (playerCount)
        {
            // Sets position to right corner of the screen
            case 1:
                AnchorRightCorner();
                break;

            // Sets position to bottom centre of the screen
            case 2:
                AnchorMiddleCenter();
                break;

            // Sets position to center of the screen
            case 3:
                AnchorMiddleCenter();
                break;

            case 4:
                AnchorMiddleCenter();
                break;

            default:
                AnchorMiddleCenter();
                break;
        }
    }

    // Should fire whenever a player joins
    // Instantiate and add their corresponding forklift transform to the icons list
    public void AddPlayerIcon(int playerCount, Transform transform)
    {
        // TODO: replace null with playerIcon
        var playerIcon = CreateMinimapIcon(playerColors[playerCount - 1], this.playerIcon, transform);
        playerIcons.Add(playerIcon);

        RepositionPanel(playerCount);
    }

    // Adds points of interests to the minimap
    void AddPointsOfInterest()
    {
        foreach (var poi in POIs)
        {
            var icon = CreateMinimapIcon(poi.color, poi.sprite, poi.gameObject.transform);
            icon.RectTransform.localPosition = GetMinimapPosition(icon.worldTransform);
        }

        // KILL all POIs (they are already loaded)
        POIs.Clear();
    }

    // Adjusts the position of each minimap player icon
    void UpdateMinimapPlayerIcons()
    {
        foreach (var mpi in playerIcons)
        {
            mpi.RectTransform.localPosition = GetMinimapPosition(mpi.worldTransform);
            mpi.RectTransform.localRotation = GetMinimapRotation(mpi.worldTransform, mpi.RectTransform);
        }
    }

    #region Utility functions

    // Create a minimap icon with a provided colour and sprite
    MinimapIcon CreateMinimapIcon(Color color, Sprite sprite, Transform transform)
    {
        var mapIcon = new MinimapIcon
        {
            worldTransform = transform,
            gameObject = Instantiate(iconPrefab, this.transform)
        };

        var image = mapIcon.gameObject.GetComponent<UnityEngine.UI.Image>();
        image.sprite = sprite ? sprite : null;
        image.color = color;
        icons.Add(mapIcon);

        return mapIcon;
    }

    // Returns the position in the minimap for a given transform
    Vector2 GetMinimapPosition(Transform transform)
    {
        Vector3 relativePosition = transform.position - pointOfReference.position;
        Vector2 UIPosition = new(relativePosition.x, relativePosition.z);
        UIPosition.Scale(scaleRatio);

        return UIPosition;
    }

    // Returns the Y rotation of from as a Z rotation for to (figure out what this means in your own time)
    Quaternion GetMinimapRotation(Transform from, Transform to)
    {
        float worldRotation = from.rotation.eulerAngles.y;
        Vector3 UIRotation = to.rotation.eulerAngles;
        UIRotation.z = -worldRotation;

        return Quaternion.Euler(UIRotation);
    }

    #endregion


    #region Anchor presets
    void AnchorRightCorner()
    {
        rectTransform.anchorMin = new Vector2(1, 0);
        rectTransform.anchorMax = new Vector2(1, 0);
        rectTransform.anchoredPosition = new Vector2((-width / 2) - margin, (height / 2) + margin);
    }

    // Silly american spelling... 'Center'...
    void AnchorBottomCenter()
    {
        rectTransform.anchorMin = new Vector2(0.5f, 0);
        rectTransform.anchorMax = new Vector2(0.5f, 0);
        rectTransform.anchoredPosition = new Vector2(0, (height / 2) + margin);
    }

    void AnchorMiddleCenter()
    {
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = new Vector2(0, 0);
    }
    #endregion

    // Struct represents the player's icon in the minimap
    // Can reference this struct through MinimapPanel.MinimapIcon
    public struct MinimapIcon
    {
        public Transform worldTransform;
        public GameObject gameObject;
        public readonly RectTransform RectTransform => gameObject.GetComponent<RectTransform>();
    }

    // Struct represents any type of point of interest
    // Can reference this struct through MinimapPanel.MinimapPointOfInterest
    public struct MinimapPointOfInterest
    {
        public Sprite sprite;
        public Color color;
        public GameObject gameObject;
    }
}
