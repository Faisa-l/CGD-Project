using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToastNotificationManager : MonoBehaviour
{
	[Header("Cache")]
	[SerializeField] private TMP_Text titleText;
	[SerializeField] private TMP_Text descriptionText;
	[SerializeField] private Animator anim;
	
	// Singleton
	private static ToastNotificationManager instance;
	
	private void Start()
	{
		// Singleton
		if (instance)
			Destroy(gameObject);
		else
			instance = this;
	}
	
	private void Show(string title, string description)
	{
		titleText.text = title;
		descriptionText.text = description;
		
		if (anim)
			anim.SetTrigger("Show");
		else
			Debug.LogWarning("Toast Notification Manager Animator hasn't been set");
	}
}