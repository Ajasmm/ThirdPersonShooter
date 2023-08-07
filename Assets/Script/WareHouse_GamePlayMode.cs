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
    [Header("TimeLine")]
    [SerializeField] PlayableDirector director;
    [SerializeField] PlayableAsset introTimeLineAsset;
    [SerializeField] PlayableAsset winTimeLineAsset;

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

        PlayIntroTimeLine();
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
    }

    private async void PlayIntroTimeLine()
    {
        GameManager.Instance.DisableCursor();

        Task waitForPlayer = WaitForPlayer();
        await waitForPlayer;

        playerTransform.gameObject.SetActive(false);
        playerTransform.SetPositionAndRotation(playerPos_IntroTimeline.position, playerPos_IntroTimeline.rotation);
        playerTransform.gameObject.SetActive(true);

        _Input.TimeLine.Skip.performed += SkipCinematics;
        if (director != null)
        {
            director.playableAsset = introTimeLineAsset;
            _Input.Disable();
            _Input.TimeLine.Enable();

            director.stopped += OnIntroTimeLineStop;
            Time.timeScale = 1;
            playerController.EnableCharacterInput(false);

            director.Play();
        }
        else OnIntroTimeLineStop(null);
    }
    private async void PlayWinTimeLine()
    {
        GameManager.Instance.DisableCursor();

        Task waitForPlayer = WaitForPlayer();
        await waitForPlayer;

        _Input.TimeLine.Skip.performed += SkipCinematics;
        if (director != null)
        {
            director.playableAsset = winTimeLineAsset;
            _Input.Disable();
            _Input.TimeLine.Enable();

            director.stopped += OnWinTimeLineStop;
            Time.timeScale = 1;
            playerController.EnableCharacterInput(false);

            player.SetActive(false);
            playerTransform.SetPositionAndRotation(playerPos_WinTimeline.position, playerPos_WinTimeline.rotation);
            player.SetActive(true);

            foreach(var binding in director.playableAsset.outputs)
            {
                if(binding.streamName == "Player Animation Track")
                {
                    director.SetGenericBinding(binding.sourceObject, player.GetComponent<Animator>());
                    break;
                }
            }

            director.Play();
        }
        else OnIntroTimeLineStop(null);
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
        PlayWinTimeLine();
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

    private void OnIntroTimeLineStop(PlayableDirector director)
    {
        // Debug.Log("Intro timeLine finfished");
        _Input.TimeLine.Skip.performed -= SkipCinematics;
        if (director) director.stopped -= OnIntroTimeLineStop;

        playerTransform.gameObject.SetActive(false);
        playerTransform.SetPositionAndRotation(playerStartPos.position, playerStartPos.rotation);
        playerTransform.gameObject.SetActive(true);

        Play();
    }
    private void OnWinTimeLineStop(PlayableDirector director)
    {
        _Input.TimeLine.Skip.performed -= SkipCinematics;
        if (director) director.stopped -= OnIntroTimeLineStop;

        _Input.Disable();
        _Input.Menu.Enable();

        if (won_UI != null) won_UI.SetActive(true);
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
