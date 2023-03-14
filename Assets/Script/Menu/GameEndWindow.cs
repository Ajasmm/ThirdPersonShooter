using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameEndWindow : MonoBehaviour
{
    [SerializeField] Button mainMenu_Button;
    [SerializeField] Button restart_Button;
    [SerializeField] Button nextLevel_Button;
    [SerializeField] int sceneIndex;


    private void OnEnable()
    {
        if(mainMenu_Button) mainMenu_Button.onClick.AddListener(OnMainMenu);
        if (restart_Button) restart_Button.onClick.AddListener(OnRestart);
        if (nextLevel_Button) nextLevel_Button.onClick.AddListener(OnNextLevel);
        GameManager.Instance.EnableCursor();
    }

    private void OnDisable()
    {
        if (mainMenu_Button) mainMenu_Button.onClick.RemoveListener(OnMainMenu);
        if (restart_Button) restart_Button.onClick.RemoveListener(OnRestart);
        if (nextLevel_Button) nextLevel_Button.onClick.RemoveListener(OnNextLevel);
    }
    
    private void OnMainMenu()
    {
        SceneManager.LoadSceneAsync(0);
        DisableAllButtons();
    }
    private void OnRestart()
    {
        SceneManager.LoadSceneAsync(this.gameObject.scene.buildIndex);
        DisableAllButtons();
    }
    private void OnNextLevel()
    {
        SceneManager.LoadSceneAsync(sceneIndex);
        gameObject.SetActive(false);
    }
    private void DisableAllButtons()
    {
        mainMenu_Button.interactable = false;
        restart_Button.interactable = false;
    }
}
