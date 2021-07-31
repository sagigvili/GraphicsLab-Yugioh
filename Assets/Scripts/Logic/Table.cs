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

    public void PlaceMonstereAt(int index, MonsterLogic monster)
    {
        MonstersOnTable.Insert(index, monster);
    }



    public void PlaceSpellTrapAt(int index, SpellTrapLogic spelltrap)
    {
        SpellsTrapsOnTable.Insert(index, spelltrap);
    }

    public bool AnyAttackOrDefenceMonsters(SpellTrapEffects effect)
    {
        if (effect == SpellTrapEffects.ChangeToAttack)
        {
            foreach (MonsterLogic ml in MonstersOnTable)
            {
                if (ml.monsterPosition == FieldPosition.Defence)
                    return true;
            }
            return false;
        } else if (effect == SpellTrapEffects.ChangeToDefence)
        {
            foreach (MonsterLogic ml in MonstersOnTable)
            {
                if (ml.monsterPosition == FieldPosition.Attack)
                    return true;
            }
            return false;
        }
        return false;
    }

}
