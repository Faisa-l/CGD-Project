using UnityEngine;

public class MovingObject : MonoBehaviour
{

    public int speed;
    public float switchTime = 2f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        switchTime -= Time.deltaTime;
        if (switchTime <= 0f)
        {
            speed = -speed;
            switchTime = 2f;
        }
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}
