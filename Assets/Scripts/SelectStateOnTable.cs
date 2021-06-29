using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectStateOnTable : SelectState
{
    public OneMonsterManager monsterInTable;

    // state = 0 - Set
    // state = 1 - Attack
    // state = 2 - Defence
    public void SetState(int state)
    {
        if (state == 1)
        {
            monsterInTable.monsterState = FieldPosition.Attack;
            MonsterLogic.MonstersCreatedThisGame[GetComponentInParent<IDHolder>().UniqueID].monsterPosition = FieldPosition.Attack;
        }
        else if (state == 2)
        {
            monsterInTable.monsterState = FieldPosition.Defence;
            MonsterLogic.MonstersCreatedThisGame[GetComponentInParent<IDHolder>().UniqueID].monsterPosition = FieldPosition.Defence;
        }
        
        

    }

}
