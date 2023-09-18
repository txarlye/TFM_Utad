using UnityEngine;
using TMPro;
using System.Collections;

public class StartRing : MonoBehaviour
{
    public int totalRings = 10; // Número total de anillos en el camino
    public float distanceBetweenRings = 20f; // Distancia entre cada anillo
    [SerializeField] private int ringsPassed = 0; // Contador de anillos pasados

    public GameObject ringPrefab; // Prefab del anillo
    public GameObject prefabAirplane;
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI warningText;
    public TextMeshProUGUI puntuación;
    public TextMeshProUGUI totalAnillosSuperados;
    public TextMeshProUGUI altitudeWarningText; // Texto de advertencia de altura
    public TextMeshProUGUI completionText;
    public TextMeshProUGUI missionTypeText;
    public TextMeshProUGUI distanceToObjectiveText;
    public Canvas canvasFinal;
    public float targetHeight = 0f; // Altura objetivo del anillo actual
    public float minDistanceToShowWarning = 50f; // Distancia mínima para mostrar la advertencia
    public float minimumCompletionPercentage;

    private int currentObjectiveIndex = 0; // Índice del objetivo actual
    private int objectivesDestroyed = 0;

    private GameObject[] rings; // Almacena todos los anillos
    [SerializeField] private int currentRing = 0; // Índice del anillo actual
    private Vector3 playerPosition;

    private bool isCheckingMissedRing = true;

    //private GameManager.MissionType currentMissionType;
    public enum RingPlacementState
    {
        Despegue,
        ManiobraEvasion,
        Personalizado
    }

    //public RingPlacementState ringPlacementState = RingPlacementState.Despegue;
    public GameObject[] Objetives;

    private void Start()
    {
        //GameManager.MissionType myMission = GameManager.Instance.CurrentMissionType;
        //currentMissionType = GameManager.Instance.CurrentMissionType;
        rings = new GameObject[totalRings];
        ringsPassed = 0;
        puntuación.SetText("0");
        if (missionTypeText != null)
        {
            //  missionTypeText.text = myMission.ToString();
        }
        // Configura la posición y rotación de los anillos según el estado seleccionado
        //switch (ringPlacementState)


        /*
         switch (myMission)
        {
            case GameManager.MissionType.Despegue:
                PlaceRingsForTakeoff();
                StartCoroutine(CheckMissedRing()); 
                break;

            case GameManager.MissionType.ManiobraEvasion:
                PlaceRingsForEvasion();
                break;
            case GameManager.MissionType.AtaqueAlObjetivo:
                totalRings = Objetives.Length;
                AtaqueAlObjetivo();
                break;
        }
         */





    }

    private void AtaqueAlObjetivo()
    {
        // Inicializar todos los objetivos como inactivos
        foreach (GameObject objective in Objetives)
        {
            objective.SetActive(false);
        }

        // Activar el primer objetivo
        if (Objetives.Length > 0)
        {
            Objetives[0].SetActive(true);
        }
    }

    public void ObjectiveDestroyed()
    {
        // Incrementar el contador de objetivos destruidos
        objectivesDestroyed++;

        // Desactivar el objetivo actual
        Objetives[currentObjectiveIndex].SetActive(false);

        // Activar el siguiente objetivo, si hay alguno
        currentObjectiveIndex++;
        if (currentObjectiveIndex < Objetives.Length)
        {
            Objetives[currentObjectiveIndex].SetActive(true);
        }
        else
        {
            // Todos los objetivos han sido destruidos 
            if (canvasFinal != null)
            {
                canvasFinal.gameObject.SetActive(true);
            }
        }

        if (puntuación != null)
        {
            puntuación.text = objectivesDestroyed.ToString();
        }
        else
        {
            Debug.LogError("La variable 'puntuación' es null.");
        }

        Debug.Log("Objectives Destroyed: " + objectivesDestroyed);
        Debug.Log("Puntuación en UI: " + puntuación.text);
    }

    private void PlaceRingsForTakeoff()
    {
        rings[0] = gameObject; // El primer aro es el objeto actual

        for (int i = 1; i < totalRings; i++)
        {
            float z = i * distanceBetweenRings; // Aumenta la posición en el eje Z

            // Simula un despegue gradualmente ascendente
            float y = Mathf.Sqrt(z * 2); // Ajusta la altura en función de la distancia
            Vector3 spawnPosition = transform.position + new Vector3(0f, y, z); // Ajusta la posición en el eje Y

            // Mantén la orientación Z del aro inicial
            Quaternion spawnRotation = transform.rotation;

            rings[i] = Instantiate(ringPrefab, spawnPosition, spawnRotation);
            rings[i].SetActive(false); // Inicialmente, todos los anillos están desactivados
        }
    }


