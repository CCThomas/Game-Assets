using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Stat/Skill")]
public class Skill : ScriptableObject {
    new public string name = "New Skill";
    public StatName[] statTypes;
    public int advantage;
    public int level;

    private int experience;
    private int experienceNeededToLevel;
    Dictionary<int, float> advanatageModifier;

    Skill() {
        advanatageModifier.Add(-3, 0.33f);
        advanatageModifier.Add(-2, 0.5f);
        advanatageModifier.Add(-1, 0.75f);
        advanatageModifier.Add(0, 1.0f);
        advanatageModifier.Add(1, 1.5f);
        advanatageModifier.Add(2, 2.0f);
        advanatageModifier.Add(3, 3.0f);
    }

    public void AddExperience(int exp) {
        experience += exp * advanatageModifier.Keys.ElementAt(advantage);
        if (experience >= experienceNeededToLevel) {
            level++;
            experience -= experienceNeededToLevel;

            // Reset experienceNeededToLevel
            experienceNeededToLevel = 1;
            for (int i = 1; i < level; i++) {
                experienceNeededToLevel *= 2;
            }
        }
    }
}
