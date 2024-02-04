using UnityEngine;

public class Chaser : MonoBehaviour
{
    public Cell TargetCell;

    public void BlowUp()
    {
        Destroy(gameObject);
    }
}
