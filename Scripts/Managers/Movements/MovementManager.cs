using UnityEngine;
using UnityEditor;

internal abstract class MovementManager {
    protected bool down;
    protected bool quick;
    protected bool up;
    protected Movement movement = new Movement();
    protected MovementState currentState;
    public MovementState intendedState = MovementState.Unkown;

    // Managers
    protected CharacterManager character;
    protected GraphicsManager graphics;

    public MovementManager(CharacterManager characterManager, GraphicsManager graphics) {
        this.character = characterManager;
        this.graphics = graphics;
    }

    public abstract void CleanUp();

    public abstract int Update(float vertical, float horizontal, Vector3 lookingDirection);

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
        Climb, Ground, Unkown
    }

}