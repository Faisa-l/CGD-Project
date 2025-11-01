using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MonoBehaviour that uses the GameObject's collider to detect and collect crates.
/// </summary>
public class CrateCollector : MonoBehaviour
{
    [SerializeField, Range(0f, 60f)]
    float collectionInterval = 30f;

    float timer = 0f;
    bool canCollect = false;
    List<ICollectable> toCollect;
    Material material;

    [SerializeField]
    Color activeColor = Color.green;

    [SerializeField]
    Color inactiveColor = Color.red;

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

    // Collects the crate (removes the object and adds some score)
    void CollectCrate(ICollectable collectable)
    {
        // Do something with its data
        Debug.Log("Score from crate: " + collectable.Score);
        Destroy(collectable.GameObject);
    }

    private void Update()
    {
        AdjustMaterial();

        // Handle timer
        timer += Time.deltaTime;
        if (timer < collectionInterval) return;
        canCollect = true;

        // Do collection + timer reset
        if (toCollect.Count > 0)
        {
            Debug.Log("TIME TO COLLECT!");
            foreach (ICollectable item in toCollect)
            {
                CollectCrate(item);
            }
            toCollect.Clear();
            timer = 0f;
            canCollect = false;
        }
    }

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
