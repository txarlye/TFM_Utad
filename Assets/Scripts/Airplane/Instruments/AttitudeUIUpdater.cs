using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AttitudeUIUpdater : MonoBehaviour
{
    public AltitudeManager myAltitude; 
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleAirplaneUI();
    }
    
    private void HandleAirplaneUI()
    { 
        //float speedMPH = airplaneCharacteristics.MPH; 
        float speedMPH = myAltitude.GetRelativeAltitude();
        GetComponent<TMP_Text>().text = "H: " + speedMPH.ToString("F0") + " m";  
        
    }
}
