using UnityEngine;

public class MinimapLine : MonoBehaviour
{
    public Transform airplane; // El objeto del avión
    private LineRenderer lineRenderer; // El componente LineRenderer
    private MissionManager.MissionType currentMissionType;
    public ObjectiveManager currentObjective;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        currentMissionType = MissionManager.Instance.currentMissionType;
        currentObjective = ObjectiveManager.Instance; 
        
        // Ajusta las propiedades del LineRenderer, como el material y otros ajustes
    }

    private void Update()
    {
        Vector3 targetPosition = Vector3.zero;

        if (currentObjective != null)
        {
            switch (currentMissionType)
            {
                case MissionManager.MissionType.DestroyTargets:
                    Debug.Log("MinimapLine: caso DestroyTargets");
                    targetPosition = currentObjective.GetTargetPosition();  
                    break;

                case MissionManager.MissionType.PassThroughRings:
                    Debug.Log("MinimapLine: caso PassThroughRings");
                    targetPosition = currentObjective.GetTargetPosition();  
                    break;

                case MissionManager.MissionType.TimedRace:
                    Debug.Log("MinimapLine: caso TimedRace");
                    targetPosition = currentObjective.GetTargetPosition();  
                    break;

                default:
                    Debug.Log("MinimapLine: caso no identificado");
                    targetPosition = currentObjective.GetTargetPosition();  
                    break;
            }

            // Ajusta los puntos finales de la línea para que vayan desde el avión a la posición del objetivo o anillo actual
            lineRenderer.SetPosition(0, airplane.position);
            lineRenderer.SetPosition(1, targetPosition);
        }
        else
        {
            Debug.Log("MinimapLine: ObjectiveManager no está asignado.");
        }
    }
}
