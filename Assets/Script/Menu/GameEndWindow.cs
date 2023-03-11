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


    private void OnEnable()
    {
        mainMenu_Button.onClick.AddListener(OnMainMenu);
        restart_Button.onClick.AddListener(OnRestart);

        GameManager.Instance.EnableCursor();
    }

    private void OnDisable()
    {
        mainMenu_Button.onClick.RemoveListener(OnMainMenu);
        restart_Button.onClick.RemoveListener(OnRestart);
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

    private void DisableAllButtons()
    {
        mainMenu_Button.interactable = false;
        restart_Button.interactable = false;
    }
}
