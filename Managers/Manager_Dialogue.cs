using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Manager_Dialogue : MonoBehaviour
{
    public static Manager_Dialogue Instance;

    public List<DialogueConversation> Conversations { get; private set; } = new();

    Window_Dialogue _window_Dialogue;
    public GameObject InteractedCharacter;
    public bool optionSelected = false;

    Coroutine _dialogueCoroutine;
    DialogueConversation _currentConversation;
    public bool StopCurrentDialogue { get; private set; } = false;

    public UnityEvent luxIntroEvent = new();
    public UnityEvent pinaIntroEvent = new();

    void Awake()
    {
        Instance = this;
    }

    public void OnSceneLoaded()
    {
        if (SceneManager.GetActiveScene().name == "Main_Menu") return;
        FindDialogueWindow();
        InitialiseDialogue();
    }

    void FindDialogueWindow()
    {
        _window_Dialogue = 
            Manager_Game.FindTransformRecursively(GameObject.Find("UI").transform, "Window_Dialogue").TryGetComponent<Window_Dialogue>(out Window_Dialogue windowDialogue) == true 
            ? windowDialogue 
            : null;
    }

    void InitialiseDialogue()
    {
        PrimaryCharacterDialogue();
        SecondaryCharacterDialogue();
    }

    void PrimaryCharacterDialogue()
    {
        Ursus();
        Pina();
        Lux();
    }

    void Ursus()
    {

    }

    void Pina()
    {
        PinaIntro();
    }

    void PinaIntro()
    {
        DialogueLine[] pinaIntroLines = {
            new DialogueLine("Who are you?", 3),
            new DialogueLine("Well, good to meet you.", 3),
            new DialogueLine("Let's go find out why there's no water. I think it's North.", 3, endOfConversation: true, dialogueEvent: pinaIntroEvent)
        };

        DialogueConversation pinaIntro = new DialogueConversation().InitialiseConversation(pinaIntroLines, "Pina", "River");
        Conversations.Add(pinaIntro);
    }

    void Lux()
    {
        LuxIntro();
    }

    void LuxIntro()
    {
        DialogueLine[] luxIntroLines = {
            new DialogueLine("Hi there, good morning! You slept really late! I hope that I didn't wake you too early, but you nearly overslept!", 3),
            new DialogueLine("My name is Lux, and I'm here to help you this year. Boy, it's quite an honour, I must say. I hope I'm up for the task!", 3),
            new DialogueLine("I really look forward to our adventures together!", 3),
            new DialogueLine("For now, we need to get to the old tree.", 3),
            new DialogueLine("To get to the Old Tree, we need to exit the cave.", 3, endOfConversation: true, dialogueEvent: luxIntroEvent)
        };

        DialogueConversation luxIntro = new DialogueConversation().InitialiseConversation(luxIntroLines, "Lux", "Ursus_Cave");
        Conversations.Add(luxIntro);
    }

    void SecondaryCharacterDialogue()
    {

    }

    public void OpenDialogue(GameObject interactedCharacter, DialogueConversation dialogueConversation, int dialogueIndex = 0)
    {
        StopDialogue();

        if (interactedCharacter == null) { Debug.LogWarning($"Interacted Character: {interactedCharacter} is null."); return; }

        StopCurrentDialogue = false;
        InteractedCharacter = interactedCharacter; _currentConversation = dialogueConversation;

        if (dialogueConversation.Completed && dialogueConversation.Lines.Count > 0) dialogueIndex = dialogueConversation.Lines.Count - 1;
        
        _dialogueCoroutine = StartCoroutine(DisplayDialogue(dialogueConversation, dialogueIndex));
    }

    public void OptionSelected(DialogueChoice dialogueChoice, Transform choiceArea)
    {
        optionSelected = true;

        foreach (Transform child in choiceArea)
        {
            Destroy(child.gameObject);
        }

        if (dialogueChoice.ReturnToIndex >= 0)
        {
            OpenDialogue(InteractedCharacter, dialogueConversation: _currentConversation, dialogueIndex: dialogueChoice.ReturnToIndex);
        }

        else if (dialogueChoice.NextLine != null)
        {
            OpenDialogue(InteractedCharacter, dialogueConversation: dialogueChoice.NextLine);
        }
    }

    IEnumerator DisplayDialogue(DialogueConversation conversation, int lineIndex)
    {
        for (; lineIndex < conversation.Lines.Count; lineIndex++)
        {
            yield return StartCoroutine(HandleDialogueLine(conversation.Lines[lineIndex]));
            {
                if (conversation.Lines[lineIndex].EndOfConversation) conversation.Completed = true;
                if (StopCurrentDialogue) break;
            }
        }
    }

    IEnumerator HandleDialogueLine(DialogueLine line)
    {
        yield return StartCoroutine(_window_Dialogue.OpenDialogueWindow(InteractedCharacter, line.Line));

        while (!_window_Dialogue.InteractedText.GetComponent<Dialogue_Text>().IsFinishedTyping()) yield return null;

        _window_Dialogue.InteractedText.GetComponent<Dialogue_Text>().ResetFinishedTyping();

        _window_Dialogue.UpdateChoicesUI(line);

        line.DialogueEvent?.Invoke();
        
        if (line.DisplayTime != 0)
        {
            yield return StartCoroutine(WaitForDisplayTimeOrEnter(line.DisplayTime));

            if (line.NextLine != null) OpenDialogue(InteractedCharacter, line.NextLine);
        }
        else
        {
            if (line.Choices.Length > 0)
            {
                while (!optionSelected) yield return null;
                if (optionSelected) optionSelected = false;
            }
        }
    }

    IEnumerator WaitForDisplayTimeOrEnter(float displayTime)
    {
        float startTime = Time.time;

        while (Time.time - startTime < displayTime)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                break;
            }

            yield return null;
        }
    }

    public void StopDialogue()
    {
        if (_dialogueCoroutine == null) return;

        StopCoroutine(_dialogueCoroutine);
        _dialogueCoroutine = null;
        StopCurrentDialogue = true;
    }

    public DialogueConversation GetConversation(string name)
    {
        return Conversations.FirstOrDefault(conversation =>
        conversation.Scene == SceneManager.GetActiveScene().name &&
        conversation.Name == name);
    }
}

