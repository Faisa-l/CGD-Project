using System;
using Interaction;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;

public class PhysicsPickup : MonoBehaviour, Pickupable
{
    [SerializeField]
    Rigidbody pickupRigidBody;

    [SerializeField]
    Collider pickupCollider;

    [SerializeField]
    Vector3 pickupPositionOffset;

    [SerializeField]
    float impactThreshold = 0.5f;

    [SerializeField]
    UnityEvent onImpactThresholdMet;

    public virtual string MessageInteract => "Press <sprite name=\"Xbox_X\"> to pick up";

    public void Interact(InteractableControl interactableControl)
    {
        var pickupController = interactableControl.GetComponent<PickupController>();

        Grab(pickupController);
    }

    public virtual void Grab(PickupController pickupController)
    {
        if (pickupController == null || pickupController.HasPickupable)
        {
            return;
        }

        pickupController.GrabPickUp(this);

        SetPhysicsValues(true);

    }

    public virtual void Drop(PickupController pickupController)
    {
        transform.parent = null;

        SetPhysicsValues(false);
    }

    public virtual void Place(PickupController pickupController)
    {

        transform.position += transform.parent.forward * 2;
        transform.parent = null;
        SetPhysicsValues(false);
    }

    public virtual void Destroy(PickupController pickupController)
    {
        transform.parent = null;
        pickupController.currentPickupable = null;
        Destroy(gameObject);
    }

    public void SetPositionInParent(Transform newParent)

    {
        transform.parent = newParent;
        transform.localPosition = pickupPositionOffset;
        transform.localRotation = Quaternion.identity;

    }
    public virtual void Use()
    {

        Debug.Log("Using the pickupable object");

    }

    void SetPhysicsValues(bool wasPickedUp)
    {
        pickupRigidBody.isKinematic = wasPickedUp;
        pickupCollider.enabled = !wasPickedUp;
    }
    void OnCollisionEnter(Collision collision)
    {
        Vector3 velocity = collision.relativeVelocity;
        if (velocity.x > impactThreshold || velocity.y > impactThreshold  || velocity.z > impactThreshold)
        {
            onImpactThresholdMet.Invoke();
        }
    }

    public virtual void Release() { }
}
