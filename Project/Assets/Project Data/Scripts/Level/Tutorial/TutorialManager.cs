using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class TutorialManager : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField] private TutorialStage[] tutorialStages;
	
	[Header("Cache")]
	[SerializeField] private TMP_Text instructionsMessageText;
	
	private int currentTutorialStage = -1;
	private bool tutorialComplete;
	
	private void Start()
	{
		ProgressTutorial();
	}
	
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			ProgressTutorial();
		}
	}
	
	#region Setters
	
	public void ProgressTutorial()
	{
		if (tutorialComplete)
			return;
		
		if (currentTutorialStage >= 0)
			tutorialStages[currentTutorialStage].onCompleted?.Invoke();
		
		currentTutorialStage++;
		
		// Has the tutorial been completed?
		if (currentTutorialStage >= tutorialStages.Length)
		{
			tutorialComplete = true;
			print("Tutorial complete!");
			return;
		}
		
		tutorialStages[currentTutorialStage].onBegun?.Invoke();
		
		// Setup new stage
		instructionsMessageText.text = tutorialStages[currentTutorialStage].instructionsMessage;
	}
	
	#endregion Setters
}

[System.Serializable]
public struct TutorialStage
{
	public string instructionsMessage;
	public UnityEvent onBegun;
	public UnityEvent onCompleted;
}