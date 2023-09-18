using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class Airplane_Tachometer : MonoBehaviour
    {
 
        [Header("Tachometer Properties")]
        public AirplaneCharacteristics airplaneCharacteristics;
        public RectTransform pointer;
        public float maxRPM = 3500f;
        public float maxRotation = 312;
        public float pointerSpeed = 2f;

        private float finalRotation = 0f;

        private bool canHandle;
        public GameObject myPrefab;

        private void Start()
        {
            canHandle = false;
            StartCoroutine(CheckForAirplane());
        }

        private IEnumerator CheckForAirplane()
        {
            while (myPrefab == null)
            {
                myPrefab = GameObject.Find("Player"); 
                yield return null;
            }

            if (myPrefab != null)
            {
                airplaneCharacteristics = myPrefab.GetComponent<AirplaneCharacteristics>();
                if (airplaneCharacteristics != null)
                {
                    canHandle = true;
                }
            }
        }

        public void HandleAirplaneUI()
        {
            if (airplaneCharacteristics && pointer)
            {
                // Calcula la rotación de la aguja basada en las RPM
                float normalizedRPM = Mathf.InverseLerp(0f, maxRPM, airplaneCharacteristics.getCurrentRPM);
                float wantedRotation = maxRotation * -normalizedRPM;

                // Aplica la rotación a la aguja
                finalRotation = Mathf.Lerp(finalRotation, wantedRotation, Time.deltaTime * pointerSpeed);
                pointer.rotation = Quaternion.Euler(0f, 0f, finalRotation);
            }
        }

        public void Update()
        {
            if (canHandle)
            {
                HandleAirplaneUI();
            }
        }
    }

