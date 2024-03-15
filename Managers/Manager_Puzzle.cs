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
    IceWall,
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

    public void LoadPuzzle()
    {
        bool setActive = false;

        Manager_Game.FindTransformRecursively(transform, Puzzle.PuzzleSet.ToString()).gameObject.SetActive(true);

        if (Puzzle.PuzzleData.PuzzleObjectives.PuzzleDuration > 0) { _puzzleDuration = Puzzle.PuzzleData.PuzzleObjectives.PuzzleDuration; _puzzleActive = true; }

        switch (Puzzle.PuzzleSet)
        {
            case PuzzleSet.Directional:
            case PuzzleSet.AntiDirectional:
            case PuzzleSet.FlappyInvaders:
            case PuzzleSet.MouseMaze:
                setActive = true;
                break;
            case PuzzleSet.IceWall:
                Manager_Game.FindTransformRecursively(transform, "Grid_IceWall").gameObject.GetComponent<Spawner_IceWall>().InitialisePuzzle(Puzzle.IceWallTypes, Puzzle.PuzzleData.IceWallData);
                setActive = true;
                break;
            case PuzzleSet.XOYXOY:
                setActive = false;
                break;
            default:
                return;
        }

        Manager_Game.FindTransformRecursively(GameObject.Find("Canvas").transform, "Puzzle_Information").gameObject.SetActive(setActive);
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
        GameObject.Find(Puzzle.PuzzleSet.ToString()).SetActive(false);

        if (completed) { Manager_Game.Instance.LoadScene(puzzle: Puzzle); } else Manager_Game.Instance.LoadScene();
    }

    public event Action OnTakeHit;
    public void TakeDamage()
    {
        _health -= 1;

        OnTakeHit?.Invoke();

        StartCoroutine(InvulnerabilityPhase());
    }

    public event Action<string> OnUseStamina;
    public void UseStamina(string stamina)
    {
        OnUseStamina?.Invoke(stamina);
    }

    IEnumerator InvulnerabilityPhase()
    {
        Invulnerable = true;
        yield return new WaitForSeconds(_invulnerabilityTime);
        Invulnerable = false;
    }
}
