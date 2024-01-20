using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cinematic : MonoBehaviour
{
    protected bool _cinematicPlayed = false;

    protected virtual void Awake()
    {
        
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.TryGetComponent<Player>(out Player player) && !_cinematicPlayed)
        //{
        //    Manager_Cutscene.Instance.PlayCutscene(name);
        //    RunCutscene();
        //    _cinematicPlayed = true;
        //}
    }
}
