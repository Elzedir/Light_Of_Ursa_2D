using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Signpost : MonoBehaviour
{
    [SerializeField] string _text;
    [SerializeField] float _displayTime;
    bool _isDisplaying = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_isDisplaying) return;

        _isDisplaying = true;
        Manager_FloatingText.Instance.ShowFloatingText(_text, 7, Color.red, true, transform.position, Vector3.zero, _displayTime);

        StartCoroutine(RechargeDisplay());
    }

    IEnumerator RechargeDisplay()
    {
        yield return new WaitForSeconds(_displayTime);

        _isDisplaying = false;
    }
}
