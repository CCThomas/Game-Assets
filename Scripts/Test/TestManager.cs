using UnityEngine;
using UnityEditor;

public class TestManager {
    // Bipedal Skeleton (Human, Elf, ect...)
    GameObject bipedalGameObject;

    // Bird Skeleton
    GameObject birdGameObject;

    // Managers
    TestCreature creature;
    TestGraphics graphics;
    TestMovement movement;

    Transform transform;

    public TestManager(TestCreature creature, Transform transform) {
        this.creature = creature;
        this.transform = transform;
    }

    public void SetModel(GameObject gameObject, TestCreature.CreatureType creatureType) {
        if (creatureType == TestCreature.CreatureType.Bird) {
            birdGameObject = gameObject;
        } else if (creatureType == TestCreature.CreatureType.Human) {
            bipedalGameObject = gameObject;
        }
    }

    public void Start(TestCreature.CreatureType form, TestMovement.MovementState state) {
        SetGraphicsManager(form);
        SetMovementManager(state);
    }

    // Returns the head of the graphic, which will be used as the Camera's Target
    public Transform GetHead() {
        return graphics.GetBoneHead();
    }

    // Return true if player model has changed
    public bool Update(float horizontal, float vertical, Vector3 lookingDirection) {
        if (creature.FormChanged()) {
            SetGraphicsManager(creature.intendedForm);
            return true;
        }

        if (movement.StateChanged()) {
            SetMovementManager(movement.intendedState);
        }
        movement.Update(horizontal, vertical, lookingDirection);
        return false;
    }

    public void UpdateAnimation() {
        movement.UpdateAnimation();
    }

    public void SetDown(bool down) {
        movement.SetDown(down);
    }

    public void SetUp(bool up) {
        movement.SetUp(up);
    }

    public void SetQuick(bool quick) {
        movement.SetQuick(quick);
    }

    public void ToggleDown() {
        movement.ToggleDown();
    }

    public void ToggleQuick() {
        movement.ToggleQuick();
    }

    public void ToggleUp() {
        movement.ToggleUp();
    }

    public void SetGraphicsManager(TestCreature.CreatureType creatureType) {
        if (creatureType == TestCreature.CreatureType.Bird) {
            graphics = new TestBirdGraphics(transform.GetComponent<Rigidbody>(), birdGameObject.transform);
            creature.currentForm = TestCreature.CreatureType.Bird;

        } else if (creatureType == TestCreature.CreatureType.Human) {
            graphics = new TestBipedalGraphics(transform.GetComponent<Rigidbody>(), bipedalGameObject.transform);
            creature.currentForm = TestCreature.CreatureType.Human;
        }

        if (movement != null) {
            movement.SetGraphics(graphics);
        }
    }

    public void SetMovementManager(TestMovement.MovementState movementState) {
        if (movementState == TestMovement.MovementState.Air) {
            throw new System.NotImplementedException("Movement State Not Implemented=Air");
        } else if (movementState == TestMovement.MovementState.Climb) {
            throw new System.NotImplementedException("Movement State Not Implemented=Climb");
        } else if (movementState == TestMovement.MovementState.Ground) {
            movement = new TestGroundMovementManager(creature, transform);
        } else if (movementState == TestMovement.MovementState.Water) {
            throw new System.NotImplementedException("Movement State Not Implemented=Water");
        }
        movement.SetGraphics(graphics);
    }
}