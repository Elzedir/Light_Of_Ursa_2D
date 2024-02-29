using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu_Main : MonoBehaviour
{
    Menu_LoadGame _loadGamePanel;

    public void Continue()
    {
        if (!Manager_Data.Instance.HasGameData()) { Debug.LogWarning("Manager_Data has no game data."); return; }

        //Manager_Save.Instance.LoadGame();
    }

    public void NewGame()
    {
        Manager_Game.Instance.StartNewGame();
    }

    public void LoadGame()
    {
        if (_loadGamePanel == null) { Debug.Log("Load found"); _loadGamePanel = Manager_Game.FindTransformRecursively(transform.parent, "LoadGamePanel").gameObject.GetComponent<Menu_LoadGame>(); }

        Debug.Log("Load called");

        _loadGamePanel.ActivateMenu();
    }

    public void Settings()
    {
        Manager_Settings.Instance.OpenMenu();
    }

    public void Exit()
    {
        Application.Quit();
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
            Manager_Puzzle.Instance.LoadPuzzle(puzzle.PuzzleData.PuzzleObjectives.PuzzleDuration, puzzle.PuzzleData.PuzzleObjectives.PuzzleScore);

            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
