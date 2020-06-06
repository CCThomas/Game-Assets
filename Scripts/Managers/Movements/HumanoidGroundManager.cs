using UnityEngine;
using UnityEditor;

internal class HumanoidGroundManager : MovementManager {
    // Trackers
    bool crouching, running, sliding; // TODO Current Crouch for after slide bug. Duplicate: Toggle Crouch off. Run, Crouch to slide. Release mid slide.
    float velocitySpeedSmooth, VelocityTurnSmooth;
    public RaycastHit holdRaycastHit;
    Vector3 distanceStart;

    // Modifiers
    float gravity = -12;
    float jumpHeight = .8f;
    float crouchHeightPercentage = .7f; // 70%
    float slideDistance = 2;
    float speedSlide = 5;
    float speedSmoothTime = 0.1f;
    float turnSmoothTime = 0.1f;
    float airControlPercent = 0.3f;

    public HumanoidGroundManager(CharacterManager characterManager, GraphicsManager graphics) : base(characterManager, graphics) {
        SetState(MovementState.Ground);
    }

    public override void CleanUp() {
        ((HumanoidGraphicsManager) graphics).SetCrouching(false);
    }

    public override int Update(float horizontal, float vertical, Vector3 lookingDirection) {
        if (sliding) {
            float currentSlideDistance = Vector3.Distance(distanceStart, character.GetPosition());
            if (slideDistance < currentSlideDistance) {
                AttemptSetSliding(false);
            } else {
                lookingDirection = graphics.creatureGameObject.forward;
                horizontal = 0;
                vertical = 1;
            }
        }

        // If character is not moving, set running to false
        if (vertical <= 0 || horizontal != 0) {
            AttemptSetRunning(false);
        }



        // Rotate Player
        float targetRotation = Mathf.Atan2(lookingDirection.x, lookingDirection.z) * Mathf.Rad2Deg + lookingDirection.y;
        graphics.Rotate(Vector3.up * Mathf.SmoothDampAngle(graphics.creatureGameObject.eulerAngles.y, targetRotation, ref VelocityTurnSmooth, GetModifiedSmoothTime(turnSmoothTime)));

        // Move Character
        float targetSpeed = running ? character.speedQuick : character.speedLeisure;
        if (crouching) {
            targetSpeed = character.speedSneak;
        }
        if (sliding) {
            targetSpeed = character.speedQuick;
        }

        movement.forward = GetModifiedMoveSpeed(vertical, movement.forward, targetSpeed);
        movement.right = GetModifiedMoveSpeed(horizontal, movement.right, targetSpeed);
        movement.up += Time.deltaTime * gravity;
        Vector3 velocity = graphics.Forward() * movement.forward + Vector3.up * movement.up + graphics.Right() * movement.right;

        Vector3 positionBeforeMoving = character.GetPosition();
        character.Move(velocity * Time.deltaTime);
        if (positionBeforeMoving == character.GetPosition() || positionBeforeMoving.y + .05 < character.GetPosition().y) {
            // Character is not/cannot move. Bug encounterd: If player slides into cornor, player will freeze
            // If player y index increases, player is moving up and cannot slide
            AttemptSetSliding(false);
        }

        // Air Stuff
        if (character.isGrounded()) {
            movement.up = -0.5f;
        } else {
            if (CanGrabHold(lookingDirection)) {
                intendedState = MovementState.Climb;
                return GetReturnInt();
            }
        }
        return GetReturnInt();
    }

