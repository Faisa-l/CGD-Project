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
    RectTransform rectTransform;

    [SerializeField]
    float margin = 15f;

    [SerializeField, Range(1f, 100f), Tooltip("Ratio between world distance and map distance.")]
    float scaleRatio = 1f;

    [SerializeField]
    Color[] playerColors;

    [SerializeField, Tooltip("Represents the position of the top-left corner of the minimap in world space")]
    Transform pointOfReference;
    
    float width;
    float height;
    List<GameObject> icons;
    List<MinimapPlayerIcon> playerIcons;

    // This shouldn't be here
    bool wasLaunchedInDebug = false;

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
        playerIcons = new List<MinimapPlayerIcon>();
        width = rectTransform.rect.width;
        height = rectTransform.rect.height;
        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        UpdateMinimapPlayerIcons();
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

        GameObject icon = Instantiate(iconPrefab, this.transform);
        icon.GetComponent<UnityEngine.UI.Image>().color = playerColors[playerCount - 1];
        icons.Add(icon);

        var mpi = new MinimapPlayerIcon
        {
            transform = transform,
            gameObject = icons[playerCount - 1]
        };
        playerIcons.Insert(playerCount - 1, mpi);

        RepositionPanel(playerCount);
    }

    // Adjusts the position of each minimap player icon
    void UpdateMinimapPlayerIcons()
    {
        foreach (var mpi in playerIcons)
        {
            Vector3 relativePosition = mpi.transform.position - pointOfReference.position;
            Vector2 UIPosition = new(relativePosition.x, relativePosition.z);
            float worldRotation = mpi.transform.rotation.eulerAngles.y;
            Vector3 UIRotation = mpi.RectTransform.rotation.eulerAngles;
            UIRotation.z = -worldRotation;
            UIPosition *= scaleRatio;

            mpi.RectTransform.localPosition = UIPosition;
            mpi.RectTransform.localRotation = Quaternion.Euler(UIRotation);
        }
    }

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
    // Can reference this struct through MinimapPanel.MinimapPlayerIcon
    public struct MinimapPlayerIcon
    {
        public Transform transform;
        public GameObject gameObject;
        public RectTransform RectTransform => gameObject.GetComponent<RectTransform>();
    }
}
