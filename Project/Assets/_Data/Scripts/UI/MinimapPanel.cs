using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

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

    [SerializeField, Tooltip("Represents the position of the top-left corner of the minimap in world space.")]
    Transform pointOfReference;
    
    float width;
    float height;
    List<GameObject> icons;
    List<MinimapIcon> playerIcons;

    // List of all points of interest this minimap should load
    static readonly List<MinimapPointOfInterest> POIs = new();

    // Any script that wants to register themselves as a POI should call this function
    public static void RegisterPointOfInterest(MinimapPointOfInterest poi)
    {
        POIs.Add(poi);
    }

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
        icons = new List<GameObject>();
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

    private void LateUpdate()
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
    // Add their corresponding forklift transform to the list and instantiate their icon
    public void AddPlayerIcon(int playerCount, Transform transform)
    {
        // TODO: replace null with playerIcon
        CreateMinimapIcon(playerColors[playerCount - 1], null);

        var mpi = new MinimapIcon
        {
            transform = transform,
            gameObject = icons[playerCount - 1]
        };
        playerIcons.Insert(playerCount - 1, mpi);

        RepositionPanel(playerCount);
    }

    // Adds points of interests to the minimap
    void AddPointsOfInterest()
    {
        foreach (var poi in POIs)
        {
            GameObject icon = CreateMinimapIcon(poi.color, poi.sprite);
            icon.GetComponent<RectTransform>().localPosition = GetMinimapPosition(poi.gameObject.transform);
        }

        // KILL all POIs (they are already loaded)
        POIs.Clear();
    }

    // Adjusts the position of each minimap player icon
    void UpdateMinimapPlayerIcons()
    {
        foreach (var mpi in playerIcons)
        {
            mpi.RectTransform.localPosition = GetMinimapPosition(mpi.transform);
            mpi.RectTransform.localRotation = GetMinimapRotation(mpi.transform, mpi.RectTransform);
        }
    }

    #region Utility functions

    // Create a minimap icon with a provided colour and sprite
    GameObject CreateMinimapIcon(Color color, Sprite sprite)
    {
        // TODO: add sprite to image
        GameObject icon = Instantiate(iconPrefab, this.transform);
        icon.GetComponent<UnityEngine.UI.Image>().color = color;
        icons.Add(icon);

        return icon;
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
        public Transform transform;
        public GameObject gameObject;
        public readonly RectTransform RectTransform => gameObject.GetComponent<RectTransform>();
    }

    // Struct represents any type of point of interest
    public struct MinimapPointOfInterest
    {
        public Sprite sprite;
        public Color color;
        public GameObject gameObject;
    }
}
