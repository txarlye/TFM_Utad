using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class InfoPanel : MonoBehaviour
{
    private XRBaseInteractor interactor;
    public float waitingTimeToDeactivate = 10.0f;
    private float timer  = 0.0f;
    private GameObject infoPanelGameObject;
    private bool isTimerActive = false; 
    
    
    private void Start()
    {
        interactor = GetComponentInParent<XRBaseInteractor>();
        if (transform.childCount >= 1) // Asegúrate de tener al menos un hijo
        { 
            infoPanelGameObject = transform.GetChild(0).gameObject; // Asignación del primer hijo --> Buton
        }
        SetActivateButton(false);
    }
    
    private void Update()
    {
        //if (GameManager.Instance.currentState != GameStates.jugar)
        if (interactor && interactor.selectTarget )
        {
            SetActivateButton(false);
            isTimerActive = false; 
        }
        else
        {
            SetActivateButton(true);
            
        }
        
        
    }
    
    private void SetActivateButton(bool activate)
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(activate);
    }
    
    private void OnEnable()
    {
        if (!isTimerActive)
        {
            isTimerActive = true;
            StartCoroutine(DeactivateInfoPanelAfterTime());
        }
    }
    private System.Collections.IEnumerator DeactivateInfoPanelAfterTime()
    {
        yield return new WaitForSeconds(waitingTimeToDeactivate);
        //infoPanelGameObject.SetActive(false); 
        infoPanelGameObject.GetComponent<MeshRenderer>().enabled = false;
        isTimerActive = false;  
    }
    
}
