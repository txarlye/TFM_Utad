using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeManager : Singleton<ModeManager>
{
    public enum ClimateMode
    {
        Cloudy,     // Nublado
        Rainy,      // Lluvioso
        Sunny,      // Soleado
        Stormy,     // Tormentoso
        Overcast,   // Cubierto
        Clear,      // Despejado
        Foggy       // Neblinoso
    }

    public enum AirplaneModel
    {
        F5,
        F18
    }
    
    public enum GameMode
    {
        Despegue,
        ManiobraEvasion,
        Personalizado
    }
    
    public ClimateMode MyClimateMode ;
    public AirplaneModel MyAirplaneModel ;
    public GameMode MyGameMode ;
    
    public ClimateMode GetMyClimateMode()
    {
        return MyClimateMode;
    } 
    public void SetClimateMode(ClimateMode newClimateMode)
    {
        MyClimateMode = newClimateMode;
    }
    public AirplaneModel GetMyAirplaneModel()
    {
        return MyAirplaneModel;
    } 
    public void SetClimateMode(AirplaneModel newAirplaneModel)
    {
        MyAirplaneModel = newAirplaneModel;
    } 
    public GameMode GetMyGameMode()
    {
        return MyGameMode;
    } 
    public void SetGameMode(GameMode newMyGameMode)
    {
        MyGameMode = newMyGameMode;
    }  
}
