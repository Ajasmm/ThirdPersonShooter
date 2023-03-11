using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GunInfo : MonoBehaviour
{
    [SerializeField] TMP_Text gunName;
    [SerializeField] TMP_Text bulletCount;
    public void UpdateData(GunUIData data)
    {
        if (data.isNull)
        {
            gunName.text = "";
            bulletCount.text = "";
            return;
        }

        gunName.text = data.bulletType.ToString() + " : " + data.gunName;
        bulletCount.text = data.bulletInMag + " / " + data.bulletInHand;
    }
}
