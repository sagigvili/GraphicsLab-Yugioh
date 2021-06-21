using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MonsterAttackVisual : MonoBehaviour 
{
    private OneMonsterManager manager;
    private WhereIsTheCardOrMonster w;

    void Awake()
    {
        manager = GetComponent<OneMonsterManager>();    
        w = GetComponent<WhereIsTheCardOrMonster>();
    }

    public void AttackTarget(int targetUniqueID, int damageTakenByTarget, int damageTakenByAttacker, int targetHPAfter)
    {
        Debug.Log(targetUniqueID);
        manager.CanAttackNow = false;
        GameObject target = IDHolder.GetGameObjectWithID(targetUniqueID);

        // bring this creature to front sorting-wise.
        w.BringToFront();
        VisualStates tempState = w.VisualState;
        w.VisualState = VisualStates.Transition;

        transform.DOMove(target.transform.position, 0.5f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InCubic).OnComplete(() =>
            {
                if(damageTakenByTarget>0)
                    DamageEffect.CreateDamageEffect(target.transform.position, damageTakenByTarget);
                if(damageTakenByAttacker>0)
                    DamageEffect.CreateDamageEffect(transform.position, damageTakenByAttacker);
                
                if (targetUniqueID == GlobalSettings.Instance.LowPlayer.PlayerID || targetUniqueID == GlobalSettings.Instance.TopPlayer.PlayerID)
                {
                    // target is a player
                    target.GetComponent<PlayerPortraitVisual>().HealthText.text = targetHPAfter.ToString();
                }

                w.SetTableSortingOrder();
                w.VisualState = tempState;

                Sequence s = DOTween.Sequence();
                s.AppendInterval(1f);
                s.OnComplete(Command.CommandExecutionComplete);
                //Command.CommandExecutionComplete();
            });
    }
        
}
