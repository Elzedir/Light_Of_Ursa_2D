using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu_LoadGame : Menu_Base
{
    Transform _saveSlotParent;
    List<SaveSlot> _saveSlots;
    SaveSlot _saveSlot;

    public void OnSaveSlotClicked(SaveSlot saveSlot)
    {
        if (saveSlot.HasData)
        {
            Manager_Game.FindTransformRecursively(transform.parent, "ConfirmationPanel").GetComponent<SaveSlot_DeleteConfirmation>().ActivateMenu(
                "Starting a New Game with this slot will override the currently saved data. Are you sure?",
                () => {
                    Manager_Data.Instance.ChangeSelectedProfileId(saveSlot.GetProfileID());
                    Manager_Data.Instance.NewGame();
                    SaveGameAndLoadScene();
                },
                () => {
                    ActivateMenu();
                }
            );
        }
        else
        {
            Manager_Data.Instance.ChangeSelectedProfileId(saveSlot.GetProfileID());
            Manager_Data.Instance.NewGame();
            SaveGameAndLoadScene();
        }
    }

    public void OnDeleteSaveGameClicked(SaveSlot saveSlot)
    {
        Manager_Game.FindTransformRecursively(transform.parent, "ConfirmationPanel").GetComponent<SaveSlot_DeleteConfirmation>().ActivateMenu(
                "Are you sure you want to clear this data?",
                () => {
                    Manager_Data.Instance.DeleteProfileData(saveSlot.GetProfileID());
                    ActivateMenu();
                },
                () => {
                    ActivateMenu();
                }
            );
    }

    private void SaveGameAndLoadScene()
    {
        Manager_Data.Instance.SaveGame();
        //Manager_Game.Instance.LoadScene(Some scene from save data);
    }

    public void ActivateMenu()
    {
        if (_saveSlots == null) _saveSlots = new();

        foreach (SaveSlot save in _saveSlots) Destroy(save.gameObject); _saveSlots.Clear();

        gameObject.SetActive(true);
        if (!_saveSlotParent) _saveSlotParent = Manager_Game.FindTransformRecursively(transform, "SavedGamesParent");
        if (!_saveSlot) _saveSlot = Manager_Game.FindTransformRecursively(transform, "SaveSlot").GetComponent<SaveSlot>();

        foreach (var saveGame in Manager_Data.Instance.GetAllProfilesGameData())
        {
            _saveSlots.Add(_createSaveSlot(saveGame.Key, saveGame.Value));
        }
    }

    SaveSlot _createSaveSlot(string profileID, GameData gameData)
    {
        GameObject saveSlotGO = Instantiate(_saveSlot.gameObject, _saveSlotParent.transform);
        saveSlotGO.name = profileID;
        SaveSlot saveSlot = saveSlotGO.GetComponent<SaveSlot>();
        saveSlot.InitialiseSaveSlot(profileID, gameData, () => { OnDeleteSaveGameClicked(saveSlot); } );
        return saveSlot;
    }

    public void DeactivateMenu()
    {
        gameObject.SetActive(false);
    }
}
