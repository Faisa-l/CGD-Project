
using UnityEngine;

public class FloatPickup : MonoBehaviour
{
   [SerializeField]
   GameObject Forkcast;

    [SerializeField] float force_multiplier;
    [SerializeField] float force_exponent;
    [SerializeField] float added_force;
    [SerializeField] float ray_dist;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Forkcast.transform.forward, out hit, ray_dist))
        {
            if (hit.collider.tag != "Float")
                return;
            
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.green);
            //Debug.Log("Hit");
            // Debug.Log(hit.transform.name);

            if (hit.collider.tag == "Float")
            {
                hit.rigidbody.freezeRotation = true;

                RotatetoLift(hit);
                hit.rigidbody.AddForce(Vector3.up * (Floatforce(hit.transform.position.y) - hit.rigidbody.GetAccumulatedForce().y));
            }

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
}
 