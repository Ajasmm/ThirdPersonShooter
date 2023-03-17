using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsWindow : MonoBehaviour
{
    VisualElement rootVisualElement;
    Button exit_Button;

    private void OnEnable()
    {
        rootVisualElement = GetComponent<UIDocument>().rootVisualElement;

        exit_Button = rootVisualElement.Q<Button>("Exit");

        exit_Button.clicked += OnExit;
    }
    private void OnDisable()
    {
        exit_Button.clicked -= OnExit;
    }

    private void OnExit()
    {
       this.gameObject.SetActive(false);
    }
}
