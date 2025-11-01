using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.SceneManagement;

public class PlacementArea : MonoBehaviour
{
    class ObjectPosition
    {
        public Vector3 position;
        public bool taken;
    }

    //defining an array the has the positions that each object will take
    List<ObjectPosition> positions = new List<ObjectPosition>();
    int max_length = 27;

    //used to get the index of a place in the positions list
    List<GameObject> objects = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for(int i = 0; i < 27; i++)
        {
            positions.Add(new ObjectPosition());

            positions[i].position = new Vector3(i%3, i/9, (i/3)%3);
            positions[i].taken = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.GetComponentInParent<PhysicsPickup>())
        {
            return;
        }


        for(int i = 0; i < max_length; i++)
        {
            if (!positions[i].taken)
            {
                positions[i].taken = true;
                other.gameObject.transform.position = positionInArea(i, other.gameObject);
                objects.Add(other.gameObject);
                return;
            }
        }
        
    }

    //temporary fix, will probably add function for removing objects from
    //  the area manually and for when they fall off later when we can discuss
    //  how we want to implement it
    private void OnTriggerExit(Collider other)
    {
        //check the object leaving the area is a PhysicsPickup
        if (!other.GetComponentInParent<PhysicsPickup>())
        {
            return;
        }

        int index = -1;
        //if it's a PhysicsPickup object, then get its index in the list and remove it
        for(int i = 0; i < objects.Count; i++)
        {
            if (objects[i] == other.gameObject)
            {
                index = i; 
                break;
            }
        }

        Debug.Log(index);

        objects.RemoveAt(index);

        foreach(ObjectPosition pos in positions)
        {
            pos.taken = false;
        }

        //then go through all the objects and reset their positions
        for(int i = 0; i < objects.Count; i++)
        {
            positions[i].taken=true;
            objects[i].transform.position = positionInArea(i, objects[i]);
        }
    }

    private Vector3 positionInArea(int idx, GameObject current_obj)
    {
        //      position in the area     placement object's position   total scale of the object in its heirarchy   the height needed to get the object out of the floor
        return positions[idx].position + gameObject.transform.position - (gameObject.transform.lossyScale / 2) + new Vector3(0, current_obj.transform.localScale.y / 2, 0);
    }
}
