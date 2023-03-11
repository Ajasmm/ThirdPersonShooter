using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitWinow : MonoBehaviour
{
    [SerializeField] private Button yes_Button;
    [SerializeField] private Button no_Button;

    private void OnEnable()
    {
        yes_Button.onClick.AddListener(OnYes);
        no_Button.onClick.AddListener(OnNo);
    }

    private void OnDisable()
    {
        yes_Button.onClick.RemoveAllListeners();
        no_Button.onClick.RemoveAllListeners();
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
