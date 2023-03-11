using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    public void AddDamage(float damage, Vector3 force);
    public void AddDamage(float damage);
    public abstract void Destroy();
    public abstract void DecreaseHealth(float damage);
}
