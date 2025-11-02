using UnityEngine;
using TMPro;

public class CollectionText : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI display;

    [SerializeField, TextArea]
    string displayText = "Collected crates\r\nTotal score\r\n";

    // This can be assigned to an event to update the displayed text
    public void UpdateText(float score)
    {
        display.SetText(displayText + score);
    }
}
