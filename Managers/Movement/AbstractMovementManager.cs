﻿using UnityEngine;
using UnityEditor;
using System;

public abstract class AbstractMovementManager
{
    public Animator animator;
    public Character character;
    public Rigidbody rigidbody;
    public Transform graphics;
    public Transform transform;

    // Layers
    protected readonly LayerMask DEFAULT = LayerMask.GetMask("Default");
    protected readonly LayerMask DEFAULT_INDEX = LayerMask.NameToLayer("Default");
    protected readonly LayerMask HOLD = LayerMask.GetMask("Hold");
    protected readonly LayerMask HOLD_INDEX = LayerMask.NameToLayer("Hold");
    protected readonly LayerMask WALL = LayerMask.GetMask("Wall");
    protected readonly LayerMask WALL_INDEX = LayerMask.NameToLayer("Wall");
    protected readonly LayerMask WATER = LayerMask.GetMask("Water");
    protected readonly LayerMask WATER_INDEX = LayerMask.NameToLayer("Water");

    // Properties
    protected readonly float maxVelocityChange = 10.0f;
    protected readonly float speedSmoothTime = 0.1f;
    protected readonly float turnSmoothTime = 0.1f;

    // Trackers
    public bool down, quick, midTransition, up;
    protected float velocityTurnSmooth, transitionTime;
    protected MovementState currentState;
    public MovementState intendedState;
    public RaycastHit raycastHit;

    public enum MovementState
    {
        Climb, Fly, Ground, Swim, Unkown
    }

    protected AbstractMovementManager(AbstractMovementManager movementManager)
    {
        animator = movementManager.animator;
        character = movementManager.character;
        rigidbody = movementManager.rigidbody;
        graphics = movementManager.graphics;
        transform = movementManager.transform;
        down = movementManager.down;
        quick = movementManager.quick;
        up = movementManager.up;
        //collider = movementManager.collider;
        raycastHit = movementManager.raycastHit;
        movementManager.CleanUp();
    }

    public AbstractMovementManager(Character character, Transform transform)
    {
        this.character = character;
        this.transform = transform;
        this.rigidbody = transform.GetComponent<Rigidbody>();
    }

    public abstract void OnCollisionStay(Collision collision);

    public abstract void OnTriggerStay(Collider collider);

    public abstract void UpdateAnimation();

    public abstract void UpdateMovement(float horizontal, float vertical, Vector3 lookDirection);

    public virtual bool AttemptInteract(Vector3 lookingDirection)
    {
        return false;
    }

    public virtual bool AttemptSetDown(bool desired)
    {
        this.down = desired;
        return true;
    }

    public virtual bool AttemptSetQuick(bool desired)
    {
        this.quick = desired;
        return true;
    }

    public virtual bool AttemptSetUp(bool desired)
    {
        this.up = desired;
        return true;
    }

    public virtual void CleanUp()
    {
        // Override in child
    }

    public virtual void SetGraphics(Transform graphics)
    {
        this.graphics = graphics;
        animator = graphics.GetComponent<Animator>();
    }

    public virtual void SetRaycastHit(RaycastHit raycastHit)
    {
        this.raycastHit = raycastHit;
    }

    public virtual bool StateChanged()
    {
        return currentState != intendedState;
    }

    protected RaycastHit Raycast(Vector3 origin, Vector3 direction, float distance, LayerMask layerMask, Color color)
    {
        RaycastHit hit;
        Physics.Raycast(origin, direction, out hit, distance, layerMask.value);
        Debug.DrawRay(origin, direction * distance, color, 2, false);
        return hit;
    }

    protected void SetCurrentState(MovementState currentState)
    {
        this.currentState = currentState;
    }

    protected void SetIntendedState(MovementState intendedState, float transitionTime)
    {
        if (this.transitionTime > Time.time)
        {
            return;
        }
        this.intendedState = intendedState;
        this.transitionTime = Time.time + transitionTime;
        midTransition = true;
    }

    protected void SetSpeedForward(float speedForward, float dampTime)
    {
        animator.SetFloat("speedForward", speedForward, dampTime, Time.deltaTime);
    }

    protected void SetSpeedRight(float speedRight, float dampTime)
    {
        animator.SetFloat("speedRight", speedRight, dampTime, Time.deltaTime);
    }

    protected void SetSpeedUp(float speedUp, float dampTime)
    {
        animator.SetFloat("speedUp", speedUp, dampTime, Time.deltaTime);
    }
}