using UnityEngine;
using System.Threading.Tasks;
using System;
using System.Threading;

[Serializable]
public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    private static bool isApllicationQuit = false;
    private GamePlayMode currentGamePlayMode;

    CancellationTokenSource cancellationTokenSource;

   [SerializeField] public GamePlayMode CurrentGamePlayMode
    {
        get { return currentGamePlayMode; }
        set { RegisterGamePlayMode(value); }
    }
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                if (isApllicationQuit) return null;

                GameObject gameManagerObj = new GameObject("GameManager");
                gameManagerObj.AddComponent<GameManager>();
            }
            return instance;
        }
        private set { instance = value; }
    }
    public GameObject player { get; private set; }
    public MyInput input{private set; get; }


    private void OnEnable()
    {
        // Singleton
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        // creating new Input
        if (input == null) input = new MyInput();
    }

    public void RegisterPlayer(GameObject player) { 
        if(!this.player) Destroy(this.player);
        this.player = player;
        DontDestroyOnLoad(player);
    }
    public void RegisterGamePlayMode(GamePlayMode gamePlayMode)
    {
        currentGamePlayMode?.Stop();
        currentGamePlayMode = gamePlayMode;

        gamePlayMode.Initialize();
    }
    
    public void EnableCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void DisableCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public Task WaitForPlayer()
    {
        if (cancellationTokenSource == null)
        {
            cancellationTokenSource = new CancellationTokenSource();
        }
        CancellationToken cancellationToken = cancellationTokenSource.Token;
        Task returnTask = Task.Run(() =>
        {
            while (player == null)
            {
                Debug.Log("Player is not registered");
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
        }, cancellationTokenSource.Token);
        return returnTask;
    }

    private void OnDestroy()
    {
        cancellationTokenSource?.Cancel();
        currentGamePlayMode?.Stop();
        EnableCursor();
        if (instance == this) isApllicationQuit = true;
    }
}
