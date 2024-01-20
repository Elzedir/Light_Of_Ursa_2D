using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.FilePathAttribute;
using static UnityEngine.Rendering.DebugUI;

public class Spawner_Arrow : MonoBehaviour
{
    List<Transform> ArrowSpawners = new();

    PuzzleSet _puzzleSet;
    PuzzleType _puzzleType;
    
    Sprite _arrowSprite;
    float _arrowsSpawned;
    Transform Target;

    float _spawnTime;
    [SerializeField] float _spawnInterval = 0.1f;
    [SerializeField] float _minSpawnInterval = 0.01f;
    [SerializeField] float _maxSpawnInterval = 0.2f;

    void Start()
    {
        _arrowSprite = Resources.Load<Sprite>("Sprites/Arrow_01");

        _puzzleSet = Manager_Puzzle.Instance.Puzzle.PuzzleData.PuzzleSet;
        _puzzleType = Manager_Puzzle.Instance.Puzzle.PuzzleData.PuzzleType;

        foreach (Transform child in transform)
        {
            ArrowSpawners.Add(child);
        }

        Target = GameObject.Find("Focus").transform;
    }

    private void Update()
    {
        if (_spawnTime >= _spawnInterval)
        {
            if (_puzzleType == PuzzleType.Fixed) SpawnArrowFixed();
            else { SpawnArrowRandom(); _spawnInterval = Random.Range(_minSpawnInterval, _maxSpawnInterval); }

            _spawnTime = 0;
        }

        _spawnTime += Time.deltaTime;
    }

    void SpawnArrowRandom()
    {
        int rotation = Random.Range(0, 3); rotation = rotation == 0 ? 0 : (rotation == 1 ? 90 : (rotation == 2 ? 180 : 270));

        if (_puzzleSet == PuzzleSet.Directional) SpawnArrowDirectional(ArrowSpawners[Random.Range(0, ArrowSpawners.Count)], rotation: rotation);

        else if (_puzzleSet == PuzzleSet.AntiDirectional) SpawnArrowAntiDirectional(ArrowSpawners[Random.Range(0, ArrowSpawners.Count)]);
    }

    void SpawnArrowFixed()
    {
        // Set the spawn interval to a specific interval and use the localscale to change how the arrows look and select a spawner for a pattern.
    }

    void SpawnArrowDirectional(Transform spawner, Vector3? move = null, float rotation = 0)
    {
        GameObject arrowGO = new GameObject($"Arrow{_arrowsSpawned}"); _arrowsSpawned++;

        arrowGO.transform.parent = spawner; arrowGO.transform.localPosition = Vector3.zero; arrowGO.transform.localRotation = Quaternion.Euler(0, 0, rotation);

        SpriteRenderer arrowSprite = arrowGO.AddComponent<SpriteRenderer>(); arrowSprite.sprite = _arrowSprite; arrowSprite.sortingLayerName = "Actors";
        
        BoxCollider2D arrowCollider = arrowGO.AddComponent<BoxCollider2D>();

        Arrow arrow = arrowGO.AddComponent<Arrow>(); arrow.Move = move ?? Vector3.down;
    }

    void SpawnArrowAntiDirectional(Transform spawner)
    {
        GameObject arrowGO = new GameObject($"Arrow{_arrowsSpawned}"); _arrowsSpawned++;

        arrowGO.transform.parent = spawner; arrowGO.transform.localPosition = Vector3.zero;

        SpriteRenderer arrowSprite = arrowGO.AddComponent<SpriteRenderer>(); arrowSprite.sprite = _arrowSprite; arrowSprite.sortingLayerName = "Actors";

        arrowGO.AddComponent<BoxCollider2D>();

        Arrow arrow = arrowGO.AddComponent<Arrow>(); arrow.Target = Target;

        arrow.Speed = 10;
    }
}
