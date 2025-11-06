using TMPro;
using UnityEngine;

public class TotalScoreText : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI display;

    [SerializeField, TextArea]
    string displayText = "Score: ";

    // This can be assigned to an event to update the displayed text
    public void UpdateText(float score)
    {
        display.SetText(displayText + score);
    }
}
