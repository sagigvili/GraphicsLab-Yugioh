using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectOperator : MonoBehaviour
{
    public List<TargetSlot> Targets = new List<TargetSlot>();
    public Transform panel;
    public SpellTrapEffects CurrentEffect;

    public void InitiateEffect(bool TargetType, SpellTrapEffects effect)
    {
        CurrentEffect = effect;
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
                if ((CurrentEffect == SpellTrapEffects.ChangeToDefence && table.MonstersOnTable[i].monsterPosition == FieldPosition.Attack) || (CurrentEffect == SpellTrapEffects.ChangeToAttack && table.MonstersOnTable[i].monsterPosition == FieldPosition.Defence) || CurrentEffect == SpellTrapEffects.DestoryMonster)
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
                targetedMonster.Die();
                break;
            case SpellTrapEffects.DestorySpellTrap:
                SpellTrapLogic targetedSpellTrap = SpellTrapLogic.SpellTrapsCreatedThisGame[targetID];
                targetedSpellTrap.Die();
                break;
            case SpellTrapEffects.ChangeToAttack:
                MonsterLogic.MonstersCreatedThisGame[targetID].monsterPosition = FieldPosition.Attack;
                t.target.GetChild(6).GetComponent<Animator>().SetTrigger("Attack_State");
                StartCoroutine(ToAttackPosition(t.target.GetComponent<OneMonsterManager>().CardImageFront.transform.parent));
                break;
            case SpellTrapEffects.ChangeToDefence:
                MonsterLogic.MonstersCreatedThisGame[targetID].monsterPosition = FieldPosition.Defence;
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
