using UnityEngine;
using System.Collections;
using System.Linq.Expressions;
using System;

public class MyPlayerController : MonoBehaviour {

    // Object
    Animator animator;
    public Transform modelHuman;
    public Transform modelBird;
    public MyCreature creature;
    public ModelType modeltype;
    public Transform model;

    // Camera
    PlayerCamera playerCamera;
    public LayerMask firstPersonLayers;
    public LayerMask thirdPersonLayers;
    public float mouseSensitivity = 10f;

    // Movement
    public bool toggleRunning;
    public bool toggleCrouch;
    float velocitySpeedSmooth, VelocityTurnSmooth;
    public bool flying;
    float turnSmoothTime = 0.1f;
    float speedSmoothTime = 0.1f;
    bool running, crouching, grounded, jumping;
    float gravity = 10.0f;
    float maxVelocityChange = 10.0f;
    bool canJump = true;
    Vector3 movement;

    // Player Controls
    PlayerInputActions controls;
    new Rigidbody rigidbody;
    float horizontal, vertical;

    public static readonly string Hip = "Armature/Hip";
    public static readonly string Stomach = Hip + "/Stomach";
    public static readonly string Chest = Stomach + "/Chest";
    public static readonly string Neck = Chest + "/Neck";
    public static readonly string Head = Neck + "/Head";
    public static readonly string Shoulder = Chest + "/Shoulder.SIDE";
    public static readonly string Bicep = Shoulder + "/Bicep.SIDE";
    public static readonly string Forearm = Bicep + "/Forearm.SIDE";
    public static readonly string Hand = Forearm + "/Hand.SIDE";
    public static readonly string FingerUpper = Hand + "/FingerUpper.SIDE";
    public static readonly string FingerLower = FingerUpper + "/FingerLower.SIDE";
    public static readonly string ThumbUpper = Hand + "/ThumbUpper.SIDE";
    public static readonly string ThumbLower = ThumbUpper + "/ThumbLower.SIDE";
    public static readonly string Tigh = Hip + "/Tigh.SIDE";
    public static readonly string Shin = Tigh + "/Shin.SIDE";
    public static readonly string Foot = Shin + "/Foot.SIDE";
    public static readonly string Toe = Foot + "/Toe.SIDE";

    private void Awake() {
        controls = new PlayerInputActions();
        controls.PlayerControls.Crouch.performed += _ => SetCrouch(InputActionType.Performed);
        controls.PlayerControls.Crouch.canceled += _ => SetCrouch(InputActionType.Canceled);
        controls.PlayerControls.Jump.performed += _ => SetJump(InputActionType.Performed);
        controls.PlayerControls.Run.performed += _ => SetRun(InputActionType.Performed);
        controls.PlayerControls.Run.canceled += _ => SetRun(InputActionType.Canceled);
        controls.PlayerControls.SwitchCamera.performed += _ => SwitchCamera();
        controls.PlayerControls.Move.performed += context => SetMove(context.ReadValue<Vector2>());
        controls.PlayerControls.Move.canceled += context => SetMove(new Vector2(0, 0));
        controls.PlayerControls.Look.performed += context => SetLook(context.ReadValue<Vector2>());
        controls.PlayerControls.Look.canceled += context => SetLook(new Vector2(0, 0));
        controls.PlayerControls.CameraDistance.performed += context => SetScroll(context.ReadValue<Vector2>());
    }

    private void SwitchCamera() {
        playerCamera.SwitchCamera();
    }

    public enum ModelType {
        Human, Bird
    }
    void SwitchModel(ModelType modelType) {
        this.modeltype = modelType;
        model.gameObject.SetActive(false);
        if (modeltype == ModelType.Bird) {
            model = modelBird;
        } else if (modeltype == ModelType.Human) {
            model = modelHuman;
        }

        float scale = creature.GetTraitValue("height");
        Vector3 v3 = new Vector3(scale, scale, scale);
        if (model.localScale != v3) {
            model.localScale = v3;
        }

        model.gameObject.SetActive(true);
        animator = model.GetComponent<Animator>();
        Transform t = GetBone(Head, ArmatureSide.NA);
        playerCamera.SetTarget(t == null ? model : t);
    }

    private void SetRun(InputActionType performed) {
        bool desired = false;
        if (performed == InputActionType.Performed && toggleRunning) {
            desired = !running;
        } else if (performed == InputActionType.Performed && !toggleRunning) {
            desired = true;
        } else if (performed == InputActionType.Canceled && !toggleRunning) {
            desired = false;
        } else {
            return;
        }
        AttemptSetRun(desired);
    }

    void SetScroll(Vector2 scrop) {
        Debug.Log(scrop.normalized);
        playerCamera.AdjustDistance(scrop.normalized);
    }

    bool AttemptSetRun(bool desired) {
        if (crouching && AttemptSetCrouch(false)) {
            running = desired;
        } else {
            running = desired;
        }
        return running == desired;
    }

