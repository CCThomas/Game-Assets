using UnityEngine;
using UnityEditor;
using System.Runtime.CompilerServices;

[CreateAssetMenu(fileName = "New Character Manager", menuName = "Character")]
public class CharacterManager : ScriptableObject {
    public float speedClimbing;
    public float speedLeisure;
    public float speedQuick;
    public float speedSneak;

    CharacterController characterController;
    float height;
    float radius;

    public void SetCharacterController(CharacterController characterController) {
        this.characterController = characterController;
        height = characterController.height;
        radius = characterController.radius;
    }

    public float GetHeight() {
        return height;
    }

    public Vector3 GetPosition() {
        return characterController.transform.position;
    }

    public Vector3 GetFullHeightCenter() {
        float x = characterController.transform.position.x;
        float y = characterController.transform.position.y + height / 2; // Y Value is the "Standing" modified value
        float z = characterController.transform.position.z;
        return new Vector3(x, y, z);
    }

    public bool isGrounded() {
        return characterController.isGrounded;
    }

    public void Lerp(Vector3 newPosition, float t) {
        characterController.transform.position = Vector3.Lerp(characterController.transform.position, newPosition, t);
    }

    public void ModifyController(float modifier) {
        ModifyController(modifier, modifier);
    }

    public void ModifyController(float modifierCentner, float modifierHeight) {
        characterController.center = Vector3.up * height / 2 * modifierCentner;
        characterController.height = height * modifierHeight;
    }

    public void Move(Vector3 velocity) {
        characterController.Move(velocity);
    }

    public void ResetController() {
        characterController.center = new Vector3(0, height / 2, 0);
        characterController.height = height;
        characterController.radius = radius;
        ModifyController(1);
    }

    public void SetRadius(float modifier) {
        this.characterController.radius = modifier;
    }
}