using TMPro;
using UnityEngine;

public class RequirementsText : MonoBehaviour 
{
    [SerializeField]
    TextMeshProUGUI display;

    [SerializeField, TextArea]
    string displayText = "Crates Required\r\n";

    // This can be assigned to an event to update the displayed score
    public void UpdateText(int requirement)
    {
        display.SetText(displayText + requirement);
    }
}
