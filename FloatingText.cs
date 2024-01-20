using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    TextMeshPro _text;
    Vector3 _moveDirection;
    float _duration;
    float _fadeDuration;
    float _lastShown;
    bool _isHiding = false;

    public void Initialise(string text, int fontSize, Color color, Vector3 moveDirection, float duration, float fadeDuration)
    {
        _text = gameObject.AddComponent<TextMeshPro>();

        _text.text = text;
        _text.fontSize = fontSize;
        _text.color = color;
        _text.alignment = TextAlignmentOptions.Center;
        _text.sortingLayerID = -967159649;
        _moveDirection = moveDirection;
        _duration = duration;
        _fadeDuration = fadeDuration;
        _lastShown = Time.time;

        StartCoroutine(Show());
    }

    void Update()
    {
        UpdateFloatingText();
    }

    IEnumerator Show()
    {
        float elapsedTime = 0;

        while (elapsedTime < _fadeDuration)
        {
            _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, Mathf.Lerp(0, _text.color.a, elapsedTime / _fadeDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, 1);
    }

    IEnumerator Hide()
    {
        float elapsedTime = 0;
        while (elapsedTime < _fadeDuration)
        {
            _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, Mathf.Lerp(_text.color.a, 0, elapsedTime / _fadeDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, 0);

        Destroy(gameObject, 0.1f);
    }

    void UpdateFloatingText()
    {
        if (!_isHiding && Time.time - _lastShown >= _duration)
        {
            StartCoroutine(Hide());
            _isHiding = true;
        }

        transform.position += _moveDirection * Time.deltaTime;
    }
}
