using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Table : MonoBehaviour 
{
    public List<MonsterLogic> MonstersOnTable = new List<MonsterLogic>();

    public void PlaceMonstereAt(int index, MonsterLogic monster)
    {
        MonstersOnTable.Insert(index, monster);
    }
        
}
