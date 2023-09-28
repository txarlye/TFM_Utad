using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 

using UnityEngine;

public class AirplanePhysics : MonoBehaviour
{
    [Header("Characteristics Properties")]
    public float maxMPH = 110f;
    public float maxLiftPower = 800f;
    public float dragFactor = 0.01f;
    public float maxAltitude = 5000f;

    private Rigidbody rb;

    public void Initialize(Rigidbody rigidbody)
    {
        rb = rigidbody;
    }

    public void ApplyPhysics(float angleOfAttack)
    {
        CalculateDrag();
        CalculateLift(angleOfAttack);
    }

    private void CalculateDrag()
    {
        float speedDrag = rb.velocity.magnitude * dragFactor;
        rb.drag = speedDrag;
    }

    private void CalculateLift(float angleOfAttack)
    {
        Vector3 liftDir = transform.up;
        float liftPower = angleOfAttack * maxLiftPower;
        Vector3 finalLiftForce = liftDir * liftPower;
        rb.AddForce(finalLiftForce);
    }

    public void LimitAltitude()
    {
        Vector3 position = transform.position;
        if (position.y > maxAltitude)
        {
            position.y = maxAltitude;
            transform.position = position;
        }
    }
}


