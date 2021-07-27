using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MonsterAttackVisual : MonoBehaviour 
{
    private OneMonsterManager manager;
    private WhereIsTheCardOrMonster w;
    private GameObject target;

    void Awake()
    {
        manager = GetComponent<OneMonsterManager>();    
        w = GetComponent<WhereIsTheCardOrMonster>();
    }

    public void AttackTarget(int targetUniqueID, int damageTakenByTarget, int damageTakenByAttacker, int targetHPAfter)
    {
        manager.CanAttackNow = false;
        // bring this monster to front sorting-wise.
        target = IDHolder.GetGameObjectWithID(targetUniqueID);
        w.BringToFront();
        VisualStates tempState = w.VisualState;
        w.VisualState = VisualStates.Transition;
        Vector3 orig_pos = transform.GetChild(6).transform.position;
        transform.GetChild(6).GetComponent<Animator>().SetTrigger("Attack");
        TurnManager.Instance.whoseTurn.PArea.Portrait.Model.GetComponent<Animator>().SetTrigger("Attack");
        GameObject newPartEffect = GameObject.Instantiate(manager.loadEffect, transform.GetChild(6).transform.position, Quaternion.identity) as GameObject;
        ParticleSystem ps = newPartEffect.GetComponentInChildren<ParticleSystem>();
        ps.Play();
        Destroy(newPartEffect, 3);
        StartCoroutine(AttackContinue(targetUniqueID, damageTakenByTarget, damageTakenByAttacker, targetHPAfter, tempState, orig_pos));
    }

    IEnumerator AttackContinue(int targetUniqueID, int damageTakenByTarget, int damageTakenByAttacker, int targetHPAfter, VisualStates tempState, Vector3 pos)
    {
        yield return new WaitForSeconds(1);
        if (manager.cardAsset.Type == MonsterType.Spellcaster)
        {
            GameObject newShotEffect = GameObject.Instantiate(manager.shotEffect, transform.GetChild(6).transform.position, Quaternion.identity) as GameObject;
            for (int i = 0; i < newShotEffect.transform.childCount; i++)
                newShotEffect.transform.GetChild(i).GetComponent<ParticleSystem>().Play();
            newShotEffect.transform.DOMove(target.transform.position, 2).OnComplete(() =>
            {
                Destroy(newShotEffect);
                FinishedAttack(targetUniqueID, damageTakenByTarget, damageTakenByAttacker, targetHPAfter, tempState, pos);
            });
        }
        else
        {
            transform.GetChild(6).DOMove(target.transform.position, 0.5f).OnComplete(() => {
                FinishedAttack(targetUniqueID, damageTakenByTarget, damageTakenByAttacker, targetHPAfter, tempState, pos);
            });
        }
        yield return null;
    }
    public void FinishedAttack(int targetUniqueID, int damageTakenByTarget, int damageTakenByAttacker, int targetHPAfter, VisualStates tempState, Vector3 pos)
    {
        if (targetHPAfter < 0)
            targetHPAfter = 0;

        if (targetUniqueID == GlobalSettings.Instance.LowPlayer.PlayerID || targetUniqueID == GlobalSettings.Instance.TopPlayer.PlayerID)
        {
            // target is a player
            target.GetComponent<PlayerPortraitVisual>().HealthText.text = targetHPAfter.ToString();
        }


        if (damageTakenByTarget > 0)
        {
            GameObject newPartEffect = GameObject.Instantiate(manager.attackEffect, target.transform.position, Quaternion.identity) as GameObject;
            ParticleSystem ps = newPartEffect.GetComponent<ParticleSystem>();
            newPartEffect.transform.position = new Vector3(newPartEffect.transform.position.x, newPartEffect.transform.position.y, newPartEffect.transform.position.z - 50);
            ps.Play();
            Destroy(newPartEffect, ps.main.duration);
            TurnManager.Instance.whoseTurn.otherPlayer.PArea.Portrait.Model.GetComponent<Animator>().SetTrigger("GetDamaged");
            transform.DOMove(transform.position, 0.5f).OnComplete(() =>
            {
                Transform t = TurnManager.Instance.whoseTurn.otherPlayer.PArea.DamageSpot;
                DamageEffect.CreateDamageEffect(t.position, damageTakenByTarget);
            });
        }

        if (damageTakenByAttacker > 0)
        {
            TurnManager.Instance.whoseTurn.PArea.Portrait.Model.GetComponent<Animator>().SetTrigger("GetDamaged");
            transform.DOMove(transform.position, 0.5f).OnComplete(() =>
            {
                Transform t = TurnManager.Instance.whoseTurn.PArea.DamageSpot;
                DamageEffect.CreateDamageEffect(t.position, damageTakenByAttacker);
            });
        }

        w.SetTableSortingOrder();
        w.VisualState = tempState;

        Sequence s = DOTween.Sequence();
        s.AppendInterval(1f);
        s.OnComplete(Command.CommandExecutionComplete);
        if (manager.cardAsset.Type != MonsterType.Spellcaster)
        {
            transform.GetChild(6).DOMove(pos, 0.5f).OnComplete(() =>
            {
                return;
            });
        }     
        
    }
}
