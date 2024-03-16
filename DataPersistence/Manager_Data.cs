using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.Profiling;

public class Manager_Data : MonoBehaviour
{

    [Header("Debugging")]
    [SerializeField] bool _disableDataPersistence = false;
    [SerializeField] bool _createGameDataIfNull = false;
    [SerializeField] bool _useTestProfileID = false;
    [SerializeField] string _testSelectedProfileID = "test";

    [Header("File Storage Config")]
    [SerializeField] string _fileName;
    [SerializeField] bool _useEncryption;

    [Header("Auto Saving Configuration")]
    [SerializeField] bool _autoSaveEnabled = false;
    [SerializeField] float _autoSaveTimeSeconds = 60f;

    GameData _gameData;
    List<IDataPersistence> _dataPersistenceObjects;
    FileDataHandler _dataHandler;

    string _selectedProfileID = "";

    Coroutine _autoSaveCoroutine;

    public static Manager_Data Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); } else if (Instance != this) { Destroy(gameObject); return; }

        if (_disableDataPersistence) Debug.LogWarning("Data Persistence is currently disabled!");

        _dataHandler = new FileDataHandler(Application.persistentDataPath, _fileName, _useEncryption);

        _initializeSelectedProfileID();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _dataPersistenceObjects = _findAllDataPersistenceObjects();
        LoadGame();

        if (_autoSaveCoroutine != null) StopCoroutine(_autoSaveCoroutine);
        _autoSaveCoroutine = StartCoroutine(_autoSave());
    }

    List<IDataPersistence> _findAllDataPersistenceObjects()
    {
        return new List<IDataPersistence>(FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<IDataPersistence>());
    }

    public void ChangeSelectedProfileId(string newProfileID)
    {
        _selectedProfileID = newProfileID;
        LoadGame();
    }
    public void DeleteProfileData(string profileID)
    {
        _dataHandler.Delete(profileID);
        _initializeSelectedProfileID();
        LoadGame();
    }

    void _initializeSelectedProfileID()
    {
        _selectedProfileID = _dataHandler.GetMostRecentlyUpdatedProfileID();

        if (!_useTestProfileID) return;

        _selectedProfileID = _testSelectedProfileID;
        Debug.LogWarning("Overrode selected profile id with test id: " + _testSelectedProfileID);
    }

    public void NewGame()
    {
        _gameData = new GameData();
    }

    public void SaveGame()
    {
        if (_disableDataPersistence) return;
        
        if (_gameData == null) { Debug.LogWarning("No data was found. A New Game needs to be started before data can be saved."); return; }

        foreach (IDataPersistence data in _dataPersistenceObjects) data.SaveData(_gameData);

        _gameData.LastUpdated = System.DateTime.Now.ToBinary();

        _dataHandler.Save(_gameData, _selectedProfileID);
    }

    public void LoadGame()
    {
        if (_disableDataPersistence) return;
        
        _gameData = _dataHandler.Load(_selectedProfileID);

        if (_gameData == null && _createGameDataIfNull) NewGame();
        
        if (_gameData == null) { Debug.Log("No data was found. A New Game needs to be started before data can be loaded."); return; }

        foreach (IDataPersistence data in _dataPersistenceObjects) data.LoadData(_gameData);
    }

    void OnApplicationQuit()
    {
        Debug.Log("Data Saved");
        SaveGame();
    }

    public bool HasGameData()
    {
        return _gameData != null;
    }

    public Dictionary<string, GameData> GetAllProfilesGameData()
    {
        return _dataHandler.LoadAllProfiles();
    }

    IEnumerator _autoSave()
    {
        while (true)
        {
            yield return new WaitForSeconds(_autoSaveTimeSeconds);
            if (_autoSaveEnabled) { SaveGame(); Debug.Log("Auto Saved Game"); }
        }
    }

    public string GetCurrentlySelectedProfile()
    {
        return _selectedProfileID;
    }
}

public interface IDataPersistence
{
    void SaveData(GameData data);
    void LoadData(GameData data);
}

public class FileDataHandler
{
    string _directoryPath = "";
    string _fileName = "";
    bool _useEncryption = false;
    readonly string _encryptionCodeWord = "word";
    readonly string _backupExtension = ".bak";

    public FileDataHandler(string directoryPath, string fileName, bool useEncryption)
    {
        _directoryPath = directoryPath;
        _fileName = fileName;
        _useEncryption = useEncryption;
    }

