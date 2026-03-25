using UnityEngine;
using UnityEngine.Events;

public class PlayerGatherPoint : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField] private int requiredPlayerCount = 4;
	[SerializeField] private UnityEvent onPlayerCountReached = new UnityEvent();
	
	private int playerCount;
	private bool active = true;
	
    void OnTriggerEnter(Collider other)
    {
		// Prevent triggering more than once
		if (!active)
			return;
		
        // Is it a player?
		if (other.CompareTag("Player"))
		{
			playerCount++;
			
			// Has the player count been reached?
			if (playerCount >= requiredPlayerCount)
			{
				onPlayerCountReached?.Invoke();
				
				// Prevent being triggered more than once
				active = false;
			}
		}
    }
	
    void OnTriggerExit(Collider other)
    {
        // Is it a player?
		if (other.CompareTag("Player"))
		{
			playerCount--;
		}
    }
}