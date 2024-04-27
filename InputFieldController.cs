using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldController : MonoBehaviour
{
    InputField _inputField;
    TextMeshProUGUI _parentText;

    bool _isInputFieldInteractable = false;

    void OnEnable()
    {
        if (!_inputField) _inputField = GetComponent<InputField>();
        _inputField.text = "";
        if (!_parentText) _parentText = GetComponentInParent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (_inputField.isFocused && _parentText.text != "")
        {
            _parentText.text = "";
        }
        else if (!_inputField.isFocused && _parentText.text != "Enter New Profile Name Here")
        {
            _parentText.text = "Enter New Profile Name Here";
        }
    }
}
