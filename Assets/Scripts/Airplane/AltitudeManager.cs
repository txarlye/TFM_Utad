using UnityEngine;

public class AltitudeManager : MonoBehaviour
{
    private Transform airplaneTransform; // Referencia al transform del avión
    private float seaLevel = 0f; // Altura del nivel del mar en metros (puede ser ajustada según tu escenario)

    [Header("Altitude Values")]
    public float relativeAltitude = 0f; // Altura relativa desde el nivel del mar
    public float totalAltitude = 0f; // Altura total desde el origen

    void Start()
    {
        // Obtén la referencia al transform del avión
        airplaneTransform = transform; // Puedes ajustar esto si el script está en un objeto diferente al avión
    }

    void Update()
    {
        // Calcula la posición actual del avión en relación con el nivel del mar
        Vector3 position = airplaneTransform.position;
        relativeAltitude = position.y - seaLevel;

        // Calcula la altura total desde el origen (puede ser útil para rastrear la altura total recorrida)
        totalAltitude = position.y;
    }
    
    // Método para obtener la altura relativa
    public float GetRelativeAltitude()
    {
        return relativeAltitude;
    }

    // Método para obtener la altura total
    public float GetTotalAltitude()
    {
        return totalAltitude;
    }
}