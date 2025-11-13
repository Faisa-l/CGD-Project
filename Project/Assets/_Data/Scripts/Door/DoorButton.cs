using UnityEngine;
using UnityEngine.InputSystem;


namespace Interaction
{
    public class DoorButton : MonoBehaviour, Interactable
    {
        public Door door;

        public string MessageInteract => "Hold E to open the door";

        //[SerializeField] InputAction hold_interaction;
        
        
        
        public void Start()
        {

        }

        public virtual void Interact(InteractableControl interactableControl)
        {
            if (door.timed)
            {
                door.moving = true;
            }
            if (!door.timed)
            {
                if (!door.moving)
                {
                    Debug.Log("Openning");
                    door.moving = true;
                }
                else
                {
                    Debug.Log("Closing");
                    door.opening = false;
                }
            }
        }
    }
}