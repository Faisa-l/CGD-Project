using UnityEngine;

public class AutoLoadScene : LoadScene
{
	[Header("Settings")]
	[SerializeField] private float delay;
	
	private void Start()
	{
		LoadWithDelay(delay);
	}
}