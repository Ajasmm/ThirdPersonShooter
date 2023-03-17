using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ExitWinow : MonoBehaviour
{
    VisualElement rootVisualElement;

    Button yes_Button;
    Button no_Button;

    private void OnEnable()
    {
        rootVisualElement = GetComponent<UIDocument>().rootVisualElement;

        yes_Button = rootVisualElement.Q<Button>("Yes");
        no_Button = rootVisualElement.Q<Button>("No");

        yes_Button.clicked += OnYes;
        no_Button.clicked += OnNo;
    }
    private void OnDisable()
    {
        yes_Button.clicked -= OnYes;
        no_Button.clicked -= OnNo;
    }

    private void OnYes()
    {

#if UNITY_EDITOR 
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    private void OnNo() 
    {
        this.gameObject.SetActive(false);
    }
}
