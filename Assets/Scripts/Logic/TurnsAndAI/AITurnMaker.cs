﻿using UnityEngine;
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
            while (Command.playingQueue) {
                yield return null;
            }
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

        return UseHeroPower() || PlaySpellOnField() || PlayATrapFromHand() || PlayASpellFromHand() || PlayAMonsterFromHand() || AttackWithAMonster();
    }

    bool PlayAMonsterFromHand()
    {
        int max = 0;
        foreach (CardLogic c in p.hand.CardsInHand)
        {
            if (c.ca.Attack > max)
            {
                max = c.ca.Attack;
            }
        }
        foreach (CardLogic c in p.hand.CardsInHand)
        {
            if (c.CanBePlayed && p.table.MonstersOnTable.Count < 3)
            {
                if(c.ca.Attack != -1 && !p.table.PlayedACard && c.ca.Attack == max)
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
                if (c.ca.SpellTrap == SpellOrTrap.Trap && c.ca.Attack == -1)
                {
                    // it is a trap card
                    int tablePos = p.PArea.tableVisual.getSpellsTrapsOnTableCountAI();
                    Debug.Log("Trap Table pos: " + tablePos);
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
                if (c.ca.SpellTrap == SpellOrTrap.Spell && c.ca.Attack == -1)
                {
                    // it is a trap card
                    int tablePos = p.PArea.tableVisual.getSpellsTrapsOnTableCountAI();
                    Debug.Log("Spell Table pos: " + tablePos);
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
        if (!p.usedPlayerPower && p.otherPlayer.table.SpellsTrapsOnTable.Count>0)
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
            if (cl.AttacksLeftThisTurn > 0 &&  cl.monsterPosition == FieldPosition.Attack)
            {
                // attack a random target with a monster
                if (p.otherPlayer.table.MonstersOnTable.Count > 0)
                {
                    
                    MonsterLogic targetMonster = SelectMonsterToAttack(cl.ca.Attack);
                    if (targetMonster != null)
                        cl.AttackMonster(targetMonster);
                    else
                        continue;
                }                    
                else
                    cl.GoFace();
                
                InsertDelay(1f);
                return true;
            }
        }
        return false;
    }

    public MonsterLogic SelectMonsterToAttack(int attackMonsterATK)
    {
        foreach (MonsterLogic cl in p.otherPlayer.table.MonstersOnTable)
        {
            if(cl.ca.monsterState == FieldPosition.Attack && cl.ca.Attack <= attackMonsterATK)
            {
                return cl;
            }
            if (cl.ca.monsterState == FieldPosition.Defence && cl.ca.Defence <= attackMonsterATK)
            {
                return cl;
            }
            if(cl.ca.monsterState == FieldPosition.Set)
            {
                int toAttack = Random.Range(0, 2);
                Debug.Log("is attack? " + toAttack);
                if (toAttack == 1)
                    return cl;
            }
        }
        return null;
    }

    bool PlaySpellOnField()
    {
        foreach (SpellTrapLogic stl in p.table.SpellsTrapsOnTable)
        {
            if (stl.ca.SpellTrap == SpellOrTrap.Spell && CanBeFaceUp(stl.Effect))
            {
                SpellTrapEffect.ActivateEffectAI(stl.ca);
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
