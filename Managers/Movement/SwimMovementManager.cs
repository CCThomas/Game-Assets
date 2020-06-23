using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwimMovementManager : AbstractMovementManager
{
    Collider collider;
    public SwimMovementManager(AbstractMovementManager movementManager) : base(movementManager)
    {
        currentState = MovementState.Swim;
        intendedState = MovementState.Swim;
        SetSwimming(true);
        transform.position = new Vector3(transform.position.x, collider.transform.position.y, transform.position.z);
    }

    public override void CleanUp()
    {
        SetSwimming(false);
    }

    public override void OnCollisionStay(Collision collision)
    {
        //throw new System.NotImplementedException();
    }

    public override void OnTriggerStay(Collider collider)
    {
        //throw new System.NotImplementedException();
    }

    public override void UpdateAnimation()
    {
        //throw new System.NotImplementedException();
    }

    public override void UpdateMovement(float horizontal, float vertical, Vector3 lookDirection)
    {
        Debug.Log("Just Keep Swimming");
        Vector3 targetVelocity = new Vector3(horizontal, 0, vertical);

        targetVelocity = transform.TransformDirection(targetVelocity);
        targetVelocity *= character.GetAbilityValue("swim");

        // Prepare Velocity
        var velocity = rigidbody.velocity;
        var velocityChange = (targetVelocity - velocity);
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);

        // Rotate Player if not sliding
        float targetRotation = Mathf.Atan2(lookDirection.x, lookDirection.z) * Mathf.Rad2Deg + lookDirection.y;
        transform.eulerAngles = (Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref velocityTurnSmooth, turnSmoothTime));

        // Apply a force that attempts to reach our target velocity
        if (collider.transform.position.y < transform.position.y)
        {
            velocityChange.y = -.01f;
        }
        else if (collider.transform.position.y > transform.position.y)
        {
            velocityChange.y = .01f;
        }
        else
        {
            velocityChange.y = 0;
        }
        rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
        if (collider.transform.position.y < transform.position.y - .1f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - .1f, transform.position.z);
        }
        else if (collider.transform.position.y > transform.position.y + .1f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + .1f, transform.position.z);
        }

        RaycastHit hitinfo = Raycast(transform.position, Vector3.down, character.GetTraitValue("height") * .9f, DEFAULT, Color.red);
        if (hitinfo.collider != null)
        {
            raycastHit = hitinfo;
            intendedState = MovementState.Ground;
        }

    }

    private void SetSwimming(bool swimming)
    {
        animator.SetBool("swimming", swimming);
    }
}
