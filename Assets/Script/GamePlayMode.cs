using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public abstract class GamePlayMode : MonoBehaviour
{
    [Header("Player Position")]
    [SerializeField] protected Transform playerPos_IntroTimeline;
    [SerializeField] protected Transform playerStartPos;
    [SerializeField] protected Transform playerPos_WinTimeline;

    [Header("UI")]
    [SerializeField] protected GameObject gamePlay_UI;
    [SerializeField] protected GameObject pauseMenu_UI;
    [SerializeField] protected GameObject won_UI;
    [SerializeField] protected GameObject fail_UI;

    protected GameObject player;
    Dictionary<string, object> data = new Dictionary<string, object>();

    CancellationTokenSource cancellationTokenSource;

    public bool isPlaying { protected set; get; } = false;
    public abstract void Initialize();
    public abstract void Update();
    public abstract void Play();
    public abstract void Pause();
    public abstract void Resume();
    public abstract void Stop();
    public abstract void Won();
    public abstract void Fail();

    public T GetData<T>(string key)
    {
        object pullObject;
        if (!data.TryGetValue(key, out pullObject) || pullObject == null)
        {
            Debug.Log($"Error with key {key}");
            return default(T);
        }
        return (T)pullObject;
    }
    public void AddData(string key, object value) { data.Add(key, value); }

    protected void DisableAllWindows()
    {
        if (gamePlay_UI) gamePlay_UI.SetActive(false);
        if (pauseMenu_UI) pauseMenu_UI.SetActive(false);
        if (won_UI) won_UI.SetActive(false);
        if (fail_UI) fail_UI.SetActive(false);
    }
    protected Task WaitForPlayer()
    {
        if(cancellationTokenSource == null) cancellationTokenSource = new CancellationTokenSource();

        Task task = Task.Run(() =>
        {
            while (GameManager.Instance.player == null)
            {
                if (cancellationTokenSource.IsCancellationRequested) { return; }
                Debug.Log("Player not registered");
            }
            player = GameManager.Instance.player;
        }, cancellationTokenSource.Token);

        return task;
    }
    protected virtual void OnDestroy()
    {
        cancellationTokenSource?.Cancel();
    }
}
