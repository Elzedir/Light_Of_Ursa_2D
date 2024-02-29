using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager_Progress : MonoBehaviour, IDataPersistence
{
    public static Manager_Progress Instance;
    public Dictionary<string, Level> LevelList { get; private set; }
    public Dictionary<string, Quest> QuestList { get; private set; }
    bool _initialised = false;


    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); } else if (Instance != this) Destroy(gameObject);
        if (!_initialised) _initialise();
    }

    void _initialise()
    {
        _initialised = true;
        LevelList = new();
        QuestList = new();

        _mainQuests();
        _sideQuests();
        _ursusCave();
        _river();
    }

    void _mainQuests()
    {
        Quest quest = new Quest(
            0,
            "Test Quest",
            new List<(int, string)>()
            {
                (0, "Find the thing"),
                (1, "Kill the thing"),
                (2, "Return with the thing")
            }
            );
    }

    void _sideQuests()
    {

    }

    void _ursusCave()
    {

    }

    void _river()
    {

    }

    public void SaveData(GameData data)
    {
        foreach (var level in LevelList)
        {
            data.Levels[level.Key] = JsonConvert.SerializeObject(level, Formatting.Indented);
        }
    }

    public void LoadData(GameData data)
    {
        foreach (var level in data.Puzzles)
        {
            LevelList[level.Key] = JsonConvert.DeserializeObject<Level>(level.Value);
        }
    }
}

public class Level
{
    public List<FMODUnity.EventReference> SongList;
}
public class Quest
{
    public int QuestID { get; private set; }
    public string QuestName { get; private set; }

    public Dictionary<(int, string), bool> Stages;

    public Quest(int questID, string questName, List<(int, string)> stages)
    {
        QuestID = questID;
        QuestName = questName;

        Stages = new();

        foreach((int, string) stage in stages)
        {
            Stages[stage] = false;
        }
    }
}