using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airplane_Attitude : MonoBehaviour
{
    [Header("Attitude Indicator Properties")]
    public AirplaneCharacteristics airplaneCharacteristics; // Cambiado a AirplaneCharacteristics en lugar de Airplane
    public RectTransform bgRect;
    public RectTransform arrowRect;
    public AltitudeManager myInitialH;
    private Vector3 initialEulerAngles;
    private float initialH;
    private float actualH;
    private float maxH;
    private float maxVerticalChange;

    private void Start()
    {
        
        initialH = myInitialH.GetRelativeAltitude();
        maxH = airplaneCharacteristics.getMaxAltitude();
        initialEulerAngles = bgRect.localEulerAngles;
        
        // Calcula la mitad del tamaño de bgRect y la usamos como la altura máxima de cambio vertical
        maxVerticalChange = bgRect.sizeDelta.y * 0.5f;
    }

    public void HandleAirplaneUI()
    {
        
        // Manejamos la altura:
        actualH = myInitialH.GetRelativeAltitude();
        // Calcula el cambio vertical en función de la altura relativa y maxH
        float verticalChange = Mathf.Clamp(actualH - initialH, -maxVerticalChange, maxVerticalChange);
        float normalizedVerticalChange = Mathf.InverseLerp(-maxVerticalChange, maxVerticalChange, verticalChange);

  
        // Manejamos la rotación:
                float rollAngle = airplaneCharacteristics.GetRollAngle();
                float pitchAngle = airplaneCharacteristics.GetPitchAngle();

                if (bgRect)
                { 
                    Quaternion rollRotation = Quaternion.Euler(0f, 0f, -rollAngle);
 
                    bgRect.transform.rotation = rollRotation;
                } 
    }

    private void Update()
    {
        HandleAirplaneUI();
    }
}