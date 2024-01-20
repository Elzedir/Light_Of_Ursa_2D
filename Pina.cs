using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pina : Controller_Agent
{
    protected override void SubscribeToEvents()
    {
        Manager_Dialogue.Instance.pinaIntroEvent?.AddListener(PinaIntro);
    }

    void PinaIntro()
    {
        FollowUrsus();
    }

    void FollowUrsus()
    {
        SetAgentDetails(targetGO: Manager_Game.Instance.Player.gameObject, followDistance: 1.5f);
    }

    void OnDestroy()
    {
        Manager_Dialogue.Instance.luxIntroEvent?.RemoveListener(PinaIntro);
    }
}
