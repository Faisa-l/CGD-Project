using UnityEngine;

public interface IDriveable
{
	public bool IsVehicleOccupied();
	public bool TryEnterVehicle(PlayerController player); // bool incase vehicle is full
	public bool TryExitVehicle(); // bool incase no position to exit to (e.g. up against wall)
}