using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    BoxCollider2D TransitionArea;
    [SerializeField] string SceneName;

    public void Start()
    {
        TransitionArea = GetComponent<BoxCollider2D>();
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.GetComponent<Player>()) return;
        if (string.IsNullOrEmpty(SceneName)) { Debug.Log("String is null."); return; }
        if (SceneManager.GetSceneByName(SceneName) == null) { Debug.Log("Scene name does not exist."); return; }
        
        Manager_Game.Instance.LoadScene(SceneName);
    }
}
