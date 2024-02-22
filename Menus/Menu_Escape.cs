using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu_Escape : Menu_Base
{
    public static Menu_Escape Instance;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        gameObject.SetActive(false);
    }

    public void ToggleMenu()
    {
        if (!_isOpen) OpenMenu();
        else CloseMenu();
    }

    public override void OpenMenu(GameObject interactedObject = null)
    {
        gameObject.SetActive(true);
        _isOpen = true;
        Manager_Game.Instance.ChangeGameState(GameState.Paused);
    }
    public override void CloseMenu(GameObject interactedObject = null)
    {
        gameObject.SetActive(false);
        _isOpen = false;
        Manager_Game.Instance.ChangeGameState(GameState.Playing);
    }

    public void Settings()
    {
        //CloseMenu();
        //Menu_Settings.Instance.OpenMenu();
    }

    public void QuitToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void QuitToDesktop()
    {
        Application.Quit();
    }
}
