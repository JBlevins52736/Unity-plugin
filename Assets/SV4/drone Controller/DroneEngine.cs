using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

[RequireComponent(typeof(BoxCollider))]
public class DroneEngine : MonoBehaviour, IEngine
{
    
    [Header("Engine Properties")]
    [SerializeField] private float maxPower = 6f;

    [Header("Propeller Properties")]
    [SerializeField] private Transform propeller;
   
    private Drone_Inputs input;
    private float currentRotSpeed = 0f;

    void Start()
    {
        input = GetComponentInParent<Drone_Inputs>();
    }
    public float MaxPower
    {
        get => maxPower;
        set
        {
            maxPower = Mathf.Max(0, value); 
         //   Debug.Log($"Max power updated to: {maxPower}");
        }
    }

    public void InitEngine()
    {
        throw new System.NotImplementedException();
    }

    public void UpdateEngine(Rigidbody rb, Drone_Inputs input)
    {
        
        Vector3 upVec = transform.forward;
        upVec.x = 0f;
        upVec.z = 0f;
        float diff = 1 - upVec.magnitude;
        float finalDiff = Physics.gravity.magnitude * diff;

        Vector3 engineForce = Vector3.zero;
        
        Vector3 thrustDirection = transform.forward * Mathf.Sign(input.Throttle);

        // Calculate engine force, scaling by throttle and max power
        engineForce = thrustDirection * ((rb.mass * Physics.gravity.magnitude + finalDiff) + (Mathf.Abs(input.Throttle) * maxPower)) / 4f;

        Debug.DrawLine(transform.position, transform.position + engineForce, Color.red);

        rb.AddForce(engineForce, ForceMode.Force);

        HandlePropellers();
    }



    void HandlePropellers()
    {
        if (!propeller)
        {
            Debug.LogWarning("Propeller is not assigned to the engine.");
            return;
        }

        // Define minimum and maximum rotation speeds
        float minRotSpeed = 1500f; 
        float maxRotSpeed = 5000f;

        // Calculate target rotation speed based on throttle
        float targetRotSpeed = Mathf.Clamp(maxRotSpeed * input.Throttle, minRotSpeed, maxRotSpeed);

        // Adjust acceleration factor for smoother transitions
        float accelerationFactor = Mathf.Abs(targetRotSpeed - currentRotSpeed) > 100f ? 10f : 2f;
        currentRotSpeed = Mathf.Lerp(currentRotSpeed, targetRotSpeed, Time.deltaTime * accelerationFactor);

        // Clamp current speed to stay within min/max limits
        currentRotSpeed = Mathf.Clamp(currentRotSpeed, minRotSpeed, maxRotSpeed);

        // Rotate the propeller
        propeller.Rotate(Vector3.forward, currentRotSpeed * Time.deltaTime);
    }



}