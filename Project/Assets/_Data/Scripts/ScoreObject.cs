using UnityEngine;

[CreateAssetMenu(fileName = "ScoreObject", menuName = "Scriptable Objects/ScoreObject")]
public class ScoreObject : ScriptableObject
{
    public float maxScore = 1000f;
    public float currentScore = 0f;
}
