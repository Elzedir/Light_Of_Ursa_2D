using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu_Main : MonoBehaviour
{
    Menu_LoadGame _loadGamePanel;
    Transform _profileContainer;

    void Start()
    {
        Manager_Game.FindTransformRecursively(transform, "ProfileText").GetComponent<TextMeshProUGUI>().text = Manager_Data.Instance.GetActiveProfile().Name;
    }

    public void Continue()
    {
        if (!Manager_Data.Instance.HasGameData()) { Debug.LogWarning("Manager_Data has no game data."); return; }

        Manager_Data.Instance.ChangeSelectedProfileId(Manager_Data.Instance.GetActiveProfile().Name);
        Manager_Game.Instance.LoadScene(Manager_Game.Instance.SceneName);
    }

    public void NewGame()
    {
        Manager_Data.Instance.NewGame(Manager_Data.Instance.GetActiveProfile().Name);
        Manager_Game.Instance.StartNewGame();
    }

    public void LoadGame()
    {
        if (_loadGamePanel == null) { Debug.Log("Load found"); _loadGamePanel = Manager_Game.FindTransformRecursively(transform.parent, "LoadGamePanel").gameObject.GetComponent<Menu_LoadGame>(); }

        Debug.Log("Load called");

        _loadGamePanel.ActivateMenu(this);
    }

    public void Settings()
    {
        Manager_Settings.Instance.OpenMenu();
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void OpenSwitchProfilePanel()
    {
        if (!_profileContainer) _profileContainer = Manager_Game.FindTransformRecursively(transform.parent, "ProfileContainer");

        GameObject switchProfilePanel= Manager_Game.FindTransformRecursively(transform.parent, "SwitchProfilePanel").gameObject;
        switchProfilePanel.SetActive(true);

        foreach (Profile profile in Manager_Data.Instance.GetAllProfiles())
        {
            if(profile.Name == "Unity") continue;
            GameObject profileGO = new GameObject(profile.Name);
            RectTransform rectTransform = profileGO.AddComponent<RectTransform>();
            profileGO.transform.SetParent(_profileContainer);
            ProfileUI profileUI = profileGO.AddComponent<ProfileUI>();
            profileUI.Initialise(this, profile.Name);
        }
    }

    public void CloseSwitchProfile()
    {
        foreach (Transform child in _profileContainer)
        {
            Destroy(child.gameObject);
        }

        Manager_Game.FindTransformRecursively(transform.parent, "SwitchProfilePanel").gameObject.SetActive(false);
    }

    public void SwitchProfile(string profile)
    {
        Manager_Data.Instance.ChangeSelectedProfileId(profile);
        CloseSwitchProfile();
        Manager_Game.FindTransformRecursively(transform, "ProfileText").GetComponent<TextMeshProUGUI>().text = Manager_Data.Instance.GetActiveProfile().Name;
    }

    public void CreateNewProfile()
    {
        Manager_Game.FindTransformRecursively(transform.parent, "CreateNewProfilePanel").gameObject.SetActive(true);
    }

    public void Create()
    {
        string profileName = Manager_Game.FindTransformRecursively(transform.parent, "InputField").gameObject.GetComponent<InputField>().text;

        Debug.Log(profileName);

        if (profileName == "") { OpenWarningPopup("Profile name is empty."); return; }

        IEnumerable<DirectoryInfo> directoryInfoList = new DirectoryInfo(Application.persistentDataPath).EnumerateDirectories();

        foreach (DirectoryInfo directoryInfo in directoryInfoList)
        {
            if (directoryInfo.Name == profileName) { OpenWarningPopup("Profile already exists."); return; }
        }

        Manager_Game.FindTransformRecursively(transform.parent, "CreateNewProfilePanel").gameObject.SetActive(false);

        Debug.Log(Application.persistentDataPath);
        Debug.Log(Manager_Data.SaveFileName);
        Debug.Log(Manager_Data.Instance._useEncryption);

        Profile newProfile = new Profile(profileName, 0, null, Manager_Data.Instance._useEncryption);

        Debug.Log(newProfile);

        newProfile.Save("", new GameData(profileName), profileName, true);

        CloseSwitchProfile();
        OpenSwitchProfilePanel();
    }

    public void CloseCreateNewProfilePanel()
    {
        Manager_Game.FindTransformRecursively(transform.parent, "CreateNewProfilePanel").gameObject.SetActive(false);
    }

    public void OpenWarningPopup(string text)
    {
        GameObject warningPanel = Manager_Game.FindTransformRecursively(transform.parent, "WarningPanel").gameObject;
        warningPanel.SetActive(true);
        Manager_Game.FindTransformRecursively(warningPanel.transform, "Warning").GetComponentInChildren<TextMeshProUGUI>().text = text;
    }

    public void CloseWarningPopup()
    {
        Manager_Game.FindTransformRecursively(transform.parent, "WarningPanel").gameObject.SetActive(false);
    }

    public void Directional()
    {
        _runTestPuzzle(PuzzleSet.Directional);
    }

    public void AntiDirectional()
    {
        _runTestPuzzle(PuzzleSet.AntiDirectional);
    }

    public void XOYXOY()
    {
        _runTestPuzzle(PuzzleSet.XOYXOY);
    }

    public void FlappyInvaders()
    {
        _runTestPuzzle(PuzzleSet.FlappyInvaders);
    }

    public void MouseMaze()
    {
        _runTestPuzzle(PuzzleSet.MouseMaze);
    }

    public void IceWall()
    {
        _runTestPuzzle(PuzzleSet.IceWall);
    }

    void _runTestPuzzle(PuzzleSet puzzleSet)
    {
        Transform puzzleTransform = Manager_Game.FindTransformRecursively(transform, puzzleSet.ToString());
        Interactable_Puzzle puzzle = puzzleTransform.GetComponent<Interactable_Puzzle>();

        SceneManager.LoadSceneAsync("Puzzle");
        SceneManager.sceneLoaded += OnSceneLoaded;

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Manager_Game.Instance.ChangeGameState(GameState.Puzzle);
            Manager_Puzzle.Instance.Puzzle = puzzle;
            Manager_Puzzle.Instance.LoadPuzzle();

            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
