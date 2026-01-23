using StarterAssets;
using UnityEngine;
using UnityEngine.Events;


namespace Interaction
{
    public class DoorButton : MonoBehaviour, Interactable
    {
        public Door door;
        private string isTimed;

        [SerializeField]
        UnityEvent OnButtonPressed;

        public string MessageInteract => isTimed;

        FirstPersonController current_character;
		
		[SerializeField]
		private bool requiresForklift = false;
        
        public void Start()
        {
            if (door.timed)
            {
                isTimed = "Press <sprite name=\"Xbox_X\"> to open door";
            }
            else
            {
                isTimed = "Hold <sprite name=\"Xbox_X\"> to open door";
            }
        }

        public virtual void Interact(InteractableControl interactableControl)
        {
            if(interactableControl.gameObject.GetComponent<PlayerController>().driving)
            {
                return;
            }


            if (door.timed)
            {
                door.opening = true;
                door.moving = true;
            }
            if (!door.timed)
            {
                Debug.Log("Openning");
                current_character = interactableControl.gameObject.GetComponent<FirstPersonController>();
                current_character.enabled = false;
                OnButtonPressed.Invoke();
                door.opening = true;
                door.moving = true;
            }
        }

        public virtual void Release()
        {
            Debug.Log("AAAAAAAAAAAAA");
			
			if (current_character)
			{
				current_character.enabled = true;
				current_character = null;
			}

            if (!door.timed)
            {
                Debug.Log("Closing");
                door.opening = false;
            }
        }
		
		public bool RequiresForklift()
		{
			return requiresForklift;
		}
    }
}