    private void PlaceRingsForEvasion()
    {
        // Inicializa la posición del primer anillo
        Vector3 spawnPosition = transform.position;
        Quaternion spawnRotation = Quaternion.Euler(0f, 90f, 0f); // Rotación para orientar el aro en el eje Z
        rings[0] = ringPrefab; // El primer anillo es el prefab

        for (int i = 1; i < totalRings; i++)
        {
            // Genera una posición aleatoria en el rango de -10 a 10 en los ejes X y Z
            float randomX = Random.Range(-10f, 10f);
            float randomZ = Random.Range(-10f, 10f);

            // Añade la distancia entre anillos a la posición actual
            spawnPosition += new Vector3(randomX, 0f, randomZ);

            // Crea el anillo en la nueva posición con la rotación adecuada
            rings[i] = Instantiate(ringPrefab, spawnPosition, spawnRotation);
            rings[i].SetActive(false); // Inicialmente, todos los anillos están desactivados
        }
    }

    public void RingPassed()
    {
        switch (currentRing)
        {
            case 0:
                currentRing++;
                StartCoroutine(ShowMessage("¡Has pasado un anillo!", 2f));
                rings[currentRing].SetActive(true);
                ringsPassed++;
                puntuación.text = ringsPassed.ToString();
                Debug.Log("StartRing:: TotalRings = " + currentRing + " de " + totalRings);
                break;

            case int n when n < (totalRings - 1):
                if (n > 0)
                {
                    rings[currentRing].SetActive(false);
                    currentRing++;
                    rings[currentRing].SetActive(true);
                    //StartCoroutine(CheckMissedRing()); 
                    ringsPassed++;
                    puntuación.text = ringsPassed.ToString();
                    StartCoroutine(ShowMessage("¡Has pasado un anillo!", 2f));
                    Debug.Log("StartRing:: TotalRings = " + currentRing + " de " + totalRings);
                    //Debug.Log("StartRing:: TotalRings = penultimo!!!");  
                }

                break;

            case int n when n == (totalRings - 1):
                // Acciones para cuando currentRing es igual o mayor que totalRings 
                // Cuando pasas por el último anillo, aquí puedes realizar acciones cuando el jugador haya pasado todos los anillos
                Debug.Log("¡Has pasado todos los anillos!");
                Debug.Log("StartRing:: TotalRings = " + currentRing + " de " + totalRings);
                ringsPassed++;
                rings[currentRing].SetActive(false);
                currentRing++;
                // Muestra el canvas de final de partida
                if (canvasFinal != null)
                {
                    canvasFinal.gameObject.SetActive(true);
                }

                // Desactiva el componente PhysicsManager del avión
                PhysicsManager physicsManager = prefabAirplane.GetComponent<PhysicsManager>();
                if (physicsManager != null)
                {
                    physicsManager.enabled = false;
                }

                ///// Muestra las estadísticas 
                showStadistics();
                break;
            default:
                break;

        }

        // Actualiza el texto de puntuación y total de anillos superados
        puntuación.text = ringsPassed.ToString();
        totalAnillosSuperados.text = ringsPassed + " / " + totalRings;
    }

    public void showStadistics()
    {
        float completionPercentage = (float)ringsPassed / (float)totalRings * 100f;

        Debug.Log(completionPercentage);
        // Actualiza el texto del canvas con el porcentaje y el mensaje correspondiente
        string completionMessage = "HEMOS COMPLETADO EL VUELO\n" +
                                   "DEBES COMPLETAR UN MÍNIMO DEL " + minimumCompletionPercentage.ToString("F2") +
                                   "% DE ACIERTOS PARA SIGUIENTES FASES.\n\n" +
                                   "TU HAS TENIDO UN " + completionPercentage.ToString("F2") +
                                   "% DE ANILLOS COMPLETADOS, ";

        // Comprueba si se ha superado la misión en base al porcentaje mínimo
        if (completionPercentage >= minimumCompletionPercentage)
        {
            completionMessage += "HAS SUPERADO LA MISIÓN.";
        }
        else
        {
            completionMessage += "NO HAS SUPERADO LA MISIÓN.";
        }

        // Actualiza el texto en el TextMeshProUGUI
        if (completionText != null)
        {
            completionText.text = completionMessage;
        }
    }

