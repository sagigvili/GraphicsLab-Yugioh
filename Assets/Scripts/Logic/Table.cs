using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Table : MonoBehaviour 
{
    public List<MonsterLogic> MonstersOnTable = new List<MonsterLogic>();

    public List<SpellTrapLogic> SpellsTrapsOnTable = new List<SpellTrapLogic>();

    public bool PlayedACard = false;

    public bool InAttackPhase = false;

    public void ResetPlayedACard()
    {
        PlayedACard = false;
    }

    public MonsterLogic GetMonsterAtIndex(int i)
    {
        return MonstersOnTable[i];
    }

    public SpellTrapLogic GetSpellTrapAtIndex(int i)
    {
        return SpellsTrapsOnTable[i];
    }

    public void PlaceMonstereAt(int index, MonsterLogic monster)
    {
        MonstersOnTable.Insert(index, monster);
    }



    public void PlaceSpellTrapAt(int index, SpellTrapLogic spelltrap)
    {
        SpellsTrapsOnTable.Insert(index, spelltrap);
    }

    public bool AnyDefenceMonsters()
    {
        foreach (MonsterLogic ml in MonstersOnTable)
        {
            if (ml.ca.MonsterState == FieldPosition.Defence)
                return true;
        }
        return false;
    }

    public bool AnyAttackMonsters()
    {
        foreach (MonsterLogic ml in MonstersOnTable)
            if (ml.ca.MonsterState == FieldPosition.Attack)
                return true;
        return false;
    }

    public bool AnySetMonsters()
    {
        foreach (MonsterLogic ml in MonstersOnTable)
            if (ml.ca.MonsterState == FieldPosition.Set)
                return true;
        return false;
    }

    public bool OnlySetMonsters()
    {
        foreach (MonsterLogic ml in MonstersOnTable)
            if (ml.ca.MonsterState != FieldPosition.Set)
                return false;
        return true;
    }

}
