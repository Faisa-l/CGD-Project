using UnityEngine;
using UnityEngine.Rendering;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform followingObject;
    [SerializeField] Transform lookingAtObject;

    [Range(0f, 1f)]
    [SerializeField] float lerpValue = 0.01f;
    [Range(0f,1f)]
    [SerializeField] float lookLerpValue = 0.5f;

    [SerializeField] Camera cameraObject;

    [SerializeField] float fovLerpValue = 0.1f;

    Transform currentlyLookingAt;

    public float startingFov 
    {
        get;
        private set;
    }

    public float fov
    {
        get { return cameraObject.fieldOfView; }
        set 
        {
            cameraObject.fieldOfView = value; 
        }  
    }
    public void resetFOV()
    {
        fov = Mathf.Lerp(fov, startingFov, fovLerpValue);
    }

    private void Awake()
    {
        startingFov = fov;

        currentlyLookingAt = new GameObject().transform;
        currentlyLookingAt.position = lookingAtObject.position;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(
            transform.position,
            followingObject.position,
            lerpValue * (1 + Vector3.Distance(transform.position, followingObject.position)));

        float distDifference =
            Vector3.Distance(transform.position, lookingAtObject.position) -
            Vector3.Distance(followingObject.position, lookingAtObject.position);

        Vector3 direction = (lookingAtObject.position - transform.position).normalized;
        direction.y = 0;
        transform.localPosition += direction * distDifference;
        
        currentlyLookingAt.position = Vector3.Lerp(
            currentlyLookingAt.position,
            lookingAtObject.position,
            lookLerpValue);

        transform.LookAt(currentlyLookingAt.position);
    }

    public void setFollowing(Transform pos)
    {
        followingObject = pos;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(currentlyLookingAt.position, 0.3f);
    }
}
