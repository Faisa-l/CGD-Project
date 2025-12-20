using System.Linq;
using TMPro;
using UnityEngine;

/// <summary>
/// Exposes the layout of the victory panel so they can be modified.
/// </summary>
public class VictoryPanelLayout : MonoBehaviour
{
    // IDEALLY THIS IS TEMPORARY
    // Making everything public and hoping none of you will change them outside of this class so there aren't a lot of getters

    /// <summary>
    /// Transform whose children are the icons for displayed stars.
    /// </summary>
    public Transform achievedStars;

    /// <summary>
    /// Transform whose chidlren are the icons for undisplayed stars.
    /// </summary>
    public Transform unachievedStars;

    /// <summary>
    /// Text UI for the object which displays the score.
    /// </summary>
    public TextMeshProUGUI scoreText;

    readonly string scoreFormat = "Score: ";

    private void OnValidate()
    {
        if (achievedStars ==  null)
        {
            Debug.Log("VictoryPanelLayout: achieved stars is not set.");
        }
        if (unachievedStars == null)
        {
            Debug.Log("VictoryPanelLayout: unachieved stars is not set.");
        }
        if (scoreText == null)
        {
            Debug.Log("VictoryPanelLayout: score text is not set.");
        }
    }

    /// <summary>
    /// Updates the score displays with the provided score.
    /// </summary>
    /// <param name="scoreObject"> The object used to update the score with. </param>
    public void UpdateDisplayedScore(ScoreObject scoreObject)
    {
        if (scoreText == null || unachievedStars == null || achievedStars == null) return;

        // Set the score text to the scoreObject's score and display as many stars as were obtained
        Transform[] starIcons = achievedStars.GetComponentsInChildren<Transform>(true).Skip(1).ToArray();
        int stars = scoreObject.Stars - 1;

        scoreText.text = scoreFormat + scoreObject.currentScore;
        for (int i = 0; i < starIcons.Length; i++)
        {
            if (i <= stars)
            {
                starIcons[i].gameObject.SetActive(true);
            }
        }

        // NOTE: the Skip().ToArray() is to ignore this object
    }
}
