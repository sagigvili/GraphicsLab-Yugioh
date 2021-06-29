using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectStateToTable : SelectState
{
    public OneCardManager monsterInHand;

    private void Update()
    {
        if (monsterInHand)
        {
            if (!monsterInHand.toLoop)
                monsterInHand.GetComponent<DragMonsterOnTable>().AfterStateSelected();
        }
    }
    // state = 0 - Set
    // state = 1 - Attack
    public void SetState(int state)
    {
        if (state == 1)
            monsterInHand.monsterState = FieldPosition.Attack;
        else
            monsterInHand.monsterState = FieldPosition.Set;
        monsterInHand.toLoop = false;
    }

}
