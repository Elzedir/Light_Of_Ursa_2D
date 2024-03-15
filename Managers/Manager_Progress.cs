using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager_Progress : MonoBehaviour, IDataPersistence
{
    public static Manager_Progress Instance;
    public Dictionary<string, Quest> QuestList { get; private set; }
    bool _initialised = false;

    // Find out what information to save about the quests. Doesn't have to be everything.

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); } else if (Instance != this) Destroy(gameObject);
        if (!_initialised) _initialise();
    }

    void _initialise()
    {
        _initialised = true;
        QuestList = new();

        _mainQuests();
        _sideQuests();
    }

    void _mainQuests()
    {
        Quest killTestQuest = new Quest(
            0,
            "Kill Test Quest",
            "You have been tasked by the farmer to kill the thing.",
            new List<Stage>
            {
                new Stage (0, "Find the thing", "The farmer has asked you to kill the thing. First, find it.", 0),
                new Stage (1, "Kill the thing", "You have found the thing, now, kill it.", 0),
                new Stage (2, "Return with the thing", "You have killed the thing, now, return with it. A hero.", 0)
            }
            );
        QuestList.Add("Kill Test Quest", killTestQuest);
    }

    void _sideQuests()
    {

    }

    public void SaveData(GameData data)
    {
        foreach (var quest in QuestList)
        {
            data.Quests[quest.Key] = JsonConvert.SerializeObject(quest.Value.QuestSaveData);
        }
    }

    public void LoadData(GameData data)
    {
        foreach (var quest in data.Quests)
        {
            QuestList[quest.Key].LoadData(JsonConvert.DeserializeObject<QuestSaveData>(quest.Value));
        }
    }
}
public class Quest
{
    public int QuestID { get; private set; }
    public string QuestName { get; private set; }
    public string QuestDescription { get; private set; }
    public List<Stage> QuestStages { get; private set; }
    public QuestSaveData QuestSaveData { get; private set; }

    public Quest(int questID, string questName, string questDescription, List<Stage> questStages)
    {
        QuestID = questID;
        QuestName = questName;
        QuestDescription = questDescription;
        QuestStages = questStages;

        QuestSaveData = new QuestSaveData(QuestStages);
    }

    public void LoadData(QuestSaveData data)
    {
        QuestSaveData = data;

        foreach(var stage in data.SaveQuestStages)
        {
            QuestStages[stage.Key].SetStage(stage.Value);
        }
    }
}

public class Stage
{
    public int StageID { get; private set; }
    public string StageTitle { get; private set; }
    public string StageDescription { get; private set; }
    public int StageCompleted { get; private set; }

    public Stage(int stageID, string stageTitle, string stageDescription, int stageCompleted)
    {
        StageID = stageID;
        StageTitle = stageTitle;
        StageDescription = stageDescription;
        StageCompleted = stageCompleted;
    }

    public void SetStage(int stageCompleted)
    {
        StageCompleted = stageCompleted;
    }
}

public class QuestSaveData
{
    public Dictionary<int, int> SaveQuestStages { get; private set; }
    
    public QuestSaveData(List<Stage> stages)
    {
        SaveQuestStages = new();

        if (stages == null) return;

        foreach (Stage stage in stages)
        {
            SaveQuestStages[stage.StageID] = stages[stage.StageID].StageCompleted;
        }
    }
}