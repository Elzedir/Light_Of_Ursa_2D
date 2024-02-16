using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum IceWallType { Mirror, Stamina, Shatter }
public class Spawner_IceWall : MonoBehaviour
{
    List<IceWallType> _icewallTypes;

    // Mirror chessboard, on one side is the floor where you can see where you can go, and on the other board, you can see the holes in the floor that you have to avoid, but
    // because you are looking at it from underneath, it's mirrorred, and so you have to do it the opposite direcition.

    public Cell_IceWall[,] Cells { get; private set; }
    Transform _cellParent;
    int _cellCount = 0;
    Cell_IceWall _playerLastCell;

    int _rows = 10;
    int _columns = 10;

    Coordinates _startPosition;

    Controller_Puzzle_IceWall _player;

    (int, int) _cellHealthRange = (5, 20);
    int _maxCellHealth;

    void Start()
    {
        List<IceWallType> gameModes = new List<IceWallType>();

        //gameModes.Add(IceWallType.Stamina);
        gameModes.Add(IceWallType.Shatter);
        //gameModes.Add(IceWallType.Mirror);

        InitialisePuzzle(gameModes);
    }

    void InitialisePuzzle(List<IceWallType> gameModes)
    {
        _cellParent = GameObject.Find("CellParent").transform;
        _player = GameObject.Find("Focus").GetComponent<Controller_Puzzle_IceWall>();
        _player.Initialise(this);

        _icewallTypes = new List<IceWallType>(gameModes);
        if (Manager_Puzzle.Instance.Puzzle.PuzzleData.PuzzleType == PuzzleType.Fixed) SpawnFixedPuzzle();
        else SpawnRandomPuzzle();
    }

    void SpawnFixedPuzzle()
    {

    }

    void SpawnRandomPuzzle()
    {
        Cells = new Cell_IceWall[_rows, _columns];

        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _columns; col++)
            {
                Cells[row, col] = CreateCell(row, col);
            }
        }

        _player.transform.position = Cells[0, 0].transform.position;
        _playerLastCell = Cells[0, 0];

        foreach(Cell_IceWall cell in Cells)
        {
            cell.ChangeColour(cell.CellHealth / _maxCellHealth);
        }
    }

    Cell_IceWall CreateCell(int row, int col)
    {
        GameObject cellGO = new GameObject($"cell{row}_{col}");
        cellGO.transform.position = new Vector3(row, col, 0);
        cellGO.transform.rotation = Quaternion.identity;
        cellGO.transform.parent = _cellParent;
        Cell_IceWall cell = cellGO.AddComponent<Cell_IceWall>();
        int cellHealth = Random.Range(_cellHealthRange.Item1, _cellHealthRange.Item2);
        if (cellHealth > _maxCellHealth) _maxCellHealth = cellHealth;
        cell.InitialiseCell(new Coordinates(row, col), this, cellHealth);

        return cell;
    }

    void Update()
    {
        if (_icewallTypes.Contains(IceWallType.Shatter)) OnStayInCell(_playerLastCell);
    }

    public void OnStayInCell(Cell_IceWall cell)
    {
        if (_icewallTypes.Contains(IceWallType.Mirror))
        {
            // move mirror
        }
        if (_icewallTypes.Contains(IceWallType.Stamina))
        {
            // decrease player stamina
        }
        if (_icewallTypes.Contains(IceWallType.Shatter))
        {
            if (!cell.DecreaseHealth(_maxCellHealth))
            {
                cell.Break();
            }
        }
    }

    public bool PlayerCanMove(IceWallDirection direction)
    {
        switch(direction)
        {
            case IceWallDirection.Up: if (_playerLastCell.Coordinates.Y + 1 < _columns && !Cells[_playerLastCell.Coordinates.X, _playerLastCell.Coordinates.Y + 1].Broken) return true; break;
            case IceWallDirection.Down: if (_playerLastCell.Coordinates.Y - 1 >= 0 && !Cells[_playerLastCell.Coordinates.X, _playerLastCell.Coordinates.Y - 1].Broken) return true; break;
            case IceWallDirection.Left: if (_playerLastCell.Coordinates.X - 1 >= 0 && !Cells[_playerLastCell.Coordinates.X - 1, _playerLastCell.Coordinates.Y].Broken) return true; break;
            case IceWallDirection.Right: if (_playerLastCell.Coordinates.X + 1 < _rows && !Cells[_playerLastCell.Coordinates.X + 1, _playerLastCell.Coordinates.Y].Broken) return true; break;
        }

        return false;
    }

    public void RefreshWall(Cell_IceWall cell)
    {
        _playerLastCell = cell;
    }
}
