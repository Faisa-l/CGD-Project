using TMPro;
using UnityEngine;


public class RequirementsText : MonoBehaviour 
{
    [SerializeField]
    TextMeshProUGUI display;

    // [SerializeField, TextArea]
    // string displayText = "Crates Required\r\n";

    CrateExtensions.TimedCrateRequirement requirement;
    float trackedScore;

    string DisplayedText => $"Crate: {requirement.requiredTag}\r\nQuota: {trackedScore}/{requirement.requiredScore}";

    private void Awake()
    {
        trackedScore = 0;
        HideText();
    }

    // Assigned to event -> Update displated requirement and reset tracked score
    public void OnRequirementUpdated(CrateExtensions.TimedCrateRequirement requirement)
    {
        this.requirement = requirement;
        trackedScore = 0f;
        display.SetText(DisplayedText);
    }

    // Assigned to event -> Display the score when current collection score updates
    public void OnCollectionScoreUpdated(float score)
    {
        trackedScore = score;
        display.SetText(DisplayedText);

    }

    // Shows the text box
    public void ShowText() => gameObject.SetActive(true);

    // Hide the text box
    public void HideText() => gameObject.SetActive(false);
    
}
