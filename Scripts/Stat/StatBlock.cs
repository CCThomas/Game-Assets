using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Statblock", menuName = "Stat/StatBlock")]
public class StatBlock : ScriptableObject {
    public Stat[] stats;
    public Skill[] skills;


    void OnEnable() {
        stats = Stat.GetInitStats();
    }
}
