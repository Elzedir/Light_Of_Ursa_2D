using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_Staff : Interactable
{
    bool _pickedUp = false;

    public override void Interact(GameObject interactor = null)
    {
        base.Interact(interactor);

        Manager_Game.Instance.Player.PickUpStaff();

        gameObject.SetActive(false);
    }
}
