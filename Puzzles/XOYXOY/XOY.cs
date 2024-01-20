using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XOY : MonoBehaviour
{
    SpriteRenderer _spriteRenderer;
    public Spawner_XOY Spawner;
    public int CurrentSpriteIndex;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Controller_Puzzle_XOY>()) ChangeXOY();
    }

    void ChangeXOY()
    {
        if (_spriteRenderer == null) { Debug.Log("SpriteRenderer is null;"); return; }

        CurrentSpriteIndex = (CurrentSpriteIndex + 1) % Spawner.XoySprites.Length;

        _spriteRenderer.sprite = Spawner.XoySprites[CurrentSpriteIndex];
        
        switch (CurrentSpriteIndex)
        {
            case 0:
                transform.parent = Spawner.XParent;
                break;
            case 1:
                transform.parent = Spawner.OParent;
                break;
            case 2:
                transform.parent = Spawner.YParent;
                break;
        }
    }
}
