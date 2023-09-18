using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class LeverAcceleration : MonoBehaviour
{
     //private PhysicsManager physicsManager;
     //private Vector3 accelerationAmount = Vector3.zero; 
     private AirplaneControls airplaneControls;
     private void Start()
     {
          //Debug.Log("LeverAcceleration: método Start"); 
          //physicsManager = GetComponentInParent<PhysicsManager>();
          airplaneControls = GetComponentInParent<AirplaneControls>();
     }

     public void OnJoystickValueChangeX(float x)
     {
          airplaneControls.SetAccelerationFromVR(-x);
     }

     public void OnJoystickValueChangeY(float y)
     {
          airplaneControls.SetDecelerationFromVR(-y);
     }
}
