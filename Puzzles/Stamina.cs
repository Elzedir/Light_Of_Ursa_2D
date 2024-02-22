using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Stamina : MonoBehaviour
{
    TextMeshProUGUI _staminaText;

    void Awake()
    {
        foreach (Transform child in transform)
        {
            _staminaText = child.GetComponent<TextMeshProUGUI>();
        }
    }

    void Start()
    {
        Manager_Puzzle.Instance.OnUseStamina += UseStamina;
    }

    void OnDestroy()
    {
        Manager_Puzzle.Instance.OnUseStamina -= UseStamina;
    }

    public void UseStamina(string stamina)
    {
        _staminaText.text = stamina;
    }
}
