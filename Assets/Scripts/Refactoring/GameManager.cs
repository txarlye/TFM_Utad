using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public enum GameState
    {
        Playing,
        Paused,
        Won,
        Lost,
        Ended
    }

    public GameState currentState;

    public void StartGame()
    {
        currentState = GameState.Playing;
         
    }

    public void PauseGame()
    {
        currentState = GameState.Paused;
        // Pausa el juego aquí
    }

    public void WinGame()
    {
        currentState = GameState.Ended;
        UIManager.Instance.StopGameTime();
        UIManager.Instance.ShowFinalCanvas();
        float currentTime = UIManager.Instance.GetCurrentTime();
        DataManager.Instance.SetBestTime(currentTime, UIManager.Instance.getCompletionPercentage());
        GameObject myAirplane = UIManager.Instance.airplaneRigidbody.gameObject;
        if (myAirplane != null)
        {
            AirplaneControls airplaneControls = myAirplane.GetComponent<AirplaneControls>();
            if (airplaneControls != null)
            {
                airplaneControls.enabled = false;
            }
        }
    }

    public void LoseGame()
    {
        currentState = GameState.Lost;
        UIManager.Instance.ShowFinalCanvas();
    }
}

