using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
public enum MazeType { Standard, Chase, Collect, Doors }
public class Spawner_Maze : MonoBehaviour
{
    List<MazeType> _mousemazeTypes;

    public Cell_MouseMaze[,] Cells { get; private set; }
    Transform _cellParent;
    int _cellCount = 0;
    Cell_MouseMaze _playerLastCell;
    Cell_MouseMaze _furthestCell;
    int _maxDistance;

    int _rows = 20;
    int _columns = 20;
    int _visibility = 50;
    int _wallBreaks = 100;
    float _newPathChance = 0.8f;

    public bool Background = false;
    Coordinates _startPosition;

    Controller_Puzzle_MouseMaze _player;

    #region Chaser
    Pathfinder_Base _pathfinder;
    Transform _chaserParent;
    List<Chaser> _chasers;
    int _chaserCount = 5;
    float _chaserSpawnDelay = 4f;
    float _chaserSpawnInterval = 2f;
    (float, float) _chaserSpeeds = (0.5f, 2);
    #endregion
    #region Collect
    Transform _collectParent;
    Dictionary<Collectable, bool> _collectables = new();
    int _collectableMinimumDistance = 1;
    int _collectableCount = 5;
    #endregion
    #region Door
    Transform _doorParent;
    Dictionary<Door_Base, bool> _doors = new();
    Dictionary<Door_Key, bool> _keys = new();
    int _keysMinimumDistance = 1;
    int _doorCount = 1;
    #endregion
    void Start()
    {
        List<MazeType> mazeTypes = new List<MazeType>();

        mazeTypes.Add(MazeType.Standard);
        mazeTypes.Add(MazeType.Collect);
        mazeTypes.Add(MazeType.Chase);
        mazeTypes.Add(MazeType.Collect);
        //mazeTypes.Add(MazeType.Doors);

        InitialisePuzzle(mazeTypes);
    }

    void InitialisePuzzle(List<MazeType> mazeTypes)
    {
        _cellParent = GameObject.Find("CellParent").transform;
        _chaserParent = GameObject.Find("ChaserParent").transform;
        _collectParent = GameObject.Find("CollectParent").transform;
        _doorParent = GameObject.Find("DoorParent").transform;
        _player = GameObject.Find("Focus").GetComponent<Controller_Puzzle_MouseMaze>();
        _player.OnBreakWall += BreakWall;

        _mousemazeTypes = new List<MazeType>(mazeTypes);
        if (Manager_Puzzle.Instance.Puzzle.PuzzleData.PuzzleType == PuzzleType.Fixed) SpawnFixedPuzzle();
        else SpawnRandomPuzzle();
    }

    void SpawnFixedPuzzle()
    {
        
    }

