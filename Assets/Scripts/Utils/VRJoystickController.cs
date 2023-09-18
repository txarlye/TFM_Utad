using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.XR.Content.Interaction
{
    public class VRJoystickController : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Speed of player movement")]
        float m_PlayerSpeed = 5.0f;
        private PhysicsManager physicsManager;   
        Vector3 m_MovementDirection;
        private void Start()
        {
            //Debug.Log("SpeedUIUpdater: método Start"); 
            physicsManager = GetComponentInParent<PhysicsManager>();
        }
        public void OnJoystickValueChangeX(float x)
        {
            // Use the joystick's X value to determine left/right movement
            m_MovementDirection.x = x; 
        }

        public void OnJoystickValueChangeY(float y)
        {
            // Use the joystick's Y value to determine forward/backward movement
            m_MovementDirection.z = y;
            //Debug.Log("Eje Y" + y);
        }

        void Update()
        {
            /*
            float joystickX = m_MovementDirection.x * 10f;
            float joystickY = m_MovementDirection.y *10000f;
            physicsManager.SetTorsion(joystickY,joystickX);
            */
            Vector2 joystickInput = new Vector2(m_MovementDirection.z, m_MovementDirection.x); // Obtener un vector de entrada
            float torsionMagnitude = joystickInput.magnitude; // Magnitud del vector de entrada
            Vector3 torsionDirection = new Vector3(joystickInput.x, 0f, -joystickInput.y); // Dirección de la torsión basada en el joystick

            // Multiplicar la dirección por la magnitud para controlar la torsión 
            physicsManager.SetTorsionVR(torsionDirection * torsionMagnitude * 1000f, 0f);
        }
    }
}