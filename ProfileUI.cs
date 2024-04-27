using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileUI : MonoBehaviour
{
    Menu_Main _menu;
    string _profileName;
    Button _button;

    public void Initialise(Menu_Main menu, string profileName)
    {
        _menu = menu;
        _profileName = profileName;
        TextMeshProUGUI text = new GameObject("ProfileText").AddComponent<TextMeshProUGUI>();
        text.text = _profileName;
        text.transform.SetParent(transform);
        text.color = Color.black;
        text.alignment = TextAlignmentOptions.Center;
        gameObject.AddComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Grid");
        _button = gameObject.AddComponent<Button>();
        _button.onClick.AddListener(ProfileClicked);
    }

    public void ProfileClicked()
    {
        _menu.SwitchProfile(_profileName);
    }
}
