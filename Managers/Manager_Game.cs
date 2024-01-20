using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    Cinematic,
    Loading,
    PlayerDead,
    Puzzle
}

public class Manager_Game : MonoBehaviour
{
    public static Manager_Game Instance;

    public GameState CurrentState;

    public Player Player;
    Vector3 _playerLastPosition;
    [SerializeField] public float InteractRange { get; private set; } = 1;

    public string LastScene;

    Interactable_Puzzle _currentPuzzle;

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); } else if (Instance != this) Destroy(gameObject);

        if (string.IsNullOrEmpty(LastScene)) LastScene = SceneManager.GetActiveScene().name;

        if(SceneManager.GetActiveScene().name == "Main_Menu") CurrentState = GameState.MainMenu;

        Manager_Spawner.OnPuzzleStatesRestored += OnPuzzleStatesRestored;
    }

    void OnDestroy()
    {
        Manager_Spawner.OnPuzzleStatesRestored -= OnPuzzleStatesRestored;
    }

    public Transform FindTransformRecursively(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;

            Transform result = FindTransformRecursively(child, name);

            if (result != null) return result;
        }
        return null;
    }

    public void LoadScene(string nextScene = null, Interactable_Puzzle puzzle = null)
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene != "Puzzle") { LastScene = currentScene; Manager_Spawner.Instance.StorePuzzleStates(); }

        if (nextScene == "Puzzle" && puzzle == null) { Debug.Log("Cannot load Puzzle Scene without a puzzle"); return; }
        if (nextScene == "Puzzle") { _playerLastPosition = Player.transform.position; _currentPuzzle = puzzle; }

        else if (nextScene == null && currentScene != "Puzzle") { Debug.Log("Cannot load null scene."); return; }
        else if (nextScene == null) nextScene = LastScene;

        SceneManager.LoadScene(nextScene);
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != nextScene) { Debug.Log("Did not load the correct scene."); return; }

            if (scene.name == "Puzzle")
            {
                ChangeGameState(GameState.Puzzle);
                Manager_Puzzle.Instance.Puzzle = _currentPuzzle;
                Manager_Puzzle.Instance.LoadPuzzle(_currentPuzzle.PuzzleData.PuzzleDuration, _currentPuzzle.PuzzleData.PuzzleScore);
            }
            else 
            {
                ChangeGameState(GameState.Playing);
                Player = FindFirstObjectByType<Player>();
                Player.transform.position = currentScene == "Puzzle" ? _playerLastPosition : FindTransformRecursively(GameObject.Find("Spawners").transform, currentScene).position;
            }

            LastScene = currentScene;

            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    void OnPuzzleStatesRestored()
    {
        if (LastScene == "Puzzle" && _currentPuzzle.PuzzleData != null) Manager_Spawner.Instance.CompletePuzzle(_currentPuzzle.PuzzleData.PuzzleID);
    }

    public void StartNewGame()
    {
        LastScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("Ursus_Cave");
        SceneManager.sceneLoaded += OnSceneLoaded;

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != "Ursus_Cave")
            {
                Debug.Log("Did not load the correct scene.");
                return;
            }

            //Manager_Cutscene.Instance.PlayCutscene("Ursus_Cave_Intro");
            ChangeGameState(GameState.Playing); // Remove when game is normal.

            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    public void ChangeGameState(GameState newState)
    {
        switch (CurrentState)
        {
            case GameState.Playing:
                break;
            case GameState.Paused:
                break;
            case GameState.Loading:
                // hide the loading screen
                // switch to the next level
                break;
            case GameState.Cinematic:
                // Hide the cinematic
                // Resume player controls
                break;
            case GameState.PlayerDead:
                // Reset the player
                break;
        }

        CurrentState = newState;

        switch (CurrentState)
        {
            case GameState.Playing:
                Time.timeScale = 1f;
                // enbale player controls
                // resume normal sound and music
                break;
            case GameState.Paused:
                Time.timeScale = 0f;
                // disable player controls
                // play pause music and stop game sounds
                break;
            case GameState.Loading:
                // play a loading screen and slowly load the map in
                // begin loading the next level
                break;
            case GameState.Cinematic:
                // Play the cinematic
                // Stop player controls
                break;
            case GameState.PlayerDead:
                // Stop the player controls and enemies moving
                // Play game over sound effect
                break;
        }
    }
    public void SetPlayer()
    {
        Player = FindFirstObjectByType<Player>();
    }
    void OnDrawGizmos()
    {
        if (Player == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Player.transform.position, InteractRange);
    }
}
