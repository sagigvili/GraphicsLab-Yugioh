using UnityEngine;
using System.Collections;

public class HeroPowerEffect : MonoBehaviour {

    public GameObject YugiPartEffect;
    public GameObject KaibaPartEffect;
    public GameObject ModelHalo;
    public void ActivateEffect(Player p)
    {
        TurnManager.Instance.whoseTurn.PArea.Portrait.Model.GetComponent<Animator>().SetTrigger("HeroPower");
        GameObject newHaloEffect = GameObject.Instantiate(ModelHalo, TurnManager.Instance.whoseTurn.PArea.Portrait.HealPoint.transform.position, Quaternion.identity) as GameObject;
        ParticleSystem ps1 = newHaloEffect.GetComponent<ParticleSystem>();
        ps1.Play();
        Destroy(newHaloEffect, 2.5f);
        if (p.PlayerID == 1) // Yugi Destroys all Kaiba's monsters
        {
            GameObject newPartEffect = GameObject.Instantiate(YugiPartEffect, YugiPartEffect.transform.position, Quaternion.identity) as GameObject;
            newPartEffect.transform.position = new Vector3(newPartEffect.transform.position.x - 500, newPartEffect.transform.position.y, newPartEffect.transform.position.z);
            ParticleSystem ps = newPartEffect.GetComponent<ParticleSystem>();
            ps.Play();
            Destroy(gameObject, ps.main.duration);
            MonsterLogic[] MonstersToDestory = TurnManager.Instance.whoseTurn.otherPlayer.table.MonstersOnTable.ToArray();
            foreach (MonsterLogic ml in MonstersToDestory)
            {
                ml.Die();
            }
                

        }
        else // Kaiba Destroys all Yugi's Spell/Trap cards
        {
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

    }
}
