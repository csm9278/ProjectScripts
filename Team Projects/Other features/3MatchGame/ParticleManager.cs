using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MemoryPoolObject
{
    ParticleSystem _particleSystem;
    float duration;

    WaitForSeconds waitSc;

    public override void InitObject()
    {
        _particleSystem = GetComponentInChildren<ParticleSystem>();
        duration = _particleSystem.main.duration;
        waitSc = new WaitForSeconds(duration);
    }

    public IEnumerator PlayParticleCo(Vector2 playpoint)
    {
        this.transform.position = playpoint;
        _particleSystem.Play();

        yield return waitSc;

        ObjectReturn();
    }
}
