using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Mag : MonoBehaviour
{
    [SerializeField] public int bulletCount_Loaded { get; private set; }
    [SerializeField] int maxCapacity;
    [SerializeField] public BulletType bulletType; 

    private void OnEnable()
    {
        bulletCount_Loaded = maxCapacity;
    }

    public bool Shoot()
    {
        if (bulletCount_Loaded <= 0) return false;

        bulletCount_Loaded--;

        return true;
    }
    public int Reload(int totalAvailableBulletsInInventory)
    {
        int bulletsInMag = bulletCount_Loaded;
        int requiredBullets = maxCapacity - bulletsInMag;

        if (totalAvailableBulletsInInventory >= requiredBullets)
        {
            totalAvailableBulletsInInventory -= requiredBullets;
            bulletCount_Loaded = maxCapacity;
        }
        else
        {
            bulletCount_Loaded += totalAvailableBulletsInInventory;
            totalAvailableBulletsInInventory = 0;
        }

        return totalAvailableBulletsInInventory;
    }
}
