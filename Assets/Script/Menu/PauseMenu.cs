using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{

    [SerializeField] Button continue_Button;
    [SerializeField] Button restart_Buttom;
    [SerializeField] Button exitToMenu_Button;

    MyInput input;

    private void OnEnable()
    {
        input = GameManager.Instance.input;
        input.Menu.Escape.performed += OnClose;

        Time.timeScale = 0;
        input.Character.Disable();

        continue_Button.onClick.AddListener(OnContinue);
        restart_Buttom.onClick.AddListener(OnRestart);
        exitToMenu_Button.onClick.AddListener(OnExitToMenu);


        GameManager.Instance.EnableCursor();
    }
    private void OnDisable()
    {
        input.Menu.Escape.performed -= OnClose;

        continue_Button.onClick.RemoveAllListeners();
        restart_Buttom.onClick.RemoveAllListeners();
        exitToMenu_Button.onClick.RemoveAllListeners();


        GameManager.Instance.DisableCursor();
    }

    private void OnContinue()
    {
        GameManager.Instance.CurrentGamePlayMode.Resume();
    }
    private void OnRestart()
    {
        Time.timeScale= 1.0f;
        SceneManager.LoadSceneAsync(this.gameObject.scene.buildIndex);
        DisableButtons();
    }
    private void OnExitToMenu()
    {
        GameManager.Instance.CurrentGamePlayMode.Stop();
        SceneManager.LoadSceneAsync(0);
        DisableButtons();
    }
    private void OnClose(InputAction.CallbackContext context)
    {
        OnContinue();
    }

    private void DisableButtons()
    {
        continue_Button.interactable = false;
        restart_Buttom.interactable = false;
        exitToMenu_Button.interactable = false;
    }
}
