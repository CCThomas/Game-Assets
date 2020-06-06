using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine.XR.WSA.Input;

[CreateAssetMenu(fileName = "New Graphics Manager", menuName = "Graphics Manager")]
public class GrapicsManagerV1 : ScriptableObject {
    Animator animator;
    Transform creatureGameObject;
    Creature creature;
    public GameObject birdGraphics;
    public GameObject humanGraphics;

    public void Start() {
        Debug.Log(birdGraphics);
    }

    public void V1(Transform creatureGameObject, Creature creature) {
        this.creatureGameObject = creatureGameObject;
        this.creature = creature;

        animator = new Animator();
        animator.runtimeAnimatorController = (RuntimeAnimatorController) RuntimeAnimatorController.Instantiate(Resources.Load("Animators/HumanoidAnimatorController.controller", typeof(RuntimeAnimatorController)));
        animator.applyRootMotion = true;
        animator.updateMode = AnimatorUpdateMode.Normal;
        animator.cullingMode = AnimatorCullingMode.CullCompletely;
    }

    void UpdatedAvatar() {
        //animator.avatar = birdGraphics.avatar;
    }
}
