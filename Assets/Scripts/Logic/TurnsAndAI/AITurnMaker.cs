using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//this class will take all decisions for AI. 

public class AITurnMaker: TurnMaker {
    public PhasesActions pa;
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
        if (Random.Range(0, 2) == 0 && p.table.MonstersOnTable.Count > 0)
            strategyAttackFirst = true;

        while (MakeOneAIMove(strategyAttackFirst))
        {
            yield return null;
        }

        InsertDelay(1f);
        GlobalSettings.Instance.EndTurnButton.GetComponentInChildren<Text>().text = "Attack Phase";
        pa.SetFlag(2);
        pa.EndTurn();
    }

    bool MakeOneAIMove(bool attackFirst)
    {
        if (Command.CardDrawPending())
            return true;
        else if (attackFirst)
            return AttackWithAMonster() || PlayAMonsterFromHand();
        else 
            return PlayATrapFromHand() || PlayASpellFromHand() || PlayAMonsterFromHand() || AttackWithAMonster();
    }

    bool PlayAMonsterFromHand()
    {
        foreach (CardLogic c in p.hand.CardsInHand)
        {
            if (c.CanBePlayed && p.table.MonstersOnTable.Count < 3)
            {
                if(c.ca.Attack != -1 && !p.table.PlayedACard)
                {
                    // it is a monster card
                    p.table.PlayedACard = true;
                    int tablePos = p.PArea.tableVisual.getMonstersOnTableCount();
                    p.PlayAMonsterFromHand(c, tablePos, FieldPosition.Attack);
                    InsertDelay(1.5f);
                    if (GlobalSettings.Instance.EndTurnButton.GetComponentInChildren<Text>().text != "End Turn")
                    {
                        pa.SetFlag(2);
                        pa.ClickByFlag();
                    }
                    return true;
                }

            }
        }
        return false;
    }

    bool PlayATrapFromHand()
    {
        foreach (CardLogic c in p.hand.CardsInHand)
        {
            if (c.CanBePlayed && p.table.SpellsTrapsOnTable.Count < 3)
            {
                if (c.ca.SpellTrap == SpellOrTrap.Trap)
                {
                    // it is a trap card
                    int tablePos = p.PArea.tableVisual.getSpellsTrapsOnTableCount();
                    p.PlayASpellFromHand(c, tablePos, SpellTrapPosition.Set);
                    InsertDelay(1.5f);
                    return true;
                }

            }
        }
        return false;
    }

    bool PlayASpellFromHand()
    {
        foreach (CardLogic c in p.hand.CardsInHand)
        {
            if (c.CanBePlayed && p.table.SpellsTrapsOnTable.Count < 3)
            {
                if (c.ca.SpellTrap == SpellOrTrap.Spell)
                {
                    // it is a trap card
                    int tablePos = p.PArea.tableVisual.getSpellsTrapsOnTableCount();
                    if (CanBeFaceUp(c.ca.Effect))
                        p.PlayASpellFromHand(c, tablePos, SpellTrapPosition.FaceUp);
                    else
                        p.PlayASpellFromHand(c, tablePos, SpellTrapPosition.Set);
                    InsertDelay(1.5f);
                    return true;
                }
                else
                {

                }

            }
        }
        return false;
    }

    bool CanBeFaceUp(SpellTrapEffects effect)
    {
        switch (effect)
        {
            case SpellTrapEffects.DestoryMonster:
                if (p.otherPlayer.table.MonstersOnTable.Count > 0)
                    return true;
                return false;
            case SpellTrapEffects.DestorySpellTrap:
                if (p.otherPlayer.table.SpellsTrapsOnTable.Count > 0)
                    return true;
                return false;
            case SpellTrapEffects.ChangeToAttack:
                if (p.otherPlayer.table.SpellsTrapsOnTable.Count == 0)
                    return false;
                foreach (SpellTrapLogic stl in p.otherPlayer.table.SpellsTrapsOnTable)
                    if (stl.ca.MonsterState == FieldPosition.Defence)
                        return true;
                return false;
            case SpellTrapEffects.ChangeToDefence:
                if (p.otherPlayer.table.SpellsTrapsOnTable.Count == 0)
                    return false;
                foreach (SpellTrapLogic stl in p.otherPlayer.table.SpellsTrapsOnTable)
                    if (stl.ca.MonsterState == FieldPosition.Attack)
                        return true;
                return false;
            case SpellTrapEffects.Revive:
                if (p.otherPlayer.graveyard.cards.Count == 0)
                    return false;
                return true;
            case SpellTrapEffects.Heal:
                return true;
            case SpellTrapEffects.DirectAttack:
                return true;
            case SpellTrapEffects.Draw:
                return true;
        }
        return false;
    }

    bool UseHeroPower()
    {
        if (!p.usedPlayerPower)
        {
            p.UsePlayerPower();
            InsertDelay(1.5f);

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
