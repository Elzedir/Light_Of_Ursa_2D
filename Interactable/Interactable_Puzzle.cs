using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class Interactable_Puzzle : Interactable
{
    public PuzzleSet PuzzleSet;
    public List<IceWallType> IceWallTypes;
    public MazeType MazeType;
    public PuzzleData PuzzleData;

    BoxCollider2D _collider;

    protected virtual void Awake()
    {
        if (PuzzleData.PuzzleID == "") PuzzleData.PuzzleID = gameObject.name;
        _collider = GetComponent<BoxCollider2D>();
    }

    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);

        if (interactor.gameObject.TryGetComponent<Player>(out Player player) && !PuzzleData.PuzzleState.PuzzleCompleted)
        { 
            Manager_Game.Instance.LoadScene("Puzzle", this); 
        }
    }

    public void CompletePuzzle()
    {
        if (PuzzleData.PuzzleState.PuzzleRepeatable) return;

        PuzzleData.PuzzleState.PuzzleCompleted = true;
        GetComponent<BoxCollider2D>().isTrigger = true;
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Blue ground");

        switch (PuzzleSet)
        {
            case PuzzleSet.Directional:
                break;
            default:
                break;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Interactable_Puzzle))]
[CanEditMultipleObjects]
public class PuzzleDataEditor : Editor
{
    SerializedProperty _puzzleData;
    SerializedProperty _puzzleSet;

    void OnEnable()
    {
        _puzzleData = serializedObject.FindProperty("PuzzleData");
        _puzzleSet = serializedObject.FindProperty("PuzzleSet");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_puzzleSet, false);

        if (_puzzleSet.enumValueIndex != (int)PuzzleSet.None)
        {
            if (_puzzleSet.enumValueIndex == (int)PuzzleSet.IceWall)
            {
                SerializedProperty iceWallTypes = serializedObject.FindProperty("IceWallTypes");
                EditorGUILayout.PropertyField(iceWallTypes, true);

                if (iceWallTypes.isArray && iceWallTypes.arraySize > 0)
                {
                    
                    EditorGUILayout.PropertyField(_puzzleData.FindPropertyRelative("IceWallData"), true);
                }
            }

            // When puzzle stlye is selected, then you show these.
            EditorGUILayout.PropertyField(_puzzleData.FindPropertyRelative("PuzzleObjectives"), true);
            EditorGUILayout.PropertyField(_puzzleData.FindPropertyRelative("PuzzleState"), true);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif