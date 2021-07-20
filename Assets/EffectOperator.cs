using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectOperator : MonoBehaviour
{
    public List<TargetSlot> Targets = new List<TargetSlot>();
    public Transform panel;
    public Effects CurrentEffect;

    public void InitiateEffect(bool TargetType, Effects effect)
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
                if ((CurrentEffect == Effects.ChangeToDefence && table.MonstersOnTable[i].monsterPosition == FieldPosition.Attack) || (CurrentEffect == Effects.ChangeToAttack && table.MonstersOnTable[i].monsterPosition == FieldPosition.Defence) || CurrentEffect == Effects.DestoryMonster)
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
        Transform target = t.target;
        int targetID = target.gameObject.GetComponent<IDHolder>().UniqueID;
        switch (CurrentEffect)
        {
            case Effects.DestoryMonster:
                MonsterLogic targetedMonster = MonsterLogic.MonstersCreatedThisGame[targetID];
                targetedMonster.Die();
                GameObject newMonsterOppField = GameObject.Instantiate(GlobalSettings.Instance.MonsterFieldPrefab, target.parent.transform.position, Quaternion.identity) as GameObject;
                newMonsterOppField.transform.SetParent(target.parent.transform);
                break;
            case Effects.DestorySpellTrap:
                SpellTrapLogic targetedSpellTrap = SpellTrapLogic.SpellTrapsCreatedThisGame[targetID];
                targetedSpellTrap.Die();
                GameObject newSpellOppTrapField = GameObject.Instantiate(GlobalSettings.Instance.SpellTrapFieldPrefab, target.parent.transform.position, Quaternion.identity) as GameObject;
                newSpellOppTrapField.transform.SetParent(target.parent.transform);
                break;
            case Effects.ChangeToAttack:
                MonsterLogic.MonstersCreatedThisGame[targetID].monsterPosition = FieldPosition.Attack;
                //t.transform.GetComponent<OneMonsterManager>().cardAsset.MonsterState = FieldPosition.Defence;
                ToAttackPosition(target);
                break;
            case Effects.ChangeToDefence:
                MonsterLogic.MonstersCreatedThisGame[targetID].monsterPosition = FieldPosition.Defence;
                //t.transform.GetComponent<OneMonsterManager>().cardAsset.MonsterState = FieldPosition.Attack;
                ToDefencePosition(target);
                break;
        }
        panel.gameObject.SetActive(false);
        GameObject newSpellTrapField = GameObject.Instantiate(GlobalSettings.Instance.SpellTrapFieldPrefab, this.transform.position, Quaternion.identity) as GameObject;
        newSpellTrapField.transform.SetParent(this.transform.parent);
        int spellID = this.gameObject.GetComponent<IDHolder>().UniqueID;
        TurnManager.Instance.whoseTurn.otherPlayer.PArea.tableVisual.RemoveSpellTrapWithID(spellID);
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
