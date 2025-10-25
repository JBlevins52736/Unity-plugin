using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class WaypointManager : MonoBehaviour
{
    [Header("Pickup Point")]
    [SerializeField] private PickupPoint pickupPoint;

    [Header("Drop-Off Zones")]
    [SerializeField] private List<Transform> dropOffLocations; // List of possible drop-off locations
    [SerializeField] private GameObject dropOffZonePrefab; // Drop-off zone prefab

    [Header("Waypoint Settings")]
    [SerializeField] private float waypointRadius = 5f;
    [SerializeField] private Transform dropOffParent;

    public static WaypointManager Instance { get; private set; }

    private DropOffPoint activeDropOffPoint;
    private GameObject currentDropOffZone;
    private Transform lastDropOffLocation;

    public PickupPoint CurrentPickupPoint => pickupPoint.IsActive ? pickupPoint : null;
    public DropOffPoint CurrentDropOffPoint => activeDropOffPoint?.IsActive == true ? activeDropOffPoint : null;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        ActivatePickup();
    }

    public void OnPickup(PickupPoint pickupPoint)
    {
        if (pickupPoint != this.pickupPoint) return;
       
        pickupPoint.Deactivate();
        ActivateRandomDropOff();
    }

    public void OnDropOff(DropOffPoint dropOffPoint)
    {
        if (dropOffPoint != activeDropOffPoint) return;

     
        Destroy(currentDropOffZone);
        activeDropOffPoint = null;
        ActivatePickup();
    }

    private void ActivatePickup()
    {
        pickupPoint.Activate();
       
    }

    private void ActivateRandomDropOff()
 {
        if (currentDropOffZone != null)
        {
            Destroy(currentDropOffZone);
        }

        // Choose a random drop-off location from the list (excluding last used location)
        Transform selectedDropOffLocation;
        do
        {
            selectedDropOffLocation = dropOffLocations[Random.Range(0, dropOffLocations.Count)];
        } while (selectedDropOffLocation == lastDropOffLocation && dropOffLocations.Count > 1);

        lastDropOffLocation = selectedDropOffLocation; 

        // Instantiate the drop-off zone at the selected location
        currentDropOffZone = Instantiate(dropOffZonePrefab, selectedDropOffLocation.position, Quaternion.identity, dropOffParent);

        activeDropOffPoint = currentDropOffZone.GetComponent<DropOffPoint>();
        activeDropOffPoint.Activate();

       
    }

  

    public void UpdateWaypointUI(Transform droneTransform)
    {
        Transform targetWaypoint = null;

        Sprite waypointSprite = null;

        if (CurrentPickupPoint != null)
        {
            targetWaypoint = CurrentPickupPoint.transform;
            waypointSprite = CurrentPickupPoint.PointImage;
        }
        else if (CurrentDropOffPoint != null)
        {
            targetWaypoint = CurrentDropOffPoint.transform;
            waypointSprite = CurrentDropOffPoint.PointImage;
        }

        if (targetWaypoint != null)
        {
            float distance = Vector3.Distance(droneTransform.position, targetWaypoint.position);
            UIManager.Instance.UpdateWaypointUIElements( waypointSprite, distance);
        }
    }
}
