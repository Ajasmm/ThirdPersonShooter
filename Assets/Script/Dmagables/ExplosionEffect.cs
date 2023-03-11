using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    [SerializeField] ParticleSystem _particleSystem;

    private void Start()
    {
        if(!_particleSystem) _particleSystem = gameObject.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule maiModule = _particleSystem.main;
        maiModule.stopAction = ParticleSystemStopAction.Callback;
    }

    private void OnParticleSystemStopped()
    {
        Destroy(gameObject);
    }
}
