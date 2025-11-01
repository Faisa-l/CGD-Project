using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MonoBehaviour that uses the GameObject's collider to detect and collect crates.
/// </summary>
public class CrateCollector : MonoBehaviour
{
    [SerializeField, Range(0f, 60f)]
    float collectionInterval = 30f;

    [SerializeField]
    Color activeColor = Color.green;

    [SerializeField]
    Color inactiveColor = Color.red;

    float timer = 0f;
    bool canCollect = false;
    List<ICollectable> toCollect;
    Material material;

    void initialise()
    {
        if (!TryGetComponent<Collider>(out Collider collectorCollider))
        {
            Debug.LogWarning("Collector is missing a collider.");
        }

        if (TryGetComponent<Renderer>(out Renderer renderer))
        {
            material = renderer.sharedMaterial;
            material.SetColor("_BaseColor", activeColor);
        }
    }

    private void OnValidate()
    {
        initialise();
    }

    private void Awake()
    {
        initialise();
        toCollect = new List<ICollectable>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // If collider is a 'collectable'
        if (other.TryGetComponent<ICollectable>(out ICollectable collectable))
        {
            // CollectCrate(collectable);
            toCollect.Add(collectable);
            Debug.Log("Added object");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<ICollectable>(out ICollectable collectable))
        {
            if (toCollect.Contains(collectable))
            {
                toCollect.Remove(collectable);
                Debug.Log("removed object");

            }
        }
    }

    private void Update()
    {
        UpdateTimer();
        AdjustMaterial();
        HandleCollection();
    }

    // Collects the crate (removes the object and adds some score)
    void CollectCrate(ICollectable collectable)
    {
        // Do something with its data
        Debug.Log("Score from crate: " + collectable.Score);
        Destroy(collectable.GameObject);
    }

    // Handle timer
    private bool UpdateTimer()
    {
        timer += Time.deltaTime;
        if (timer < collectionInterval) return false;
        canCollect = true;
        return true;
    }

    // Collects items in toCollect if canCollect is true
    private void HandleCollection()
    {
        if (!canCollect) return;
        if (toCollect.Count == 0) return;
        
        foreach (ICollectable item in toCollect)
        {
            CollectCrate(item);
        }
        toCollect.Clear();
        timer = 0f;
        canCollect = false;
        
    }

    // Change material on object based on canCollect state
    private void AdjustMaterial()
    {
        if (canCollect)
        {
            material.SetColor("_BaseColor", activeColor);
        }
        else
        {
            material.SetColor("_BaseColor", inactiveColor);
        }
    }
}
