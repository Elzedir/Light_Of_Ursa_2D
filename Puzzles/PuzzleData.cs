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
    public PuzzleSaveData PuzzleSaveData;

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

        PuzzleSaveData = new PuzzleSaveData(PuzzleID, PuzzleState.PuzzleCompleted);
    }

    public void LoadData(PuzzleSaveData data)
    {
        PuzzleSaveData = data;

        PuzzleState.PuzzleCompleted = data.PuzzleCompleted;
    }
}

[Serializable]
public class PuzzleSaveData
{
    public string PuzzleID { get; private set; }
    public bool PuzzleCompleted { get; private set; }

    public PuzzleSaveData(string puzzleID, bool puzzleCompleted)
    {
        PuzzleID = puzzleID;
        PuzzleCompleted = puzzleCompleted;
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
    public Coordinates StartPosition = new Coordinates(0, 0);
    public int CellHealthMin = 5;
    public int CellHealthMax = 20;
    public int PlayerExtraStaminaPercentage = 10;

    public IceWallData
        (
        int rows,
        int columns,
        Coordinates startPosition,
        int cellHealthMin,
        int cellHealthMax,
        int playerExtraStaminaPercentage
        )
    {
        Rows = rows; 
        Columns = columns;
        StartPosition = startPosition;
        CellHealthMin = cellHealthMin;
        CellHealthMax = cellHealthMax;
        PlayerExtraStaminaPercentage = playerExtraStaminaPercentage;
    }
}

