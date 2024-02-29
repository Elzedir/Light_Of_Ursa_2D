using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SaveSlot_DeleteConfirmation : MonoBehaviour
{
    TextMeshProUGUI _displayText;
    Button _confirmButton;
    Button _cancelButton;

    void _getReferences()
    {
        _displayText = Manager_Game.FindTransformRecursively(transform, "ConfirmationText").GetComponent<TextMeshProUGUI>();
        _confirmButton = Manager_Game.FindTransformRecursively(transform, "ConfirmButton").GetComponent<Button>();
        _cancelButton = Manager_Game.FindTransformRecursively(transform, "CancelButton").GetComponent<Button>();
    }

    public void ActivateMenu(string displayText, UnityAction confirmAction, UnityAction cancelAction)
    {
        if (_displayText == null || _confirmButton == null || _cancelButton == null) _getReferences();

        gameObject.SetActive(true);

        _displayText.text = displayText;

        _confirmButton.onClick.RemoveAllListeners();
        _cancelButton.onClick.RemoveAllListeners();

        _confirmButton.onClick.AddListener(() =>
        {
            _deactivateMenu();
            confirmAction();
        });
        _cancelButton.onClick.AddListener(() =>
        {
            _deactivateMenu();
            cancelAction();
        });
    }

    void _deactivateMenu()
    {
        gameObject.SetActive(false);
    }
}
