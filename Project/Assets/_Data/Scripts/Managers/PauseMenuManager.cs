using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuManager : MonoBehaviour
{
	[SerializeField] GameObject pauseMenuPanel;
	
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape) ||
			Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame)
        {
			TogglePause();
        }
    }
	
	public void TogglePause()
	{
		if (GameManager.instance.currentState != GameManager.instance.playingState)
			return;
		
		if (pauseMenuPanel.activeInHierarchy)
			Unpause();
		else
			Pause();
	}
	
	public void Pause()
	{
		Time.timeScale = 0.0f;
		pauseMenuPanel.SetActive(true);
	}
	
	public void Unpause()
	{
		Time.timeScale = 1.0f;
		pauseMenuPanel.SetActive(false);
	}
}