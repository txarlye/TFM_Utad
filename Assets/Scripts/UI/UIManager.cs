using System.Collections;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.Serialization; // Importa el espacio de nombres de TextMeshPro

public class UIManager : Singleton<UIManager>
{
    public TextMeshProUGUI puntuacion; // Referencia al componente de texto de la puntuación
    public TextMeshProUGUI velocidad; // Referencia al componente de texto de la velocidad
    public TextMeshProUGUI altura; // Referencia al componente de texto de la altura
    public TextMeshProUGUI distanciaAlObjetivo; // Referencia al componente de texto de la distancia al objetivo
    public TextMeshProUGUI objetivosDestruidos; // Referencia al componente de texto de los objetivos destruidos o anillos pasados
    public GameObject canvasFinal; // Referencia al canvas del final del juego
    public GameObject canvasVolverMenuInicial;
    public TextMeshProUGUI completionText; // Referencia al texto de estadísticas
    public TextMeshProUGUI arosPasadosTotales;
    public Rigidbody airplaneRigidbody; // Referencia al Rigidbody del avión
    public Transform airplaneTransform;
    public AirplaneControls airplaneControls; 
    public TextMeshProUGUI successText;
    public TextMeshProUGUI failText;
    public TextMeshProUGUI typeOfMission;
    [FormerlySerializedAs("textoMejorTiempoPartida")] 
    public TextMeshProUGUI textoTiempoPartida;  
    [FormerlySerializedAs("textoMejorTiempoObjetivo")] 
    public TextMeshProUGUI textoMejorTiempoPartida;
    public float initialAltitude = 0f;
    public AltitudeManager altitudeManager; 
    private int score = 0;
    private int arosPasados = 0;
    private ObjectiveManager currentObjective;
    [Header("Timed Race UI")]
    public TextMeshProUGUI newRecordText;  // Texto para mostrar nuevos récords

    public float minimumCompletionPercentage = 90;
    private float bestGlobalTime;
    private float bestRingTime;
    private float completionPercentage;
    private float endTime;
    
    void Start()
    {
        // Inicializar la puntuación y los aros pasados a 0
        UpdateScore(0);
        UpdateArosPasadosTotales(0);
        UpdateTipeOfMission();
        currentObjective = ObjectiveManager.Instance; 
       
    }
    void Update()
    {
        if (airplaneControls != null)
        {
            float currentSpeed = airplaneControls.GetCurrentSpeed(); // Asume que tienes un método GetCurrentSpeed() en AirplaneControls
            UpdateSpeed(currentSpeed);
        }
        UpdateAltitude();
        UpdateDistanceToTarget();
        
        if (MissionManager.Instance.currentMissionType == MissionManager.MissionType.TimedRace)
        {
            float mejorTiempoPartida = DataManager.Instance.GetBestTime(); 

            textoTiempoPartida.text = $"\nTu tiempo actual de partida: {GetCurrentTime().ToString():F2} s";
            textoMejorTiempoPartida.text = $"\nTu tiempo mejor tiempo: {mejorTiempoPartida:F2} s";
        }
    }

    public float GetCurrentTime()
    { 
        if (GameManager.Instance.currentState == GameManager.GameState.Ended)
        {
            return endTime;
        }
        else
        {
            return Time.time;
        }
    }
    public void StopGameTime()
    {
        endTime = Time.time;
    }
    public void UpdateNewRecordText(float currentTime, float bestTime)
    {
        if (currentTime < bestTime)
        {
            newRecordText.text = $"Has superado tu marca en {bestTime - currentTime:F2} segundos !!! Enhorabuena!!!";
            //newRecordText.gameObject.SetActive(true);
            ShowNewRecordTextWithDelay(1.0f);
        }
        else
        {
            newRecordText.gameObject.SetActive(false);
        }
    }
    private IEnumerator ShowNewRecordTextWithDelay(float delay)
    { 
        yield return new WaitForSeconds(delay); 

        if (newRecordText != null)
        {
            Debug.Log("Activando newRecordText");
            ShowStatistics();
            newRecordText.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("newRecordText es null");
        }
    }
     
