using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HudManager : MonoBehaviour
{
	[Header("Enter Vehicle")]
	[SerializeField] private GameObject[] enterVehiclePrompts; // 0 = unused. 1 = player 1 etc.
	[SerializeField] private Image[] enterVehicleProgressSliders; // 0 = unused. 1 = player 1 etc.
	[SerializeField] private TMP_Text[] enterVehiclePromptTexts; // 0 = unused. 1 = player 1 etc.
	
	public void SetVehiclePromptStatus(int playerNumber, bool newStatus)
	{
		if (playerNumber <= 0)
			return;
		
		enterVehiclePrompts[playerNumber].SetActive(newStatus);
	}
	
	public void SetVehiclePromptProgress(int playerNumber/*, float amount*/)
	{
		if (playerNumber <= 0)
			return;
		
		//enterVehicleProgressSliders[playerNumber].fillAmount = amount;
	}
	
	public void SetVehiclePromptText(int playerNumber, string newText)
	{
		if (playerNumber <= 0)
			return;
		
		enterVehiclePromptTexts[playerNumber].text = newText;
	}
}