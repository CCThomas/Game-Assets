using UnityEngine;
using UnityEditor;

public class CreatureManager {
    Collider[] colliders;
    Creature creature;
    Transform transform;

    GraphicsManager graphicsManager;
    MovementManagerV1 movementManager;

    public CreatureManager(Collider[] colliders, Creature creature, Transform transform) {
        this.colliders = colliders;
        this.creature = creature;
        this.transform = transform;
        SetGraphicsManager(creature.currentForm);
        SetMovementManager(MovementManagerV1.MovementState.Air);
    }

    public int Update(float horizontal, float vertical, Vector3 lookingDirection) {
        if (movementManager.StateChanged()) {
            SetMovementManager(movementManager.intendedState);
        }
        return movementManager.Update(horizontal, vertical, lookingDirection);
    }

    public int LateUpdate() {
        return movementManager.UpdateAnimation();
    }

    public virtual void SetDown(bool down) {
        movementManager.SetDown(down);
    }

    public virtual void SetQuick(bool quick) {
        movementManager.SetQuick(quick);
    }

    public virtual void SetUp(bool up) {
        movementManager.SetUp(up);
    }

    public virtual void ToggleDown() {
        movementManager.ToggleDown();
    }

    public virtual void ToggleQuick() {
        movementManager.ToggleQuick();
    }

    void SetGraphicsManager(Creature.CreatureType type) {
        if (type == Creature.CreatureType.Bird) {
            graphicsManager = new BirdGraphics(transform);
        } else if (type == Creature.CreatureType.Human) {
            graphicsManager = new HumanGraphics(transform);
        }
    }

    void SetMovementManager(MovementManagerV1.MovementState state) {
        if (movementManager != null) {
            movementManager.CleanUp();
        }

        if (state == MovementManagerV1.MovementState.Air) {
            movementManager = new AirMovementManager(colliders, creature, graphicsManager);
        } else if (state == MovementManagerV1.MovementState.Ground) {
            movementManager = new GroundMovementManager(colliders, creature, graphicsManager);
        }
    }
}

internal abstract class MovementManagerV1 {
    protected bool down;
    protected bool quick;
    protected bool up;
    protected Movement movement = new Movement();
    protected MovementState currentState;
    public MovementState intendedState;

    protected Collider[] colliders;
    protected Creature creature;
    protected GraphicsManager graphics;

    public MovementManagerV1(Collider[] colliders, Creature creature, GraphicsManager graphics) {
        this.colliders = colliders;
        this.creature = creature;
        this.graphics = graphics;
    }

    public abstract void CleanUp();

    public abstract int Update(float horizontal, float vertical, Vector3 lookingDirection);

    public abstract int UpdateAnimation();

    internal RaycastHit Raycast(Vector3 origin, Vector3 direction, float distance, LayerMask layerMask) {
        return Raycast(origin, direction, distance, layerMask, Color.red);
    }

    internal RaycastHit Raycast(Vector3 origin, Vector3 direction, float distance, LayerMask layerMask, Color color) {
        RaycastHit hit;
        Physics.Raycast(origin, direction, out hit, distance, layerMask.value);
        Debug.DrawRay(origin, direction * distance, color, 2, false);
        return hit;
    }

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

    public void SetState(MovementState currentState) {
        this.currentState = currentState;
        if (intendedState == MovementState.Unkown) {
            intendedState = currentState;
        }
    }

    public bool StateChanged() {
        return currentState != intendedState;
    }

    public class Movement {
        public float forward { get; set; }
        public float right { get; set; }
        public float up { get; set; }
    }

    public enum MovementState {
        Air, Climb, Ground, Water, Unkown
    }
}