using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

public class Manager_Cutscene : MonoBehaviour
{
    public static Manager_Cutscene Instance;

    [Header("Debugging")]
    [SerializeField] bool _playCutscenes = true;

    PlayableDirector _director;

    public Dictionary<string, List<Cutscene>> ScriptedCutscenes;

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); } else if (Instance != this) Destroy(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        _initialiseScriptedCutscenes();
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _director = GetComponent<PlayableDirector>();

        if (ScriptedCutscenes.TryGetValue(scene.name, out List<Cutscene> cutscenes))
        {
            foreach(Cutscene cutscene in cutscenes)
            {
                cutscene.SetDirector(GameObject.Find(cutscene.Name).GetComponent<PlayableDirector>());

                if (cutscene.IsConditionsFulfilled(scene.name))
                {
                    StartCoroutine(PlayCutscene(cutscene.Director));
                }
            }
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public IEnumerator PlayCutscene(PlayableDirector director)
    {
        if (!_playCutscenes) yield break;

        yield return null;

        Manager_Game.Instance.ChangeGameState(GameState.Cinematic);

        director.Play();

        yield return new WaitForSeconds((float)director.duration);

        Manager_Game.Instance.ChangeGameState(GameState.Playing);
    }

    void _initialiseScriptedCutscenes()
    {
        ScriptedCutscenes = new();
        _Old_Tree();
    }

    void _Old_Tree()
    {
        ScriptedCutscenes.Add(
            "Old_Tree",
            new List<Cutscene>
            {
                new Cutscene("Old_Tree_Intro", "Old_Tree", "Chapter 1: Finding Ursus")
            }
            );
    }
}

public class Cutscene
{
    public PlayableDirector Director { get; private set; }
    public bool HasBeenPlayed { get; private set; }
    public string Name { get; private set; }
    public string LevelName { get; private set; }
    public string DisplayText { get; private set; }

    public Cutscene(string name, string levelName, string displayText)
    {
        HasBeenPlayed = false;
        Name = name;
        LevelName = levelName;
        DisplayText = displayText;
    }

    public void SetDirector(PlayableDirector director)
    {
        Director = director;
    }

    public bool IsConditionsFulfilled(string level)
    {
        if (
            !HasBeenPlayed &&
            LevelName == level
            )
        return true;

        return false;
    }
}

public class CinematicWaitPoint
{
    public Vector3 Position;
    public float WaitTime;

    public CinematicWaitPoint(Vector3 position, float waitTime)
    {
        Position = position;
        WaitTime = waitTime;
    }
}
