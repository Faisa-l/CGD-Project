using UnityEngine;

public class FloatPickup : MonoBehaviour
{
    [SerializeField] GameObject Forkcast;
    [SerializeField] GameObject PickupLocation;
    [SerializeField] GameObject ForkliftLeftLocation;
    [SerializeField] GameObject ForkliftRightLocation;
    [SerializeField] GameObject ForkliftBackLocation;
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
        if(held_object == null)
        {
            has_object = false;
            ray_dist = 1.5f;
        }

        //Debug.Log(has_forklift);
        //Debug.Log(held_object);
        object_selected = false;
        forklift_selected = false;
        if (Physics.Raycast(Forkcast.transform.position, Forkcast.transform.forward, out hit, ray_dist))
        {
            Debug.DrawRay(
                Forkcast.transform.position, 
                transform.TransformDirection(Vector3.forward) * hit.distance, 
                Color.blue);

            if (hit.collider.tag == "LeftSide")
            {
                Debug.Log("Looking at Left Side");
                
                forklift_selected = true;
                if (has_forklift == true)
                {
                    ray_dist = 0;
                }
            }
            
            if (hit.collider.tag == "BackSide")
            {
                Debug.Log("Looking at Back Side");
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
            Debug.Log($"Forklift Interaction with {hit.collider.name}");

            GameObject lifting_forklift = hit.collider.gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.parent.gameObject;
            lifting_forklift.GetComponent<Collider>().enabled = false;

            if (hit.collider.tag == "LeftSide")
            {
                SetForkliftPositionInParent(lifting_forklift.transform, ForkliftLeftLocation.transform);   
                lifting_forklift.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
                held_object = lifting_forklift;
                has_forklift = true;
            }

            if (hit.collider.tag == "RightSide")
            {
                SetForkliftPositionInParent(lifting_forklift.transform, ForkliftRightLocation.transform);
                lifting_forklift.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
                held_object = lifting_forklift;
                has_forklift = true;
            }

            if (hit.collider.tag == "BackSide")
            {
                lifting_forklift.transform.rotation = Forkcast.transform.rotation;
                SetForkliftPositionInParent(lifting_forklift.transform, ForkliftBackLocation.transform);
                held_object = lifting_forklift;
                has_forklift = true;
            }
        }
        else if (forklift_selected == false && held_object != null && has_forklift == true)
        {
            //Debug.Log("Dropping Forklift");
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

    public void SetForkliftPositionInParent(Transform newPosition, Transform pickUptransform)
    {        
        newPosition.parent = pickUptransform.transform;
        newPosition.transform.position = pickUptransform.transform.position;
        newPosition.GetComponentInParent<Rigidbody>().isKinematic = true;
    }

    public void UnsetPositionInParent(Transform newPosition)
    {
        newPosition.parent = null;
        newPosition.GetComponent<Rigidbody>().isKinematic = false;

        newPosition.GetComponent<Collider>().enabled = true;
    }

    void SetPhysicsValues(GameObject gameobject)
    {
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        gameObject.GetComponent<Collider>().enabled = false;

    }
}
 