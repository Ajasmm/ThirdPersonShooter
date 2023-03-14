using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] Button play_Button;
    [SerializeField] Button settings_Button;
    [SerializeField] Button exit_Button;

    [Header("Menu Windows")]
    [SerializeField] private GameObject settingsWindow;
    [SerializeField] private GameObject exitWindow;
    
    MyInput inputSystem;

    private void OnEnable()
    {
        inputSystem = GameManager.Instance.input;
        inputSystem.Menu.Enable();

        inputSystem.Menu.Escape.performed += OnExitByEscapeButton;

        play_Button.onClick.AddListener(OnPlay);
        settings_Button.onClick.AddListener(OnSettings);
        exit_Button.onClick.AddListener(OnExit);

        DisableAllWindows();

        Time.timeScale = 1.0f;
        GameManager.Instance.EnableCursor();
    }

    private void OnDisable()
    {
        inputSystem.Menu.Disable();

        inputSystem.Menu.Escape.performed -= OnExitByEscapeButton;

        play_Button.onClick.RemoveAllListeners();
        settings_Button.onClick.RemoveAllListeners();
        exit_Button.onClick.RemoveAllListeners();

        GameManager.Instance.DisableCursor();
    }

    public void OnPlay()
    {
        SceneManager.LoadSceneAsync(1);
        GameManager.Instance.CurrentGamePlayMode?.Stop();
        play_Button.interactable = false;
    }

    public void OnSettings()
    {
        DisableAllWindows();
        settingsWindow.SetActive(true);
    }

    public void OnExit()
    {
        DisableAllWindows();
        exitWindow.SetActive(true);
    }
    public void OnExitByEscapeButton(InputAction.CallbackContext context)
    {
        OnExit();
    }

    private void DisableAllWindows()
    {
        settingsWindow.SetActive(false);
        exitWindow.SetActive(false);
    }
}
