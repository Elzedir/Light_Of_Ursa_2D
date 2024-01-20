using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

public class Manager_Cutscene : MonoBehaviour
{
    public static Manager_Cutscene Instance;
    PlayableDirector _director;
    bool _cutscenePlayed = false;

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); } else if (Instance != this) Destroy(gameObject);
        _director = GetComponent<PlayableDirector>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (_director.state == PlayState.Playing && Manager_Game.Instance.CurrentState != GameState.Cinematic)
        {
            Manager_Game.Instance.ChangeGameState(GameState.Cinematic);
        }
        else if (_director.state != PlayState.Playing && Manager_Game.Instance.CurrentState == GameState.Cinematic && _cutscenePlayed)
        {
            Manager_Game.Instance.ChangeGameState(GameState.Playing);
            _cutscenePlayed = false;
        }
    }

    public void PlayCutscene(string name)
    {
        TimelineAsset cutscene = Resources.Load<TimelineAsset>($"Cutscenes/{name}");
        _director.playableAsset = cutscene;
        _director.Play();
        _cutscenePlayed = true;
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
