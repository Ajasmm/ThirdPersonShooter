using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsWindow : MonoBehaviour
{
    [SerializeField] Button exit_Button;

    private void OnEnable()
    {
        exit_Button.onClick.AddListener(OnExit);
    }
    private void OnDisable()
    {
        exit_Button.onClick.RemoveAllListeners();
    }

    private void OnExit()
    {
       this.gameObject.SetActive(false);
    }
}
