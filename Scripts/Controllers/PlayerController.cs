using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

    // Object
    public string startGraphicKey;
    public List<Graphic> graphics;
    public Character character;
    CharacterManager characterManager;
    Dictionary<string, Transform> graphicsDict;

    // Camera
    CameraManager cameraManager;
    public LayerMask firstPersonLayers;
    public LayerMask thirdPersonLayers;
    public float mouseSensitivity = 10f;

    // Controls
    PlayerInputActions controls;
    new Rigidbody rigidbody;
    float horizontal, vertical;
    public bool toggleCrouching, toggleRunning;

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
        controls.PlayerControls.SwitchModel.performed += _ => SwitchModel();
    }

    // Use this for initialization
    void Start() {
        character.Initialize();
        graphicsDict = Graphic.ToDictionary(graphics);

        characterManager = new CharacterManager(transform, character);
        characterManager.UpdateGraphics(CharacterManager.ModelType.Humanoid, graphicsDict[startGraphicKey]);

        cameraManager = new CameraManager(transform, firstPersonLayers, thirdPersonLayers);
        cameraManager.SetTarget(characterManager.GetHead());
    }

    // Update is called once per frame
    void LateUpdate() {
        cameraManager.LateUpdate();
        characterManager.UpdateAnimation();
    }

    void FixedUpdate() {
        characterManager.UpdateMovement(horizontal, vertical, cameraManager.LookingDirection());
    }

    Transform GetGraphic(string key) {
        return graphicsDict[key];
    }

    void SetCrouch(InputActionType type) {
        bool desired = false;
        if (type == InputActionType.Performed && toggleCrouching) {
            desired = !characterManager.crouch;
        } else if (type == InputActionType.Performed && !toggleCrouching) {
            desired = true;
        } else if (type == InputActionType.Canceled && !toggleCrouching) {
            desired = false;
        } else {
            return;
        }
        characterManager.AttemptSetCrouch(desired);
    }

    void SetJump(InputActionType type) {
        characterManager.AttemptSetJump(true);
    }

    void SetLook(Vector2 direction) {
        float yaw = direction.x * mouseSensitivity * Time.deltaTime;
        float pitch = direction.y * mouseSensitivity * Time.deltaTime;
        cameraManager.Rotate(yaw, pitch);
    }

    void SetMove(Vector2 direction) {
        horizontal = direction.x;
        vertical = direction.y;
    }

    void SetRun(InputActionType type) {
        bool desired = false;
        if (type == InputActionType.Performed && toggleRunning) {
            desired = !characterManager.run;
        } else if (type == InputActionType.Performed && !toggleRunning) {
            desired = true;
        } else if (type == InputActionType.Canceled && !toggleRunning) {
            desired = false;
        } else {
            return;
        }
        characterManager.AttemptSetRun(desired);
    }

    void SetScroll(Vector2 scroll) {
        cameraManager.AdjustDistance(scroll.normalized);
    }

    void SwitchCamera() {
        cameraManager.SwitchCamera();
    }

    void SwitchModel() {
        if (characterManager.modelType == CharacterManager.ModelType.Bird) {
            characterManager.UpdateGraphics(CharacterManager.ModelType.Humanoid, GetGraphic("human")); ;
        } else if (characterManager.modelType == CharacterManager.ModelType.Humanoid) {
            characterManager.UpdateGraphics(CharacterManager.ModelType.Bird, GetGraphic("bird"));
        }
        cameraManager.SetTarget(characterManager.GetHead());
    }

    void OnCollisionStay() {
        characterManager.OnCollisionStay();
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

[System.Serializable]
public class Graphic {
    public string key;
    public Transform model;

    public static Dictionary<string, Transform> ToDictionary(List<Graphic> graphics) {
        Dictionary<string, Transform> dict = new Dictionary<string, Transform>();
        foreach (Graphic graphic in graphics) {
            graphic.model.gameObject.SetActive(false);
            dict.Add(graphic.key, graphic.model);
        }
        return dict;
    }
}