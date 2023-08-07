using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickSystem : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;
    [SerializeField] float distance = 10;
    [SerializeField] PlayerController characterMovement;
    [SerializeField] WeaponManager weaponManager;
    [SerializeField] Animator animator;
    [SerializeField] Transform lookAtObject;

    Gun gun;

    Transform myTransform;

    Vector3 screenPos = new Vector3(0.5F, 0.5F, 0F);
    Ray ray;
    RaycastHit hitObject;
    
    PickableItem selectedItem, tempSelectedItem;
    MyInput inputSystem;

    Vector3 screenCenter;

    private void OnEnable()
    {
        inputSystem = GameManager.Instance.input;

        inputSystem.Character.Action.performed += OnAction;

        // If no Animator or Character controller is attached then Disable this behaviour
        if (animator == null) animator = gameObject.GetComponent<Animator>();
        if (animator == null) { this.enabled = false; }

        if (characterMovement == null) characterMovement = gameObject.GetComponent<PlayerController>();
        if (characterMovement == null) {this.enabled = false; }
    }
    private void OnDisable()
    {
        inputSystem.Character.Action.performed -= OnAction;
    }


    private void Start()
    {
        screenCenter.x = Screen.currentResolution.width / 2;
        screenCenter.y = Screen.currentResolution.height / 2;

        myTransform = transform;
    }

    private void Update()
    {
        ray = Camera.main.ScreenPointToRay(screenCenter);

        if(Physics.Raycast(ray,out hitObject, distance, layerMask))
        {
            lookAtObject.position = hitObject.point;

            if (hitObject.distance < 5)
                tempSelectedItem = hitObject.transform.gameObject.GetComponent<PickableItem>();
            else
                tempSelectedItem = null;

            if (tempSelectedItem == null)
            {
                if (selectedItem != null) selectedItem.DisaplayDetails(false);
                selectedItem = null;
                return;
            }
            selectedItem = tempSelectedItem;
            selectedItem.DisaplayDetails(true);

        }
        else
        {
            if (selectedItem != null) selectedItem.DisaplayDetails(false);
            selectedItem = null;

            lookAtObject.position = ray.GetPoint(5);
        }
    }

    public void OnAction(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.phase != InputActionPhase.Performed) return;
        if (selectedItem == null) return;

        PickableItemType pickedItemType = selectedItem.Pick();

        switch (pickedItemType)
        {
            case PickableItemType.Gun:
                PickGun();
                break;
            case PickableItemType.Bullet:
                PickBullet();
                break;
        }

    }

    public void PickGun()
    {
        Gun gun = selectedItem.gameObject.GetComponent<Gun>();
        if(gun == null)
        {
            return;
        }
        weaponManager.AddGun(gun);
    }

    public void PickBullet()
    {
        
    }

}
