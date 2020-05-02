using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[Serializable]
public class Stat : ScriptableObject {
    public StatName statName;
    public int advantage;
    public int value;
    public int level;

    public static Stat[] GetInitStats() {
        Stat[] stats = new Stat[System.Enum.GetValues(typeof(StatName)).Length];
        int index = 0;
        foreach (StatName statName in System.Enum.GetValues(typeof(StatName))) {
            stats[index] = (Stat) CreateInstance<Stat>();
            stats[index].statName = statName;
            index++;
        }
        Console.WriteLine(stats);
        return stats;
    }
}

public enum StatName {
    Agility, Courage, Health, Intelligence, Strength
}