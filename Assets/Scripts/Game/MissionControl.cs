using UnityEngine;
using TMPro;
using System.Collections;

public class MissionControl : MonoBehaviour
{
    public int totalRings = 10;
    public float distanceBetweenRings = 20f;
    [SerializeField] private int ringsPassed = 0;

    public GameObject ringPrefab;
    public GameObject prefabAirplane;
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI warningText;
    public TextMeshProUGUI puntuación;
    public TextMeshProUGUI totalAnillosSuperados;
    public TextMeshProUGUI altitudeWarningText;
    public TextMeshProUGUI completionText;
    public TextMeshProUGUI missionType;
    public Canvas canvasFinal;
    public float targetHeight = 0f;
    public float minDistanceToShowWarning = 50f;
    public float minimumCompletionPercentage;

    private GameObject[] rings;
    private int currentRing = 0;
    private Vector3 playerPosition;

    public enum RingPlacementState
    {
        Despegue,
        ManiobraEvasion,
        Personalizado
    }

    public RingPlacementState ringPlacementState = RingPlacementState.Despegue;

    private void Start()
    {
        rings = new GameObject[totalRings];
        ringsPassed = 0;
        puntuación.SetText("0");
        
        // Obtén el tipo de misión desde el Modo Manager
        ModeManager.GameMode myMode = ModeManager.Instance.GetMyGameMode();

        if (myMode == null)
        {
            myMode = ModeManager.GameMode.Despegue;
        }
        switch (myMode)
        {
            case ModeManager.GameMode.Despegue:
                ringPlacementState = RingPlacementState.Despegue;
                missionType.text = "Despegue";
                break;

            case ModeManager.GameMode.ManiobraEvasion:
                ringPlacementState = RingPlacementState.ManiobraEvasion;
                missionType.text = "Maniobra de Evasion";
                break;

            case ModeManager.GameMode.Personalizado:
                ringPlacementState = RingPlacementState.Personalizado;
                missionType.text = "Personalizado";
                break;
        }
    }

    private void PlaceRingsForTakeoff()
    {
        // Crea el primer aro como una instancia del prefab y asígnalo a rings[0]
        rings[0] = Instantiate(ringPrefab, transform.position, transform.rotation);

        for (int i = 1; i < totalRings; i++)
        {
            float z = i * distanceBetweenRings;
            float y = Mathf.Sqrt(z * 2);
            Vector3 spawnPosition = transform.position + new Vector3(0f, y, z);
            Quaternion spawnRotation = transform.rotation;

            rings[i] = Instantiate(ringPrefab, spawnPosition, spawnRotation);
            rings[i].SetActive(false);
        }
    }

    private void PlaceRingsForEvasion()
    {
        Vector3 spawnPosition = transform.position;
        Quaternion spawnRotation = Quaternion.Euler(0f, 90f, 0f);
        rings[0] = ringPrefab;

        for (int i = 1; i < totalRings; i++)
        {
            float randomX = Random.Range(-10f, 10f);
            float randomZ = Random.Range(-10f, 10f);
            spawnPosition += new Vector3(randomX, 0f, randomZ);

            rings[i] = Instantiate(ringPrefab, spawnPosition, spawnRotation);
            rings[i].SetActive(false);
        }
    }

    public void RingPassed()
    {
        currentRing++;

        if (currentRing > 0)
        {
            //rings[currentRing].SetActive(false);
        }

        if (currentRing < totalRings - 1)
        {
            rings[currentRing].SetActive(true);
            if (rings[currentRing] != null && rings[currentRing].activeSelf)
            {
                StartCoroutine(CheckMissedRing());
            }
            else
            {
                Debug.LogWarning("El siguiente anillo no está activo o es nulo.");
            }

            ringsPassed++;
            puntuación.text = ringsPassed.ToString();

            StartCoroutine(ShowMessage("¡Has pasado un anillo!", 2f));
        }
        else
        {
            Debug.Log("¡Has pasado todos los anillos!");

            if (canvasFinal != null)
            {
                canvasFinal.gameObject.SetActive(true);
            }

            PhysicsManager physicsManager = prefabAirplane.GetComponent<PhysicsManager>();
            if (physicsManager != null)
            {
                physicsManager.enabled = false;
            }
        }

        UpdateDataInfo();
    }

