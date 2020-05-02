using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "New Effect", menuName = "Inventory/Effect")]
public class Effect : ScriptableObject {

    new public string name;
    public EffectType effectType;
    public float modifier;
    public bool isPassive;

    [HideInInspector]
    public bool isConstant;
    [HideInInspector]
    public float passiveFrequencyInSeconds;
    [HideInInspector]
    public float passiveExpirationInSeconds;
}

public enum EffectType {
    Damage, Defence, Health, Stamina
}

#if UNITY_EDITOR
[CustomEditor(typeof(Effect))]
public class Effect_Editor : Editor {
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // for other non-HideInInspector fields
 
        Effect script = (Effect) target;
 
         // if bool is true, show other fields 
        if (script.isPassive && !script.isConstant) {
            script.isConstant = EditorGUILayout.Toggle("Is Constant", script.isConstant);
            script.passiveFrequencyInSeconds = EditorGUILayout.FloatField("Frequency in seconds", script.passiveFrequencyInSeconds);
            script.passiveExpirationInSeconds = EditorGUILayout.FloatField("Expiration Time", script.passiveExpirationInSeconds);
        } else if (script.isPassive) {
            script.isConstant = EditorGUILayout.Toggle("Is Constant", script.isConstant);
        }
    }
}
#endif