using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectStateOnTable : SelectState
{
    public OneCardManager cardInTable;

    // state = 0 - Set
    // state = 1 - Attack
    // state = 2 - Defence
    public void SetState(int state)
    {
        if (cardInTable.cardAsset.Attack != -1)
        {
            if (state == 1)
            {
                cardInTable.cardAsset.MonsterState = FieldPosition.Attack;
                MonsterLogic.MonstersCreatedThisGame[GetComponentInParent<IDHolder>().UniqueID].monsterPosition = FieldPosition.Attack;
            }
            else if (state == 2)
            {
                cardInTable.cardAsset.MonsterState = FieldPosition.Defence;
                MonsterLogic.MonstersCreatedThisGame[GetComponentInParent<IDHolder>().UniqueID].monsterPosition = FieldPosition.Defence;
            }
        } else
        {
            cardInTable.cardAsset.SpellTrapState = SpellTrapPosition.FaceUp;
            SpellTrapLogic.SpellTrapsCreatedThisGame[GetComponentInParent<IDHolder>().UniqueID].State = SpellTrapPosition.FaceUp;
        }

        
        

    }

}
