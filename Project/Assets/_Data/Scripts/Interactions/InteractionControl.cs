using UnityEngine;
using TMPro;

namespace Interaction
{
    public class InteractableControl : MonoBehaviour
    {
        [SerializeField]
        Camera playerCamera;

        [SerializeField]
        TextMeshProUGUI interactText;

        [SerializeField]
        Canvas canvas;

        [SerializeField]
        float interactDistance = 3f;

        Interactable currentTargetedInteraction;

        public void Start()
        {
            interactText.text = "Test";
        }

        public void Update()
        {
            UpdateCurrentInteratable();

            UpdateInteractText();
        }

        void UpdateCurrentInteratable()
        {
            var ray = playerCamera.ViewportPointToRay(new Vector2(0.5f, 0.5f));

            Physics.Raycast(ray, out var hit, interactDistance);

            currentTargetedInteraction = hit.collider?.GetComponent<Interactable>();

        }
        void UpdateInteractText()
        {
            if (currentTargetedInteraction == null)
            {
                interactText.text = string.Empty;
                return;
            }
            interactText.text = currentTargetedInteraction.MessageInteract;
        }
        
        public void OnInteract()
        {
            if (currentTargetedInteraction != null)
                currentTargetedInteraction.Interact(this);
            else
                GetComponent<PickupController>().CheckDropInput();
        }
    }
}
