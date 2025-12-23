using UnityEngine;

[CreateAssetMenu(fileName = "ScoreObject", menuName = "Scriptable Objects/ScoreObject")]
public class ScoreObject : ScriptableObject
{   
    /// <summary>
    /// Maximum score can obtain in a stage.
    /// </summary>
    public float maxScore = 1000f;

    /// <summary>
    /// Current score held.
    /// </summary>
    public float currentScore = 0f;

    [SerializeField, Range(0f, 1f), Tooltip("Score ranges for each star to be rewarded, as a percentage.")]
    float starRangePercentage = 0.2f;

    /// <summary>
    /// Number of stars based on the current score.
    /// </summary>
    public int Stars => GetStars();

    /* DISCLAIMER READ THIS ABOUT HOW SCORE RANGES WILL WORK:
    * (x / range) is only valid if star ranges are for each 20%.
    * If this were to change than you need to alter this value.
    * If we want to have variable size star ranges then this system would need to change.
    */

    // Returns how many stars there are based on the percentage of maximum score
    private int GetStars()
    {
        float x = currentScore / maxScore;

        if (x < 0.2) 
        { 
            Debug.Log("Score too low!"); 
            return 0; 
        }

        if (x > 1.0) 
        {
            Debug.LogError("Score out of range"); 
            return -1; 
        }

        // Return the range index which x is in
        return Mathf.FloorToInt(x / starRangePercentage);
    }
}
