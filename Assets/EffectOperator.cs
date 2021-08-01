using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectOperator : MonoBehaviour
{
    public List<TargetSlot> Targets = new List<TargetSlot>();
    public Transform panel;
    public SpellTrapEffects CurrentEffect;
    public CardAsset caForEffect;

    public void InitiateEffect(bool TargetType, SpellTrapEffects effect, CardAsset _caForEffect = null)
    {
        CurrentEffect = effect;
        caForEffect = _caForEffect;
        getTargets(TargetType);
    }
    public void getTargets(bool isTargetMonsters)
    {
        TableVisual tabVis = TurnManager.Instance.whoseTurn.otherPlayer.PArea.tableVisual;
        Table table = TurnManager.Instance.whoseTurn.otherPlayer.table;
        if (isTargetMonsters)
        {
            for (int i = 0; i < tabVis.getMonstersOnTableCount(); i++)
            {
                if ((CurrentEffect == SpellTrapEffects.ChangeToDefence && table.MonstersOnTable[i].ca.MonsterState == FieldPosition.Attack) || (CurrentEffect == SpellTrapEffects.ChangeToAttack && table.MonstersOnTable[i].ca.MonsterState == FieldPosition.Defence) || CurrentEffect == SpellTrapEffects.DestoryMonster)
                {
                    Targets[i].getTargetDetails(tabVis.getMonsterOnTable(i));
                    Targets[i].gameObject.SetActive(true);
                }

            }
        }
        else
        {
            for (int i = 0; i < tabVis.getSpellsTrapsOnTableCount(); i++)
            {
                Targets[i].getTargetDetails(tabVis.getSpellTrapOnTable(i));
                Targets[i].gameObject.SetActive(true);
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
                GameObject destroyEffectObj = GameObject.Instantiate(caForEffect.AttackEffect, destroyEffectPos, Quaternion.identity) as GameObject;
                GameObject.Destroy(destroyEffectObj, 3.0f);
                new DelayCommand(1.0f).AddToQueue();
                targetedMonster.Die();
                break;
            case SpellTrapEffects.DestorySpellTrap:
                SpellTrapLogic targetedSpellTrap = SpellTrapLogic.SpellTrapsCreatedThisGame[targetID];
                Transform trans1 = IDHolder.GetGameObjectWithID(targetID).transform;
                Vector3 destroyEffectPos1 = new Vector3(trans1.position.x, trans1.position.y, trans1.position.z - 20);
                GameObject destroyEffectObj1 = GameObject.Instantiate(caForEffect.AttackEffect, destroyEffectPos1, Quaternion.identity) as GameObject;
                ParticleSystem psDST = destroyEffectObj1.GetComponent<ParticleSystem>();
                psDST.Play();
                GameObject.Destroy(destroyEffectObj1, psDST.main.duration);
                new DelayCommand(1.0f).AddToQueue();
                targetedSpellTrap.Die();
                break;
            case SpellTrapEffects.ChangeToAttack:
                MonsterLogic.MonstersCreatedThisGame[targetID].ca.MonsterState = FieldPosition.Attack;
                t.target.GetChild(6).GetComponent<Animator>().SetTrigger("Attack_State");
                StartCoroutine(ToAttackPosition(t.target.GetComponent<OneMonsterManager>().CardImageFront.transform.parent));
                break;
            case SpellTrapEffects.ChangeToDefence:
                MonsterLogic.MonstersCreatedThisGame[targetID].ca.MonsterState = FieldPosition.Defence;
                t.target.GetChild(6).GetComponent<Animator>().SetTrigger("Defence_State");
                StartCoroutine(ToDefencePosition(t.target.GetComponent<OneMonsterManager>().CardImageFront.transform.parent));
                break;
        }
        panel.gameObject.SetActive(false);
    }

    /// <summary>
    /// flip to the front
    /// </summary>
    IEnumerator ToAttackPosition(Transform t)
    {
        // displace the card so that we can select it in the scene easier.
        t.DORotate(new Vector3(0, 0, 0), 1);
        for (float i = 1; i >= 0; i -= Time.deltaTime)
            yield return 0;
    }

    IEnumerator ToDefencePosition(Transform t)
    {
        t.DORotate(new Vector3(0, 0, 90), 1);
        for (float i = 1; i >= 0; i -= Time.deltaTime)
            yield return 0;
    }
}
