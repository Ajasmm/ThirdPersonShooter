using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandWepon : MonoBehaviour
{
    [SerializeField] float activeTime = 1.5F;

    private void OnEnable()
    {
        Invoke("DisableObject", activeTime);
    }

   
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;

        other.gameObject.GetComponent<PlayerController>().AddDamage(100);
    }

    private void DisableObject()
    {
        gameObject.SetActive(false);
    }
}
