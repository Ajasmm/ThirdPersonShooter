using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private UIDocument mainMenu;

    VisualElement rootVisualElement;

    Button play_Button;
    Button settings_Button;
    Button exit_Button;

    [Header("Menu Windows")]
    [SerializeField] private GameObject settingsWindow;
    [SerializeField] private GameObject exitWindow;
    
    MyInput inputSystem;

    private void OnEnable()
    {
        inputSystem = GameManager.Instance.input;
        inputSystem.Menu.Enable();

        inputSystem.Menu.Escape.performed += OnExitByEscapeButton;

        rootVisualElement = mainMenu.GetComponent<UIDocument>().rootVisualElement;

        play_Button = rootVisualElement.Q<Button>("Play");
        settings_Button = rootVisualElement.Q<Button>("Settings");
        exit_Button = rootVisualElement.Q<Button>("Exit");

        play_Button.clicked += OnPlay;
        settings_Button.clicked += OnSettings;
        exit_Button.clicked += OnExit;

        DisableAllWindows();

        Time.timeScale = 1.0f;
        GameManager.Instance.EnableCursor();
    }

    private void OnDisable()
    {
        inputSystem.Menu.Disable();

        inputSystem.Menu.Escape.performed -= OnExitByEscapeButton;

        play_Button.clicked -= OnPlay;
        settings_Button.clicked -= OnSettings;
        exit_Button.clicked -= OnExit;

        GameManager.Instance.DisableCursor();
    }

    public void OnPlay()
    {
        SceneManager.LoadSceneAsync(1);
        GameManager.Instance.CurrentGamePlayMode?.Stop();
        play_Button.SetEnabled(false);
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