    private void Update()
    {
        // Verifica si el jugador ha pasado todos los anillos
        if (ringsPassed >= totalRings)
        {
            // Aquí puedes realizar acciones cuando el jugador haya pasado todos los anillos
            Debug.Log("¡Has pasado todos los anillos!");
        }

        UpdateDataInfo();
        // Actualiza la distancia al objetivo
        //Vector3 currentObjectivePosition = GetCurrentRingPosition();
        //float distanceToObjective = Vector3.Distance(prefabAirplane.transform.position, currentObjectivePosition);
        //distanceToObjectiveText.text = distanceToObjective.ToString("F2") + " m";
    }

    private IEnumerator ShowMessage(string message, float duration)
    {
        messageText.text = message;
        messageText.gameObject.SetActive(true);

        yield return new WaitForSeconds(duration);

        messageText.gameObject.SetActive(false);
    }

    private void UpdateDataInfo()
    {
        puntuación.text = ringsPassed.ToString();
        totalAnillosSuperados.text = ringsPassed + " / " + totalRings;
    }


    private IEnumerator CheckMissedRing()
    {
        Vector3 lastRingPosition = rings[currentRing].transform.position;
        playerPosition = prefabAirplane.gameObject.transform.position;
        bool allRingsCompleted = false;

        while (!allRingsCompleted)
        {
            yield return new WaitForSeconds(0.1f);

            if (isCheckingMissedRing && currentRing >= 0 && currentRing < (totalRings))
            {
                // Comprueba si el jugador se ha pasado el aro sin chocar con él
                if ((playerPosition.z) > lastRingPosition.z)
                {
                    // El jugador se ha pasado el aro sin chocar
                    if (currentRing == 0)
                    {
                        ringsPassed--; // Resta un punto
                        puntuación.text = ringsPassed.ToString();
                        currentRing++;
                        rings[currentRing].SetActive(true);
                        targetHeight = rings[currentRing].transform.position.y;
                        warningText.gameObject.SetActive(true);
                        yield return new WaitForSeconds(3f);
                        warningText.gameObject.SetActive(false);
                        float auxLog = playerPosition.z - lastRingPosition.z;
                        Debug.Log("Distancia al aro" + auxLog);
                    }
                    else
                    {
                        rings[currentRing].SetActive(false);
                        ringsPassed--; // Resta un punto
                        puntuación.text = ringsPassed.ToString();
                        currentRing++;
                        if (currentRing < totalRings)
                        {
                            rings[currentRing].SetActive(true);
                            targetHeight = rings[currentRing].transform.position.y;
                        }

                        warningText.gameObject.SetActive(true);
                        yield return new WaitForSeconds(3f);
                        warningText.gameObject.SetActive(false);
                        float auxLog = playerPosition.z - lastRingPosition.z;
                        Debug.Log("Distancia al aro" + auxLog);
                    }
                }

                //////////////////////////////////////////
                // Calcula la distancia entre la altura del avión y la altura objetivo del anillo
                float heightDifference = playerPosition.y - targetHeight;

                if (Mathf.Abs(heightDifference) > minDistanceToShowWarning)
                {
                    string altitudeMessage = heightDifference > 0
                        ? "Precaución: debe subir " + heightDifference.ToString("F2") +
                          "m su altura para llegar a su destino"
                        : "Precaución: debe bajar " + Mathf.Abs(heightDifference).ToString("F2") +
                          "m su altura para llegar a su destino";
                    altitudeWarningText.text = altitudeMessage;
                    altitudeWarningText.gameObject.SetActive(true);
                }
                else
                {
                    // Si la distancia es mayor, oculta el mensaje de advertencia de altura
                    altitudeWarningText.gameObject.SetActive(false);
                }

                if (currentRing >= totalRings)
                {
                    allRingsCompleted = true;
                    isCheckingMissedRing = false;
                }
            }

            if (currentRing >= 0 && currentRing < totalRings)
            {
                lastRingPosition = rings[currentRing].transform.position;
                playerPosition = prefabAirplane.gameObject.transform.position;
            }
        }
    }




    public void GetCurrentRingPosition()
    {
        //antes era vector3 y no void
        /*
          if (currentMissionType == GameManager.MissionType.AtaqueAlObjetivo)
        {
            // Devuelve la posición del objetivo actual en el modo AtaqueAlObjetivo
            if (currentObjectiveIndex >= 0 && currentObjectiveIndex < Objetives.Length)
            {
                return Objetives[currentObjectiveIndex].transform.position;
            }
            else
            { 
                return Vector3.zero;
            }
        }
        else
        {
            if (currentRing >= 0 && currentRing < totalRings)
            {
                if (currentRing == 0)
                { 
                    return transform.position;
                }
                else
                { 
                    return rings[currentRing].transform.position;
                }
            }
            else
            { 
                return Vector3.zero;
            }
        }
         */

    }


}
