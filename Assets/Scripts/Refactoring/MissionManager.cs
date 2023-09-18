using UnityEngine;

public class MissionManager : Singleton<MissionManager>
{
    public enum MissionType { DestroyTargets, PassThroughRings, TimedRace }
    public MissionType currentMissionType;

    public ObjectiveManager objective;
    
    void Start()
    {
        if (DataManager.Instance != null)
        {
            Debug.Log("MissionManager:: tenemos DataManager");
            MissionType selectedType = DataManager.Instance.GetMissionType();
            if (System.Enum.IsDefined(typeof(MissionType), selectedType))
            {
                currentMissionType = selectedType;
                Debug.Log("MissionManager:: deberia haberse seteado");
            }
        }
        
        switch (currentMissionType)
        {
            case MissionType.DestroyTargets: 
                objective.ActivateObjective();
                AudioManager.Instance.Play("ambienteAeroplano");
                break;

            case MissionType.PassThroughRings: 
                objective.ActivateObjective();
                AudioManager.Instance.Play("ambienteAeroplano");
                break;

            case MissionType.TimedRace: 
                AudioManager.Instance.Play("ambienteAeroplano");
                break;
        }
    }

    void Update()
    {
        // Aquí puedes verificar el estado de la misión según el tipo
        switch (currentMissionType)
        {
            case MissionType.DestroyTargets:
                if (objective.currentState != ObjectiveManager.ObjectiveState.Completed)
                {
                    // Lógica para verificar el estado de la misión DestroyTargets
                }
                if (objective.currentState == ObjectiveManager.ObjectiveState.Completed)
                {
                    // Lógica para cuando la misión se completa
                    GameManager.Instance.WinGame();
                }
                break;

            case MissionType.PassThroughRings:
                if (objective.currentState != ObjectiveManager.ObjectiveState.Completed)
                {
                    // Lógica para verificar el estado de la misión PassThroughRings
                }
                break;

            case MissionType.TimedRace:
                if (objective.currentState != ObjectiveManager.ObjectiveState.Completed)
                {
                    // Lógica para verificar el estado de la misión TimedRace
                }
                break;
        }
    }
}


