using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public virtual void Interact(GameObject interactor)
    {
        if (!WithinInteractRange(interactor)) { Debug.Log("Not within interact range."); return; }
    }

    bool WithinInteractRange(GameObject interactor)
    {
        return Vector3.Distance(transform.position, interactor.transform.position) <= Manager_Game.Instance.InteractRange;
    }
}
