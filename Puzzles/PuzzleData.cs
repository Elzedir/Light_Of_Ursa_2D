using System;
using UnityEngine;

[Serializable]
public class PuzzleData
{
    public PuzzleSet PuzzleSet;
    public string PuzzleID;
    public PuzzleState PuzzleState;
    public PuzzleObjectives PuzzleObjectives;
    public IceWallData IceWallData;

    public PuzzleData
        (
        string puzzleID,
        PuzzleSet puzzleSet,
        PuzzleState puzzleState,
        PuzzleObjectives puzzleObjectives,
        IceWallData iceWallData
        )
    {
        PuzzleID = puzzleID;
        PuzzleSet = puzzleSet;
        PuzzleState = puzzleState;
        PuzzleObjectives = puzzleObjectives;
        IceWallData = iceWallData;
    }
}

[Serializable]
public class PuzzleState
{
    public PuzzleType PuzzleType;
    public bool PuzzleRepeatable = false;
    public bool PuzzleCompleted;

    public PuzzleState
        (
        PuzzleType puzzleType,
        bool puzzleRepeatable,
        bool puzzleCompleted
        )
    {
        PuzzleType = puzzleType;
        PuzzleRepeatable = puzzleRepeatable;
        PuzzleCompleted = puzzleCompleted;
    }
}

[Serializable]
public class PuzzleObjectives
{
    [Range(0, 9)] public int PuzzleDifficulty;
    public bool PuzzleObjective;
    public float PuzzleDuration;
    public float PuzzleScore;

    public PuzzleObjectives
        (
        bool puzzleObjective,
        float puzzleDuration,
        float puzzleScore
        )
    {
        PuzzleObjective = puzzleObjective;
        PuzzleDuration = puzzleDuration;
        PuzzleScore = puzzleScore;
    }
}

[Serializable]
public class IceWallData
{
    [Range(0, 50)] public int Rows = 10;
    [Range(0, 50)] public int Columns = 10;
    public Coordinates StartPosition;
    public (int, int) CellHealthRange;
    public int PlayerExtraStaminaPercentage;

    public IceWallData
        (
        int rows,
        int columns,
        Coordinates startPosition,
        (int, int) cellHealthRange,
        int playerExtraStaminaPercentage
        )
    {
        Rows = rows; 
        Columns = columns;
        StartPosition = startPosition;
        CellHealthRange = cellHealthRange;
        PlayerExtraStaminaPercentage = playerExtraStaminaPercentage;
    }
}