    void SpawnRandomPuzzle()
    {
        Cells = new Cell_MouseMaze[_rows, _columns];

        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _columns; col++)
            {
                Cells[row, col] = CreateCell(row, col);
            }
        }

        _startPosition = Cells[0, 0].Coordinates;

        NodeArray.Nodes = NodeArray.InitializeArray(_rows, _columns);

        CreateMaze(null, Cells[0, 0], 0);

        if (_mousemazeTypes.Contains(MazeType.Standard)) _furthestCell.MarkCell(Color.red);
        if (_mousemazeTypes.Contains(MazeType.Chase)) StartCoroutine(SpawnChasers());
        if (_mousemazeTypes.Contains(MazeType.Collect) && _collectables.Count < _collectableCount)
        {
            for (int i = 0; i < (_collectableCount - _collectables.Count); i++) 
            {
                SpawnCollectable(Cells[Random.Range(1, _rows), Random.Range(1, _columns)].transform.position); 
            } 
        }
        if (_mousemazeTypes.Contains(MazeType.Doors))
        {
            for (int i = 0; i < _doorCount; i++)
            {
                // Change this to account for multiple doors
                SpawnKey(SpawnDoor(_furthestCell));
            }
        }
    }

    Cell_MouseMaze CreateCell(int row, int col)
    {
        GameObject cellGO = new GameObject($"cell{row}_{col}");
        cellGO.transform.position = new Vector3(row, col, 0);
        cellGO.transform.rotation = Quaternion.identity;
        cellGO.transform.parent = _cellParent;
        Cell_MouseMaze cell = cellGO.AddComponent<Cell_MouseMaze>();
        cell.InitialiseCell(new Coordinates(row, col), this);

        return cell;
    }

    void CreateMaze(Cell_MouseMaze previousCell, Cell_MouseMaze currentCell, int distanceFromStart)
    {
        currentCell.Visited = true;
        _cellCount++;

        if (_mousemazeTypes.Contains(MazeType.Collect) && _collectables.Count < _collectableCount && _cellCount > _collectableMinimumDistance && Random.Range(0, 100) > 98) { SpawnCollectable(currentCell.transform.position); }

        ClearWalls(previousCell, currentCell);

        if (distanceFromStart > _maxDistance)
        {
            _maxDistance = distanceFromStart;
            _furthestCell = currentCell;
        }

        var unvisitedNeighbors = GetNextUnvisitedCell(currentCell).OrderBy(_ => Random.Range(1, 10)).ToList();

        if (Random.Range(0, 1f) < _newPathChance && unvisitedNeighbors.Count > 0)
        {
            foreach(Cell_MouseMaze nextCell in unvisitedNeighbors)
            {
                if (nextCell != null && !nextCell.Visited) CreateMaze(currentCell, unvisitedNeighbors[Random.Range(0, unvisitedNeighbors.Count)], distanceFromStart + 1);
            }
        }
        else
        {
            foreach (Cell_MouseMaze nextCell in unvisitedNeighbors)
            {
                if (nextCell != null && !nextCell.Visited) CreateMaze(currentCell, nextCell, distanceFromStart + 1);
            }
        }
    }

    void ClearWalls(Cell_MouseMaze currentCell, Cell_MouseMaze nextCell)
    {
        if (currentCell == null) return;

        Node currentNode = Pathfinder_Base.GetNodeAtPosition(currentCell.Coordinates.X, currentCell.Coordinates.Y);
        Node nextNode = Pathfinder_Base.GetNodeAtPosition(nextCell.Coordinates.X, nextCell.Coordinates.Y);

        if (currentCell.transform.position.x < nextCell.transform.position.x)
        {
            currentCell.ClearWall(Wall.Right);
            nextCell.ClearWall(Wall.Left);
            currentNode.UpdateMovementCost(Direction.Right, 1);
            nextNode.UpdateMovementCost(Direction.Left, 1);
            return;
        }

        if (currentCell.transform.position.x > nextCell.transform.position.x)
        {
            currentCell.ClearWall(Wall.Left);
            nextCell.ClearWall(Wall.Right);
            currentNode.UpdateMovementCost(Direction.Left, 1);
            nextNode.UpdateMovementCost(Direction.Right, 1);
            return;
        }

        if (currentCell.transform.position.y < nextCell.transform.position.y)
        {
            currentCell.ClearWall(Wall.Top);
            nextCell.ClearWall(Wall.Bottom);
            currentNode.UpdateMovementCost(Direction.Top, 1);
            nextNode.UpdateMovementCost(Direction.Bottom, 1);
            return;
        }

        if (currentCell.transform.position.y > nextCell.transform.position.y)
        {
            currentCell.ClearWall(Wall.Bottom);
            nextCell.ClearWall(Wall.Top);
            currentNode.UpdateMovementCost(Direction.Bottom, 1);
            nextNode.UpdateMovementCost(Direction.Top, 1);
            return;
        }
    }

    IEnumerable<Cell_MouseMaze> GetNextUnvisitedCell(Cell_MouseMaze currentCell)
    {
        int x = (int)currentCell.transform.position.x;
        int y = (int)currentCell.transform.position.y;

        if (x + 1 < _rows)
        {
            if (!Cells[x + 1, y].Visited) yield return Cells[x + 1, y];
            
        }

        if (x - 1 >= 0)
        {
            if (!Cells[x - 1, y].Visited) yield return Cells[x - 1, y];
            
        }

        if (y + 1 < _columns)
        {
            if (!Cells[x, y + 1].Visited) yield return Cells[x, y + 1];
            
        }

        if (y - 1 >= 0)
        {
            if (!Cells[x, y - 1].Visited) yield return Cells[x, y - 1];
        }
    }

    void SpawnCollectable(Vector3 position)
    {
        GameObject collectableGO = new GameObject($"Collectable_{_collectables.Count}");
        collectableGO.transform.parent = _collectParent;
        collectableGO.transform.position = position;
        Collectable collectable = collectableGO.AddComponent<Collectable>();
        _collectables.Add(collectable, false);
        collectable.SpawnCollectable(this);
    }

    IEnumerator SpawnChasers()
    {
        yield return new WaitForSeconds(_chaserSpawnDelay);

        _chasers = new();

        for (int i = 0; i < _chaserCount; i++)
        {
            SpawnChaser();

            yield return new WaitForSeconds(_chaserSpawnInterval);
        }
    }

    void SpawnChaser()
    {
        GameObject chaserGO = new GameObject($"Chaser_{_chasers.Count}");
        chaserGO.transform.parent = _chaserParent;
        Chaser chaser = chaserGO.AddComponent<Chaser>();
        _chasers.Add(chaser);
        chaser.InitialiseChaser(Cells[0,0], this, Random.Range(_chaserSpeeds.Item1, _chaserSpeeds.Item2));
        chaser.Pathfinder.RunPathfinder(_rows, _columns, chaser.CurrentCell.Coordinates, _playerLastCell.Coordinates, chaser, this);
    }

    Door_Base SpawnDoor(Cell_MouseMaze cell)
    {
        GameObject doorGO = new GameObject($"Door_{_doors.Count}");
        doorGO.transform.parent = _doorParent;
        Door_Base door = doorGO.AddComponent<Door_Base>();
        door.InitialiseDoor(MouseMazeColour.Blue, Color.blue, cell);
        _doors.Add(door, true);
        return door;
    }

    void SpawnKey(Door_Base door)
    {
        GameObject keyGO = new GameObject($"Key_{_doors.Count}");
        keyGO.transform.parent = door.transform;
        keyGO.transform.localPosition = Cells[Random.Range(_keysMinimumDistance, _rows), Random.Range(_keysMinimumDistance, _columns)].transform.position;
        Door_Key key = keyGO.AddComponent<Door_Key>();
        key.InitialiseDoorKey(door.MouseMazeDoorColour, door.DoorColor);
        _keys.Add(key, false);
    }

    public void RefreshMaze(Cell_MouseMaze playerCell)
    {
        _playerLastCell = playerCell;

        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _columns; col++)
            {
                if (Mathf.Abs(playerCell.Coordinates.X - row) + Mathf.Abs(playerCell.Coordinates.Y - col) <= _visibility) Cells[row, col].Show();
                else Cells[row, col].Hide();
            }
        }

        if (_chasers != null && _chasers.Count > 0) RefreshChaserPaths();
    }

    void BreakWall()
    {
        if (_wallBreaks <= 0) { return; }

        _wallBreaks--;

        int row = _playerLastCell.Coordinates.X;
        int col = _playerLastCell.Coordinates.Y;

        Cell_MouseMaze closestCell = null; float minDistance = float.MaxValue;

        CheckNeighbor(row - 1, col);
        CheckNeighbor(row + 1, col);
        CheckNeighbor(row, col - 1);
        CheckNeighbor(row, col + 1);

        void CheckNeighbor(int neighborRow, int neighborCol)
        {
            if (neighborRow < 0 || neighborRow >= _rows || neighborCol < 0 || neighborCol >= _columns) return;

            float distance = Vector3.Distance(Cells[neighborRow, neighborCol].transform.position, _player.transform.position);

            if (distance > minDistance) return;

            minDistance = distance;

            closestCell = Cells[neighborRow, neighborCol];
        }

        ClearWalls(_playerLastCell, closestCell);

        if (_mousemazeTypes.Contains(MazeType.Chase)) RefreshChaserPaths();
    }

    void RefreshChaserPaths()
    {
        if (_chasers.Count == 0) return;

        foreach (Chaser chaser in _chasers)
        {
            chaser.Pathfinder.RunPathfinder(_rows, _columns, chaser.CurrentCell.Coordinates, _playerLastCell.Coordinates, chaser, this);
        }
    }
    public void GetNewRoute(Chaser chaser)
    {
        chaser.Pathfinder.RunPathfinder(_rows, _columns, chaser.CurrentCell.Coordinates, _playerLastCell.Coordinates, chaser, this);
    }

    void OnDestroy()
    {
        _player.OnBreakWall -= BreakWall;
    }

    public void CollectableCollected(Collectable collectable)
    {
        if (!_collectables.ContainsKey(collectable)) return;

        _collectables[collectable] = true;
        collectable.gameObject.SetActive(false);

        if (_collectables.Count > 0) return;

        // Collectible objective completed.
    }

    public Cell_MouseMaze GetCell(int row, int col)
    {
        if (row >= 0 && row < Cells.GetLength(0) && col >= 0 && col < Cells.GetLength(1)) return Cells[row, col];
        
        return null;
    }
}
