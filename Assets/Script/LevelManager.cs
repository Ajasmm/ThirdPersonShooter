using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] GamePlayMode gamePlayMode;
    Coroutine initRoutine;

    private void OnEnable()
    {
        InitializeAsync();
    }
   
    private async void InitializeAsync()
    {
        Task waiForPlayerTask = GameManager.Instance.WaitForPlayer();
        if(!waiForPlayerTask.IsCompleted) await waiForPlayerTask;

        if (gamePlayMode) GameManager.Instance?.RegisterGamePlayMode(gamePlayMode);
    }
}
