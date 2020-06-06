using UnityEngine;
using UnityEditor;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using UnityEngine.UIElements;
using System.Runtime.ExceptionServices;

public class MyCreatureController {
    // VariableS Needed
    Transform transform;
    MyCreature creature;
    Rigidbody rigidbody;

    // Variables Set
    public ModelType modelType;
    Animator animator;
    Transform graphics;

    // Properties
    float gravity = 10.0f;
    float maxVelocityChange = 10.0f;
    float speedSmoothTime = 0.1f;
    float turnSmoothTime = 0.1f;

    // Trackers
    public bool crouch, run, jump, grounded, fly, slide;
    float velocitySpeedSmooth, VelocityTurnSmooth, timeSpentSliding;
    Vector3 animationTracker;
    Transform hold;

    public MyCreatureController(Transform transform, MyCreature creature) {
        this.transform = transform;
        this.rigidbody = transform.GetComponent<Rigidbody>();
        this.creature = creature;
    }

    public bool AttemptSetCrouch(bool desired) {
        if (slide) {
            return crouch == desired;
        }
        if (!desired && CanStandUp()) {
            crouch = desired;
        } else if (desired) {
            crouch = desired;
            if (run) {
                AttemptSetRun(false);
                AttemptSetSlide(true);
            }
        }
        SetCrouching(crouch);

        CapsuleCollider collider = graphics.GetComponent<CapsuleCollider>();

        if (crouch && modelType == ModelType.Humanoid) {
            collider.height = .8f;
        } else if (!crouch && modelType == ModelType.Humanoid) {
            collider.height = 1;
        }
        return crouch == desired;
    }

    public bool AttemptSetJump(bool desired) {
        if (slide) {
            return jump == desired;
        }
        if (desired && !jump) {
            if (grounded && AttemptSetCrouch(false)) {
                jump = true;
                AttemptSetRun(false);
            } else if (!grounded && modelType == ModelType.Bird) {
                fly = true;
            }
        }
        return jump == desired;
    }

    public bool AttemptSetRun(bool desired) {
        if (slide) {
            return run == desired;
        }
        if (jump && desired) {
            // Do nothing
        } else if (desired && crouch && AttemptSetCrouch(false)) {
            run = desired;
        } else {
            run = desired;
        }
        SetQuick(run);
        return run == desired;
    }

    public bool AttemptSetSlide(bool desired) {
        slide = desired;
        SetSliding(slide);
        if (slide) {
            timeSpentSliding = Time.time;
        }
        return slide == desired;
    }

    public Transform GetChest() {
        string path = "path";
        if (modelType == ModelType.Bird) {
            return transform;
        } else if (modelType == ModelType.Humanoid) {
            path = "Armature/Hip/Stomach/Chest";
        }
        return graphics.Find(path);
    }

    public Transform GetHead() {
        string path = "path";
        if (modelType == ModelType.Bird) {
            return transform;
        } else if (modelType == ModelType.Humanoid) {
            path = "Armature/Hip/Stomach/Chest/Neck/Head";
        }
        return graphics.Find(path);
    }

    public void OnCollisionStay() {
        grounded = true;
        fly = false;
    }

    public void UpdateGraphics(ModelType modelType, Transform graphics) {
        this.modelType = modelType;
        if (this.graphics != null) {
            this.graphics.gameObject.SetActive(false);
        }
        this.graphics = graphics;
        this.graphics.gameObject.SetActive(true);
        this.animator = graphics.GetComponent<Animator>();

        // For Testing only
        if (modelType == ModelType.Humanoid) {
            fly = false;
            //if (this.graphics.localScale.y != creature.GetTraitValue("height")) {
            //this.graphics.localScale *= creature.GetTraitValue("height");
            //}
        }
    }

    public void UpdateMovement(float horizontal, float vertical, Vector3 lookingDirection) {
        if (slide && timeSpentSliding + .5 < Time.time) {
            AttemptSetSlide(false);
        }

        // Calculate how fast we should be moving
        animationTracker.z = vertical;
        animationTracker.x = horizontal;
        Vector3 targetVelocity = fly || slide ? new Vector3(0, 0, 1) : new Vector3(horizontal, 0, vertical);
        if (grounded && targetVelocity.x == 0 && targetVelocity.z == 0) {
            AttemptSetRun(false);
            AttemptSetSlide(false);
        }

        targetVelocity = transform.TransformDirection(targetVelocity);
        targetVelocity *= GetSpeed();

        // Prepare Velocity
        var velocity = rigidbody.velocity;
        var velocityChange = (targetVelocity - velocity);
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);