    public void UpdateTipeOfMission()
    {
        if (typeOfMission != null)
        {
            MissionManager.MissionType currentType = MissionManager.Instance.currentMissionType;
            string displayText = "";

            switch (currentType)
            {
                case MissionManager.MissionType.DestroyTargets:
                    textoMejorTiempoPartida.gameObject.SetActive(false);
                    textoTiempoPartida.gameObject.SetActive(false);
                    newRecordText.gameObject.SetActive(false);
                    displayText = "Caza al objetivo";
                    break;
                case MissionManager.MissionType.PassThroughRings:
                    textoMejorTiempoPartida.gameObject.SetActive(false);
                    textoTiempoPartida.gameObject.SetActive(false);
                    newRecordText.gameObject.SetActive(false);
                    displayText = "Maniobra de despegue";
                    break;
                case MissionManager.MissionType.TimedRace:
                    textoMejorTiempoPartida.gameObject.SetActive(true);
                    textoTiempoPartida.gameObject.SetActive(true);
                    newRecordText.gameObject.SetActive(false);
                
                    float mejorTiempoPartida = DataManager.Instance.GetBestTime(); 
                
                    
                    textoTiempoPartida.text = $"\nTu tiempo actual de partida: {GetCurrentTime().ToString():F2} s";
                    textoMejorTiempoPartida.text = $"\nTu tiempo mejor tiempo: {mejorTiempoPartida:F2} s";
                    displayText = "Contrarreloj";
                    break;
                default:
                    displayText = "Tipo de misión desconocido";
                    break;
            }

            typeOfMission.text = displayText;
        }
        else
        {
            Debug.LogError("La variable 'typeOfMission' es null.");
        }
    }

    public void UpdateDistanceToTarget()
    {
        if (distanciaAlObjetivo != null && airplaneTransform != null)
        {
            Vector3 targetPosition = currentObjective.GetTargetPosition();
            float distance = Vector3.Distance(airplaneTransform.position, targetPosition);
            distanciaAlObjetivo.text = $"Dist.: {distance.ToString("F2")} m";
            //Debug.Log("desde uimanager distancia ="+distance);
        }
        else
        {
            Debug.LogError("La variable 'distanciaAlObjetivo' o 'airplaneTransform' es null.");
        }
    }
    public void UpdateAltitude()
    {
        if (altura != null && altitudeManager != null)
        {
            float displayedAltitude = altitudeManager.GetRelativeAltitude() + initialAltitude;
            altura.text = $"H: {displayedAltitude.ToString("F2")} m";
        }
        else
        {
            Debug.LogError("La variable 'altura' o 'altitudeManager' es null.");
        }
    }
    public void UpdateScoreText(int score)
    {
        if (puntuacion != null)
        {
            puntuacion.text = score.ToString();
        }
        else
        {
            Debug.LogError("La variable 'puntuación' es null.");
        }
    }

    public void UpdateSpeed(float speed)
    {
        if (velocidad != null)
        {
            velocidad.text = $"V: {speed.ToString("F2")} m/s";
        }
    }

    public void UpdateAltitude(float altitude)
    {
        if (altura != null)
        {
            altura.text = $"Altura: {altitude.ToString("F2")} m";
        }
    }

    public void UpdateDistanceToTarget(float distance)
    {
        if (distanciaAlObjetivo != null)
        {
            distanciaAlObjetivo.text = $"Distancia al objetivo: {distance.ToString("F2")} m";
        }
    }

    public void UpdateObjectivesDestroyed(int destroyed, int total)
    {
        if (objetivosDestruidos != null)
        {
            objetivosDestruidos.text = $"Objetivos: {destroyed}/{total}";
        }
    }

    public float getMinimumCompletionPercentage()
    {
        return minimumCompletionPercentage;
    }
    public void ShowFinalCanvas()
    {
        StartCoroutine(ShowFinalCanvasWithDelay(3.0f));
    }
    public void ShowMenuIntermedio()
    {
        StartCoroutine(ShowMenuIntermedioWithDelay(0.5f));
    }
    
