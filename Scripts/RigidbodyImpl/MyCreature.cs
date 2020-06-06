using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.InputSystem;
[CreateAssetMenu(fileName = "New My Creature", menuName = "My/My Creature")]
public class MyCreature : ScriptableObject {

    public new string name;
    public Race race;
    public bool intialized;

    Dictionary<string, Trait> traitsDictionary = new Dictionary<string, Trait>();
    public List<Trait> traits = new List<Trait>() {
        new Trait("agility", 1),
        new Trait("height", 1)
    };


    Dictionary<string, Ability> abilityDictionary = new Dictionary<string, Ability>();
    public List<Ability> abilities = new List<Ability>() {
        new Ability("air_control_percent", "agility", 0.7f),
        new Ability("jump_height", "agility", 2),
        new Ability("height", "agility", 1),
        new Ability("speed_crouch", "agility", 3),
        new Ability("speed_run", "agility", 7),
        new Ability("speed_walk", "agility", 5),
    };

    public List<Tree> Tree = new List<Tree>() {
        new Tree(),
    };

    public void Initialize() {
        InitializeDictionary(traits);
        InitializeDictionary(abilities);
        if (!intialized) {
            intialized = true;
            Debug.Log(traitsDictionary);
            foreach (TraitModifier traitModifier in race.modifiers) {
                if (traitModifier.type == TraitModifierType.Ability) {
                    abilityDictionary[traitModifier.key].value *= traitModifier.modifier;
                } else if (traitModifier.type == TraitModifierType.Trait) {
                    traitsDictionary[traitModifier.key].value *= traitModifier.modifier;
                }
            }
        }
    }

    private void InitializeDictionary(List<Ability> list) {
        foreach (Ability ability in list) {
            abilityDictionary.Add(ability.name, ability);
        }
    }

    private void InitializeDictionary(List<Trait> list) {
        foreach (Trait trait in list) {
            traitsDictionary.Add(trait.name, trait);
        }
    }

    public float GetAbilityValue(string key) {
        return abilityDictionary[key].value * traitsDictionary[abilityDictionary[key].traitKey].value;
    }

    internal float GetTraitValue(string key) {
        return traitsDictionary[key].value;
    }
}

[CreateAssetMenu(fileName = "New Race", menuName = "My/My Race")]
public class Race : ScriptableObject {
    public new string name;
    public List<TraitModifier> modifiers;


}

[System.Serializable]
public class TraitModifier {
    public string name;
    public string key;
    public float modifier;
    public TraitModifierType type;
}
public enum TraitModifierType {
    Trait, Ability
}

[System.Serializable]
public class Trait {
    public string name;
    public float value;
    public float defaulValue;

    public Trait(string name, float defaultValue) {
        this.name = name;
        this.value = this.defaulValue = defaultValue;
    }
}


[System.Serializable]
public class Ability : Trait {
    public string traitKey;
    public int level;

    public Ability(string name, string traitKey) : base(name, 1) {
        this.traitKey = traitKey;
        level = 0;
    }

    public Ability(string name, string traitKey, float defaulValue) : base(name, defaulValue) {
        this.traitKey = traitKey;
        level = 0;
    }
}