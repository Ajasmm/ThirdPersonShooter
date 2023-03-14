using System;
using TMPro;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using Unity.VisualScripting;
using UnityEngine.Windows;

public class WareHouse_GamePlayMode : GamePlayMode
{
    [SerializeField] PlayableDirector director;
    [SerializeField] List<AI_Enemy> zombies;
    [SerializeField] float timeToCompleteLevel;
    [SerializeField] TMP_Text time;
    [SerializeField] TMP_Text zombieCount;
    [SerializeField] GunInfo[] gunInfos;


    Transform playerTransform;
    PlayerController playerController;

    MyInput _Input;

    float countDownTime;
    int totalZombies;

    public override void Initialize()
    {
        GetPlayer();

        DisableAllWindows();
        _Input = GameManager.Instance.input;
        _Input.Character.Escape.performed += OnPause;

        countDownTime = timeToCompleteLevel;
        totalZombies = zombies.Count;

        AddData("GunInfos", gunInfos);
        foreach (AI_Enemy enemy in zombies) enemy.OnDestroy += OnZombieDie;
        UpdateZombieCount();
        UpdateTime(countDownTime);

        PlayTimeLine();
    }
    private async void GetPlayer()
    {
        Task waitForPlayer = GameManager.Instance.WaitForPlayer();
        await waitForPlayer;

        player = GameManager.Instance.player;
        playerTransform = player.GetComponent<Transform>();
        playerController = player.GetComponent<PlayerController>();
        playerController.ResetHealth();
        playerController.DisableRagdoll();
        playerController.ResetGunAndInventory();
        playerController.EnableCharacterInput(false);

        playerTransform.gameObject.SetActive(false);
        playerTransform.SetPositionAndRotation(playerStartPos_Cinematics.position, playerStartPos_Cinematics.rotation);
        playerTransform.gameObject.SetActive(true);
    }

    private async void PlayTimeLine()
    {
        GameManager.Instance.DisableCursor();

        Task waitForPlayer = GameManager.Instance.WaitForPlayer();
        await waitForPlayer;

        _Input.TimeLine.Skip.performed += SkipCinematics;
        if (director != null)
        {
            _Input.Disable();
            _Input.TimeLine.Enable();

            director.stopped += OnTimeLineStop;
            Time.timeScale = 1;
            playerController.EnableCharacterInput(false);

            director.Play();
        }
        else OnTimeLineStop(null);
    }

    public override void Play()
    {

        Time.timeScale = 1;
        isPlaying = true;

        _Input.Disable();
        _Input.Character.Enable();

        DisableAllWindows();
        if (gamePlay_UI) gamePlay_UI.SetActive(true);
        GameManager.Instance.DisableCursor();

        playerController.EnableCharacterInput(isPlaying);
    }
    public override void Resume()
    {
        isPlaying = true;
        Time.timeScale = 1.0f;
        playerController.EnableCharacterInput(isPlaying);
        GameManager.Instance.DisableCursor();
        _Input.Disable();
        _Input.Character.Enable();
        DisableAllWindows();
        gamePlay_UI.SetActive(true);
    }
    public override void Pause()
    {
        isPlaying = false;
        Time.timeScale = 0;
        playerController.EnableCharacterInput(isPlaying);
        GameManager.Instance.EnableCursor();
        _Input.Disable();
        _Input.Menu.Enable();
        DisableAllWindows();
        pauseMenu_UI.SetActive(true);
    }
    public override void Stop()
    {
        isPlaying = false;
        Time.timeScale = 1.0f;

        _Input.Character.Escape.performed -= OnPause;
        _Input.Disable();

        playerController.EnableCharacterInput(isPlaying);
        GameManager.Instance.EnableCursor();

        DisableAllWindows();
    }
    public override void Won()
    {
        Stop();
        won_UI.SetActive(true);
    }
    public override void Fail()
    {
        Stop();
        fail_UI.SetActive(true);
    }

    public override void Update()
    {
        if (!isPlaying) return;

        countDownTime -= Time.deltaTime;
        UpdateTime(countDownTime);

    }

    private void OnTimeLineStop(PlayableDirector director)
    {
        _Input.TimeLine.Skip.performed -= SkipCinematics;
        if (director) director.stopped -= OnTimeLineStop;

        playerTransform.gameObject.SetActive(false);
        playerTransform.SetPositionAndRotation(playerStartPos.position, playerStartPos.rotation);
        playerTransform.gameObject.SetActive(true);

        Play();
    }
    private void UpdateTime(float reminingTime)
    {
        int seconds, minutes;

        reminingTime -= Time.deltaTime;
        minutes = (int)reminingTime / 60;
        seconds = (int)reminingTime % 60;

        this.time.SetText($"{minutes} : {seconds}");
        if (countDownTime <= 0 && isPlaying) Fail();
    }
    private void UpdateZombieCount()
    {
        zombieCount.text = string.Format($"Zombies : {zombies.Count} / {totalZombies}");
        if (zombies.Count == 0 && isPlaying) Invoke("Won", 0.5F);
    }
    private void OnZombieDie(AI_Enemy zombie)
    {
        zombies.Remove(zombie);
        UpdateZombieCount();
    }
    private void SkipCinematics(InputAction.CallbackContext context)
    {
        director.Stop();
    }
    private void OnPause(InputAction.CallbackContext context)
    {
        Pause();
    }
}
