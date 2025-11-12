using UnityEngine;
using UnityEngine.InputSystem;


namespace Interaction
{
    public class DoorButton : MonoBehaviour, Interactable
    {
        [SerializeField] InputAction hold_interaction;

        public Door door;
        public string MessageInteract => "Hold E to open the door";

        public void Start()
        {
            hold_interaction.started += _ => { Debug.Log("Started"); };

            hold_interaction.canceled += _ => { Debug.Log("Cancelled"); };
        }

        public virtual void Interact(InteractableControl interactableControl)
        {
            Debug.Log("Pressed");
            door.moving = true;
        }
    }
}