using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Interactable_Puzzle : Interactable
{
    [SerializeField] public PuzzleData PuzzleData;

    BoxCollider2D _collider;

    protected virtual void Awake()
    {
        PuzzleData.PuzzleID = gameObject.name;
        _collider = GetComponent<BoxCollider2D>();
    }

    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);

        if (interactor.gameObject.TryGetComponent<Player>(out Player player) && !PuzzleData.PuzzleCompleted)
        { 
            Manager_Game.Instance.LoadScene("Puzzle", this); 
        }
    }

    public void CompletePuzzle()
    {
        if (PuzzleData.PuzzleRepeatable) return;

        PuzzleData.PuzzleCompleted = true;

        switch (PuzzleData.PuzzleSet)
        {
            case PuzzleSet.Directional:
                GetComponent<BoxCollider2D>().isTrigger = true;
                break;
            default:
                break;
        }
    }
}
