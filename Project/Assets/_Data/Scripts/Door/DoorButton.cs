using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Interaction
{
    public class DoorButton : MonoBehaviour, Interactable
    {
        public Door door;
        private string isTimed;   
     
        public string MessageInteract => isTimed;
        
        
        
        public void Start()
        {
            if (door.timed)
            {
                isTimed = "Press E to open the timed door";
            }
            else
            {
                isTimed = "Hold E to open the door";
            }
        }

        public virtual void Interact(InteractableControl interactableControl)
        {
            if (door.timed)
            {
                door.moving = true;
            }
            if (!door.timed)
            {
                Debug.Log("Openning");
                door.moving = true;
            }
        }

        public virtual void Release()
        {
            if (!door.timed)
            {
                Debug.Log("Closing");
                door.opening = false;
            }
        }
    }
}