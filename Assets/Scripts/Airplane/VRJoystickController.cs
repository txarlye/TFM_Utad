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
            physicsManager = GetComponentInParent<PhysicsManager>();
        }
        public void OnJoystickValueChangeX(float x)
        {
            m_MovementDirection.x = x; 
        }

        public void OnJoystickValueChangeY(float y)
        {
            m_MovementDirection.z = y;
            //Debug.Log("Eje Y" + y);
        }

        void Update()
        {
            Vector2 joystickInput = new Vector2(m_MovementDirection.z, m_MovementDirection.x); // Obtener un vector de entrada
            float torsionMagnitude = joystickInput.magnitude; // Magnitud del vector de entrada
            Vector3 torsionDirection = new Vector3(joystickInput.x, 0f, -joystickInput.y); // Dirección de la torsión basada en el joystick

            // Multiplicar la dirección por la magnitud para controlar la torsión 
            physicsManager.SetTorsionVR(torsionDirection * torsionMagnitude * 1000f, 0f);
        }
    }
}