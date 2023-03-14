using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Menu_GamePlayMode : GamePlayMode
{
    [SerializeField] GameObject mainMenu_UI;
    [SerializeField] MyInput _Input;

    public override void Initialize()
    {
        GetPlayer();
        _Input = GameManager.Instance.input;
        PlayAsync();
    }
    private async void PlayAsync()
    {
        Task waitForPlayer = GameManager.Instance.WaitForPlayer();
        await waitForPlayer;

        player = GameManager.Instance.player;
        Play();
    }
    public override void Play()
    {
        GameManager.Instance.EnableCursor();

        player.SetActive(false);
        player.transform.SetPositionAndRotation(playerStartPos.position, playerStartPos.rotation);
        player.SetActive(true);

        _Input.Disable();
        _Input.Menu.Enable();

    }   
    public override void Pause()
    {
        
    }
    public override void Resume()
    {
        
    }
    public override void Stop()
    {

        GameManager.Instance.DisableCursor();
        player?.SetActive(false);
        if(mainMenu_UI!=null) mainMenu_UI?.SetActive(false);
    }
    public override void Won()
    {
        Stop();
    }
    public override void Fail()
    {
        Stop();
    }

    public override void Update()
    {

    }

    private async void GetPlayer()
    {
        Task waitForPlayer = GameManager.Instance.WaitForPlayer();
        await waitForPlayer;

        player = GameManager.Instance.player;
        PlayerController playerController = player.GetComponent<PlayerController>();
        playerController.ResetHealth();
        playerController.DisableRagdoll();
        playerController.ResetGunAndInventory();
        playerController.EnableCharacterInput(false);
    }
}
