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
    [SerializeField] PlayableAsset introTimeline;
    [SerializeField] PlayableAsset winTimeline;

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

        PlayIntroTimeline();
    }

    

    public override void Update()
    {
        if (!isPlaying) return;

        UpdateTime(tempTime);
    }
    private async void PlayIntroTimeline()
    {
        GameManager.Instance.DisableCursor();
        _Input.TimeLine.Skip.performed += SkipTimeLine;

        Task waitForPlayer = WaitForPlayer();
        await waitForPlayer;

        player.SetActive(false);
        Transform playerTransform = player.GetComponent<Transform>();
        playerTransform.position = playerPos_IntroTimeline.position;
        playerTransform.rotation = playerPos_IntroTimeline.rotation;
        player.SetActive(true);

        if (director)
        {
            director.playableAsset = introTimeline;
            director.stopped += OnIntroTimelineStop;
            Animator playerAnimator = player.GetComponent<Animator>();
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

            director.Play();
        }
        else OnIntroTimelineStop(null);
    }
    private async void PlayWinTimeline()
    {
        GameManager.Instance.DisableCursor();
        _Input.TimeLine.Skip.performed += SkipTimeLine;

        Task waitForPlayer = WaitForPlayer();
        await waitForPlayer; 
        
        player.SetActive(false);
        Transform playerTransform = player.GetComponent<Transform>();
        playerTransform.position = playerPos_WinTimeline.position;
        playerTransform.rotation = playerPos_WinTimeline.rotation;
        player.SetActive(true);

        if (director)
        {

            director.playableAsset = winTimeline;
            director.stopped += OnWinTimelineStop;
            Animator playerAnimator = player.GetComponent<Animator>();
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

            Time.timeScale = 1;
            playerController.EnableCharacterInput(false);

            director.Play();
        }
        else OnWinTimelineStop(null);
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
        PlayWinTimeline();
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
        Task waitForPlayer = WaitForPlayer();
        await waitForPlayer;

        playerController = player.GetComponent<PlayerController>();
        playerController.ResetHealth();
        playerController.DisableRagdoll();
        playerController.ResetGunAndInventory();
        playerController.EnableCharacterInput(false);
    }
    private void OnIntroTimelineStop(PlayableDirector director)
    {
        player.SetActive(false);
        Transform playerTransform = player.GetComponent<Transform>();
        playerTransform.position = playerStartPos.position;
        playerTransform.rotation = playerStartPos.rotation;
        player.SetActive(true);

        _Input.TimeLine.Skip.performed -= SkipTimeLine;
        if (director) director.stopped -= OnIntroTimelineStop;

        Play();
    }
    private void OnWinTimelineStop(PlayableDirector director)
    {
        _Input.TimeLine.Skip.performed -= SkipTimeLine;
        if (director) director.stopped -= OnIntroTimelineStop;

        _Input.Disable();
        _Input.Menu.Enable();

        if (won_UI != null) won_UI.SetActive(true); 
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
