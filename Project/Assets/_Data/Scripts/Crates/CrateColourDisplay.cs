using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class CrateColourDisplay : MonoBehaviour
{

    CrateExtensions.ScheduleQuota requirement;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Dictionary<string, Color> colorMap = new Dictionary<string, Color>()
    {
        {"Red", Color.red},
        {"Green", Color.green},
        {"Blue", Color.blue},

        // Add more colors if needed
    };


    public void UpdateColourDisplay()
    {
        string requiredColour = $"{requirement.requiredTag}";
        requiredColour.ToLower();

        if (colorMap.TryGetValue(requiredColour, out Color newColor))
        {
            GetComponent<Renderer>().sharedMaterial.color = newColor;
        }
        else
        {
            Debug.LogWarning("Colour change error in QuotaColourDisplay prefab");
        }
    }
}