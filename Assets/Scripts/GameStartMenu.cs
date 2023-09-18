using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class GameStartMenu : MonoBehaviour
{
    [Header("UI Pages")]
    public GameObject mainMenu;
    public GameObject options;
    public GameObject about;

    [Header("Main Menu Buttons")]
    public Button startButton;
    public Button optionButton;
    public Button aboutButton;
    public Button quitButton;
    
    [Header("Mission Dropdown")]
    public TMP_Dropdown missionDropdown; 
    [Header("Mission Videos")] 
    public GameObject destroyTargetsVideo;
    public GameObject passThroughRingsVideo;
    public GameObject passThroughRingsText;
    public GameObject timedRaceVideo;
    public GameObject timedRaceText;
    public List<Button> returnButtons;

    // Start is called before the first frame update
    void Start()
    {
        EnableMainMenu();
        RefreshDropdown();
        AudioManager.Instance.StopAll(); 
        //Hook events
        startButton.onClick.AddListener(StartGame);
        optionButton.onClick.AddListener(EnableOption);
        aboutButton.onClick.AddListener(EnableAbout);
        quitButton.onClick.AddListener(QuitGame);

        foreach (var item in returnButtons)
        {
            item.onClick.AddListener(EnableMainMenu);
        }
    }

    public void RefreshDropdown()
    { 
        missionDropdown.ClearOptions();
        List<string> missionOptions = new List<string>();
        foreach (MissionManager.MissionType missionType in System.Enum.GetValues(typeof(MissionManager.MissionType)))
        {
            missionOptions.Add(missionType.ToString());
        }
        missionDropdown.AddOptions(missionOptions);

        // Agregar un oyente para el evento onValueChanged del Dropdown
        missionDropdown.onValueChanged.AddListener(delegate { OnMissionChanged(missionDropdown); }); 
    }
    
    public void OnMissionChanged(TMP_Dropdown change)
    {
        MissionManager.MissionType selectedMission = (MissionManager.MissionType)change.value;
        DataManager.Instance.SetMissionType(selectedMission);
        ShowMissionVideo(selectedMission);
    }
    public void ShowMissionVideo(MissionManager.MissionType missionType)
    {
        switch (missionType)
        {
            case MissionManager.MissionType.DestroyTargets:
                destroyTargetsVideo.SetActive(true);
                passThroughRingsVideo.SetActive(false);
                passThroughRingsText.SetActive(false);
                timedRaceText.SetActive(false);
                break;
            case MissionManager.MissionType.PassThroughRings:
                destroyTargetsVideo.SetActive(false);
                passThroughRingsVideo.SetActive(true);
                passThroughRingsText.SetActive(true);
                timedRaceText.SetActive(false);
                break;
            case MissionManager.MissionType.TimedRace:
                destroyTargetsVideo.SetActive(true);
                passThroughRingsVideo.SetActive(false);
                passThroughRingsText.SetActive(false);
                timedRaceText.SetActive(true);
                break;
        }
 
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        HideAll();
        SceneTransitionManager.singleton.GoToSceneAsync(1);
    }

    public void HideAll()
    {
        mainMenu.SetActive(false);
        options.SetActive(false);
        about.SetActive(false);
    }

    public void EnableMainMenu()
    {
        mainMenu.SetActive(true);
        options.SetActive(false);
        about.SetActive(false);
    }
    public void EnableOption()
    {
        mainMenu.SetActive(false);
        options.SetActive(true);
        about.SetActive(false);
    }
    public void EnableAbout()
    {
        mainMenu.SetActive(false);
        options.SetActive(false);
        about.SetActive(true);
    }
}
