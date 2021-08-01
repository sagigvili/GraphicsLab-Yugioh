using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectStateToTable : SelectState
{
    public OneCardManager cardInHand;
    public Transform setState;
    public Transform summonState;

    // state = 0 - Set
    // state = 1 - Attack
    public void SetMonsterState(int state)
    {
        if (state == 1)
            cardInHand.cardAsset.MonsterState = FieldPosition.Attack;
        else
            cardInHand.cardAsset.MonsterState = FieldPosition.Set;
        Command.CommandExecutionComplete();
        cardInHand.GetComponent<DragMonsterOnTable>().AfterStateSelected();
        
    }

    public void SetSpellTrapState(int state)
    {
        if (state == 1)
            cardInHand.cardAsset.SpellTrapState = SpellTrapPosition.FaceUp;
        else
            cardInHand.cardAsset.SpellTrapState = SpellTrapPosition.Set;
        Command.CommandExecutionComplete();
        cardInHand.GetComponent<DragSpellTrapOnTable>().AfterStateSelected();
    }

}
