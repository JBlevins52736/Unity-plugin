using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneCamera : MonoBehaviour
{ 
    [Header("Target Settings")]
    [SerializeField] private Transform target;

    [Header("Camera Settings")]
    [SerializeField] private float followDistance = 10f;
    [SerializeField] private float heightOffset = 5f; 
    [SerializeField] private float followSpeed = 5f; 
    [SerializeField] private float rotationSpeed = 5f;

    void FixedUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("CameraFollow: No target assigned to follow.");
            return;
        }

        Vector3 direction = -target.forward;
        Vector3 desiredPosition = target.position + direction * followDistance + Vector3.up * heightOffset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * followSpeed);

        Quaternion desiredRotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * rotationSpeed);

    }
}
