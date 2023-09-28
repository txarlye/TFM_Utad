using System;
using System.Collections;
using UnityEngine;

public class AirplaneStateManager : Singleton<AirplaneStateManager>
{
    public enum AirplaneState
    {
        EnTierra,
        Despegando,
        EnVuelo,
        SubiendoEnAlturaOAcelerando
    }

    [SerializeField]private AirplaneState currentState;
 
   

    public AirplaneState GetCurrentState()
    {
        return currentState;
    }

    public void SetState(AirplaneState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
            HandleStateChange();
        }
    }

    private void Start()
    {
        HandleStateChange();
    }

    private void HandleStateChange()
    {
        AudioManager.Instance.StopAll();
        switch (currentState)
        {
            case AirplaneState.EnTierra:
                AudioManager.Instance.Play("CabinaEnTierra");
                break;
            case AirplaneState.Despegando:
                AudioManager.Instance.Play("CabinaDespegue");
                StartCoroutine(WaitForSoundToEnd("CabinaDespegue", AirplaneState.EnVuelo));
                break;
            case AirplaneState.EnVuelo:
                AudioManager.Instance.Play("CabinaDuranteVuelo");
                break;
            case AirplaneState.SubiendoEnAlturaOAcelerando:
                AudioManager.Instance.Play("CabinaSubiendoEnAltura");
                StartCoroutine(WaitForSoundToEnd("CabinaSubiendoEnAltura", AirplaneState.EnVuelo));
                break;
            default:
                break;
        }
    }
    
    private IEnumerator WaitForSoundToEnd(string soundName, AirplaneState nextState)
    {
        while (AudioManager.Instance.IsPlaying(soundName))
        {
            yield return null;
        }
        SetState(nextState);
    }
}

