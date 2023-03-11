using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class GamePlayMode : MonoBehaviour
{
    [SerializeField] protected Transform playerStartPos_Cinematics;
    [SerializeField] protected Transform playerStartPos;

    protected GameObject player;

    Dictionary<string, object> data = new Dictionary<string, object>();

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
    
}
