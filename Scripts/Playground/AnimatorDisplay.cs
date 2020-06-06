using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnimatorDisplay : MonoBehaviour {

    HumanoidGraphicsManager graphics;

    public List<string> steps;
    float time;

    // Start is called before the first frame update
    void Start() {
        graphics = new HumanoidGraphicsManager(transform.GetChild(0));
        time = Time.time;
    }

    string name = "";
    float delay;
    // Update is called once per frame
    void Update() {
        if (time + delay < Time.time) {
            time = Time.time;
            if (name == "run")
                graphics.animator.SetFloat("speedForward", 0);
            if (name == "slide")
                graphics.animator.SetBool("sliding", false);
            graphics.animator.SetBool("crouching", false);
            if (name == "crouch")
                graphics.animator.SetBool("crouching", false);

            name = steps[0];
            if (name == "run") {
                graphics.animator.SetFloat("speedForward", 1);
                delay = 5;
            }
            if (name == "slide") {
                graphics.animator.SetBool("sliding", true);
                graphics.animator.SetBool("crouching", true);
                delay = 2;
            }
            if (name == "crouch") {
                graphics.animator.SetBool("crouching", true);
                delay = 5;
            }

            steps.RemoveAt(0);
            steps.Add(name);
        }
    }
}
