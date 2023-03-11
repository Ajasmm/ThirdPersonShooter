using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float lifeTIme;

    float tempTime;

    Queue<GameObject> pool;
    
    public void SetPool(Queue<GameObject> pool)
    {
        this.pool = pool;
        pool.Enqueue(this.gameObject);
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        tempTime = 0;
    }

    private void Update()
    {
        tempTime += Time.deltaTime;
        if(tempTime> lifeTIme)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        pool.Enqueue(this.gameObject);
    }
}
