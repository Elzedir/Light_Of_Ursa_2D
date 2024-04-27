using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.Profiling;
using NUnit.Framework;

public class Manager_Data : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] bool _disableDataPersistence = false;
    [SerializeField] bool _createGameDataIfNull = false;
    [SerializeField] bool _useTestProfileID = false;
    [SerializeField] string _testSelectedProfileName = "test";

    public static string SaveFileName { get; private set; } = "Data.LightOfUrsus";

    [Header("File Storage Config")]
    [SerializeField] public bool _useEncryption;

    [Header("Auto Saving Configuration")]
    [SerializeField] bool _autoSaveEnabled = false;
    [SerializeField] float _autoSaveTimeSeconds = 60f;
    [SerializeField] int _numberOfAutoSaves = 5;

    GameData _gameData;
    List<IDataPersistence> _dataPersistenceObjects;
    Profile _activeProfile;
    public List<Profile> AllProfiles;

    Coroutine _autoSaveCoroutine;

    public static Manager_Data Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); } else if (Instance != this) { Destroy(gameObject); return; }

        if (_disableDataPersistence) Debug.LogWarning("Data Persistence is currently disabled!");

        _activeProfile = _initializeProfiles();
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

    public void ChangeSelectedProfileId(string newProfile)
    {
        _activeProfile = CreateProfile(newProfile);
        LoadGame();
    }

    public void DeleteProfileData(string profileID)
    {
        _activeProfile.Delete(profileID);
        _initializeProfiles();
        LoadGame();
    }

    Profile _initializeProfiles()
    {
        if (!_useTestProfileID) return GetMostRecentlyUpdatedProfileID();

        Debug.LogWarning("Overrode selected profile id with test id: " + _testSelectedProfileName);
        return new Profile(_testSelectedProfileName, 0, null, Manager_Data.Instance._useEncryption);
    }

    public void NewGame(string profile)
    {
        _gameData = new GameData(profile);
    }

    public void SaveGame(string saveGameName = "")
    {
        if (_disableDataPersistence) return;

        if (_gameData == null) { Debug.LogWarning("No data was found. A New Game needs to be started before data can be saved."); return; }

        foreach (IDataPersistence data in _dataPersistenceObjects) data.SaveData(_gameData);

        _gameData.LastUpdated = System.DateTime.Now.ToBinary();

        _activeProfile.Save(saveGameName, _gameData, _activeProfile.Name);
    }

    public void LoadGame()
    {
        if (_disableDataPersistence || _activeProfile.Name == "Create New Profile") return;

        _gameData = _activeProfile.Load(GetLatestSave(_activeProfile.Name), _activeProfile.Name);

        if (_gameData == null && _createGameDataIfNull) NewGame(_activeProfile.Name);

        if (_gameData == null) { Debug.Log("No data was found. A New Game needs to be started before data can be loaded."); return; }

        foreach (IDataPersistence data in _dataPersistenceObjects) data.LoadData(_gameData);
    }

    void OnApplicationQuit()
    {
        Debug.Log("Data Saved");
        SaveGame("ExitSave");
    }

    public bool HasGameData()
    {
        return _gameData != null;
    }

    public List<Profile> GetAllProfiles()
    {
        List<Profile> profiles = new();

        foreach (var profile in LoadAllProfiles())
        {
            profiles.Add(profile.Key);
        }

        return profiles;
    }

    public Dictionary<string, GameData> GetAllSavedGames(Profile profile)
    {
        Dictionary<string, GameData> savedGames = new();

        foreach (string saveGame in profile.SavedGames)
        {
            savedGames.Add(saveGame, profile.Load(saveGame, profile.Name));
        }

        return savedGames;
    }

    IEnumerator _autoSave()
    {
        while (true)
        {
            yield return new WaitForSeconds(_autoSaveTimeSeconds);

            if (_autoSaveEnabled)
            {
                SaveGame($"AutoSave_{_activeProfile.AutoSaveCounter}");
                _activeProfile.AutoSaveCounter++;
                if (_activeProfile.AutoSaveCounter > _numberOfAutoSaves) _activeProfile.AutoSaveCounter = 1;
                Debug.Log("Auto Saved Game");
            }
        }
    }

    public Profile GetActiveProfile()
    {
        return _activeProfile;
    }

    public Profile CreateProfile(string profile)
    {
        IEnumerable saveGameDirectories = new DirectoryInfo(Path.Combine(Application.persistentDataPath, profile)).EnumerateDirectories();

        int autoSaveCounter = 0;
        List<string> saveGames = new();

        foreach (DirectoryInfo saveGame in saveGameDirectories)
        {
            if (saveGame.Name.Contains("AutoSave"))
            {
                autoSaveCounter++;
                saveGames.Add(saveGame.Name);
            }
            else
            {
                saveGames.Add(saveGame.Name);
            }
        }

        return new Profile(
            profile,
            autoSaveCounter,
            saveGames,
            Manager_Data.Instance._useEncryption
            );
    }

    public Profile GetMostRecentlyUpdatedProfileID()
    {
        Profile mostRecentProfile = null;

        Dictionary<Profile, GameData> profilesGameData = LoadAllProfiles();

        foreach (KeyValuePair<Profile, GameData> pair in profilesGameData)
        {
            Profile profile = pair.Key;
            GameData gameData = pair.Value;

            if (gameData == null) continue;

            if (mostRecentProfile != null) { if (DateTime.FromBinary(gameData.LastUpdated) > DateTime.FromBinary(profilesGameData[mostRecentProfile].LastUpdated)) mostRecentProfile = profile; }
            else mostRecentProfile = profile;
        }

        if (mostRecentProfile == null) return new Profile("Create New Profile", 0, null, Manager_Data.Instance._useEncryption);

        return mostRecentProfile;
    }

    public Dictionary<Profile, GameData> LoadAllProfiles()
    {
        Dictionary<Profile, GameData> profileDictionary = new();

        IEnumerable<DirectoryInfo> directoryInfoList = new DirectoryInfo(Application.persistentDataPath).EnumerateDirectories();

        foreach (DirectoryInfo directoryInfo in directoryInfoList)
        {
            if (directoryInfo.Name == "Unity") continue;
               
            Profile newProfile = Manager_Data.Instance.CreateProfile(directoryInfo.Name);

            GameData profileData = newProfile.Load(GetLatestSave(directoryInfo.Name), directoryInfo.Name);

            if (profileData != null) profileDictionary.Add(newProfile, profileData);

            else Debug.LogError($"Profile data is null for profile: {directoryInfo.Name}");
        }

        return profileDictionary;
    }

    public string GetLatestSave(string profile)
    {
        IEnumerable saveGameDirectories = new DirectoryInfo(Path.Combine(Application.persistentDataPath, profile)).EnumerateDirectories();

        DateTime lastSaved = DateTime.MinValue;
        string saveGameName = null;

        foreach (DirectoryInfo directoryInfo in saveGameDirectories)
        {
            if (directoryInfo.LastWriteTime > lastSaved) { lastSaved = directoryInfo.LastWriteTime; saveGameName = directoryInfo.Name; }
        }

        return saveGameName;
    }
}

