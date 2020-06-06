using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TestMovement {
    protected bool down;
    protected bool quick;
    protected bool up;
    protected Movement movement = new Movement();
    public MovementState currentState;
    public MovementState intendedState;
    protected TestCreature creature;
    protected TestGraphics graphics;
    protected Transform transform;

    public TestMovement(TestCreature creature, Transform transform) {
        this.creature = creature;
        this.transform = transform;
    }

    internal abstract int Update(float horizontal, float vertical, Vector3 lookingDirection);

    internal abstract void UpdateAnimation();

    public bool IsGrounded() {
        // TODO HERE This may not work
        Vector3 position = transform.position;
        position.y += 0.1f;
        return Raycast(transform.position, -Vector3.up, 0.2f, ~0).collider != null;
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

    public void SetGraphics(TestGraphics graphics) {
        this.graphics = graphics;
    }

    public void SetState(MovementState state) {
        currentState = intendedState = state;
    }

    public bool StateChanged() { return currentState != intendedState; }

    // Virtual Methods
    public virtual void SetDown(bool down) {
        this.down = down;
    }

    public virtual void SetQuick(bool quick) {
        this.quick = quick;
    }

    public virtual void SetUp(bool up) {
        this.up = up;
    }

    public virtual void ToggleDown() {
        down = !down;
    }

    public virtual void ToggleQuick() {
        quick = !quick;
    }

    public virtual void ToggleUp() {
        up = !up;
    }

    public enum MovementState {
        Air, Climb, Ground, Water
    }

    public class Movement {
        public float forward { get; set; }
        public float right { get; set; }
        public float up { get; set; }
    }
}

class TestGroundMovementManager : TestMovement {

    // Crouching (down), Jumping (up), and Running (quick).
    bool crouching, jumping, running;

    // Sliding Trackers
    bool sliding;
    Vector3 slideStartLocation;

    // Smoothing Trackers
    float velocitySpeedSmooth, VelocityTurnSmooth;

    // Game Object creature is trying to hold
    public RaycastHit holdRaycastHit;

    // Modifiers
    float gravity = -12;
    float jumpHeight = .8f;
    float crouchHeightPercentage = .7f; // 70%
    float slideDistance = 2;
    float speedSmoothTime = 0.1f;
    float turnSmoothTime = 0.01f;
    float airControlPercent = 0.3f;

    public TestGroundMovementManager(TestCreature creature, Transform transform) : base(creature, transform) {
        SetState(MovementState.Ground);
    }

    internal override int Update(float horizontal, float vertical, Vector3 lookingDirection) {
        // If sliding
        if (sliding && slideDistance < Vector3.Distance(slideStartLocation, transform.position)) {
            Debug.Log("TODO Slding is done");
            sliding = false;
        } else if (sliding) {
            lookingDirection = transform.forward;
            horizontal = 0;
            vertical = 1;
        }

        // If character is not moving, set desired run to false
        if (vertical <= 0 || horizontal != 0) {
            Debug.Log("TODO Run false");
            quick = false;
        }

        // if desired state does not equal current state.
        if (crouching != down) {
            Debug.Log("TODO Crouch. desiredCrouch=" + crouching + ", down=" + down);
            crouching = down;
            graphics.SetCrouching(down);
        }
        bool grounded = IsGrounded();
        if (jumping != up && IsGrounded()) {
            Debug.Log("TODO Jump. grounded=" + grounded);
            jumping = up;
            float jumpVelocity = Mathf.Sqrt(-2 * gravity * jumpHeight);
            movement.up = jumpVelocity;
        }
        if (running != quick) {
            Debug.Log("TODO Run");
            running = quick;
            graphics.SetQuick(quick);
        }

        // Rotate Player
        float targetRotation = Mathf.Atan2(lookingDirection.x, lookingDirection.z) * Mathf.Rad2Deg + lookingDirection.y;
        transform.eulerAngles = (Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref VelocityTurnSmooth, GetModifiedSmoothTime(turnSmoothTime)));

        // Move Character
        float targetSpeed = 0;
        if (crouching) {
            targetSpeed = creature.speedSneak;
        } else if (running) {
            targetSpeed = creature.speedQuick;
        } else if (sliding) {
            targetSpeed = (creature.speedQuick + creature.speedLeisure) / 2;
        } else {
            targetSpeed = creature.speedLeisure;
        }

        movement.forward = GetModifiedMoveSpeed(vertical, movement.forward, targetSpeed);
        movement.right = GetModifiedMoveSpeed(horizontal, movement.right, targetSpeed);
        Vector3 velocity = transform.forward * movement.forward + Vector3.up * movement.up + transform.right * movement.right;

        Vector3 positionBeforeMoving = transform.position;
        graphics.AddForce(velocity);
        //transform.Translate(velocity * Time.deltaTime, Space.World);
        if (positionBeforeMoving == transform.position) {
            // Character is not/cannot move. Bug encounterd: If player slides into cornor, player will freeze
            Debug.Log("TODO Stop player from sliding is position is no longer moving");
            sliding = false;
        }

        // Jumping and Attempting to Climb
        if (IsGrounded()) {
            jumping = up = false;
        } else if (CanGrabHold(lookingDirection)) {
            intendedState = MovementState.Climb;
        }
        return 0;
    }

    internal override void UpdateAnimation() {
        // Update Animation. Look at Animator View for more information on the games animations for a player.
        float movingSpeed = down ? creature.speedSneak : creature.speedLeisure;
        float animationSpeedPercentF = quick ? movement.forward / creature.speedQuick : movement.forward / movingSpeed * .5f;
        float animationSpeedPercentS = quick ? movement.right / creature.speedQuick : movement.right / movingSpeed * .5f;
        //animationSpeedPercentF *= down ? 0.5f : 1;
        //animationSpeedPercentS *= down ? 0.5f : 1;

        animationSpeedPercentF = animationSpeedPercentF < .1f && animationSpeedPercentF > -.1 ? 0 : animationSpeedPercentF;
        animationSpeedPercentS = animationSpeedPercentS < .1f && animationSpeedPercentS > -.1 ? 0 : animationSpeedPercentS;

        //Debug.Log("animationSpeedPercentS=" + animationSpeedPercentS);
        //Debug.Log("animationSpeedPercentF=" + animationSpeedPercentF);
        graphics.SetSpeedRight(animationSpeedPercentS, speedSmoothTime);
        graphics.SetSpeedForward(animationSpeedPercentF, speedSmoothTime);
        graphics.SetSpeedUp(movement.up, .5f);
    }

    bool CanGrabHold(Vector3 lookingDirection) {
        Debug.Log("TODO: bool CanGrabHold(Vector3 lookingDirection)");
        return false;
    }

    /**
     * Can Stand up takes the position of the Character Controller, and checks if the character controller would collide with something if was extended to full height.
     * <p>
     * The Y Coord is decided by placing it at the center of the Character Controller. That way it will not take feet collision into account. 
     * (There was a bug where placing the Y Coord at Zero would have the RayCast always Collide with something)
     */
    bool CanStandUp() {
        Vector3 origin = transform.position;
        Vector3 direction = Vector3.up.normalized;
        RaycastHit hit = Raycast(origin, direction, graphics.standingHeight, ~0, Color.blue);
        return hit.collider == null;
    }

    float GetModifiedMoveSpeed(float input, float current, float target) {
        if (input == 0) {
            return 0;
        }
        float targetSpeed = input > 0 ? target : target * -1;
        return Mathf.SmoothDamp(current, targetSpeed, ref velocitySpeedSmooth, GetModifiedSmoothTime(speedSmoothTime));
    }

    float GetModifiedSmoothTime(float smoothTime) {
        if (IsGrounded()) {
            return smoothTime;
        }

        if (airControlPercent == 0) {
            return float.MaxValue;
        }
        return smoothTime / airControlPercent;
    }
}
