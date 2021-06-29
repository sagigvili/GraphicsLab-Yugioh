using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Table : MonoBehaviour 
{
    public List<MonsterLogic> MonstersOnTable = new List<MonsterLogic>();

    //public List<SpellLogic> SpellsOnTable = new List<SpellLogic>();

    //public List<TrapLogic> TrapsOnTable = new List<TrapLogic>();

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

    

    //public void PlaceSpellAt(int index, SpellLogic spell)
    //{
    //    SpellsOnTable.Insert(index, spell);
    //}

    //public void PlaceTrapAt(int index, TrapLogic trap)
    //{
    //    TrapsOnTable.Insert(index, trap);
    //}

}