public interface IDataPersistence
{
    void SaveData(GameData data);
    void LoadData(GameData data);
}

public class Profile
{
    public string Name;
    public List<string> SavedGames;
    bool _useEncryption = false;
    public int AutoSaveCounter;
    readonly string _encryptionCodeWord = "word";
    readonly string _backupExtension = ".bak";

    public Profile(string name, int autoSaveCounter, List<string> savedGames, bool useEncryption)
    {
        Name = name;
        AutoSaveCounter = autoSaveCounter;
        if (savedGames != null) SavedGames = new(savedGames);
        else SavedGames = new();
        _useEncryption = useEncryption;
    }

    public GameData Load(string savedGame, string profileID, bool allowRestoreFromBackup = true)
    {
        if (profileID == null) return null;

        string fullPath = Path.Combine(Application.persistentDataPath, profileID, savedGame, Manager_Data.SaveFileName);
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

                if (_useEncryption) dataToLoad = _encryptDecrypt(dataToLoad);

                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                if (allowRestoreFromBackup)
                {
                    Debug.LogWarning("Failed to load data file. Attempting to roll back.\n" + e);

                    if (_attemptRollback(fullPath)) loadedData = Load(savedGame, profileID, false);
                }
                else
                {
                    Debug.LogError($"Error occured when trying to load file: {fullPath} and backup did not work. \n {e}");
                }
            }
        }

        return loadedData;
    }

    public void Save(string savedGame, GameData data, string profileID, bool profileSave = false)
    {
        if (profileID == null || SceneManager.GetActiveScene().name == "Main_Menu" && !profileSave) return;
        if (savedGame == "") { savedGame = $"NewSave_{SavedGames.Count}"; }

        string fullPath = Path.Combine(Application.persistentDataPath, profileID, savedGame, Manager_Data.SaveFileName);
        string backupFilePath = fullPath + _backupExtension;

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(data, true);

            if (_useEncryption) dataToStore = _encryptDecrypt(dataToStore);

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }

            if (Load(savedGame, profileID) != null) File.Copy(fullPath, backupFilePath, true);

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

        string fullPath = Path.Combine(Application.persistentDataPath, profileID, Manager_Data.SaveFileName);

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

    string _encryptDecrypt(string data)
    {
        string modifiedData = "";

        for (int i = 0; i < data.Length; i++) modifiedData += (char)(data[i] ^ _encryptionCodeWord[i % _encryptionCodeWord.Length]);

        return modifiedData;
    }

    bool _attemptRollback(string fullPath)
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
    public string CurrentProfileName;

    public string SceneName;
    public string LastScene;
    public Vector3 PlayerPosition;
    public bool StaffPickedUp;

    public SerializableDictionary<string, string> QuestSaveData;
    public SerializableDictionary<string, string> PuzzleSaveData;

    public GameData(string profile)
    {
        PlayerPosition = Vector3.zero;
        CurrentProfileName = profile;
        StaffPickedUp = false;
        PuzzleSaveData = new();
        QuestSaveData = new();
    }
}