using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuUILogic : MonoBehaviour
{

    private Stack<GameObject> menuStack = new Stack<GameObject>();
    private GameObject currentMenu;

    public TMP_Text roomCodeDisplay;
    private string roomCode;
    private bool isGameOn;

    [SerializeField]
    private Button _singlePlayerGameButton;

    [SerializeField]
    private Button _multiplayerGameButton;

    [SerializeField]
    private GameObject _magicBar;

    [SerializeField]
    private GameObject _oasisTempPanel;

    [SerializeField]
    private GameObject _currentTempPanel;


    [SerializeField]
    private GameObject _gamePauseMenu;

    [SerializeField]
    private GameObject _gameUI;


    void Start()
    {
        currentMenu = GameObject.Find("MainMenu");
        if (currentMenu != null)
        {
            currentMenu.SetActive(true);
            menuStack.Push(currentMenu);
        }

        _singlePlayerGameButton.onClick.AddListener(SinglePlayerGameStart);
        _multiplayerGameButton.onClick.AddListener(MultiplayerGameStart);

    }

    public void SwitchMenu(GameObject newMenu)
    {
        if (currentMenu != null)
        {
            currentMenu.SetActive(false);
        }
        if(isGameOn)
        {
            isGameOn = false;
            // if you are in the menu (aside for the gamePauseMenu, game should be off)
            _gamePauseMenu.SetActive(false);
        }
        newMenu.SetActive(true);
        menuStack.Push(newMenu);
        currentMenu = newMenu;
        
    }

    public void GoBack()
    {
        if (menuStack.Count > 1)
        {
            menuStack.Pop().SetActive(false);
            currentMenu = menuStack.Peek();
            currentMenu.SetActive(true);
        }
    }


    public void ShowGameMenu()
    {
        if(isGameOn)
        {
            _magicBar.SetActive(false);
            _gameUI.SetActive(false);
            _gamePauseMenu.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        if (isGameOn)
        {
            _gamePauseMenu.SetActive(false);
            _magicBar.SetActive(true);
            _gameUI.SetActive(true);
        }
    }

    private void SinglePlayerGameStart()
    {
        if(currentMenu != null)
        {
            currentMenu.SetActive(false);
        }
        _magicBar.SetActive(true);
        //_currentTempPanel.SetActive(true);
        //_oasisTempPanel.SetActive(true);
        _gameUI.SetActive(true);
        isGameOn = true;
    }

    private void MultiplayerGameStart()
    {
        if(currentMenu != null)
        {
            currentMenu.SetActive(false);
        }
        _magicBar.SetActive(true);
        _gameUI.SetActive(true);
        isGameOn = true;
    }

    public bool IsGameOn()
    {
        return isGameOn;
    }


    public void QuitGame()
    {
        Application.Quit();
    }
}
