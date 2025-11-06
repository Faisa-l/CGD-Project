using System;
using System.Security.Cryptography.X509Certificates;
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

        public virtual void Interact(InteractableControl interactableControl)
        {
            ConsolePrint();
            Destroy(gameObject);         
        }
    }
}
