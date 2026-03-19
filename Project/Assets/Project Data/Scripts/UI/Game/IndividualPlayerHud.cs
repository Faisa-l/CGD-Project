using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IndividualPlayerHud : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField] private int playerNumber;
	
	[Header("Lift")]
	[SerializeField] private GameObject upArrow;
	[SerializeField] private GameObject downArrow;
	[SerializeField] private Animator upAnim;
	[SerializeField] private Animator downAnim;
	
	[Header("Crates")]
	[SerializeField] private GameObject[] crates;
	[SerializeField] private Image[] crateImages;
	[SerializeField] private TMP_Text[] cratesText;
	
	// Cache
	private CratePickUp cratePickUp;
	
	private void Start()
	{
		StartCoroutine(FindPlayer());
	}
	
	private void Update()
	{
		UpdateCrates();
	}
	
	private IEnumerator FindPlayer()
	{
		// Delay searching until after the forklifts are created
		yield return new WaitForSeconds(0.1f);
		
		// Find the associated player so we can subscribe to its events
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		
		// Find which player has the name matching the player number
		foreach (var player in players)
		{
			if (player.name.Contains(playerNumber.ToString()))
			{
				Setup(player.GetComponent<DrivingController>());
				break;
			}
		}
	}
	
	#region Setters
	
	public void Setup(DrivingController player)
	{
		if (!player)
			return;
	
		// Subscribe to the player's events
		
		// Fork arm lifting
		player.onLift.AddListener(OnLift);
		
		// Crate pickup
		
		// Find the script
		cratePickUp = player.transform.GetComponentInChildren<CratePickUp>();
		
		// Subscribe to crate interaction events
		cratePickUp.onGrabbed.AddListener(OnGrabbed);
		cratePickUp.onDropped.AddListener(OnDropped);
	}
	
	private void UpdateCrates()
	{
		if (!cratePickUp)
			return;
		
		// Hide all crate icons
		foreach (var crate in crates)
		{
			crate.SetActive(false);
		}
		
		// Show enough crate icons to match held ones
		for(int i = 0; i < cratePickUp.heldObjectsCount; i++)
		{
			// Show crate square
			crates[i].SetActive(true);
		   
			// Show crate score value
			cratesText[i].text = cratePickUp.heldObjects[i].GetComponent<ICollectable>().Score.ToString();
			
			// Match crate icon colour to real crate
			crateImages[i].color = CrateExtensions.GetColourFromTag(cratePickUp.heldObjects[i].GetComponent<ICollectable>().Tag);
		}
	}
	
	#endregion Setters
	
	#region Events
	
	private void OnLift(bool isLifting)
	{
		Debug.LogWarning(isLifting);
		
		// Going up
		if (isLifting)
		{
			upArrow.SetActive(true);
			upAnim.SetTrigger("Flash");
			
			downArrow.SetActive(false);
		}
		// Going down
		else
		{
			downArrow.SetActive(true);
			downAnim.SetTrigger("Flash");
			
			upArrow.SetActive(false);
		}
	}
	
	private void OnGrabbed()
	{
		if (!cratePickUp)
			return;
		
		UpdateCrates();
	}
	
	private void OnDropped()
	{
		if (!cratePickUp)
			return;
		
		UpdateCrates();
	}
	
	#endregion Events
}