    private void UpdateDataInfo()
    {
        puntuación.text = ringsPassed.ToString();
        totalAnillosSuperados.text = currentRing + " / " + totalRings;
    }

    private IEnumerator ShowMessage(string message, float duration)
    {
        messageText.text = message;
        messageText.gameObject.SetActive(true);

        yield return new WaitForSeconds(duration);

        messageText.gameObject.SetActive(false);
    }

    private IEnumerator CheckMissedRing()
    {
        Vector3 lastRingPosition = rings[currentRing].transform.position;
        playerPosition = prefabAirplane.gameObject.transform.position;
        bool allRingsCompleted = false;
        bool gameOver = false;

        while (!gameOver)
        {
            yield return new WaitForSeconds(0.1f);

            if (currentRing >= 0 && currentRing < totalRings)
            {
                if (playerPosition.z > lastRingPosition.z)
                {
                    rings[currentRing].SetActive(false);
                    ringsPassed--;
                    puntuación.text = ringsPassed.ToString();
                    currentRing++;

                    if (currentRing < totalRings)
                    {
                        rings[currentRing].SetActive(true);
                    }

                    warningText.gameObject.SetActive(true);
                    yield return new WaitForSeconds(3f);
                    warningText.gameObject.SetActive(false);

                    float auxLog = playerPosition.z - lastRingPosition.z;
                    Debug.Log("Distancia al aro" + auxLog);
                }

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
                    altitudeWarningText.gameObject.SetActive(false);
                }

                if (currentRing >= totalRings - 1)
                {
                    allRingsCompleted = true;
                }
            }

            if (currentRing >= 0 && currentRing < totalRings)
            {
                lastRingPosition = rings[currentRing].transform.position;
                playerPosition = prefabAirplane.gameObject.transform.position;
            }

            if (allRingsCompleted)
            {
                PhysicsManager physicsManager = prefabAirplane.GetComponent<PhysicsManager>();
                if (physicsManager != null)
                {
                    physicsManager.enabled = false;
                }

                float completionPercentage = (float)ringsPassed / (float)totalRings * 100f;
                Debug.Log(completionPercentage);

                string completionMessage = "HEMOS COMPLETADO EL VUELO\n" +
                                           "DEBES COMPLETAR UN MÍNIMO DEL " + minimumCompletionPercentage.ToString("F2") + "% DE ACIERTOS PARA SIGUIENTES FASES.\n\n" +
                                           "TU HAS TENIDO UN " + completionPercentage.ToString("F2") + "% DE ANILLOS COMPLETADOS, ";

                if (completionPercentage >= minimumCompletionPercentage)
                {
                    completionMessage += "HAS SUPERADO LA MISIÓN.";
                }
                else
                {
                    completionMessage += "NO HAS SUPERADO LA MISIÓN.";
                }

                if (completionText != null)
                {
                    completionText.text = completionMessage;
                }

                if (canvasFinal != null)
                {
                    canvasFinal.gameObject.SetActive(true);
                }

                gameOver = true;
            }
        }
    }

    public Vector3 GetCurrentRingPosition()
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
    
    public void ChangeMissionType(int newMissionType)
    {
        // Actualiza el estado de la misión según el valor recibido
        switch (newMissionType)
        {
            case 0:
                ModeManager.Instance.SetGameMode(ModeManager.GameMode.Despegue);
                break;

            case 1:
                ModeManager.Instance.SetGameMode(ModeManager.GameMode.ManiobraEvasion);
                break;

            case 2:
                ModeManager.Instance.SetGameMode(ModeManager.GameMode.Personalizado);
                break;
 
        }

        // Actualiza el texto del tipo de misión
        switch (ringPlacementState)
        {
            case RingPlacementState.Despegue:
                missionType.text = "Despegue";
                break;

            case RingPlacementState.ManiobraEvasion:
                missionType.text = "Maniobra de Evitación";
                break;

            case RingPlacementState.Personalizado:
                missionType.text = "Personalizado"; 
                break;
        }
    }
    
    
}

