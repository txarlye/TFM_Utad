using UnityEngine;
using TMPro;
using System.Collections;

public class SpeedUIUpdater : MonoBehaviour
{
    public PhysicsManager physicsManager; 
    

    

    private void Update()
    { 
            HandleAirplaneUI(); 
    }

    private void HandleAirplaneUI()
    {  
            float speedMPH = physicsManager.GetCurrentSpeed();
            GetComponent<TMP_Text>().text = "V: " + speedMPH.ToString("F0") + " mph";  
        
    }
}