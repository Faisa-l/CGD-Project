using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Cheats/Cheat Code")]
public class CheatCode : ScriptableObject
{
	[Header("Settings")]
	[Tooltip("Button inputs required to activate the cheat")]
	[SerializeField] private string[] requiredInputs;
	[Tooltip("Specific functionality that will happen once the code is activated. Seperated so one cheat action can have multiple codes to activate it")]
	[SerializeField] private CheatAction action;
	
	private bool activated = false;
	
	// Prevent activated cheat codes from persisting through gameplay
	private void OnEnable()
    {
        activated = false;
    }
	
	public string GetRequiredInput(int index)
	{
		return requiredInputs[index];
	}
	
	public void Activate()
	{
		// So other scenes can check it's activated (such as gameplay scene)
		activated = true;
		
		action.DoAction();
	}
	
	public bool IsActivated()
	{
		return activated;
	}
}