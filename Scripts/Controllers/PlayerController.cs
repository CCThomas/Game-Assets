using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    // // Player
    // public float walkSpeed = 2;
    // public float runSpeed = 6;
    // public float currentSpeed;
    // public float gravity = -12;
    // public float jumpHeight = 1f;
    // float velocityY;

    // public float speedSmoothTime = 0.1f;
    // float speedSmoothVelocity;

    // public float turnSmoothTime = 0.2f;
    // float turnSmoothVelocity;

    // public Transform cameraTransform;
    // public CharacterController controller;
    // Animator animator;

    // // Start is called before the first frame update
    // void Start() {
    //     animator = GetComponent<Animator>();
    //     cameraTransform = Camera.main.transform;
    //     controller = GetComponent<CharacterController>();
    // }

    // // Update is called once per frame
    // void Update() {
    //     Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    //     Vector2 inputDirection = input.normalized;

    //     if (Input.GetKey(KeyCode.Space)) {
    //         Jump();
    //     }

    //     // Rotate player to face input direction.
    //     // If statement stops the player from reseting it's direction if the user is not pressing a key.
    //     if (inputDirection != Vector2.zero) {
    //         float targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
    //         transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
    //     }

    //     // Determines if the Player is currently running.
    //     bool running = Input.GetKey(KeyCode.LeftShift);

    //     // If magnitude is zero, player is not moving and speed should be set to zero.
    //     float targetSpeed = (running ? runSpeed : walkSpeed) * inputDirection.magnitude;
    //     currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);


    //     // Move Player
    //     velocityY += Time.deltaTime * gravity;
    //    Vector3 velocity = transform.forward * currentSpeed + Vector3.up * velocityY;
    //    controller.Move(velocity * Time.deltaTime);
    //    currentSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;

    //    if (controller.isGrounded) {
    //        velocityY = 0;
    //    }

    //     // Update Animation. Look at Animator View for more information on the games animations for a player.
    //     float animationSpeedPercent = running ? currentSpeed/runSpeed : currentSpeed/walkSpeed *.5f;
    //     animator.SetFloat("speedPercent", animationSpeedPercent, speedSmoothTime, Time.deltaTime);
    // }

    // void LateUpdate() {
    // }

    // void Jump() {
    //     if (controller.isGrounded) {
    //         float jumpVelocity = Mathf.Sqrt(-2 * gravity * jumpHeight);
    //         velocityY = jumpVelocity;
    //     }
    // }
    
	public float walkSpeed = 2;
	public float runSpeed = 6;
	public float gravity = -12;
	public float jumpHeight = 1;
	[Range(0,1)]
	public float airControlPercent;

	public float turnSmoothTime = 0.2f;
	float turnSmoothVelocity;

	public float speedSmoothTime = 0.1f;
	float speedSmoothVelocity;
	float currentSpeed;
	float velocityY;

	Animator animator;
	Transform cameraT;
	CharacterController controller;

	void Start () {
		animator = GetComponent<Animator> ();
		cameraT = Camera.main.transform;
		controller = GetComponent<CharacterController> ();
	}

	void Update () {
		// input
		Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		Vector2 inputDir = input.normalized;
		bool running = Input.GetKey (KeyCode.LeftShift);

		Move (inputDir, running);

		if (Input.GetKeyDown (KeyCode.Space)) {
			Jump ();
		}
		// animator
		float animationSpeedPercent = ((running) ? currentSpeed / runSpeed : currentSpeed / walkSpeed * .5f);
		animator.SetFloat ("speedPercent", animationSpeedPercent, speedSmoothTime, Time.deltaTime);

	}

	void Move(Vector2 inputDir, bool running) {
		if (inputDir != Vector2.zero) {
			float targetRotation = Mathf.Atan2 (inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
			transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(turnSmoothTime));
		}
			
		float targetSpeed = ((running) ? runSpeed : walkSpeed) * inputDir.magnitude;
		currentSpeed = Mathf.SmoothDamp (currentSpeed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));

		velocityY += Time.deltaTime * gravity;
		Vector3 velocity = transform.forward * currentSpeed + Vector3.up * velocityY;

		controller.Move (velocity * Time.deltaTime);
		currentSpeed = new Vector2 (controller.velocity.x, controller.velocity.z).magnitude;

		if (controller.isGrounded) {
			velocityY = 0;
		}

	}

	void Jump() {
		if (controller.isGrounded) {
			float jumpVelocity = Mathf.Sqrt (-2 * gravity * jumpHeight);
			velocityY = jumpVelocity;
		}
	}

	float GetModifiedSmoothTime(float smoothTime) {
		if (controller.isGrounded) {
			return smoothTime;
		}

		if (airControlPercent == 0) {
			return float.MaxValue;
		}
		return smoothTime / airControlPercent;
	}
}