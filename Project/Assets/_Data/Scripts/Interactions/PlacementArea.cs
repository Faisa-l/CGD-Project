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
    //defining an array that can hold a 3x3x3 cube of objects
    GameObject[] objects = new GameObject[27];

    //current position of the latest item in the array
    int current_pos = -1;

    //defining an array the has the positions that each object will take
    Vector3[] positions = new Vector3[27];


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for(int i = 0; i < 27; i++)
        {
            positions[i] = new Vector3(i%3, i/9, (i/3)%3);
        }

        //for(int i = 0; i < 27; i++)
        //{
        //    objects[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);

        //    //the nex objects transform = the position in the array it is + the center of the gameobject - half the size of the gameobject + half the height of the placing objects
        //    objects[i].transform.position = positions[i] + gameObject.transform.position - (gameObject.transform.lossyScale/2) + new Vector3(0,objects[i].transform.localScale.y/2,0);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {

        //if the object collider has the physics pickup component, add it to the array
        if(other.GetComponentInParent<PhysicsPickup>())
        {
            current_pos++;
            objects[current_pos] = other.gameObject;
            objects[current_pos].transform.position = positionInArea(current_pos);
        }
    }

    private Vector3 positionInArea(int idx)
    {
        return positions[idx] + gameObject.transform.position - (gameObject.transform.lossyScale / 2) + new Vector3(0, objects[idx].transform.localScale.y / 2, 0);
    }
}
