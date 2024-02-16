using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu_Main : MonoBehaviour
{
    public void Continue()
    {
        //Manager_Save.Instance.LoadGame();
    }

    public void NewGame()
    {
        Manager_Game.Instance.StartNewGame();
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
        RunTestPuzzle(PuzzleSet.Directional);
    }

    public void AntiDirectional()
    {
        RunTestPuzzle(PuzzleSet.AntiDirectional);
    }

    public void XOYXOY()
    {
        RunTestPuzzle(PuzzleSet.XOYXOY);
    }

    public void FlappyInvaders()
    {
        RunTestPuzzle(PuzzleSet.FlappyInvaders);
    }

    public void MouseMaze()
    {
        RunTestPuzzle(PuzzleSet.MouseMaze);
    }

    public void IceWall()
    {
        RunTestPuzzle(PuzzleSet.IceWall);
    }

    void RunTestPuzzle(PuzzleSet puzzleSet)
    {
        Transform puzzleTransform = Manager_Game.Instance.FindTransformRecursively(transform, puzzleSet.ToString());
        Interactable_Puzzle puzzle = puzzleTransform.GetComponent<Interactable_Puzzle>();

        SceneManager.LoadScene("Puzzle");
        SceneManager.sceneLoaded += OnSceneLoaded;

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Manager_Game.Instance.ChangeGameState(GameState.Puzzle);
            Manager_Puzzle.Instance.Puzzle = puzzle;
            Manager_Puzzle.Instance.LoadPuzzle(puzzle.PuzzleData.PuzzleDuration, puzzle.PuzzleData.PuzzleScore);

            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