    public GameData Load(string profileID, bool allowRestoreFromBackup = true)
    {
        if (profileID == null) return null;

        string fullPath = Path.Combine(_directoryPath, profileID, _fileName);
        GameData loadedData = null;

        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";

                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                if (_useEncryption) dataToLoad = EncryptDecrypt(dataToLoad);

                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                if (allowRestoreFromBackup)
                {
                    Debug.LogWarning("Failed to load data file. Attempting to roll back.\n" + e);

                    if (AttemptRollback(fullPath)) loadedData = Load(profileID, false);
                }
                else
                {
                    Debug.LogError($"Error occured when trying to load file: {fullPath} and backup did not work. \n {e}");
                }
            }
        }

        return loadedData;
    }

    public void Save(GameData data, string profileID)
    {
        if (profileID == null || 
            Manager_Game.Instance.CurrentState == GameState.MainMenu // NB Have to include Dead, Puzzle, etc. All the states
            ) return;
        
        string fullPath = Path.Combine(_directoryPath, profileID, _fileName);
        string backupFilePath = fullPath + _backupExtension;

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(data, true);

            if (_useEncryption) dataToStore = EncryptDecrypt(dataToStore);
            
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }

            if (Load(profileID) != null) File.Copy(fullPath, backupFilePath, true);
            
            else throw new Exception("Save file could not be verified and backup could not be created.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error occured when trying to save data to file: {fullPath} \n {e}.");
        }
    }

    public void Delete(string profileID)
    {
        if (profileID == null) return;

        string fullPath = Path.Combine(_directoryPath, profileID, _fileName);

        try
        {
            if (File.Exists(fullPath)) Directory.Delete(Path.GetDirectoryName(fullPath), true);

            else Debug.LogWarning($"Tried to delete profile data, but data was not found: {fullPath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to delete profile data for profileID: {profileID} at path: {fullPath} \n {e}");
        }
    }

    public Dictionary<string, GameData> LoadAllProfiles()
    {
        Dictionary<string, GameData> profileDictionary = new();

        IEnumerable<DirectoryInfo> directoryInfoList = new DirectoryInfo(_directoryPath).EnumerateDirectories();

        foreach (DirectoryInfo directoryInfo in directoryInfoList)
        {
            string profileID = directoryInfo.Name;

            if (!File.Exists(Path.Combine(_directoryPath, profileID, _fileName)))
            {
                if (profileID != "Unity") Debug.LogWarning($"Directory: {_directoryPath} has no data: {profileID} for file: {_fileName}");
                continue;
            }

            GameData profileData = Load(profileID);

            if (profileData != null) profileDictionary.Add(profileID, profileData);

            else Debug.LogError($"Profile data is null for profile: {profileID}");
        }

        return profileDictionary;
    }

    public string GetMostRecentlyUpdatedProfileID()
    {
        string mostRecentProfileID = null;

        Dictionary<string, GameData> profilesGameData = LoadAllProfiles();

        foreach (KeyValuePair<string, GameData> pair in profilesGameData)
        {
            string profileID = pair.Key;
            GameData gameData = pair.Value;

            if (gameData == null) continue;

            if (mostRecentProfileID != null) { if (DateTime.FromBinary(gameData.LastUpdated) > DateTime.FromBinary(profilesGameData[mostRecentProfileID].LastUpdated)) mostRecentProfileID = profileID; }
            else mostRecentProfileID = profileID;
        }

        return mostRecentProfileID;
    }

    private string EncryptDecrypt(string data)
    {
        string modifiedData = "";

        for (int i = 0; i < data.Length; i++) modifiedData += (char)(data[i] ^ _encryptionCodeWord[i % _encryptionCodeWord.Length]);

        return modifiedData;
    }

    private bool AttemptRollback(string fullPath)
    {
        bool success = false;
        string backupFilePath = fullPath + _backupExtension;

        try
        {
            if (File.Exists(backupFilePath))
            {
                File.Copy(backupFilePath, fullPath, true);
                success = true;
                Debug.LogWarning($"Had to roll back to backup file at: {backupFilePath}");
            }

            else throw new Exception("Tried to roll back, but no backup file exists to roll back to.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error occured when trying to roll back to backup file at: {backupFilePath} \n {e}");
        }

        return success;
    }
}

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] List<TKey> _keys = new();
    [SerializeField] List<TValue> _values = new();

    public void OnBeforeSerialize()
    {
        _keys.Clear();
        _values.Clear();

        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            _keys.Add(pair.Key);
            _values.Add(pair.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        Clear();

        if (_keys.Count != _values.Count) { Debug.LogError($"Key count: {_keys.Count} does not match value count: {_values.Count}"); }

        for (int i = 0; i < _keys.Count; i++) this.Add(_keys[i], _values[i]);
    }
}

[Serializable]
public class GameData
{
    public long LastUpdated;
    public string CurrentProfile;

    public string SceneName;
    public string LastScene;
    public Vector3 PlayerPosition;
    public bool StaffPickedUp;

    public SerializableDictionary<string, string> QuestSaveData;
    public SerializableDictionary<string, string> PuzzleSaveData;

    public GameData()
    {
        PlayerPosition = Vector3.zero;
        CurrentProfile = "None";
        StaffPickedUp = false;
        PuzzleSaveData = new();
        QuestSaveData = new();
    }
}