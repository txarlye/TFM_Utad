using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ButtonInteraction : MonoBehaviour
{
    public GameObject button; 
    public Canvas myInfoCanvas;
    public GameObject InfoPanelGameObject;
    public UnityEvent onPress;
    public UnityEvent onRelease;
  
    
    GameObject presser;
    bool isPressed;

    void Start()
    {

        isPressed = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isPressed)
        {
            button.transform.localPosition = new Vector3(0, 0, 0);
            presser = other.gameObject;
            onPress.Invoke();
            isPressed = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
       
        button.transform.localPosition = new Vector3(0, 0.005f, 0);
        onRelease.Invoke();
        isPressed = false;
        
    }

    public void SpawnSphere()
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.localScale = new Vector3(0.116120003f, 0.116120003f, 0.116120003f);
        sphere.transform.localPosition = new Vector3(-0.2f, 1.2f, 0.5f);
        sphere.AddComponent<Rigidbody>();
    }
    public void changeToNextState()
    {
        //GameManager.Instance.changeToNextState();
    }

    public void showPanelInfoCanvas()
    {
        if (myInfoCanvas != null)
        {
            // Si no tiene una cámara asignada, intenta obtener la cámara principal de la escena.
            if (myInfoCanvas.worldCamera == null)
            {
                myInfoCanvas.worldCamera = Camera.main;
            }

            // Activa el Canvas.
            //myInfoCanvas.gameObject.SetActive(true);
            myInfoCanvas.gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
    }
    public void hidePanelInfoCanvas()
    {
        if (myInfoCanvas != null)
        {
            //myInfoCanvas.gameObject.SetActive(false);
            StartCoroutine(HideCanvasAfterTime(3.0f));
        }
    }
    private IEnumerator HideCanvasAfterTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    
        if (myInfoCanvas != null)
        {
            myInfoCanvas.gameObject.SetActive(false);
            InfoPanelGameObject.gameObject.SetActive(false);
        }
    }
}