    private IEnumerator ShowMenuIntermedioWithDelay(float delay)
    { 
        yield return new WaitForSeconds(delay); 

        if (canvasVolverMenuInicial != null)
        {
            Debug.Log("Activando canvasFinal");
            ShowStatistics();
            canvasVolverMenuInicial.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("canvasFinal es null");
        }
    }
    private IEnumerator ShowFinalCanvasWithDelay(float delay)
    { 
        yield return new WaitForSeconds(delay); 

        if (canvasFinal != null)
        {
            Debug.Log("Activando canvasFinal");
            ShowStatistics();
            canvasFinal.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("canvasFinal es null");
        }
    }
    public void ShowStatistics()
    {
        int totalRings = ObjectiveManager.Instance.getTotalRings(); // Obtener el total de anillos desde ObjectiveManager
        //float minimumCompletionPercentage = 50.0f; // Puedes establecer esto según tus necesidades o obtenerlo de otra clase
        int arosTotales = ObjectiveManager.Instance.GetTotalObjectives();
        if (arosTotales != 0)
        {
            completionPercentage = (float)arosPasados / (float)arosTotales * 100f;
        }
        else
        {
            completionPercentage = 0;
        }
        
        if (MissionManager.Instance.currentMissionType == MissionManager.MissionType.TimedRace)
        {
            CheckAndDisplayNewRecord();
        }
        
        string completionMessage = $"HEMOS COMPLETADO EL VUELO\n" +
                                   $"DEBES COMPLETAR UN MÍNIMO DEL {minimumCompletionPercentage:F2}% DE ACIERTOS PARA SIGUIENTES FASES.\n\n" +
                                   $"TU HAS TENIDO UN {completionPercentage:F2}% DE ANILLOS COMPLETADOS, ";

        if (completionPercentage >= minimumCompletionPercentage)
        {
            completionMessage += "HAS SUPERADO LA MISIÓN.";
        }
        else
        {
            completionMessage += "NO HAS SUPERADO LA MISIÓN.";
        }

        if (completionText != null)
        {
            completionText.text = completionMessage;
        }
        Debug.Log(completionMessage);
    }
    
    private void CheckAndDisplayNewRecord()
    {
        float currentTime = GetCurrentTime();
        float bestTime = DataManager.Instance.GetBestTime();

        // Comprobar si es un nuevo récord
        if (currentTime < bestTime || bestTime == 0)
        {
            // Actualizar el mejor tiempo
            DataManager.Instance.SetBestTime(currentTime, completionPercentage);

            // Actualizar y mostrar newRecordText
            float timeImproved = bestTime - currentTime;
            newRecordText.text = $"Enhorabuena, has superado tu anterior marca en {timeImproved:F2} segundos!!!";
            newRecordText.gameObject.SetActive(true);
        }
        else
        {
            // Ocultar newRecordText si no es un nuevo récord
            newRecordText.gameObject.SetActive(false);
        }
    }
    public float getCompletionPercentage()
    {
        return completionPercentage;
    }
    public void UpdateArosPasadosTotales(int increment)
    {
        arosPasados += increment;
        int arosTotales = ObjectiveManager.Instance.GetTotalObjectives();
        if (arosPasadosTotales != null)
        {
            arosPasadosTotales.text = $"{arosPasados} de {arosTotales}";
        }
    }
    
    public void UpdateScore(int increment)
    {
        score += increment;
        if (puntuacion != null)
        {
            puntuacion.text = score.ToString();
        }
        else
        {
            Debug.LogError("La variable 'puntuación' es null.");
        }
    }
    
    public void ShowSuccessText()
    {
        successText.gameObject.SetActive(true);
        StartCoroutine(HideMessageAfterDelay(successText, 2f));
    }

    public void ShowFailText()
    {
        failText.gameObject.SetActive(true);
        StartCoroutine(HideMessageAfterDelay(failText, 2f));
    }

    private IEnumerator HideMessageAfterDelay(TextMeshProUGUI text, float delay)
    {
        yield return new WaitForSeconds(delay);
        //airplaneTransform.gameObject.GetComponent<AirplaneControls>().enabled = true;
        text.gameObject.SetActive(false);
    } 
}

