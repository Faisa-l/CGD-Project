using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Changeable Variables")]
    public int moveBy;
    public float speed = 1.0f;
    public float delay = 0.0f;

    [Header("Boolean Variables")]
    public bool moving = false;
    public bool opening = true;
    public bool timed = false;
    
    
    
    private Vector3 startPos;
    private Vector3 endPos;
    



    void Start()
    {
        startPos = transform.position;
        endPos = startPos;
        endPos.y += moveBy;
    }

    void Update()
    {
        if (moving)
        {
            if (opening)
            {
                MoveDoor(endPos);
            }
            else
            {
                MoveDoor(startPos);
            }
        }
        
        //if(transform.position == startPos)
        //{
        //    opening = true;
        //}
    }

    void MoveDoor(Vector3 goalPos)
    {
        float dist = Vector3.Distance(transform.position, goalPos);
        if (dist > .1f)
        {
            transform.position = Vector3.Lerp(transform.position, goalPos, speed * Time.deltaTime);
        }
        else
        {
            if (opening && timed)
            {
               delay += Time.deltaTime;
               if (delay > 1.5f)
               {
                   opening = false;
               }
            }
            else
            {
                //moving = false;
                //opening = true;
            }
        }
    }

    public bool Moving
    {
        get { return moving; }
        set { moving = value; }
    }

}

