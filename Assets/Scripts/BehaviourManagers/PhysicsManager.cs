using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PhysicsManager : MonoBehaviour
{
     [Header("Thrust Settings")] public float thrust = 200f; // Fuerza de empuje para mover la aeronave hacia adelante.
    public float acceleration = 10f; // Aceleración de la velocidad.
    [Range(1, 100)] public float thrust_multiplier = 10f; // Multiplicador de aceleración.

    [Header("Rotation Settings")] [Range(1, 500)]
    public float yaw_multiplier = 100f; // Multiplicador para la rotación alrededor del eje Y (giro).

    [Range(1, 500)] public float pitch_multiplier = 100f;

    [Header("Speed Settings")] [Range(1, 2000)]
    public float maxSpeed = 200f; // Velocidad máxima.

    public float speedScale = 1f; // Escala para ajustar la velocidad.
    public float vrTorsionMultiplier = 1.0f;
    private Rigidbody rigidbody;

    private bool accelerating = false; // Indica si estamos acelerando.
    private float currentSpeed = 0f; // Velocidad actual del avión.
    private float currentSpeedPitch = 0f;
    private float accelerationAmount = 0f;
    private float accelerationAmountVR = 0f;

    private StartRing myFirstTorus;
    private List<GameObject> passedRings = new List<GameObject>();
    private StartRing missionControl;

    private bool canCallRingPassed = true;

    //private GameManager.MissionType myMission;
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        passedRings.Clear();
    }

    public void Start()
    {
        // myMission = GameManager.Instance.CurrentMissionType;
    }

    void FixedUpdate()
    {
        float pitch = Input.GetAxis("Vertical");
        float yaw = Input.GetAxis("Horizontal");

        // Detectar si se está acelerando (M o N están siendo presionadas).
        accelerating = Input.GetKey(KeyCode.M) || Input.GetKey(KeyCode.N);

        if (accelerating || Mathf.Abs(accelerationAmountVR) > 0.01f)
        {
            accelerationAmount = Input.GetKey(KeyCode.M) ? 1f : -1f;
            ///////// If we are in VR we get value for accelerationAmountVR:
            if (accelerationAmountVR != 0)
            {
                //Debug.Log("accelerationAmountVR =" + accelerationAmountVR);
                accelerationAmount = accelerationAmountVR;
            }

            ///////////// Fordward ////////////////////
            currentSpeed += accelerationAmount * thrust * acceleration * Time.deltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, 0f, thrust * thrust_multiplier);
            ///////////// Ud & down //////////////////// 
            currentSpeedPitch += pitch * thrust * Time.deltaTime;
            currentSpeedPitch = Mathf.Clamp(currentSpeedPitch, 0f, thrust * thrust_multiplier);
        }

        // Aplicar una fuerza relativa en el eje Z para mover la aeronave hacia adelante.
        rigidbody.AddRelativeForce(0f, 0f, currentSpeed);

        // Aplicar una fuerza vertical en el eje Y para controlar el movimiento hacia arriba y abajo.
        rigidbody.AddRelativeForce(0f, pitch * thrust * Time.deltaTime, 0f);

        rigidbody.AddRelativeTorque(
            pitch * pitch_multiplier * Time.deltaTime,
            yaw * yaw_multiplier * Time.deltaTime,
            -yaw * yaw_multiplier * 2 * Time.deltaTime);
    }

    // Método para configurar la escala de velocidad.
    public void SetSpeedScale(float scale)
    {
        speedScale = Mathf.Max(0f, scale);
    }

    // Método para obtener la velocidad actual escalada.
    public float GetCurrentSpeed()
    {
        // Devuelve la velocidad reducida.
        return (currentSpeed / speedScale) * 10;
    }

    // Método para configurar la velocidad máxima.
    public void SetMaxSpeed(float speed)
    {
        maxSpeed = Mathf.Max(0f, speed);
    }

    public void SetCurrentSpeed(float speed)
    {
        // Configura la velocidad actual basada en la entrada del joystick.
        currentSpeed = Mathf.Clamp(speed * thrust * thrust_multiplier, 0f, thrust * thrust_multiplier);
    }

    public void SetTorsion(float pitch, float yaw)
    {
        // Modificar la dirección del avión en función de los valores del joystick
        rigidbody.AddRelativeTorque(
            pitch * pitch_multiplier * Time.deltaTime,
            yaw * yaw_multiplier * Time.deltaTime,
            -yaw * yaw_multiplier * 2 * Time.deltaTime);
    }

    public void SetTorsionVR(Vector3 torsionDirection, float torsionMagnitude)
    {
        // Implementa aquí la lógica específica para controlar la torsión del avión
        // cuando se usa el joystick VR.

        // Por ejemplo, puedes aplicar fuerzas de torsión adicionales o ajustar la rotación
        // basada en la dirección y magnitud proporcionadas.

        // Ejemplo de aplicación de fuerzas de torsión adicionales (sujeto a ajustes):
        rigidbody.AddRelativeTorque(
            torsionDirection.x * vrTorsionMultiplier * Time.deltaTime,
            torsionDirection.y * vrTorsionMultiplier * Time.deltaTime,
            torsionDirection.z * vrTorsionMultiplier * Time.deltaTime);
    }

    public void setAcceleratingON()
    {
        accelerating = true;
    }

    public void setAcceleratingOFF()
    {
        accelerating = false;
    }

    public void SetAceleracionVR(float acceleration)
    {
        accelerationAmountVR = acceleration;
    }
}
