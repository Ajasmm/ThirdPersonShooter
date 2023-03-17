using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PauseMenu : MonoBehaviour
{
    VisualElement rootElement;

    Button resume_Button;
    Button restart_Buttom;
    Button menu_Button;

    MyInput input;

    private void OnEnable()
    {
        input = GameManager.Instance.input;
        input.Menu.Escape.performed += OnClose;

        rootElement = GetComponent<UIDocument>().rootVisualElement;

        resume_Button = rootElement.Q<Button>("Resume");
        restart_Buttom = rootElement.Q<Button>("Restart");
        menu_Button = rootElement.Q<Button>("Menu");

        resume_Button.clicked += OnResume;
        restart_Buttom.clicked += OnRestart;
        menu_Button.clicked += OnMenu;

        GameManager.Instance.EnableCursor();
    }
    private void OnDisable()
    {
        input.Menu.Escape.performed -= OnClose;

        resume_Button.clicked -= OnResume;
        restart_Buttom.clicked -= OnRestart;
        menu_Button.clicked -= OnMenu;

        GameManager.Instance.DisableCursor();
    }

    private void OnResume()
    {
        GameManager.Instance.CurrentGamePlayMode.Resume();
    }
    private void OnRestart()
    {
        Time.timeScale= 1.0f;
        SceneManager.LoadSceneAsync(this.gameObject.scene.buildIndex);
        DisableButtons();
    }
    private void OnMenu()
    {
        GameManager.Instance.CurrentGamePlayMode.Stop();
        SceneManager.LoadSceneAsync(0);
        DisableButtons();
    }
    private void OnClose(InputAction.CallbackContext context)
    {
        OnResume();
    }

    private void DisableButtons()
    {
        resume_Button.SetEnabled(false);
        restart_Buttom.SetEnabled(false);
        menu_Button.SetEnabled(false);
    }
}
