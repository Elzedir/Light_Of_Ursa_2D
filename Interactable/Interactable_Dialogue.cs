using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_Dialogue : Interactable
{
    public override void Interact(GameObject interactor = null)
    {
        base.Interact(interactor);

        Manager_Dialogue.Instance.OpenDialogue(gameObject, Manager_Dialogue.Instance.GetConversation(name));
    }
}
