using System;
using TMPro;
using UnityEngine;


public class RequirementsText : MonoBehaviour 
{
    [SerializeField]
    TextMeshProUGUI display;

    [SerializeField, TextArea]
    string displayText = "Crates Required\r\n";

    private void Awake()
    {
        HideText();
    }

    // This can be assigned to an event to update the displayed score
    public void UpdateText(CrateExtensions.CrateRequirement requirement)
    {
        string tag = Enum.GetName(typeof(CrateExtensions.CrateTag), requirement.requiredTag);
        display.SetText($"{displayText}{tag} : {requirement.requiredCount}");
    }

    // Shows the text box
    public void ShowText()
    {
        gameObject.SetActive(true);
    }

    // 
    public void HideText()
    {
        gameObject.SetActive(false);
    }
}
