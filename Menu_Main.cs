using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu_Main : MonoBehaviour
{
    Menu_LoadGame _loadGamePanel;

    void Start()
    {
        Manager_Game.FindTransformRecursively(transform, "ProfileText").GetComponent<TextMeshProUGUI>().text = Manager_Data.Instance.GetCurrentlySelectedProfile();
    }

    public void Continue()
    {
        if (!Manager_Data.Instance.HasGameData()) { Debug.LogWarning("Manager_Data has no game data."); return; }

        Manager_Data.Instance.ChangeSelectedProfileId(Manager_Data.Instance.GetCurrentlySelectedProfile());
        Manager_Game.Instance.LoadScene(Manager_Game.Instance.SceneName);
    }

    public void NewGame()
    {
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

    public void SwitchProfile()
    {
        Manager_Game.FindTransformRecursively(transform.parent, "SwitchProfilePanel").gameObject.SetActive(true);
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
