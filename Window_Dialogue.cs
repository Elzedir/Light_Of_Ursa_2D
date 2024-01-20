using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Window_Dialogue : MonoBehaviour
{
    public GameObject InteractedCharacter;
    public Image InteractedIcon;
    public TextMeshProUGUI InteractedText;

    public GameObject PlayerDialogueOptionPrefab;
    public Transform ChoiceArea;

    public bool isOpen = false;

    public void Start()
    {
        gameObject.SetActive(false);
    }

    public IEnumerator OpenDialogueWindow(GameObject interactedObject, string text)
    {
        isOpen = true;
        InteractedCharacter = interactedObject;
        gameObject.SetActive(true);

        yield return StartCoroutine(UpdateInteractedObject(text));
    }

    public IEnumerator UpdateInteractedObject(string text)
    {
        UpdateInteractedObjectImage();

        Dialogue_Text interactedTextScript = InteractedText.GetComponent<Dialogue_Text>();
        yield return StartCoroutine(interactedTextScript.UpdateDialogue(text));
    }

    public void UpdateInteractedObjectImage()
    {
        if (InteractedIcon == null) { Debug.Log("InteractedIcon is null"); return; }
        if (InteractedCharacter == null) { Debug.Log("InteractedCharacter is null"); return; }

        Sprite sprite = InteractedCharacter.GetComponent<SpriteRenderer>().sprite;

        if (sprite == null) { Debug.Log("Interacted sprite is null"); return; }

        InteractedIcon.sprite = sprite;
    }

    public void UpdateChoicesUI(DialogueLine currentLine)
    {
        foreach (Transform child in ChoiceArea)
        {
            Destroy(child.gameObject);
        }

        if (currentLine.Choices != null && currentLine.Choices.Length > 0)
        {
            int i = 0;

            foreach (Transform child in ChoiceArea)
            {
                if (i < currentLine.Choices.Length)
                {
                    Button choiceButton = child.GetComponent<Button>();
                    TextMeshProUGUI buttonText = choiceButton.GetComponentInChildren<TextMeshProUGUI>();

                    if (choiceButton != null && buttonText != null)
                    {
                        DialogueChoice choice = currentLine.Choices[i];
                        buttonText.text = choice.Choice;
                        choiceButton.onClick.AddListener(() => Manager_Dialogue.Instance.OptionSelected(choice, ChoiceArea));

                        i++;
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }

    public void CloseDialogueWindow()
    {
        isOpen = false;
        gameObject.SetActive(false);
        Manager_Dialogue.Instance.StopDialogue();
    }
}
