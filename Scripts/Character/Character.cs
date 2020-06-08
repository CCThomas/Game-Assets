using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character/Character")]
public class Character : ScriptableObject {
    public Race race;
    public bool intialized;

    Dictionary<string, Trait> traitsDictionary = new Dictionary<string, Trait>();
    public List<Trait> traits = new List<Trait>() {
        new Trait("agility", 1),
        new Trait("height", 1),
        new Trait("hit_points", 1),
        new Trait("speed", 5)
    };


    Dictionary<string, Ability> abilityDictionary = new Dictionary<string, Ability>();
    public List<Ability> abilities = new List<Ability>() {
        new Ability("air_control_percent", "agility", 0.7f),
        new Ability("jump_height", "agility", 2),
        new Ability("speed_run", "speed", 1.5f),
        new Ability("speed_sneak", "speed", .75f),
        new Ability("speed_walk", "speed", 1),
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