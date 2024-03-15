using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class Cell_IceWall : Cell_Base
{
    Spawner_IceWall _spawner;
    public TextMeshPro CellText;
    public bool Broken { get; private set; }
    public int CellHealth { get; private set; }
    bool _onCooldown = false;
    Coroutine _autoBreak;

    bool _staminaFinishCell = false;

    public void InitialiseCell(Coordinates coordinates, Spawner_IceWall spawner, int cellHealth)
    {
        Coordinates = coordinates;
        _spawner = spawner;
        CellHealth = cellHealth;

        _spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        _spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/Grid");

        _boxCollider = gameObject.AddComponent<BoxCollider2D>();
        _boxCollider.size = new Vector2(0.75f, 0.75f);
        _boxCollider.isTrigger = true;

        GameObject textGO = new GameObject();
        textGO.transform.parent = transform;
        textGO.transform.localPosition = Vector3.zero;
        CellText = textGO.AddComponent<TextMeshPro>();
        CellText.text = CellHealth.ToString();
        CellText.alignment = TextAlignmentOptions.Center;
        CellText.sortingLayerID = -967159649;
        CellText.fontSize = 2; 
        CellText.color = Color.red;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.name == "Focus") 
        { 
            _spawner.RefreshWall(this);

            if (_staminaFinishCell) Manager_Puzzle.Instance.PuzzleEnd(true);
        }
    }

    public bool DecreaseHealth(int maxHealth)
    {
        if (CellHealth == 0) return false;
        if (_onCooldown) return true;

        _onCooldown = true;
        CellHealth --;
        CellText.text = $"{CellHealth}->{(CellHealth / maxHealth).ToString("F2")}";
        ChangeColour((float)CellHealth / maxHealth);
        if (CellHealth == 1) _autoBreak = StartCoroutine(AutoBreak());
        
        StartCoroutine(_healthCooldown());
        return true;
    }

    public void Break()
    {
        if (_autoBreak != null) StopCoroutine(_autoBreak);
        Broken = true;
        CellText.text = "";
    }

    IEnumerator AutoBreak()
    {
        ChangeColour(1);
        List<Sprite> cracks = new();

        cracks.Add(Resources.Load<Sprite>("Sprites/Grid"));
        cracks.Add(Resources.Load<Sprite>("Sprites/Grid_OpenOneSide"));
        cracks.Add(Resources.Load<Sprite>("Sprites/Grid_OpenTwoSides"));
        cracks.Add(Resources.Load<Sprite>("Sprites/Grid_OpenThreeSides"));
        cracks.Add(Resources.Load<Sprite>("Sprites/Grid_OpenAllSides"));

        foreach (Sprite sprite in cracks)
        {
            _spriteRenderer.sprite = sprite;
            yield return new WaitForSeconds(1f);
        }

        _spriteRenderer.color = Color.black;

        Break();
    }

    public void ChangeColour(float colourScale)
    {
        _spriteRenderer.color = new Color(colourScale, colourScale, colourScale);
    }

    IEnumerator _healthCooldown()
    {
        yield return new WaitForSeconds(1);
        _onCooldown = false;
    }

    public void StaminaFinishCell()
    {
        MarkCell(Color.red);
        _staminaFinishCell = true;
    }
}
