using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField] private TMP_Text text;
	
	public void Setup(string newString, Color colour)
	{
		text.text = newString;
		text.color = colour;
	}
}