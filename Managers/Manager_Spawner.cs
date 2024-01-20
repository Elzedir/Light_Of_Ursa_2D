using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager_Spawner : MonoBehaviour
{
    public static Manager_Spawner Instance;
    List<GameObject> _spawners = new();
    public GameObject PuzzleSpawner { get; private set; }
    Dictionary<string, PuzzleData> _allPuzzleData = new();

    public delegate void OnPuzzleStatesRestoredDelegate();
    public static event OnPuzzleStatesRestoredDelegate OnPuzzleStatesRestored;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); SceneManager.sceneLoaded += OnSceneLoad; } else if (Instance != this) Destroy(gameObject);
        AssignSpawners();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
    }

    void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Puzzle" || scene.name == "Main_Menu") return;
        StartCoroutine(OnSceneLoadNumerator());
    }

    IEnumerator OnSceneLoadNumerator()
    {
        yield return new WaitForSeconds(0.1f);

        AssignSpawners();
        RestorePuzzleStates();
    }

    void AssignSpawners()
    {
        GameObject spawnersParent = GameObject.Find("Spawners");

        if (spawnersParent == null) return;
        
        foreach (Transform child in spawnersParent.transform)
        {
            if (!_spawners.Contains(child.gameObject)) _spawners.Add(child.gameObject);

            if (child.gameObject.name == "PuzzleSpawner") PuzzleSpawner = child.gameObject;
        }

    }
    public void StorePuzzleStates()
    {
        foreach (Transform child in PuzzleSpawner.transform)
        {
            if (child.TryGetComponent<Interactable_Puzzle>(out Interactable_Puzzle puzzle)) SavePuzzleState(puzzle.PuzzleData);
        }
    }
    public void RestorePuzzleStates()
    {
        if (PuzzleSpawner == null) { Debug.Log("Puzzle Spawner not present in scene."); return; }

        foreach (Transform child in PuzzleSpawner.transform)
        {
            if (child.TryGetComponent<Interactable_Puzzle>(out Interactable_Puzzle puzzle))
            {
                LoadPuzzleState(puzzle); SavePuzzleState(puzzle.PuzzleData);
            }
        }

        OnPuzzleStatesRestored?.Invoke();
    }
    void SavePuzzleState(PuzzleData data)
    {
        if (!_allPuzzleData.ContainsKey(data.PuzzleID)) _allPuzzleData.Add(data.PuzzleID, new PuzzleData
                    (
                    data.PuzzleID,
                    data.PuzzleSet,
                    data.PuzzleType,
                    data.PuzzleRepeatable,
                    data.PuzzleCompleted,
                    data.PuzzleDuration,
                    data.PuzzleScore
                    ));
        else _allPuzzleData[data.PuzzleID] = data;
    }
    void LoadPuzzleState(Interactable_Puzzle puzzle)
    {
        if (_allPuzzleData.TryGetValue(puzzle.PuzzleData.PuzzleID, out PuzzleData data)) puzzle.PuzzleData = data;
    }
    public void CompletePuzzle(string puzzleID)
    {
        if (_allPuzzleData.ContainsKey(puzzleID)) GameObject.Find(puzzleID).GetComponent<Interactable_Puzzle>().CompletePuzzle(); 
        else Debug.Log($"_allPuzzleData does not contain {puzzleID}");
    }
}

[System.Serializable]
public class PuzzleData
{
    public string PuzzleID;
    public PuzzleSet PuzzleSet;
    public PuzzleType PuzzleType;
    public bool PuzzleRepeatable = false;
    public bool PuzzleCompleted;
    public float PuzzleDuration;
    public float PuzzleScore;

    public PuzzleData
        (
        string puzzleID,
        PuzzleSet puzzleSet,
        PuzzleType puzzleType,
        bool puzzleRepeatable,
        bool puzzleCompleted,
        float duration,
        float score
        )
    {
        PuzzleID = puzzleID;
        PuzzleSet = puzzleSet;
        PuzzleType = puzzleType;
        PuzzleRepeatable = puzzleRepeatable;
        PuzzleCompleted = puzzleCompleted;
        PuzzleDuration = duration;
        PuzzleScore = score;
    }
}
