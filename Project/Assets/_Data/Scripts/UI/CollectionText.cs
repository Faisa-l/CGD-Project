using UnityEngine;
using TMPro;
using System.Collections;

public class CollectionText : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI display;

    [SerializeField, TextArea]
    string displayText = "Collected crates\r\nTotal score\r\n";

    [SerializeField, Range(0f, 10f)]
    float visbilityDuration = 3f;

    WaitForSeconds interval;

    private void Awake()
    {
        interval = new WaitForSeconds(visbilityDuration);
        gameObject.SetActive(false);
    }

    // This can be assigned to an event to update the displayed text
    public void UpdateText(float score)
    {
        display.SetText(displayText + score);
        gameObject.SetActive(true);
        StartCoroutine(TogglePanelVisibility());
    }

    // Toggles displaying the text after a delay
    IEnumerator TogglePanelVisibility()
    {
        yield return interval;
        gameObject.SetActive(false);
    }
}