using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Manager_Save
{
    public delegate void OnSaveDelegate();
    public static event OnSaveDelegate OnSave;

    public delegate void OnLoadDelegate();
    public static event OnLoadDelegate OnLoad;

    public static SaveData SaveData;

    public static void SavePlayer()
    {
        OnSave?.Invoke();
    }

    public static void LoadPlayer(Player player)
    {
        OnLoad?.Invoke();
    }
}

[CreateAssetMenu(fileName = "SaveData", menuName = "SaveData", order = 0)]
[System.Serializable]
public class SaveData : ScriptableObject
{
    public string Name;
    public Manager_Game Manager_Game;
    public Manager_Progress Manager_Progress;
    public Manager_Spawner Manager_Spawner;

    public void SaveManager<T>(T manager) where T : MonoBehaviour
    {
        
    }

    public void LoadManager<T>(T manager) where T : MonoBehaviour
    {

    }

    public void OnLoadData()
    {

    }
}

public interface SaveableData
{
    void Save();

    void Load();
}