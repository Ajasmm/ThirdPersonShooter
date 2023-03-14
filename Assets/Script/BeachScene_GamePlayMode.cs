using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using System;
using UnityEngine.Playables;
using System.Linq;
using Cinemachine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem;

public class BeachScene_GamePlayMode : GamePlayMode
{
    [Header("Mission")]
    [SerializeField] List<AI_Enemy> zombies;
    [SerializeField] float timeToFinish = 20F;
    [SerializeField] TMP_Text zombieCount;
    [SerializeField] TMP_Text time;

    [Header("UI")]
    [SerializeField] GunInfo[] gunInfos;

    [Header("Timeline")]
    [SerializeField] PlayableDirector director;
    [SerializeField] CinemachineVirtualCamera virtualCamera;

    MyInput _Input;
    float tempTime;
    int totalZombies;

    PlayerController playerController;

    public override void Initialize()
    {
        GetPlayer();

        AddData("GunInfos", gunInfos);
        DisableAllWindows();
        totalZombies = zombies.Count;
        UpdateZombieCount();
        foreach (AI_Enemy zombie in zombies) zombie.OnDestroy += OnZombieDie;

        _Input = GameManager.Instance.input;
        _Input.Disable();
        _Input.Character.Escape.performed += OnPause;

        tempTime = timeToFinish;

        PlayTimeLine();
    }

    

    public override void Update()
    {
        if (!isPlaying) return;

        UpdateTime(tempTime);
    }
    private async void PlayTimeLine()
    {
        GameManager.Instance.DisableCursor();
        _Input.TimeLine.Skip.performed += SkipTimeLine;

        Task waitForPlayer = GameManager.Instance.WaitForPlayer();
        await waitForPlayer;

        if (director)
        {
            Animator playerAnimator = player.GetComponent<Animator>();
            playerAnimator.applyRootMotion = true;

            foreach (var bindings in director.playableAsset.outputs)
            {
                if (bindings.streamName == "Player Animation Track")
                {
                    director.SetGenericBinding(bindings.sourceObject, playerAnimator);
                    break;
                }
            }
            virtualCamera.m_LookAt = player.transform;

            _Input.Disable();
            _Input.TimeLine.Enable();

            playerController.EnableCharacterInput(false);

            director.stopped += OnTimeLineStop;
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
    public override void Pause()
    {
        Time.timeScale = 0;
        isPlaying = false;

        DisableAllWindows();
        if(pauseMenu_UI) pauseMenu_UI.SetActive(true);
        GameManager.Instance.EnableCursor();

        _Input.Disable();
        _Input.Menu.Enable();
        playerController.EnableCharacterInput(isPlaying);
    }
    public override void Resume()
    {
        Time.timeScale = 1;
        isPlaying = true;

        DisableAllWindows();
        if(gamePlay_UI) gamePlay_UI.SetActive(true);
        GameManager.Instance.DisableCursor();

        _Input.Disable();
        _Input.Character.Enable();
        playerController.EnableCharacterInput(isPlaying);
    }
    public override void Stop()
    {
        isPlaying = false;
        Time.timeScale = 1;

        DisableAllWindows();
        playerController.EnableCharacterInput(isPlaying);

        _Input.Character.Escape.performed -= OnPause;
        _Input.Disable();

        foreach (AI_Enemy zombie in zombies) zombie.OnDestroy -= OnZombieDie;
    }
    public override void Won()
    {
        Stop();
        if(won_UI) won_UI.SetActive(true);
    }
    public override void Fail()
    {
        Stop();
        if(fail_UI) fail_UI.SetActive(true);
    }

    [Tooltip("time in seconds")]
    private void UpdateTime(float time)
    {
        int seconds, minutes;

        tempTime -= Time.deltaTime;
        minutes = (int) time / 60;
        seconds = (int) time % 60;

        this.time.SetText($"{minutes} : {seconds}");
        if (tempTime <= 0 && isPlaying) Fail();
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
    private async void GetPlayer()
    {
        Task waitForPlayer = GameManager.Instance.WaitForPlayer();
        await waitForPlayer;

        player = GameManager.Instance.player;
        player.SetActive(false);
        Transform playerTransform = player.GetComponent<Transform>();
        playerTransform.position = playerStartPos_Cinematics.position;
        playerTransform.rotation = playerStartPos_Cinematics.rotation;
        player.SetActive(true);

        playerController = player.GetComponent<PlayerController>();
        playerController.ResetHealth();
        playerController.DisableRagdoll();
        playerController.ResetGunAndInventory();
        playerController.EnableCharacterInput(false);
    }
    private void OnTimeLineStop(PlayableDirector director)
    {
        player.SetActive(false);
        Transform playerTransform = player.GetComponent<Transform>();
        playerTransform.position = playerStartPos.position;
        playerTransform.rotation = playerStartPos.rotation;
        player.SetActive(true);

        _Input.TimeLine.Skip.performed -= SkipTimeLine;
        if (director) director.stopped -= OnTimeLineStop;

        Play();
    }
    private void SkipTimeLine(InputAction.CallbackContext context)
    {
        director.Stop();
    }
    private void OnPause(InputAction.CallbackContext context)
    {
        Pause();
    }
}
