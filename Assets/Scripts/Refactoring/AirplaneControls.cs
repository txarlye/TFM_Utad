using UnityEngine;

public class AirplaneControls : Singleton<AirplaneControls>
{
    [Header("Control Properties")]
    [Range(1, 100)]
    public float pitchSpeed = 1000f;
    [Range(1, 100)]
    public float rollSpeed = 1000f;
    [Range(1, 100)]
    public float yawSpeed = 1000f;
    
    public float accelerationSpeed = 10f; // Aumentado a 10000
    public float decelerationSpeed = 1f;
    
    public bool invertPitch = false;
    public bool invertYaw = false;
    public float accelerationCoefficient = 100f; // Por defecto es 100, y puedes cambiarlo hasta 10000 en el inspector
    public float maxSpeed = 700f;
    [Header("Control Coefficients")]
    [Range(0, 1)]
    public float pitchCoefficient = 1f;
    [Range(0, 1)]
    public float yawCoefficient = 1f;

    [Header("Roll Effect")]
    public float rollEffect = 0.2f; // Controla la cantidad de alabeo cuando se gira
    [Header("Additional Options")]
    public bool applyGravity = false; // Si la gravedad afecta al avión
    public bool allowReverse = false; // Si el avión puede moverse hacia atrás
    
    [Range(1, 1000)]
    public float accelerationMultiplier = 500f;
    
    private Rigidbody rb;
    private AirplanePhysics airplanePhysics;
    private bool accelerating;
    private bool decelerating;
    private bool isVRControl = false;
    private float lastNonZeroAcceleration = 0f;
    private Vector3 currentVelocity;
    private int auxFirstTimeAcceleration = 0;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("No se pudo encontrar el componente Rigidbody.");
        }
        
    }

    public void Initialize(Rigidbody rigidbody, AirplanePhysics physics)
    {
        rb = rigidbody;
        airplanePhysics = physics;
    }

    void Update()
    {
        float pitch = Input.GetAxis("Vertical") * (invertPitch ? -1 : 1);
        float yaw = Input.GetAxis("Horizontal") * (invertYaw ? -1 : 1);

        accelerating = Input.GetKey(KeyCode.M);
        decelerating = Input.GetKey(KeyCode.N);
        
        HandleControls(pitch, yaw);
        if (accelerating)
        {
            auxFirstTimeAcceleration += 1;

            if (auxFirstTimeAcceleration <= 1)
            {
                AudioManager.Instance.StopAll();
                AudioManager.Instance.Play("DespegueF5");
            } 
            isVRControl = false;
            currentVelocity += transform.forward * accelerationSpeed * Time.deltaTime;
            if (rb.velocity.magnitude < maxSpeed)
            {
                isVRControl = false;
                currentVelocity += transform.forward * accelerationSpeed * Time.deltaTime; 
            }
            else
            {
                currentVelocity = rb.velocity;
            }
            
        }
        else if (decelerating)
        {
            isVRControl = false;
            currentVelocity -= transform.forward * decelerationSpeed * Time.deltaTime;
        }
        
        // Alinea currentVelocity con la orientación del avión si se está girando
        if (Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f || Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f)
        {
            currentVelocity = transform.forward * currentVelocity.magnitude;
        }
        // Aplicar la velocidad actual al Rigidbody
        rb.velocity = currentVelocity;
        
        if (accelerating || decelerating || Mathf.Abs(pitch) > Mathf.Epsilon || Mathf.Abs(yaw) > Mathf.Epsilon)
        {
            isVRControl = true;
        }
        if (Mathf.Abs(lastNonZeroAcceleration) > Mathf.Epsilon)
        { 
            float appliedAcceleration = isVRControl ? lastNonZeroAcceleration * accelerationMultiplier : lastNonZeroAcceleration;
            currentVelocity += transform.forward * appliedAcceleration * Time.deltaTime;
            //rb.velocity += transform.forward * appliedAcceleration* Time.deltaTime; 
        } 
    }

    void HandleControls(float pitch, float yaw)
    {
        HandlePitch(pitch);
        HandleYaw(yaw);
        HandleRoll(yaw); // Añadido para manejar el alabeo

        Vector3 lateralMovement = yaw * yawSpeed * yawCoefficient * transform.right;
       
        rb.AddForce(lateralMovement);
        
        if (accelerating)
        {
            Vector3 acceleration = transform.forward * accelerationSpeed * accelerationCoefficient;
            rb.AddForce(acceleration);
        }

        if (decelerating)
        {
            Vector3 deceleration = -transform.forward * decelerationSpeed* accelerationCoefficient;
            if (!allowReverse && rb.velocity.magnitude < 0.1f)
            {
                deceleration = Vector3.zero; // Detiene el avión si no se permite el movimiento hacia atrás
            }
            rb.AddForce(deceleration, ForceMode.Acceleration);
        }

        if (applyGravity)
        {
            rb.AddForce(Vector3.down * 9.81f); // Añade la fuerza de gravedad
        }
    }

    void HandlePitch(float pitch)
    {
        Vector3 pitchTorque = pitch * pitchSpeed * pitchCoefficient * transform.right;
        rb.AddTorque(pitchTorque);
    }

    void HandleYaw(float yaw)
    {
        Vector3 yawTorque = yaw * yawSpeed * yawCoefficient * transform.up;
        rb.AddTorque(yawTorque);
    }

    void HandleRoll(float yaw)
    {
        // Añade un pequeño torque en el eje Z cuando se gira
        Vector3 rollTorque = yaw * rollSpeed * rollEffect * transform.forward;
        rb.AddTorque(-rollTorque);
    }
    
    /////Para control VR:
    public void SetAccelerationFromVR(float acceleration)
    {
        isVRControl = true;
        if (Mathf.Abs(acceleration) > Mathf.Epsilon)
        {
            lastNonZeroAcceleration = acceleration * accelerationMultiplier; // Actualiza la última aceleración no cero
        }
        Vector3 accelerationForce = (transform.forward * accelerationSpeed * acceleration) * accelerationMultiplier;
        rb.AddForce(accelerationForce);
    }


    public void SetDecelerationFromVR(float deceleration)
    {
        isVRControl = true;
        if (Mathf.Abs(deceleration) > Mathf.Epsilon)
        {
            lastNonZeroAcceleration = -deceleration * accelerationMultiplier; // Actualiza la última aceleración no cero
        }
        Vector3 decelerationForce = (-transform.forward * decelerationSpeed * deceleration ) * accelerationMultiplier;
        rb.AddForce(decelerationForce, ForceMode.Acceleration);
    }

    public float GetCurrentSpeed()
    {
        if (rb != null)
        {
            return rb.velocity.magnitude;
        }
        else
        {
            Debug.LogError("Rigidbody no inicializado.");
            return 0f;
        }
    }
}