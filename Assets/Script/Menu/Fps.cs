using UnityEngine;
using TMPro;

public class Fps : MonoBehaviour
{
    [Tooltip("in seconds")]
    [SerializeField] float samplingInterval;

    float tempInterval;

    TMP_Text time;

    private void Start()
    {
        time = GetComponent<TMP_Text>();
    }
    private void Update()
    {
        tempInterval += Time.deltaTime;
        if (tempInterval > samplingInterval)
        {
            time.text = (Time.deltaTime * 1000).ToString("N2");
            tempInterval = 0;
        }
    }
}
