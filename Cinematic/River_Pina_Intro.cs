using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class River_Pina_Intro : Cinematic
{
    List<CinematicWaitPoint> _points = new();

    protected override void Awake()
    {
        if (SceneManager.GetActiveScene().name == "River") CreateCinematicData();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Player>(out Player player) && !_cinematicPlayed)
        {
            Manager_Game.Instance.ChangeGameState(GameState.Cinematic);
            StartCoroutine(MovePlayer(player));
            _cinematicPlayed = true;
        }
    }

    void CreateCinematicData()
    {
        CinematicWaitPoint[] River_Pina_Intro = {
        new CinematicWaitPoint(new Vector3(13f, -6.5f, 0), 0),
        new CinematicWaitPoint(new Vector3(16.5f, -8.5f, 0), 1.5f),
        new CinematicWaitPoint(new Vector3(18f, -4.75f, 0), 1f)
        };

        _points.AddRange(River_Pina_Intro);
    }

    IEnumerator MovePlayer(Player player)
    {
        Controller_Agent playerAgent = player.GetComponent<Controller_Agent>();

        if (playerAgent == null) { Debug.Log("Player does not have Controller_Agent script."); yield break; }

        if (!playerAgent.isActiveAndEnabled) playerAgent.enabled = true;

        playerAgent.ToggleAgent(true);

        foreach (CinematicWaitPoint point in _points)
        {
            playerAgent.SetAgentDetails(targetPosition: point.Position, speed: 0.5f);

            yield return new WaitUntil(() => Vector2.Distance(playerAgent.transform.position, point.Position) < 0.1f);

            yield return new WaitForSeconds(point.WaitTime);
        }

        playerAgent.ResetAgent();
        playerAgent.ToggleAgent(false);
        Manager_Game.Instance.ChangeGameState(GameState.Playing);
    }
}
