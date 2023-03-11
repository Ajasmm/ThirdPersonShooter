using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private Transform lookAtObject;
    [SerializeField] private Transform lookObject;
    [SerializeField] Animator animator;
    [SerializeField] PlayerController characterMovement;

    [SerializeField] private Gun[] weaponSlot;

    Gun activeGun = null;

    int currentSlot = 0;
    Dictionary<BulletType, int> bulletInventory;

    Transform myTransform, triggerTransform, holdTransform;
    float triggerWeight = 1, holdWeight = 1;

    MyInput input;

    GunUIData gunData;
    GunInfo[] gunInfos;

    Coroutine getGuninfoRotine;

    private void OnEnable()
    {
        myTransform = gameObject.GetComponent<Transform>();
        if (bulletInventory == null)
        {
            bulletInventory = new Dictionary<BulletType, int>();
            AddBullet(BulletType.Sniper, 10);
        }
        if(weaponSlot == null || weaponSlot.Length != 2) weaponSlot = new Gun[2];

        gunData = new GunUIData();
        getGuninfoRotine = StartCoroutine(GetGunInfosUI());

        input = GameManager.Instance.input;
        input.Character.Drop.performed += OnDrop;
    }
    private void OnDisable()
    {
        if(getGuninfoRotine != null) StopCoroutine(getGuninfoRotine);
        input.Character.Drop.performed -= OnDrop;
    }

    private void Update()
    {
        for (int i = 0; i < weaponSlot.Length; i++)
        {
            if (gunInfos == null || gunInfos[i] == null) continue;

            if (weaponSlot[i] == null)
            {
                gunData.isNull = true;
                if (gunInfos[i] != null) gunInfos[i].UpdateData(gunData);
            }
            else
            {
                gunData.isNull = false;
                gunData.gunName = weaponSlot[i].gunName;
                gunData.bulletType = weaponSlot[i].mag.bulletType;
                gunData.bulletInMag = weaponSlot[i].mag.bulletCount_Loaded;
                bulletInventory.TryGetValue(gunData.bulletType, out gunData.bulletInHand);

                if (gunInfos[i]) gunInfos[i].UpdateData(gunData);
            }

        }
    }


    public void AddGun(Gun gun)
    {
        DropGun(currentSlot);

        weaponSlot[currentSlot] = gun;
        EquipGun();

    }
    public void DropGun(int slot)
    {
        if (weaponSlot[slot])
        {
            UnEquipGun();
            weaponSlot[slot].Drop(myTransform.position + myTransform.forward + myTransform.up, myTransform.rotation);
            weaponSlot[slot].gameObject.SetActive(true);
            weaponSlot[slot] = null;
        }
    }
    public void DropGun()
    {
        if (weaponSlot[currentSlot])
        {
            UnEquipGun();
            weaponSlot[currentSlot].Drop(myTransform.position + myTransform.forward + myTransform.up, myTransform.rotation);
            weaponSlot[currentSlot].gameObject.SetActive(true);
            weaponSlot[currentSlot] = null;
        }
    }


    private void EquipGun()
    {
        if (weaponSlot[currentSlot])
        {
            weaponSlot[currentSlot].gameObject.SetActive(true);
            weaponSlot[currentSlot].Equip(myTransform, lookAtObject, lookObject, out triggerTransform, out triggerWeight, out holdTransform, out holdWeight);
            characterMovement.gun = weaponSlot[currentSlot];
            activeGun = weaponSlot[currentSlot];


        }
    }
    private void UnEquipGun()
    {
        if (weaponSlot[currentSlot])
        {
            weaponSlot[currentSlot].gameObject.SetActive(false);
            characterMovement.gun = null;
            activeGun = null;
            triggerTransform = null;
            holdTransform = null;
        }
    }

    public void SwitchWeapon(int slot)
    {
        if (currentSlot == slot)
        {
            if (activeGun) UnEquipGun();
            else EquipGun();
            return;
        }
        else
        {
            UnEquipGun();
            currentSlot = slot;
            EquipGun();
        }
    }
    public void FreeHand()
    {
        if (activeGun) SwitchWeapon(currentSlot);
    }
    public void UpdateBulletCount(BulletType bulletType, int newBulletCount)
    {
        if (bulletInventory == null) bulletInventory = new Dictionary<BulletType, int>();

        if (!bulletInventory.ContainsKey(bulletType)) bulletInventory.Add(bulletType, newBulletCount);
        else bulletInventory[bulletType] = newBulletCount;
    }
    public void AddBullet(BulletType bulletType, int bulletCount)
    {
        if (bulletInventory == null) bulletInventory = new Dictionary<BulletType, int>();

        if(!bulletInventory.ContainsKey(bulletType)) bulletInventory.Add(bulletType, bulletCount);
        else
        {
            int currentBulletCount = bulletInventory[bulletType];
            currentBulletCount += bulletCount;
            bulletInventory[bulletType] = currentBulletCount;
        }
    }
    public int GetBulletCount(BulletType bulletType)
    {
        if (bulletInventory.ContainsKey(bulletType)) return bulletInventory[bulletType];
        else return 0;
    }
    public void ResetWeaponSlot()
    {
        weaponSlot = new Gun[2];
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (activeGun == null) return;


        if (activeGun)
        {
            activeGun.LookAtTarget();
            animator.SetLookAtWeight(1);
            animator.SetLookAtPosition(lookAtObject.position);
        }

        if (triggerTransform)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
            animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 1);
            animator.SetIKHintPosition(AvatarIKHint.RightElbow, myTransform.position + myTransform.right + myTransform.up);
            animator.SetIKPosition(AvatarIKGoal.RightHand, triggerTransform.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, triggerTransform.rotation);
        }

        if (holdTransform)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
            animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 1);
            animator.SetIKHintPosition(AvatarIKHint.LeftElbow, myTransform.position + -myTransform.right + myTransform.up);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, holdTransform.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, holdTransform.rotation);
        }
    }
    private void OnDrop(InputAction.CallbackContext context)
    {
        DropGun(currentSlot);
    }
    private IEnumerator GetGunInfosUI()
    {
        GamePlayMode gamePlayMode;
        gunInfos = null;

        while(gunInfos == null)
        {
            gamePlayMode = GameManager.Instance.CurrentGamePlayMode;
            gunInfos = gamePlayMode?.GetData<GunInfo[]>("GunInfos");
            yield return null;
        }
    }
}