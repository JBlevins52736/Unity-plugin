using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropOffPoint : MonoBehaviour
{
    [Header("Drop-Off Point Settings")]
  
    [SerializeField] private Sprite pointImage;
    [SerializeField] private bool isActive = false; 

    private bool isDroneInside = false; 


    public Sprite PointImage => pointImage;
    public bool IsActive => isActive;

    public void Activate()
    {
        isActive = true;
        gameObject.SetActive(true);
        //Debug.Log($"{pointName} is now active.");
    }

    public void Deactivate()
    {
        isActive = false;
        gameObject.SetActive(false);
        isDroneInside = false; 
        //Debug.Log($"{pointName} is now inactive.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isActive && other.CompareTag("Player"))
        {
            isDroneInside = true;
          
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isActive && other.CompareTag("Player"))
        {
            isDroneInside = false;
  
        }
    }

    // Method to check if the cargo can be dropped at this drop-off point
    public bool CanDropCargo()
    {
        return isActive && isDroneInside;
    }
}
