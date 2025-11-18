
using Interaction;
using UnityEngine;

public class FloatPickup : MonoBehaviour
{
    [SerializeField] GameObject Forkcast;
    [SerializeField] GameObject PickupLocation;
    [SerializeField] GameObject ForkliftLocation;
    [SerializeField] Vector3 pickupPositionOffset;

    [SerializeField] float force_multiplier;
    [SerializeField] float force_exponent;
    [SerializeField] float added_force;
    [SerializeField] float ray_dist;
    [SerializeField] bool object_selected;
    [SerializeField] bool forklift_selected;
    [SerializeField] bool has_object;
    [SerializeField] bool has_forklift;
    [SerializeField] RaycastHit hit;
    [SerializeField] private GameObject held_object;
    float timer = 0;



    public string MessageInteract => "Picks Up";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(has_forklift);
        Debug.Log(held_object);
        object_selected = false;
        forklift_selected = false;
        if (Physics.Raycast(Forkcast.transform.position, Forkcast.transform.forward, out hit, ray_dist))
        {
            if (hit.collider.tag == "LeftSide")
            {
                Debug.Log("Looking at Left Side");
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.blue);
                forklift_selected = true;
                if (has_forklift == true)
                {
                    ray_dist = 0;
                }
            }
            
            if (hit.collider.tag == "BackSide")
            {
                Debug.Log("Looking at Back Side");
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.blue);
                //pickupPositionOffset = new Vector3(0, 0, -1);
                forklift_selected = true;
                if(has_forklift == true)
                {
                    ray_dist = 0;
                }
            }
            
            if (hit.collider.tag == "RightSide")
            {
                Debug.Log("Looking at Right Side");
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.blue);
                forklift_selected = true;

                if (has_forklift == true)
                {
                    ray_dist = 0;
                }
            }

            if (hit.collider.tag != "Float")
                return;
            
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.green);

            if (hit.collider.tag == "Float")
            {
                object_selected = true;
                hit.rigidbody.freezeRotation = true;

                if (!has_object)
                {                   
                    RotatetoLift(hit);
                }
                else if(has_object == true)
                {
                    ray_dist = 0;
                }

                hit.rigidbody.AddForce(Vector3.up * (Floatforce(hit.transform.position.y) - hit.rigidbody.GetAccumulatedForce().y));
            }

        }        
    }

    public void PickUpSelected()
    {
        if (object_selected && !has_object)
        {
            SetPositionInParent(hit.collider.gameObject.transform);
            held_object = hit.collider.gameObject;
            has_object = true;
        }
        else if (object_selected == false && has_object == true)
        {
            ray_dist = 1.5f;
            UnsetPositionInParent(held_object.transform);
            held_object = null;
            has_object = false;
        }
    }

    public void PickUpSelectedForklift()
    {
        if (forklift_selected == true && !has_object && has_forklift == false)
        {
            Debug.Log("Forklift Interaction");
            if (hit.collider.tag == "LeftSide")
            {
                ForkliftLocation.transform.rotation = Quaternion.Euler(0f, -90f, 0f); ;
                SetForkliftPositionInParent(hit.collider.gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.parent.gameObject.transform);   
                held_object = hit.collider.gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.parent.gameObject;
                has_forklift = true;
            }
        }
        else if (forklift_selected == false && held_object != null && has_forklift == true)
        {
            Debug.Log("Dropping Forklift");
            ray_dist = 1.5f;
            UnsetPositionInParent(held_object.transform);
            has_forklift = false;
        }
    }

    float Floatforce(float y)
    {
        float force;
        force =  force_multiplier * Mathf.Exp(-y * force_exponent) + added_force;

        return force;
    }

    void RotatetoLift(RaycastHit hit)
    {
        if (Mathf.Floor(Forkcast.transform.rotation.y*10) != Mathf.Floor(hit.transform.rotation.y*10))
        {
            hit.collider.transform.Rotate(0, 0.1f, 0);
        }
    }

    public void SetPositionInParent(Transform newPosition)

    {
        newPosition.parent = PickupLocation.transform;
        newPosition.transform.position = PickupLocation.transform.position;
        newPosition.transform.rotation = PickupLocation.transform.rotation;
        newPosition.GetComponent<Rigidbody>().isKinematic = true;
    }

    public void SetForkliftPositionInParent(Transform newPosition)
    {        
        newPosition.parent = ForkliftLocation.transform;
        //newPosition.transform.position = ForkliftLocation.transform.position;
        newPosition.transform.rotation = ForkliftLocation.transform.rotation;
        newPosition.GetComponentInParent<Rigidbody>().isKinematic = true;
    }

    public void UnsetPositionInParent(Transform newPosition)

    {
        newPosition.parent = null;
        //newPosition.GetComponent<Rigidbody>().isKinematic = false;
    }

    void SetPhysicsValues(GameObject gameobject)
    {
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        gameObject.GetComponent<Collider>().enabled = false;

    }
}
 