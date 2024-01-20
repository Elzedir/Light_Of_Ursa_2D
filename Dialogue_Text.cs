using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Dialogue_Text : MonoBehaviour
{
    TextMeshProUGUI TextBox;
    [SerializeField] float delayBetweenCharacters = 0.1f;
    bool finishedTyping = false;

    void Awake()
    {
        TextBox = GetComponent<TextMeshProUGUI>();
    }

    public IEnumerator UpdateDialogue(string text)
    {
        TextBox.text = "";
        yield return StartCoroutine(WriteTextCoroutine(text));
    }

    IEnumerator WriteTextCoroutine(string text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            TextBox.text += text[i];
            float delay = Input.GetKey(KeyCode.Space) ? delayBetweenCharacters / 10 : delayBetweenCharacters;
            yield return new WaitForSeconds(delay);
            if (Manager_Dialogue.Instance.StopCurrentDialogue) break;
        }

        finishedTyping = true;
    }

    public bool IsFinishedTyping()
    {
        return finishedTyping;
    }

    public void ResetFinishedTyping()
    {
        finishedTyping = false;
    }
}
