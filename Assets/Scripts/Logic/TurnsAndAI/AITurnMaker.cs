using UnityEngine;
using System.Collections;

//this class will take all decisions for AI. 

public class AITurnMaker: TurnMaker {

    public override void OnTurnStart()
    {
        base.OnTurnStart();
        // dispay a message that it is enemy`s turn
        new ShowMessageCommand(false, 2.0f).AddToQueue();
        p.DrawACard();
        StartCoroutine(MakeAITurn());
    }

    // THE LOGIC FOR AI
    IEnumerator MakeAITurn()
    {
        bool strategyAttackFirst = false;
        if (Random.Range(0, 2) == 0)
            strategyAttackFirst = true;

        while (MakeOneAIMove(strategyAttackFirst))
        {
            yield return null;
        }

        InsertDelay(1f);

        TurnManager.Instance.EndTurn();
    }

    bool MakeOneAIMove(bool attackFirst)
    {
        if (Command.CardDrawPending())
            return true;
        else if (attackFirst)
            return AttackWithAMonster() || PlayACardFromHand() || UsePlayerPower();
        else 
            return PlayACardFromHand() || AttackWithAMonster() || UsePlayerPower();
    }

    bool PlayACardFromHand()
    {
        foreach (CardLogic c in p.hand.CardsInHand)
        {
            if (c.CanBePlayed)
            {

            }
            //Debug.Log("Card: " + c.ca.name + " can NOT be played");
        }
        return false;
    }

    bool UsePlayerPower()
    {
        if (!p.usedPlayerPowerThisTurn)
        {
            // use HP
            p.UsePlayerPower();
            InsertDelay(1.5f);
            //Debug.Log("AI used hero power");
            return true;
        }
        return false;
    }

    bool AttackWithAMonster()
    {
        foreach (MonsterLogic cl in p.table.MonstersOnTable)
        {
            if (cl.AttacksLeftThisTurn > 0)
            {
                // attack a random target with a monster
                if (p.otherPlayer.table.MonstersOnTable.Count > 0)
                {
                    int index = Random.Range(0, p.otherPlayer.table.MonstersOnTable.Count);
                    MonsterLogic targetMonster = p.otherPlayer.table.MonstersOnTable[index];
                    cl.AttackMonster(targetMonster);
                }                    
                else
                    cl.GoFace();
                
                InsertDelay(1f);
                //Debug.Log("AI attacked with monster");
                return true;
            }
        }
        return false;
    }

    void InsertDelay(float delay)
    {
        new DelayCommand(delay).AddToQueue();
    }

}
