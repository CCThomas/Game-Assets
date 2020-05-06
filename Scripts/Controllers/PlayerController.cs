using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    // Player
    public float walkSpeed = 2;
    public float runSpeed = 6;
    public float currentSpeed;
    public float gravity = -12;
    public float jumpHeight = 1f;
    float velocityY;
    [Range(0, 1)]
    public float airControlPercent;

    public float speedSmoothTime = 0.1f;
    float speedSmoothVelocity;

    public float turnSmoothTime = 0.2f;
    float turnSmoothVelocity;
	bool crouching;


    Animator animator;
    Transform cameraTransform;
    CharacterController characterController;
    Transform modelTransform;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();

        modelTransform = transform.GetChild((int)PlayerChild.Graphics);
        animator = GetComponentInChildren<Animator>();
        cameraTransform = transform.GetChild((int)PlayerChild.Camera);
        CameraController cameraController = (CameraController)cameraTransform.GetComponent("CameraController");
        cameraController.SetThirdPersonTarget(transform.GetChild((int)PlayerChild.CameraFocus));
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 inputDirection = input.normalized;
        Debug.Log(inputDirection.x + "," + inputDirection.y);

        // Determines if the Player is currently running.
        bool running = Input.GetKey(KeyCode.LeftShift);

        Move(inputDirection, running);

		if (Input.GetKeyDown(KeyCode.LeftControl)) {
			Crouch();
		}
        if (Input.GetKey(KeyCode.Space))
        {
            Jump();
        }

        // Update Animation. Look at Animator View for more information on the games animations for a player.
        float animationSpeedPercent = running ? currentSpeed / runSpeed : currentSpeed / walkSpeed * .5f;
        animationSpeedPercent *= crouching ? 0.5f : 1;
        animator.SetFloat("speedPercent", animationSpeedPercent, speedSmoothTime, Time.deltaTime);
    }

    void LateUpdate()
    {
    }

    float GetModifiedSmoothTime(float smoothTime)
    {
        if (characterController.isGrounded)
        {
            return smoothTime;
        }

        if (airControlPercent == 0)
        {
            return float.MaxValue;
        }
        return smoothTime / airControlPercent;
    }

	void Crouch() {
		crouching = !crouching;
		animator.SetBool("crouching", crouching);
		if (crouching) {
			characterController.height = 1.20f;
			characterController.center = new Vector3(0, 0.65f, 0);
		} else {
			characterController.height = 1.74f;
			characterController.center = new Vector3(0, 0.87f, 0);
		}
	}

    void Jump()
    {
        if (characterController.isGrounded)
        {
            float jumpVelocity = Mathf.Sqrt(-2 * gravity * jumpHeight);
            velocityY = jumpVelocity;
        }
    }

    void Move(Vector2 inputDirection, bool running)
    {
        // Rotate player to face input direction.
        float targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
        modelTransform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(modelTransform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(turnSmoothTime));

        // If magnitude is zero, player is not moving and speed should be set to zero.
        float targetSpeed = running ? runSpeed : walkSpeed;
        targetSpeed *= crouching ? .5f : 1;
        targetSpeed *= inputDirection.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));

        // Move Player
        velocityY += Time.deltaTime * gravity;
        Vector3 velocity = modelTransform.forward * currentSpeed + Vector3.up * velocityY;
        characterController.Move(velocity * Time.deltaTime);
        currentSpeed = new Vector2(characterController.velocity.x, characterController.velocity.z).magnitude;

        // Set Velcity Y to zero is the player is on the ground (done falling)
        if (characterController.isGrounded)
        {
            velocityY = 0;
        }
    }
}

enum PlayerChild
{
    Camera = 1,
    CameraFocus = 2,
    Graphics = 0
}