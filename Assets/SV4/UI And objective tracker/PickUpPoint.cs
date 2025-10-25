using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupPoint : MonoBehaviour
{
    [Header("Pickup Point Settings")]
    [SerializeField] private string pointName; // Name of the pickup point
    [SerializeField] private Sprite pointImage; // Image representing the pickup point
    [SerializeField] private bool isActive = true; // Determines if the pickup point is available

    [Header("Package Settings")]
    [SerializeField] private GameObject packagePrefab; 
    private GameObject spawnedPackage;


    public Sprite PointImage => pointImage;
    public bool IsActive => isActive;

    public void Activate()
    {
        isActive = true;
      

        if (packagePrefab != null && spawnedPackage == null)
        {
            spawnedPackage = Instantiate(packagePrefab, transform.position, Quaternion.identity, transform);
          
        }
    }

    public void Deactivate()
    {
        isActive = false;
 

        if (spawnedPackage != null)
        {
            Destroy(spawnedPackage);
            spawnedPackage = null;
          
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isActive && other.CompareTag("Drone"))
        {
         
            Deactivate(); // Deactivate once picked up
            WaypointManager.Instance.OnPickup(this); // Notify the WaypointManager
        }
    }

    private void OnDrawGizmos()
    {
        if (!isActive)
        {Gizmos.DrawSphere(transform.position, 1f);
        }
        Gizmos.color = Color.blue;
        
    }
}