        if (fly) {
            // Rotate Player
            transform.forward = lookingDirection;

            // Apply a force that attempts to reach our target velocity    
            rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
        } else {
            // Rotate Player
            if (!slide) {
                float targetRotation = Mathf.Atan2(lookingDirection.x, lookingDirection.z) * Mathf.Rad2Deg + lookingDirection.y;
                transform.eulerAngles = (Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref VelocityTurnSmooth, GetModifiedSmoothTime(turnSmoothTime)));
            }
            if (grounded) {
                // Apply a force that attempts to reach our target velocity
                velocityChange.y = 0;
                rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

                // Jump
                if (jump) {
                    jump = false;
                    float verticalSpeed = Mathf.Sqrt(2 * creature.GetAbilityValue("jump_height") * gravity);
                    rigidbody.velocity = new Vector3(velocity.x, verticalSpeed, velocity.z);
                }

            } else {
                if (modelType == ModelType.Humanoid && CanGrabHold(lookingDirection)) {
                    // TODO
                    Debug.Log("Implement Climbing");
                }
            }

            // We apply gravity manually for more tuning control
            rigidbody.AddForce(new Vector3(0, -gravity * rigidbody.mass, 0));

            grounded = false;
        }

        Debug.Log("before=" + graphics.GetChild(0).position);
        graphics.GetChild(0).position = new Vector3(0, 0, 0);
        Debug.Log("aftyer=" + graphics.GetChild(0).position);
        graphics.GetChild(1).position *= 0;
    }

    public void UpdateAnimation() {
        float animationSpeedPercentZ = animationTracker.z;
        float animationSpeedPercentX = animationTracker.x;
        SetSpeedForward(animationSpeedPercentZ, speedSmoothTime);
        SetSpeedRight(animationSpeedPercentX, speedSmoothTime);
        SetSpeedUp(rigidbody.velocity.y, speedSmoothTime);
    }

    bool CanGrabHold(Vector3 lookingDirection) {
        Vector3 origin = GetChest().position;
        LayerMask holdLayerMask = LayerMask.GetMask("Hold");
        RaycastHit hit = Raycast(origin, lookingDirection, .8f, holdLayerMask, Color.green);
        if (hit.collider != null) {
            //holdRaycastHit = hit;
            return true;
        }
        return false;
    }

    bool CanStandUp() {
        Vector3 origin = transform.position;
        origin.y += .1f;
        Vector3 direction = Vector3.up.normalized;
        RaycastHit hit = Raycast(origin, direction, /*creature.height*/ 1.7f, transform.gameObject.layer, Color.blue);
        return hit.collider == null;
    }

    float GetModifiedSmoothTime(float smoothTime) {
        if (grounded) {
            return smoothTime;
        }

        float airControlPercent = creature.GetAbilityValue("air_control_percent");
        if (airControlPercent == 0) {
            return float.MaxValue;
        }
        return smoothTime / airControlPercent;
    }

    float GetSpeed() {
        float movingSpeed = creature.GetAbilityValue("speed_walk");
        if (slide) {
            movingSpeed = creature.GetAbilityValue("speed_run");
        } else if (crouch) {
            movingSpeed = creature.GetAbilityValue("speed_crouch");
        } else if (run) {
            movingSpeed = creature.GetAbilityValue("speed_run");
        }
        return movingSpeed;
    }

    RaycastHit Raycast(Vector3 origin, Vector3 direction, float distance, LayerMask layerMask, Color color) {
        RaycastHit hit;
        Physics.Raycast(origin, direction, out hit, distance, layerMask.value);
        Debug.DrawRay(origin, direction * distance, color, 2, false);
        return hit;
    }

    void SetCrouching(bool crouching) {
        animator.SetBool("crouching", crouching);
    }

    void SetQuick(bool running) {
        animator.SetBool("running", running);
    }

    void SetSliding(bool sliding) {
        animator.SetBool("sliding", sliding);
    }

    void SetSpeedForward(float speedForward, float dampTime) {
        animator.SetFloat("speedForward", speedForward, dampTime, Time.deltaTime);
    }

    void SetSpeedRight(float speedRight, float dampTime) {
        animator.SetFloat("speedRight", speedRight, dampTime, Time.deltaTime);
    }

    void SetSpeedUp(float speedUp, float dampTime) {
        animator.SetFloat("speedUp", speedUp, dampTime, Time.deltaTime);
    }

    public enum ModelType {
        Bird, Humanoid
    }
}