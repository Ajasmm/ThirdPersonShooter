using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IDamagable
{
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private WeaponManager weaponManager;
    [SerializeField] public Transform cameraFollowTartget;

    [Header("Animation Parameters")]
    [SerializeField] private float transitionSpeed = 4;

    [Header("Mouse Sencitivity")]
    [SerializeField] private float normalMouseSencitivity = 0.2F;
    [SerializeField] private float scopeMouseSencitivity = 0.1F;
    float mouseSencitivity = 0.2F;

    [Header("Wepone")]
    [SerializeField] public Gun gun;

    [Header("Camera")]
    [SerializeField] CinemachineVirtualCamera normalCam;
    [SerializeField] CinemachineVirtualCamera aimCam;
    [SerializeField] CinemachineVirtualCamera deadCam;

    [Header("Health")]
    [SerializeField] private float Health;
    public float health;
    
    MyInput inputSystem;

    Rigidbody[] ragdollBodies;

    // Input
    Vector2 inputMovement;
    Vector2 inputLook;
    bool run, jump, aim, fire;


    // Aniamtion
    Vector2 animationMovement;
    int isPlayingHash, forwardHash, sidewaysHash, jumpHash;


    // Character
    CharacterState characterState = CharacterState.Idle;


    Transform myTransform;

    // Temp Variables
    float lookRotationX = 0;
    Vector3 lookRotation;
    float lookRotationY = 0;

    private void OnEnable()
    {
        // Accessing the Input
        inputSystem = GameManager.Instance.input;

        // Input event registering
        inputSystem.Character.Run.performed += OnRun;
        inputSystem.Character.Run.canceled += OnRun;

        inputSystem.Character.Jump.performed += OnJump;
        inputSystem.Character.Jump.canceled += OnJump;

        inputSystem.Character.Aim.performed += OnAim;
        inputSystem.Character.Aim.canceled += OnAim;

        inputSystem.Character.Fire.performed += OnFire;
        inputSystem.Character.Fire.canceled += OnFire;

        inputSystem.Character.Weapon_1.performed += OnWeapon_1;
        inputSystem.Character.Weapon_2.performed += OnWeapon_2;

        inputSystem.Character.Reload.performed += OnReload;

        // If no Animator or Character controller is attached then Disable this behaviour
        if (animator == null) animator = gameObject.GetComponent<Animator>();
        if (animator == null) { this.enabled = false; }

        if (characterController == null) characterController = gameObject.GetComponent<CharacterController>();
        if (characterController == null) { this.enabled = false; }

    }
    private void OnDisable()
    {
        // Input event registering
        inputSystem.Character.Run.performed -= OnRun;
        inputSystem.Character.Run.canceled -= OnRun;

        inputSystem.Character.Jump.performed -= OnJump;
        inputSystem.Character.Jump.canceled -= OnJump;

        inputSystem.Character.Aim.performed -= OnAim;
        inputSystem.Character.Aim.canceled -= OnAim;

        inputSystem.Character.Fire.performed -= OnFire;
        inputSystem.Character.Fire.canceled -= OnFire;

        inputSystem.Character.Weapon_1.performed -= OnWeapon_1;
        inputSystem.Character.Weapon_2.performed -= OnWeapon_2;

        inputSystem.Character.Reload.performed -= OnReload;
    }


    void Start()
    {
        // creating hash to call animator parametors
        isPlayingHash = Animator.StringToHash("IsPlaying");
        forwardHash = Animator.StringToHash("Forward");
        sidewaysHash = Animator.StringToHash("SideWays");
        jumpHash = Animator.StringToHash("Jump");

        myTransform = transform;

        ragdollBodies = gameObject.GetComponentsInChildren<Rigidbody>();
        DisableRagdoll();

        if(GameManager.Instance.player == null) GameManager.Instance.RegisterPlayer(this.gameObject);
        else Destroy(gameObject);
    }


    void Update()
    {
        switch (characterState)
        {
            case CharacterState.Idle:
                WhileIdle();
                break;
            case CharacterState.Aim:
                WhileAim();
                break;
            case CharacterState.Fire:
                WhileAFire();
                break;
            case CharacterState.Jumping:
                WhileJump();
                break;
        }

        animationMovement.x = AnimationUtils.Increase(animationMovement.x, inputMovement.x, transitionSpeed * Time.deltaTime);
        animationMovement.y = AnimationUtils.Increase(animationMovement.y, inputMovement.y, transitionSpeed * Time.deltaTime);

        animator.SetFloat(sidewaysHash, animationMovement.x);
        animator.SetFloat(forwardHash, animationMovement.y);
    }

    private void OnAnimatorMove()
    {
        animator.ApplyBuiltinRootMotion();
        LookAround();
    }

    // State driven functions
    private void WhileIdle()
    {
        inputMovement = inputSystem.Character.Movement.ReadValue<Vector2>();

        if (run) inputMovement *= 2;

        mouseSencitivity = normalMouseSencitivity;
        if(gun) gun.Idle();

        if (fire) characterState = CharacterState.Fire;
        else if (aim) characterState = CharacterState.Aim;

        if(jump) Jump();

        SwitchToNormalCam();
    }
    private void WhileAim()
    {
        inputMovement = inputSystem.Character.Movement.ReadValue<Vector2>();

        mouseSencitivity = scopeMouseSencitivity;
        if (gun)
        {
            gun.Aim();
            gun.SideWays(inputMovement.x);
        }
        else
            SwitchToAimCam();

        if (fire) characterState = CharacterState.Fire;
        else if (!aim) characterState = CharacterState.Idle;

        if (jump) Jump();

    }
    private void WhileAFire()
    {
        inputMovement = inputSystem.Character.Movement.ReadValue<Vector2>();

        mouseSencitivity = scopeMouseSencitivity;
        if (gun)
        {
            gun.Fire();
            gun.SideWays(inputMovement.x);
        }
        if (aim && !fire) characterState = CharacterState.Aim;
        else if(!fire) characterState = CharacterState.Idle;

        if (jump) Jump();

        SwitchToAimCam();
    }
    private void WhileJump()
    {
        SwitchToNormalCam();
    }


    // Input event functions
    private void OnRun(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed) run = true;
        else if (context.phase == InputActionPhase.Canceled) run = false;
    }
    private void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed) jump = true;
        else if (context.phase == InputActionPhase.Canceled) jump = false;
    }
    private void OnAim(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed) aim = true;
        else if (context.phase == InputActionPhase.Canceled) aim = false;
    }
    private void OnFire(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed) fire = true;
        else if (context.phase == InputActionPhase.Canceled) fire = false;
    }
    private void OnReload(InputAction.CallbackContext context) {
        if (gun)
        {
            int bulletInHand = weaponManager.GetBulletCount(gun.mag.bulletType);
            bulletInHand = gun.Reload(bulletInHand);
            weaponManager.UpdateBulletCount(gun.mag.bulletType, bulletInHand);
        }
    }


    // Actions
    private void LookAround()
    {
        if (inputSystem.Character.enabled) inputLook = inputSystem.Character.Look.ReadValue<Vector2>();
        else inputLook = Vector2.zero;
        
        lookRotationX = inputLook.x;
        lookRotationX *= mouseSencitivity;

        lookRotation = Vector3.zero;
        lookRotationY += inputLook.y * -mouseSencitivity;
        lookRotationY = Mathf.Clamp(lookRotationY, -55F, 55F);
        lookRotation.x = lookRotationY;

        if(myTransform != null) myTransform.Rotate(Vector3.up, lookRotationX);
        cameraFollowTartget.localEulerAngles = lookRotation;

    }
    private void Jump()
    {        characterState = CharacterState.Jumping;
        animator.SetTrigger(jumpHash);
        
        if (gun) gun.Idle();
    }
    private void OnJumpFinish()
    {
        characterState = CharacterState.Idle; 
    }// This function is called by the aniomation event that trigger when the jump animation finishes


    private void SwitchToNormalCam()
    {
        normalCam.Priority = 10;
        aimCam.Priority = 0;
    }
    private void SwitchToAimCam()
    {
        normalCam.Priority = 0;
        aimCam.Priority = 10;
    }

    private void OnWeapon_1(InputAction.CallbackContext context)
    {
        if (context.performed) weaponManager.SwitchWeapon(0);
    }
    private void OnWeapon_2(InputAction.CallbackContext context)
    {
        if (context.performed) weaponManager.SwitchWeapon(1);
    }


    public void Reset()
    {
        characterState = CharacterState.Idle;
        if(gun) gun.Idle();
    }
    public void EnableCharacterInput(bool value)
    {
        animator.SetBool(isPlayingHash, value);
        if (!value && gun) weaponManager.FreeHand();
        weaponManager.enabled = value;
        
    }
    public void AddDamage(float damage, Vector3 force)
    {
        if (health <= 0) return;
        DecreaseHealth(damage);
    }
    public void AddDamage(float damage)
    {
        if (health <= 0) return;
        DecreaseHealth(damage);
    }
    public void Destroy()
    {
        
    }
    public void ResetHealth()
    {
        health = Health;
        DisableRagdoll();
    }

    public void DecreaseHealth(float damage)
    {
        health -= damage;
        if(health <= 0)
        {
            if (gun) weaponManager.DropGun();

            EnableRagdoll();
            GameManager.Instance.CurrentGamePlayMode.Fail();
        }
    }
    public void EnableRagdoll()
    {
        normalCam.Priority = 0;
        aimCam.Priority = 0;
        deadCam.Priority = 40;

        animator.enabled = false;
        if(characterController) characterController.detectCollisions = false;
        foreach (Rigidbody rigidbody in ragdollBodies) rigidbody.isKinematic = false;
    }
    public void DisableRagdoll()
    {
        normalCam.Priority = 10;
        aimCam.Priority = 0;
        deadCam.Priority = 0;

        animator.enabled = true;
        if (characterController) characterController.detectCollisions = true;
        foreach (Rigidbody rigidbody in ragdollBodies) rigidbody.isKinematic = true;
    }
    
}
