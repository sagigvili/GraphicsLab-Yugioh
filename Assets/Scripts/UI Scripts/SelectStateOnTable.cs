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
                MonsterLogic.MonstersCreatedThisGame[GetComponentInParent<IDHolder>().UniqueID].ca.MonsterState = FieldPosition.Attack;
            }
            else if (state == 2)
            {
                cardInTable.cardAsset.MonsterState = FieldPosition.Defence;
                MonsterLogic.MonstersCreatedThisGame[GetComponentInParent<IDHolder>().UniqueID].ca.MonsterState = FieldPosition.Defence;
            }
        } else
        {
            var tempColor1 = cardInTable.CardImageFront.color;
            tempColor1.a = 255;
            cardInTable.CardImageFront.color = tempColor1;
            cardInTable.cardAsset.SpellTrapState = SpellTrapPosition.FaceUp;
            SpellTrapLogic.SpellTrapsCreatedThisGame[GetComponentInParent<IDHolder>().UniqueID].ca.SpellTrapState = SpellTrapPosition.FaceUp;
        }

        
        

    }

}
