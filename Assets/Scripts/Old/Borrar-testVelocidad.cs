using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testVelocidad : MonoBehaviour
{
    public float thrust; //to get the thrust force to move the aircraft forward
    [Range(1,100)]
    public float thrust_multiplier;
    [Range(1,500)]
    public float yaw_multiplier;// to multiply the player input yaw value. (Rotation around the Y axis)
    [Range(1,500)]
    public float pitch_multiplier;
    new Rigidbody rigidbody;
    
    
    void Awake() 
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate() 
        /*FixedUpdate is called every fixed frame-rate frame at every 0.02 seconds (50 calls per second).
         This makes the physics movement independent from our machine frame rate*/
    {
        float pitch = Input.GetAxis("Vertical");
        float yaw = Input.GetAxis("Horizontal");

        rigidbody.AddRelativeForce(0f,0f,thrust * thrust_multiplier * Time.deltaTime);
        rigidbody.AddRelativeTorque(pitch * pitch_multiplier * Time.deltaTime,
            yaw * yaw_multiplier * Time.deltaTime, -yaw * yaw_multiplier * 2 * Time.deltaTime);
    }
}
