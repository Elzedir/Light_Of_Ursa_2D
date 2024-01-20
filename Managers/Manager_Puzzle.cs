using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum PuzzleSet
{
    None,
    Directional,
    AntiDirectional,
    Light,
    AntiLight,
    XOYXOY,
    AntiXOYXOY,
    FlappyInvaders,
    AntiFlappyInvaders,
    Air,
    AntiAir,
    BulletHell,
    Telescope,
    MouseMaze,
    IceBlock,
    UndyneSpear
}

public enum PuzzleType
{
    Random,
    Fixed
}

public class Manager_Puzzle : MonoBehaviour
{
    public static Manager_Puzzle Instance;

    public event Action OnTakeHit;

    public Interactable_Puzzle Puzzle;

    bool _puzzleActive;
    float _puzzleDuration;
    float _puzzleTime;

    int _health = 3;
    float _invulnerabilityTime = 5f;
    public bool Invulnerable { get; private set; } = false;

    public Sprite BulletSprite { get; private set; }

    void Awake()
    {
        if (Instance == null) { Instance = this; } else if (Instance != this) Destroy(gameObject);
    }

    void Start()
    {
        BulletSprite = Resources.Load<Sprite>("Sprites/Grid");
        //BulletParent = GameObject.Find("BulletParent").transform;
    }

    public void LoadPuzzle(float duration, float score)
    {
        bool setActive = false;

        if (duration == 0 && score == 0) { Debug.Log("Duration and score cannot both be 0."); return; }

        Manager_Game.Instance.FindTransformRecursively(transform, Puzzle.PuzzleData.PuzzleSet.ToString()).gameObject.SetActive(true);

        if (duration > 0) { _puzzleDuration = duration; _puzzleActive = true; }

        switch (Puzzle.PuzzleData.PuzzleSet)
        {
            case PuzzleSet.Directional:
            case PuzzleSet.AntiDirectional:
            case PuzzleSet.FlappyInvaders:
                setActive = true;
                break;
            case PuzzleSet.XOYXOY:
                setActive = false;
                break;
            default:
                return;
        }

        Manager_Game.Instance.FindTransformRecursively(GameObject.Find("Canvas").transform, "Puzzle_Information").gameObject.SetActive(setActive);
    }

    void Update()
    {
        if (!_puzzleActive) return;

        if (_puzzleTime > _puzzleDuration)
        {
            PuzzleEnd(true);
            return;
        }

        _puzzleTime += Time.deltaTime;
    }

    public void PuzzleEnd(bool completed)
    {
        _puzzleActive = false;
        GameObject.Find(Puzzle.PuzzleData.PuzzleSet.ToString()).SetActive(false);

        if (completed) { Manager_Game.Instance.LoadScene(puzzle: Puzzle); } else Manager_Game.Instance.LoadScene();
    }

    public void TakeDamage()
    {
        _health -= 1;

        OnTakeHit?.Invoke();

        StartCoroutine(InvulnerabilityPhase());
    }

    IEnumerator InvulnerabilityPhase()
    {
        Invulnerable = true;
        yield return new WaitForSeconds(_invulnerabilityTime);
        Invulnerable = false;
    }
}