public class DialogueConversation
{
    public bool Completed;
    public string Name;
    public string Scene;
    public List<DialogueLine> Lines = new();

    public DialogueConversation InitialiseConversation(DialogueLine[] lines, string name, string scene) { Name = name; Scene = scene; Lines.AddRange(lines); return this; }
}

public class DialogueLine
{
    public string Line;
    public DialogueChoice[] Choices;
    public int DisplayTime;
    public DialogueConversation NextLine;
    public bool EndOfConversation;
    public UnityEvent DialogueEvent;

    public DialogueLine(string line, int displayTime = 0, DialogueConversation nextLine = null, DialogueChoice[] choices = null, bool endOfConversation = false, UnityEvent dialogueEvent = null)
    {
        if (displayTime == 0 && nextLine == null) { Debug.Log("Both display time and next line are null"); return; }

        Line = line;
        Choices = choices;
        DisplayTime = displayTime;
        NextLine = nextLine;
        EndOfConversation = endOfConversation;
        DialogueEvent = dialogueEvent;
    }
}

public class DialogueChoice
{
    public string Choice;
    public int ReturnToIndex = -1;
    public DialogueConversation NextLine;

    public DialogueChoice(string choice, int returnToIndex = -1, DialogueConversation nextConversation = null)
    {
        if (returnToIndex == -1 && nextConversation == null) { Debug.Log("Return line and return conversation are both null"); return; }

        Choice = choice;
        ReturnToIndex = returnToIndex;
        NextLine = nextConversation;
    }
}