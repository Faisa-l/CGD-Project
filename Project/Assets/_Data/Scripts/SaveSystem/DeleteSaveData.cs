using UnityEngine;

public class DeleteSaveData : MonoBehaviour
{
	public void Delete()
	{
		SaveManager.instance.ClearSave();
	}
}