using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirplaneCharacteristics : MonoBehaviour
{
    [Header("Characteristics Properties")]
    public float maxMPH = 110f;
    public float rbLerpSpeed = 0.01f;

    [Header("Lift Properties")]
    public float maxLiftPower = 800f;
    public AnimationCurve liftCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    public float flapLiftPower = 100f;

    [Header("Drag Properties")]
    public float dragFactor = 0.01f;
    public float flapDragFactor = 0.005f;

    [Header("Control Properties")]
    public float pitchSpeed = 1000f;
    public float rollSpeed = 1000f;
    public float yawSpeed = 1000f;
    public AnimationCurve controlSurfaceEfficiency = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private float forwardSpeed;
    public float ForwardSpeed
    {
        get { return forwardSpeed; }
    }

    private float mph;

    public float getMph()
    {
        return mph;
    }

    public float getnormalizeMPH()
    {
        return normalizeMPH;
    }
    private BaseAirplane_Input input;
    private Rigidbody rb;
    private float startDrag;
    private float startAngularDrag;
    private float maxMPS;
    private float normalizeMPH;
    private float angleOfAttack;
    private float pitchAngle;
    private float rollAngle;
    private float csEfficiencyValue;
    const float mpsToMph = 2.23694f;
    private float currentRPM = 0f;
    public float minRPM = 1000f; // RPM mínimas cuando la velocidad es cero
    public float maxRPM = 3500f; // RPM actual
    public float maxAltitude = 5000f;
    public float getCurrentRPM
    {
        get { return currentRPM; }
    }

    public void InitCharacteristics(Rigidbody curRB, BaseAirplane_Input curInput)
    {
        // Inicialización básica
        input = curInput;
        rb = curRB;
        startDrag = rb.drag;
        startAngularDrag = rb.angularDrag;

        // Encuentra la velocidad máxima en metros por segundo
        maxMPS = maxMPH / mpsToMph;
    }

    // Actualiza todos los métodos de características de vuelo
    public void UpdateCharacteristics()
    {
        if (rb)
        {
            // Procesa la física de vuelo
            CalculateForwardSpeed();
            CalculateLift();
            CalculateDrag();

            // Procesa el control
            HandleControlSurfaceEfficiency();
            HandleYaw();
            HandlePitch();
            HandleRoll();
            HandleBanking();

            // Calcula los ángulos de cabeceo y alabeo
            CalculateRollAndPitch();
            // Maneja el Rigidbody
            HandleRigidbodyTransform();

            CalculateRPM();
            
            // Limita la altura máxima
            Vector3 position = transform.position;
            if (position.y > maxAltitude)
            {
                //Manejar aquí
                position.y = maxAltitude;
                transform.position = position;
            }
        }
    }

    // Obtiene la velocidad local hacia adelante en metros por segundo y la convierte a millas por hora
    void CalculateForwardSpeed()
    {
        // Transforma el vector de velocidad del Rigidbody de espacio mundial a espacio local
        Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
        forwardSpeed = Mathf.Max(0f, localVelocity.z);

        // Encuentra las millas por hora a partir de metros por segundo
        mph = forwardSpeed * mpsToMph;
        normalizeMPH = Mathf.InverseLerp(0f, maxMPH, mph);
    }

    void CalculateRPM()
    {
        // Calcula las RPM basadas en la velocidad del avión
        float normalizedSpeed = forwardSpeed / maxMPH; // Normaliza la velocidad en el rango de 0 a 1
        float targetRPM = Mathf.Lerp(minRPM, maxRPM, normalizedSpeed); // Interpola las RPM entre minRPM y maxRPM 

        // Aplica un suavizado para las RPM actuales
        currentRPM = Mathf.Lerp(currentRPM, targetRPM, Time.deltaTime);
    }

    // Genera una fuerza de sustentación lo suficientemente fuerte como para elevar el avión del suelo
    void CalculateLift()
    {
        // Obtiene el ángulo de ataque
        angleOfAttack = Vector3.Dot(rb.velocity.normalized, transform.forward);
        angleOfAttack *= angleOfAttack;

        // Crea la dirección de la sustentación
        Vector3 liftDir = transform.up;
        float liftPower = liftCurve.Evaluate(normalizeMPH) * maxLiftPower;

        // Agrega sustentación de flap
        float finalLiftPower = flapLiftPower * input.NormalizedFlaps;

        // Aplica la fuerza final de sustentación al Rigidbody
        Vector3 finalLiftForce = liftDir * (liftPower + finalLiftPower) * angleOfAttack;
        rb.AddForce(finalLiftForce);
    }

    // Obtiene una fuerza de arrastre para mantener el avión relativamente estable en el aire
    void CalculateDrag()
    {
        // Arrastre de velocidad
        float speedDrag = forwardSpeed * dragFactor;

        // Arrastre de flap
        float flapDrag = input.Flaps * flapDragFactor;

        // ¡Agrégalo todo!
        float finalDrag = startDrag + speedDrag + flapDrag;

        rb.drag = finalDrag;
        rb.angularDrag = startAngularDrag * forwardSpeed;
    }

    void HandleRigidbodyTransform()
    {
        if (rb.velocity.magnitude > 1f)
        {
            Vector3 updatedVelocity = Vector3.Lerp(rb.velocity, transform.forward * forwardSpeed, forwardSpeed * angleOfAttack * Time.deltaTime * rbLerpSpeed);
            rb.velocity = updatedVelocity;

            Quaternion updatedRotation = Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(rb.velocity, transform.up), Time.deltaTime * rbLerpSpeed);
            rb.MoveRotation(updatedRotation);
        }
    }

    void HandleControlSurfaceEfficiency()
    {
        csEfficiencyValue = controlSurfaceEfficiency.Evaluate(normalizeMPH);
    }

    void HandlePitch()
    {
        Vector3 flatForward = transform.forward;
        flatForward.y = 0f;
        flatForward = flatForward.normalized;
        pitchAngle = Vector3.Angle(transform.forward, flatForward);

        Vector3 pitchTorque = input.Pitch * pitchSpeed * transform.right * csEfficiencyValue;
        rb.AddTorque(pitchTorque);
    }

    void HandleRoll()
    {
        Vector3 flatRight = transform.right;
        flatRight.y = 0f;
        flatRight = flatRight.normalized;
        rollAngle = Vector3.SignedAngle(transform.right, flatRight, transform.forward);

        Vector3 rollTorque = -input.Roll * rollSpeed * transform.forward * csEfficiencyValue;
        rb.AddTorque(rollTorque);
    }

    void HandleYaw()
    {
        Vector3 yawTorque = input.Yaw * yawSpeed * transform.up * csEfficiencyValue;
        rb.AddTorque(yawTorque);
    }

    void HandleBanking()
    {
        float bankSide = Mathf.InverseLerp(-90f, 90f, rollAngle);
        float bankAmount = Mathf.Lerp(-1f, 1f, bankSide);
        Vector3 bankTorque = bankAmount * rollSpeed * transform.up;
        rb.AddTorque(bankTorque);
    }

    void CalculateRollAndPitch()
    {
        // Calcular el ángulo de alabeo según tu lógica (por ejemplo, usando transform.right)
        rollAngle = Vector3.SignedAngle(transform.right, Vector3.up, transform.forward);

        // Calcular el ángulo de cabeceo según tu lógica (por ejemplo, usando transform.forward)
        pitchAngle = Vector3.SignedAngle(transform.forward, Vector3.up, transform.right);
    }

    public float GetRollAngle()
    {
        return rollAngle;
    }

    public float GetPitchAngle()
    {
        return pitchAngle;
    }

    public float getMaxAltitude()
    {
        return maxAltitude;
    }
}

