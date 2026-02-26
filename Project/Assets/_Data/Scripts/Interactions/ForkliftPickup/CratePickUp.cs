using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;


public class CratePickUp : MonoBehaviour
{
    public enum PickUpDirection {
        None,
        Right,
        Left,
        Forward
    }

    [SerializeField] GameObject PickupLocation;
    [SerializeField] Vector3 pickupPositionOffset;


    [SerializeField] List<GameObject> pickupList = new();

    [SerializeField] List<GameObject> heldObjects = new();

    [SerializeField] int maxObjects;

    [SerializeField] bool forkLiftSelected;

    [SerializeField] bool holdingForklift;

    [SerializeField] bool liftFull;

    [SerializeField] TMP_Text interactionUIText;

    [SerializeField] Transform leftPickUpOffset;

    [SerializeField] Transform rightPickUpOffset;

    [SerializeField] Transform forwardPickUpOffset;


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
        {
            forkLiftSelected = false;
            return;
        }

        if (pickupList[0].tag == "Player")
        {
            forkLiftSelected = true;

        }

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

        if(heldObjects.Count == 0)
            return;

        if (heldObjects[0].gameObject.tag == "Player")
        {
            Debug.Log("Holding Fork");
            holdingForklift = true;
        }
        else
        {
            holdingForklift = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //if the other object is a box and the current player isn't holding a forklift
        if (other.tag == "Float" && !holdingForklift)
        {
            interactionUIText.gameObject.SetActive(true);
            if (heldObjects.Count > 0 && !heldObjects.Contains(other.gameObject))
                interactionUIText.text = "Press <sprite name=\"Xbox_Y\"> to pick up box\nPress <sprite name=\"Xbox_X\"> to drop box";
            else if(!heldObjects.Contains(other.gameObject))
                interactionUIText.text = "Press <sprite name=\"Xbox_Y\"> to pick up box";

            pickupList.Add(other.gameObject);
        }

        //if the other object is a player and the current player isn't holding any boxes
        if(other.tag == "Player" && heldObjects.Count == 0)
        {
            interactionUIText.gameObject.SetActive(true);
            interactionUIText.text = "Press <sprite name=\"Xbox_X\"> to pick up player";

            pickupList.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        pickupList.Remove(other.gameObject);

        if(!holdingForklift && heldObjects.Count == 0)
        {
            interactionUIText.gameObject.SetActive(false);
        }

        if (heldObjects.Count > 0)
            interactionUIText.text = "Press <sprite name=\"Xbox_X\"> to drop box";
        if (holdingForklift)
            interactionUIText.text = "Press <sprite name=\"Xbox_X\"> to drop player";
    }

    public void PickUpSelected()
    { 
        if(forkLiftSelected)
            return;

        if(pickupList.Count == 0 && heldObjects.Count == 0)
            return;
        if(holdingForklift)
            return;
        if (!liftFull && pickupList.Count > 0)
        {
            heldObjects.Add(pickupList[0]);          

            var heldCount = heldObjects.Count - 1;

            SetPositionInParent(heldObjects[heldCount].gameObject.transform, heldCount);

            if (heldObjects[heldCount].TryGetComponent<PhysicsPickup>(out var pickup))
            {
                pickup.OnGrabbed.Invoke();
                onGrabbed?.Invoke();
                pickupList.RemoveAt(0);
            }
        }

        if (heldObjects.Count > 0)
            interactionUIText.text = "Press <sprite name=\"Xbox_X\"> to drop box";
        if (holdingForklift)
            interactionUIText.text = "Press <sprite name=\"Xbox_X\"> to drop player";
    }

    public void PickUpSelectedForklift()
    {
        if(!forkLiftSelected)
            return;

        var Angle = CalculateAngleOfPickup(pickupList[0]);
        if (Angle == PickUpDirection.Left)
        {
            Debug.Log("Left");
            SetForkliftPostitionInParent(Angle);
            pickupList[0].GetComponent<Rigidbody>().useGravity = false;
            //pickupList[0].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            heldObjects.Add((pickupList[0]).gameObject);


        }
        else if (Angle == PickUpDirection.Right)
        {
            Debug.Log("Right");

        }
        else if (Angle == PickUpDirection.Forward)
        {
            Debug.Log("Forward");
        }
        else
        {
            Debug.Log("Invalid");
            return;
        }


        

    }

    public void DropHeld()
    {
        if (heldObjects.Count == 0) return;

        if (heldObjects[0].TryGetComponent<PhysicsPickup>(out var pickup))
        {
            pickup.OnDropped.Invoke();

            onDropped?.Invoke();
        }
        var heldCount = heldObjects.Count - 1;
        UnsetPositionInParent(heldObjects[heldCount].gameObject.transform, heldCount);
        heldObjects.Remove(heldObjects[heldCount]);
        heldCount--;
        

        //Swap Feature (if we have something in our pickup radius and we are holding something, drop what we are holding and pick up the new object) To be added if we feel its needed
        //else if (pickupList.Count > 0)
        //{
        //    if (heldObjects.Count == 0)
        //        return;
        //    else if (heldObjects.Count > 0)
        //    {
        //        if (heldObjects[0].TryGetComponent<PhysicsPickup>(out var pickup))
        //        {
        //            Debug.Log("Invoking Drop");
        //            pickup.OnDropped.Invoke();
        //            onDropped?.Invoke();
        //        }
        //        var heldCount = heldObjects.Count - 1;
        //        UnsetPositionInParent(heldObjects[heldCount].gameObject.transform, heldCount);
        //        heldObjects.Remove(heldObjects[heldCount]);
        //        heldCount--;
        //        PickUpSelected();
        //    }
        //}

    }

    public void SetPositionInParent(Transform newPosition, int heldcount)
    {
        
            pickupPositionOffset = new Vector3(0, heldObjects[0].transform.lossyScale.y * heldcount, 0);

            newPosition.parent = PickupLocation.transform;
            newPosition.transform.position = PickupLocation.transform.position + pickupPositionOffset;
            newPosition.transform.rotation = PickupLocation.transform.rotation;
            newPosition.GetComponent<Rigidbody>().isKinematic = true;

    }

    public void UnsetPositionInParent(Transform newPosition, int heldcount)
    {
        if (heldObjects[0].gameObject.tag == "Player")
        {
            heldObjects[0].GetComponent<Rigidbody>().useGravity = true;
            heldObjects[0].GetComponent <Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            heldObjects[0].GetComponent<DrivingController>().togglePlayerLifted();
        }
        newPosition.parent = null;
        newPosition.GetComponent<Rigidbody>().isKinematic = false;

        newPosition.GetComponent<Collider>().enabled = true;
    }

    public PickUpDirection CalculateAngleOfPickup(GameObject gameObject)
    {
        //Debug.Log("Calculating Angle");

        float sign = Mathf.Sign(Vector3.Dot(transform.forward, gameObject.transform.right));
        float angle = Mathf.Acos(Vector3.Dot(transform.forward, gameObject.transform.forward)) * 180f/Mathf.PI * sign;

       // Debug.Log($"Dot Angle: {angle} Sign: {sign}");

        if((-45 < angle && angle < 45))
        {
            //face front
           // Debug.Log("Forward");
            return PickUpDirection.Forward;
        }

        if(45 < angle && angle < 135)
        {
            //face left
            //Debug.Log("Left");
            return PickUpDirection.Left;
        }

        if(-135 < angle && angle < -45)
        {
            //face right
            //Debug.Log("Right");
            return PickUpDirection.Right;
        }
    
        return PickUpDirection.None;

    }

    public void SetForkliftPostitionInParent(PickUpDirection direction)
    {
        var otherPlayer = pickupList[0].gameObject;
        otherPlayer.GetComponent<DrivingController>().togglePlayerLifted();
        if(direction == PickUpDirection.Left)
        {
            otherPlayer.transform.parent = leftPickUpOffset;
            otherPlayer.transform.position = leftPickUpOffset.position;
            otherPlayer.transform.rotation = leftPickUpOffset.rotation;
        }

    }
}

