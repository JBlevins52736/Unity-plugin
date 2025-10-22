using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class CargoPrefabSetup : MonoBehaviour
{
    
    private bool isPlayerInTriggerZone = false;
    private DroneController droneController;


    void Start()
    {
       
        

        Collider collider = GetComponent<BoxCollider>();
        if (collider != null)
        {
            collider.isTrigger = true; // Ensure the collider is set as a trigger
        }
        else
        {
            //Debug.LogWarning("Cargo prefab requires a Collider component.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject.name);
        if (other.CompareTag("Player"))
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowActionPrompt(true, "Press E to PickUp Cargo");
            }
           // Debug.Log("Drone entered cargo trigger zone.");
            isPlayerInTriggerZone = true;
            droneController = other.transform.parent.GetComponent<DroneController>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager.Instance.ShowActionPrompt(false, "");
            //Debug.Log("Drone exited cargo trigger zone.");
            isPlayerInTriggerZone = false;
            
        }
    }



    private void Update()
    {
        if (isPlayerInTriggerZone && droneController != null && !droneController.HasCargo)
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Joystick1Button2))
            {
                print("pickup");
                GameObject cargo = gameObject;

                // Notify the drone controller to pick up the cargo
                droneController.PickUpCargo(cargo);

                // Notify the WaypointManager that the package was picked up
                if (UIManager.Instance.isRunning == false)
                {
                    UIManager.Instance.StartTimer();
                }

                if (WaypointManager.Instance != null)
                {
                    //Debug.Log("trigger waypoint");
                    WaypointManager.Instance.OnPickup(GetComponentInParent<PickupPoint>());
                }

                // Hide the pickup prompt

                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowActionPrompt(false, "");
                }

               

                // Destroy the cargo at the pickup point
                Destroy(gameObject);
            }
        }
    }
}

