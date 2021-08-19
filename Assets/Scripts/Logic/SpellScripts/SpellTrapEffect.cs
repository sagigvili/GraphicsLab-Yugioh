using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellTrapEffect 
{
    static public void ActivateEffect(SpellTrapLogic stl)
    {
        Player p = TurnManager.Instance.whoseTurn;
        CardAsset ca = stl.ca;
        switch (ca.Effect)
        {
            case SpellTrapEffects.DestoryMonster:
                GameObject tsDM = GameObject.Instantiate(GlobalSettings.Instance.TargetSelectorPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                tsDM.GetComponent<EffectOperator>().InitiateEffect(true, SpellTrapEffects.DestoryMonster, stl);
                tsDM.SetActive(true);
                break;
            case SpellTrapEffects.DestorySpellTrap:
                GameObject tsDST = GameObject.Instantiate(GlobalSettings.Instance.TargetSelectorPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                tsDST.GetComponent<EffectOperator>().InitiateEffect(false, SpellTrapEffects.DestorySpellTrap, stl);
                tsDST.SetActive(true);
                break;
            case SpellTrapEffects.DirectAttack:
                int hp_amount = System.Int32.Parse(p.otherPlayer.PArea.Portrait.HealthText.text) - ca.amount;
                if (hp_amount < 0)
                    hp_amount = 0;
                string final_hp_other_amount = hp_amount.ToString();
                Vector3 posD = p.otherPlayer.PArea.Portrait.HealPoint.position;
                posD = new Vector3(posD.x, posD.y, 628);
                p.otherPlayer.PArea.Portrait.Model.GetComponent<Animator>().SetTrigger("GetDamaged");
                GameObject attackPartEffect = GameObject.Instantiate(ca.AttackEffect, posD, Quaternion.identity) as GameObject;
                ParticleSystem psD = attackPartEffect.GetComponent<ParticleSystem>();
                psD.Play();
                GameObject.Destroy(attackPartEffect, psD.main.duration);
                Transform t1 = p.otherPlayer.PArea.DamageSpot;
                DamageEffect.CreateDamageEffect(t1.position, ca.amount);
                p.otherPlayer.PArea.Portrait.HealthText.text = final_hp_other_amount;
                p.otherPlayer.Health -= ca.amount;
                stl.Die();
                break;
            case SpellTrapEffects.Draw:
                p.DrawACard();
                p.DrawACard();
                stl.Die();
                break;
            case SpellTrapEffects.Heal:
                string final_hp_amount = (System.Int32.Parse(p.PArea.Portrait.HealthText.text) + ca.amount).ToString();
                Vector3 pos = p.PArea.Portrait.HealPoint.position;
                pos = new Vector3(pos.x, pos.y, 628);
                p.PArea.Portrait.Model.GetComponent<Animator>().SetTrigger("Heal");
                GameObject healPartEffect = GameObject.Instantiate(ca.LoadEffect, pos, Quaternion.identity) as GameObject;
                ParticleSystem ps = healPartEffect.GetComponent<ParticleSystem>();
                ps.Play();
                GameObject.Destroy(healPartEffect, 3.0f);
                new DelayCommand(1.5f).AddToQueue();
                Transform t = p.PArea.DamageSpot;
                DamageEffect.CreateDamageEffect(t.position, ca.amount, false);
                p.PArea.Portrait.HealthText.text = final_hp_amount;
                p.Health += ca.amount;
                stl.Die();
                break;
            case SpellTrapEffects.Negate:
                break;
            case SpellTrapEffects.ChangeToAttack:
                GameObject tsPCA = GameObject.Instantiate(GlobalSettings.Instance.TargetSelectorPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                tsPCA.GetComponent<EffectOperator>().InitiateEffect(true, SpellTrapEffects.ChangeToAttack, stl);
                tsPCA.SetActive(true);
                break;
            case SpellTrapEffects.ChangeToDefence:
                GameObject tsPCB = GameObject.Instantiate(GlobalSettings.Instance.TargetSelectorPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                tsPCB.GetComponent<EffectOperator>().InitiateEffect(true, SpellTrapEffects.ChangeToDefence, stl);
                tsPCB.SetActive(true);
                break;
            case SpellTrapEffects.Revive:
                p.PArea.graveyardVisual.CardsCanvas.SetActive(true);
                stl.Die();
                break;
        }
    }


    static public void ActivateEffectAI(CardAsset ca)
    {
        Player p = TurnManager.Instance.whoseTurn;
        switch (ca.Effect)
        {
            case SpellTrapEffects.DestoryMonster:
                int max = 0;
                int index = -1;
                int i = 0;
                foreach(MonsterLogic ml in p.otherPlayer.table.MonstersOnTable)
                {
                    if (ml.ca.Attack > max)
                    {
                        max = ml.ca.Attack;
                        index = i;
                    }
                    i++;
                }
                Transform trans = IDHolder.GetGameObjectWithID(p.otherPlayer.table.GetMonsterAtIndex(index).ID).transform;
                Vector3 destroyEffectPos = new Vector3(trans.position.x + 2000, trans.position.y, 500);
                GameObject destroyEffectObj = GameObject.Instantiate(ca.AttackEffect, destroyEffectPos, Quaternion.identity) as GameObject;
                GameObject.Destroy(destroyEffectObj, 5.0f);
                new DelayCommand(5.0f).AddToQueue();
                p.otherPlayer.table.GetMonsterAtIndex(index).Die();
                break;
            case SpellTrapEffects.DestorySpellTrap:
                int i2 = Random.Range(0, 2);
                while (!p.otherPlayer.table.GetSpellTrapAtIndex(i2).ca)
                    i2 = Random.Range(0, 2);
                Transform trans1 = IDHolder.GetGameObjectWithID(p.otherPlayer.table.GetSpellTrapAtIndex(i2).ID).transform;
                Vector3 destroyEffectPos1 = new Vector3(trans1.position.x - 100, trans1.position.y - 600, trans1.position.z);
                GameObject destroyEffectObj1 = GameObject.Instantiate(ca.AttackEffect, destroyEffectPos1, Quaternion.identity) as GameObject;
                ParticleSystem psDST = destroyEffectObj1.transform.GetComponent<ParticleSystem>();
                psDST.Play();
                GameObject.Destroy(destroyEffectObj1, psDST.main.duration);
                new DelayCommand(psDST.main.duration - 0.5f).AddToQueue();
                p.otherPlayer.table.GetSpellTrapAtIndex(i2).Die();
                break;
            case SpellTrapEffects.DirectAttack:
                int hp_amount = System.Int32.Parse(p.otherPlayer.PArea.Portrait.HealthText.text) - ca.amount;
                if (hp_amount < 0)
                    hp_amount = 0;
                string final_hp_other_amount = hp_amount.ToString();
                Vector3 posD = p.otherPlayer.PArea.Portrait.HealPoint.position;
                posD = new Vector3(posD.x, posD.y, 628);
                p.otherPlayer.PArea.Portrait.Model.GetComponent<Animator>().SetTrigger("GetDamaged");
                GameObject attackPartEffect = GameObject.Instantiate(ca.AttackEffect, posD, Quaternion.identity) as GameObject;
                ParticleSystem psD = attackPartEffect.GetComponent<ParticleSystem>();
                psD.Play();
                GameObject.Destroy(attackPartEffect, psD.main.duration);
                Transform t1 = p.otherPlayer.PArea.DamageSpot;
                DamageEffect.CreateDamageEffect(t1.position, ca.amount);
                p.otherPlayer.PArea.Portrait.HealthText.text = final_hp_other_amount;
                p.otherPlayer.Health -= ca.amount;
                break;
            case SpellTrapEffects.Draw:
                p.DrawACard();
                p.DrawACard();
                break;
            case SpellTrapEffects.Heal:
                string final_hp_amount = (System.Int32.Parse(p.PArea.Portrait.HealthText.text) + ca.amount).ToString();
                Vector3 pos = p.PArea.Portrait.HealPoint.position;
                p.PArea.Portrait.Model.GetComponent<Animator>().SetTrigger("Heal");
                pos = new Vector3(pos.x, pos.y, 500);
                GameObject healPartEffect = GameObject.Instantiate(ca.LoadEffect, pos, Quaternion.identity) as GameObject;
                ParticleSystem ps = healPartEffect.GetComponent<ParticleSystem>();
                ps.Play();
                GameObject.Destroy(healPartEffect, 3.0f);
                new DelayCommand(0.5f).AddToQueue();
                Transform t = p.PArea.DamageSpot;
                DamageEffect.CreateDamageEffect(t.position, ca.amount, false);
                p.PArea.Portrait.HealthText.text = final_hp_amount;
                p.Health += ca.amount;
                break;
            case SpellTrapEffects.Negate:
                break;
            case SpellTrapEffects.ChangeToAttack:
                List<int> DefMonstersIndexes = new List<int>();
                int i7 = 0;
                int count = 0;
                foreach (MonsterLogic ml in p.otherPlayer.table.MonstersOnTable)
                {
                    if (ml.ca.MonsterState == FieldPosition.Defence)
                    {
                        DefMonstersIndexes.Insert(count,i7);
                        count++;
                    }
                    i7++;
                }
                int toChangeindex = Random.Range(0, count-1);
                int tochange = DefMonstersIndexes[toChangeindex];
                Debug.Log(tochange);
                p.otherPlayer.table.GetMonsterAtIndex(tochange).ChangeState(FieldPosition.Attack);
                break;
            case SpellTrapEffects.ChangeToDefence:
                List<int> AtkMonstersIndexes = new List<int>();
                int i8 = 0;
                int count1 = 0;
                foreach (MonsterLogic ml in p.otherPlayer.table.MonstersOnTable)
                {
                    if (ml.ca.MonsterState == FieldPosition.Attack)
                    {
                        AtkMonstersIndexes.Insert(count1, i8);
                        count1++;
                    }
                    i8++;
                }
                int toChangeindex1 = Random.Range(0, count1-1);
                int tochange1 = AtkMonstersIndexes[toChangeindex1];
                Debug.Log(tochange1);
                p.otherPlayer.table.GetMonsterAtIndex(tochange1).ChangeState(FieldPosition.Defence);
                break;
            case SpellTrapEffects.Revive:
                p.PArea.graveyardVisual.GetRandomCardFromGraveyard().GetComponent<OneCardManager>().NewCardFromGraveYard();
                new DelayCommand(1.3f).AddToQueue();
                break;

        }
    }


    

}


