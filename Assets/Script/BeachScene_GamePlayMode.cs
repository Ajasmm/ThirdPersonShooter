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
    [SerializeField] GameObject gamePlay_UI;
    [SerializeField] GameObject pauseMenu_UI;
    [SerializeField] GameObject Won_UI;
    [SerializeField] GameObject Fail_UI;
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
        AddData("GunInfos", gunInfos);
        totalZombies = zombies.Count;
        foreach (AI_Enemy zombie in zombies) zombie.OnDestroy += OnZombieDie;
        UpdateZombieCount();

        GetPlayer();

        _Input = GameManager.Instance.input;
        _Input.Character.Disable();
        _Input.Menu.Disable();
        _Input.Character.Escape.performed += OnPause;
        _Input.TimeLine.Skip.performed += SkipCinematics;

        if(gamePlay_UI) gamePlay_UI.SetActive(false);
        if(pauseMenu_UI) pauseMenu_UI.SetActive(false);
        if (Won_UI) Won_UI.SetActive(false);
        if(Fail_UI) Fail_UI.SetActive(false);

        tempTime = timeToFinish;

        PlayAsync();
    }

    

    public override void Update()
    {
        if (!isPlaying) return;

        UpdateTime(tempTime);
    }
    private async void PlayAsync()
    {
        Task waitForPlayer = GameManager.Instance.WaitForPlayer();
        await waitForPlayer;

        Play();
    }
    public override void Play()
    {
        Time.timeScale = 1;
        isPlaying = true;
        DisableAllWindow();
        GameManager.Instance.DisableCursor();
        PlayTimeline();
    }
    public override void Pause()
    {
        Time.timeScale = 0;
        isPlaying = false;

        DisableAllWindow();
        if(pauseMenu_UI) pauseMenu_UI.SetActive(true);
        GameManager.Instance.EnableCursor();

        _Input.Menu.Enable();
        _Input.Character.Disable();
        playerController.EnableCharacterInput(isPlaying);
    }
    public override void Resume()
    {
        Time.timeScale = 1;
        isPlaying = true;

        DisableAllWindow();
        if(gamePlay_UI) gamePlay_UI.SetActive(true);
        GameManager.Instance.DisableCursor();

        _Input.Character.Enable();
        _Input.Menu.Disable();
        playerController.EnableCharacterInput(isPlaying);
    }
    public override void Stop()
    {
        isPlaying = false;

        DisableAllWindow();
        playerController.EnableCharacterInput(isPlaying);
        GameManager.Instance.EnableCursor();

        _Input.TimeLine.Skip.performed -= SkipCinematics;
        _Input.Character.Escape.performed -= OnPause;

        _Input.Character.Disable();
        _Input.Menu.Enable();
        foreach (AI_Enemy zombie in zombies) zombie.OnDestroy -= OnZombieDie;
        isPlaying = false;
    }
    public override void Won()
    {
        Stop();
        if(Won_UI) Won_UI.SetActive(true);
    }
    public override void Fail()
    {
        Stop();
        if(Fail_UI) Fail_UI.SetActive(true);
    }

    [Tooltip("time in seconds")]
    private void UpdateTime(float time)
    {
        int seconds, minutes;

        tempTime -= Time.deltaTime;
        minutes = (int) time / 60;
        seconds = (int) time % 60;

        this.time.SetText($"{minutes} : {seconds}");
        if (tempTime <= 0) Fail();
    }
    private void UpdateZombieCount()
    {
        zombieCount.text = string.Format($"Zombies : {zombies.Count} / {totalZombies}");
        if (zombies.Count == 0) Invoke("Won", 0.5F);
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
        playerController.GetComponent<WeaponManager>().ResetWeaponSlot();
    }
    private void PlayTimeline()
    {
        if (director != null)
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
            _Input.TimeLine.Enable();
            playerController.EnableCharacterInput(false);
            director.stopped += OnDirectorStop;
            director.Play();
        }
        else OnDirectorStop(null);
    }
    private void OnDirectorStop(PlayableDirector director)
    {
        player.SetActive(false);
        Transform playerTransform = player.GetComponent<Transform>();
        playerTransform.position = playerStartPos.position;
        playerTransform.rotation = playerStartPos.rotation;
        player.SetActive(true);

        _Input.TimeLine.Disable();
        _Input.Character.Enable();
        if(director) playerController.EnableCharacterInput(true);

        DisableAllWindow();
        if (gamePlay_UI) gamePlay_UI.SetActive(true);

        _Input.TimeLine.Skip.performed -= SkipCinematics;
    }
    private void SkipCinematics(InputAction.CallbackContext context)
    {
        director.Stop();
    }
    private void OnPause(InputAction.CallbackContext context)
    {
        Pause();
    }
    private void DisableAllWindow()
    {
        if (gamePlay_UI) gamePlay_UI.SetActive(false);
        if (pauseMenu_UI) pauseMenu_UI.SetActive(false);
        if(Won_UI) Won_UI.SetActive(false);
        if(Fail_UI) Fail_UI.SetActive(false);
    }
}
