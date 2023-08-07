using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

public class Gun : PickableItem
{
    [Header("Animation Parameters")]
    [SerializeField] protected Animator animator;
    [SerializeField] float transitionSpeed = 4;
    [SerializeField] Transform triggerTransform;
    [SerializeField] float triggerWeight = 1;
    [SerializeField] Transform holdTransform;
    [SerializeField] float holdWeight = 1;


    [Header("Gun Parameters")]
    [SerializeField] private float mass = 20;
    [SerializeField] public string gunName = "AK-47";
    [SerializeField] public Mag mag;
    [SerializeField] private float damagePerBulletShot = 50;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform tipTransform;
    [SerializeField] private int playerLayer;

    [Header("Scope Settings")]
    [SerializeField] private CinemachineVirtualCamera scopeCamera;
    [SerializeField] private GameObject[] scopeGameObjects;

    private Transform lookAtObject, lookObject;

    [Header("Sound")]
    [SerializeField] private AudioSource shootingSound_withBullets;


    int fireHash, sidewaysHash;

    float aimPosition, sideWaysPosition, animAimPosition, animSideWaysPosition;
    bool scopeState = false;


    Transform myTransform;

    Queue<GameObject> bulletPool;
    Ray ray;
    RaycastHit raycastHit;
    int defaultLayer;

    private void OnEnable()
    {
        DisaplayDetails(false);
        animator.enabled = false;
        
}
    private void Start()
    {
        fireHash = Animator.StringToHash("Fire");
        sidewaysHash = Animator.StringToHash("SideWays");
        rBody.mass = mass;

        myTransform = transform;

        bulletPool= new Queue<GameObject>();
        for (int i = 1; i < 20; i++)
        {
            GameObject tempObject = Instantiate(bulletPrefab, this.transform);
            tempObject.GetComponent<Bullet>().SetPool(bulletPool);
        }

        defaultLayer = gameObject.layer;
    }

    private void Update()
    {
        animAimPosition = AnimationUtils.Increase(animAimPosition, aimPosition, transitionSpeed * Time.deltaTime);
        animSideWaysPosition = AnimationUtils.Increase(animSideWaysPosition, sideWaysPosition, transitionSpeed * Time.deltaTime);
        animSideWaysPosition = Mathf.Clamp(animSideWaysPosition, -1, 0);
        animator.SetFloat(fireHash, animAimPosition);
        animator.SetFloat(sidewaysHash, animSideWaysPosition);

        if(aimPosition == 0 && scopeState)
        {
            scopeState = false;
            Scope(scopeState);
        }
        else if(aimPosition != 0 && !scopeState)
        {
            scopeState = true;
            Scope(scopeState);
        }
    }


    // Pickable Item
    public override PickableItemType Pick()
    {
        itemCollider.enabled = false;
        Destroy(rBody);
        return itemType;
    }
    public override void Drop(Vector3 positionToDrop, Quaternion rotationWhileDropping)
    {
        lookAtObject = null;

        aimPosition = 0F;
        animAimPosition = 0F;
        animator.SetFloat(fireHash, animAimPosition);
        animator.enabled = false;

        transform.parent = null;
        transform.SetPositionAndRotation(positionToDrop, rotationWhileDropping);

        itemCollider.enabled = true;
        rBody = gameObject.AddComponent<Rigidbody>();
        rBody.mass = mass;

        gameObject.layer = defaultLayer;
    }
    public void Equip(Transform parent, Transform lookAtObject, Transform lookObject, out Transform triggerTransform, out float triggerWeight, out Transform holdTransform, out float holdWeight)
    {
        this.lookAtObject = lookAtObject;
        this.lookObject = lookObject;

        animator.enabled = true;
        aimPosition = 0F;
        animAimPosition = 0F;
        scopeState = false;

        transform.parent = parent;

        triggerTransform = this.triggerTransform; 
        triggerWeight = this.triggerWeight; 
        holdTransform = this.holdTransform; 
        holdWeight= this.holdWeight;

        gameObject.layer = playerLayer;
    }


    public void LookAtTarget()
    {
        if (!scopeState && aimPosition != 0) myTransform.LookAt(lookAtObject);
        else if(scopeState) myTransform.rotation = lookObject.rotation;
    }

    public void Shoot(AnimationEvent shootingEvent)
    {
        if (shootingEvent.animatorClipInfo.weight < 0.9F) return;

        if (mag == null) return;
        if (mag.Shoot())
        {
            ray.origin = tipTransform.position;
            ray.direction = tipTransform.forward;


            GameObject bulletEffet = bulletPool.Dequeue();
            bulletEffet.SetActive(true);

            if (Physics.Raycast(ray, out raycastHit, 500))
            {
                bulletEffet.transform.position = raycastHit.point;
                AddDamage(raycastHit.collider);
            }
            else bulletEffet.SetActive(false);

            shootingSound_withBullets.Stop();
            shootingSound_withBullets.Play();
        }
        else
        {
            aimPosition = 0.5F;
        }
    }
    public void AddDamage(Collider hittedCollider)
    {
        IDamagable damagableObject = null;

        try
        {
            damagableObject = hittedCollider.GetComponent<IDamagable>();

        }
        catch
        {
            // Debug.LogError("Error while getting damagable");
            return;
        }
        if (damagableObject == null) return;

        damagableObject.AddDamage(damagePerBulletShot, myTransform.forward);
    }

    private void Scope(bool scopeState)
    {
        for (int i = 0; i < scopeGameObjects.Length; i++) scopeGameObjects[i].SetActive(!scopeState);

        if (scopeState) scopeCamera.Priority = 20;
        else scopeCamera.Priority = 0;
    }

    public int Reload(int bulletInHand)
    {
        int remainigBullets = mag.Reload(bulletInHand);
        return remainigBullets;
    }
    public void Idle() => aimPosition = 0F;
    public void Aim() => aimPosition = 0.5F;
    public void Fire()
    {
        if (mag.bulletCount_Loaded == 0) aimPosition = 0.5F;
        else aimPosition = 1F;
    }

    public void SideWays(float sidewayPosition) => this.sideWaysPosition = sidewayPosition;

    private void OnDisable()
    {
        scopeState = false;
        Scope(false);
    }
}
