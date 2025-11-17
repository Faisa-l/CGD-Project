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
                isTimed = "Press X to open door";
            }
            else
            {
                isTimed = "Hold X to open door";
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