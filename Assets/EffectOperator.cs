using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectOperator : MonoBehaviour
{
    public List<TargetSlot> Targets = new List<TargetSlot>();
    public Transform panel;
    private SpellTrapEffects CurrentEffect;
    private SpellTrapLogic stl;

    public void InitiateEffect(bool TargetType, SpellTrapEffects effect, SpellTrapLogic _stl = null)
    {
        CurrentEffect = effect;
        stl = _stl;
        getTargets(TargetType);
    }
    public void getTargets(bool isTargetMonsters)
    {
        TableVisual tabVis = TurnManager.Instance.whoseTurn.otherPlayer.PArea.tableVisual;
        Table table = TurnManager.Instance.whoseTurn.otherPlayer.table;
        if (isTargetMonsters)
        {
            for (int i = 0; i < 3; i++)
            {
                if (tabVis.doesMonsterIndexExist(i))
                {
                    if ((CurrentEffect == SpellTrapEffects.ChangeToDefence && table.MonstersOnTable[i].ca.MonsterState == FieldPosition.Attack) || (CurrentEffect == SpellTrapEffects.ChangeToAttack && table.MonstersOnTable[i].ca.MonsterState == FieldPosition.Defence) || CurrentEffect == SpellTrapEffects.DestoryMonster)
                    {
                        Targets[i].getTargetDetails(tabVis.getMonsterOnTable(i));
                        Targets[i].gameObject.SetActive(true);
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                if (tabVis.doesSpellTrapIndexExist(i))
                {
                    Targets[i].getTargetDetails(tabVis.getSpellTrapOnTable(i));
                    Targets[i].gameObject.SetActive(true);
                }
            }
        }
 
    }

    public void ActivateEffect(TargetSlot t)
    {
        int targetID = t.target.gameObject.GetComponent<IDHolder>().UniqueID;
        switch (CurrentEffect)
        {
            case SpellTrapEffects.DestoryMonster:
                MonsterLogic targetedMonster = MonsterLogic.MonstersCreatedThisGame[targetID];
                Transform trans = IDHolder.GetGameObjectWithID(targetID).transform;
                Vector3 destroyEffectPos = new Vector3(trans.position.x + 2000, trans.position.y, 500);
                GameObject destroyEffectObj = GameObject.Instantiate(stl.ca.AttackEffect, destroyEffectPos, Quaternion.identity) as GameObject;
                GameObject.Destroy(destroyEffectObj, 5.0f);
                new DelayCommand(5.0f).AddToQueue();
                targetedMonster.Die();
                break;
            case SpellTrapEffects.DestorySpellTrap:
                SpellTrapLogic targetedSpellTrap = SpellTrapLogic.SpellTrapsCreatedThisGame[targetID];
                Transform trans1 = IDHolder.GetGameObjectWithID(targetID).transform;
                Vector3 destroyEffectPos1 = new Vector3(trans1.position.x - 100, trans1.position.y - 600, trans1.position.z);
                GameObject destroyEffectObj1 = GameObject.Instantiate(stl.ca.AttackEffect, destroyEffectPos1, Quaternion.identity) as GameObject;
                ParticleSystem psDST = destroyEffectObj1.transform.GetComponent<ParticleSystem>();
                psDST.Play();
                GameObject.Destroy(destroyEffectObj1, psDST.main.duration);
                new DelayCommand(psDST.main.duration - 0.5f).AddToQueue();
                targetedSpellTrap.Die();
                break;
            case SpellTrapEffects.ChangeToAttack:
                MonsterLogic.MonstersCreatedThisGame[targetID].ChangeState(FieldPosition.Attack);
                break;
            case SpellTrapEffects.ChangeToDefence:
                MonsterLogic.MonstersCreatedThisGame[targetID].ChangeState(FieldPosition.Defence);
                break;
        }
        panel.gameObject.SetActive(false);
        stl.Die();
    }
}
