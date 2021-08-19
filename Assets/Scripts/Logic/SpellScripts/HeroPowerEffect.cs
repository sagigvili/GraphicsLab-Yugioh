using UnityEngine;
using System.Collections;
using DG.Tweening;

public class HeroPowerEffect : MonoBehaviour {

    public GameObject YugiPartEffect;
    public GameObject KaibaPartEffect;
    public GameObject ModelHalo;
    public Snow snow;
    public GameObject weather_system;
    public Light MonstersSnowLayer;
    public void ActivateEffect(Player p)
    {

        if (p.PlayerID == 1) // Yugi Destroys all Kaiba's monsters
        {
            TurnManager.Instance.whoseTurn.PArea.Portrait.Model.GetComponent<Animator>().SetTrigger("HeroPower");
            Vector3 pos = new Vector3(TurnManager.Instance.whoseTurn.PArea.Portrait.HealPoint.transform.position.x - 700, TurnManager.Instance.whoseTurn.PArea.Portrait.HealPoint.transform.position.y - 200, 600);
            GameObject newHaloEffect = GameObject.Instantiate(ModelHalo, pos, Quaternion.identity) as GameObject;
            for (int i = 0; i< newHaloEffect.transform.childCount; i++)
            {
                ParticleSystem ps1 = newHaloEffect.transform.GetChild(i).GetComponent<ParticleSystem>();
                ps1.Play();
            }
            LSystemLightning l = gameObject.AddComponent<LSystemLightning>();
            l.Init_system(snow, weather_system);
            MonstersSnowLayer.gameObject.SetActive(true);
            StartCoroutine(IncreaseLightIntensity());
            new DelayCommand(20f).AddToQueue();
            transform.DOMove(transform.position, 3f).OnComplete(() => {
                l.create_flake();
            });
            transform.DOMove(transform.position, 6f).OnComplete(() => {
                l.create_flake();
            });
            transform.DOMove(transform.position, 9f).OnComplete(() => {
                l.create_flake();
            });
            transform.DOMove(transform.position, 12f).OnComplete(() => {
                l.create_flake();
            });
            transform.DOMove(transform.position, 16f).OnComplete(() => {
                l.create_flake();
            });
            transform.DOMove(transform.position, 20f).OnComplete(() => {
                l.abort();
                Destroy(newHaloEffect);
                TurnManager.Instance.whoseTurn.PArea.Portrait.Model.GetComponent<Animator>().SetTrigger("Idle");
                transform.DOMove(transform.position, 3f).OnComplete(() => {
                    MonsterLogic[] MonstersToDestory = TurnManager.Instance.whoseTurn.otherPlayer.table.MonstersOnTable.ToArray();
                    foreach (MonsterLogic ml in MonstersToDestory)
                    {
                        ml.Die();
                    }
                    MonstersSnowLayer.gameObject.SetActive(false);
                });
            });  

        }
        else // Kaiba Destroys all Yugi's Spell/Trap cards
        {
            TurnManager.Instance.whoseTurn.PArea.Portrait.Model.GetComponent<Animator>().SetTrigger("HeroPower");
            GameObject newHaloEffect = GameObject.Instantiate(ModelHalo, TurnManager.Instance.whoseTurn.PArea.Portrait.HealPoint.transform.position, Quaternion.identity) as GameObject;
            ParticleSystem ps1 = newHaloEffect.GetComponent<ParticleSystem>();
            ps1.Play();
            Destroy(newHaloEffect, 2.5f);
            SpellTrapLogic[] SpellTrapsToDestory = TurnManager.Instance.whoseTurn.otherPlayer.table.SpellsTrapsOnTable.ToArray();

            foreach (SpellTrapLogic stl in SpellTrapsToDestory)
            {
                GameObject spellTrapToRemove = IDHolder.GetGameObjectWithID(stl.ID);
                GameObject newPartEffect = GameObject.Instantiate(KaibaPartEffect, spellTrapToRemove.transform.position, Quaternion.identity) as GameObject;

                ParticleSystem ps = newPartEffect.GetComponent<ParticleSystem>();
                ps.Play();
                Destroy(newPartEffect, 3.0f);
                stl.Die();
            }
            new DelayCommand(1.5f).AddToQueue();
        }

        IEnumerator IncreaseLightIntensity()
        {
            for (float i = 0; i < 19; i++)
            {
                yield return new WaitForSeconds(1f);
                MonstersSnowLayer.intensity += 1;
            }
        }

    }
}
