using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class StarScore : MonoBehaviour
{
    [SerializeField]
    ScoreObject ScoreObject;

    [SerializeField]
    Slider ScoreSlider;

    [SerializeField]
    GameObject ScoreUpdateUI;

    private float timeCheck;
    public void CheckStarScore()
    {
        float percentage = ScoreObject.maxScore / ScoreObject.CurrentScore;

        if (percentage < 2)
        {
            Debug.Log("Score too low");
        }
        else if (percentage >= 2 && percentage < 4)
        {
            Debug.Log("1 star");
        }
        else if (percentage >= 4 && percentage < 6)
        {
            Debug.Log("2 star");
        }
        else if (percentage >= 6 && percentage < 8)
        {
            Debug.Log("3 star");
        }
        else if (percentage >= 8 && percentage < 10)
        {
            Debug.Log("4 star");
        }
        else if (percentage == 10)
        {
            Debug.Log("5 star");
        }
        else if (percentage > 10)
        {
            Debug.Log("Score too high");
        }
        else
        {
            Debug.Log("Score invalid");
        }
    }

    public void ShowStars()
    {
        ScoreUpdateUI.SetActive(true);
        float percentage = ScoreObject.maxScore / ScoreObject.CurrentScore;
        ScoreSlider.value = percentage;
        timeCheck = Time.time + 8;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > timeCheck)
        {
            ScoreUpdateUI.SetActive(false);
        }
    }
}
