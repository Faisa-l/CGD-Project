using System;
using UnityEngine;

namespace Interaction
{
    public class CheckBoxPickup : MonoBehaviour, Interactable
    {

        public string MessageInteract => "Press E to pick up the box";


        void ConsolePrint()
        {
            Console.WriteLine("Box picked up!");
        }

        public void Interact(InteractableControl interactableControl)
        {
            ConsolePrint();
            Destroy(gameObject);
        }
    }
}
