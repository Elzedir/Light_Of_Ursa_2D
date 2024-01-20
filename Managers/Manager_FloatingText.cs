using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Manager_FloatingText : MonoBehaviour
{
    public static Manager_FloatingText Instance;

    GameObject textContainer;
    GameObject textPrefab;

    List<FloatingText> _floatingTexts = new();

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); } else if (Instance != this) Destroy(gameObject);
    }

    void Start()
    {
        
    }

    public void ShowFloatingText(string text, int fontSize, Color color, bool fixedTextPosition, Vector3 position, Vector3 moveDirection, float duration)
    {
        GameObject floatingTextGO = new GameObject(text);
        FloatingText floatingText = floatingTextGO.AddComponent<FloatingText>();
        floatingText.Initialise(text, fontSize, color, moveDirection, duration, 2);
        floatingText.transform.parent = transform;
        _floatingTexts.Add(floatingText);

        if (fixedTextPosition) floatingText.transform.position = new Vector3 (position.x, position.y + 0.5f, position.z);
        else floatingText.transform.position = Camera.main.WorldToScreenPoint(position);
    }
}
