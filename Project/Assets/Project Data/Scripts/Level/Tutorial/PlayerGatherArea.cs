using UnityEngine;
using UnityEngine.Events;

public class PlayerGatherPoint : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField] private int requiredPlayerCount = 4;
	[SerializeField] private UnityEvent onPlayerCountReached = new UnityEvent();
	
	private int playerCount;
	
    void OnTriggerEnter(Collider other)
    {
        // Is it a player?
		if (other.CompareTag("Player"))
		{
			playerCount++;
			
			// Has the player count been reached?
			if (playerCount >= requiredPlayerCount)
			{
				onPlayerCountReached?.Invoke();
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