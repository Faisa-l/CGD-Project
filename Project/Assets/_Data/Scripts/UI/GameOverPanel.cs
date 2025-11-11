using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class GameOverPanel : MonoBehaviour
{
	[SerializeField] private GameObject panel;
	[SerializeField] private EventSystem eventSystem;
	[SerializeField] private GameObject firstButton;
	
	private void Start()
	{
		// Subscribe to events
		GameOverState.onEntered += Show;
	}
	
	private void Show()
	{
		panel.SetActive(true);
		
		eventSystem.SetSelectedGameObject(firstButton);
	}
	
	private void Hide()
	{
		panel.SetActive(false);
	}
	
	private void OnDestroy()
	{
		// Unsubscribe to events
		GameOverState.onEntered -= Show;
	}
}