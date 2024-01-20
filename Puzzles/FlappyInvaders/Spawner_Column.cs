using System.Collections.Generic;
using UnityEngine;

public class Spawner_Column : MonoBehaviour
{
    List<Transform> _columnSpawners = new();
    List<Transform> _mineSpawners = new();

    Transform _bulletParent;

    PuzzleSet _puzzleSet;
    PuzzleType _puzzleType;

    Sprite _columnSprite;
    Sprite _mineSprite;
    int _columnsSpawned;
    int _minesSpawned;

    public float ColumnSpeed = 1;

    float _columnSpawnTime;
    float _columnSpawnInterval;
    float _minColumnInterval = 0.5f;
    float _maxColumnInterval = 1f;

    float _mineSpawnTime;
    float _mineSpawnInterval;
    float _minMineInterval = 0.3f;
    float _maxMineInterval = 1f;

    float _minColumnHeight = 1f;
    float _maxColumnHeight = 3.5f;

    void Start()
    {
        foreach (Transform child in transform)
        {
            switch (child.name)
            {
                case "ColumnSpawner": foreach (Transform grandchild in child) _columnSpawners.Add(grandchild); break;
                case "MineSpawner": foreach (Transform grandchild in child) _mineSpawners.Add(grandchild); break;
                case "BulletParent": _bulletParent = child; break;
                default: break;
            }
        }

        _columnSprite = Resources.Load<Sprite>("Sprites/Grid");
        _mineSprite = Resources.Load<Sprite>("Sprites/Mine");

        _puzzleSet = Manager_Puzzle.Instance.Puzzle.PuzzleData.PuzzleSet;
        _puzzleType = Manager_Puzzle.Instance.Puzzle.PuzzleData.PuzzleType;
    }

    void Update()
    {
        if (_columnSpawnTime >= _columnSpawnInterval)
        {
            if (_puzzleType == PuzzleType.Fixed) SpawnColumnFixed();
            else { SpawnColumnRandom(); _columnSpawnInterval = Random.Range(_minColumnInterval, _maxColumnInterval); }
            _columnSpawnTime = 0;
        }

        if (_mineSpawnTime >= _mineSpawnInterval)
        {
            if (_puzzleType == PuzzleType.Fixed) SpawnMineFixed();
            else { SpawnMineRandom(); _mineSpawnInterval = Random.Range(_minMineInterval, _maxMineInterval); }
            _mineSpawnTime = 0;
        }

        _mineSpawnTime += Time.deltaTime;
        _columnSpawnTime += Time.deltaTime;
    }

    void SpawnColumnFixed()
    {

    }

    void SpawnColumnRandom()
    {
        SpawnColumn(_columnSpawners[Random.Range(0, _columnSpawners.Count)]);
    }

    void SpawnColumn(Transform spawner, float columnHeight = 0)
    {
        columnHeight = columnHeight != 0 ? columnHeight : Random.Range(_minColumnHeight, _maxColumnHeight);
        columnHeight = spawner.position.y > 0 ? -columnHeight : columnHeight;

        GameObject columnGO = new GameObject($"Column{_columnsSpawned}");
        Column column = columnGO.AddComponent<Column>();
        column.Initialise(columnHeight, spawner: spawner, _columnSprite, ColumnSpeed);
        _columnsSpawned++;
    }

    void SpawnMineFixed()
    {

    }

    void SpawnMineRandom()
    {
        SpawnMine(_mineSpawners[Random.Range(0, _mineSpawners.Count)]);
    }

    void SpawnMine(Transform spawner)
    {
        GameObject mineGO = new GameObject($"Mine{_minesSpawned}");
        mineGO.transform.position = new Vector3(spawner.position.x, spawner.position.y + Random.Range(-0.5f, 0.5f), spawner.position.z);
        mineGO.transform.parent = spawner.transform;

        Mine mine = mineGO.AddComponent<Mine>();
        mine.Initialise(_bulletParent, _mineSprite, ColumnSpeed);
        _minesSpawned++;
    }
}
