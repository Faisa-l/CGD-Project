using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public void LoadScene(string sceneToLoad)
	{
		if (sceneToLoad != string.Empty)
			SceneManager.LoadScene(sceneToLoad);
	}
}