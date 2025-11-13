using UnityEngine;

public class ForkPickUp : MonoBehaviour
{
    [SerializeField]
    GameObject InteractArea;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
       //if (other.gameObject.tag == "Float")
       //{
       //    Debug.Log("Press E to PickUp");
       //}

    }
}
