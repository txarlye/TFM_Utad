using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airplane: MonoBehaviour
{
    // Variables para indicadores y sistemas de la aeronave
    private float tachometer;          // Tacómetro
    private float altimeter;           // Altímetro
    private float fuelIndicator;       // Indicador de Combustible
    private float flaps;               // Flaps
    private float airspeedIndicator;   // Indicador de Velocidad del Aire
    private float dashboardLights;     // Luces del Cuadro de Mandos

    // Variables para sistemas y controles
    private float enginePower;         // Potencia del Motor
    private float propellerRPM;        // Revoluciones por Minuto de la Hélice

    private Vector3 controlSurfaces;   // Superficies de Control (alerones, elevadores, timón)
    private float fuelCapacity;        // Capacidad de Combustible
    private float fuelConsumptionRate; // Tasa de Consumo de Combustible

    // Variables para sistemas avanzados y características
    private bool gpsEnabled;           // GPS activado
    private bool radioCommunication;   // Comunicación de Radio
    private bool radarSystem;          // Sistema de Radar

    // Variables para sistemas de armas (si aplicable)
    private int missileCount;          // Cantidad de Misiles
    private int bulletsRemaining;      // Balas Restantes

    // Variables para condiciones ambientales y realismo
    private float outsideTemperature;  // Temperatura Exterior
    private float pressure;            // Presión Atmosférica
    private float humidity;            // Humedad
    private float damageState;         // Estado de Daños

    [SerializeField]
    private GameObject myGameObjectPrefab;

    private AirplaneCharacteristics myPhysics;
    void Start()
    {
        // Inicialización y configuración inicial de variables...
    }

    void Update()
    {
        // Actualizamos los valores:
        UpdateAltimeter();
    }

    public void SetUp(GameObject prefab)
    {
        //Debug.Log("Setting up Airplane component");
        myGameObjectPrefab = prefab; 
    }
    
    void UpdateAltimeter()
    {
        // Calcula la altitud del avión usando la posición en el mundo
        altimeter = transform.position.y;
    }

    public float getAltimeter()
    {
        return altimeter;
    }

    public GameObject getMyGameObjectPrefab()
    {
        return myGameObjectPrefab;
    }

    public void setUpMyPhyshics(AirplaneCharacteristics newAirplaneCharacteristics)
    {
        myPhysics = newAirplaneCharacteristics;
    }

    public AirplaneCharacteristics getAirplaneCharacteristics()
    {
        return myPhysics;
    }
    
}
