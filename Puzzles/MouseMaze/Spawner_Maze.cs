using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner_Maze : MonoBehaviour
{
    Cell[,] _cells;

    int _rows = 20;
    int _columns = 20;
    [SerializeField] int _visibility = 2;

    public bool Background = false;
    int[,] _startPosition;

    void Start()
    {
        InitialisePuzzle();
    }

    void InitialisePuzzle()
    {
        if (Manager_Puzzle.Instance.Puzzle.PuzzleData.PuzzleType == PuzzleType.Fixed) SpawnFixedPuzzle();
        else SpawnRandomPuzzle();
    }

    void SpawnFixedPuzzle()
    {
        
    }

    void SpawnRandomPuzzle()
    {
        _cells = new Cell[_rows, _columns];

        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _columns; col++)
            {
                _cells[row, col] = CreateCell(row, col);
            }
        }

        //Cell startCell = _cells[Random.Range(0, _rows), Random.Range(0, _columns)];
        Cell startCell = _cells[0, 0];

        _startPosition = new int[startCell.Row, startCell.Col];

        CreateMaze(null, startCell);
    }

    Cell CreateCell(int row, int col)
    {
        GameObject cellGO = new GameObject($"cell{row}{col}");
        cellGO.transform.position = new Vector3(row, col, 0);
        cellGO.transform.rotation = Quaternion.identity;
        cellGO.transform.parent = transform;
        Cell cell = cellGO.AddComponent<Cell>();
        cell.InitialiseCell(row, col, this);
        return cell;
    }

    void CreateMaze(Cell previousCell, Cell currentCell)
    {
        currentCell.Visited = true;
        ClearWalls(previousCell, currentCell);

        var unvisitedNeighbors = GetNextUnvisitedCell(currentCell).OrderBy(_ => Random.Range(1, 10)).ToList();

        foreach (Cell nextCell in unvisitedNeighbors)
        {
            if (nextCell != null && !nextCell.Visited) CreateMaze(currentCell, nextCell);
        }
    }

    void ClearWalls(Cell previousCell, Cell currentCell)
    {
        if (previousCell == null) return;

        if (previousCell.transform.position.x < currentCell.transform.position.x)
        {
            previousCell.ClearWall(Wall.Right);
            currentCell.ClearWall(Wall.Left);
            return;
        }

        if (previousCell.transform.position.x > currentCell.transform.position.x)
        {
            previousCell.ClearWall(Wall.Left);
            currentCell.ClearWall(Wall.Right);
            return;
        }

        if (previousCell.transform.position.y < currentCell.transform.position.y)
        {
            previousCell.ClearWall(Wall.Top);
            currentCell.ClearWall(Wall.Bottom);
            return;
        }

        if (previousCell.transform.position.y > currentCell.transform.position.y)
        {
            previousCell.ClearWall(Wall.Bottom);
            currentCell.ClearWall(Wall.Top);
            return;
        }
    }

    IEnumerable<Cell> GetNextUnvisitedCell(Cell currentCell)
    {
        int x = (int)currentCell.transform.position.x;
        int y = (int)currentCell.transform.position.y;

        if (x + 1 < _rows)
        {
            var cellToRight = _cells[x + 1, y];

            if (cellToRight.Visited == false)
            {
                yield return cellToRight;
            }
        }

        if (x - 1 >= 0)
        {
            var cellToLeft = _cells[x - 1, y];

            if (cellToLeft.Visited == false)
            {
                yield return cellToLeft;
            }
        }

        if (y + 1 < _columns)
        {
            var cellToFront = _cells[x, y + 1];

            if (cellToFront.Visited == false)
            {
                yield return cellToFront;
            }
        }

        if (y - 1 >= 0)
        {
            var cellToBack = _cells[x, y - 1];

            if (cellToBack.Visited == false)
            {
                yield return cellToBack;
            }
        }
    }

    public void RefreshMaze(Cell playerCell)
    {
        int playerX = playerCell.Row;
        int playerY = playerCell.Col;

        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _columns; col++)
            {
                Cell cell = _cells[row, col];

                int distance = Mathf.Abs(playerX - row) + Mathf.Abs(playerY - col);

                if (distance <= _visibility) cell.Show();
                else cell.Hide();
            }
        }
    }
}
