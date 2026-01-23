using System;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

namespace Interaction
{
    public class CheckBoxPickup : MonoBehaviour, Interactable
    {
		[SerializeField]
		private bool requiresForklift = false;

        public string MessageInteract => "Press <sprite name=\"Xbox_X\"> to pick up the box";

        void ConsolePrint()
        {
            Console.WriteLine("Box picked up!");
        }

        public virtual void Interact(InteractableControl interactableControl)
        {
            ConsolePrint();
            Destroy(gameObject);         
        }

        public void Release() { }
		
		public bool RequiresForklift()
		{
			return requiresForklift;
		}
    }
}
