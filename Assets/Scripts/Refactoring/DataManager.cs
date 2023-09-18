using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{ 
    public static DataManager Instance;
    
    //Persistent Data:
    [Header("Persistent Data:")]
    public MissionManager.MissionType selectedMissionType;
    public float bestTime; 
    
    [Header("Debugging")]
    public bool manualMissionTypeSelection = false; // Nuevo: interruptor para la selección manual
    public MissionManager.MissionType manualSelectedMissionType; 
    private void Awake()
    { 
            // start of new code
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            // end of new code

            Instance = this;
            DontDestroyOnLoad(gameObject); 
            
            // Cargar datos guardados
            if (manualMissionTypeSelection) // Nuevo: si el interruptor está activado, usa el valor manual
            {
                selectedMissionType = manualSelectedMissionType;
            }
            else
            {
                LoadMissionType(); // Cargar el tipo de misión guardado solo si no estamos en modo manual
            }
            LoadBestTime();
    }
    
    public void SetMissionType(MissionManager.MissionType missionType)
    {
        selectedMissionType = missionType;
        PlayerPrefs.SetInt("MissionType", (int)missionType); 
        PlayerPrefs.Save();
    }
    public MissionManager.MissionType GetMissionType()
    {
        return selectedMissionType;
    }
    private void LoadMissionType()
    {
        if (PlayerPrefs.HasKey("MissionType"))
        {
            selectedMissionType = (MissionManager.MissionType)PlayerPrefs.GetInt("MissionType");
        }
    }
    public void SetBestTime(float time, float completionPercentage)
    {
        float minimumCompletionPercentage = UIManager.Instance.getMinimumCompletionPercentage();
        float currentBestTime = GetBestTime();
    
        if (completionPercentage >= minimumCompletionPercentage)
        {
            if (time < currentBestTime || currentBestTime == 0)
            {
                bestTime = time;
                PlayerPrefs.SetFloat("BestTime", time);
                PlayerPrefs.Save();
            }
        }
    }

    public float GetBestTime()
    {
        return PlayerPrefs.GetFloat("BestTime", 0);
    }
    private void LoadBestTime()
    {
        if (PlayerPrefs.HasKey("BestTime"))
        {
            bestTime = PlayerPrefs.GetFloat("BestTime");
        }
    } 
    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }
}