using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager_Spawner : MonoBehaviour, IDataPersistence
{
    public static Manager_Spawner Instance;
    List<GameObject> _spawners = new();
    public GameObject PuzzleSpawner { get; private set; }
    Dictionary<string, PuzzleData> _allPuzzleData = new();

    public delegate void OnPuzzleStatesRestoredDelegate();
    public static event OnPuzzleStatesRestoredDelegate OnPuzzleStatesRestored;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); } else if (Instance != this) Destroy(gameObject);
        AssignSpawners();
    }

    public void SaveData(GameData data)
    {
        foreach (var puzzle in _allPuzzleData)
        {
            data.Puzzles[puzzle.Key] = JsonConvert.SerializeObject(puzzle.Value.PuzzleSaveData, Formatting.Indented);
        }
    }

    public void LoadData(GameData data)
    {
        StartCoroutine(OnLoadNumerator(data));
    }

    IEnumerator OnLoadNumerator(GameData data)
    {
        yield return new WaitForSeconds(0.1f);

        AssignSpawners();

        RestorePuzzleStates(data);
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
    public void RestorePuzzleStates(GameData data)
    {
        if (PuzzleSpawner == null) { Debug.Log("Puzzle Spawner not present in scene."); return; }

        foreach (Transform child in PuzzleSpawner.transform)
        {
            if (child.TryGetComponent<Interactable_Puzzle>(out Interactable_Puzzle puzzle))
            {
                LoadPuzzleState(puzzle); SavePuzzleState(puzzle.PuzzleData);
            }
        }

        foreach (var puzzleData in data.Puzzles)
        {
            _allPuzzleData[puzzleData.Key].LoadData(JsonConvert.DeserializeObject<PuzzleSaveData>(puzzleData.Value));
        }

        foreach (Transform child in PuzzleSpawner.transform)
        {
            if (child.TryGetComponent<Interactable_Puzzle>(out Interactable_Puzzle puzzle))
            {
                if (puzzle.PuzzleData.PuzzleState.PuzzleCompleted) puzzle.CompletePuzzle();
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
                    data.PuzzleState,
                    data.PuzzleObjectives,
                    data.IceWallData
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

        Manager_Audio managerAudio = GameObject.Find("Manager_Audio").GetComponent<Manager_Audio>();

        int i = 0;

        foreach (Transform child in PuzzleSpawner.transform)
        {
            if (child.GetComponent<Interactable_Puzzle>().PuzzleData.PuzzleState.PuzzleCompleted)
            {
                managerAudio.LocalParameters[i].SetValue(1);
            }

            i++;
        }
    }
}