    public override int UpdateAnimation() {
        // Update Animation. Look at Animator View for more information on the games animations for a player.
        float movingSpeed = crouching ? character.speedSneak : character.speedLeisure;
        float animationSpeedPercentF = running ? movement.forward / character.speedQuick : movement.forward / movingSpeed * .5f;
        float animationSpeedPercentS = running ? movement.right / character.speedQuick : movement.right / movingSpeed * .5f;
        animationSpeedPercentF *= crouching ? 0.5f : 1;
        animationSpeedPercentS *= crouching ? 0.5f : 1;

        animationSpeedPercentF = animationSpeedPercentF < .1f && animationSpeedPercentF > -.1 ? 0 : animationSpeedPercentF;
        animationSpeedPercentS = animationSpeedPercentS < .1f && animationSpeedPercentS > -.1 ? 0 : animationSpeedPercentS;


        ((HumanoidGraphicsManager) graphics).SetSpeedRight(animationSpeedPercentS, speedSmoothTime);
        ((HumanoidGraphicsManager) graphics).SetSpeedForward(animationSpeedPercentF, speedSmoothTime);
        ((HumanoidGraphicsManager) graphics).SetSpeedUp(movement.up, .5f);
        return 0;
    }

    public override void SetDown(bool down) {
        AttemptSetCrouch(down);
    }

    public override void SetQuick(bool quick) {
        AttemptSetRunning(quick);
    }

    public override void SetUp(bool up) {
        if (up && character.isGrounded() && !sliding) {
            if (AttemptSetCrouch(false)) {
                float jumpVelocity = Mathf.Sqrt(-2 * gravity * jumpHeight);
                movement.up = jumpVelocity;
                AttemptSetRunning(false); // Currently does not support run jump
            }
        }
    }

    public override void ToggleDown() {
        AttemptSetCrouch(!crouching);
    }

    public override void ToggleQuick() {
        AttemptSetRunning(!running);
    }


    bool AttemptSetCrouch(bool attemptCrouching) {
        if (!attemptCrouching && CanStandUp()) {
            this.crouching = attemptCrouching;
            character.ResetController();
            ((HumanoidGraphicsManager) graphics).SetCrouching(this.crouching);
        } else if (attemptCrouching) {
            this.crouching = attemptCrouching;
            character.ModifyController(crouchHeightPercentage);
            ((HumanoidGraphicsManager) graphics).SetCrouching(this.crouching);
            if (running) {
                AttemptSetSliding(true);
            }
        }

        return crouching == attemptCrouching;
    }

    bool AttemptSetRunning(bool attemptSetRunning) {
        if (attemptSetRunning && crouching) {
            // Running will be determined by if the character can stand up
            running = AttemptSetCrouch(false);
        } else {
            running = attemptSetRunning;
        }

        return running == attemptSetRunning;
    }

    bool AttemptSetSliding(bool attempSliding) {
        sliding = attempSliding;
        ((HumanoidGraphicsManager) graphics).SetSliding(attempSliding);
        if (sliding) {
            AttemptSetRunning(false);
            distanceStart = character.GetPosition();
        }
        return true;
    }

    bool CanGrabHold(Vector3 lookingDirection) {
        Vector3 position = character.GetPosition();
        Vector3 origin = new Vector3(position.x, position.y + (character.GetHeight() * crouchHeightPercentage), position.z);
        LayerMask holdLayerMask = LayerMask.GetMask("Hold");
        RaycastHit hit = Raycast(origin, lookingDirection, character.GetHeight() * crouchHeightPercentage, holdLayerMask, Color.green);
        if (hit.collider != null) {
            holdRaycastHit = hit;
            return true;
        }
        return false;
    }

    /**
     * Can Stand up takes the position of the Character Controller, and checks if the character controller would collide with something if was extended to full height.
     * <p>
     * The Y Coord is decided by placing it at the center of the Character Controller. That way it will not take feet collision into account. 
     * (There was a bug where placing the Y Coord at Zero would have the RayCast always Collide with something)
     */
    bool CanStandUp() {
        Vector3 origin = character.GetFullHeightCenter();
        Vector3 direction = Vector3.up.normalized;
        RaycastHit hit = Raycast(origin, direction, character.GetHeight() / 2, ~0, Color.blue);
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
        if (character.isGrounded()) {
            return smoothTime;
        }

        if (airControlPercent == 0) {
            return float.MaxValue;
        }
        return smoothTime / airControlPercent;
    }

    int GetReturnInt() {
        return crouching ? (int) HumanoidController.CameraTarget.Crouching : (int) HumanoidController.CameraTarget.Standing;
    }
}