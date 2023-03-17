using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameEndWindow : MonoBehaviour
{
    [SerializeField] int sceneIndex = -1;

    VisualElement rootVisualElement;

    Button mainMenu_Button;
    Button restart_Button;
    Button nextLevel_Button;


    private void OnEnable()
    {
        rootVisualElement = gameObject.GetComponent<UIDocument>().rootVisualElement;

        restart_Button = rootVisualElement.Q<Button>("Restart");
        nextLevel_Button = rootVisualElement.Q<Button>("Next");
        mainMenu_Button = rootVisualElement.Q<Button>("Menu");

        mainMenu_Button.clicked += OnMainMenu;
        restart_Button.clicked += OnRestart;

        if (sceneIndex > 0) nextLevel_Button.clicked += OnNextLevel;
        else if (nextLevel_Button != null) rootVisualElement.Q<VisualElement>("Buttons").Remove(nextLevel_Button);

        GameManager.Instance.EnableCursor();
    }


    private void OnDisable()
    {
        mainMenu_Button.clicked -= OnMainMenu;
        restart_Button.clicked -= OnRestart;

        if (sceneIndex > 0) nextLevel_Button.clicked -= OnNextLevel;
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
        mainMenu_Button.SetEnabled(false);
        restart_Button.SetEnabled(false);

        if (sceneIndex < 0) nextLevel_Button?.SetEnabled(false);
    }
}