    private void SetJump(InputActionType performed) {
        if (!jumping && AttemptSetCrouch(false)) {
            jumping = true;
            AttemptSetRun(false);
        }
    }

    private void SetCrouch(InputActionType performed) {
        bool desired = false;
        if (performed == InputActionType.Performed) {
            desired = true;
        } else if (performed == InputActionType.Canceled) {
            desired = false;
        } else {
            return;
        }
        AttemptSetCrouch(desired);
    }

    bool AttemptSetCrouch(bool desired) {
        if (!desired && CanStandUp()) {
            crouching = desired;
            SetCrouching(crouching);
        } else if (desired) {
            crouching = desired;
            SetCrouching(crouching);
            AttemptSetRun(false);
        }
        return crouching == desired;
    }

    bool CanStandUp() {
        Vector3 origin = transform.position;
        Vector3 direction = Vector3.up.normalized;
        RaycastHit hit = Raycast(origin, direction, /*creature.height*/ 1.7f, ~0, Color.blue);
        return hit.collider == null;
    }

    // Use this for initialization
    void Start() {
        rigidbody = GetComponent<Rigidbody>();
        playerCamera = new PlayerCamera(transform, firstPersonLayers, thirdPersonLayers);
        // TODO Set PlayerCamera Position to Head
        SwitchModel(ModelType.Bird);
        //model.localScale *= creature.height; TODO
    }

    void LateUpdate() {
        playerCamera.LateUpdate();


        float movingSpeed = GetSpeed();
        float animationSpeedPercentZ = movement.z / movingSpeed * .5f;
        float animationSpeedPercentX = movement.x / movingSpeed * .5f;

        //Debug.Log(animationSpeedPercentZ + "=" + movement.z + "/" + movingSpeed);
        //Debug.Log(animationSpeedPercentX + "=" + movement.x + "/" + movingSpeed);
        SetSpeedForward(animationSpeedPercentZ, speedSmoothTime);
        SetSpeedRight(animationSpeedPercentX, speedSmoothTime);
        SetSpeedUp(movement.y, speedSmoothTime);
    }

    // Fixed Update is called once per frame
    // Put Physics Updates here
    void FixedUpdate() {
        if (flying) {
            // Rotate Player
            transform.forward = playerCamera.LookingDirection();

            // Calculate how fast we should be moving
            Vector3 targetVelocity = new Vector3(horizontal, 0, vertical);

            targetVelocity = transform.TransformDirection(targetVelocity);
            targetVelocity *= GetSpeed();

            // Apply a force that attempts to reach our target velocity
            var velocity = rigidbody.velocity;
            var velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            //velocityChange.y = 0;
            rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
            movement = velocityChange;
        } else {
            // Rotate Player
            float targetRotation = Mathf.Atan2(playerCamera.LookingDirection().x, playerCamera.LookingDirection().z) * Mathf.Rad2Deg + playerCamera.LookingDirection().y;
            transform.eulerAngles = (Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref VelocityTurnSmooth, GetModifiedSmoothTime(turnSmoothTime)));

            if (grounded) {
                // Calculate how fast we should be moving
                Vector3 targetVelocity = new Vector3(horizontal, 0, vertical);

                targetVelocity = transform.TransformDirection(targetVelocity);
                targetVelocity *= GetSpeed();

                // Apply a force that attempts to reach our target velocity
                var velocity = rigidbody.velocity;
                var velocityChange = (targetVelocity - velocity);
                velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                velocityChange.y = 0;
                rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
                movement = velocityChange;

                // Jump
                if (canJump && jumping) {
                    jumping = false;
                    rigidbody.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
                }
            }

            // We apply gravity manually for more tuning control
            rigidbody.AddForce(new Vector3(0, -gravity * rigidbody.mass, 0));

            grounded = false;
        }
    }

    void OnCollisionStay() {
        grounded = true;
    }

    float CalculateJumpVerticalSpeed() {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * creature.GetAbilityValue("jump_height") * gravity);
    }

    internal RaycastHit Raycast(Vector3 origin, Vector3 direction, float distance, LayerMask layerMask) {
        return Raycast(origin, direction, distance, layerMask, Color.red);
    }

    internal RaycastHit Raycast(Vector3 origin, Vector3 direction, float distance, LayerMask layerMask, Color color) {
        RaycastHit hit;
        Physics.Raycast(origin, direction, out hit, distance, layerMask.value);
        Debug.DrawRay(origin, direction * distance, color, 2, false);
        return hit;
    }

    Transform GetBone(string name, ArmatureSide side) {
        if (side != ArmatureSide.NA) {
            if (side == ArmatureSide.Left) {
                name = name.Replace(".SIDE", ".L");
            }
            if (side == ArmatureSide.Right) {
                name = name.Replace(".SIDE", ".R");
            }
        }
        return model.Find(name);
    }

    public enum ArmatureSide {
        Left, Right, NA
    }

    float GetSpeed() {
        float movingSpeed = creature.GetAbilityValue("speed_walk");
        if (crouching) {
            movingSpeed = creature.GetAbilityValue("speed_crouch");
        } else if (running) {
            movingSpeed = creature.GetAbilityValue("speed_run");
        }
        return movingSpeed;
    }

    float GetModifiedSmoothTime(float smoothTime) {
        if (/*character.isGrounded()*/true) {
            return smoothTime;
        }

        float airControlPercent;
        if (airControlPercent == 0) {
            return float.MaxValue;
        }
        return smoothTime / airControlPercent;
    }

    void SetLook(Vector2 direction) {
        float yaw = direction.x * mouseSensitivity * Time.deltaTime;
        float pitch = direction.y * mouseSensitivity * Time.deltaTime;
        playerCamera.Rotate(yaw, pitch);
    }
    void SetMove(Vector2 direction) {
        horizontal = direction.x;
        vertical = direction.y;
    }

    public void SetCrouching(bool crouching) {
        Debug.Log("crouching=" + crouching);
        animator.SetBool("crouching", crouching);
    }

    public void SetQuick(bool crouching) {
        animator.SetBool("running", crouching);
    }

    public void SetSliding(bool sliding) {
        animator.SetBool("sliding", sliding);
    }

    public void SetSpeedForward(float speedForward, float dampTime) {
        animator.SetFloat("speedForward", speedForward, dampTime, Time.deltaTime);
    }

    public void SetSpeedRight(float speedRight, float dampTime) {
        animator.SetFloat("speedRight", speedRight, dampTime, Time.deltaTime);
    }

    public void SetSpeedUp(float speedUp, float dampTime) {
        animator.SetFloat("speedUp", speedUp, dampTime, Time.deltaTime);
    }

    private void OnEnable() {
        controls.Enable();
    }

    private void OnDisable() {
        controls.Disable();
    }

    private enum InputActionType {
        Canceled, Performed
    }
}

