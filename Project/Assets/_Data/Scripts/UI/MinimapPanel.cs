using UnityEngine;

/// <summary>
/// Handles positioning the minimap relative to how many players are on the screen
/// </summary>
public class MinimapPanel : MonoBehaviour
{
    [SerializeField]
    RectTransform rectTransform;

    [SerializeField]
    float margin = 15f;

    float width;
    float height;


    private void OnValidate()
    {
        if (!TryGetComponent<RectTransform>(out rectTransform))
        {
            Debug.LogWarning("Minimap panel has no RectTransform (somehow?)");
        }
    }

    private void Awake()
    {
        width = rectTransform.rect.width;
        height = rectTransform.rect.height;
        gameObject.SetActive(false);
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
}
