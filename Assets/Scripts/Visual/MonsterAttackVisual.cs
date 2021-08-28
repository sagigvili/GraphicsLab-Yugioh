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
        // Loading visualization before attacking
        GameObject newPartEffect = GameObject.Instantiate(manager.loadEffect, transform.GetChild(6).transform.position, Quaternion.identity) as GameObject;
        ParticleSystem ps = newPartEffect.GetComponentInChildren<ParticleSystem>();
        ps.Play();
        Destroy(newPartEffect, 3);

        // Exectue attack visualization
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
        else if (manager.cardAsset.Attribute == MonsterAttribute.Dark)
        {
            transform.DOMove(transform.position, 0.8f).OnComplete(() =>
            {
                GameObject newShotEffect = GameObject.Instantiate(manager.shotEffect, transform.GetChild(6).transform.position, Quaternion.identity) as GameObject;
                newShotEffect.transform.DOMove(target.transform.position, 2f).OnComplete(() =>
                {
                    Destroy(newShotEffect);
                    FinishedAttack(targetUniqueID, damageTakenByTarget, damageTakenByAttacker, targetHPAfter, tempState, pos);
                });
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

        if (isItPlayerTarget(targetUniqueID))
        {
            // target is a player
            target.GetComponent<PlayerPortraitVisual>().HealthText.text = targetHPAfter.ToString();
        }

        if (!isItPlayerTarget(targetUniqueID))
        {
            if (damageTakenByAttacker == 0 && damageTakenByTarget == 0 && target.GetComponent<OneCardManager>().cardAsset.MonsterState == FieldPosition.Attack && manager.cardAsset.MonsterState == FieldPosition.Attack)
            {
                GameObject newPartEffect = GameObject.Instantiate(manager.attackEffect, target.transform.position, Quaternion.identity) as GameObject;
                if (newPartEffect.GetComponent<ParticleSystem>() != null)
                {
                    ParticleSystem ps = newPartEffect.GetComponent<ParticleSystem>();
                    newPartEffect.transform.position = new Vector3(newPartEffect.transform.position.x, newPartEffect.transform.position.y, newPartEffect.transform.position.z - 50);
                    ps.Play();
                    Destroy(newPartEffect, ps.main.duration);
                }
                else
                {
                    newPartEffect.transform.position = new Vector3(newPartEffect.transform.position.x + 1400f, newPartEffect.transform.position.y, newPartEffect.transform.position.z - 150);
                    Destroy(newPartEffect, 10f);
                }
                OpponentFightBack();
                transform.DOMove(transform.position, 1f).OnComplete(() =>
                {
                    GameObject newPartEffect1 = GameObject.Instantiate(target.GetComponent<OneMonsterManager>().attackEffect, transform.position, Quaternion.identity) as GameObject;
                    if (newPartEffect1.GetComponent<ParticleSystem>() != null)
                    {
                        ParticleSystem ps1 = newPartEffect1.GetComponent<ParticleSystem>();
                        newPartEffect1.transform.position = new Vector3(newPartEffect1.transform.position.x, newPartEffect1.transform.position.y, newPartEffect1.transform.position.z - 50);
                        ps1.Play();
                        Destroy(newPartEffect1, ps1.main.duration);
                    }
                    else
                    {
                        newPartEffect1.transform.position = new Vector3(newPartEffect1.transform.position.x, newPartEffect1.transform.position.y, 0);
                        Destroy(newPartEffect1, 10f);
                    }

                    transform.GetChild(6).GetComponent<Animator>().SetTrigger("Die");
                    target.transform.GetChild(6).GetComponent<Animator>().SetTrigger("Die");
                });
            }
        }

        // damageTakenByAttacker is higher than 0 only 
        // when attacking a defence position monster with more DEF than my ATK
        // or an attack position monster with more ATK than my ATK
        if (damageTakenByAttacker > 0)
        {
            if (target.GetComponent<OneCardManager>().cardAsset.MonsterState == FieldPosition.Attack && target.transform.childCount > 6)
            {
                OpponentFightBack();
                transform.DOMove(transform.position, 6f).OnComplete(() =>
                {
                    GameObject newPartEffect = GameObject.Instantiate(target.GetComponent<OneMonsterManager>().attackEffect, transform.position, Quaternion.identity) as GameObject;
                    if (newPartEffect.GetComponent<ParticleSystem>() != null)
                    {
                        ParticleSystem ps = newPartEffect.GetComponent<ParticleSystem>();
                        newPartEffect.transform.position = new Vector3(newPartEffect.transform.position.x, newPartEffect.transform.position.y, newPartEffect.transform.position.z - 50);
                        ps.Play();
                        Destroy(newPartEffect, ps.main.duration);
                    }
                    else
                    {
                        if (!isItPlayerTarget(targetUniqueID))
                            newPartEffect.transform.position = new Vector3(newPartEffect.transform.position.x + 1400f, newPartEffect.transform.position.y, 0);
                        Destroy(newPartEffect, 10f);
                    }
                    TurnManager.Instance.whoseTurn.PArea.Portrait.Model.GetComponent<Animator>().SetTrigger("GetDamaged");
                    if (!isItPlayerTarget(targetUniqueID))
                        if (target.GetComponent<OneMonsterManager>().cardAsset.MonsterState == FieldPosition.Attack)
                            transform.GetChild(6).GetComponent<Animator>().SetTrigger("Die");
                    transform.DOMove(transform.position, 0.5f).OnComplete(() =>
                    {
                        Transform t = TurnManager.Instance.whoseTurn.PArea.DamageSpot;
                        DamageEffect.CreateDamageEffect(t.position, damageTakenByAttacker);
                    });
                });
            }
            else
            {
                TurnManager.Instance.whoseTurn.PArea.Portrait.Model.GetComponent<Animator>().SetTrigger("GetDamaged");
                if (!isItPlayerTarget(targetUniqueID))
                    if (target.GetComponent<OneMonsterManager>().cardAsset.MonsterState == FieldPosition.Attack)
                        transform.GetChild(6).GetComponent<Animator>().SetTrigger("Die");
                transform.DOMove(transform.position, 0.5f).OnComplete(() =>
                {
                    Transform t = TurnManager.Instance.whoseTurn.PArea.DamageSpot;
                    DamageEffect.CreateDamageEffect(t.position, damageTakenByAttacker);
                });
            }
        }

        if (damageTakenByTarget > 0)
        {
            GameObject newPartEffect = GameObject.Instantiate(manager.attackEffect, target.transform.position, Quaternion.identity) as GameObject;
            if (newPartEffect.GetComponent<ParticleSystem>() != null)
            {
                ParticleSystem ps = newPartEffect.GetComponent<ParticleSystem>();
                newPartEffect.transform.position = new Vector3(newPartEffect.transform.position.x, newPartEffect.transform.position.y, newPartEffect.transform.position.z - 50);
                ps.Play();
                Destroy(newPartEffect, ps.main.duration);
            } else
            {
                if (!isItPlayerTarget(targetUniqueID))
                    newPartEffect.transform.position = new Vector3(newPartEffect.transform.position.x + 1400f, newPartEffect.transform.position.y, newPartEffect.transform.position.z - 150);
                Destroy(newPartEffect, 10f);
            }
            TurnManager.Instance.whoseTurn.otherPlayer.PArea.Portrait.Model.GetComponent<Animator>().SetTrigger("GetDamaged");
            if (!isItPlayerTarget(targetUniqueID) && target.transform.childCount > 6)
                target.transform.GetChild(6).GetComponent<Animator>().SetTrigger("Die");
            transform.DOMove(transform.position, 0.5f).OnComplete(() =>
            {
                Transform t = TurnManager.Instance.whoseTurn.otherPlayer.PArea.DamageSpot;
                DamageEffect.CreateDamageEffect(t.position, damageTakenByTarget);
            });
        } else if (!isItPlayerTarget(targetUniqueID))
        {
            if (target.GetComponent<OneCardManager>().cardAsset.MonsterState == FieldPosition.Defence && target.GetComponent<OneCardManager>().cardAsset.Defence < manager.cardAsset.Attack)
            {
                try
                {
                    target.transform.GetChild(6).GetComponent<Animator>().SetTrigger("Die");
                }
                catch
                {

                }
            }
            GameObject newPartEffect = GameObject.Instantiate(manager.attackEffect, target.transform.position, Quaternion.identity) as GameObject;
            if (newPartEffect.GetComponent<ParticleSystem>() != null)
            {
                ParticleSystem ps = newPartEffect.GetComponent<ParticleSystem>();
                newPartEffect.transform.position = new Vector3(newPartEffect.transform.position.x, newPartEffect.transform.position.y, newPartEffect.transform.position.z - 50);
                ps.Play();
                Destroy(newPartEffect, ps.main.duration);
            }
            else
            {
                newPartEffect.transform.position = new Vector3(newPartEffect.transform.position.x + 1400f, newPartEffect.transform.position.y, newPartEffect.transform.position.z - 150);
                Destroy(newPartEffect, 10f);
            }
        }


        w.SetTableSortingOrder();
        w.VisualState = tempState;

        Sequence s = DOTween.Sequence();
        s.AppendInterval(3f);
        s.OnComplete(Command.CommandExecutionComplete);
        if (manager.cardAsset.Type != MonsterType.Spellcaster || manager.cardAsset.Attribute != MonsterAttribute.Dark)
        {
            transform.GetChild(6).DOMove(pos, 0.5f).OnComplete(() =>
            {
                if (!isItPlayerTarget(targetUniqueID))
                    AddModelToTarget(damageTakenByTarget);
                return;
            });
        } else if (!isItPlayerTarget(targetUniqueID))
        {
            AddModelToTarget(damageTakenByTarget);
        }
        
    }

    public void OpponentFightBack()
    {
        OneMonsterManager targetManager = target.GetComponent<OneMonsterManager>();
        target.transform.GetChild(6).GetComponent<Animator>().SetTrigger("Attack");
        TurnManager.Instance.whoseTurn.otherPlayer.PArea.Portrait.Model.GetComponent<Animator>().SetTrigger("Attack");
        GameObject newPartEffect = GameObject.Instantiate(targetManager.loadEffect, target.transform.GetChild(6).transform.position, Quaternion.identity) as GameObject;
        ParticleSystem ps = newPartEffect.GetComponentInChildren<ParticleSystem>();
        ps.Play();
        Destroy(newPartEffect, 1);
        if (targetManager.cardAsset.Type == MonsterType.Spellcaster)
        {
            GameObject newShotEffect = GameObject.Instantiate(targetManager.shotEffect, target.transform.GetChild(6).transform.position, Quaternion.identity) as GameObject;
            for (int i = 0; i < newShotEffect.transform.childCount; i++)
                newShotEffect.transform.GetChild(i).GetComponent<ParticleSystem>().Play();
            newShotEffect.transform.DOMove(transform.position, 2).OnComplete(() =>
            {
                Destroy(newShotEffect);
                try
                {
                    transform.GetChild(6).GetComponent<Animator>().SetTrigger("Die");
                }
                catch
                {

                }
            });
        }
        else if (targetManager.cardAsset.Attribute == MonsterAttribute.Dark)
        {
            GameObject newShotEffect = GameObject.Instantiate(targetManager.shotEffect, target.transform.GetChild(6).transform.position, Quaternion.identity) as GameObject;
            newShotEffect.transform.DOMove(transform.position, 0.8f).OnComplete(() =>
            {
                Destroy(newShotEffect);
                try
                {
                    transform.GetChild(6).GetComponent<Animator>().SetTrigger("Die");
                }
                catch
                {

                }
            });
        }
        else
        {
            target.transform.GetChild(6).DOMove(transform.position, 2f).SetLoops(2, LoopType.Yoyo).OnComplete(() => {
                try
                {
                    transform.GetChild(6).GetComponent<Animator>().SetTrigger("Die");
                }
                catch
                {

                }
            });
        }
    }

    bool isItPlayerTarget(int ID)
    {
        return (ID == GlobalSettings.Instance.LowPlayer.PlayerID || ID == GlobalSettings.Instance.TopPlayer.PlayerID);
    }

    void AddModelToTarget(int damageTakenByTarget)
    {
        if (target.transform.childCount <= 6)
        {
            // if a target was attacked and still exists it has to have a model
            OneMonsterManager targetManager = target.GetComponent<OneMonsterManager>();
            GameObject model = GameObject.Instantiate(targetManager.model, targetManager.transform.position, Quaternion.identity) as GameObject;
            if (TurnManager.Instance.whoseTurn.otherPlayer.PArea.tableVisual.owner == AreaPosition.Top)
            {
                model.transform.rotation = new Quaternion(model.transform.rotation.x, 180.0f, model.transform.rotation.z, model.transform.rotation.w);
            }
            Vector3 modelPos = new Vector3(model.transform.localPosition.x + manager.transXOffset, model.transform.localPosition.y + manager.transYOffset, model.transform.localPosition.z + manager.transZOffset);
            model.transform.localPosition = modelPos;
            if (targetManager.cardAsset.MonsterState == FieldPosition.Defence)
                model.GetComponent<Animator>().SetTrigger("Defence_State");
            model.transform.SetParent(target.transform);
            if (targetManager.cardAsset.Defence < manager.cardAsset.Attack)
                model.GetComponent<Animator>().SetTrigger("Die");
        }
    }
}