class PlayerCamera {
    // Needed From Parent
    Camera playerCamera;
    Transform transform;
    Transform target;
    public LayerMask firstPersonLayers;
    public LayerMask thirdPersonLayers;

    // Properties
    float distanceFromTarget = 2f;
    float rotationSmoothTime = .12f;
    Vector2 pitchMinMax = new Vector2(-40, 85);
    Vector2 scrollMinMax = new Vector2(1f, 3f);

    // Trackers
    bool inverted = false;
    float yaw, pitch;
    CameraMode currentCameaMode;
    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;

    public PlayerCamera(Transform parent, LayerMask firstPersonLayers, LayerMask thirdPersonLayers) {
        GameObject cameraGameObject = new GameObject("Camera for " + parent.name);
        playerCamera = cameraGameObject.AddComponent<Camera>();
        cameraGameObject.transform.parent = parent;
        transform = cameraGameObject.transform;
        this.firstPersonLayers = firstPersonLayers;
        this.thirdPersonLayers = thirdPersonLayers;
        currentCameaMode = CameraMode.FirstPerson;

        // Lock Cursor to Game
        Cursor.lockState = CursorLockMode.Locked;

        UpdateCamera();
    }

    public void Rotate(float yawModifier, float pitchModifier) {
        yaw += yawModifier;
        pitch -= pitchModifier * (inverted ? -1 : 1);
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
    }

    public void LateUpdate() {
        // Rotate Camera
        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
        transform.eulerAngles = currentRotation;

        // Move Camera to target location
        if (currentCameaMode == CameraMode.FirstPerson) {
            transform.position = target.position;
        } else if (currentCameaMode == CameraMode.ThirdPerson) {
            transform.position = target.position - transform.forward * distanceFromTarget;
        }
    }

    public void SwitchCamera() {
        if (currentCameaMode == CameraMode.ThirdPerson) {
            currentCameaMode = CameraMode.FirstPerson;
        } else {
            currentCameaMode = CameraMode.ThirdPerson;
        }
        UpdateCamera();
    }

    internal Vector3 LookingDirection() {
        return transform.forward;
    }

    void UpdateCamera() {
        if (currentCameaMode == CameraMode.FirstPerson) {
            playerCamera.cullingMask = firstPersonLayers;

        } else {
            playerCamera.cullingMask = thirdPersonLayers;
        }
    }

    public void SetTarget(Transform target) {
        this.target = target;
    }

    internal void AdjustDistance(Vector2 scroll) {
        distanceFromTarget += scroll.y * .2f;
        if (distanceFromTarget < scrollMinMax.x) {
            distanceFromTarget = scrollMinMax.x;
        } else if (distanceFromTarget > scrollMinMax.y) {
            distanceFromTarget = scrollMinMax.y;
        }
    }

    public enum CameraMode {
        FirstPerson, ThirdPerson
    }
}