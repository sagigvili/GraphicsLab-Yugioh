using UnityEngine;
using System.Collections;

public class HeroPowerEffect : MonoBehaviour {

    public GameObject partEffect;

    public void ActivateEffect(Player p)
    {
        if (p.PlayerID == 1) // Yugi Destroys all Kaiba's monsters
        {
            GameObject newPartEffect = GameObject.Instantiate(partEffect, partEffect.transform.position, Quaternion.identity) as GameObject;
            ParticleSystem ps = newPartEffect.GetComponent<ParticleSystem>();
            ps.Play();
            Destroy(gameObject, ps.main.duration);
            MonsterLogic[] MonstersToDestory = TurnManager.Instance.whoseTurn.otherPlayer.table.MonstersOnTable.ToArray();
            foreach (MonsterLogic ml in MonstersToDestory)
                ml.Die();
        }
        else // Kaiba Destroys all Yugi's Spell/Trap cards
        {
            GameObject newPartEffect = GameObject.Instantiate(partEffect, partEffect.transform.position, Quaternion.identity) as GameObject;
            newPartEffect.transform.position = new Vector3(partEffect.transform.position.x, partEffect.transform.position.y - 750, partEffect.transform.position.z);
            ParticleSystem ps = newPartEffect.GetComponent<ParticleSystem>();
            ps.Play();
            Destroy(gameObject, ps.main.duration);
            SpellTrapLogic[] SpellTrapsToDestory = TurnManager.Instance.whoseTurn.otherPlayer.table.SpellsTrapsOnTable.ToArray();
            foreach (SpellTrapLogic stl in SpellTrapsToDestory)
                stl.Die();
        }

    }
}
