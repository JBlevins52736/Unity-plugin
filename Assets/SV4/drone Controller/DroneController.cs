
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[RequireComponent(typeof(Drone_Inputs))]
public class DroneController : Rigded_Body_Setup
{

    [Header("Control Properties")]
    [SerializeField] private float minMaxPitch = 15f;
    [SerializeField] private float minMaxRoll = 30f;
    [SerializeField] private float yawPower = 4f;
    [SerializeField] private float lerpSpeed = 2f;
    [SerializeField] private float MaxAcclerations= 4f;

    [SerializeField] private List<GameObject> Wings;
    [SerializeField] private float wingTiltAngle = 15f;

    [Header("Cargo Status")]
    public bool HasCargo = false;

    [Header("Waypoint Settings")]
    [SerializeField] private WaypointManager waypointManager;

    

    private Drone_Inputs input;
    private List<IEngine> engines = new List<IEngine>();

    private float finalPitch;
    private float finalRoll;
    private float yaw;
    private float finalYaw;
 

    void Start()
    {
        PlatformController.singleton.Init("COM4", 115200); // Updated from COM7 to COM4 for ESP32 connection
        input = GetComponent<Drone_Inputs>();
        engines = GetComponentsInChildren<IEngine>().ToList<IEngine>();
        
   

    }

    void Update()
    {
        PlatformController.singleton.Pitch = -finalPitch;

        PlatformController.singleton.Roll = finalRoll;

        PlatformController.singleton.Yaw = Mathf.Lerp(PlatformController.singleton.Yaw,input.Pedals * 10 ,0.02f) ;

        AdjustEnginePower();
        
        HandleCargoDrop();

    }
    private void AdjustEnginePower()
    {
        foreach (var engine in engines)
        {
            engine.MaxPower = MaxAcclerations;
        }
        //Debug.Log($"All engines adjusted by {MaxAcclerations} power.");
    }


    public void PickUpCargo(GameObject cargo)
    {
        if (HasCargo)
        {
            Debug.Log("Drone already carrying cargo. Cannot pick up another.");
            return;
        }

        // Attach the cargo to the drone
        HasCargo = true;
        

        Debug.Log($"Cargo {cargo.name} picked up and attached to the drone.");
    }

    // Method to simulate dropping cargo
    public void DropCargo()
    {
        if (!HasCargo)
        {
            Debug.Log("No cargo to drop.");
            return;
        }

        if (WaypointManager.Instance == null || WaypointManager.Instance.CurrentDropOffPoint == null ||
            !WaypointManager.Instance.CurrentDropOffPoint.CanDropCargo())
        {
            Debug.Log("Cannot drop cargo outside a valid drop-off zone.");
            return;
        }

       
        HasCargo = false;
        UIManager.Instance.UpdateDeliverysCount();

        WaypointManager.Instance.OnDropOff(WaypointManager.Instance.CurrentDropOffPoint);
        Debug.Log("Cargo successfully dropped.");
    }
    private void HandleCargoDrop()
    {
        bool canDrop = waypointManager.CurrentDropOffPoint != null && waypointManager.CurrentDropOffPoint.CanDropCargo();

        // Update UI with appropriate message
        if (canDrop)
        {
            UIManager.Instance?.ShowActionPrompt(true, "Press E to Drop Cargo");
        }
        else if (HasCargo)
        {
            UIManager.Instance?.ShowActionPrompt(false);
        }

        if (HasCargo && Input.GetKeyDown(KeyCode.E)|| Input.GetKeyDown(KeyCode.Joystick1Button2))
        {
            if (canDrop)
            {
                DropCargo();
                UIManager.Instance?.ShowActionPrompt(false); // Hide UI after dropping
            }
            else
            {
                Debug.Log("Cannot drop cargo outside a valid drop-off zone.");
            }
        }
    }
    protected override void HandlePhysics()
    {
        HandleEngines();
        HandleControls();
        HandleWings();
    }
    
    protected virtual void HandleEngines()
    {
        //rb.AddForce(Vector3.up * (rb.mass * Physics.gravity.magnitude));
        foreach (IEngine engine in engines)
        {
            engine.UpdateEngine(rb, input);
        }
    }

    protected virtual void HandleWings()
    {

        float pitchTilt = input.Cyclic.y * wingTiltAngle; // Forward/backward tilt
        float rollTilt =input.Cyclic.x * wingTiltAngle; // Left/right tilt
        float yawTilt = input.Pedals * wingTiltAngle;

        foreach (GameObject wing in Wings)
        {
            if (wing != null)
            {
                
                Quaternion targetRotation = Quaternion.Euler(0f, -pitchTilt, 0f);
                 
                wing.transform.localRotation = Quaternion.Lerp(wing.transform.localRotation, targetRotation, Time.deltaTime * lerpSpeed);
            }
        }
    }

    protected virtual void HandleControls()
    {
        float pitch = -input.Cyclic.y * minMaxPitch;
        float roll = input.Cyclic.x * minMaxRoll;
        yaw += input.Pedals * yawPower;

        finalPitch = Mathf.Lerp(finalPitch, pitch, Time.deltaTime * lerpSpeed);
        finalRoll = Mathf.Lerp(finalRoll, roll, Time.deltaTime * lerpSpeed);
        finalYaw = Mathf.Lerp(finalYaw, yaw, Time.deltaTime * lerpSpeed);

        Quaternion rot = Quaternion.Euler(finalPitch, finalYaw, finalRoll);
        rb.MoveRotation(rot);
    }
}
