using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class SpellTrapEffect 
{
    static public void ActivateEffect(CardAsset ca)
    {
        switch (ca.Effect)
        {
            case SpellTrapEffects.DestoryMonster:
                GameObject tsDM = GameObject.Instantiate(GlobalSettings.Instance.TargetSelectorPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                tsDM.GetComponent<EffectOperator>().InitiateEffect(true, SpellTrapEffects.DestoryMonster);
                tsDM.SetActive(true);
                break;
            case SpellTrapEffects.DestorySpellTrap:
                GameObject tsDST = GameObject.Instantiate(GlobalSettings.Instance.TargetSelectorPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                tsDST.GetComponent<EffectOperator>().InitiateEffect(false, SpellTrapEffects.DestorySpellTrap);
                tsDST.SetActive(true);
                break;
            case SpellTrapEffects.DirectAttack:
                int hp_amount = System.Int32.Parse(TurnManager.Instance.whoseTurn.otherPlayer.PArea.Portrait.HealthText.text) - ca.amount;
                if (hp_amount < 0)
                    hp_amount = 0;
                string final_hp_other_amount = hp_amount.ToString();
                TurnManager.Instance.whoseTurn.otherPlayer.PArea.Portrait.HealthText.text = final_hp_other_amount;
                TurnManager.Instance.whoseTurn.otherPlayer.Health -= ca.amount;
                break;
            case SpellTrapEffects.Draw:
                TurnManager.Instance.whoseTurn.DrawACard();
                TurnManager.Instance.whoseTurn.DrawACard();
                break;
            case SpellTrapEffects.Heal:
                string final_hp_amount = (System.Int32.Parse(TurnManager.Instance.whoseTurn.PArea.Portrait.HealthText.text) + ca.amount).ToString();
                TurnManager.Instance.whoseTurn.PArea.Portrait.HealthText.text = final_hp_amount;
                TurnManager.Instance.whoseTurn.Health += ca.amount;
                break;
            case SpellTrapEffects.Negate:
                break;
            case SpellTrapEffects.ChangeToAttack:
                GameObject tsPCA = GameObject.Instantiate(GlobalSettings.Instance.TargetSelectorPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                tsPCA.GetComponent<EffectOperator>().InitiateEffect(true, SpellTrapEffects.ChangeToAttack);
                tsPCA.SetActive(true);
                break;
            case SpellTrapEffects.ChangeToDefence:
                GameObject tsPCB = GameObject.Instantiate(GlobalSettings.Instance.TargetSelectorPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                tsPCB.GetComponent<EffectOperator>().InitiateEffect(true, SpellTrapEffects.ChangeToDefence);
                tsPCB.SetActive(true);
                break;
            case SpellTrapEffects.Revive:
                TurnManager.Instance.whoseTurn.PArea.graveyardVisual.CardsCanvas.SetActive(true);
                break;

        }
    }


    static public void ActivateEffectAI(CardAsset ca)
    {
        switch (ca.Effect)
        {
            case SpellTrapEffects.DestoryMonster:
                int max = 0;
                int index = -1;
                int i = 0;
                foreach(MonsterLogic ml in TurnManager.Instance.whoseTurn.otherPlayer.table.MonstersOnTable)
                {
                    if (ml.ca.Attack > max)
                    {
                        max = ml.ca.Attack;
                        index = i;
                    }
                    i++;
                }
                TurnManager.Instance.whoseTurn.otherPlayer.table.GetMonsterAtIndex(index).Die();
                break;
            case SpellTrapEffects.DestorySpellTrap:
                int i2 = Random.Range(0, TurnManager.Instance.whoseTurn.otherPlayer.table.SpellsTrapsOnTable.Count-1);
                TurnManager.Instance.whoseTurn.otherPlayer.table.GetSpellTrapAtIndex(i2).Die();
                break;
            case SpellTrapEffects.DirectAttack:
                int hp_amount = System.Int32.Parse(TurnManager.Instance.whoseTurn.otherPlayer.PArea.Portrait.HealthText.text) - ca.amount;
                if (hp_amount < 0)
                    hp_amount = 0;
                string final_hp_other_amount = hp_amount.ToString();
                TurnManager.Instance.whoseTurn.otherPlayer.PArea.Portrait.HealthText.text = final_hp_other_amount;
                TurnManager.Instance.whoseTurn.otherPlayer.Health -= ca.amount;
                break;
            case SpellTrapEffects.Draw:
                TurnManager.Instance.whoseTurn.DrawACard();
                TurnManager.Instance.whoseTurn.DrawACard();
                break;
            case SpellTrapEffects.Heal:
                string final_hp_amount = (System.Int32.Parse(TurnManager.Instance.whoseTurn.PArea.Portrait.HealthText.text) + ca.amount).ToString();
                TurnManager.Instance.whoseTurn.PArea.Portrait.HealthText.text = final_hp_amount;
                TurnManager.Instance.whoseTurn.Health += ca.amount;
                break;
            case SpellTrapEffects.Negate:
                break;
            case SpellTrapEffects.ChangeToAttack:
                List<int> DefMonstersIndexes = new List<int>();
                int i7 = 0;
                int count = 0;
                foreach (MonsterLogic ml in TurnManager.Instance.whoseTurn.otherPlayer.table.MonstersOnTable)
                {
                    if (ml.ca.monsterState == FieldPosition.Defence)
                    {
                        DefMonstersIndexes.Insert(count,i7);
                        count++;
                    }
                    i7++;
                }
                int toChangeindex = Random.Range(0, count);
                int tochange = DefMonstersIndexes[toChangeindex];
                TurnManager.Instance.whoseTurn.otherPlayer.table.GetMonsterAtIndex(tochange).ChangeState(FieldPosition.Attack);
                break;
            case SpellTrapEffects.ChangeToDefence:
                List<int> AtkMonstersIndexes = new List<int>();
                int i8 = 0;
                int count1 = 0;
                foreach (MonsterLogic ml in TurnManager.Instance.whoseTurn.otherPlayer.table.MonstersOnTable)
                {
                    if (ml.ca.monsterState == FieldPosition.Defence)
                    {
                        AtkMonstersIndexes.Insert(count1, i8);
                        count1++;
                    }
                    i8++;
                }
                int toChangeindex1 = Random.Range(0, count1);
                int tochange1 = AtkMonstersIndexes[toChangeindex1];
                TurnManager.Instance.whoseTurn.otherPlayer.table.GetMonsterAtIndex(tochange1).ChangeState(FieldPosition.Defence);
                break;
            case SpellTrapEffects.Revive:
                TurnManager.Instance.whoseTurn.PArea.graveyardVisual.CardsCanvas.SetActive(true);
                break;

        }
    }


    

}


