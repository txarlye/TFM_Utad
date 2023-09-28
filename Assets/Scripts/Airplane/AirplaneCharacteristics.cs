using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirplaneCharacteristics : MonoBehaviour
{
    
    
         private float forwardSpeed;
        private float normalizeMPH;
        private float pitchAngle;
        private float rollAngle; 
        private float currentRPM = 0f; 
        public float maxAltitude = 5000f;
        
    public float ForwardSpeed
    {
        get { return forwardSpeed; }
    }

    private float mph;

    public float getMph()
    {
        return mph;
    }

    public float getnormalizeMPH()
    {
        return normalizeMPH;
    }
    
    public float getCurrentRPM
    {
        get { return currentRPM; }
    }


    public float GetRollAngle()
    {
        return rollAngle;
    }

    public float GetPitchAngle()
    {
        return pitchAngle;
    }

    public float getMaxAltitude()
    {
        return maxAltitude;
    }
     
    
}

