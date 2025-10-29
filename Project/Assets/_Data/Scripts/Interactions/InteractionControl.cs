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
        float interactDistance = 3f;

        Interactable currentTargetedInteraction;

        public void Update()
        {
            UpdateCurrentInteratable();

            UpdateInteractText();

            CheckForInput();
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

        void CheckForInput()
        {
           if (Input.GetKey(KeyCode.E) && currentTargetedInteraction != null)
           {
               currentTargetedInteraction.Interact(this);
           }
        }
    }
}
