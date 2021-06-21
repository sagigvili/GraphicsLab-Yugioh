using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class BattleStats 
{
    public static List<CardLogic> PlayerCards = new List<CardLogic>();
    public static List<CardLogic> EnemyCards = new List<CardLogic>();

    public static List<MonsterLogic> PlayerMonsters = new List<MonsterLogic>();
    public static List<MonsterLogic> EnemyMonsters = new List<MonsterLogic>();
}
