using UnityEngine;

public class AutoRotate : MonoBehaviour
{
	[SerializeField] private Vector3 rotation;
	
	private void Update()
	{
		transform.Rotate(rotation.x * Time.deltaTime, rotation.y * Time.deltaTime, rotation.z * Time.deltaTime, Space.Self);
	}
}