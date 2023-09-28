using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : Singleton<ObjectiveManager>
{
    public enum ObjectiveState
    {
        Inactive,
        Active,
        Completed
    }

    public ObjectiveState currentState;
    [Header("Caza al objetivo")]
    public List<GameObject> objectives; 
    [Header("Game of Rings - Despegar")]
    public GameObject ringPrefab;
    public GameObject ringStartPoint;
    public int totalRings = 10;
    public float distanceBetweenRings = 20f;
    [SerializeField] private List<GameObject> listOfRings;
    
    private int currentObjectiveIndex;
     

    void Start()
    {
        //MissionManager.MissionType currentMissionType = MissionManager.Instance.currentMissionType;
        MissionManager.MissionType currentMissionType = DataManager.Instance.GetMissionType();
        
        Debug.Log("Desde ObjectiveManager leemos como MissionManager" + currentMissionType.ToString());
        currentState = ObjectiveState.Active;
        switch (currentMissionType)
        {
            case MissionManager.MissionType.DestroyTargets: 
                for (int i = 1; i < objectives.Count; i++)
                {
                    objectives[i].SetActive(false);
                } 
                objectives[0].SetActive(true);
                currentObjectiveIndex = 0;
                break;

            case MissionManager.MissionType.PassThroughRings:
                PassThroughRings();
                break;

            case MissionManager.MissionType.TimedRace:
                PassThroughRings();
                break;
        }
    }

    public int getTotalRings()
    {
        return totalRings;
    }
    private void PassThroughRings()
    {
        foreach (GameObject obj in objectives)
        {
            obj.SetActive(false);
        }
                
                
        objectives = new List<GameObject>();
        Vector3 spawnPosition = ringStartPoint.transform.position + Vector3.forward * distanceBetweenRings; 

        for (int i = 0; i < totalRings; i++)
        {
            float z = i * distanceBetweenRings; // Aumenta la posición en el eje Z

            // Simula un despegue gradualmente ascendente
            float y = Mathf.Log(z + 1) * 5; 
            Vector3 newPosition = spawnPosition + new Vector3(0f, y, z);  

            Quaternion spawnRotation = Quaternion.identity;

            GameObject ring = Instantiate(ringPrefab, newPosition, spawnRotation);
            ring.SetActive(false); // Inicialmente, todos los anillos están desactivados
            listOfRings.Add(ring);
        }

        // Activa el primer anillo
        if (listOfRings.Count > 0)
        {
            listOfRings[0].SetActive(true);
        } 
                
        StartCoroutine(CheckMissedRing());

    }
    private IEnumerator CheckMissedRing()
    {
        bool allRingsCompleted = false;
        currentObjectiveIndex = 0; 

        while (!allRingsCompleted)
        {
            yield return new WaitForSeconds(0.1f);

            if (currentObjectiveIndex >= 0 && currentObjectiveIndex < listOfRings.Count)
            {
                Vector3 lastRingPosition = listOfRings[currentObjectiveIndex].transform.position;
                Vector3 playerPosition = ringStartPoint.gameObject.transform.position;

                if (playerPosition.z > lastRingPosition.z)
                {
                    listOfRings[currentObjectiveIndex].SetActive(false); // Desactivar el anillo actual
                    UIManager.Instance.UpdateScore(-1); // Restar puntuación

                    ActivateNextRing(); // Avanzar al siguiente anillo y activarlo

                    if (currentState == ObjectiveState.Completed)
                    {
                        allRingsCompleted = true; // Todos los anillos han sido completados o pasados
                    }
                    UIManager.Instance.ShowFailText();
                }
            }
        }
    }

    public void ObjectiveDestroyed()
    {
        MissionManager.MissionType currentMissionType = MissionManager.Instance.currentMissionType;

        switch (currentMissionType)
        {
            case MissionManager.MissionType.DestroyTargets:
                // Ocultar el objetivo actual
                objectives[currentObjectiveIndex].SetActive(false);
                Debug.Log("ObjectiveManager llama a ObjectiveDestroyed()");
                // Avanzar al siguiente objetivo y activarlo
                if (currentObjectiveIndex < objectives.Count - 1)
                {
                    currentObjectiveIndex++;
                    objectives[currentObjectiveIndex].SetActive(true);
                }
                else
                {
                    // Todos los objetivos han sido destruidos
                    Debug.Log("Todos los objetivos han sido destruidos.");
                    currentState = ObjectiveState.Completed;
                    GameManager.Instance.WinGame();
                }

                UIManager.Instance.UpdateScore(1);
                UIManager.Instance.UpdateArosPasadosTotales(1);
                break;

            case MissionManager.MissionType.PassThroughRings:
                //La logica de puntuación la lleva el player en onTriguer...
                break;

            case MissionManager.MissionType.TimedRace: 
                /* de la versión que guardábamos el tiempo entre anillos
                 float currentTime = Time.time - objectiveStartTime;
                if (currentObjectiveIndex < DataManager.Instance.objectiveTimes.Count)
                {
                    UIManager.Instance.UpdateNewRecordText(currentTime, DataManager.Instance.objectiveTimes[currentObjectiveIndex]);
                    if (currentTime < DataManager.Instance.objectiveTimes[currentObjectiveIndex])
                    {
                        DataManager.Instance.objectiveTimes[currentObjectiveIndex] = currentTime;
                    }
                }
                else
                {
                    DataManager.Instance.objectiveTimes.Add(currentTime);
                    UIManager.Instance.UpdateNewRecordText(currentTime, currentTime);
                }

                if (currentState == ObjectiveState.Completed)
                {
                    float currentGlobalTime = Time.time - objectiveStartTime;
                    if (currentGlobalTime < DataManager.Instance.GetGameTimeRing())
                    {
                        DataManager.Instance.SetGameTimeRing(currentGlobalTime);
                        UIManager.Instance.UpdateNewRecordText(currentGlobalTime, DataManager.Instance.GetGameTimeRing());
                    }
                }
                 */
                
                break;
        }
    }

    
    public Vector3 GetTargetPosition()
    {
        Vector3 targetPosition = Vector3.zero;

        switch (MissionManager.Instance.currentMissionType)
        {
            case MissionManager.MissionType.DestroyTargets:
                if (currentObjectiveIndex < objectives.Count)
                {
                    targetPosition = objectives[currentObjectiveIndex].transform.position;
                }
                break;

            case MissionManager.MissionType.PassThroughRings:
                if (currentObjectiveIndex < listOfRings.Count)
                {
                    targetPosition = listOfRings[currentObjectiveIndex].transform.position;
                }
                break;

            case MissionManager.MissionType.TimedRace:
                if (currentObjectiveIndex < listOfRings.Count)
                {
                    targetPosition = listOfRings[currentObjectiveIndex].transform.position;
                }
                break;

            default:
                
                break;
        }
        //Debug.Log("Desde Objetivemanager - distancia a objetivo = "+targetPosition);
        return targetPosition;
    }


    public void ActivateObjective()
    {
        currentState = ObjectiveState.Active;

        MissionManager.MissionType currentMissionType = MissionManager.Instance.currentMissionType;

        switch (currentMissionType)
        {
            case MissionManager.MissionType.DestroyTargets:
                // Mostrar el primer objetivo
                objectives[0].SetActive(true);
                currentObjectiveIndex = 0;
                break;

            case MissionManager.MissionType.PassThroughRings:
                // Lógica para activar este tipo de misión
                break;

            case MissionManager.MissionType.TimedRace:
                // Lógica para activar este tipo de misión
                break;
        }
    }

    public void ActivateNextRing()
    {
        if (currentObjectiveIndex < listOfRings.Count - 1)
        {
            currentObjectiveIndex++;
            listOfRings[currentObjectiveIndex].SetActive(true);
        }
        else
        {
            // Todos los aros han sido pasados
            Debug.Log("Todos los aros han sido pasados.");
            currentState = ObjectiveState.Completed;
            GameManager.Instance.WinGame();
        }
    }

    public int GetTotalObjectives()
    {
        MissionManager.MissionType currentMissionType = MissionManager.Instance.currentMissionType;
        switch (currentMissionType)
        {
            case MissionManager.MissionType.DestroyTargets:
                return objectives.Count;

            case MissionManager.MissionType.PassThroughRings:
                return listOfRings.Count;

            case MissionManager.MissionType.TimedRace:
                return listOfRings.Count;
                //return 0; // Cambia esto según tu implementación

            default:
                return 0;
        }
    } 
}
