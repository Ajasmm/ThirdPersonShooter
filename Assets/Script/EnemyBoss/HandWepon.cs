using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandWepon : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;

        other.gameObject.GetComponent<PlayerController>().AddDamage(100);
    }
}
