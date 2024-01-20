using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Health : MonoBehaviour
{
    List<GameObject> _hearts = new();

    void Awake()
    {
        foreach (Transform child in transform)
        {
            _hearts.Add(child.gameObject);
        }
    }

    void Start()
    {
        Manager_Puzzle.Instance.OnTakeHit += Hit;
    }

    public void Hit()
    {
        foreach (GameObject heart in _hearts)
        {
            if (heart.activeSelf) { heart.SetActive(false); return; }
        }

        Manager_Puzzle.Instance.PuzzleEnd(false);
    }
}
