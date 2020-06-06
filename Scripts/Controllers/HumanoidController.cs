using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class HumanoidController : MonoBehaviour {
    //Player Character
    public CharacterManager character;

    // Camera
    public LayerMask firstPersonLayers = ~0;
    public LayerMask thirdPersonLayers = ~0;
    public CameraTarget cameraTarget = CameraTarget.Standing;

    // Controls
    public bool toggleCrouch;
    public bool toggleRunning;
    public float mouseSensitivity = 10f;

    // Managers
    CameraManager cameraManager;
    HumanoidManager humanoidManager;

    // Move
    PlayerInputActions controlls;

    private void Awake() {
        controlls = new PlayerInputActions();
        controlls.PlayerControls.Crouch.performed += _ => SetCrouch(InputActionType.Performed);
        controlls.PlayerControls.Crouch.canceled += _ => SetCrouch(InputActionType.Canceled);
        controlls.PlayerControls.Jump.performed += _ => SetJump(InputActionType.Performed);
        controlls.PlayerControls.Run.performed += _ => SetRun(InputActionType.Performed);
        controlls.PlayerControls.Run.canceled += _ => SetRun(InputActionType.Canceled);
        controlls.PlayerControls.SwitchCamera.performed += _ => SwitchCamera();
        controlls.PlayerControls.Move.performed += context => SetMove(context.ReadValue<Vector2>());
        controlls.PlayerControls.Look.performed += context => SetLook(context.ReadValue<Vector2>());
    }


    // Start is called before the first frame update
    void Start() {
        character.SetCharacterController(GetComponent<CharacterController>());

        cameraManager = new CameraManager(transform, firstPersonLayers, thirdPersonLayers);
        UpdateCameraTarget();
    }

    // Update is called once per frame
    void Update() {
        CameraTarget currentCameraTarget = (CameraTarget) humanoidManager.Update(forward, right, cameraManager.LookingDirection());
        if (currentCameraTarget != cameraTarget) {
            cameraTarget = currentCameraTarget;
            UpdateCameraTarget();
        }
    }

    void LateUpdate() {
        humanoidManager.LateUpdate();
        cameraManager.LateUpdate();
    }

    void SetCrouch(InputActionType type) {
        if (type == InputActionType.Performed && toggleCrouch) {
            humanoidManager.ToggleDown();
        } else if (type == InputActionType.Performed && !toggleCrouch) {
            humanoidManager.SetDown(true);
        } else if (type == InputActionType.Canceled && !toggleCrouch) {
            humanoidManager.SetDown(false);
        }
    }

    void SetJump(InputActionType type) {
        if (type == InputActionType.Performed) {
            humanoidManager.SetUp(true);
        } else if (type == InputActionType.Canceled) {
            humanoidManager.SetUp(false);
        }
    }
    void SetRun(InputActionType type) {
        if (type == InputActionType.Performed && toggleRunning) {
            humanoidManager.ToggleQuick();
        } else if (type == InputActionType.Performed && !toggleRunning) {
            humanoidManager.SetQuick(true);
        } else if (type == InputActionType.Canceled && !toggleRunning) {
            humanoidManager.SetQuick(false);
        }
    }

    float forward, right;
    void SetMove(Vector2 direction) {
        forward = direction.x;
        right = direction.y;
    }

    void SetLook(Vector2 lookingDirection) {
        float yaw = lookingDirection.x;
        float pitch = lookingDirection.y;
        cameraManager.Rotate(yaw, pitch);
    }
    void SwitchCamera() {
        cameraManager.SwitchCamera();
    }

    void UpdateCameraTarget() {
        Transform cameraFocus = transform.GetChild((int) PlayerChild.Focus);
        cameraManager.SetTarget(cameraFocus.GetChild((int) cameraTarget));
    }

    private void OnEnable() {
        controlls.Enable();
    }

    private void OnDisable() {
        controlls.Disable();
    }

    public enum CameraTarget {
        Standing = 1, Crouching = 0, Climbing = 0, Hanging = 0
    }
    enum PlayerChild {
        Graphics = 0,
        Focus = 1
    }

    enum InputActionType {
        Canceled, Performed, Started
    }
}