using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager_Progress : MonoBehaviour
{
    
}

public class Level
{
    public bool Completed { get; private set; } = false;
    public List<Stage> StageList = new();

    public void InitialiseStages()
    {

    }

    public void CompleteLevel()
    {
        Completed = true;
    }
}

public class Stage
{
    public bool Completed { get; private set; } = false;
    public List<Quest> MainQuestList = new();
    public List<Quest> SideQuestList = new();

    public void CompleteStage()
    {
        Completed = true;
    }
}

public enum QuestState { NotFound, Found, Started, Completed }

public class Quest
{
    public QuestState State { get; private set; }

    public void SetQuestState(QuestState state)
    {
        State = state;
    }
}