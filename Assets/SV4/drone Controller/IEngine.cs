using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEngine
{
    float MaxPower { get; set; }
    void InitEngine();
    void UpdateEngine(Rigidbody rb, Drone_Inputs input);
}




