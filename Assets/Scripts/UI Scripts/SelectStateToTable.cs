using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectStateToTable : SelectState
{
    public OneCardManager cardInHand;
    public Transform setState;
    public Transform summonState;

    private void Update()
    {
        if (cardInHand)
        {
            if (!cardInHand.toLoop)
                if (cardInHand.cardAsset.Attack == -1)
                    cardInHand.GetComponent<DragSpellTrapOnTable>().AfterStateSelected();
                else
                    cardInHand.GetComponent<DragMonsterOnTable>().AfterStateSelected();
        }
    }
    // state = 0 - Set
    // state = 1 - Attack
    public void SetMonsterState(int state)
    {
        if (state == 1)
            cardInHand.cardAsset.MonsterState = FieldPosition.Attack;
        else
            cardInHand.cardAsset.MonsterState = FieldPosition.Set;
        cardInHand.toLoop = false;
    }

    public void SetSpellTrapState(int state)
    {
        if (state == 1)
            cardInHand.cardAsset.SpellTrapState = SpellTrapPosition.FaceUp;
        else
            cardInHand.cardAsset.SpellTrapState = SpellTrapPosition.Set;
        cardInHand.toLoop = false;
    }

}
