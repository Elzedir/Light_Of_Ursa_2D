using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Window_Text : MonoBehaviour
{
    TextMeshProUGUI _text;

    void Awake()
    {
        gameObject.SetActive(false);
    }

    public IEnumerator UpdateText(string text, float fadeInTime = 1, float persistTime = 1, float fadeOutTime = 1)
    {
        gameObject.SetActive(true);

        if (!_text) _text = GetComponentInChildren<TextMeshProUGUI>();

        _text.text = text;

        yield return StartCoroutine(TextFadeIn(fadeInTime, persistTime, fadeOutTime));
    }

    IEnumerator TextFadeIn(float fadeInTime, float persistTime, float fadeOutTime)
    {
        float elapsedTime = 0;
        Color startColor = new Color(_text.color.r, _text.color.g, _text.color.b, 0);
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 1);

        while (elapsedTime < fadeInTime)
        {
            elapsedTime += Time.deltaTime;
            _text.color = Color.Lerp(startColor, endColor, elapsedTime / fadeInTime);
            yield return null;
        }

        _text.color = endColor;

        yield return new WaitForSeconds(persistTime);

        StartCoroutine(TextFadeOut(fadeOutTime));
    }

    IEnumerator TextFadeOut(float fadeOutTime)
    {
        float elapsedTime = 0;
        Color startColor = _text.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0);

        while (elapsedTime < fadeOutTime)
        {
            elapsedTime += Time.deltaTime;
            _text.color = Color.Lerp(startColor, endColor, elapsedTime / fadeOutTime);
            yield return null;
        }

        _text.color = endColor;

        gameObject.SetActive(false);
    }
}
