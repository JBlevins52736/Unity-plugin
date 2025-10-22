using System.Drawing;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Rigded_Body_Setup : MonoBehaviour
{
    [SerializeField] private float weightInLbs = 1f;

    const float lbsToKg = 0.454f;

    protected Rigidbody rb;
    protected float startDrag;
    protected float startAngularDrag;

   
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb)
        {
            rb.mass = weightInLbs * lbsToKg;
            startDrag = rb.drag;
            startAngularDrag = rb.angularDrag;
        }
    }

    void FixedUpdate()
    {
        if (!rb)
        {
            return;
        }

        HandlePhysics();
    }

    protected virtual void HandlePhysics() { }
}