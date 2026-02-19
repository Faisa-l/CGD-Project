using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class CratePickUp : MonoBehaviour
{
    [SerializeField] GameObject PickupLocation;
    [SerializeField] Vector3 pickupPositionOffset;


    [SerializeField] List<GameObject> pickupList = new();
    [SerializeField] int maxObjects;
    [SerializeField] List<GameObject> heldObjects = new();


    //[SerializeField] bool forkLiftSelected;

    [SerializeField] bool liftFull;


    public UnityEvent onGrabbed = new UnityEvent();
    public UnityEvent onDropped = new UnityEvent();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        liftFull = heldObjects.Count == maxObjects;

        if (pickupList.Count == 0)
            return;

        for (int i = 0; i < pickupList.Count; i++)
        {
            for (int j = 0; j < heldObjects.Count; j++)
            {
                if(heldObjects[j] == pickupList[i])
                {
                    pickupList.RemoveAt(i);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Float")
        {
            Debug.Log("Crate in Pickup Radius");
            pickupList.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        pickupList.Remove(other.gameObject);
    }

    public void PickUpSelected()
    { 
        if(pickupList.Count == 0 && heldObjects.Count == 0)
            return;


        if (!liftFull && pickupList.Count > 0)
        {
            heldObjects.Add(pickupList[0]);          

            var heldCount = heldObjects.Count - 1;

            SetPositionInParent(heldObjects[heldCount].gameObject.transform, heldCount);

            if (heldObjects[heldCount].TryGetComponent<PhysicsPickup>(out var pickup))
            {
                Debug.Log("Invoking Pickup");
                pickup.OnGrabbed.Invoke();
                onGrabbed?.Invoke();
                pickupList.RemoveAt(0);
            }
        }
        else if(pickupList.Count == 0)
        {
            if(heldObjects.Count == 0)
                return ;
            else if (heldObjects.Count > 0)
            {
                if(heldObjects[0].TryGetComponent<PhysicsPickup>(out var pickup))
                {
                    Debug.Log("Invoking Drop");
                    pickup.OnDropped.Invoke();

                    onDropped?.Invoke();
                }
                var heldCount = heldObjects.Count - 1;
                UnsetPositionInParent(heldObjects[heldCount].gameObject.transform, heldCount);
                heldObjects.Remove(heldObjects[heldCount]);
                heldCount--;
            }
        else if(liftFull)
            {
                if (heldObjects[0].TryGetComponent<PhysicsPickup>(out var pickup))
                {
                    Debug.Log("Invoking Drop");
                    pickup.OnDropped.Invoke();

                    onDropped?.Invoke();
                }
                var heldCount = heldObjects.Count - 1;
                UnsetPositionInParent(heldObjects[heldCount].gameObject.transform, heldCount);
                heldObjects.Remove(heldObjects[heldCount]);
                heldCount--;
            }

        }
    }

    public void SetPositionInParent(Transform newPosition, int heldcount)
    {
        
            pickupPositionOffset = new Vector3(0, heldObjects[0].transform.position.y * heldcount, 0);

            newPosition.parent = PickupLocation.transform;
            newPosition.transform.position = PickupLocation.transform.position + pickupPositionOffset;
            newPosition.transform.rotation = PickupLocation.transform.rotation;
            newPosition.GetComponent<Rigidbody>().isKinematic = true;

    }

    public void UnsetPositionInParent(Transform newPosition, int heldcount)
    {
        newPosition.parent = null;
        newPosition.GetComponent<Rigidbody>().isKinematic = false;

        newPosition.GetComponent<Collider>().enabled = true;
    }

    //public void PickUpSelectedForklift()
    //{
    //    if (forklift_selected == true && !has_object && has_forklift == false)
    //    {
    //        Debug.Log($"Forklift Interaction with {hit.collider.name}");
    //
    //        GameObject lifting_forklift = hit.collider.gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.parent.gameObject;
    //        lifting_forklift.GetComponent<Collider>().enabled = false;
    //
    //        if (hit.collider.tag == "LeftSide")
    //        {
    //            SetForkliftPositionInParent(lifting_forklift.transform, ForkliftLeftLocation.transform);
    //            lifting_forklift.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
    //            held_object = lifting_forklift;
    //            has_forklift = true;
    //        }
    //
    //        if (hit.collider.tag == "RightSide")
    //        {
    //            SetForkliftPositionInParent(lifting_forklift.transform, ForkliftRightLocation.transform);
    //            lifting_forklift.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
    //            held_object = lifting_forklift;
    //            has_forklift = true;
    //        }
    //
    //        if (hit.collider.tag == "BackSide")
    //        {
    //            lifting_forklift.transform.rotation = Forkcast.transform.rotation;
    //            SetForkliftPositionInParent(lifting_forklift.transform, ForkliftBackLocation.transform);
    //            held_object = lifting_forklift;
    //            has_forklift = true;
    //        }
    //
    //        if (has_forklift)
    //        {
    //            held_object.GetComponent<DrivingController>().togglePlayerLifted();
    //        }
    //    }
    //    else if (forklift_selected == false && held_object != null && has_forklift == true)
    //    {
    //        //Debug.Log("Dropping Forklift");
    //        ray_dist = 1.5f;
    //        UnsetPositionInParent(held_object.transform);
    //        held_object.GetComponent<DrivingController>().togglePlayerLifted();
    //        has_forklift = false;
    //    }
    //}
}

