using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    [Header("Profile")]
    [SerializeField] string _profileID = "";

    [Header("Content")]
    [SerializeField] GameData _saveGameData;
    
    public bool HasData { get; private set; } = false;

    Button _profileIDButton;
    TextMeshProUGUI _profileIDText;
    Button _clearSaveButton;

    public void InitialiseSaveSlot(string profileID, GameData gameData, UnityAction loadGameAction, UnityAction clearSaveAction)
    {
        if (!_profileIDButton) _profileIDButton = Manager_Game.FindTransformRecursively(transform, "ProfileIDButton").gameObject.GetComponent<Button>();
        if (!_profileIDText) _profileIDText = Manager_Game.FindTransformRecursively(transform, "ProfileID").gameObject.GetComponent<TextMeshProUGUI>();
        if (!_clearSaveButton) _clearSaveButton = Manager_Game.FindTransformRecursively(transform, "ClearSaveButton").gameObject.GetComponent<Button>();

        _profileID = profileID;
        _profileIDText.text = profileID;
        if (gameData != null) { _saveGameData = gameData; HasData = true; }
        else _saveGameData = new GameData(_saveGameData.CurrentProfileName);

        _profileIDButton.onClick.AddListener(() =>
        {
            loadGameAction();
        });

        _clearSaveButton.onClick.AddListener(() =>
        {
            clearSaveAction();
        });
    }

    public string GetProfileID()
    {
        return _profileID;
    }
}