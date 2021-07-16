using UnityEngine;
using System.Collections;

public class SpellTrapEffect
{
    static public void ActivateEffect(CardAsset ca)
    {
        switch (ca.Effect)
        {
            case Effects.DestoryMonster:
                GameObject tsDM = GameObject.Instantiate(GlobalSettings.Instance.TargetSelectorPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                tsDM.GetComponent<EffectOperator>().InitiateEffect(true, Effects.DestoryMonster);
                tsDM.SetActive(true);
                break;
            case Effects.DestorySpellTrap:
                GameObject tsDST = GameObject.Instantiate(GlobalSettings.Instance.TargetSelectorPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                tsDST.GetComponent<EffectOperator>().InitiateEffect(false, Effects.DestorySpellTrap);
                tsDST.SetActive(true);
                break;
            case Effects.DirectAttack:
                int hp_amount = System.Int32.Parse(TurnManager.Instance.whoseTurn.otherPlayer.PArea.Portrait.HealthText.text) - ca.amount;
                if (hp_amount < 0)
                    hp_amount = 0;
                string final_hp_other_amount = hp_amount.ToString();
                TurnManager.Instance.whoseTurn.otherPlayer.PArea.Portrait.HealthText.text = final_hp_other_amount;
                TurnManager.Instance.whoseTurn.otherPlayer.Health -= ca.amount;
                break;
            case Effects.Draw:
                TurnManager.Instance.whoseTurn.DrawACard();
                TurnManager.Instance.whoseTurn.DrawACard();
                break;
            case Effects.Heal:
                string final_hp_amount = (System.Int32.Parse(TurnManager.Instance.whoseTurn.PArea.Portrait.HealthText.text) + ca.amount).ToString();
                TurnManager.Instance.whoseTurn.PArea.Portrait.HealthText.text = final_hp_amount;
                TurnManager.Instance.whoseTurn.Health += ca.amount;
                break;
            case Effects.Negate:
                break;
            case Effects.ChangeToAttack:
                GameObject tsPCA = GameObject.Instantiate(GlobalSettings.Instance.TargetSelectorPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                tsPCA.GetComponent<EffectOperator>().InitiateEffect(true, Effects.ChangeToAttack);
                tsPCA.SetActive(true);
                break;
            case Effects.ChangeToDefence:
                GameObject tsPCB = GameObject.Instantiate(GlobalSettings.Instance.TargetSelectorPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                tsPCB.GetComponent<EffectOperator>().InitiateEffect(true, Effects.ChangeToDefence);
                tsPCB.SetActive(true);
                break;
            case Effects.Revive:
                break;

        }
    }

}


